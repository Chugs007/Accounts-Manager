using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsManager.MasterAccount.IO
{
    interface IAccountsConfigParser
    {
        (string passwordHash, string salt) ParseConfigFile();
    }
}
