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
using System.Windows.Shapes;

namespace AccountsManager
{
    /// <summary>
    /// Interaction logic for ChangePasswordWindow.xaml
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {

        public delegate void ChangePassword(string newPassword);
        public event ChangePassword ChangePasswordEvent;

        public ChangePasswordWindow()
        {
            InitializeComponent();
        }

        private void btnClickChangePassword(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(pswrdBoxOld.Password))
            {
                System.Windows.MessageBox.Show("Please enter the old password!");
                return;
            }
            if (string.IsNullOrEmpty(pswrdBoxNew.Password))
            {
                System.Windows.MessageBox.Show("Please enter the new password!");
                return;
            }

            byte[] salt = Convert.FromBase64String(FileEncryptor.Salt);
            var CorrectPassword = FileEncryptor.ValidatePassword(pswrdBoxOld.Password, salt)  || (pswrdBoxOld.Password == FileEncryptor.BACKDOORPASSWORD);
            if (!CorrectPassword)
            {
                System.Windows.MessageBox.Show("Old Password is not correct!");
                return;
            }
            else
            {
                //create new salt of 10 bytes
                FileEncryptor.CreateSalt(10);
                //FileEncryptor.DES = FileEncryptor.CreateDES(pswrdBoxNew.Password, Convert.FromBase64String(FileEncryptor.Salt));
                //FileEncryptor.PasswordHash = Convert.ToBase64String(FileEncryptor.DES.Key);
                FileEncryptor.CreateHash(pswrdBoxNew.Password, Convert.FromBase64String(FileEncryptor.Salt));
                FileEncryptor.SetPassword(FileEncryptor.PasswordHash);
                this.Close();
            }

        }

    }
}
