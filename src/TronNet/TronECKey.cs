using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TronNet.Crypto;

namespace TronNet
{
    /// <summary>
    /// 波场ECKey：主要用于波场钱包地址生成
    /// ECKey封装了椭圆曲线算法，初始化ECKey，即可获得一个密钥对。 
    /// </summary>
    public class TronECKey
    {
        private readonly ECKey _ecKey;
        private string _publicAddress = null;
        private readonly TronNetwork _network = TronNetwork.MainNet;
        private string _privateKeyHex = null;
        public TronECKey(string privateKey, TronNetwork network)
        {
            _ecKey = new ECKey(privateKey.HexToByteArray(), true);
            _network = network;
        }

        public TronECKey(byte[] vch, bool isPrivate, TronNetwork network)
        {
            _ecKey = new ECKey(vch, isPrivate);
            _network = network;
        }

        internal TronECKey(ECKey ecKey, TronNetwork network)
        {
            _ecKey = ecKey;
            _network = network;
        }

        internal TronECKey(TronNetwork network)
        {
            _ecKey = new ECKey();
            _network = network;
        }

        public static TronECKey GenerateKey(TronNetwork network)
        {
            return new TronECKey(network);
        }

        internal byte GetPublicAddressPrefix()
        {
            return _network == TronNetwork.MainNet ? 0x41 : 0xa0;
        }

        /// <summary>
        /// 创建钱包地址
        /// </summary>
        /// <param name="privateKey">私钥</param>
        /// <param name="network">波场网络选择（默认为波场主网</param>
        /// <returns></returns>
        public static string GetPublicAddress(string privateKey, TronNetwork network = TronNetwork.MainNet)
        {
            var key = new TronECKey(privateKey.HexToByteArray(), true, network);

            return key.GetPublicAddress();
        }

        /// <summary>
        /// 获取当前钱包地址
        /// </summary>
        /// <returns></returns>
        public string GetPublicAddress()
        {
            if (!string.IsNullOrWhiteSpace(_publicAddress)) return _publicAddress;

            var initaddr = _ecKey.GetPubKeyNoPrefix().ToKeccakHash();
            var address = new byte[initaddr.Length - 11];
            Array.Copy(initaddr, 12, address, 1, 20);
            address[0] = GetPublicAddressPrefix();

            var hash = Base58Encoder.TwiceHash(address);
            var bytes = new byte[4];
            Array.Copy(hash, bytes, 4);
            var addressChecksum = new byte[25];
            Array.Copy(address, 0, addressChecksum, 0, 21);
            Array.Copy(bytes, 0, addressChecksum, 21, 4);

            if (_network == TronNetwork.MainNet)
            {
                _publicAddress = Base58Encoder.Encode(addressChecksum);
            }
            else
            {
                _publicAddress = addressChecksum.ToHex();
            }
            return _publicAddress;
        }

        /// <summary>
        /// 获取当前私钥
        /// </summary>
        /// <returns></returns>
        public string GetPrivateKey()
        {
            if (string.IsNullOrWhiteSpace(_privateKeyHex))
            {
                _privateKeyHex = _ecKey.PrivateKey.D.ToByteArrayUnsigned().ToHex();
            }
            return _privateKeyHex;
        }

        /// <summary>
        /// 获取当前公钥
        /// </summary>
        /// <returns></returns>
        public byte[] GetPubKey()
        {
            return _ecKey.GetPubKey();
        }
    }
}
