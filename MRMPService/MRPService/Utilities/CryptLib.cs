using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MRMPService.Utiliies
{
    /*****************************************************************
     * CrossPlatform CryptLib
     * 
     * <p>
     * This cross platform CryptLib uses AES 256 for encryption. This library can
     * be used for encryptoion and de-cryption of string on iOS, Android and Windows
     * platform.<br/>
     * Features: <br/>
     * 1. 256 bit AES encryption
     * 2. Random IV generation. 
     * 3. Provision for SHA256 hashing of key. 
     * </p>
     * 
     * @since 1.0
     * @author navneet
     *****************************************************************/
    public class CryptLib : IDisposable
    {
        UTF8Encoding _enc;
        Aes _aes;
        byte[] _key, _pwd, _ivBytes, _iv;
		private bool disposedValue = false;

		/***
         * Encryption mode enumeration
         */
		private enum EncryptMode { ENCRYPT, DECRYPT };

        static readonly char[] CharacterMatrixForRandomIVStringGeneration = {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
            'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '_'
        };

        /**
         * This function generates random string of the given input length.
         * 
         * @param _plainText
         *            Plain text to be encrypted
         * @param _key
         *            Encryption Key. You'll have to use the same key for decryption
         * @return returns encrypted (cipher) text
         */
        internal static string GenerateRandomIV(int length)
        {
            char[] _iv = new char[length];
            byte[] randomBytes = new byte[length];

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes); //Fills an array of bytes with a cryptographically strong sequence of random values. 
            }

			return Convert.ToBase64String(randomBytes);
        }



        public CryptLib()
        {
            _enc = new UTF8Encoding();
			_aes = Aes.Create();
            _aes.Mode = CipherMode.CBC;
            _aes.Padding = PaddingMode.PKCS7;
            _aes.KeySize = 256;
            _aes.BlockSize = 128;
            _key = new byte[32];
            _iv = new byte[_aes.BlockSize / 8]; //128 bit / 8 = 16 bytes
            _ivBytes = new byte[16];
        }

        /**
         * 
         * @param _inputText
         *            Text to be encrypted or decrypted
         * @param _encryptionKey
         *            Encryption key to used for encryption / decryption
         * @param _mode
         *            specify the mode encryption / decryption
         * @param _initVector
         * 			  initialization vector
         * @return encrypted or decrypted string based on the mode
        */
        private String encryptDecrypt(byte[] _encryptDecryptBytes, string _encryptionKey, EncryptMode _mode, string _initVector)
        {
            string _out = "";// output string
                             //_encryptionKey = MD5Hash (_encryptionKey);
            _pwd = Encoding.UTF8.GetBytes(_encryptionKey);
            _ivBytes = Encoding.UTF8.GetBytes(_initVector);

            int len = _pwd.Length;
            if (len > _key.Length)
            {
                len = _key.Length;
            }
            int ivLenth = _ivBytes.Length;
            if (ivLenth > _iv.Length)
            {
                ivLenth = _iv.Length;
            }

            Array.Copy(_pwd, _key, len);
            Array.Copy(_ivBytes, _iv, ivLenth);
            _aes.Key = _key;
            _aes.IV = _iv;

            if (_mode.Equals(EncryptMode.ENCRYPT))
			{
				//encrypt
				using (var memoryStream = new MemoryStream())
				{
					using (var encryptor = _aes.CreateEncryptor())
					{
						using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
						{
							cryptoStream.Write(_encryptDecryptBytes, 0, _encryptDecryptBytes.Length);
							cryptoStream.Close();
							_out = Convert.ToBase64String(memoryStream.ToArray());
						}
					} 
				}
			}
			if (_mode.Equals(EncryptMode.DECRYPT))
            {
				//decrypt
				using (var memoryStream = new MemoryStream())
				{
					using (var decryptor = _aes.CreateDecryptor())
					{
						using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write))
						{
							cryptoStream.Write(_encryptDecryptBytes, 0, _encryptDecryptBytes.Length);
							cryptoStream.Close();
							_out = _enc.GetString(memoryStream.ToArray());
						}
					}
				}
            }
            return _out;// return encrypted/decrypted string
        }

        /**
         * This function encrypts the plain text to cipher text using the key
         * provided. You'll have to use the same key for decryption
         * 
         * @param _plainText
         *            Plain text to be encrypted
         * @param _key
         *            Encryption Key. You'll have to use the same key for decryption
         * @return returns encrypted (cipher) text
         */
        public string encrypt(byte[] _plainText, string _key, string _initVector)
        {
            return encryptDecrypt(_plainText, _key, EncryptMode.ENCRYPT, _initVector);
        }

        /***
         * This funtion decrypts the encrypted text to plain text using the key
         * provided. You'll have to use the same key which you used during
         * encryprtion
         * 
         * @param _encryptedText
         *            Encrypted/Cipher text to be decrypted
         * @param _key
         *            Encryption key which you used during encryption
         * @return encrypted value
         */

        public string decrypt(byte[] _encryptedText, string _key, string _initVector)
        {
            return encryptDecrypt(_encryptedText, _key, EncryptMode.DECRYPT, _initVector);
        }

        /***
         * This function decrypts the encrypted text to plain text using the key
         * provided. You'll have to use the same key which you used during
         * encryption
         * 
         * @param _encryptedText
         *            Encrypted/Cipher text to be decrypted
         * @param _key
         *            Encryption key which you used during encryption
         */
        public static string getHashSha256(string text)
        {
			using (SHA256 sha256 = SHA256.Create())
			{
				var computedHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));

				StringBuilder hexString = new StringBuilder();

				foreach (var hash in computedHash)
					hexString.Append(hash.ToString("x2"));

				return hexString.ToString();
			}
		}

        //this function is no longer used.
        private static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits
                //for each byte
                strBuilder.Append(result[i].ToString("x2"));
            }
            Console.WriteLine("md5 hash of they key=" + strBuilder.ToString());
            return strBuilder.ToString();
        }

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					if (_aes != null)
						_aes.Dispose();
				}

				Array.Clear(_key, 0, _key.Length);
				Array.Clear(_pwd, 0, _pwd.Length);
				Array.Clear(_ivBytes, 0, _ivBytes.Length);
				Array.Clear(_iv, 0, _iv.Length);

				disposedValue = true;
			}
		}

		 ~CryptLib()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}

