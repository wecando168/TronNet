using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TronNet.Contracts;

namespace TronNet
{
    /// <summary>
    /// 波场客户端接口
    /// </summary>
    public interface ITronClient
    {
        TronNetwork TronNetwork { get; }
        IGrpcChannelClient GetChannel();
        IWalletClient GetWallet();
        ITransactionClient GetTransaction();
        IContractClient GetContract();
    }
}
