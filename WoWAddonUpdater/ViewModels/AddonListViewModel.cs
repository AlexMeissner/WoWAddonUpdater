using System;
using System.Linq;
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
            try
            {
                var updatedAddons = new AddonUpdater().GetViewModel();

                foreach (var updatedAddon in updatedAddons)
                {
                    var match = Addons.Where(x => x.Name.Equals(updatedAddon.Name));

                    if (match.Count() > 0)
                    {
                        var addon = match.First();
                        updatedAddon.DownloadUrl = addon.DownloadUrl;
                        updatedAddon.Blacklisted = addon.Blacklisted;
                        updatedAddon.InstalledVersionDate = addon.InstalledVersionDate;
                    }
                }

                Addons = updatedAddons;
            }
            catch (Exception exception)
            {
                Logger.Exception(exception);
            }
        }
    }
}