using System;
using System.Text.RegularExpressions;

namespace WoWAddonUpdater.Functions
{
    public class FileVersion
    {
        public FileVersion(string version)
        {
            try
            {
                var subVersions = Regex.Replace(version, "[a-z]", string.Empty).Split('.');
                Major = int.Parse(subVersions[0]);
                Minor = int.Parse(subVersions[1]);
                Patch = int.Parse(subVersions[2]);
            }
            catch (Exception exception)
            {
                Logger.Exception(exception);
            }
        }

        public int Major { get; set; } = 0;
        public int Minor { get; set; } = 0;
        public int Patch { get; set; } = 0;
    }
}