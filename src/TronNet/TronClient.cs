using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TronNet.Contracts;
using TronNet.Protocol;

namespace TronNet
{
    /// <summary>
    /// 波场客户端
    /// </summary>
    class TronClient : ITronClient
    {
        private readonly ILogger<TronClient> _logger;
        private readonly IOptions<TronNetOptions> _options;
        private readonly IGrpcChannelClient _channelClient;
        private readonly IWalletClient _walletClient;
        private readonly ITransactionClient _transactionClient;

        public TronNetwork TronNetwork => _options.Value.Network;

        /// <summary>
        /// 波场客户端
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="options"></param>
        /// <param name="channelClient"></param>
        /// <param name="walletClient"></param>
        /// <param name="transactionClient"></param>
        public TronClient(ILogger<TronClient> logger, IOptions<TronNetOptions> options, IGrpcChannelClient channelClient, IWalletClient walletClient, ITransactionClient transactionClient)
        {
            _logger = logger;
            _options = options;
            _channelClient = channelClient;
            _walletClient = walletClient;
            _transactionClient = transactionClient;
        }

        /// <summary>
        /// 获取频道
        /// </summary>
        /// <returns></returns>
        public IGrpcChannelClient GetChannel()
        {
            return _channelClient; 
        }

        /// <summary>
        /// 获取钱包
        /// </summary>
        /// <returns></returns>
        public IWalletClient GetWallet()
        {
            return _walletClient;
        }

        /// <summary>
        /// 获取交易
        /// </summary>
        /// <returns></returns>
        public ITransactionClient GetTransaction()
        {
            return _transactionClient;
        }

        /// <summary>
        /// 获取智能合约
        /// </summary>
        /// <returns></returns>
        public IContractClient GetContract()
        {
            return null;
        }
    }
}
