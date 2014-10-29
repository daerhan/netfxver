using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using Microsoft.Win32;

namespace netfxver
{
    public class FrameworkVersion
    {
        #region Constants

        private const String DotNetFxSetupKey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\";
        private const String VersionKeyNameRegex = @"^v[1-9](\.[0-9]*)*$";

        // Registry keys applying to specific framework versions
        private const String VersionValueName = "Version";
        private const String InstallValueName = "Install";
        private const String ServicePackValueName = "SP";
        private const String InstallPathValueName = "InstallPath";
        private const String ReleaseValueName = "Release";

        // Release versions for .Net 4.5+ Framework 
        private const Int32 Release4_5_2 = 379893;
        private const Int32 Release4_5_1 = 378675;
        private const Int32 Release4_5_0 = 378389;

        #endregion

        #region Properties

        public String FriendlyName { get; set; }

        public String KeyName { get; set; }

        public String Version { get; set; }

        public Int32 ServicePack { get; set; }

        public Boolean Installed { get; set; }

        public String InstallPath { get; set; }

        public Int32? Release { get; set; }

        public Boolean HasRelease { get { return Release.HasValue; } }

        #endregion

        public static IEnumerable<FrameworkVersion> GetInstalledFrameworkVersions()
        {
            var result = new List<FrameworkVersion>();
            using (RegistryKey netfxkey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "")
                                                     .OpenSubKey(DotNetFxSetupKey))
            {
                foreach (string versionKeyName in netfxkey.GetSubKeyNames())
                {
                    if (IsFrameworkVersionKey(versionKeyName))
                    {
                        using (RegistryKey currentKey = netfxkey.OpenSubKey(versionKeyName))
                        {
                            // Either the key represents a single version
                            if (HasRequiredVersionInfo(currentKey))
                                result.Add(ConvertKeyToFrameworkVersion(currentKey));
                            else // Or it has multiple sub-versions / profiles under it
                                result.AddRange(GetSubFrameworkVersions(currentKey));
                        }

                    }
                }
            }
            return result;
        }

        private static FrameworkVersion ConvertKeyToFrameworkVersion(RegistryKey targetVersionKey, RegistryKey baseVersionKey = null)
        {
            return new FrameworkVersion
            {
                FriendlyName = GetFriendlyVersionName(targetVersionKey, baseVersionKey),
                KeyName = targetVersionKey.Name,
                Version = targetVersionKey.GetValue(VersionValueName, null) as String,
                ServicePack = (Int32)targetVersionKey.GetValue(ServicePackValueName, 0),
                Installed = (Int32)targetVersionKey.GetValue(InstallValueName, 0) == 1,
                InstallPath = targetVersionKey.GetValue(InstallPathValueName, null) as String,
                Release = targetVersionKey.GetValue(ReleaseValueName, null) as Int32?
            };
        }

        private static String GetFriendlyVersionName(RegistryKey targetVersionKey, RegistryKey baseVersionKey)
        {
            if (HasReleaseKey(targetVersionKey))
            {
                Int32 release = Convert.ToInt32(targetVersionKey.GetValue(ReleaseValueName));

                // Release should match one of the following. If it doesn't default to the "standard" way of determining name.
                if (release >= Release4_5_2)
                    return String.Format("{0} 4.5.2 or later", targetVersionKey.GetLocalKeyName());
                if (release >= Release4_5_1)
                    return String.Format("{0} 4.5.1 or later", targetVersionKey.GetLocalKeyName());
                if (release >= Release4_5_0)
                    return String.Format("{0} 4.5 or later", targetVersionKey.GetLocalKeyName());
            }
            return baseVersionKey != null
                ? String.Format("{0} {1}", baseVersionKey.GetLocalKeyName(), targetVersionKey.GetLocalKeyName())
                : targetVersionKey.GetLocalKeyName();
        }

        private static Boolean HasRequiredVersionInfo(RegistryKey targetVersionKey)
        {
            if (targetVersionKey == null || targetVersionKey.ValueCount < 2)
                return false;

            string[] values = targetVersionKey.GetValueNames();

            return values.Any(valueName => String.Equals(valueName, VersionValueName, StringComparison.InvariantCultureIgnoreCase))
                   && values.Any(valueName => String.Equals(valueName, InstallValueName, StringComparison.InvariantCultureIgnoreCase));
        }

        private static Boolean HasReleaseKey(RegistryKey targetKey)
        {
            return targetKey.GetValueNames()
                            .Any(valueName => String.Equals(valueName, 
                                                            ReleaseValueName, 
                                                            StringComparison.InvariantCultureIgnoreCase));
        }


        private static IEnumerable<FrameworkVersion> GetSubFrameworkVersions(RegistryKey targetKey)
        {
            var result = new List<FrameworkVersion>();
            foreach (var subKeyName in targetKey.GetSubKeyNames())
            {
                using (var subKey = targetKey.OpenSubKey(subKeyName))
                {
                    if(HasRequiredVersionInfo(subKey))
                        result.Add(ConvertKeyToFrameworkVersion(subKey, targetKey));
                }
            }
            return result;
        } 

        private static Boolean IsFrameworkVersionKey(String keyName)
        {
            return Regex.IsMatch(keyName, VersionKeyNameRegex);
        }
    }
}