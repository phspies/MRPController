using MRMPService.Utiliies;
using System;
using System.Text;

namespace MRPWorkerService.Database
{
    public class SimpleAES
    {
		private readonly CryptLib _cryptLib;

		public SimpleAES()
		{
			_cryptLib = new CryptLib();
		}

        public string Encrypt(string unencrypted, string _encryptionKey, string _initVector)
        {
			return _cryptLib.encrypt(Encoding.UTF8.GetBytes(unencrypted), _encryptionKey, _initVector);
        }

        public string Decrypt(string encrypted, string _encryptionKey, string _initVector)
        {
			return _cryptLib.decrypt(Convert.FromBase64String(encrypted), _encryptionKey, _initVector);
        }
    }
}
