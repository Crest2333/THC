﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWpf.Views;

namespace TWpf.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        public ObservableCollection<NavigationItem> NavItems { get; }
        = new ObservableCollection<NavigationItem>();

        [ObservableProperty]
        private NavigationItem selectedNavItem;

        [ObservableProperty]
        private object currentView;

        public MainViewModel()
        {
            // 初始化导航项
            NavItems.Add(new NavigationItem
            {
                Title = "Modbus",
                IconPath = "home.png",
                Command = new RelayCommand(() => NavigateToHome())
            });

            NavItems.Add(new NavigationItem
            {
                Title = "扫码枪",
                IconPath = "home.png",
                Command = new RelayCommand(() => CurrentView = new BarcodeScanner())
            });
             NavItems.Add(new NavigationItem
             {
                 Title = "欧姆龙",
                 IconPath = "home.png",
                 Command = new RelayCommand(() => CurrentView = new OmroView())
             });
        }


        private void NavigateToHome() => CurrentView = new ModbusView();

        partial void OnSelectedNavItemChanged(NavigationItem value)
        {
            value.Command?.Execute(null);
        }
    }
}
