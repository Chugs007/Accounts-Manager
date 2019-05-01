using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace AccountsManager.Users.IO
{
    public class UserAccountsFileWriter : IUserAccountsWriter
    {
        private string filePath;

        public UserAccountsFileWriter(string filePath)
        {
            this.filePath = filePath;
        }      

        public void WriteAccountsToFile(IList<UserAccount> userAccounts)
        {
            XmlDocument xmldocument = new XmlDocument();
            xmldocument.Load(filePath);
            XmlNode root = xmldocument.DocumentElement;
            root.RemoveAll();
            xmldocument.Save(filePath);
            using (XmlWriter xmlWriter = XmlWriter.Create(filePath))
            {
                UserAccounts users = new UserAccounts();
                List<Account> accounts = new List<Account>();
                foreach(var user in userAccounts)
                {
                    Account account = new Account();
                    account.Domain = user.Domain;
                    account.UserName = user.UserName;
                    account.Password = user.Password;
                    accounts.Add(account);
                }
                users.User = accounts.ToArray();
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserAccounts));
                xmlSerializer.Serialize(xmlWriter,users);
            }
            //    foreach (var UserAccount in userAccounts)
            //{
            //    XmlNode xmlnode = xmldocument.CreateNode("element", "UserAccount", "");
            //    XmlNode userNameNode = xmldocument.CreateNode("element", "UserName", "");
            //    userNameNode.InnerText = UserAccount.UserName;
            //    xmlnode.AppendChild(userNameNode);
            //    XmlNode passwordNode = xmldocument.CreateNode("element", "Password", "");
            //    passwordNode.InnerText = UserAccount.Password;
            //    xmlnode.AppendChild(passwordNode);
            //    XmlNode domainNode = xmldocument.CreateNode("element", "Domain", "");
            //    domainNode.InnerText = UserAccount.Domain;
            //    xmlnode.AppendChild(domainNode);
            //    root.AppendChild(xmlnode);
            //}
           
        }
    }
}
