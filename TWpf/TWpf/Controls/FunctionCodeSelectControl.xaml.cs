using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
using TWpf.Modbus;

namespace TWpf.Controls
{
    /// <summary>
    /// FunctionCodeSelectControl.xaml 的交互逻辑
    /// </summary>
    public partial class FunctionCodeSelectControl : UserControl
    {

        #region 依赖属性
        public static readonly DependencyProperty SelectedFunctionTypeProperty =
            DependencyProperty.Register("SelectedFunctionType", typeof(FunctionType),
                typeof(FunctionCodeSelectControl), new PropertyMetadata(FunctionType.ReadOutputRegister, OnSelectedFunctionTypeChanged));

        public FunctionType SelectedFunctionType
        {
            get { return (FunctionType)GetValue(SelectedFunctionTypeProperty); }
            set { SetValue(SelectedFunctionTypeProperty, value); }
        }

        private static void OnSelectedFunctionTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var comboBox = d as FunctionCodeSelectControl;
            if (comboBox != null && e.NewValue != null)
            {
                comboBox.FunctionComboBox.SelectedItem =
                    comboBox.FunctionItems.FirstOrDefault(i => i.Type == (FunctionType)e.NewValue);
            }
        }
        #endregion

        #region 属性
        public List<FunctionItem> FunctionItems { get; private set; }

        public FunctionItem SelectedFunctionItem
        {
            get { return FunctionItems.FirstOrDefault(i => i.Type == SelectedFunctionType); }
            set
            {
                if (value != null && value.Type != SelectedFunctionType)
                {
                    SelectedFunctionType = value.Type;
                    OnPropertyChanged("SelectedFunctionItem");
                }
            }
        }
        #endregion

        public FunctionCodeSelectControl()
        {
            InitializeComponent();
            FunctionItems = GetFunctionItems();
            DataContext = this;
        }

        private List<FunctionItem> GetFunctionItems()
        {
            var items = new List<FunctionItem>();
            foreach (FunctionType type in Enum.GetValues(typeof(FunctionType)))
            {
                items.Add(new FunctionItem
                {
                    Type = type,
                    Code = ((byte)type).ToString("D2"),
                    Description = GetDescription(type)
                });
            }
            return items;
        }

        private string GetDescription(Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            if (field == null)
                return value.ToString();

            DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute != null ? attribute.Description : value.ToString();
        }

        #region INotifyPropertyChanged实现
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    public class FunctionItem
    {
        public FunctionType Type { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
