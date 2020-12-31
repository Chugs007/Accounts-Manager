using AccountsManager.MasterConfig;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;


namespace AccountsManager.Users.IO
{
    public class UserAccountsrFileParser : IUserAccountsFileParser
    {
        private string filePath;

        public UserAccountsrFileParser(string uaFilePath)
        {
            filePath = uaFilePath;
        }

        public IList<UserAccount> ParseFile()
        {
            ObservableCollection<UserAccount> userAccounts = new ObservableCollection<UserAccount>();
            try
            {                
                using (XmlReader xmlReader = XmlReader.Create(filePath))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserAccounts));
                    UserAccounts users =  xmlSerializer.Deserialize(xmlReader) as UserAccounts;
                    foreach(var user in users.User)
                    {
                        UserAccount userAccount = new UserAccount();
                        userAccount.Domain = user.Domain;
                        userAccount.UserName = user.UserName;
                        userAccount.Password = user.Password;
                        userAccounts.Add(userAccount);
                    }                     
                }                        
            }
            catch (InvalidOperationException ex)
            {
                (System.Windows.Application.Current.MainWindow as MainWindow).lblStatus.Visibility = System.Windows.Visibility.Visible;
                MasterConfigManager.getInstance().setFileEncrypted(true);
                return userAccounts;                
            }
            return userAccounts;
        }
    }
}
