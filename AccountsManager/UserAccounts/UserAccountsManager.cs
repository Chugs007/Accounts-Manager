using AccountsManager.Users.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsManager.Users
{
    public sealed class UserAccountsManager
    {
        private IList<UserAccount> userAccounts;
        private IUserAccountsFileParser amfp;
        private IUserAccountsWriter amfw;
        private static string filePath;
        private static Lazy<UserAccountsManager> lazy = new Lazy<UserAccountsManager>(() => new UserAccountsManager(filePath));

        private UserAccountsManager(string filePath)
        {
            userAccounts = new ObservableCollection<UserAccount>();
            amfp = new UserAccountsrFileParser(filePath);
            amfw = new UserAccountsFileWriter(filePath);
        }

        public static UserAccountsManager getInstance(string filePath = "")
        {
            UserAccountsManager.filePath = filePath;
            return lazy.Value;
        }

        public UserAccount CurrentUserAccount { get; set; }

        private void sortUserAccounts()
        {
            userAccounts = new ObservableCollection<UserAccount>(userAccounts.OrderBy(x => x.Domain).ToList());
        }

        public IList<UserAccount> getUserAccounts()
        {
            
            userAccounts = amfp.ParseFile();                      
            return userAccounts;
        }

        public UserAccount getUserAccount(UserAccount account)
        {
            return userAccounts.First(x => x.Domain == account.Domain && x.UserName == account.UserName && x.Password == account.Password);
        }

        public void addUserAccount(string user, string password, string domain)
        {
            if (userAccounts.Any(x => x.Domain == domain && x.UserName == user && x.Password == password))
            {
                System.Windows.MessageBox.Show("Identical information for domain: " + domain + ", user: " + user + ", password: " + password +
                   ", already exists.");
                return;
            }
            UserAccount ua = new UserAccount() { UserName = user, Password = password, Domain = domain };
            userAccounts.Add(ua);
            sortUserAccounts();
            amfw.WriteAccountsToFile(userAccounts.ToList());            
        }

        public void deleteUserAccount(UserAccount account)
        {
            userAccounts.Remove(account);
            amfw.WriteAccountsToFile(userAccounts.ToList());            
        }

        public void editUserAccount(UserAccount account, string user, string password, string domain)
        {
            UserAccount ua = userAccounts.First(x => x == account);
            ua.UserName = user;
            ua.Password = password;
            ua.Domain = domain;
            amfw.WriteAccountsToFile(userAccounts.ToList());            
        }

        public UserAccount searchForUserAccount(string text)
        {
            return userAccounts.ToList().Find(x => x.Domain.ToLower().Contains(text.ToLower()));

        }
    }
}
