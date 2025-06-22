using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Normalizing.Models
{
    public class MachineStatus
    {
        /// <summary>
        /// 电压值（单位：伏特）
        /// </summary>
        public float Voltage { get; set; }

        /// <summary>
        /// 电流值（单位：安培）
        /// </summary>
        public float Current { get; set; }

        /// <summary>
        /// 功率值（单位：瓦特）
        /// </summary>
        public float Power { get; set; }

        /// <summary>
        /// 能量值（单位：焦耳）
        /// </summary>
        public float Energy { get; set; }

        /// <summary>
        /// IGBT电源状态（true表示开启，false表示关闭）
        /// </summary>
        public DeviceStatus IgbmPowerState { get; set; }

        /// <summary>
        /// Y轴跟踪到位状态（true表示到位，false表示未到位）
        /// </summary>
        public DeviceStatus YAxisTrackingState { get; set; }

        /// <summary>
        /// Z轴跟踪到位状态（true表示到位，false表示未到位）
        /// </summary>
        public DeviceStatus ZAxisTrackingState { get; set; }

        /// <summary>
        /// 顶料缸安全位状态（true表示安全，false表示不安全）
        /// </summary>
        public DeviceStatus TopCylinderSafetyState { get; set; }

        /// <summary>
        /// 允许加工状态（true表示允许，false表示不允许）
        /// </summary>
        public DeviceStatus ProcessingAllowedState { get; set; }

        /// <summary>
        /// 送料完成状态（true表示完成，false表示未完成）
        /// </summary>
        public DeviceStatus FeedingCompletedState { get; set; }

        /// <summary>
        /// 自动运行状态（true表示自动运行，false表示手动或停止）
        /// </summary>
        public DeviceStatus AutoRunState { get; set; }
    }
}
