using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace AccountsManager.MasterConfig.IO
{
    public class MasterConfigFileWriter : IMasterConfigWriter
    {
        private string acctMgrConfigFilePath;

        public MasterConfigFileWriter(string configFilePath)
        {
            acctMgrConfigFilePath = configFilePath;

        }

        public void WriteToConfigFile(string passwordHash, string salt)
        {            
            using (XmlWriter xmlWriter = XmlWriter.Create(acctMgrConfigFilePath))
            {
                AccountsConfig u = new AccountsConfig();
                u.Password = new AccountsConfigPassword();
                u.Password.PasswordHash = passwordHash;
                u.Password.PasswordSalt = salt;
                u.Encrypted = MasterConfigManager.getInstance().getIsFileEncrypted().ToString();
                XmlSerializer serializer = new XmlSerializer(typeof(AccountsConfig));
                serializer.Serialize(xmlWriter, u);
            }                      
        }
    }
}
