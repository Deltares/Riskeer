using System;
using System.ComponentModel;
using DelftTools.Controls.Swf.DataEditorGenerator.FromType;
using DelftTools.Utils.Aop;

namespace DelftTools.Tests.Controls.Swf.DataEditorGenerator.Domain
{
    [Entity]
    public class BindModel
    {
        [Category("Main")]
        public string Name { get; set; }

        [Category("Main")]
        public string Description { get; set; }

        [Category("Timers")]
        [Description("Start time")]
        public DateTime StartTime { get; set; }

        [Category("Timers")]
        [Description("Stop time")]
        public DateTime StopTime { get; set; }

        [Category("Timers")]
        [Description("Time step")]
        public double TimeStep { get; set; }
        
        [Category("Parameters")]
        [Description("Minimal water level")]
        [Unit("m AD")]
        public double MinimumWaterLevel { get; set; }
    }
}