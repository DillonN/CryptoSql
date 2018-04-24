using CryptoSql.Common.Enums;
using CryptoSql.Common.Extensions;
using CryptoSql.MySql.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CryptoSql.Common.Models.Commands
{
    public class QueryCommand : Command
    {
        public Query Query { get; private set; }
        public Dictionary<string, string> Columns { get; private set; }
        public string Database { get; private set; }
        public string Table { get; private set; }
        public bool IfNotExists { get; private set; }
        public Dictionary<string, string> Where { get; private set; }
        public List<Dictionary<string, string>> MultiSet { get; private set; }

        public int Limit { get; private set; }

        private string _columnDefinitions;

        public QueryCommand(string data) :
            base(CommandType.Query)
        {
            ParseCommand(data);
        }

        private void ParseCommand(string data)
        {
            // TODO we would like to have type casting here instead of downstream (e.g. on insert values)

            var args = data.Split(new[] {' ', '\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);

            if (args.Length < 1) 
                throw new InvalidOperationException("Command string requires 1 arg");

            switch (args[0].ToLower())
            {
                case "select":
                    Query = Query.Select;
                    break;
                case "commit":
                    Query = Query.Commit;
                    break;
                case "create":
                    Query = Query.Create;
                    break;
                case "update":
                    Query = Query.Update;
                    break;
                case "insert":
                    Query = Query.Insert;
                    break;
                case "delete":
                    Query = Query.Delete;
                    break;
                default:
                    throw new InvalidOperationException($"Do not know about query command {args[0]}");
            }

            if (args.Length >= 2)
            {
                var target = args[1];

                if (Query == Query.Select)
                {
                    Columns = new Dictionary<string, string>();
                    for (var i = 1; i < args.Length; i++)
                    {                        
                        Columns[args[i].Trim(' ', ',', '\n', '\r')] = null;
                        if (!args[i].EndsWith(",")) break;
                    }
                }
                else if (Query == Query.Create && args.Length >= 3)
                {
                    if (target.ToLower() == "database")
                    {
                        Database = args[2];
                    }
                    else if (target.ToLower() == "table")
                    {
                        if (args.Length >= 5 && 
                            args[2].ToLower() == "if" &&
                            args[3].ToLower() == "not" &&
                            args[4].ToLower() == "exists")
                        {
                            IfNotExists = true;
                            Table = args[5];
                        }
                        else
                        {
                            Table = args[2];
                        }

                        _columnDefinitions = data.Substring("(", ")");
                    }
                }
                else if (Query == Query.Update)
                {
                    Table = target;
                }
                else if (Query == Query.Insert)
                {
                    // This needs to be fixed to support brackets and commas in strings

                    Debug.Assert(target.ToLower() == "into");
                    Table = args[2];
                    
                    var columns = data.Substring("(", ")", false).Split(',');

                    var values = data.Substring("values", "", true, false, true).Split(',');

                    if (values.Count(v => v.Contains(')')) <= 1)
                    {
                        if (Columns == null) Columns = new Dictionary<string, string>();

                        for (var i = 0; i < values.Length; i++)
                        {
                            Columns[columns[i].Trim()] = values[i].Trim(' ', '\'', '"', '(', ')');
                        }
                    }
                    else
                    {
                        // Multiple values are defined as tuples, so include all of them
                        // TODO make this more efficient with deffered exe
                        MultiSet = new List<Dictionary<string, string>>();
                        
                        var dict = new Dictionary<string, string>();
                        for (var i = 0; i < values.Length; i++)
                        {
                            dict[columns[i % columns.Length].Trim()] = values[i].Trim('\r', '\n', ' ', '\'', '"', '(', ')');
                            if (values[i].EndsWith(")"))
                            {
                                // Last entry in this tuple
                                MultiSet.Add(dict);
                                dict = new Dictionary<string, string>();
                            }
                        }
                    }
                }
                else if (Query == Query.Delete)
                {
                    Table = args[2];
                }

                var afterEquals = false;
                var last = "";
                var func = QueryFunctions.None;
                for (var i = 0; i < args.Length; i++)
                {
                    switch (args[i].ToLower())
                    {
                        case "limit":
                            func = QueryFunctions.Limit;
                            continue;
                        case "from":
                            func = QueryFunctions.From;
                            continue;
                        case "where":
                            func = QueryFunctions.Where;
                            continue;
                        case "set":
                            func = QueryFunctions.Set;
                            continue;
                        default:
                            if (func == QueryFunctions.None) continue;
                            break;
                    }
                    switch (func)
                    {
                        case QueryFunctions.Limit:
                            if (int.TryParse(args[i], out var limit))
                                Limit = limit;
                            continue;
                        case QueryFunctions.From:
                            Table = args[i];
                            continue;
                        case QueryFunctions.Where:
                        case QueryFunctions.Set:
                            if (args[i] == "=")
                            {
                                afterEquals = true;
                            }
                            else if (afterEquals)
                            {
                                if (func == QueryFunctions.Where)
                                {
                                    if (Where == null) Where = new Dictionary<string, string>();
                                    Where[last] = args[i].Trim(',', '"', '\'');
                                }
                                else
                                {
                                    if (Columns == null) Columns = new Dictionary<string, string>();
                                    Columns[last] = args[i].Trim(',', '"', '\'');
                                }

                                afterEquals = false;
                            }
                            else
                            {
                                last = args[i].Trim('"', '\'');
                            }

                            continue;
                    }
                }
            }
        }

        public IEnumerable<ColumnInfo> ColumnDefinitions()
        {
            var defs = _columnDefinitions.Split(',');
            foreach (var def in defs)
            {
                if (def.ToLower().TrimStart().StartsWith("primary"))
                    continue;  //TODO

                var args = def.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                if (args.Length < 2)
                    throw new Exception("Column definitions invalid");
                var name = args[0].TrimStart();
                Type type;

                if (args[1].ToLower().Contains("int"))
                {
                    type = typeof(int);
                }
                else if (args[1].ToLower().Contains("varchar"))
                {
                    type = typeof(string);
                }
                else if (args[1].ToLower().Contains("date"))
                {
                    type = typeof(DateTime);
                }
                else
                {
                    throw new Exception("Column definitions invalid");
                }

                yield return new ColumnInfo(name, Table, type);
            }
        }
    }
}
