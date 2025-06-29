﻿using LiveChartsCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TWpf.ViewModel;

namespace TWpf.Controls
{
    /// <summary>
    /// TemperatureControl.xaml 的交互逻辑
    /// </summary>
    /// <summary>
    /// TemperatureControl.xaml 的交互逻辑
    /// </summary>
    public partial class TemperatureControl : UserControl
    {

        public ObservableCollection<ISeries> Series { get; set; }

        public TemperatureData Temperature
        {
            get { return (TemperatureData)GetValue(TemperatureProperty); }
            set { SetValue(TemperatureProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Temperature.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TemperatureProperty =
            DependencyProperty.Register("Temperature", typeof(TemperatureData), typeof(TemperatureControl), new PropertyMetadata(null));

        

        public TemperatureControl()
        {
            InitializeComponent();
        }
    }
}
