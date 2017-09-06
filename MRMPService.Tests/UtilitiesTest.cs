using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MRMPService;
using MRMPService.Utilities;
using MRMPService.Utiliies;
using System.Text;

namespace MRMPService.Tests
{
	[TestClass]
	public class UtilitiesTest
	{
		[TestMethod]
		public void TestEncryptDecrypt()
		{
			CryptLib cryptLib = new CryptLib();

			string key = "12345ABCD";
			string iv = "12345ABCD";
			string valueToEncrypt = "testing encrypt decrypt";
			byte[] bytesToEncrypt = Encoding.UTF8.GetBytes(valueToEncrypt);

			var encryptedText = cryptLib.encrypt(bytesToEncrypt, key, iv);

			byte[] valueToDecrypt = Convert.FromBase64String(encryptedText);
			var decryptedValue = cryptLib.decrypt(valueToDecrypt, key, iv);

			Assert.AreEqual(valueToEncrypt, decryptedValue);
		}
	}
}
