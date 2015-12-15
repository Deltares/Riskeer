using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Core.Common.Controls.Swf.Properties;
using DevExpress.XtraWizard;

namespace Core.Common.Controls.Swf
{
    public partial class WizardDialog : DialogBase, IView
    {
        private readonly IList<IComponent> pages;
        private readonly IDictionary<IComponent, WizardPage> wizardPages = new Dictionary<IComponent, WizardPage>();

        public WizardDialog(IWin32Window owner, Icon icon) : base(owner, icon)
        {
            pages = new List<IComponent>();
            InitializeComponent();
            wizardControl1.SelectedPageChanged += WizardControl1SelectedPageChanged;
            wizardControl1.SelectedPageChanging += WizardControl1SelectedPageChanging;
            wizardControl1.FinishClick += WizardControl1FinishClick;
            wizardControl1.CancelClick += WizardControl1CancelClick;
            wizardControl1.NextClick += WizardControl1NextClick;
            wizardControl1.PrevClick += WizardControl1PrevClick;

            KeyPreview = true; //catch key events in pages
            KeyUp += OnPageModified;

            updateButtonsTimer.Interval = 300;
            updateButtonsTimer.Tick += UpdateNavigationButtonsTimerTick;
        }

        public virtual object Data { get; set; }

        public Image Image { get; set; }
        public ViewInfo ViewInfo { get; set; }

        public IEnumerable<IComponent> Pages
        {
            get
            {
                return pages;
            }
        }

        public IList<string> PageTitles
        {
            get
            {
                return Enumerable.OfType<WizardPage>(wizardControl1.Pages).Select(p => p.Text).ToList();
            }
        }

        public IList<string> PageDescriptions
        {
            get
            {
                return Enumerable.OfType<WizardPage>(wizardControl1.Pages).Select(p => p.DescriptionText).ToList();
            }
        }

        public IComponent CurrentPage
        {
            get
            {
                if ((wizardControl1.SelectedPageIndex == 0) ||
                    (wizardControl1.SelectedPageIndex == wizardControl1.Pages.Count - 1))
                {
                    return wizardControl1.Pages[wizardControl1.SelectedPageIndex];
                }
                return pages[wizardControl1.SelectedPageIndex - 1];
            }
            set
            {
                wizardControl1.SelectedPageIndex = pages.IndexOf(value) + 1;
            }
        }

        public string WelcomeMessage
        {
            get
            {
                return welcomeWizardPage1.IntroductionText;
            }
            set
            {
                welcomeWizardPage1.IntroductionText = value;
                richTextBoxWelcome.Text = value;
            }
        }

        public string FinishedPageMessage
        {
            get
            {
                return completionWizardPage1.FinishText;
            }
            set
            {
                completionWizardPage1.FinishText = value;
                richTextBoxFinished.Text = value;
            }
        }

        public string Title
        {
            get
            {
                return wizardControl1.Text;
            }
            set
            {
                wizardControl1.Text = value;
            }
        }

        public void EnsureVisible(object item) {}

        public virtual void UpdateNavigationButtons()
        {
            if (!(CurrentPage is IWizardPage) || wizardPages == null || !wizardPages.ContainsKey(CurrentPage))
            {
                return;
            }

            var wizardPage = CurrentPage as IWizardPage;
            var containerPage = wizardPages[CurrentPage];

            if (containerPage != null)
            {
                containerPage.AllowBack = wizardPage.CanDoPrevious();
                containerPage.AllowNext = wizardPage.CanDoNext();
            }
        }

        public void AddPage(IComponent page, string title, string description)
        {
            var pageControl = (Control) page;
            pageControl.Dock = DockStyle.Fill;

            var wizardPage = new WizardPage
            {
                Text = title, DescriptionText = description, Dock = DockStyle.Fill
            };
            wizardPage.Controls.Add(pageControl);

            wizardControl1.Controls.Add(wizardPage);
            wizardControl1.Pages.Insert(wizardControl1.Pages.Count - 1, wizardPage);

            wizardPages[page] = wizardPage;
            pages.Add(page);
        }

        public void RemovePage(IComponent page)
        {
            var wizardPage = wizardControl1.Controls.OfType<WizardPage>().FirstOrDefault(p => p.Controls.Contains((Control) page));
            if (wizardPage == null)
            {
                return;
            }

            wizardControl1.Controls.Remove(wizardPage);
            wizardControl1.Pages.Remove(wizardPage);

            wizardPages.Remove(page);
            pages.Remove(page);
        }

        public DialogResult ShowModal()
        {
            if (ShowDialog() == DialogResult.OK)
            {
                return DialogResult.OK;
            }
            return DialogResult.Cancel;
        }

        protected bool WelcomePageVisible
        {
            set
            {
                welcomeWizardPage1.Visible = value;
            }
        }

        protected bool CompletionPageVisible
        {
            set
            {
                completionWizardPage1.Visible = value;
            }
        }

        protected virtual void OnPageReverted(IWizardPage page) {}

        protected virtual void OnPageCompleted(IWizardPage page) {}

