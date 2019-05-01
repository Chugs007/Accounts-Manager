using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsManager.MasterAccount.XML.Writer
{
    interface IAccountsConfigWriter
    {
        void WriteToConfigFile(string passwordHash, string salt);   
    }
}
