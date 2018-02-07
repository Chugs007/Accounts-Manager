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
        private static FileStream fsCrypt;
        private static CryptoStream cs;
        private static FileStream fsIn;     

        public FileEncryptor()
        {
           
        }

        public static bool IsEncrypted
        {
            get;
            set;
        }

        public static RijndaelManaged DES
        {
            get;
            set;
        }
             
        //salt should only be created when password is created or changed, store it somewhere, and use it to generate hash when validating password.
        public static string CreateSalt(int size)
        {            
            var rng = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[size];
            
            rng.GetBytes(buffer);
            var salt = Convert.ToBase64String(buffer);
            return salt;
        }

        public static string CreateHash(string password,string passwordSalt)
        {
            var salt = Convert.FromBase64String(passwordSalt);            
            DES = FileEncryptor.CreateDES(password, salt);
            var hash = Convert.ToBase64String(FileEncryptor.DES.Key);
            return hash;
        }
      
        public static void Encrypt(string file, string password, string salt)
        {
            var saltValue = Convert.FromBase64String(MasterPasswordManager.getInstance().getPasswordSalt());
            EncryptFile(file,password,saltValue);
        }

        public static void Decrypt(string file, string password,string salt)
        {
            var saltValue = Convert.FromBase64String(salt);
            DecryptFile(file,password,saltValue);
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
