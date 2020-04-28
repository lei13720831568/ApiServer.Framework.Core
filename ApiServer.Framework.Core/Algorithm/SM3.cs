using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ApiServer.Framework.Core.Algorithm
{
    public class SM3Algorithm : HashAlgorithm
    {
        /// <summary>
        /// 算法名称
        /// </summary>
        public string AlgorithmName { get { return "SM3"; } }

        /// <summary>
        /// 哈希值大小（以位为单位）
        /// </summary>
        public override int HashSize { get { return 256; } }

        /// <summary>
        /// 哈希值大小（以字节数为单位）
        /// </summary>
        public const int HashSizeInBytes = 32;

        /// <summary>
        /// 分组块大小（以字节数为单位）
        /// </summary>
        public const int GroupBlockSizeInBytes = 64;

        /// <summary>
        /// 初始状态向量
        /// </summary>
        private static readonly UInt32[] IV = { 0x7380166F, 0x4914B2B9, 0x172442D7, 0xDA8A0600, 0xA96F30BC, 0x163138AA, 0xE38DEE4D, 0xB0FB0E4E };

        /// <summary>
        /// 填充数据
        /// </summary>
        private static readonly byte[] SM3_PADDING = {
            0x80, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
               0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
               0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
               0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };

        /// <summary>
        /// 当前状态向量
        /// </summary>
        private UInt32[] V = new UInt32[8];

        /// <summary>
        /// 内部数据缓冲区
        /// </summary>
        private UInt32[] W = new UInt32[68];

        /// <summary>
        /// 4字节数据单元存储器
        /// </summary>
        private byte[] M = new byte[4];

        /// <summary>
        /// 已处理的字节计数
        /// </summary>
        private UInt64 BytesCount = 0;

        /// <summary>
        /// 缓冲区X存储位置偏移量
        /// </summary>
        private int WOff = 0;

        /// <summary>
        /// 缓冲区M存储位置偏移量
        /// </summary>
        private int MOff = 0;

        /// <summary>
        /// 构造函数
        /// </summary>
        public SM3Algorithm()
        {
            Initialize();
        }

        /// <summary>
        /// 拷贝构造函数
        /// </summary>
        /// <param name="Source">要复制的对象实例</param>
        public SM3Algorithm(SM3Algorithm Source)
        {
            BytesCount = Source.BytesCount;

            WOff = Source.WOff;
            if (WOff > 0) Array.Copy(Source.W, W, WOff);

            MOff = Source.MOff;
            if (MOff > 0) Array.Copy(Source.M, M, MOff);

            // 拷贝状态向量
            Array.Copy(Source.V, V, V.Length);
        }

        /// <summary>
        /// 创建 SM3 的默认实现的实例
        /// </summary>
        /// <returns>SM3 的新实例</returns>
        public new static SM3Algorithm Create()
        {
            return new SM3Algorithm();
        }

        /// <summary>
        /// 创建 SM3 的指定实现的实例
        /// </summary>
        /// <param name="hashName">要使用的 SM3 的特定实现的名称，支持的值有"SM3"、"System.Security.Cryptography.SM3"</param>
        /// <returns>使用指定实现的 SM3 的新实例</returns>
        public new static SM3Algorithm Create(string hashName)
        {
            if (String.Equals(hashName, "SM3", StringComparison.OrdinalIgnoreCase)
                || String.Equals(hashName, "System.Security.Cryptography.SM3", StringComparison.OrdinalIgnoreCase))
                return new SM3Algorithm();
            else
                return null;
        }

        /// <summary>
        /// 类初始化
        /// </summary>
        public override void Initialize()
        {
            BytesCount = 0;
            WOff = 0;
            MOff = 0;

            // 初始化状态向量
            Array.Copy(IV, V, V.Length);

            // 清除敏感数据
            Array.Clear(W, 0, W.Length);
        }

        /// <summary>
        /// 将数据路由到哈希算法以计算哈希值
        /// </summary>
        /// <param name="input">要计算其哈希代码的输入</param>
        /// <param name="offset">字节数组中的偏移量，从该位置开始使用数据</param>
        /// <param name="length">字节数组中用作数据的字节数</param>
        protected override void HashCore(byte[] input, int offset, int length)
        {
            while ((MOff != 0) && (length > 0))
            {
                Update(input[offset]);
                offset++;
                length--;
            }

            while (length >= 4)
            {
                ProcessWord(input, offset);

                offset += 4;
                length -= 4;
                BytesCount += 4;
            }

            while (length > 0)
            {
                Update(input[offset]);
                offset++;
                length--;
            }
        }

        /// <summary>
        /// 完成最终计算，并返回数据流的正确哈希值
        /// </summary>
        /// <returns>计算所得的哈希代码</returns>
        protected override byte[] HashFinal()
        {
            Finish();

            byte[] output = new byte[32];
            int i = 0;
            foreach (UInt32 n in V)
            {
                output[i++] = (byte)(n >> 24);
                output[i++] = (byte)(n >> 16);
                output[i++] = (byte)(n >> 8);
                output[i++] = (byte)(n & 0xFF);
            }

            Initialize();
            return output;
        }

        /// <summary>
        /// 处理单个数据
        /// </summary>
        /// <param name="input">输入的单字节数据</param>
        public void Update(byte input)
        {
            M[MOff++] = input;
            if (MOff == 4)
            {
                ProcessWord(M, 0);
                MOff = 0;
            }

            BytesCount++;
        }

        /// <summary>
        /// 处理批量数据
        /// </summary>
        /// <param name="input">包含输入数据的字节数组</param>
        /// <param name="offset">数据在字节数组中的起始偏移量</param>
        /// <param name="length">数据长度</param>
        public void BlockUpdate(byte[] input, int offset, int length)
        {
            HashCore(input, offset, length);
        }

        /// <summary>
        /// 处理批量数据
        /// </summary>
        /// <param name="input">包含输入数据的字节数组</param>
        public void BlockUpdate(byte[] input)
        {
            HashCore(input, 0, input.Length);
        }

        /// <summary>
        /// 完成最终计算，并返回数据流的正确哈希值
        /// </summary>
        /// <returns>计算所得的哈希代码</returns>
        public byte[] DoFinal()
        {
            return HashFinal();
        }

        /// <summary>
        /// 完成最终计算，并返回数据流的正确哈希值
        /// </summary>
        /// <param name="input">包含输入数据的字节数组</param>
        /// <param name="offset">数据在字节数组中的起始偏移量</param>
        /// <param name="length">数据长度</param>
        /// <returns>计算所得的哈希代码</returns>
        public byte[] DoFinal(byte[] input, int offset, int length)
        {
            HashCore(input, offset, length);
            return HashFinal();
        }

        /// <summary>
        /// 完成最终计算，并返回数据流的正确哈希值
        /// </summary>
        /// <param name="input">包含输入数据的字节数组</param>
        /// <returns>计算所得的哈希代码</returns>
        public byte[] DoFinal(byte[] input)
        {
            HashCore(input, 0, input.Length);
            return HashFinal();
        }

        private void ProcessWord(byte[] input, int offset)
        {
            W[WOff++] = (UInt32)((input[offset] << 24) | (input[offset + 1] << 16) | (input[offset + 2] << 8) | input[offset + 3]);
            if (WOff == 16)
            {
                ProcessBlock();
            }
        }

        private void ProcessBlock()
        {
            UInt32[] W1 = new UInt32[64];

            // 消息扩展（生成132个4字节数据）
            // a：将消息分组B(i)划分为16个4字节整型数据

            // b：W[j] = P1(W[j-16] ^ W[j-9] ^ ROTL(W[j-3],15)) ^ ROTL(W[j - 13],7) ^ W[j-6]
            for (int j = 16; j < 68; j++)
            {
                W[j] = P1(W[j - 16] ^ W[j - 9] ^ ROTL(W[j - 3], 15)) ^ ROTL(W[j - 13], 7) ^ W[j - 6];
            }

            // c：W1[j] = W[j] ^ W[j+4]
            for (int j = 0; j < 64; j++)
            {
                W1[j] = W[j] ^ W[j + 4];
            }

            // 压缩函数
            UInt32 A = V[0];
            UInt32 B = V[1];
            UInt32 C = V[2];
            UInt32 D = V[3];
            UInt32 E = V[4];
            UInt32 F = V[5];
            UInt32 G = V[6];
            UInt32 H = V[7];

            for (int j = 0; j < 16; j++)
            {
                UInt32 Q = ROTL(A, 12);
                UInt32 SS1 = ROTL((Q + E + ROTL(0x79CC4519, j)), 7);
                UInt32 SS2 = SS1 ^ Q;
                UInt32 TT1 = FF0(A, B, C) + D + SS2 + W1[j];
                UInt32 TT2 = GG0(E, F, G) + H + SS1 + W[j];
                D = C;
                C = ROTL(B, 9);
                B = A;
                A = TT1;
                H = G;
                G = ROTL(F, 19);
                F = E;
                E = P0(TT2);
            }

            for (int j = 16; j < 64; j++)
            {
                UInt32 Q = ROTL(A, 12);
                UInt32 SS1 = ROTL((Q + E + ROTL(0x7A879D8A, j)), 7);
                UInt32 SS2 = SS1 ^ Q;
                UInt32 TT1 = FF1(A, B, C) + D + SS2 + W1[j];
                UInt32 TT2 = GG1(E, F, G) + H + SS1 + W[j];
                D = C;
                C = ROTL(B, 9);
                B = A;
                A = TT1;
                H = G;
                G = ROTL(F, 19);
                F = E;
                E = P0(TT2);
            }

            V[0] ^= A;
            V[1] ^= B;
            V[2] ^= C;
            V[3] ^= D;
            V[4] ^= E;
            V[5] ^= F;
            V[6] ^= G;
            V[7] ^= H;

            // 重置缓冲区
            WOff = 0;
        }

        private void Finish()
        {
            // 计算实际消息比特长度
            UInt64 BitsLength = BytesCount << 3;

            // 计算填充字节数
            int LeftBytes = (WOff << 2) + MOff;
            int PaddingBytes = (LeftBytes < 56) ? (56 - LeftBytes) : (120 - LeftBytes);

            // 加入填充数据块
            HashCore(SM3_PADDING, 0, PaddingBytes);

            // 加入实际消息比特长度
            byte[] L = BitConverter.GetBytes(BitsLength);
            Array.Reverse(L);
            HashCore(L, 0, 8);
        }

        // 四字节无符号整数位循环左移位操作
        private UInt32 ROTL(UInt32 x, int n)
        {
            return (x << n) | (x >> (32 - n));
        }

        // 布尔函数
        private UInt32 FF0(UInt32 x, UInt32 y, UInt32 z)
        {
            return x ^ y ^ z;
        }

        private UInt32 FF1(UInt32 x, UInt32 y, UInt32 z)
        {
            return (x & y) | (x & z) | (y & z);
        }

        private UInt32 GG0(UInt32 x, UInt32 y, UInt32 z)
        {
            return x ^ y ^ z;
        }

        private UInt32 GG1(UInt32 x, UInt32 y, UInt32 z)
        {
            return (x & y) | (~x & z);
        }

        // 置换函数
        private UInt32 P0(UInt32 x)
        {
            return x ^ ROTL(x, 9) ^ ROTL(x, 17);
        }

        private UInt32 P1(UInt32 x)
        {
            return x ^ ROTL(x, 15) ^ ROTL(x, 23);
        }
    }
}
