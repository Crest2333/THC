﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Normalizing.Models
{
    public partial class Axis : ObservableObject
    {

        /// <summary>
        /// 名称
        /// </summary>
        [Description("轴名称")]
        public string Name { get; set; }

        /// <summary>
        /// 实际位置
        /// </summary>
        [Description("实际位置")]
        public float Location { get; set; }

        /// <summary>
        /// 实际速度
        /// </summary>
        [Description("实际速度")]
        public float Speed { get; set; }

        /// <summary>
        /// 目标位置
        /// </summary>
        [Description("目标位置")]
        public float TargetLocation { get; set; }

        /// <summary>
        /// 目标速度
        /// </summary>
        [Description("目标速度")]
        public float TargetSpeed { get; set; }

        /// <summary>
        /// 实际电流
        /// </summary>
        [Description("实际电流")]
        public float ActualCurrent { get; set; }
    }
}
