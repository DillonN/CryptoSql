using System;
using System.Collections.Generic;
using CryptoSql.Common.Enums;

namespace CryptoSql.Common.Models
{
    public class CommandResponse
    {
        public readonly CommandResponseType Type;

        private Dictionary<ColumnInfo, object[]> _results;
        public Dictionary<ColumnInfo, object[]> Results
        {
            get
            {
                if (Type != CommandResponseType.Query)
                    throw new InvalidOperationException("No results for non-query response");
                return _results;
            }
            set
            {
                if (value != null && Type != CommandResponseType.Query)
                {
                    _results = null;
                    throw new ArgumentException("Cannot have results without query!");
                }

                _results = value;
            }
        }

        public CommandResponse(CommandResponseType type, Dictionary<ColumnInfo, object[]> results = null)
        {
            Type = type;
            Results = results;

            Results = results;
        }
    }
}
