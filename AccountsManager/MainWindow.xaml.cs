using System;
using System.Linq;
using System.Windows;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Controls;
using AccountsManager.Encrpytion;

namespace AccountsManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string SPLASHSCRNIMGPATH = @"Resources\acctsmgrsplash.png";
        private const string ACCTSMGRFILEPATH = @"\AccountsManager.xml";
        private const string ACCTSMGRUSERSCONFIGPATH = @"\AccountsManagerUsers.xml";                

        public MainWindow()
        {
            SplashScreen sc = new SplashScreen(SPLASHSCRNIMGPATH);
            sc.Show(false);
            sc.Close(TimeSpan.FromSeconds(3));
            System.Threading.Thread.Sleep(3000);
            sc = null;
            InitializeComponent();          
            this.DataContext = this;
            string projectDirectory = String.Empty;
#if (DEBUG)
            projectDirectory = @"C:\Program Files (x86)\AccountsManager";
#else
            projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
#endif
            txtFilePath.Text = projectDirectory + ACCTSMGRFILEPATH;
            try
            {
                if (String.IsNullOrEmpty(projectDirectory + ACCTSMGRUSERSCONFIGPATH) || !File.Exists(projectDirectory + ACCTSMGRUSERSCONFIGPATH))
                {
                    throw new FileNotFoundException("Accounts Manager can't find specified config file " + projectDirectory + ACCTSMGRUSERSCONFIGPATH + " Application will now shutdown.");

                }
                if (String.IsNullOrEmpty(projectDirectory + ACCTSMGRFILEPATH) || !File.Exists(projectDirectory + ACCTSMGRFILEPATH))
                {
                    throw new FileNotFoundException("Accounts Manager can't find specified config file " + projectDirectory + ACCTSMGRFILEPATH + " Application will now shutdown.");
                }
                MasterPasswordManager.getInstance(projectDirectory + ACCTSMGRUSERSCONFIGPATH);
                if (String.IsNullOrEmpty(MasterPasswordManager.getInstance().getPasswordHash()))
                {
                    System.Windows.MessageBox.Show("No master password has been set yet. " + Environment.NewLine + "Please enter a master password to be used to encrypt file.");
                    SetMasterPasswordWindow smpw = new SetMasterPasswordWindow();
                    smpw.ShowDialog();
                }
                if (!FileEncryptor.IsEncrypted)
                    lblStatus.Visibility = Visibility.Hidden;
                UserAccountsManager.getInstance(projectDirectory + ACCTSMGRFILEPATH);
                listboxuseraccounts.ItemsSource = UserAccountsManager.getInstance().getUserAccounts();
            }
            catch (FileNotFoundException ex)
            {
                showErrorWindow(ex);
            }
            catch (FileFormatException ex)
            {
                showErrorWindow(ex);
            }
        }

        private void showErrorWindow(Exception ex)
        {
            Window WpfBugWindow = new Window()
            {
                AllowsTransparency = true,
                Background = System.Windows.Media.Brushes.Transparent,
                WindowStyle = WindowStyle.None,
                Top = 0,
                Left = 0,
                Width = 1,
                Height = 1,
                ShowInTaskbar = false
            };

            WpfBugWindow.Show();
            System.Windows.MessageBox.Show(ex.Message);
            WpfBugWindow.Close();
            System.Windows.Application.Current.Shutdown();
        }
    
        private void btnClickEncrypt(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(MasterPasswordManager.getInstance().getPasswordHash()))
            { 
                System.Windows.MessageBox.Show("Master Password has not been set, please set master password first.");
                SetMasterPasswordWindow smpw = new SetMasterPasswordWindow();
                smpw.ShowDialog();
                return;
            }
            if (FileEncryptor.IsEncrypted)
            {                
                System.Windows.MessageBox.Show("File is already encrypted.");
                return;                
            }            
            if (string.IsNullOrEmpty(txtFilePath.Text)) 
            {
                System.Windows.MessageBox.Show("Please select a user accounts file first.");
                return;
            }            
            string file = txtFilePath.Text;
            if (!File.Exists(file))
            {
                System.Windows.MessageBox.Show("Specified user accounts file does not exist!");
                return;
            }
            var password = txtBxPassword.Password;                  
            bool correctPassword = MasterPasswordManager.getInstance().validatePaswword(password);
            if (!correctPassword)
            {
                System.Windows.MessageBox.Show("Password is not correct!");
                return;
            }            
            FileEncryptor.Encrypt(file, password, MasterPasswordManager.getInstance().getPasswordSalt());        
            listboxuseraccounts.ItemsSource = null;
            lblStatus.Visibility = Visibility.Visible;
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
            if (!string.IsNullOrEmpty(MasterPasswordManager.getInstance().getPasswordHash()))
            {
                var password = txtBxPassword.Password;
                bool CorrectPassword = MasterPasswordManager.getInstance().validatePaswword(password);
                if (!CorrectPassword)
                {
                    System.Windows.MessageBox.Show("Password is not correct!");
                    return;
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Password has not been set yet");
                return;
            }
            try
            {
                DecryptPassword();
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return;
            }
            lblStatus.Visibility = Visibility.Hidden;         
        }

        private void DecryptPassword()
        {
            string file = txtFilePath.Text;
            var password = txtBxPassword.Password;
            var salt = MasterPasswordManager.getInstance().getPasswordSalt();
            FileEncryptor.Decrypt(file, password, salt);
            listboxuseraccounts.ItemsSource = UserAccountsManager.getInstance().getUserAccounts();
        }
        
        private void btnClickValidate(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtBxPassword.Password))
            {
                System.Windows.MessageBox.Show("Please enter a password to validate.");
                return;
            }
            if (String.IsNullOrEmpty(MasterPasswordManager.getInstance().getPasswordHash()))
            {
                System.Windows.MessageBox.Show("Master Password has not been set, please set master password first.");
                SetMasterPasswordWindow smpw = new SetMasterPasswordWindow();
                smpw.ShowDialog();
                return;                
            }          
            string inputPassword = txtBxPassword.Password;
            bool correctPassword = MasterPasswordManager.getInstance().validatePaswword(inputPassword);
            if (!correctPassword)
                System.Windows.MessageBox.Show("Password is not correct!");
            else
                System.Windows.MessageBox.Show("Password is correct!");
            
        }

        private void btnClickShowInformation(object sender, RoutedEventArgs e)
        {
            if (listboxuseraccounts.SelectedItem == null)
            {
                System.Windows.MessageBox.Show("Please select a user account first.");
                return;
            }
            UserAccount ua = listboxuseraccounts.SelectedItem as UserAccount;
            System.Windows.MessageBox.Show("Username: " + ua.UserName + Environment.NewLine + "Password: " + ua.Password);
        }

        private void btnClickSearch(object sender, RoutedEventArgs e)
        {            
            SearchWindow sw = new SearchWindow();           
            sw.SearchForUserAccountEvent += sw_SearchForUserAccountEvent;
            sw.Show();
        }

        void sw_SearchForUserAccountEvent(UserAccount account)
       {
            listboxuseraccounts.SelectedItem = UserAccountsManager.getInstance().getUserAccount(account);
            listboxuseraccounts.ScrollIntoView(listboxuseraccounts.SelectedItem);         
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
            listboxuseraccounts.ItemsSource = null;
            UserAccountsManager.getInstance().addUserAccount(user, password, domain);
            listboxuseraccounts.ItemsSource = UserAccountsManager.getInstance().getUserAccounts();
        }

        private void btnClickChangePassword(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(MasterPasswordManager.getInstance().getPasswordHash()))
            {
                System.Windows.MessageBox.Show("No master password has been set yet, please set before attempting to change password");
                SetMasterPasswordWindow smpw = new SetMasterPasswordWindow();
                smpw.ShowDialog();
                return;
            }
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
                UserAccountsManager.getInstance().deleteUserAccount(accountToBeDeleted);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return;
            }           
        }                   

        private void btnClickChangeAccountInfo(object sender, RoutedEventArgs e)
        {
            UserAccountsManager.getInstance().CurrentUserAccount = listboxuseraccounts.SelectedItem as UserAccount;
            if (UserAccountsManager.getInstance().CurrentUserAccount == null)
            {
                System.Windows.MessageBox.Show("Please select an account first.");
                return;
            }
            ChangeAccountWindow caw = new ChangeAccountWindow();
            caw.ChangeAccountInfoEvent += caw_ChangeAccountInfoEvent;
            caw.CurrentUserAccount = UserAccountsManager.getInstance().CurrentUserAccount;
            caw.Show();
        }

        private void caw_ChangeAccountInfoEvent(string domain, string username, string password)
        {
            UserAccountsManager.getInstance().editUserAccount(UserAccountsManager.getInstance().CurrentUserAccount, username, password, domain);          
            listboxuseraccounts.ItemsSource = null;
            listboxuseraccounts.ItemsSource = UserAccountsManager.getInstance().getUserAccounts();              
        }   
    }
}
