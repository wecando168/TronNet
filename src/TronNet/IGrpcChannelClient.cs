using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TronNet
{
    /// <summary>
    /// Google远程过程调用频道客户端接口
    /// Google Remote Procedure Call Channel Client Interface
    /// </summary>
    public interface IGrpcChannelClient
    {
        
        Grpc.Core.Channel GetProtocol();
        Grpc.Core.Channel GetSolidityProtocol();
    }
}
