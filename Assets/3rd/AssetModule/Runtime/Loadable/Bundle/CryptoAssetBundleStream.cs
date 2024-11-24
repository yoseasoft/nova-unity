using System;
using System.IO;
using System.Security.Cryptography;

namespace AssetModule
{
    /// <summary>
    /// 加密ab包的文件流
    /// 参考:https://github.com/mao-test-h/SeekableAesAssetBundle/blob/master/Assets/SeekableAesAssetBundle/Scripts/SeekableAesStream.cs
    /// </summary>
    public class CryptoAssetBundleStream : FileStream
    {
        readonly AesManaged _aes;

        readonly ICryptoTransform _encryptor;

        /// <summary>
        /// 加密ab包的文件流
        /// </summary>
        /// <param name="path">文件目录</param>
        /// <param name="mode"></param>
        /// <param name="access"></param>
        /// <param name="share"></param>
        /// <param name="bufferSize">建议和Unity读取值一样(Unity默认LoadFromStream的bufferSize为32KB)</param>
        /// <param name="password">可以固定的密码</param>
        /// <param name="salt">每个文件的salt值必需都不一样, 不然会容易被破解</param>
        public CryptoAssetBundleStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, byte[] password, string salt) : base(path, mode, access, share, bufferSize)
        {
            using var key = new Rfc2898DeriveBytes(password, System.Text.Encoding.UTF8.GetBytes(salt), 10000);
            _aes = new AesManaged
            {
                KeySize = 128,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.None
            };
            _aes.Key = key.GetBytes(_aes.KeySize / 8);
            // zero buffer is adequate since we have to use new salt for each stream
            _aes.IV = new byte[16];
            _encryptor = _aes.CreateEncryptor(_aes.Key, _aes.IV);
        }

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => true;

        /// <summary>
        /// 使用AES密钥生成的bytes异或加密数据
        /// </summary>
        void XOrByAesKey(byte[] buffer, int offset, int count, long streamPos)
        {
            // find block number
            var blockSizeInByte = _aes.BlockSize / 8;
            var blockNumber = (streamPos / blockSizeInByte) + 1;
            var keyPos = streamPos % blockSizeInByte;

            // buffer
            var outBuffer = new byte[blockSizeInByte];
            var nonce = new byte[blockSizeInByte];
            var init = false;

            for (var i = offset; i < count; i++)
            {
                // encrypt the nonce to form next xro buffer(unique key)
                if (!init || (keyPos % blockSizeInByte) == 0)
                {
                    BitConverter.GetBytes(blockNumber).CopyTo(nonce, 0);
                    _encryptor.TransformBlock(nonce, 0, nonce.Length, outBuffer, 0);
                    if (init) keyPos = 0;
                    init = true;
                    blockNumber++;
                }

                buffer[i] ^= outBuffer[keyPos];
                keyPos++;
            }
        }

        /// <summary>
        /// 仅加密文件前32KB的数据(性能优化考虑, AssetBundle.LoadFromStream时Unity默认单次读取32KB, 所以可以仅解密一次)
        /// </summary>
        const int NeedEncodeBytesNum = 32 * 1024;

        /// <summary>
        /// 加密数据(只加密需要的部分)
        /// </summary>
        void Cipher(byte[] buffer, int offset, int count, long streamPos)
        {
            if (streamPos >= NeedEncodeBytesNum)
                return;

            if (streamPos + count > NeedEncodeBytesNum)
            {
                int restNum = NeedEncodeBytesNum - (int)streamPos;
                byte[] needAesEncryptBuffer = new byte[restNum];
                for (int i = 0; i < restNum; i++)
                    needAesEncryptBuffer[i] = buffer[i];
                XOrByAesKey(needAesEncryptBuffer, offset, restNum, streamPos);
                for (int i = 0; i < restNum; i++)
                    buffer[i] = needAesEncryptBuffer[i];
            }
            else
                XOrByAesKey(buffer, offset, count, streamPos);
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        public override int Read(byte[] buffer, int offset, int count)
        {
            long streamPos = Position;
            int num = base.Read(buffer, offset, count);
            Cipher(buffer, offset, count, streamPos);
            return num;
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        public override void Write(byte[] buffer, int offset, int count)
        {
            Cipher(buffer, offset, count, Position);
            base.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _encryptor?.Dispose();
                _aes?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}