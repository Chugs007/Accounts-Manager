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
    class AccountsManagerConfigReader
    {

        private string filePath = String.Empty;

        public AccountsManagerConfigReader(String fileName)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                
            }
            if (!File.Exists(fileName))
            {

            }
            filePath = fileName;
        }

        
        public void readConfigFile()
        {
            if (String.IsNullOrEmpty(filePath))
            {

            }
            XmlReader xmlReader = null;
            try
            {
                xmlReader = XmlReader.Create(filePath);
                XmlSerializer serializer = new XmlSerializer(typeof(User));
                User u = serializer.Deserialize(xmlReader) as User;
                if (u != null)
                {
                    FileEncryptor.PasswordHash = u.Password.PasswordHash;
                    FileEncryptor.Salt = u.Password.PasswordSalt;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                xmlReader.Close();
            }
        }
    }
}
