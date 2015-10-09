using System;
using System.Threading;
using System.Windows.Forms;
using log4net;

namespace DelftTools.Controls.Swf
{
    public partial class ProgressBarDialog : Form
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProgressBarDialog));

        public ProgressBarDialog(string label)
        {
            InitializeComponent();
            Label = label;
        }

        public string Label
        {
            get
            {
                return label1.Text;
            }
            set
            {
                label1.Text = value;
            }
        }

        public static void PerformTask(string label, Action action)
        {
            if (!Environment.UserInteractive)
            {
                action();
                return;
            }

            var closed = false;
            var progressBar = new ProgressBarDialog(label);
            var handle = progressBar.Handle; //trigger creation
            Exception taskException = null;

            ThreadPool.QueueUserWorkItem(s =>
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    taskException = e;

                    log.Error(e.Message, e);
                }
                finally
                {
                    closed = true;
                    try
                    {
                        progressBar.Invoke(new MethodInvoker(progressBar.Close));
                    }
                    catch (ObjectDisposedException)
                    {
                        //gulp
                    }
                }
            });

            if (!closed) //not entirely thread safe
            {
                ModalHelper.ShowModal(progressBar);
            }

            if (taskException != null)
            {
                throw taskException; //we loose stacktrace..but ok
            }
        }
    }
}