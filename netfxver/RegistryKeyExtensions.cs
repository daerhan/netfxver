using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace netfxver
{
    public static class RegistryKeyExtensions
    {
        public static String GetLocalKeyName(this RegistryKey key)
        {
            return key.Name.Contains("\\")
                ? key.Name.Substring(key.Name.LastIndexOf("\\") + 1)
                : key.Name;
        }
    }
}
