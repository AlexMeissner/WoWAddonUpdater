using System;

namespace WoWAddonUpdater.ViewModels
{
    [Serializable]
    public class ViewModelData
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public string DownloadUrl { get; set; }
        public bool Blacklisted { get; set; }
        public uint CurseID { get; set; }
        public DateTime InstalledVersionDate { get; set; }
        public DateTime AvailableVersionDate { get; set; }
    }
}