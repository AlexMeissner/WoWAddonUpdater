using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WoWAddonUpdater.Command;

namespace WoWAddonUpdater.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public bool IsAutoUpdateActive { get; set; } = Properties.Settings.Default.IsAutoUpdateEnabled;
        public Brush AutoUpdaterBackgroundBrush => IsAutoUpdateActive ? Brushes.LightGreen : Brushes.Transparent;
        public Thickness ResizeBorderThickness { get; set; } = new Thickness(2);
        public int TitlebarHeight { get; set; } = 20;
        public AddonListViewModel Addons { get; set; } = new AddonListViewModel();

        public ICommand AutoUpdateCommand { get; set; }
        public ICommand MinimizeCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public MainWindowViewModel()
        {
            AutoUpdateCommand = new RelayCommand(ToggleAutoUpdater);
            MinimizeCommand = new RelayCommand(Minimize);
            CloseCommand = new RelayCommand(Close);

            System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Visible = true;
            notifyIcon.Click += Restore;

            using (Stream iconStream = Application.GetResourceStream(new Uri("/Icon.ico", UriKind.Relative)).Stream)
            {
                notifyIcon.Icon = new System.Drawing.Icon(iconStream);
            }

            // TODO: Minimize();
        }

        private void ToggleAutoUpdater()
        {
            IsAutoUpdateActive = !IsAutoUpdateActive;
        }

        private void Minimize()
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
            Application.Current.MainWindow.Hide();
        }

        private void Close()
        {
            Application.Current.MainWindow.Close();
        }

        private void Restore(object sender, EventArgs e)
        {
            Application.Current.MainWindow.Show();
            Application.Current.MainWindow.WindowState = WindowState.Normal;
        }
    }
}