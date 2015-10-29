using System.Collections.Generic;
using System.Drawing;
using Core.Common.TestUtils;
using Core.GIS.SharpMap.Rendering.Thematics;
using Core.GIS.SharpMap.Styles;
using NUnit.Framework;

namespace Core.GIS.SharpMap.Tests.Rendering.Thematics
{
    [TestFixture]
    public class GradientThemeTest
    {
        [Test]
        public void ReturnMaxColorForMaxValue()
        {
            var minVectorStyle = new VectorStyle
            {
                Fill = new SolidBrush(Color.Red)
            };
            var maxVectorStyle = new VectorStyle
            {
                Fill = new SolidBrush(Color.Blue)
            };

            var theme = new GradientTheme("red to blue", 10.0, 100.123, minVectorStyle, maxVectorStyle, null, null, null);

            var color = theme.GetFillColor(100.123);

            AssertColor(Color.Blue, color);
        }

        [Test]
        public void GenerateThemeWithMaxDoubleAndMinDoubleValue()
        {
            var minVectorStyle = new VectorStyle
            {
                Fill = new SolidBrush(Color.Red)
            };
            var maxVectorStyle = new VectorStyle
            {
                Fill = new SolidBrush(Color.Blue)
            };

            var theme = new GradientTheme("red to blue", double.MinValue, double.MaxValue, minVectorStyle, maxVectorStyle, null, null, null);

            var color = theme.GetFillColor(100);

            AssertColor(Color.FromArgb(255, 127, 0, 127), color);
        }

        [Test]
        public void ClonePerformanceTest()
        {
            var colorBlend = new ColorBlend(new[]
            {
                Color.Black,
                Color.White
            }, new[]
            {
                0.0f,
                1.0f
            });
            var gradientTheme = new GradientTheme("aa", 0, 20, new VectorStyle(), new VectorStyle(), colorBlend,
                                                  colorBlend, colorBlend, 5)
            {
                NoDataValues = new List<double>
                {
                    -9999
                }
            };
            TestHelper.AssertIsFasterThan(30, () => gradientTheme.Clone());
        }

        [Test]
        public void CloneGradientThemeWithNoDataValues()
        {
            var colorBlend = new ColorBlend(new[]
            {
                Color.Black,
                Color.White
            }, new[]
            {
                0.0f,
                1.0f
            });
            var gradientTheme = new GradientTheme("aa", 0, 20, new VectorStyle(), new VectorStyle(), colorBlend,
                                                  colorBlend, colorBlend, 5)
            {
                NoDataValues = new List<double>
                {
                    -9999
                },
                UseCustomRange = true
            };

            var gradientThemeClone = (GradientTheme) gradientTheme.Clone();

            Assert.IsTrue(gradientThemeClone.UseCustomRange);
            Assert.AreEqual(gradientTheme.NoDataValues, (gradientThemeClone).NoDataValues);
            Assert.AreEqual(5, gradientThemeClone.NumberOfClasses);
            Assert.AreEqual(2, gradientThemeClone.FillColorBlend.Colors.Length);
        }

        [Test]
        public void GenerateThemeItems()
        {
            var colorBlend = new ColorBlend(new[]
            {
                Color.Black,
                Color.White
            }, new[]
            {
                0.0f,
                1.0f
            });
            var gradientTheme = new GradientTheme("aa", 0, 3, new VectorStyle(), new VectorStyle(), colorBlend,
                                                  colorBlend, colorBlend, 3)
            {
                NoDataValues = new List<double>
                {
                    -9999
                }
            };
            //assert 3 items were generated..at 0,1.5 and 3
            Assert.AreEqual(3, gradientTheme.ThemeItems.Count);
            Assert.AreEqual("0", gradientTheme.ThemeItems[0].Range);
            //use toString to make sure the machines decimal separator is used
            Assert.AreEqual(1.5.ToString(), gradientTheme.ThemeItems[1].Range);
            Assert.AreEqual("3", gradientTheme.ThemeItems[2].Range);
        }

