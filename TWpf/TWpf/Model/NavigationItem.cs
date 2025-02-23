using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TWpf.ViewModel
{
    public partial class NavigationItem : ObservableObject
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string IconPath { get; set; } 

        /// <summary>
        ///  跳转
        /// </summary>
        public ICommand Command { get; set; }

        [ObservableProperty]
        public bool isSelected;

    }
}
