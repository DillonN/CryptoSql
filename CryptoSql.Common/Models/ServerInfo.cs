using System;
using System.Net;

namespace CryptoSql.Common.Models {
    [Serializable]
    public class ServerInfo
    {
        public string VersionString { get; set; }
        public int Port { get; set; }
        public IPAddress Address { get; set; }
        public int MaxConnections { get; set; }
        public bool ReportTimings { get; set; }
    }
}
