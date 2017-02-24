using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTorrentChecker
{
    interface IProvider
    {
        List<string> GetTorrents();
    }
}
