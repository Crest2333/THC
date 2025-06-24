using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace Normalizing.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _applicationTitle = "WPF UI - Normalizing";

        [ObservableProperty]
        private ObservableCollection<object> _menuItems = new()
        {
              new NavigationViewItem()
            {
                Content = "机床信息",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Info12 },
                TargetPageType = typeof(Views.Pages.MachineToolPage)
            },
                new NavigationViewItem()
            {
                Content = "手动控制",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings16 },
                TargetPageType = typeof(Views.Pages.ManualControlPage)
            },
                  new NavigationViewItem()
            {
                Content = "自动控制",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings16 },
                TargetPageType = typeof(Views.Pages.AutoControlPage)
            }
        };

      

        [ObservableProperty]
        private ObservableCollection<object> _footerMenuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "Settings",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                TargetPageType = typeof(Views.Pages.SettingsPage)
            }
        };

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new()
        {
            new MenuItem { Header = "Home", Tag = "tray_home" }
        };
    }
}
