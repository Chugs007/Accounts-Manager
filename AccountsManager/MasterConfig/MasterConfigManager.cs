using System;
using System.Windows;
using AccountsManager.Encrpytion;
using AccountsManager.MasterConfig.IO;

namespace AccountsManager.MasterConfig
{
    public class MasterConfigManager
    {    
        private string passwordHash;
        private string passwordSalt;
        private static string configXmlFilePath;
        private IMasterConfigParser amcp;
        private IMasterConfigWriter amcw;
        private bool isFileEncrypted;
        private static Lazy<MasterConfigManager> instance = new Lazy<MasterConfigManager>(( ) => new MasterConfigManager(configXmlFilePath));

        private MasterConfigManager(string configXmlFile)  
        {            
            configXmlFilePath = configXmlFile; 
            amcp = new MasterConfigFileParser(configXmlFile);
            amcw = new MasterConfigFileWriter(configXmlFile);            
            var configFileData = amcp.ParseConfigFile();                                        
            passwordHash = configFileData.passwordHash;
            passwordSalt = configFileData.salt;
            isFileEncrypted = configFileData.isFileEncrypted;

        }

        public static MasterConfigManager getInstance(string filePath= "")
        {
            MasterConfigManager.configXmlFilePath = filePath;
            return instance.Value;
        }

        public string getPasswordHash()
        {
            return passwordHash;
        }
        
        public string getPasswordSalt()
        {
            return passwordSalt;
        }

        public bool getIsFileEncrypted()
        {
            return isFileEncrypted;
        }
        public void setFileEncrypted(bool value)
        {
            isFileEncrypted = value;
        }

        public bool validatePaswword(string password)
        {                          
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
            amcw.WriteToConfigFile(passwordHash, passwordSalt);
        }

        public void changePassword(string oldPassword, string newPassword)
        {
            var correctPassword = validatePaswword(oldPassword);
            if (!correctPassword)
            {
                MessageBox.Show("Old Password is not correct!");
                return;
            }
            else
            {
                setPassword(newPassword);
            }
        }
    }


}