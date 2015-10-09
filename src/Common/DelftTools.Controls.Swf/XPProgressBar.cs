using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf
{
    public enum GradientMode
    {
        Vertical,
        VerticalCenter,
        Horizontal,
        HorizontalCenter,
        Diagonal
    };

    public class XPProgressBar : Control
    {
        #region "  Constructor  "

        private const string CategoryName = "Xp ProgressBar";

        #endregion

        #region "  BackImage  "

        [RefreshProperties(RefreshProperties.Repaint)]
        [Category(CategoryName)]
        public override Image BackgroundImage
        {
            get
            {
                return base.BackgroundImage;
            }
            set
            {
                base.BackgroundImage = value;
                InvalidateBuffer();
            }
        }

        #endregion

        #region "  Text Override  "

        [Category(CategoryName)]
        [Description("The Text displayed in the Progress Bar")]
        [DefaultValue("")]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                if (base.Text != value)
                {
                    base.Text = value;
                    Invalidate();
                }
            }
        }

        #endregion

        #region "  Dispose  "

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (mPenIn != null)
                {
                    mPenIn.Dispose();
                    mPenIn = null;
                }

                if (mPenOut != null)
                {
                    mPenOut.Dispose();
                    mPenOut = null;
                }
                if (mPenOut2 != null)
                {
                    mPenOut2.Dispose();
                    mPenOut2 = null;
                }

                if (mDobleBack != null)
                {
                    mDobleBack.Dispose();
                    mDobleBack = null;
                }
                if (mBrush1 != null)
                {
                    mBrush1.Dispose();
                    mBrush1 = null;
                }

                if (mBrush2 != null)
                {
                    mBrush2.Dispose();
                    mBrush2 = null;
                }

                base.Dispose(disposing);
            }
        }

        #endregion

        #region "  Private Fields  "

        private Color mColor1 = Color.FromArgb(170, 240, 170);

        private Color mColor2 = Color.FromArgb(10, 150, 10);

        private Color mColorBackground = Color.White;

        private Color mColorText = Color.Black;

        private Image mDobleBack = null;

        private GradientMode mGradientStyle = GradientMode.VerticalCenter;

        private int mMax = 100;

        private int mMin = 0;

        private int mPosition = 50;

        private byte mSteepDistance = 2;

        private byte mSteepWidth = 6;

        #endregion

        #region "  Colors   "

        [Category(CategoryName)]
        [Description("The Back Color of the Progress Bar")]
        public Color ColorBackground
        {
            get
            {
                return mColorBackground;
            }
            set
            {
                mColorBackground = value;
                InvalidateBuffer(true);
            }
        }

        [Category(CategoryName)]
        [Description("The Border Color of the gradient in the Progress Bar")]
        public Color ColorBarBorder
        {
            get
            {
                return mColor1;
            }
            set
            {
                mColor1 = value;
                InvalidateBuffer(true);
            }
        }

        [Category(CategoryName)]
        [Description("The Center Color of the gradient in the Progress Bar")]
        public Color ColorBarCenter
        {
            get
            {
                return mColor2;
            }
            set
            {
                mColor2 = value;
                InvalidateBuffer(true);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Description("Set to TRUE to reset all colors like the Windows XP Progress Bar ®")]
        [Category(CategoryName)]
        [DefaultValue(false)]
        public bool ColorsXP
        {
            get
            {
                return false;
            }
            //set
            //{
            //    ColorBarBorder = Color.FromArgb(170, 240, 170);
            //    ColorBarCenter = Color.FromArgb(10, 150, 10);
            //    ColorBackground = Color.White;
            //}
        }

        [Category(CategoryName)]
        [Description("The Color of the text displayed in the Progress Bar")]
        public Color ColorText
        {
            get
            {
                return mColorText;
            }
            set
            {
                mColorText = value;

                if (!string.IsNullOrEmpty(Text))
                {
                    Invalidate();
                }
            }
        }

        #endregion

        #region "  Position   "

        [RefreshProperties(RefreshProperties.Repaint)]
        [Category(CategoryName)]
        [Description("The Current Position of the Progress Bar")]
        public int Position
        {
            get
            {
                return mPosition;
            }
            set
            {
                if (value > mMax)
                {
                    mPosition = mMax;
                }
                else if (value < mMin)
                {
                    mPosition = mMin;
                }
                else
                {
                    mPosition = value;
                }
                Invalidate();
            }
        }

        [RefreshProperties(RefreshProperties.Repaint)]
        [Category(CategoryName)]
        [Description("The Max Position of the Progress Bar")]
        public int PositionMax
        {
            get
            {
                return mMax;
            }
            set
            {
                if (value > mMin)
                {
                    mMax = value;

                    if (mPosition > mMax)
                    {
                        Position = mMax;
                    }

                    InvalidateBuffer(true);
                }
            }
        }

        [RefreshProperties(RefreshProperties.Repaint)]
        [Category(CategoryName)]
        [Description("The Min Position of the Progress Bar")]
        public int PositionMin
        {
            get
            {
                return mMin;
            }
            set
            {
                if (value < mMax)
                {
                    mMin = value;

                    if (mPosition < mMin)
                    {
                        Position = mMin;
                    }
                    InvalidateBuffer(true);
                }
            }
        }

        [Category(CategoryName)]
        [Description("The number of Pixels between two Steeps in Progress Bar")]
        [DefaultValue((byte) 2)]
        public byte SteepDistance
        {
            get
            {
                return mSteepDistance;
            }
            set
            {
                if (value >= 0)
                {
                    mSteepDistance = value;
                    InvalidateBuffer(true);
                }
            }
        }

        #endregion

        #region  "  Progress Style   "

        [Category(CategoryName)]
        [Description("The Style of the gradient bar in Progress Bar")]
        [DefaultValue(GradientMode.VerticalCenter)]
        public GradientMode GradientStyle
        {
            get
            {
                return mGradientStyle;
            }
            set
            {
                if (mGradientStyle != value)
                {
                    mGradientStyle = value;
                    CreatePaintElements();
                    Invalidate();
                }
            }
        }

        [Category(CategoryName)]
        [Description("The number of Pixels of the Steeps in Progress Bar")]
        [DefaultValue((byte) 6)]
        public byte SteepWidth
        {
            get
            {
                return mSteepWidth;
            }
            set
            {
                if (value > 0)
                {
                    mSteepWidth = value;
                    InvalidateBuffer(true);
                }
            }
        }

        #endregion

        #region "  Text Shadow  "

        private bool mTextShadow = false;

        [Category(CategoryName)]
        [Description("Set the Text shadow in the Progress Bar")]
        [DefaultValue(true)]
        public bool TextShadow
        {
            get
            {
                return mTextShadow;
            }
            set
            {
                mTextShadow = value;
                Invalidate();
            }
        }

        #endregion

        #region "  Text Shadow Alpha  "

        private byte mTextShadowAlpha = 150;

        [Category(CategoryName)]
        [Description("Set the Alpha Channel of the Text shadow in the Progress Bar")]
        [DefaultValue((byte) 150)]
        public byte TextShadowAlpha
        {
            get
            {
                return mTextShadowAlpha;
            }
            set
            {
                if (mTextShadowAlpha != value)
                {
                    mTextShadowAlpha = value;
                    TextShadow = true;
                }
            }
        }

        #endregion

        #region "  Paint Methods  "

        #region "  OnPaint  "

        protected override void OnPaint(PaintEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("Paint " + this.Name + "  Pos: "+this.Position.ToString());
            if (!IsDisposed)
            {
                int mSteepTotal = mSteepWidth + mSteepDistance;
                float mUtilWidth = Width - 6 + mSteepDistance;

                if (mDobleBack == null)
                {
                    mUtilWidth = Width - 6 + mSteepDistance;
                    int mMaxSteeps = (int) (mUtilWidth/mSteepTotal);
                    Width = 6 + mSteepTotal*mMaxSteeps;

                    mDobleBack = new Bitmap(Width, Height);

                    Graphics g2 = Graphics.FromImage(mDobleBack);

                    CreatePaintElements();

                    g2.Clear(mColorBackground);

                    if (BackgroundImage != null)
                    {
                        TextureBrush textuBrush = new TextureBrush(BackgroundImage, WrapMode.Tile);
                        g2.FillRectangle(textuBrush, 0, 0, Width, Height);
                        textuBrush.Dispose();
                    }
                    //				g2.DrawImage()

/*
                    g2.DrawRectangle(mPenOut2, outnnerRect2);
*/
                    //g2.DrawRectangle(mPenOut, outnnerRect);
                    g2.DrawRectangle(mPenIn, outnnerRect);
                    g2.Dispose();
                }

                Image image = new Bitmap(mDobleBack);

                Graphics gtemp = Graphics.FromImage(image);

                int mCantSteeps = (int) ((((float) mPosition - mMin)/(mMax - mMin))*mUtilWidth/mSteepTotal);

                for (int i = 0; i < mCantSteeps; i++)
                {
                    DrawSteep(gtemp, i);
                }

                if (!string.IsNullOrEmpty(Text))
                {
                    //gtemp.TextRenderingHint = TextRenderingHint.AntiAlias;
                    DrawCenterString(gtemp, ClientRectangle);
                }

                e.Graphics.DrawImage(image, e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle, GraphicsUnit.Pixel);
                image.Dispose();
                gtemp.Dispose();
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent) {}

        #endregion

        #region "  OnSizeChange  "

        protected override void OnSizeChanged(EventArgs e)
        {
            if (!IsDisposed)
            {
                if (Height < 12)
                {
                    Height = 12;
                }

                base.OnSizeChanged(e);
                InvalidateBuffer(true);
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(100, 29);
            }
        }

        #endregion

        #region "  More Draw Methods  "

        private void DrawSteep(Graphics g, int number)
        {
            g.FillRectangle(mBrush1, 4 + number*(mSteepDistance + mSteepWidth), mSteepRect1.Y + 1, mSteepWidth,
                            mSteepRect1.Height);
            g.FillRectangle(mBrush2, 4 + number*(mSteepDistance + mSteepWidth), mSteepRect2.Y + 1, mSteepWidth,
                            mSteepRect2.Height - 1);
        }

        private void InvalidateBuffer()
        {
            InvalidateBuffer(false);
        }

        private void InvalidateBuffer(bool InvalidateControl)
        {
            if (mDobleBack != null)
            {
                mDobleBack.Dispose();
                mDobleBack = null;
            }

            if (InvalidateControl)
            {
                Invalidate();
            }
        }

        private void DisposeBrushes()
        {
            if (mBrush1 != null)
            {
                mBrush1.Dispose();
                mBrush1 = null;
            }

            if (mBrush2 != null)
            {
                mBrush2.Dispose();
                mBrush2 = null;
            }
        }

        private void DrawCenterString(Graphics gfx, Rectangle box)
        {
            SizeF ss = gfx.MeasureString(Text, Font);

            float left = box.X + (box.Width - ss.Width)/2;
            float top = box.Y + (box.Height - ss.Height)/2;

            if (mTextShadow)
            {
                SolidBrush mShadowBrush = new SolidBrush(Color.FromArgb(mTextShadowAlpha, Color.Black));
                gfx.DrawString(Text, Font, mShadowBrush, left + 1, top + 1);
                mShadowBrush.Dispose();
            }
            SolidBrush mTextBrush = new SolidBrush(mColorText);
            gfx.DrawString(Text, Font, mTextBrush, left, top);
            mTextBrush.Dispose();
        }

        #endregion

        #region "  CreatePaintElements   "

        private Rectangle innerRect;
        private LinearGradientBrush mBrush1;
        private LinearGradientBrush mBrush2;
        private Pen mPenIn = new Pen(Color.FromArgb(239, 239, 239));

        private Pen mPenOut = new Pen(Color.FromArgb(104, 104, 104));
        private Pen mPenOut2 = new Pen(Color.FromArgb(190, 190, 190));

        private Rectangle mSteepRect1;
        private Rectangle mSteepRect2;
        private Rectangle outnnerRect;
        private Rectangle outnnerRect2;

        private void CreatePaintElements()
        {
            DisposeBrushes();

            switch (mGradientStyle)
            {
                case GradientMode.VerticalCenter:

                    mSteepRect1 = new Rectangle(
                        0,
                        2,
                        mSteepWidth,
                        Height/2 + (int) (Height*0.05));
                    mBrush1 = new LinearGradientBrush(mSteepRect1, mColor1, mColor2, LinearGradientMode.Vertical);

                    mSteepRect2 = new Rectangle(
                        0,
                        mSteepRect1.Bottom - 1,
                        mSteepWidth,
                        Height - mSteepRect1.Height - 4);
                    mBrush2 = new LinearGradientBrush(mSteepRect2, mColor2, mColor1, LinearGradientMode.Vertical);
                    break;

                case GradientMode.Vertical:
                    mSteepRect1 = new Rectangle(
                        0,
                        3,
                        mSteepWidth,
                        Height - 7);
                    mBrush1 = new LinearGradientBrush(mSteepRect1, mColor1, mColor2, LinearGradientMode.Vertical);
                    mSteepRect2 = new Rectangle(
                        -100,
                        -100,
                        1,
                        1);
                    mBrush2 = new LinearGradientBrush(mSteepRect2, mColor2, mColor1, LinearGradientMode.Horizontal);
                    break;

                case GradientMode.Horizontal:
                    mSteepRect1 = new Rectangle(
                        0,
                        3,
                        mSteepWidth,
                        Height - 7);

                    //					mBrush1 = new LinearGradientBrush(rTemp, mColor1, mColor2, LinearGradientMode.Horizontal);
                    mBrush1 = new LinearGradientBrush(ClientRectangle, mColor1, mColor2, LinearGradientMode.Horizontal);
                    mSteepRect2 = new Rectangle(
                        -100,
                        -100,
                        1,
                        1);
                    mBrush2 = new LinearGradientBrush(mSteepRect2, Color.Red, Color.Red, LinearGradientMode.Horizontal);
                    break;

                case GradientMode.HorizontalCenter:
                    mSteepRect1 = new Rectangle(
                        0,
                        3,
                        mSteepWidth,
                        Height - 7);
                    //					mBrush1 = new LinearGradientBrush(rTemp, mColor1, mColor2, LinearGradientMode.Horizontal);
                    mBrush1 = new LinearGradientBrush(ClientRectangle, mColor1, mColor2, LinearGradientMode.Horizontal);
                    mBrush1.SetBlendTriangularShape(0.5f);

                    mSteepRect2 = new Rectangle(
                        -100,
                        -100,
                        1,
                        1);
                    mBrush2 = new LinearGradientBrush(mSteepRect2, Color.Red, Color.Red, LinearGradientMode.Horizontal);
                    break;

                case GradientMode.Diagonal:
                    mSteepRect1 = new Rectangle(
                        0,
                        3,
                        mSteepWidth,
                        Height - 7);
                    //					mBrush1 = new LinearGradientBrush(rTemp, mColor1, mColor2, LinearGradientMode.ForwardDiagonal);
                    mBrush1 =
                        new LinearGradientBrush(ClientRectangle, mColor1, mColor2, LinearGradientMode.ForwardDiagonal);
                    //					((LinearGradientBrush) mBrush1).SetBlendTriangularShape(0.5f);

                    mSteepRect2 = new Rectangle(
                        -100,
                        -100,
                        1,
                        1);
                    mBrush2 = new LinearGradientBrush(mSteepRect2, Color.Red, Color.Red, LinearGradientMode.Horizontal);
                    break;

                default:
                    mBrush1 = new LinearGradientBrush(mSteepRect1, mColor1, mColor2, LinearGradientMode.Vertical);
                    mBrush2 = new LinearGradientBrush(mSteepRect2, mColor2, mColor1, LinearGradientMode.Vertical);
                    break;
            }

            innerRect = new Rectangle(
                ClientRectangle.X + 2,
                ClientRectangle.Y + 2,
                ClientRectangle.Width - 4,
                ClientRectangle.Height - 4);
            outnnerRect = new Rectangle(
                ClientRectangle.X,
                ClientRectangle.Y,
                ClientRectangle.Width - 1,
                ClientRectangle.Height - 1);
            outnnerRect2 = new Rectangle(
                ClientRectangle.X + 1,
                ClientRectangle.Y + 1,
                ClientRectangle.Width,
                ClientRectangle.Height);
        }

        #endregion

        #endregion
    }
}