using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TronNet.Protocol;

namespace TronNet
{
    /// <summary>
    /// 交易客户端接口
    /// Transaction Client Interface
    /// </summary>
    public interface ITransactionClient
    {
        /// <summary>
        /// 创建异步交易
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        Task<TransactionExtention> CreateTransactionAsync(string from, string to, long amount);

        /// <summary>
        /// 获取交易签名
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        Transaction GetTransactionSign(Transaction transaction, string privateKey);

        /// <summary>
        /// 广播异步交易
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<Return> BroadcastTransactionAsync(Transaction transaction);
    }
}
