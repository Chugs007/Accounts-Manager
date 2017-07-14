using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Threading;

namespace AccountsManager
{    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {                
        private string password;
        private byte[] salt;        
        private ObservableCollection<UserAccount> UserAccounts;
        private UserAccount currentUserAccount;
        private FileEncryptor fe;
       
        public MainWindow()
        {
            SplashScreen sc = new SplashScreen(@"Resources\acctsmgrsplash.png");
            sc.Show(false);
            sc.Close(TimeSpan.FromSeconds(3));
            System.Threading.Thread.Sleep(3000);
            sc = null;
            InitializeComponent();                      
            UserAccounts = new ObservableCollection<UserAccount>();
            this.DataContext = this;
            listboxuseraccounts.ItemsSource = UserAccounts;
        }

        private void btnClickEncrypt(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtFilePath.Text))
            {
                System.Windows.MessageBox.Show("Please select a file first.");
                return;
            }
            //FileEncryptor.ParseXmlFile(txtFilePath.Text);
            string file = txtFilePath.Text;
            password = txtBxPassword.Password;         
            if (FileEncryptor.FirstTime)
                salt = FileEncryptor.CreateSalt(10);
            salt = Convert.FromBase64String(FileEncryptor.Salt);
            if (!string.IsNullOrEmpty(FileEncryptor.PasswordHash))
            {
                bool CorrectPassword = FileEncryptor.ValidatePassword(password, salt);
                if (!CorrectPassword)
                {
                    System.Windows.MessageBox.Show("Password is not correct!");
                    return;
                }
                if (FileEncryptor.IsEncrypted)
                {
                    System.Windows.MessageBox.Show("File is already encrypted.");
                    return;
                }
            }
            //Task.Factory.StartNew(() => FileEncryptor.Encrypt(file, password, salt));
            FileEncryptor.Encrypt(file, password, salt);
            UserAccounts.Clear();         
        }

        private void btnClickDecrypt(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtFilePath.Text))
            {
                System.Windows.MessageBox.Show("Please select a file first.");
                return;
            }
            if (!FileEncryptor.IsEncrypted)
            {
                System.Windows.MessageBox.Show("File is already decrypted.");
                return;
            }            
            if (!string.IsNullOrEmpty(FileEncryptor.PasswordHash))
            {
                password = txtBxPassword.Password;
                salt = Convert.FromBase64String(FileEncryptor.Salt);
                bool CorrectPassword = FileEncryptor.ValidatePassword(password, salt);
                if (!CorrectPassword)
                {
                    System.Windows.MessageBox.Show("Password is not correct!");
                    return;
                }
            }
            DecryptPassword();
            ReadXmlFile(txtFilePath.Text);
        }

        private void DecryptPassword()
        {
            string file = txtFilePath.Text;
            password = txtBxPassword.Password;        
            salt = Convert.FromBase64String(FileEncryptor.Salt);
            FileEncryptor.Decrypt(file, password, salt);
        }

        private void ReadXmlFile(string fileName)
        {
            // Open document           
            string filename =fileName;
            txtFilePath.Text = filename;
            UserAccounts.Clear();
            try
            {
                XmlDocument xmldocument = new XmlDocument();
                xmldocument.Load(filename);
                XmlNodeList nodelist = xmldocument.SelectNodes("/UserAccounts/UserAccount");
                foreach (XmlNode node in nodelist)
                {
                    UserAccount ua = new UserAccount();
                    ua.Domain = node["Domain"].InnerText;
                    ua.UserName = node["UserName"].InnerText;
                    ua.Password = node["Password"].InnerText;
                    UserAccounts.Add(ua);
                }

               UserAccounts= new ObservableCollection<UserAccount>(UserAccounts.OrderBy(x => x.Domain).ToList());                
               listboxuseraccounts.ItemsSource = null;
               listboxuseraccounts.ItemsSource = UserAccounts;
               FileEncryptor.IsEncrypted = false;          
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(XmlException))
                {
                    System.Windows.MessageBox.Show("File is encrypted please enter a password in password box and click decrypt, then refresh.");
                    FileEncryptor.IsEncrypted = true;
                    return;
                }             
            }
        }

        private void btnClickBrowse(object sender, RoutedEventArgs e)
        {            
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog() { DefaultExt = "xml",Filter= "XML Files|*.xml"};                                  
            lblLoading.Visibility = Visibility.Visible;
            if (ofd.ShowDialog() == true)
            {              
                fe = new FileEncryptor(ofd.FileName);        
                ReadXmlFile(ofd.FileName);
            }
            lblLoading.Visibility = Visibility.Hidden;                        
        }

        private void btnClickValidate(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtBxPassword.Password))
            {
                System.Windows.MessageBox.Show("Please enter a password first!");
                return;
            }
            if (string.IsNullOrEmpty(FileEncryptor.Salt))
            {
                System.Windows.MessageBox.Show("Selected file has not been previously encrypted before, please encrypt first before attempting to validate a password!");
                return;
            }
            string inputPassword = txtBxPassword.Password;
            salt = Convert.FromBase64String(FileEncryptor.Salt);
            bool CorrectPassword = FileEncryptor.ValidatePassword(inputPassword,salt);
            if (!CorrectPassword)
                System.Windows.MessageBox.Show("Password is not correct!");
            else
                System.Windows.MessageBox.Show("Password is correct!");
            
        }

        private void btnClickShowPassword(object sender, RoutedEventArgs e)
        {
            if (listboxuseraccounts.SelectedItem == null)
            {
                System.Windows.MessageBox.Show("Please select a user account first.");
                return;
            }
            UserAccount ua = listboxuseraccounts.SelectedItem as UserAccount;
            System.Windows.MessageBox.Show(ua.Password);
        }

        private void btnClickSearch(object sender, RoutedEventArgs e)
        {            
            SearchWindow sw = new SearchWindow();
            sw.UserAccounts = UserAccounts.ToList();
            sw.SearchForUserAccountEvent += sw_SearchForUserAccountEvent;
            sw.Show();
        }

        void sw_SearchForUserAccountEvent(UserAccount user)
        {
            listboxuseraccounts.SelectedItem = UserAccounts.First(x => x == user);
            listboxuseraccounts.ScrollIntoView(listboxuseraccounts.SelectedItem);
        }

        private void btnClickRefresh(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtFilePath.Text))
            {
                System.Windows.MessageBox.Show("Please select a file first, then try again.");
                return;
            }
            ReadXmlFile(txtFilePath.Text);
        }

        private void btnClickAddUser(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtFilePath.Text))
            {
                System.Windows.MessageBox.Show("Please select a file first");
                return;
            }
            AccountCreationsWindow acw = new AccountCreationsWindow();
            acw.AddUserAccountEvent += acw_AddUserAccountEvent;
            acw.ShowDialog();        
        }

        private void acw_AddUserAccountEvent(string user, string password, string domain)
        {
            UserAccounts.Add(new UserAccount() { UserName = user, Password = password, Domain = domain });
            XmlDocument xmldocument = new XmlDocument();
            xmldocument.Load(txtFilePath.Text);
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
            xmldocument.Save(txtFilePath.Text);
            ReadXmlFile(txtFilePath.Text);
        }

        private void btnClickChangePassword(object sender, RoutedEventArgs e)
        {
            ChangePasswordWindow cpw = new ChangePasswordWindow();
            cpw.Show();           
        }

        private void btnClickDelete(object sender, RoutedEventArgs e)
        {       
            if (string.IsNullOrEmpty(txtFilePath.Text))
            {
                System.Windows.MessageBox.Show("Please select a file first");
                return;
            }
            if (listboxuseraccounts.SelectedItem == null)
            {
                System.Windows.MessageBox.Show("Please select a user account first.");
                return;
            }
            try
            {
                UserAccount accountToBeDeleted = listboxuseraccounts.SelectedItem as UserAccount;
                UserAccounts.Remove(accountToBeDeleted);
                WriteToXmlFile();
                ReadXmlFile(txtFilePath.Text);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return;
            }           
        }             

        private void WriteToXmlFile()
        {
            XmlDocument xmldocument = new XmlDocument();
            xmldocument.Load(this.txtFilePath.Text);
            XmlNode root = xmldocument.DocumentElement;
            root.RemoveAll();
            foreach (var UserAccount in UserAccounts)
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
            xmldocument.Save(this.txtFilePath.Text);            
        }

        private void btnClickChangeAccountInfo(object sender, RoutedEventArgs e)
        {
            currentUserAccount = listboxuseraccounts.SelectedItem as UserAccount;
            if (currentUserAccount == null)
            {
                System.Windows.MessageBox.Show("Please select an account first.");
                return;
            }
            ChangeAccountWindow caw = new ChangeAccountWindow();
            caw.ChangeAccountInfoEvent += caw_ChangeAccountInfoEvent;
            caw.CurrentUserAccount = currentUserAccount;
            caw.Show();
        }

        private void caw_ChangeAccountInfoEvent(string domain, string username, string password)
        {          
            UserAccount ua = UserAccounts.First(x => x == currentUserAccount);
            ua.Domain = domain;
            ua.UserName = username;
            ua.Password = password;
            listboxuseraccounts.ItemsSource = null;
            listboxuseraccounts.ItemsSource = UserAccounts;
            WriteToXmlFile();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Please specify the directory in which you wish to create new accounts manager file.");
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string directoryName = fbd.SelectedPath;
                File.Copy(Directory.GetCurrentDirectory() + @"\AccountsManagerUsers.xml", directoryName + @"\AccountsManagerUsers.xml");
                File.Copy(Directory.GetCurrentDirectory() + @"\AccountsManager.xml", directoryName + @"\AccountsManager.xml");
            }
        }
    }
}
