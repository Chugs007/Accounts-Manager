using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AccountsManager
{
    public class AccountsManagerFileWriter
    {
        private string filePath;

        public AccountsManagerFileWriter(string filePath)
        {
            this.filePath = filePath;
        }

        public void writeAccountToFile(UserAccount account)
        {
            string user = account.UserName;
            string password = account.Password;
            string domain = account.Domain;
            XmlDocument xmldocument = new XmlDocument();
            xmldocument.Load(filePath);
            XmlNode xmlnode = xmldocument.CreateNode("element", "UserAccount", "");
            XmlNode userNameNode = xmldocument.CreateNode("element", "UserName", "");
            userNameNode.InnerText = user;
            xmlnode.AppendChild(userNameNode);
            XmlNode passwordNode = xmldocument.CreateNode("element", "Password", "");
            passwordNode.InnerText = password;
            xmlnode.AppendChild(passwordNode);
            XmlNode domainNode = xmldocument.CreateNode("element", "Domain", "");
            domainNode.InnerText = domain;
            xmlnode.AppendChild(domainNode);
            XmlElement rootelement = xmldocument.DocumentElement;
            rootelement.AppendChild(xmlnode);
            xmldocument.Save(filePath);
        }

        public void writeAccountsToFile(List<UserAccount> userAccounts)
        {
            XmlDocument xmldocument = new XmlDocument();
            xmldocument.Load(filePath);
            XmlNode root = xmldocument.DocumentElement;
            root.RemoveAll();
            foreach (var UserAccount in userAccounts)
            {
                XmlNode xmlnode = xmldocument.CreateNode("element", "UserAccount", "");
                XmlNode userNameNode = xmldocument.CreateNode("element", "UserName", "");
                userNameNode.InnerText = UserAccount.UserName;
                xmlnode.AppendChild(userNameNode);
                XmlNode passwordNode = xmldocument.CreateNode("element", "Password", "");
                passwordNode.InnerText = UserAccount.Password;
                xmlnode.AppendChild(passwordNode);
                XmlNode domainNode = xmldocument.CreateNode("element", "Domain", "");
                domainNode.InnerText = UserAccount.Domain;
                xmlnode.AppendChild(domainNode);
                root.AppendChild(xmlnode);
            }
            xmldocument.Save(filePath);
        }
    }
}
