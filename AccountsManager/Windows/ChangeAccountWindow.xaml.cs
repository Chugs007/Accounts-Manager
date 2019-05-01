using AccountsManager.Users;
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
    /// Interaction logic for ChangeAccountWindow.xaml
    /// </summary>
    public partial class ChangeAccountWindow : Window
    {
        public delegate void ChangeAccountInfo(string domain, string username, string password);
        public event ChangeAccountInfo ChangeAccountInfoEvent;

        public ChangeAccountWindow()
        {
            InitializeComponent();
        }

        public UserAccount CurrentUserAccount
        {
            get;
            set;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ChangeAccountInfoEvent?.Invoke(txtBoxDomain.Text, txtBoxUserName.Text, txtBoxPassword.Text);
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.txtBoxDomain.Text = CurrentUserAccount.Domain;
            this.txtBoxUserName.Text = CurrentUserAccount.UserName;
            this.txtBoxPassword.Text = CurrentUserAccount.Password;
        }
    }
}
