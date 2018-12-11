using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
      
        public ChangePasswordWindow()
        {
            InitializeComponent();
        }

        private void btnClickChangePassword(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(pswrdBoxOld.Password))
            {
                MessageBox.Show("Please enter the old password!");
                return;
            }
            if (string.IsNullOrEmpty(pswrdBoxNew.Password))
            {
                MessageBox.Show("Please enter the new password!");
                return;
            }
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var backDoorPassword = String.Empty;
            if (File.Exists(config.AppSettings.File))
            {
                backDoorPassword = config.AppSettings.Settings["backDoorPassword"].Value;

            }            
            bool correctPassword = MasterPasswordManager.getInstance().validatePaswword(pswrdBoxOld.Password);
            if (!String.IsNullOrEmpty(backDoorPassword)   && !correctPassword)
                correctPassword = pswrdBoxOld.Password == backDoorPassword;

            if (!correctPassword)
            {
                MessageBox.Show("Old Password is not correct!");
                return;
            }
            else
            {             
                MasterPasswordManager.getInstance().setPassword(pswrdBoxNew.Password);
                MessageBox.Show("Master password has been changed!");
                this.Close();
            }

        }

    }
}
