using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsManager
{
    interface IFileParser
    {
        IList<UserAccount> ParseFile();
    }
}
