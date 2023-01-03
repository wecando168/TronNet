using System;
using System.Diagnostics.SymbolStore;
using System.Net;
using System.Threading.Tasks;
using Google.Api;
using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StowayNet;
using TronNet;
using TronNet.Crypto;
using TronNet.Protocol;
using TronNet.Test;
using static TronNet.Protocol.Transaction.Types;

namespace ConsoleClient
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            //const string tronGridApiKey = "ed8977d6-19e4-4507-a262-e2c36bf60c9e";
            //const string accessKey = "ed8977d6-19e4-4507-a262-e2c36bf60c9e";
            //const string secretKey = "ed8977d6-19e4-4507-a262-e2c36bf60c9e";

            //const string base58CheckMainPublicAddress = "TMzReQzPzk1uCLfR1jRJDuK3qF7CGQbEKZ";
            //const string base58CheckTestPublicAddressA = "TAef4SWpzGpeFLvLSMeEsR1sSadBB3cuGC";
            //const string base58CheckTestPublicAddressB = "TEaL2LvMDdkCbfiDK8gJon9VizGk7EgRqu";

            //const string base58MainNetUsdtContractAddress = "TR7NHqjeKQxGTCi8q8ZY4pL8otSzgjLj6t";
            //const string base58NileNetUsdtContractAddress = "TXLAQ63Xg1NAzckPwKHvzw7CSEmLMEqcdj";
            //const bool base58checkVisible = true;

            //const string hexMainPublicAddress = "4183dc7ae79526d33236dcfebdc443b99005092c5e";
            //const string hexTestPublicAddressA = "410775f9923e8ecf8e98d24a9cfe4afbbda4584b82";
            //const string hexTestPublicAddressB = "413284ec7124fbac27da15417f85a5dece09a87e61";

            //const string hexMainNetUsdtContractAddress = "41a614f803b6fd780986a42c78ec9c7f77e6ded13c";
            //const string hexNileNetUsdtContractAddress = "41ea51342dabbb928ae1e576bd39eff8aaf070a8c6";
            //const bool hexVisible = false;

            //const string hexMainTransactionIdA = "717ce7ee3217810063a81848c1e12dd322af98541207c6435b5676fa1eb05fb2";
            //const string hexMainTransactionIdB = "9b7eb9883fb1cb35e8f260e1b6a3389c3c89aa8b9ece78df3d43beee21f46da4";
            //const string hexNileTransactionIdA = "e0c504feebea0495e01413c04b6a024f8ba50162421a5f15d46b08c2db11d940";
            //const string hexNileTransactionIdB = "b9a515bc356b1bcca63adba76dc9b97a69ae3d94ceec6e70d81c1c7052bfc79b";

            void ConfigureServices(IServiceCollection services)
            {
                services.AddTronNet(x =>
                {
                    x.Network = TronNetwork.NileTestNet;
                    x.Channel = new GrpcChannelOption { Host = "grpc.nile.trongrid.io", Port = 50051 };
                    x.SolidityChannel = new GrpcChannelOption { Host = "grpc.nile.trongrid.io", Port = 50061 };
                    x.ApiKey = "ed8977d6-19e4-4507-a262-e2c36bf60c9e";
                });
            }

            TronNileTestRecord record = TronNileTestServiceExtension.GetTestRecord();
            Wallet.WalletClient wallet = record.TronClient.GetWallet().GetProtocol();


            //测试网转出账号：TL88smNX7TLVTCfP8osUDubVV6DYg6Ffff
            //测试网转出账号私钥：1f5e7f9db1e3a8730ff467fe60ef0b101c8c92dbe641cd629457922ab25da25f
            var privateStr = "1f5e7f9db1e3a8730ff467fe60ef0b101c8c92dbe641cd629457922ab25da25f";
            var tronKey = new TronECKey(privateStr, record.Options.Value.Network);
            var from = tronKey.GetPublicAddress();
            //测试网转入账号：TCcKzvqTjHUszJuM5FStaEVTN5bob6Xfff
            //测试网转入账号私钥：9abb77092212241005b6504a340ae97f1bd3162c0f7964a9d108cb1ac08e37d6
            var to = "TCcKzvqTjHUszJuM5FStaEVTN5bob6Xfff";
            var amount = 100_000L; // 1 TRX, api only receive trx in Sun, and 1 trx = 1000000 Sun

            //生成交易
            var transactionExtension = await CreateTransactionAsync(from, to, amount, wallet);

            bool getTtransactionExtensionResult = false;
            if (transactionExtension.Result.Result == true)
            {
                var transaction = transactionExtension.Transaction;

                //生成交易签名扩展
                var transactionSignExtention = await wallet.GetTransactionSign2Async(new TransactionSign
                {
                    PrivateKey = ByteString.CopyFrom(privateStr.HexToByteArray()),
                    Transaction = transaction
                });
                //交易广播
                if (transactionSignExtention != null && transactionSignExtention.Result.Result == true)
                {
                    var transactionSigned = transactionSignExtention.Transaction;
                    var transactionBytes = transaction.ToByteArray();
                    //交易签名转数组
                    var transaction4 = SignTransaction2Byte(transactionBytes, privateStr.HexToByteArray(), transactionSigned);
                    //交易签名结果转数组
                    var transaction5 = transactionSigned.ToByteArray();
                    if (transaction4.ToHex() == transaction5.ToHex())
                    {
                        var result = await wallet.BroadcastTransactionAsync(transactionSigned);
                        getTtransactionExtensionResult = result.Result;
                    }
                    else
                    {
                        getTtransactionExtensionResult = false;
                    }
                }
            }
        }

        /// <summary>
        /// 生成一个交易的扩展
        /// </summary>
        /// <param name="from">发送方</param>
        /// <param name="to">接收方</param>
        /// <param name="amount">数量</param>
        /// <param name="wallet">钱包客户端</param>
        /// <returns></returns>
        static async Task<TransactionExtention> CreateTransactionAsync(string from, string to, long amount, Wallet.WalletClient wallet)
        {

            var fromAddress = Base58Encoder.DecodeFromBase58Check(from);
            var toAddress = Base58Encoder.DecodeFromBase58Check(to);

            var transferContract = new TransferContract
            {
                OwnerAddress = ByteString.CopyFrom(fromAddress),
                ToAddress = ByteString.CopyFrom(toAddress),
                Amount = amount
            };

            var transaction = new Transaction();

            var contract = new Transaction.Types.Contract();

            try
            {
                contract.Parameter = Google.Protobuf.WellKnownTypes.Any.Pack(transferContract);
            }
            catch (Exception)
            {
                return new TransactionExtention
                {
                    Result = new Return { Result = false, Code = Return.Types.response_code.OtherError },
                };
            }
            var newestBlock = await wallet.GetNowBlock2Async(new EmptyMessage());

            contract.Type = Transaction.Types.Contract.Types.ContractType.TransferContract;
            transaction.RawData = new Transaction.Types.raw();
            transaction.RawData.Contract.Add(contract);
            transaction.RawData.Timestamp = DateTime.Now.Ticks;
            transaction.RawData.Expiration = newestBlock.BlockHeader.RawData.Timestamp + 10 * 60 * 60 * 1000;
            var blockHeight = newestBlock.BlockHeader.RawData.Number;
            var blockHash = Sha256Sm3Hash.Of(newestBlock.BlockHeader.RawData.ToByteArray()).GetBytes();

            var bb = ByteBuffer.Allocate(8);
            bb.PutLong(blockHeight);

            var refBlockNum = bb.ToArray();

            transaction.RawData.RefBlockHash = ByteString.CopyFrom(blockHash.SubArray(8, 8));
            transaction.RawData.RefBlockBytes = ByteString.CopyFrom(refBlockNum.SubArray(6, 2));

            var transactionExtension = new TransactionExtention
            {
                Transaction = transaction,
                Txid = ByteString.CopyFromUtf8(transaction.GetTxid()),
                Result = new Return { Result = true, Code = Return.Types.response_code.Success },
            };
            Console.WriteLine($"交易信息\r\n" +
                $"交易ID：{transactionExtension.Transaction.GetTxid()}\r\n" +
                $"合约类型：{transactionExtension.Transaction.RawData}\r\n" +
                $"交易ID：{transactionExtension.Txid}\r\n" +
                $"交易ID：{transactionExtension.Txid}\r\n" +
                $"交易ID：{transactionExtension.Txid}\r\n" +
                $"交易ID：{transactionExtension.Txid}\r\n" +
                $"交易ID：{transactionExtension.Txid}\r\n");
            return transactionExtension;
        }

        static byte[] SignTransaction2Byte(byte[] transaction, byte[] privateKey, Transaction transactionSigned)
        {
            var ecKey = new ECKey(privateKey, true);
            var transaction1 = Transaction.Parser.ParseFrom(transaction);
            var rawdata = transaction1.RawData.ToByteArray();
            var hash = rawdata.ToSHA256Hash();
            var sign = ecKey.Sign(hash).ToByteArray();

            transaction1.Signature.Add(ByteString.CopyFrom(sign));

            return transaction1.ToByteArray();
        }
    }

    
}



