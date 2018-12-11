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
    /// Interaction logic for AccountCreationsWindow1.xaml
    /// </summary>
    public partial class AccountCreationsWindow : Window
    {

        public delegate void AddUserAccount(string user, string password, string domain);
        public event AddUserAccount AddUserAccountEvent;

        public AccountCreationsWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtBoxUserName.Text) && string.IsNullOrEmpty(txtBoxPassword.Text) && string.IsNullOrEmpty(txtBoxDomain.Text))
            {
                MessageBox.Show("Please make sure all fields have been filled before attempting to add");
                return;
            }
            if (AddUserAccountEvent != null)
            {
                AddUserAccountEvent?.Invoke(txtBoxUserName.Text, txtBoxPassword.Text, txtBoxDomain.Text);
            }
            this.Close();
        }
    }
}
