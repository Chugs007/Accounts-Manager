using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace AccountsManager.MasterAccount.XML.Reader
{
    class AccountsXMLConfigParser : IAccountsConfigParser
    {

        private string filePath = String.Empty;

        public AccountsXMLConfigParser(String fileName)
        {            
            filePath = fileName;
        }

        public (string passwordHash, string salt) ParseConfigFile()
        {            
            string passwordHash = string.Empty;
            string salt = string.Empty;
            using (XmlReader xmlReader = XmlReader.Create(filePath))
            {                 
                XmlSerializer serializer = new XmlSerializer(typeof(User));
                User u = serializer.Deserialize(xmlReader) as User;
                if (u != null)
                {                   
                    passwordHash = u.Password.PasswordHash;
                    salt = u.Password.PasswordSalt;
                }            
            }
            if (String.IsNullOrEmpty(passwordHash) || (String.IsNullOrEmpty(salt)))
            {
                throw new FileFormatException("Unable to read from Accounts Manager Config File");
            }
            return (passwordHash, salt);
        }
    }
}
