using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsManager.UserAccounts
{
    public class UserAccount
    {
        public string UserName
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

        public string Domain
        {
            get;
            set;
        }

        public override string ToString()
        {
            return string.Format(" {0}", Domain);
        }
    }
}
