using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostInCryptic.Shared.Constants
{
    public static class Constants
    {
#if DEBUG
        public static string BaseUrl = "https://localhost:44382";
#else
        public static string BaseUrl = "https://cryptic.u-mod.club";
#endif
    }
}
