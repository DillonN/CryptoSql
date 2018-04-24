using System;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace CryptoSql.Blockchain.Ethereum.StorageProviders.Transaction.Monolithic
{
    internal static class DataSetConverter
    {
        // With compression we can save significant space
        // Takes more time, but since we are limited to ~100kb this is important
        public static byte[] ToBytes(DataSet ds, bool compression = true)
        {
            using (var memory = new MemoryStream())
            {
                var formatter = new BinaryFormatter();

                if (!compression)
                {
                    formatter.Serialize(memory, ds);
                }
                else
                {
                    using (var gzip = new GZipStream(memory, CompressionMode.Compress, true))
                    {
                        formatter.Serialize(gzip, ds);
                    }
                }

                return memory.ToArray();
            }
        }

        public static DataSet FromBytes(byte[] bytes, bool compression = true)
        {
            object ds;
            using (var memory = new MemoryStream(bytes))
            {
                var formatter = new BinaryFormatter();

                if (!compression)
                {
                    ds = formatter.Deserialize(memory);
                }
                else
                {
                    using (var gzip = new GZipStream(memory, CompressionMode.Decompress, true))
                    {
                        ds = formatter.Deserialize(gzip);
                    }
                }
            }

            return ds as DataSet ?? throw new ArgumentException();
        }
    }
}
