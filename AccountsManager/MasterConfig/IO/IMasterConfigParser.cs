using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsManager.MasterConfig.IO
{
    interface IMasterConfigParser
    {
        (string passwordHash, string salt,bool isFileEncrypted) ParseConfigFile();
    }
}
