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
    /// Interaction logic for SearchWindow.xaml
    /// </summary>
    public partial class SearchWindow : Window
    {
        public delegate void SearchForUserAccount(UserAccount user);
        public event SearchForUserAccount SearchForUserAccountEvent;

        public SearchWindow()
        {
            InitializeComponent();
        }

        public List<UserAccount> UserAccounts
        {
            get;
            set;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtBoxSearch.Text))
            {
                System.Windows.MessageBox.Show("Please enter something to search for");
                return;
            }
            UserAccount ua = UserAccounts.Find(x => x.Domain.ToLower().Contains(txtBoxSearch.Text.ToLower()));
            if (ua == null)
            {
                System.Windows.MessageBox.Show("No matches found");
                return;
            }

            if (SearchForUserAccountEvent != null)
            {
                SearchForUserAccountEvent(ua);
                this.Close();
            }
        }
    }
}
