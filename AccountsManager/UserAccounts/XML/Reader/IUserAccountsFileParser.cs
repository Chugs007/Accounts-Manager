using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsManager.UserAccounts.XML.Reader
{
    interface IUserAccountsFileParser
    {
        IList<UserAccount> ParseFile();
    }
}
