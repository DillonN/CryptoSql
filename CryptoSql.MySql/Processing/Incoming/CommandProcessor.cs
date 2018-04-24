using CryptoSql.Common.Enums;
using CryptoSql.Common.Models;
using CryptoSql.Common.Models.Commands;
using CryptoSql.MySql.Packet.Decoded.CommandPackets;
using CryptoSql.MySql.Packet.Encoded;
using CryptoSql.MySql.Packet.Encoded.Interfaces;
using CryptoSql.MySql.Packet.Encoded.ResultsSet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoSql.MySql.Processing.Incoming {
    internal class CommandProcessor
    {
        public const string VersionField = "@@version_comment";

        public Func<Command, Stopwatch, Task<CommandResponse>> CommandReceivedAsync;

        internal async Task<IServerResponse> GetResponse(CommandPacket packet, ServerInfo info, Stopwatch timer)
        {
            Command command = null;

            switch (packet.CommandType)
            {
                case CommandType.Query:
                    var query = new QueryCommand(packet.Payload);
                    if (query.Columns?.Keys?.FirstOrDefault() == VersionField)
                    {
                        var colInfo = new Dictionary<ColumnInfo, object[]>
                        {
                            {
                                new ColumnInfo(VersionField, "", typeof(string)), 
                                new object[]
                                {
                                    info.VersionString
                                }
                            }
                        };
                        return new ResultsSetPacket(colInfo);
                    }

                    command = query;
                    break;

                case CommandType.Database:
                    command = new DatabaseCommand(packet.Payload);
                    break;
            }

            if (command != null && CommandReceivedAsync != null)
            {
                return await GetBridgeResponse(command, timer);
            }

            throw new ArgumentOutOfRangeException("Cannot handle command");
        }

        internal async Task<IServerResponse> GetBridgeResponse(Command command, Stopwatch timer)
        {
            var resp = await CommandReceivedAsync(command, timer);
            switch (resp.Type)
            {
                case CommandResponseType.OK:
                    return new OKPacket();
                case CommandResponseType.Error:
                    // TODO error
                    return new OKPacket();
                case CommandResponseType.Query:
                    if (resp.Results == null)
                    {
                        throw new Exception($"{nameof(CommandReceivedAsync)} did not return a valid {nameof(CommandResponse)} object");
                    }
                    return new ResultsSetPacket(resp.Results);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