        [Test]
        public void GradientThemeScaleToTest()
        {
            var minStyle = new VectorStyle
            {
                Fill = new SolidBrush(Color.Red)
            };
            var maxStyle = new VectorStyle
            {
                Fill = new SolidBrush(Color.Blue)
            };

            var theme = new GradientTheme("NotLinkedToVariable", 0.0, 10.0, minStyle, maxStyle, null, null, null, 3);
            var colorMid = theme.GetFillColor(5.0);

            theme.ScaleTo(-10.0, 30.0);

            Assert.AreEqual(-10.0, theme.Min);
            Assert.AreEqual(30.0, theme.Max);
            Assert.AreEqual(3, theme.NumberOfClasses);
            Assert.AreEqual(3, theme.ThemeItems.Count);
            Assert.IsNull(theme.TextColorBlend);
            Assert.IsNull(theme.LineColorBlend);
            Assert.IsNull(theme.FillColorBlend);

            AssertColor(Color.Red, theme.GetFillColor(-10.0));
            AssertColor(colorMid, theme.GetFillColor(10.0));
            AssertColor(Color.Blue, theme.GetFillColor(30.0));

            theme.ScaleTo(-12.3, -12.3);

            Assert.AreEqual(-12.3, theme.Min);
            Assert.AreEqual(-12.3, theme.Max);
            Assert.AreEqual(3, theme.NumberOfClasses);
            Assert.AreEqual(2, theme.ThemeItems.Count); // Because max == min, only these values are defined and therefore < NumberOfClasses
            Assert.IsNull(theme.TextColorBlend);
            Assert.IsNull(theme.LineColorBlend);
            Assert.IsNull(theme.FillColorBlend);

            AssertColor(Color.Red, theme.GetFillColor(-12.3));
        }

        [Test]
        public void GradientThemeScaleToWithColorBlends()
        {
            var blend = new ColorBlend(new[]
            {
                Color.Black,
                Color.White
            }, new[]
            {
                0f,
                1f
            });
            var theme = ThemeFactory.CreateGradientTheme("", null, blend, 0.0, 5.0, 3, 3, false, true, 3);

            AssertColor(Color.Black, theme.GetFillColor(-1.0));
            AssertColor(Color.Black, theme.GetFillColor(0.0));
            AssertColor(Color.FromArgb(255, 64, 64, 64), theme.GetFillColor(1.25));
            AssertColor(Color.FromArgb(255, 128, 128, 128), theme.GetFillColor(2.5));
            AssertColor(Color.FromArgb(255, 191, 191, 191), theme.GetFillColor(3.75));
            AssertColor(Color.White, theme.GetFillColor(5.0));
            AssertColor(Color.White, theme.GetFillColor(6.0));

            theme.ScaleTo(-2.0, 1.0);

            AssertColor(Color.Black, theme.GetFillColor(-3.0));
            AssertColor(Color.Black, theme.GetFillColor(-2.0));
            AssertColor(Color.FromArgb(255, 64, 64, 64), theme.GetFillColor(-1.25));
            AssertColor(Color.FromArgb(255, 128, 128, 128), theme.GetFillColor(-0.5));
            AssertColor(Color.FromArgb(255, 191, 191, 191), theme.GetFillColor(0.25));
            AssertColor(Color.White, theme.GetFillColor(1.0));
            AssertColor(Color.White, theme.GetFillColor(2.0));
        }

        [Test]
        public void GetStyleForNoDataValue()
        {
            var minVectorStyle = new VectorStyle
            {
                Fill = new SolidBrush(Color.Red)
            };
            var maxVectorStyle = new VectorStyle
            {
                Fill = new SolidBrush(Color.Blue)
            };
            var theme = new GradientTheme("red to blue", 10.0, 100.123, minVectorStyle, maxVectorStyle, null, null, null)
            {
                NoDataValues = new List<double>
                {
                    12.3
                }
            };

            var result = (VectorStyle) theme.GetStyle(10.0);
            AssertColor(Color.Red, ((SolidBrush) result.Fill).Color); // Expecting SolidBrush
            AssertColor(Color.FromArgb(255, 138, 43, 226), result.Line.Color);

            result = (VectorStyle) theme.GetStyle(12.3);
            AssertColor(Color.Transparent, ((SolidBrush) result.Fill).Color); // Expecting SolidBrush
            AssertColor(Color.Transparent, result.Line.Color);
        }

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            LogHelper.ConfigureLogging();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            LogHelper.ResetLogging();
        }

        /// <summary>
        /// Asserts that <paramref name="expectedColor"/> argb values are the same as those of
        /// <paramref name="actualColor"/>
        /// </summary>
        /// <param name="expectedColor"></param>
        /// <param name="actualColor"></param>
        private static void AssertColor(Color expectedColor, Color actualColor)
        {
            Assert.AreEqual(expectedColor.A, actualColor.A);
            Assert.AreEqual(expectedColor.R, actualColor.R);
            Assert.AreEqual(expectedColor.G, actualColor.G);
            Assert.AreEqual(expectedColor.B, actualColor.B);
        }
    }
}