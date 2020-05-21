using NUnit.Framework;
using AccountsManager.Encrpytion;
using System;
using System.Security.Cryptography;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestCreateSalt()
        {
            int length = 10;
            try
            {
                var result = FileEncryptor.CreateSalt(length);
                var bytes = Convert.FromBase64String(result);
            }
            catch
            {
                Assert.Fail("Salt value does not contain valid base64 characters");
            }
            Assert.Pass("Salt value is valid, contains only base64 characters");            
        }

        [Test]
        public void TestCreateHash()
        {
            string password = "abc";
            int length = 10;
            try
            {
                var salt = FileEncryptor.CreateSalt(length);
                var hash = FileEncryptor.CreateHash(password, salt);
                var hashBytes = Convert.FromBase64String(hash);
            }
            catch
            {
                Assert.Fail("Hash value does not contain valid base64 characters");
            }
            Assert.Pass("Hash value is valid, contains only base64 characters");
        }

        [Test]
        public void TestCreateDES()
        {
            string key = "sfasdfasdfasdfasdf";
            int length = 10;
            try
            {
                var salt = FileEncryptor.CreateSalt(length);
                var saltBytes = Convert.FromBase64String(salt);
                RijndaelManaged rijndaelManaged = FileEncryptor.CreateDES(key, saltBytes);
                Assert.NotNull(rijndaelManaged);
            }
            catch
            {
                Assert.Fail("Hash value does not contain valid base64 characters");
            }                        
        }

    }
}