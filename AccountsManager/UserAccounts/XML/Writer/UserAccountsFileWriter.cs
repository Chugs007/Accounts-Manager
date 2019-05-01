using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AccountsManager.UserAccounts.XML.Writer
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
