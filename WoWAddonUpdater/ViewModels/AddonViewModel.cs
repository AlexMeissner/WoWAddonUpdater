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
        public Visibility UpdateButtonVisibility => IsUpToDate ? Visibility.Hidden : Visibility.Visible;
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

        public Brush BackgroundBrush
        {
            get
            {
                if (Blacklisted)
                {
                    return Brushes.Black;
                }
                else if (IsUpToDate)
                {
                    return Brushes.DarkGreen;
                }
                else
                {
                    return Brushes.DarkRed;
                }
            }
        }

        public ICommand DownloadCommand { get; set; }

        public AddonViewModel()
        {
            DownloadCommand = new RelayCommand(Update);
        }

        private void Update()
        {
            System.Console.WriteLine("TODO: Update Addon");
        }
    }
}