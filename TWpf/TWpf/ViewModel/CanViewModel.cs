using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kvaser.CanLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace TWpf.ViewModel
{
    public class TextBoxModel : INotifyPropertyChanged
    {
        private byte _value;

        public int Index { get; set; }
        public byte Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Message
    {
        public string Type { get; set; }

        public int Id { get; set; }

        public string Data { get; set; }

        public DateTime Time { get; set; }
    }
    public partial class CanViewModel : ObservableObject, IDisposable
    {
        private int handle;
        [ObservableProperty]
        private int? id;
        [ObservableProperty]
        private int? channel;

        [ObservableProperty]
        private int? bitrate;

        private int? count;

        public int? Count
        {
            get { return count; }
            set
            {
                if (!EqualityComparer<int?>.Default.Equals(count, value))
                {
                    count = value;
                    GenerateTextBoxes();
                    OnPropertyChanged();
                }

            }
        }

        public ObservableCollection<TextBoxModel> Datas { get; set; } = new ObservableCollection<TextBoxModel>();

        public ObservableCollection<Message> Messages { get; set; } = new ObservableCollection<Message>();
        private void GenerateTextBoxes()
        {
            Datas.Clear();

            for (int i = 0; i < Count; i++)
            {
                Datas.Add(new TextBoxModel
                {
                    Index = i + 1,
                    Value = 0
                });
            }
        }

        [RelayCommand]
        public void Connection()
        {
            if (!Channel.HasValue || !Bitrate.HasValue)
            {
                MessageBox.Show("缺少连接参数");
                return;
            }

            try
            {
                Canlib.canInitializeLibrary();
                handle = Canlib.canOpenChannel(Channel.Value, Canlib.canOPEN_ACCEPT_VIRTUAL);
                if (handle == 0)
                {
                    handle = Canlib.canOpenChannel(Channel.Value, Canlib.canOPEN_ACCEPT_VIRTUAL);
                }
                Canlib.canSetBitrate(handle, Bitrate.Value);
                Canlib.canBusOn(handle);
            }
            catch (Exception ex)
            {

            }
        }

        public bool IsReceive { get; set; }


        [RelayCommand]
        public void Receive()
        {
            if (IsReceive)
            {
                MessageBox.Show("正在接收");
                return;
            }
            if (!Id.HasValue || handle == 0 || !Count.HasValue)
            {
                MessageBox.Show("缺少发送参数");
                return;
            }
            Task.Run(() =>
            {
                while (true)
                {
                    var data = new byte[8];
                    var status = Canlib.canReadWait(handle, out int id, data, out var dlc, out var flag, out var time, 100);
                    if (status == Canlib.canStatus.canOK)
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            Messages.Add(new Message
                            {
                                Type = "接收",
                                Id = id,
                                Data = BitConverter.ToString(data),
                                Time = DateTime.Now,
                            });
                        });
                    }
                }
            });
            IsReceive = true;
        }

        [RelayCommand]
        public void Write()
        {
            if (!Id.HasValue || handle == 0 || !Count.HasValue)
            {
                MessageBox.Show("缺少发送参数");
                return;
            }
            var data = Datas.Select(c => c.Value).ToArray();
            var status = Canlib.canWrite(handle, Id.Value, data, Count.Value, Canlib.canMSG_STD);
            if (status == Canlib.canStatus.canOK)
            {
                Messages.Add(new Message
                {
                    Type = "发送",
                    Id = Id.Value,
                    Data = BitConverter.ToString(data),
                    Time = DateTime.Now,
                });
            }
        }

        public void Dispose()
        {
            if ( handle != 0 )
            {
                Canlib.canClose(handle);
            }
        }
    }
}
