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
            if (String.IsNullOrEmpty(fileName))
            {
                
            }
            if (!File.Exists(fileName))
            {

            }
            filePath = fileName;
        }

        public (string passwordHash, string salt) parseaccountsConfigFile()
        {            
            if (String.IsNullOrEmpty(filePath))
            {

            }
            string passwordHash = string.Empty;
            string salt = string.Empty;
            XmlReader xmlReader = null;
            try
            {
                xmlReader = XmlReader.Create(filePath);
                XmlSerializer serializer = new XmlSerializer(typeof(User));
                User u = serializer.Deserialize(xmlReader) as User;
                if (u != null)
                {                   
                    passwordHash = u.Password.PasswordHash;
                    salt = u.Password.PasswordSalt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                xmlReader.Close();
            }

            return (passwordHash, salt);
        }
    }
}
