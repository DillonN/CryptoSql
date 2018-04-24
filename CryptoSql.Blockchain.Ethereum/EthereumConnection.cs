using CryptoSql.Blockchain.Ethereum.Models;
using CryptoSql.Common.Extensions;
using CryptoSql.Common.Models;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("CryptoSql.Blockchain.Ethereum.Tests")]

namespace CryptoSql.Blockchain.Ethereum
{
    public class EthereumConnection
    {
        private const ulong StartingTransactionPrice = 21000;
        public const uint MaxGasLimit = 32768;

        private readonly Web3 _web3;
        private readonly Account _account;

        public uint GasLimit;

        public static double LastGasPrice { get; private set; }

        internal EthereumConnection(Uri rpcAddress, string keyStore, string password, uint gasLimit = 180000)
        {
            _account = Account.LoadFromKeyStore(keyStore, password);
            var rpc = new RpcClient(rpcAddress);
            _web3 = new Web3(_account, rpc);
            GasLimit = gasLimit;
        }

        #region Nethereum methods

        internal async Task<byte[]> GetTransactionData(string address)
        {
            var trans = await _web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(address);
            return trans?.Input.ToByteArray();
        }

        internal async Task<TransactionInfo> PostNewTransaction(byte[] data,  double? price = null, bool overrideMaxGas = false)
        {
            var gasToRun = EstimateGasCost(data);
            if (!overrideMaxGas && gasToRun > MaxGasLimit)
                throw new InvalidOperationException(
                    $"Transaction requires too much gas ({gasToRun}, max {MaxGasLimit}). Run with {nameof(overrideMaxGas)} parameter set to true to force transaction to run.");

            var gas = new HexBigInteger(GasLimit);
            var value = new HexBigInteger(0);
            var strData = data.ToHex(true);
            var input = new TransactionInput(strData, _account.Address, _account.Address, gas, value);

            if (price != null && price > 0)
            {
                input.GasPrice = new HexBigInteger(new BigInteger(price.Value));
            }

            //var hash = await _web3.TransactionManager.SendTransactionAsync(input);
            var receipt = await _web3
                .TransactionManager
                .TransactionReceiptService
                .SendRequestAndWaitForReceiptAsync(input);

            //if (!await AwaitTransactionMined(receipt.TransactionHash, timeout, data))
            //    throw new TimeoutException("Transaction not mined in timeout");
            var cost = new EthCostInfo((double) receipt.GasUsed.Value, (double) input.GasPrice.Value);
            var info = new TransactionInfo(receipt.TransactionHash, cost, receipt.Status.Value == 1);

            LastGasPrice = (double) input.GasPrice.Value;

            return info;
        }

        internal static ulong EstimateGasCost(byte[] data)
        {
            var gas = StartingTransactionPrice;
            gas += (ulong) data.Count(b => b == 0) * 4;
            gas += (ulong) data.Count(b => b != 0) * 68;
            return gas;
        }

        private async Task<bool> AwaitTransactionMined(string hash, int timeout, byte[] data)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < timeout)
            {
                var bytes = await GetTransactionData(hash);
                if (bytes.SequenceEqual(data))
                {
                    stopwatch.Stop();
                    return true;
                }

                await Task.Delay(1000);
            }

            stopwatch.Stop();

            return false;
        }

        #endregion
    }
}
