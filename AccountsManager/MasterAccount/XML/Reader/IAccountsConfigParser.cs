using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsManager.MasterAccount.XML.Reader
{
    interface IAccountsConfigParser
    {
        (string passwordHash, string salt) ParseConfigFile();
    }
}
