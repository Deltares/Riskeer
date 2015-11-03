using System.ComponentModel;
using System.Globalization;
using System.Threading;

namespace Core.Common.Base.Workflow
{
    /// <summary>
    /// Extends the basic <see cref="System.ComponentModel.BackgroundWorker"/> class with logic for taking into
    /// account the current thread culture (during creation) while "doing work"
    /// </summary>
    /// <remarks>
    /// This class can be removed as soon as the target framework is switched to ".NET Framework 4.5", which supports the properties:
    /// - CultureInfo.DefaultThreadCurrentCulture
    /// - CultureInfo.DefaultThreadCurrentUICulture
    /// </remarks>
    public class BackgroundWorker : System.ComponentModel.BackgroundWorker
    {
        private readonly CultureInfo uiCulture;
        private readonly CultureInfo culture;

        public BackgroundWorker()
        {
            uiCulture = Thread.CurrentThread.CurrentUICulture;
            culture = Thread.CurrentThread.CurrentCulture;
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = uiCulture;
            Thread.CurrentThread.CurrentCulture = culture;

            base.OnDoWork(e);
        }
    }
}