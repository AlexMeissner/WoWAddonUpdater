using System.Windows.Input;
using System.Collections.ObjectModel;
using WoWAddonUpdater.Command;
using WoWAddonUpdater.Functions;

namespace WoWAddonUpdater.ViewModels
{
    public class AddonListViewModel : BaseViewModel
    {
        public ObservableCollection<AddonViewModel> Addons { get; set; } = new ObservableCollection<AddonViewModel>();

        public ICommand RefreshCommand { get; set; }

        public AddonListViewModel()
        {
            RefreshCommand = new RelayCommand(OnRefresh);
        }

        private void OnRefresh()
        {
            AddonUpdater updater = new AddonUpdater();

            var a = updater.GetViewModel();

            foreach(var b in a)
            {
                Addons.Add(b); 
            }
        }
    }
}