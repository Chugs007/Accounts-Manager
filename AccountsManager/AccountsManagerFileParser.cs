using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AccountsManager
{
    public class AccountsManagerFileParser
    {
        private string filePath;

        public AccountsManagerFileParser(string uaFilePath)
        {
            filePath = uaFilePath;
        }

        public ObservableCollection<UserAccount> parseFile()
        {
            ObservableCollection<UserAccount> userAccounts = new ObservableCollection<UserAccount>();
            try
            {
                XmlDocument xmldocument = new XmlDocument();
                xmldocument.Load(filePath);
                XmlNodeList nodelist = xmldocument.SelectNodes("/UserAccounts/UserAccount");
                foreach (XmlNode node in nodelist)
                {
                    UserAccount ua = new UserAccount();
                    ua.Domain = node["Domain"].InnerText;
                    ua.UserName = node["UserName"].InnerText;
                    ua.Password = node["Password"].InnerText;
                    userAccounts.Add(ua);
                }
               
                FileEncryptor.IsEncrypted = false;                
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(XmlException))
                {
                    System.Windows.MessageBox.Show("File is encrypted please enter a password in password box and click decrypt, then refresh.");
                    FileEncryptor.IsEncrypted = true;
                    return userAccounts;
                }
            }
            return userAccounts;
        }
    }
}
