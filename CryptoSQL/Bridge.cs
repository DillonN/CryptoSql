using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoSql.Common.Enums;
using CryptoSql.Common.Interfaces;
using CryptoSql.Common.Models;
using CryptoSql.Common.Models.Commands;
using CryptoSql.Config;

namespace CryptoSql
{
    /// <summary>
    /// Serves as the main bridge point between blockchain/sql implementations
    /// </summary>
    internal class Bridge
    {
        private readonly List<IServerDaemon> _daemons;
        private string _currentDB;

        public readonly Dictionary<ProviderType, IStorageProvider> Providers;
        public ServerInfo Info;

        public Bridge(ServerInfo info)
        {
            Info = info;
            _daemons = new List<IServerDaemon>();
            Providers = new Dictionary<ProviderType, IStorageProvider>();
        }

        public void AddDaemon(IServerDaemon daemon)
        {
            _daemons.Add(daemon);
            daemon.CommandReceivedAsync = CommandReceivedAsync;
        }

        public void StartAllDaemons()
        {
            foreach (var daemon in _daemons)
            {
                daemon.Start(Info);
            }
        }

        private async Task<CommandResponse> CommandReceivedAsync(Command command, Stopwatch timer)
        {
            try
            {
                if (command is DatabaseCommand db)
                {
                    return await UpdateDatabase(db);
                }

                if (command is QueryCommand query)
                {
                    switch (query.Query)
                    {
                        case Query.Select:
                            return await SelectQuery(query);
                        case Query.Commit:
                            return await CommitQuery();
                        case Query.Create:
                            return await CreateQuery(query);
                        case Query.Update:
                            return await UpdateQuery(query);
                        case Query.Insert:
                            return await InsertQuery(query);
                        case Query.Delete:
                            return await DeleteQuery(query);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine($"Command {command.Type} failed in {timer?.ElapsedMilliseconds}ms");
                return new CommandResponse(CommandResponseType.Error);
            }

            Console.WriteLine($"Command {command.Type} completed in {timer?.ElapsedMilliseconds}ms");
            return new CommandResponse(CommandResponseType.Error);
        }

        private async Task<CommandResponse> UpdateDatabase (DatabaseCommand dbCommand)
        {
            var found = false;
            var notFound = new List<ProviderType>();

            foreach (var type in Providers.Keys)
            {
                if (!ConfigManager.Config.DatabaseAddressed.TryGetValue(type, out var addresses) || addresses == null)
                {
                    addresses = new Dictionary<string, string>();
                    ConfigManager.Config.DatabaseAddressed[type] = addresses;
                }
                else if (addresses.TryGetValue(dbCommand.Database, out var address))
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    if (!await Providers[type].UseAddress(address))
                    {
                        stopwatch.Stop();
                        throw new Exception("Address present but no backing found");
                    }

                    stopwatch.Stop();
                    Console.WriteLine($"Provider {type} retrieved info in {stopwatch.Elapsed.TotalSeconds}s");

                    found = true;
                    continue;
                }
                
                notFound.Add(type);
            }

            if (!found) throw new ArgumentException("Database not found");

            foreach (var type in notFound)
            {
                await Providers[type].Reset();
            }

            _currentDB = dbCommand.Database;
            return new CommandResponse(CommandResponseType.OK);
        }

        private async Task<CommandResponse> SelectQuery(QueryCommand qc)
        {
            if (qc.Columns.Keys.First().ToLower() == "database()")
            {
                var info = new ColumnInfo("DATABASE()", "", typeof(string));
                var dict = new Dictionary<ColumnInfo, object[]>
                {
                    [info] = new object[] {_currentDB}
                };
                return new CommandResponse(CommandResponseType.Query, dict);
            }
            var results = new Dictionary<ColumnInfo, object[]>();
            foreach (var provider in Providers.Values)
            {
                // Just overwrite the results so the timings go through for all
                results = provider.SelectRowsAsync(qc.Table, qc.Columns);
            }

            return new CommandResponse(CommandResponseType.Query, results);
        }

        private async Task<CommandResponse> CommitQuery()
        {
            var sb = new StringBuilder("Committing...");
            sb.AppendLine();

            foreach (var type in Providers.Keys)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var newAdr = await Providers[type].CommitAsync();
                stopwatch.Stop();
                AssureAddressDictExists(type);

                ConfigManager.Config.DatabaseAddressed[type][_currentDB] = newAdr.Hash;
                sb.AppendLine($"Provider {type} committed with address {newAdr.Hash}. Cost {newAdr.Cost}, time {stopwatch.Elapsed.TotalSeconds}s");
            }
            ConfigManager.SaveConfig();

            Console.WriteLine(sb.ToString());
            return new CommandResponse(CommandResponseType.OK);
        }

        private async Task<CommandResponse> CreateQuery(QueryCommand qc)
        {
            foreach (var type in Providers.Keys)
            {
                if (!string.IsNullOrWhiteSpace(qc.Database))
                {
                    AssureAddressDictExists(type);
                    if (ConfigManager.Config.DatabaseAddressed[type].ContainsKey(qc.Database))
                    {
                        throw new InvalidOperationException("Cannot create database that already exists!");
                    }

                    await Providers[type].Reset();
                    ConfigManager.Config.DatabaseAddressed[type][qc.Database] = null;
                }
                else if (!string.IsNullOrWhiteSpace(qc.Table))
                {
                    Providers[type].NewTableAsync(qc.Table, qc.ColumnDefinitions());
                }
            }

            return new CommandResponse(CommandResponseType.OK);
        }

        private async Task<CommandResponse> UpdateQuery(QueryCommand qc)
        {
            foreach (var provider in Providers.Values)
            {
                provider.UpdateRowAsync(qc.Table, qc.Columns);
            }

            return new CommandResponse(CommandResponseType.OK);
        }

        private async Task<CommandResponse> InsertQuery(QueryCommand qc)
        {
            foreach (var provider in Providers.Values)
            {
                if (qc.MultiSet != null)
                {
                    provider.UpdateRowsAsync(qc.Table, qc.MultiSet);
                }
                else
                {
                    provider.InsertRow(qc.Table, qc.Columns);
                }
            }

            return new CommandResponse(CommandResponseType.OK);
        }

        private async Task<CommandResponse> DeleteQuery(QueryCommand qc)
        {
            foreach (var provider in Providers.Values)
            {
                provider.DropRowsAsync(qc.Table, null);
            }

            return new CommandResponse(CommandResponseType.OK);
        }

        private static void AssureAddressDictExists(ProviderType type)
        {
            if (!ConfigManager.Config.DatabaseAddressed.ContainsKey(type) ||
                ConfigManager.Config.DatabaseAddressed[type] == null)
            {
                ConfigManager.Config.DatabaseAddressed[type] = new Dictionary<string, string>();
            }
        }
    }
}
