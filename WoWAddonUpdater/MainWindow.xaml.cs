using System.Windows;
using System.ComponentModel;
using WoWAddonUpdater.ViewModels;

namespace WoWAddonUpdater
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnClose(object sender, CancelEventArgs e)
        {
            Properties.Settings.Default.IsAutoUpdateEnabled = (DataContext as MainWindowViewModel).IsAutoUpdateActive;
            Properties.Settings.Default.Save();
        }
    }
}