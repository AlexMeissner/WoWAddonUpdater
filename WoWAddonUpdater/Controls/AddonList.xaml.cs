using System.Windows;
using System.Windows.Controls;
using WoWAddonUpdater.ViewModels;

namespace WoWAddonUpdater.Controls
{
    public partial class AddonList : UserControl
    {
        public static readonly DependencyProperty AddonsProperty = DependencyProperty.Register("Addons", typeof(AddonListViewModel), typeof(AddonList), new PropertyMetadata(default(AddonListViewModel)));

        public AddonListViewModel Addons
        {
            get => (AddonListViewModel)GetValue(AddonsProperty);
            set => SetValue(AddonsProperty, value);
        }

        public AddonList()
        {
            InitializeComponent();
        }
    }
}