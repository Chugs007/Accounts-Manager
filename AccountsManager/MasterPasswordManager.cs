using System;
using System.Windows;

namespace AccountsManager
{
    public class MasterPasswordManager
    {
        public const string ADMINACCT = "admin";        
        private string passwordHash;
        private string passwordSalt;
        private static string configXmlFilePath;
        private AccountsManagerConfigFileParser amcp;
        private AccountsManagerConfigFileWriter amcw;        
        private static Lazy<MasterPasswordManager> instance = new Lazy<MasterPasswordManager>(( ) => new MasterPasswordManager(configXmlFilePath));

        private MasterPasswordManager(string configXmlFile)  
        {            
            configXmlFilePath = configXmlFile; 
            amcp = new AccountsManagerConfigFileParser(configXmlFile);
            amcw = new AccountsManagerConfigFileWriter(configXmlFile);            
            var configFileData = amcp.parseFile();                                        
            passwordHash = configFileData.passwordHash;
            passwordSalt = configFileData.salt;            

        }

        public static MasterPasswordManager getInstance(string filePath= "")
        {
            MasterPasswordManager.configXmlFilePath = filePath;
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
            amcw.WriteToFile(passwordHash, passwordSalt);
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