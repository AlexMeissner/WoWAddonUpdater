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
        public bool Blacklisted { get; set; }
        public uint CurseID { get; set; }
        public Brush BackgroundBrush => Blacklisted ? Brushes.Black : Brushes.DarkGreen;

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