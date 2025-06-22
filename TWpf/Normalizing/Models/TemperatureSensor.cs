using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Normalizing.Models
{
    public class TemperatureSensor
    {
        /// <summary>
        /// 轨顶
        /// </summary>
        public float RailHead { get; set; }

        /// <summary>
        /// 轨脚
        /// </summary>
        public float RailFoot { get; set; }

        /// <summary>
        /// 内轨脚
        /// </summary>
        public float InnerRailFoot { get; set; }

        /// <summary>
        /// 冷却
        /// </summary>
        public float Cooling { get; set; }
    }
}