        protected virtual void OnDialogFinished() {}

        /// <summary>
        /// Override this method if you want to choose the next page on basis of choices that were made in one of the pages
        /// The direction is forward
        /// </summary>
        /// <param name="previousPageIndex">The index of the page you are leaving</param>
        /// <returns>The next page index, -1 if the default is used</returns>
        protected virtual int FindNextPageIndexMovingForward(int previousPageIndex)
        {
            return -1;
        }

        /// <summary>
        /// Override this method if you want to choose the next page on basis of choices that were made in one of the pages
        /// The direction is backward
        /// </summary>
        /// <param name="previousPageIndex">The index of the page you are leaving</param>
        /// <returns>The next page index, -1 if the default is used</returns>
        protected virtual int FindNextPageIndexMovingBackward(int previousPageIndex)
        {
            return -1;
        }

        protected virtual void WizardControl1SelectedPageChanging(object sender, WizardPageChangingEventArgs e)
        {
            var nextPageIndex = EvaluateNextPageIndex(wizardControl1.SelectedPageIndex, e.Direction);
            if (nextPageIndex > 0)
            {
                e.Page = wizardControl1.Pages[nextPageIndex];
            }
        }

        private void WizardControl1SelectedPageChanged(object sender, WizardPageChangedEventArgs e)
        {
            UpdateNavigationButtons();
        }

        private void WizardControl1PrevClick(object sender, WizardCommandButtonClickEventArgs e)
        {
            if (CurrentPage is IWizardPage)
            {
                IWizardPage wizardPage = (IWizardPage) CurrentPage;
                if (!wizardPage.CanDoPrevious())
                {
                    // setting handled to true will prevent default handling 
                    e.Handled = true;
                }
                else
                {
                    try
                    {
                        OnPageReverted(wizardPage);
                    }
                    catch (Exception ee)
                    {
                        ShowExceptionInMessageWindow(ee);
                        e.Handled = true;
                    }
                }
            }
        }

        private void WizardControl1NextClick(object sender, WizardCommandButtonClickEventArgs e)
        {
            if (CurrentPage is IWizardPage)
            {
                var wizardPage = (IWizardPage) CurrentPage;
                if (!wizardPage.CanDoNext())
                {
                    // setting handled to true will prevent default handling 
                    e.Handled = true;
                }
                else
                {
                    try
                    {
                        OnPageCompleted(wizardPage);
                    }
                    catch (Exception ee)
                    {
                        ShowExceptionInMessageWindow(ee);
                        e.Handled = true;
                    }
                }
            }
        }

        private void ShowExceptionInMessageWindow(Exception ee)
        {
            var message = string.Format(Resources.WizardDialog_WizardControl1NextClick_An_error_occurred_Please_verify_your_input_Error_0_, ee.Message);
            MessageBox.Show(message, Resources.WizardDialog_WizardControl1NextClick_Error_occurred, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void WizardControl1CancelClick(object sender, CancelEventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void WizardControl1FinishClick(object sender, CancelEventArgs e)
        {
            if (CurrentPage is IWizardPage)
            {
                IWizardPage wizardPage = (IWizardPage) CurrentPage;
                if (!wizardPage.CanFinish())
                {
                    return;
                }
            }

            OnDialogFinished();
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Sometimes you want hide a page depending on choices in other pages
        /// Use this method to determine the index of the page you want to go to
        /// depending on choices that were made and the direction you are moving
        /// </summary>
        /// <param name="previousPageIndex">The index of the page you are leaving</param>
        /// <param name="direction">Moving forward or backward through the wizard</param>
        /// <returns>The next page index, -1 if the default is used</returns>
        private int EvaluateNextPageIndex(int previousPageIndex, Direction direction)
        {
            return (direction == Direction.Forward) ? FindNextPageIndexMovingForward(previousPageIndex) : FindNextPageIndexMovingBackward(previousPageIndex);
        }

        #region Auto Updates

        private void OnPageModified(object sender, object e)
        {
            updateButtonsTimer.Start(); //delayed refresh
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_PARENTNOTIFY = 0x0210;
            const int WM_SETFOCUS = 0x0007;
            const int WM_LBUTTONDOWN = 0x0201;

            if (m.Msg == WM_SETFOCUS) //called after child (file) dialog is closed
            {
                OnPageModified(null, null);
            }
            else if (m.Msg == WM_PARENTNOTIFY)
            {
                var subEvent = m.WParam.ToInt32() & 0xFFFF; //LOWORD
                if (subEvent == WM_LBUTTONDOWN) //called after mouse down anywhere in dialog
                {
                    OnPageModified(null, null);
                }
            }
            base.WndProc(ref m);
        }

        private void UpdateNavigationButtonsTimerTick(object sender, EventArgs e)
        {
            UpdateNavigationButtons(); //refresh
            updateButtonsTimer.Stop();
        }

        #endregion

        protected override Button GetCancelButton()
        {
            return null; // Cancel support already implemented by WizardControl
        }
    }
}