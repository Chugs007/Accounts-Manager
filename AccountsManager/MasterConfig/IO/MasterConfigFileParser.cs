using System;
using System.Xml;
using System.Xml.Serialization;

namespace AccountsManager.MasterConfig.IO
{
    public class MasterConfigFileParser : IMasterConfigParser
    {

        private string filePath = String.Empty;

        public MasterConfigFileParser(String fileName)
        {            
            filePath = fileName;
        }

        public (string passwordHash, string salt,bool isFileEncrypted) ParseConfigFile()
        {            
            string passwordHash = string.Empty;
            string salt = string.Empty;
            bool isFileEncrypted = false;
                using (XmlReader xmlReader = XmlReader.Create(filePath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(AccountsConfig));
                    AccountsConfig u = serializer.Deserialize(xmlReader) as AccountsConfig;
                    if (u != null)
                    {
                        passwordHash = u.Password.PasswordHash;
                        salt = u.Password.PasswordSalt;
                        isFileEncrypted = Convert.ToBoolean(u.Encrypted);
                    }
                }
                return (passwordHash, salt,isFileEncrypted);         
        }
    }
}
