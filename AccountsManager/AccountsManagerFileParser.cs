using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AccountsManager
{
    public class AccountsManagerFileParser : IFileParser
    {
        private string filePath;

        public AccountsManagerFileParser(string uaFilePath)
        {
            filePath = uaFilePath;
        }

        public IList<UserAccount> ParseFile()
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
            catch (XmlException ex)
            {
                (System.Windows.Application.Current.MainWindow as MainWindow).lblStatus.Visibility = System.Windows.Visibility.Visible;
                FileEncryptor.IsEncrypted = true;
                return userAccounts;                
            }
            return userAccounts;
        }
    }
}
