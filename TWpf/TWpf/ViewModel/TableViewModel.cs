using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using TWpf.Extensions;
using TWpf.Views;


namespace TWpf.ViewModel
{
    internal partial class TableViewModel : ObservableObject
    {
        [ObservableProperty]
        private string carNo;

        [ObservableProperty]
        private string vinNo;

        [ObservableProperty]
        private ObservableCollection<Car> carList;

        private readonly List<Car> dataSource;
        private readonly Dispatcher _dispatcher;

        [RelayCommand]
        private void Search()
        {
            _dispatcher.Invoke(() =>
            {
                var list = dataSource
               .WhereIf(!string.IsNullOrEmpty(carNo), c => c.CarNo == carNo)
               .WhereIf(!string.IsNullOrEmpty(vinNo), c => c.VinNo == vinNo);
                CarList = new ObservableCollection<Car>(list);
            });
        }

        [RelayCommand]
        private void OpenUpdate(Car car)
        {
            var window=new UpdateCarWindow(car);
            window.ShowDialog();
        }

        public TableViewModel(Dispatcher dispatcher)
        {
            dataSource = GenerateCarSeedData();
            _dispatcher = dispatcher;
            carList = new ObservableCollection<Car>(dataSource);
        }
        private List<Car> GenerateCarSeedData()
        {
            var carList = new List<Car>();
            Random random = new Random();
            string[] carTypes = { "丰田卡罗拉", "本田思域", "大众朗逸", "宝马 3 系", "奔驰 C 级" };
            for (int i = 1; i <= 50; i++)
            {
                string vinNo = $"VIN{i:D10}";
                string carNo = $"粤A{i:D5}";
                string carType = carTypes[random.Next(0, carTypes.Length)];
                DateTime year = new DateTime(2015 + random.Next(0, 11), 1, 1);
                carList.Add(new Car(carNo, vinNo, carType, year));
            }
            return carList;
        }
    }

    public class Car
    {
        public Car(string carNo, string vinNo, string carTypeName, DateTime year)
        {
            Year = year;
            CarNo = carNo;
            VinNo = vinNo;
            CarTypeName = carTypeName;
        }

        public DateTime Year { get; set; }
        public string CarNo { get; set; }

        public string VinNo { get; set; }

        public string CarTypeName { get; set; }
    }
}
