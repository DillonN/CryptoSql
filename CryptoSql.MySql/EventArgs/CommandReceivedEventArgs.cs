using CryptoSql.Common.Models.Commands;

namespace CryptoSql.MySql.EventArgs
{
    public class CommandReceivedEventArgs : System.EventArgs
    {
        public readonly Command Command;
        public readonly int ThreadID;

        public CommandReceivedEventArgs(Command command, int threadID)
        {
            Command = command;
            ThreadID = threadID;
        }
    }
}
