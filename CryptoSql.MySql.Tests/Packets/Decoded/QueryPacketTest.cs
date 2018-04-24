using CryptoSql.Common.Enums;
using CryptoSql.Common.Extensions;
using CryptoSql.Common.Models;
using CryptoSql.Common.Models.Commands;
using CryptoSql.MySql.Packet.Decoded.CommandPackets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace CryptoSql.MySql.Tests.Packets.Decoded
{
    [TestClass]
    public class QueryPacketTest
    {
        private const string TableDef =
            @"CREATE TABLE IF NOT EXISTS tasks (
task_id INT(11) NOT NULL AUTO_INCREMENT,
subject VARCHAR(45) DEFAULT NULL,
start_date DATE DEFAULT NULL,
end_date DATE DEFAULT NULL,
description VARCHAR(200) DEFAULT NULL,
PRIMARY KEY (task_id)
)";

        private const string DBDef = "CREATE DATABASE test";

        private const string Commit = "commit";

        private const string UpdateQuery = @"UPDATE test 
SET tutorial_title = 'Blarg'
WHERE tutorial_id = 3";

        private const string InsertQuery =
            @"INSERT INTO Customers (CustomerName, ContactName, Address)
VALUES ('Cardinal', 'Tom B. Erichsen', 'Skagen 21')";

        private const string MultiInsertQuery = @"INSERT INTO Customers (CustomerName, ContactName, Address)
VALUES ('Cardinal', 'Tom B. Erichsen', 'Skagen 21'),
('Blarg', 'Honk', 'test'),
('Sura', 'Late', 'hanh')";

        [TestMethod]
        public void QueryPacket_ShouldMatch()
        {
            var data = "210000000373656c65637420404076657273696f6e5f636f6d6d656e74206c696d69742031".ToByteArray();
            var packet = new CommandPacket(data);

            Assert.AreEqual(CommandType.Query, packet.CommandType);
            var query = new QueryCommand(packet.Payload);
            Assert.AreEqual(Query.Select, query.Query);
            Assert.AreEqual("@@version_comment", query.Columns.Keys.First());
            Assert.AreEqual(1, query.Limit);
        }

        [TestMethod]
        public void CreateDBQuery_ShouldMatch()
        {
            var query = new QueryCommand(DBDef);
            Assert.AreEqual(Query.Create, query.Query);
            Assert.AreEqual("test", query.Database);
        }

        [TestMethod]
        public void CommitQuery_ShouldMatch()
        {
            var query = new QueryCommand(Commit);
            Assert.AreEqual(Query.Commit, query.Query);
        }

        [TestMethod]
        public void CreateTableQuery_ShouldMatch()
        {
            var query = new QueryCommand(TableDef);
            var defs = query.ColumnDefinitions().ToList();

            Assert.AreEqual(Query.Create, query.Query);
            Assert.IsTrue(query.IfNotExists);

            Assert.AreEqual(new ColumnInfo("task_id", "tasks", typeof(int)), defs[0]);
            Assert.AreEqual(new ColumnInfo("subject", "tasks", typeof(string)), defs[1]);
            Assert.AreEqual(new ColumnInfo("start_date", "tasks", typeof(DateTime)), defs[2]);
            Assert.AreEqual(new ColumnInfo("end_date", "tasks", typeof(DateTime)), defs[3]);
            Assert.AreEqual(new ColumnInfo("description", "tasks", typeof(string)), defs[4]);
        }

        [TestMethod]
        public void UpdateQuery_ShouldMatch()
        {
            var query = new QueryCommand(UpdateQuery);

            Assert.AreEqual(Query.Update, query.Query);
            Assert.AreEqual("test", query.Table);
            Assert.AreEqual("Blarg", query.Columns["tutorial_title"]);
            Assert.AreEqual("3", query.Where["tutorial_id"]);
        }

        [TestMethod]
        public void InsertQuery_ShouldMatch()
        {
            var query = new QueryCommand(InsertQuery);
            Assert.AreEqual(Query.Insert, query.Query);
            Assert.AreEqual("Customers", query.Table);

            Assert.AreEqual("Cardinal", query.Columns["CustomerName"]);
            Assert.AreEqual("Tom B. Erichsen", query.Columns["ContactName"]);
            Assert.AreEqual("Skagen 21", query.Columns["Address"]);
        }

        [TestMethod]
        public void MultiInsertQuery_ShouldMatch()
        {
            var query = new QueryCommand(MultiInsertQuery);
            Assert.AreEqual(Query.Insert, query.Query);
            Assert.AreEqual("Customers", query.Table);

            var l1 = query.MultiSet[0];
            Assert.AreEqual("Cardinal", l1["CustomerName"]);
            Assert.AreEqual("Tom B. Erichsen", l1["ContactName"]);
            Assert.AreEqual("Skagen 21", l1["Address"]);

            var l2 = query.MultiSet[1];
            Assert.AreEqual("Blarg", l2["CustomerName"]);
            Assert.AreEqual("Honk", l2["ContactName"]);
            Assert.AreEqual("test", l2["Address"]);

            var l3 = query.MultiSet[2];
            Assert.AreEqual("Sura", l3["CustomerName"]);
            Assert.AreEqual("Late", l3["ContactName"]);
            Assert.AreEqual("hanh", l3["Address"]);
        }
    }
}