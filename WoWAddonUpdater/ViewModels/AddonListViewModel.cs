using System.Windows.Input;
using System.Collections.ObjectModel;
using WoWAddonUpdater.Command;
using WoWAddonUpdater.Functions;

namespace WoWAddonUpdater.ViewModels
{
    public class AddonListViewModel : BaseViewModel
    {
        public ObservableCollection<AddonViewModel> Addons { get; set; }

        public ICommand RefreshCommand { get; set; }

        public string Name { get; set; } = "AddonlistViewModel";

        public AddonListViewModel()
        {
            RefreshCommand = new RelayCommand(OnRefresh);
        }

        private void OnRefresh()
        {
            AddonUpdater updater = new AddonUpdater();
            Addons = updater.GetViewModel();
        }
    }
}