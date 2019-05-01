using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace AccountsManager.MasterAccount.IO
{
    public class AccountsXMLConfigWriter : IAccountsConfigWriter
    {
        private string acctMgrConfigFilePath;

        public AccountsXMLConfigWriter(string configFilePath)
        {
            acctMgrConfigFilePath = configFilePath;

        }

        public void WriteToConfigFile(string passwordHash, string salt)
        {            
            using (XmlWriter xmlWriter = XmlWriter.Create(acctMgrConfigFilePath))
            {
                User u = new User();
                u.Name = MasterPasswordManager.ADMINACCT; 
                u.Password = new UserPassword();
                u.Password.PasswordHash = passwordHash;
                u.Password.PasswordSalt = salt;
                XmlSerializer serializer = new XmlSerializer(typeof(User));
                serializer.Serialize(xmlWriter, u);
            }                      
        }
    }
}
