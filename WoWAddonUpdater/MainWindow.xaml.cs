using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using System.Runtime.Serialization.Formatters.Binary;
using WoWAddonUpdater.ViewModels;
using System.Collections.ObjectModel;

namespace WoWAddonUpdater
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DeserializeViewModel();
        }

        private void OnClose(object sender, CancelEventArgs e)
        {
            SerializeViewModel();

            Properties.Settings.Default.IsAutoUpdateEnabled = (DataContext as MainWindowViewModel).IsAutoUpdateActive;
            Properties.Settings.Default.Save();
        }

        private void SerializeViewModel()
        {
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    if (DataContext is MainWindowViewModel mainWindowViewModel)
                    {
                        var addonData = mainWindowViewModel.AddonsViewModel.Addons.Select(x => new ViewModelData()
                        {
                            Name = x.Name,
                            Icon = x.Icon,
                            DownloadUrl = x.DownloadUrl,
                            Blacklisted = x.Blacklisted,
                            CurseID = x.CurseID,
                            InstalledVersionDate = x.InstalledVersionDate,
                            AvailableVersionDate = x.AvailableVersionDate
                        }).ToArray();

                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        binaryFormatter.Serialize(stream, addonData);
                        Properties.Settings.Default.AddonData = Convert.ToBase64String(stream.ToArray());
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Exception(exception);
            }
        }

        private void DeserializeViewModel()
        {
            try
            {
                using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(Properties.Settings.Default.AddonData)))
                {
                    if (stream.Length > 0 && DataContext is MainWindowViewModel mainWindowViewModel)
                    {
                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        var addonData = binaryFormatter.Deserialize(stream) as ViewModelData[];

                        mainWindowViewModel.AddonsViewModel.Addons = new ObservableCollection<AddonViewModel>(addonData.Select(x => new AddonViewModel()
                        {
                            Name = x.Name,
                            Icon = x.Icon,
                            DownloadUrl = x.DownloadUrl,
                            Blacklisted = x.Blacklisted,
                            CurseID = x.CurseID,
                            InstalledVersionDate = x.InstalledVersionDate,
                            AvailableVersionDate = x.AvailableVersionDate
                        }));
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Exception(exception);
            }
        }
    }
}