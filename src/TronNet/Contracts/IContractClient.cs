using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TronNet.Accounts;

namespace TronNet.Contracts
{
    /// <summary>
    /// 智能合约客户端接口
    /// Contract Client Interface
    /// </summary>
    public interface IContractClient
    {
        ContractProtocol Protocol { get; }

        Task<string> TransferAsync(string contractAddress, ITronAccount ownerAccount, string toAddress, decimal amount, string memo, long feeLimit);

        Task<decimal> BalanceOfAsync(string contractAddress, ITronAccount ownerAccount);
    }
}
