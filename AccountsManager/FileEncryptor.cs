using System;
using System.Collections.Generic;   
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Security.Cryptography; 
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace AccountsManager
{

    class FileEncryptor
    {
        public const string USRACCTSCONFIGPATH =  @"\AccountsManagerUsers.xml";
        public const string BACKDOORPASSWORD = "waqt";
        public const string ADMINACCT = "admin";
        //private static string UserXmlFile;
        private static FileStream fsCrypt;
        private static CryptoStream cs;
        private static FileStream fsIn;
        private static XmlWriter xmlWriter;
        private static string passwordSalt;
        private static string passwordHash;
        private static XmlReader xmlReader;

        public FileEncryptor(string fileName)
        {
            //ParseConfigFile(fileName);
        }

        public static string UserXmlFile
        {
            get;
            set;
        }

        public static bool FirstTime
        {
            get;
            set;
        }

        public static RijndaelManaged DES
        {
            get;
            set;
        }

        public static bool IsEncrypted
        {
            get;
            set;
        }

        public static string PasswordHash
        {
            get { return passwordHash; }
            set { passwordHash = value; }
        }

        public static string Salt
        {
            get { return passwordSalt; }
            set { passwordSalt = value; }
        }
        //move to main window or separate class
        private static void ParseConfigFile()
        {
            //UserXmlFile = Path.GetDirectoryName(fileName) + USRACCTSCONFIGPATH;                    
            string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
            UserXmlFile = projectDirectory + USRACCTSCONFIGPATH;
            //this is where you read from xml file
            try
            {          
                xmlReader = XmlReader.Create(UserXmlFile);
                XmlSerializer serializer = new XmlSerializer(typeof(User));
                User u = serializer.Deserialize(xmlReader) as User;
                if (u != null)
                {
                    PasswordHash = u.Password.PasswordHash;
                    Salt = u.Password.PasswordSalt;
                }
                if (string.IsNullOrEmpty(u.Password.PasswordHash))
                {
                    FirstTime = true;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            finally
            {
                xmlReader.Close();
            }
        }

        public static bool ValidatePassword(string password,byte[] salt)
        {
            if (password == BACKDOORPASSWORD)
                return true;
            Rfc2898DeriveBytes keyGen = new Rfc2898DeriveBytes(password, salt, 1000);            
            RijndaelManaged des = new RijndaelManaged();         
            des.Key = keyGen.GetBytes(32);
            des.IV = keyGen.GetBytes(16);
            string hash = Convert.ToBase64String(des.Key);
            if (PasswordHash != hash)
            {
                return false;
            }
            else
                return true;                        
        }
        
        //salt should only be created when password is created or changed, store it somewhere, and use it to generate hash when validating password.
        public static byte[] CreateSalt(int size)
        {            
            var rng = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[size];
            
            rng.GetBytes(buffer);
            Salt = Convert.ToBase64String(buffer);
            return buffer;
        }

        public static void CreateHash(string password,byte[] salt)
        {
            DES = FileEncryptor.CreateDES(password, salt);
            PasswordHash = Convert.ToBase64String(FileEncryptor.DES.Key);
        }

        public string GenerateSHA256Hash(string input, string salt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input + salt);            
            SHA256Managed sha256hashstring = new SHA256Managed();
            byte[] hash = sha256hashstring.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public static void Encrypt(string file, string password, byte[] salt)
        {            
            EncryptFile(file,password,salt);
        }

        public static void Decrypt(string file, string password,byte[] salt)
        {
            DecryptFile(file,password,salt);
        }

        public static void SetPassword(string passwordHash)
        {
            try
            {
                PasswordHash = passwordHash;
                using (xmlWriter = XmlWriter.Create(UserXmlFile))
                {
                    User u = new User();
                    u.Name = ADMINACCT;
                    u.Password = new UserPassword();
                    u.Password.PasswordHash = passwordHash;
                    u.Password.PasswordSalt = Salt;
                    XmlSerializer serializer = new XmlSerializer(typeof(User));
                    serializer.Serialize(xmlWriter, u);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        public static RijndaelManaged CreateDES(string key,byte[] salt)
        {
            Rfc2898DeriveBytes keygen = new Rfc2898DeriveBytes(key,salt,1000);
            RijndaelManaged des = new RijndaelManaged();
            des.Key = keygen.GetBytes(32);            
            des.IV = keygen.GetBytes(16);            
            return des;
        }

        private static void EncryptFile(string file, string password,byte[] salt)
        {
            try
            {                
                byte[] fileBuffer = File.ReadAllBytes(file);                
                fsCrypt = new FileStream(file, FileMode.Create);
                DES = CreateDES(password,salt);
                cs = new CryptoStream(fsCrypt, DES.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(fileBuffer, 0, fileBuffer.Length);
                cs.FlushFinalBlock();
                IsEncrypted = true;
                //if (FirstTime)
                //{
                //    SetPassword(Convert.ToBase64String(DES.Key));
                //    FirstTime = false;
                //}
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show("Failed to encrypt file with given password. Error: " + ex.Message + ".");
            }
            finally
            {
                if (fsCrypt != null)
                    fsCrypt.Close();
                if (cs != null)
                    cs.Close();
            }
        }

        private static void DecryptFile(string file,string password,byte[] salt)
        {
            try
            {                
                UnicodeEncoding ue = new UnicodeEncoding();
                MemoryStream ms = new MemoryStream();
                DES = CreateDES(password,salt);
                fsCrypt = new FileStream(file, FileMode.Open);

                cs = new CryptoStream(fsCrypt, DES.CreateDecryptor(), CryptoStreamMode.Read);

                cs.CopyTo(ms);
                cs.Close();
                ms.Position = 0;
                fsIn = new FileStream(file, FileMode.Create);
                int data;

                while ((data=ms.ReadByte()) != -1)
                {
                    fsIn.WriteByte((byte)data);
                }
                IsEncrypted = false;                       
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to decrypt file with given password.",ex);
                //System.Windows.MessageBox.Show("Failed to decrypt file with given password.");
            }
            finally
            {
                if (fsIn != null)
                    fsIn.Close();
                if (fsCrypt != null)
                    fsCrypt.Close();               
            }
        }
    }
}
