using CryptoSql.Common.Enums;

namespace CryptoSql.Common.Models.Commands
{
    public class DatabaseCommand : Command
    {
        public string Database;

        public DatabaseCommand(string database) :
            base(CommandType.Database)
        {
            Database = database;
        }
    }
}
