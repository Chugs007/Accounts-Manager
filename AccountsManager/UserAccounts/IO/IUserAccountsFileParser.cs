using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsManager.Users.IO
{ 
    interface IUserAccountsFileParser
    {
        IList<UserAccount> ParseFile();
    }
}
