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
            Plc = new Plc(CpuType.S71200, "192.168.0.6", 0, 1);
        }
        public Plc Plc { get; }

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
