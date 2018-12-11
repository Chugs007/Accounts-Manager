using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Windows;

namespace AccountsManager
{
    class AccountsManagerConfigFileParser
    {

        private string filePath = String.Empty;

        public AccountsManagerConfigFileParser(String fileName)
        {            
            filePath = fileName;
        }

        public (string passwordHash, string salt) parseFile()
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

            return (passwordHash, salt);
        }
    }
}
