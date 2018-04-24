using CryptoSql.Common.Enums;

namespace CryptoSql.Common.Models.Commands
{
    public class Command
    {
        public CommandType Type;

        public Command(CommandType type)
        {
            Type = type;
        }
    }
}
