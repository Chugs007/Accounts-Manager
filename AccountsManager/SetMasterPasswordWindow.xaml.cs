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
    /// Interaction logic for SetMasterPasswordWindow.xaml
    /// </summary>
    public partial class SetMasterPasswordWindow : Window
    {
        public SetMasterPasswordWindow()
        {
            InitializeComponent();
        }


        private bool MasterPasswordSet = false;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String inputPassword = txtBoxPassword.Text;
            if (String.IsNullOrEmpty(inputPassword))
            {
                MessageBox.Show("Please enter a valid password!");
                return;
            }
            byte[] salt = FileEncryptor.CreateSalt(10);
            salt = Convert.FromBase64String(FileEncryptor.Salt);
            FileEncryptor.DES = FileEncryptor.CreateDES(inputPassword, salt);
            FileEncryptor.SetPassword(Convert.ToBase64String(FileEncryptor.DES.Key));
            MasterPasswordSet = true;
            this.Close();
            MessageBox.Show("Master Password has been set!");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (!MasterPasswordSet)
            {
                MessageBoxResult mbr= MessageBox.Show("Master password must be set before using application. " +
                    "Select yes if you wish to quit, otherwise no to input a master password.", "", MessageBoxButton.YesNo);
                if (mbr == MessageBoxResult.Yes)
                {
                    return;
                }
                else
                {
                    SetMasterPasswordWindow smpw = new SetMasterPasswordWindow();
                    smpw.ShowDialog();
                }
            }
        }
    }
}
