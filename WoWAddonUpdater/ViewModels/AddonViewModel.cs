using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WoWAddonUpdater.Command;
using WoWAddonUpdater.Functions;

namespace WoWAddonUpdater.ViewModels
{
    public class AddonViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public string DownloadUrl { get; set; }
        public bool Blacklisted { get; set; }
        public uint CurseID { get; set; }
        public DateTime InstalledVersionDate { get; set; }
        public DateTime AvailableVersionDate { get; set; }
        public Visibility UpdateButtonVisibility => IsUpToDate || Blacklisted ? Visibility.Hidden : Visibility.Visible;
        public Brush BackgroundBrush => Blacklisted ? Brushes.Black : (SolidColorBrush)(new BrushConverter().ConvertFrom("#222"));
        public bool IsUpToDate
        {
            get
            {
                if (InstalledVersionDate != null && AvailableVersionDate != null)
                {
                    return InstalledVersionDate >= AvailableVersionDate;
                }

                return false;
            }
        }

        public ICommand DownloadCommand { get; set; }

        public AddonViewModel()
        {
            DownloadCommand = new RelayCommand(Update);
        }

        private void Update()
        {
            AddonUpdater addonUpdater = new AddonUpdater();

            if (addonUpdater.UpdateAddon(DownloadUrl))
            {
                string updateMessage = string.Format("Updated {0} from {1} to {2}", Name, InstalledVersionDate.ToString(), AvailableVersionDate.ToString());
                Logger.Message(updateMessage);
                InstalledVersionDate = DateTime.Now;
            }
            else
            {
                string updateMessage = string.Format("Failed to update {0} from {1} to {2}", Name, InstalledVersionDate.ToString(), AvailableVersionDate.ToString());
                Logger.Message(updateMessage);
            }
        }
    }
}