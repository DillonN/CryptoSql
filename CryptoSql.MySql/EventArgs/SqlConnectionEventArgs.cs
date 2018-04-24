namespace CryptoSql.MySql.EventArgs
{
    internal class SqlConnectionClosedEventArgs : System.EventArgs
    {
        public uint ThreadID;

        public SqlConnectionClosedEventArgs(uint id)
        {
            ThreadID = id;
        }
    }
}
