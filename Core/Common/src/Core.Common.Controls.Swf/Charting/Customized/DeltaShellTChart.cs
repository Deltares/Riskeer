using System;
using System.Windows.Forms;
using log4net;
using Steema.TeeChart;

namespace Core.Common.Controls.Swf.Charting.Customized
{
    public class DeltaShellTChart : TChart
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DeltaShellTChart));

        protected override void OnPaint(PaintEventArgs pe)
        {
            try
            {
                base.OnPaint(pe);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw;
            }
        }
    }
}