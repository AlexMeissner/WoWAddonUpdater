using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WoWAddonUpdater.Command;

namespace WoWAddonUpdater.ViewModels
{
    public class AddonViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string DirectoryPath { get; set; }
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
            InstalledVersionDate = DateTime.Now;
            Logger.Message("TODO: Update Addon");
        }
    }
}