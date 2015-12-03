using System;
using System.Drawing;
using System.Windows.Forms;

namespace Core.Common.Controls.Swf
{
    public partial class ImageResolutionDialog : Form
    {
        private DialogResult result;
        private bool updatingControls;

        private Image baseImage;

        public ImageResolutionDialog()
        {
            InitializeComponent();
            Resolution = 100;
        }

        public double Resolution { get; set; }

        public Image BaseImage
        {
            private get
            {
                return baseImage;
            }
            set
            {
                baseImage = value;
                double minWidthResolution = 100.0/BaseImage.Width;
                double minHeightResolution = 100.0/BaseImage.Width;
                if (minHeightResolution > 1.0 || minWidthResolution > 1.0)
                {
                    trackBar1.Minimum = (int) Math.Max(minHeightResolution, minHeightResolution);
                }
                else
                {
                    trackBar1.Minimum = 1;
                }
            }
        }

        public string Title { get; set; }

        public DialogResult ShowModal()
        {
            if (BaseImage == null)
            {
                return DialogResult.Cancel;
            }

            result = DialogResult.Cancel;

            UpdateControls();

            Show();

            while (Visible)
            {
                Application.DoEvents();
            }

            return result;
        }

        private void OkButtonClicked(object sender, EventArgs e)
        {
            result = DialogResult.OK;
            CloseDialog();
        }

        private void CancelButtonClicked(object sender, EventArgs e)
        {
            CloseDialog();
        }

        private void CloseDialog()
        {
            Hide();
            Dispose();
        }

        private void ResolutionTrackBarValueChanged(object sender, EventArgs e)
        {
            if (updatingControls)
            {
                return;
            }

            Resolution = trackBar1.Value;
            UpdateControls(trackBar1);
        }

        private void TextBoxWidthTextChanged(object sender, EventArgs e)
        {
            if (updatingControls)
            {
                return;
            }

            double newWidth;
            if (!Double.TryParse(textBoxWidth.Text, out newWidth))
            {
                textBoxWidth.BackColor = Color.Red;
                return;
            }
            SetResolution((newWidth/BaseImage.Width)*100, textBoxWidth);
        }

        private void TextBoxHeightTextChanged(object sender, EventArgs e)
        {
            if (updatingControls)
            {
                return;
            }

            double newHeight;
            if (!Double.TryParse(textBoxHeight.Text, out newHeight))
            {
                textBoxHeight.BackColor = Color.Red;
                return;
            }
            var newResolution = (newHeight/BaseImage.Height)*100;
            SetResolution(newResolution, textBoxHeight);
        }

        private void TextBoxPercentageTextChanged(object sender, EventArgs e)
        {
            if (updatingControls)
            {
                return;
            }

            double newResolution;
            if (!Double.TryParse(textBoxPercentage.Text, out newResolution))
            {
                textBoxPercentage.BackColor = Color.Red;
                return;
            }
            SetResolution(newResolution, textBoxPercentage);
        }

        private void UpdateControls(Control controlToIgnore = null)
        {
            updatingControls = true;
            if (controlToIgnore != trackBar1)
            {
                trackBar1.Value = (int) Resolution;
            }
            if (controlToIgnore != textBoxPercentage)
            {
                textBoxPercentage.Text = Resolution.ToString();
            }
            if (controlToIgnore != textBoxWidth)
            {
                textBoxWidth.Text = (Resolution/100*BaseImage.Width).ToString();
            }
            if (controlToIgnore != textBoxHeight)
            {
                textBoxHeight.Text = (Resolution/100*BaseImage.Height).ToString();
            }
            updatingControls = false;
            UpdateImageBox();
        }

        private void UpdateImageBox()
        {
            pictureBox1.Image = ExportImageHelper.CreateResizedImage(BaseImage, (int) (Resolution/100*BaseImage.Width), (int) (Resolution/100*BaseImage.Height));
            pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
        }

        private void SetResolution(double newResolution, Control control)
        {
            if (!ValidResolution(newResolution))
            {
                control.BackColor = Color.Red;
                return;
            }
            Resolution = newResolution;

            control.BackColor = Color.White;
            UpdateControls(control);
        }

        private bool ValidResolution(double newResolution)
        {
            return newResolution >= trackBar1.Minimum &&
                   newResolution <= trackBar1.Maximum &&
                   Resolution/100*BaseImage.Width > 1 &&
                   Resolution/100*BaseImage.Height > 1;
        }
    }
}