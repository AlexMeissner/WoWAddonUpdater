using System.Windows.Media;

namespace WoWAddonUpdater.ViewModels
{
    public class AddonViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string DirectoryPath { get; set; }
        public string Icon { get; set; }
        public bool Blacklisted { get; set; }
        public Brush BackgroundBrush => Blacklisted ? Brushes.Red : Brushes.Green;
    }
}