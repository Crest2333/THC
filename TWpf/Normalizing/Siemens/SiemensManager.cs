using Normalizing.Models;
using S7.Net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Normalizing.Siemens
{
    public class SiemensManager : IDisposable
    {
        public SiemensManager()
        {
            Plc = new Plc(CpuType.S71500, "192.168.0.14", 0, 1);
        }
        public Plc Plc { get; }

        public async Task<AxisInfo> ReadAxisInfoAsync()
        {
            return await Task.FromResult(new AxisInfo
            {
                XAxis = new Axis
                {
                    Name = "X轴",
                    Speed = 1,
                },
                YAxis = new Axis
                {
                    Name = "Y轴",
                    Speed = 2,
                },
                ZAxis = new Axis
                {
                    Name = "Z轴",
                    Speed = 3,
                }
            });
        }

        public void Dispose()
        {
            if (Plc.IsConnected)
            {
                Plc.Close();
            }
        }

        public async Task<bool> OpenAsync()
        {
            try
            {
                await Plc.OpenAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
