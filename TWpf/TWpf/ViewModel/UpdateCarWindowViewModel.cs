using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWpf.Views;

namespace TWpf.ViewModel
{
    public partial class UpdateCarWindowViewModel : ObservableObject
    {
        private readonly UpdateCarWindow _updateCarWindow;
        [ObservableProperty]
        private  Car car;

        public UpdateCarWindowViewModel(UpdateCarWindow  updateCarWindow ,Car car)
        {
            _updateCarWindow = updateCarWindow;
            this.car = car;
        }

        [RelayCommand]
        private void Save()
        {
            _updateCarWindow.Close();
        }
    }
}
