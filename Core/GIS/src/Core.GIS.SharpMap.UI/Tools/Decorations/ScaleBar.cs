using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Core.GIS.SharpMap.UI.Tools.Decorations
{
    public enum ScaleBarStyle
    {
        ws_bsStandard = 0,
        ws_bsMeridian = 1,
        ws_bsMeridian1 = 2
    }

    public enum sbScaleText
    {
        ws_stNoText = 0,
        ws_stUnitsOnly = 1,
        ws_stFraction = 2
    }

/*    public enum ScaleBarUnits
    {
        ws_suCustom = 0,
        ws_suMeter = 9001,
        ws_suFootUS = 9003,
        ws_suYardSears = 9012,
        ws_suYardIndian = 9013,
        ws_suMileUS = 9035,
        ws_suKilometer = 9036
    }*/

    public enum MapUnits
    {
        ws_muCustom = 0,
        ws_muMeter = 9001,
        ws_muFootUS = 9003,
        ws_muYardSears = 9012,
        ws_muYardIndian = 9013,
        ws_muMileUS = 9035,
        ws_muKilometer = 9036,
        ws_muDegree = 9102
    }

/*
    public enum WSPenStyle
    {
        ws_psSolid = 0,
        ws_psDash = 1,
        ws_psDot = 2,
        ws_psDashDot = 3,
        ws_psDashDotDot = 4,
        ws_psNull = 5,
        ws_psInsideFrame = 6
    }
*/

    public class ScaleBar
    {
        public ScaleBar()
        {
            MapUnit = MapUnits.ws_muCustom;
            BarUnit = MapUnits.ws_muCustom;
            BorderVisible = false;
            BarOutLine = true;
            BarOutlineColor = Color.Black;
            BorderWidth = 1;
            BorderStyle = DashStyle.Solid;
            BorderColor = Color.Black;
            ForeColor = Color.Black;
            BackColor = Color.Green;
            BarColor1 = Color.White;
            BarColor2 = Color.Black;
            BarStyle = ScaleBarStyle.ws_bsStandard;
            NumTics = 4;

            ScaleText = sbScaleText.ws_stNoText;
            BarWidth = 6;
            font = new Font("Arial", 8);
            TransparentBackground = true;
            MarginLeft = 5;
            MarginRight = 15;
            AlignMent = StringAlignment.Near;
        }

        public Color BackColor { get; set; }

        /// <summary>
        /// Color for the first tick
        /// </summary>
        public Color BarColor1
        {
            get
            {
                return barColor1;
            }
            set
            {
                barColor1 = value;
                FireViewChange();
            }
        }

        /// <summary>
        /// Color for the second tick
        /// </summary>
        public Color BarColor2
        {
            get
            {
                return barColor2;
            }
            set
            {
                barColor2 = value;
                FireViewChange();
            }
        }

        public bool BarOutLine { get; set; }
        public Color BarOutlineColor { get; set; }
        public ScaleBarStyle BarStyle { get; set; }

        /// <summary>
        /// This is the unit that's shown with the scalebar. If you are going to use Custom Unit, please call the SetCustomUnit method.
        /// </summary>
        /// <remarks>
        /// Possible values include ws_suCustom(0), ws_suMeter(9001), ws_suFootUS(9003), ws_suYardSears(9012), ws_suYardIndian(9013), ws_suMileUS(9035), and ws_suKilometer(9036).
        /// </remarks>
        public MapUnits BarUnit
        {
            get
            {
                return barUnit;
            }
            set
            {
                barUnit = value;
                GetUnitInformation(barUnit, out m_fBarUnitFactor, out m_strBarUnitName, out m_strBarUnitShortName);
                FireViewChange();
            }
        }

        public int BarWidth
        {
            get
            {
                return barWidth;
            }
            set
            {
                barWidth = value;
                if (barWidth < 1)
                {
                    barWidth = 1;
                }
                FireViewChange();
            }
        }

        public Color BorderColor { get; set; }
        public DashStyle BorderStyle { get; set; }
        public bool BorderVisible { get; set; }
        public int BorderWidth { get; set; }
        public Font font { get; set; }
        public Color ForeColor { get; set; }

        /// <summary>
        /// Set it to true if you are going to use thousand separators and regional decimal point character.
        /// </summary>
        public bool FormatNumber { get; set; }

        /// <summary>
        /// This is the unit used on the map. We use this unit to calculate the map scale.
        /// </summary>
        /// <remarks>
        /// Possible values include ws_muCustom(0), ws_muMeter(9001), ws_muFootUS(9003), ws_muYardSears(9012), ws_muYardIndian(9013), ws_muMileUS(9035), ws_muKilometer(9036), and ws_muDegree(9102).
        /// </remarks>
        public MapUnits MapUnit
        {
            get
            {
                return mapUnit;
            }
            set
            {
                mapUnit = value;
                //update the map unit information
                GetUnitInformation(mapUnit, out m_fMapUnitFactor, out m_strMapUnitName, out m_strMapUnitShortName);
                /*               CalcScale(TODO);
                FireViewChange();
                dirty = true;*/
            }
        }

        /// <summary>
        /// Left margin of the scale bar.
        /// </summary>
        public int MarginLeft { get; set; }

        /// <summary>
        /// Right margin of the scale bar.
        /// </summary>
        public int MarginRight { get; set; }

        /// <summary>
        /// How many tics for the scale bar.
        /// </summary>
        public int NumTics { get; set; }

        /// <summary>
        /// If you know the real scale of the map, you can set the scale directly here; otherwise, use methods SetScaleD( )/SetScale( ).
        /// </summary>
        public double Scale { get; set; }

        /// <remarks>
        /// ws_stNoText means will show on test.
        /// ws_stUnitsOnly will show the unit name with with the bar.
        /// Ws_stFraction will show the scale (1:xxxx).
        /// </remarks>
        public sbScaleText ScaleText { get; set; }

        public StringAlignment AlignMent { get; set; }

        /// <summary>
        /// Calculate the scale for the map. MapWidth is the width of the map extent (in map units). WidthInPixel is the the screen width of the map (in pixels).
        /// </summary>
        /// <param name="mapWidth"></param>
        /// <param name="widthInPixel"></param>
        public void SetScale(double mapWidth, int widthInPixel)
        {
            m_fMapWidth = mapWidth;
            m_nPageWidth = widthInPixel;
/*
            CalcScale(TODO);
            FireViewChange();
*/
        }

        public void SetScaleD(Graphics g, double lon1, double lon2, double lat, int widthInPixel)
        {
            m_fLon1 = lon1;
            m_fLon2 = lon2;
            m_fLat = lat;
            m_nPageWidth = widthInPixel;
/*
            CalcScale(TODO);
            FireViewChange();
*/
        }

        public void SetCustomUnit(double factor, string name, string short_name)
        {
//If the user wants to use customer unit, then the map unit and the bar unit will be the same
//Map Unit
            if (factor <= 0.0) //factor should be >0
            {
                factor = 1.0;
            }
            m_fMapUnitFactor = factor;
            // wcstombs(m_strMapUnitName, name, MaxNameLength);    
            // wcstombs(m_strMapUnitShortName, short_name, MaxNameLength);    

//Bar Unit   
            m_fBarUnitFactor = factor;
            // wcstombs(m_strBarUnitName, name, MaxNameLength);    
            // wcstombs(m_strBarUnitShortName, short_name, MaxNameLength);    

/*
            CalcScale(TODO);
            FireViewChange();
*/
            // return S_OK;
        }

        public void DrawTheControl(Graphics graphics, Rectangle rectangle)
        {
            int nWidthDc = rectangle.Right - rectangle.Left;
            int nHeightDc = rectangle.Bottom - rectangle.Top;
            int nPixelsPerTic;
            double sbUnitsPerTic;

            if (!TransparentBackground)
            {
                DrawBackground(graphics, rectangle);
            }
            if (BorderVisible && BorderWidth > 0)
            {
                DrawBorder(graphics, rectangle);
            }

            CalcScale(graphics);
            //Get the scale first.
            //return if the scale is just too small}
            if (m_fScale < fVerySmall)
            {
                return;
            }
            //Initialize the locale. So the we can use the latest locale setting to show the numbers.   
            //  m_locale.Init();

            //Draw the bar.
            CalcBarScale(graphics, nWidthDc - MarginLeft - MarginRight, NumTics, m_fScale, m_fBarUnitFactor,
                         out nPixelsPerTic, out sbUnitsPerTic);

            int nOffsetX;
            switch (AlignMent)
            {
                case StringAlignment.Near:
                    nOffsetX = rectangle.Left + MarginLeft;
                    break;
                case StringAlignment.Far:
                    nOffsetX = rectangle.Left + (nWidthDc - NumTics*nPixelsPerTic - MarginLeft - MarginRight)/2 +
                               MarginLeft;
                    break;
                default:
                    nOffsetX = rectangle.Left + (nWidthDc - NumTics*nPixelsPerTic - MarginLeft - MarginRight) +
                               MarginLeft;
                    break;
            }

            int nOffsetY = rectangle.Top + (nHeightDc - BarWidth)/2;
            DrawBar(graphics, nPixelsPerTic, nOffsetX, nOffsetY);
            //return;
            DrawVerbalScale(graphics, nWidthDc/2, nOffsetY - GapScaleText_Bar);

            DrawSegmentText(graphics, nOffsetX, nOffsetY + BarWidth + GapBar_SegmentText, NumTics, nPixelsPerTic,
                            sbUnitsPerTic, m_strBarUnitShortName);
        }

        public void DrawVerbalScale(Graphics hdc, int x, int y)
        {
            //Get the scale text.
            string scaleBarText = GetScaleBarText(m_fScale, ScaleText);
            //Draw the text.
            var stringFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Far
            };
            hdc.DrawString(scaleBarText, font, new SolidBrush(Color.Black), x, y, stringFormat);
            //DrawTextWithAlign(hdc, buf, x, y, TA_BOTTOM | TA_CENTER);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="offsetX">offset in  pixels</param>
        /// <param name="offsetY">offset in pixels</param>
        /// <param name="numberOfTicks"></param>
        /// <param name="tickLength"></param>
        /// <param name="barWidth">height of the bar in pixels</param>
        /// <param name="BarColor1">Color for the first tick.</param>
        /// <param name="BarColor2">Color for the second tick.</param>
        /// <param name="drawOutline"></param>
        /// <param name="outlineColor"></param>
        /// 
        public static void DrawBarWithStyle(Graphics graphics, int offsetX, int offsetY, int numberOfTicks,
                                            int tickLength, int barWidth, Color BarColor1, Color BarColor2,
                                            bool drawOutline, Color outlineColor, ScaleBarStyle BarStyle)
        {
            if (barWidth > 1)
            {
                switch (BarStyle)
                {
                    case ScaleBarStyle.ws_bsStandard:
                        DrawTickBarStandard(graphics, numberOfTicks, barWidth, BarColor1, BarColor2,
                                            tickLength, offsetX,
                                            offsetY, drawOutline, outlineColor);
                        break;
                    case ScaleBarStyle.ws_bsMeridian:
                        DrawTickBarMeridian(graphics, numberOfTicks, barWidth, BarColor1, BarColor2,
                                            tickLength, offsetX,
                                            offsetY, drawOutline, outlineColor);
                        break;
                    case ScaleBarStyle.ws_bsMeridian1:
                        DrawTickBarMeridian1(graphics, numberOfTicks, barWidth, BarColor1, BarColor2,
                                             tickLength, offsetX,
                                             offsetY, drawOutline, outlineColor);
                        break;
                    default:
                        DrawTickBarStandard(graphics, numberOfTicks, barWidth, BarColor1, BarColor2,
                                            tickLength, offsetX,
                                            offsetY, drawOutline, outlineColor);
                        break;
                }
            }
            else
            {
                DrawTicLine(graphics, numberOfTicks, BarColor1, BarColor2, tickLength, offsetX, offsetY);
            }
        }

        public static string ToFormattedString(double value, double epsilon)
        {
            string s;
            //double epislon = 0.0000001; // or however near zero you want to consider as zero
            if (Math.Abs(value) > epsilon)
            {
                int digits = (int) Math.Log10(Math.Abs(value));
                // if (digits >= 0) ++digits; // if you care about the exact number
                s = string.Format(Math.Abs(digits) >= 5 ? "{0:0.#E+0}" : "{0:0.##}", value);
            }
            else
            {
                s = "0";
            }
            return s;
        }

        private void FireViewChange()
        {
            //throw new NotImplementedException();
        }

        private void CalcScale(Graphics graphics)
        {
            double fScale;
            fScale = MapUnit == MapUnits.ws_muDegree 
                ? CalcRFScaleD(graphics, m_fLon1, m_fLon2, m_fLat, m_nPageWidth) 
                : CalcRFScale(graphics, m_fMapWidth, m_nPageWidth, m_fMapUnitFactor);
            m_fScale = fScale;
        }

        private static double CalcRFScale(Graphics graphics, double widthMap, double widthPage, double MapUnitFactor)
        {
            var nPxlPerInch = graphics.DpiX;
            double ratio;

            if (widthPage <= 0)
            {
                return 0.0;
            }
            //convert map width to meters
            double fMapWidth = widthMap*MapUnitFactor;
            //convert page width to meters.
            try
            {
                double fPageWidth = widthPage/nPxlPerInch*metersPerInch;
                ratio = Math.Abs(fMapWidth/fPageWidth);
            }
            catch (Exception)
            {
                ratio = 0.0;
            }
            return ratio;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="lon1"></param>
        /// <param name="lon2"></param>
        /// <param name="lat"></param>
        /// <param name="widthPage"></param>
        /// <returns></returns>
        private static double CalcRFScaleD(Graphics graphics, double lon1, double lon2, double lat, double widthPage)
        {
            double distance = GC_Range(lon1, lon2, lat);
            return CalcRFScale(graphics, distance, widthPage, 1);
        }

        private double GC_Range(double lon1, double lat1, double lon2, double lat2)
        {
            double dLon = DiffLongitude(lon1, lon2);
            double arg1 = Math.Sin(lat1*DegreesToRadians)*Math.Sin(lat2*DegreesToRadians);
            double arg2 = Math.Cos(lat1*DegreesToRadians)*Math.Cos(lat2*DegreesToRadians)*
                          Math.Cos(dLon*DegreesToRadians);

            return metersPerDegreeAtEquator*Math.Acos(arg1 + arg2)/DegreesToRadians;
        }

        private static double DiffLongitude(double lon1, double lon2)
        {
            if (lon1 > 180.0)
            {
                lon1 = 360.0 - lon1;
            }
            if (lon2 > 180.0)
            {
                lon2 = 360.0 - lon2;
            }

            if ((lon1 >= 0.0) && (lon2 >= 0.0))
            {
                return lon2 - lon1;
            }
            if ((lon1 < 0.0) && (lon2 < 0.0))
            {
                return lon2 - lon1;
            }
            // different hemispheres
            if (lon1 < 0)
            {
                lon1 = -1*lon1;
            }
            if (lon2 < 0)
            {
                lon2 = -1*lon2;
            }
            double diff = lon1 + lon2;
            if (diff > 180.0)
            {
                diff = 360.0 - diff;
            }
            return diff;
        }

        /// <summary>
        /// Calculate the distance between 2 points: (lon1,lat) and (lon2,lat). 
        /// </summary>
        /// <param name="lon1"></param>
        /// <param name="lon2"></param>
        /// <param name="lat"></param>
        /// <returns></returns>
        private static double GC_Range(double lon1, double lon2, double lat)
        {
            double dLon = DiffLongitude(lon1, lon2);
            lat = Math.Abs(lat);
            if (lat >= 90.0)
            {
                lat = 89.999;
            }
            double distance = Math.Cos(lat*DegreesToRadians)*metersPerDegreeAtEquator*dLon;
            return distance;
        }

        private void GetMapUnitInfo(out double factor, out string name, out string shortName)
        {
            factor = m_fMapUnitFactor;
            name = m_strMapUnitName;
            shortName = m_strMapUnitShortName;
        }

        private void GetBarUnitInfo(out double factor, out string name, out string shortName)
        {
            factor = m_fBarUnitFactor;
            name = m_strBarUnitName;
            shortName = m_strBarUnitShortName;
        }

        private static void GetUnitInformation(MapUnits mapUnit, out double factor, out string name,
                                               out string shortName)
        {
            switch (mapUnit)
            {
                case MapUnits.ws_muCustom:
                    factor = 1.0;
                    name = "Unknown";
                    shortName = "Unknown";
                    break;
                case MapUnits.ws_muMeter:
                    factor = 1.0;
                    name = "Meters";
                    shortName = "m";
                    break;
                case MapUnits.ws_muFootUS:
                    factor = 0.30480061;
                    name = "Feet";
                    shortName = "f";
                    break;
                case MapUnits.ws_muYardIndian:
                    factor = 0.914398415;
                    name = "Yards";
                    shortName = "yd";
                    break;
                case MapUnits.ws_muYardSears:
                    factor = 0.914398531;
                    name = "Yard";
                    shortName = "yd";
                    break;
                case MapUnits.ws_muMileUS:
                    factor = 1609.347219;
                    name = "Miles";
                    shortName = "mi";
                    break;
                case MapUnits.ws_muKilometer:
                    factor = 1000.0;
                    name = "Kilometers";
                    shortName = "km";
                    break;
                case MapUnits.ws_muDegree:
                    factor = 0.0175;
                    name = "Degrees";
                    shortName = "d";
                    break;
                default:
                    factor = 1.0;
                    name = "Unknown";
                    shortName = "Unknown";
                    break;
            }
        }

        private void DrawBar(Graphics graphics, int nTicLength, int offsetX, int offsetY)
        {
            Color cr1 = BarColor1;
            Color cr2 = BarColor2;
            Color crOutline = BarOutlineColor;
            DrawBarWithStyle(graphics, offsetX, offsetY, NumTics, nTicLength, BarWidth, cr1, cr2, BarOutLine, crOutline,
                             BarStyle);
        }

        private void DrawBorder(Graphics graphics, Rectangle rc)
        {
            //resize the rectangle to draw the border 
/*
            var rc2 = new Rectangle(rc.Left + BorderWidth/2, rc.Top + BorderWidth/2, rc.Width - BorderWidth + 1/2,
                                    rc.Height - BorderWidth + 1/2);
*/

            var pen = new Pen(BorderColor)
            {
                Width = BorderWidth, DashStyle = BorderStyle
            };
            graphics.DrawRectangle(pen, rc);
            //FillRectangle(hdc, rc, BS_NULL, 0, nPenStyle, crPen, m_nBorderWidth);
        }

        private void DrawBackground(Graphics graphics, Rectangle rectangle)
        {
            //increase the right and bottom 
            var rectangle1 = new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width + 1, rectangle.Height + 1);

            graphics.FillRectangle(new SolidBrush(BackColor), rectangle1);

            /* var pen = new Pen(BorderColor)
                          {
                              DashStyle = System.Drawing.Drawing2D.DashStyle.Solid,
                              Width = BorderWidth
                          };
            graphics.DrawRectangle(pen, rectangle1);*/
        }

        private static void CalcBarScale(Graphics g, int nWidthDC, int nNumTics, double fMapScale, double fBarUnitFactor,
                                         out int pixelsPerTic, out double sbUnitsPerTic)
        {
            int nMinPixelsPerTic = nWidthDC/(nNumTics*2);
            double fBarScale = fMapScale/fBarUnitFactor;
            var nPixelsPerInch = (int) g.DpiX;
            double fBarUnitsPerPixel = fBarScale*metersPerInch/nPixelsPerInch;

            //calculate the result
            sbUnitsPerTic = nMinPixelsPerTic*fBarUnitsPerPixel;
            sbUnitsPerTic = GetRoundIncrement(sbUnitsPerTic);
            pixelsPerTic = (int) (sbUnitsPerTic/fBarUnitsPerPixel);
        }

        private static double GetRoundIncrement(double startValue)
        {
            int nPower; //power of 10. Range of -5 to 10 gives huge scale range.
            double dCandidate = double.MaxValue; //Candidate value for new interval.
            for (nPower = PowerRangeMin; nPower <= PowerRangeMax; nPower++)
            {
                double dMultiplier = Math.Pow(10.0, nPower); //Mulitiplier, =10^exp, to apply to nice numbers.
                for (int i = 0; i < nNiceNumber; i++)
                {
                    dCandidate = NiceNumberArray[i]*dMultiplier;
                    if (dCandidate > startValue)
                    {
                        return dCandidate;
                    }
                }
            }
            return dCandidate; //return the maximum
        }

        private void DrawSegmentText(Graphics graphics, int x, int y, int NumTics, int TicWidth, double SBUnitsPerTic,
                                     string str_unit)
        {
            int i;
            //int precision;

            var f = new StringFormat
            {
                Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near
            };

//  ostrstream s;
            double value;
            //int Align;
            //char buf[MAX_LEN];
            Brush brush = new SolidBrush(Color.Black);

            //? DrawTextWithAlign( "0", x, y, TA_TOP | TA_LEFT) : DrawTextWithAlign( str_unit, x, y, TA_TOP | TA_LEFT);
            graphics.DrawString(ScaleText == sbScaleText.ws_stUnitsOnly ? "0" : str_unit, font, brush, x, y, f);

//Set the output format.

            //precision = PresitionOfSegmentText(SBUnitsPerTic);
            //s.precision(precision);
            //s.setf(ios::left, ios::adjustfield);
            //s.setf(ios::
            //fixed,
            //ios::floatfield)

            f.Alignment = StringAlignment.Center;
            f.LineAlignment = StringAlignment.Near;
            for (i = 1; i <= NumTics; i++)
            {
                value = SBUnitsPerTic*i;
                //s << value << ends;

                // Align = TA_TOP | TA_CENTER;
                if (FormatNumber)
                {
                    string buf = ToFormattedString(value, 0.000001);
                    //format the number
                    //memset(buf, 0, MAX_LEN);
                    //m_locale.FormatNumberStr(s.str(), buf, MAX_LEN-1);
                    graphics.DrawString(buf, font, brush, x + TicWidth*i, y, f);
                    //DrawTextWithAlign(hdc, buf, x + TicWidth * i, y, Align); 
                }
                else
                {
                    graphics.DrawString(ToFormattedString(value, 0.000001), font, brush, x + TicWidth*i, y, f);
                }
                //DrawTextWithAlign(hdc, s.str(), x + TicWidth * i, y, Align); 
            }
        }

        private string GetScaleBarText(double scale, sbScaleText scale_text)
        {
            var epsilon = 0.01;

            //text
            if (scale_text == sbScaleText.ws_stUnitsOnly)
            {
                //unit
                return m_strBarUnitName;
            }
            else if (scale_text == sbScaleText.ws_stFraction)
            {
                //scale(1:xxxx)
                return "1:" + ToFormattedString(scale, epsilon);
            }
            return "";
        }

        private static void DrawTickBarMeridian1(Graphics graphics, int nNumTics, int nBarWidth, Color crBar1,
                                                 Color crBar2, int nTicLength, int nOffsetX, int nOffsetY, bool outline,
                                                 Color crOutline)
        {
            Color cr1;
            Color cr2;
            //Create pens.
            if (outline)
            {
                cr1 = crOutline;
                cr2 = crOutline;
            }
            else
            {
                cr1 = crBar1;
                cr2 = crBar2;
            }
            var hPen1 = new Pen(cr1, 1);
            var hPen2 = new Pen(cr2, 1);

            //Create brushes.
            Brush lb1 = new SolidBrush(crBar1);
            Brush lb2 = new SolidBrush(crBar2);

            var x = nOffsetX;
            var y1 = nOffsetY;

            var y12 = y1 + nBarWidth/2;
            for (int i = 0; i < nNumTics; i++)
            {
                if (i%2 != 0)
                {
                    graphics.DrawRectangle(hPen2, x, y1, nTicLength + 1, nBarWidth);
                    graphics.FillRectangle(lb2, x, y1, nTicLength + 1, nBarWidth);
                }
                else
                {
                    graphics.DrawRectangle(hPen1, x, y1, nTicLength + 1, nBarWidth/2);
                    graphics.FillRectangle(lb1, x, y12, nTicLength + 1, nBarWidth/2);
                }
                x += nTicLength;
            }
        }

        private static void DrawTickBarMeridian(Graphics graphics, int nNumTics, int nBarWidth, Color BarColor1,
                                                Color BarColor2, int nTicLength, int nOffsetX,
                                                int nOffsetY, bool outline, Color crOutline)
        {
            Color cr1;
            Color cr2;

            //Create pens.
            if (outline)
            {
                cr1 = crOutline;
                cr2 = crOutline;
            }
            else
            {
                cr1 = BarColor1;
                cr2 = BarColor2;
            }
            var pen1 = new Pen(cr1, 1);
            var pen2 = new Pen(cr2, 1);
            var lb1 = new SolidBrush(BarColor1);
            var lb2 = new SolidBrush(BarColor2);
            //Create brushes.

            var x = nOffsetX;
            var y1 = nOffsetY;
            var y2 = y1 + nBarWidth;
            var y12 = y1 + nBarWidth/2;
            for (int i = 0; i < nNumTics; i++)
            {
                if (i%2 != 0)
                {
                    graphics.DrawRectangle(pen2, x, y1, nTicLength + 1, nBarWidth/2);
                    graphics.FillRectangle(lb2, x, y1, nTicLength + 1, nBarWidth/2);
                    graphics.DrawRectangle(pen1, x, y12, nTicLength + 1, nBarWidth/2);
                    graphics.FillRectangle(lb1, x, y12, nTicLength + 1, nBarWidth/2);
                }
                else
                {
                    graphics.DrawRectangle(pen1, x, y1, nTicLength + 1, nBarWidth/2);
                    graphics.FillRectangle(lb1, x, y1, nTicLength + 1, nBarWidth/2);
                    graphics.DrawRectangle(pen2, x, y12, nTicLength + 1, nBarWidth/2);
                    graphics.FillRectangle(lb2, x, y12, nTicLength + 1, nBarWidth/2);
                }
                x += nTicLength;
            }
        }

        private static void DrawTickBarStandard(Graphics graphics, int numberOfTicks, int barWidth, Color barColor1,
                                                Color barColor2, int tickLength, int offsetX, int offsetY,
                                                bool drawOutline,
                                                Color crOutline)
        {
            Color cr1;
            Color cr2;

            if (drawOutline)
            {
                cr1 = crOutline;
                cr2 = crOutline;
            }
            else
            {
                cr1 = barColor1;
                cr2 = barColor2;
            }

            var pen1 = new Pen(new SolidBrush(cr1), 1);
            var pen2 = new Pen(new SolidBrush(cr2), 1);
            Brush lb1 = new SolidBrush(barColor1);
            Brush lb2 = new SolidBrush(barColor2);
            var x = offsetX;
            var y1 = offsetY;

            for (int i = 0; i < numberOfTicks; i++)
            {
                if (i%2 != 0)
                {
                    graphics.DrawRectangle(pen1, x, y1, tickLength, barWidth);
                    graphics.FillRectangle(lb1, x, y1, tickLength, barWidth);
                }
                else
                {
                    graphics.DrawRectangle(pen2, x, y1, tickLength, barWidth);
                    graphics.FillRectangle(lb2, x, y1, tickLength, barWidth);
                }

                x += tickLength;
            }
        }

        private static void DrawTicLine(Graphics graphics, int nNumTics, Color crBar1, Color crBar2, int nTicLength,
                                        int nOffsetX, int nOffsetY)
        {
            var pen = new Pen(crBar1, 1);
            var pen2 = new Pen(crBar2, 1);

            var x = nOffsetX;
            var y = nOffsetY;
            for (int i = 0; i < nNumTics; i++)
            {
                graphics.DrawLine(i%2 != 0 ? pen : pen2, x, y, x + nOffsetX, y);
                x += nTicLength;
            }
        }

        #region fields

        private double m_fMapWidth = 0.0; //the width in map unit
        private long m_nPageWidth = 0; //the width in pixel for the map on screen
        private double m_fLon1 = 0.0; //longitude 1 for map that is lat/lon
        private double m_fLon2 = 0.0; //longitude 2 for map that is lat/lon
        private double m_fLat = 0.0; //latitude 1 for map that is lat/lon 
        private double m_fScale = 0.0; //mapscale

        private double m_fBarUnitFactor;
        private double m_fMapUnitFactor;
        private string m_strMapUnitName;
        private string m_strMapUnitShortName;

        public bool TransparentBackground { get; set; }
        private string m_strBarUnitShortName = "";
        private string m_strBarUnitName;

        #endregion

        #region constants

        private const double DegreesToRadians = 0.01745329252; // Convert Degrees to Radians
        private const double metersPerInch = 0.0254;
        private const double metersPerMile = 1609.347219;
        private const double milesPerDegreeAtEquator = 69.171;
        private const double metersPerDegreeAtEquator = metersPerMile*milesPerDegreeAtEquator;
        private const double fVerySmall = 0.00000001;
        private const int GapScaleText_Bar = 3;
        private const int GapBar_SegmentText = 1;
        private const int PowerRangeMin = -5;
        private const int PowerRangeMax = 10;
        private const int nNiceNumber = 4;

        private static readonly double[] NiceNumberArray = new double[]
        {
            1,
            2,
            2.5,
            5
        };

        private Color barColor1;
        private Color barColor2;
        private int barWidth;
        private MapUnits mapUnit;
        private MapUnits barUnit;

        #endregion
    }
}