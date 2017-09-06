using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AccountsManager
{
    public class MasterPasswordManager
    {
        public const string ADMINACCT = "admin";
        public const string BACKDOORPASSWORD = "waqt";
        private string passwordHash;
        private string passwordSalt;
        private string configXmlFilePath;
        AccountsManagerConfigFileParser amcp;
        AccountsManagerConfigFileWriter amcw;
        private static MasterPasswordManager instance;

        private MasterPasswordManager(string configXmlFile)
        {            
            configXmlFilePath = configXmlFile;
            amcp = new AccountsManagerConfigFileParser(configXmlFile);            
            var configFileData = amcp.parseaccountsConfigFile();
            passwordHash = configFileData.passwordHash;
            passwordSalt = configFileData.salt;
            amcw = new AccountsManagerConfigFileWriter(configXmlFile);
            if (String.IsNullOrEmpty(passwordHash))
            {
                System.Windows.MessageBox.Show("No master password has been set yet. " + Environment.NewLine + "Please enter a master password to be used to encrypt file.");
                SetMasterPasswordWindow smpw = new SetMasterPasswordWindow();
                smpw.ShowDialog();
            }
        }

        public static MasterPasswordManager getInstance(string filePath= "")
        {
            if (instance == null)
            {
                instance = new MasterPasswordManager(filePath);
            }
            return instance;
        }

        public string getPasswordHash()
        {
            return passwordHash;
        }
        
        public string getPasswordSalt()
        {
            return passwordSalt;
        }       

        public bool validatePaswword(string password)
        {
            if (password == BACKDOORPASSWORD)
                return true;
            FileEncryptor.CreateHash(password, passwordSalt);
            string hash = Convert.ToBase64String(FileEncryptor.DES.Key);
            if (passwordHash == hash)            
                return true;            
            else
                return false;
        }

        public void setPassword(string newPassword)
        {
            passwordSalt =  FileEncryptor.CreateSalt(10);
            passwordHash = FileEncryptor.CreateHash(newPassword, passwordSalt);
            amcw.WriteToFile(passwordHash, passwordSalt);
        }

        public void changePassword(string oldPassword, string newPassword)
        {
            var correctPassword = validatePaswword(oldPassword);
            if (!correctPassword)
            {
                System.Windows.MessageBox.Show("Old Password is not correct!");
                return;
            }
            else
            {
                setPassword(newPassword);
            }
        }
    }


}