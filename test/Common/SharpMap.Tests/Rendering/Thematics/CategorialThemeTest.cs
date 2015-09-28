using System;
using System.Drawing;
using DelftTools.Utils.Collections.Generic;
using NUnit.Framework;
using SharpMap.Api;
using SharpMap.Rendering.Thematics;
using SharpMap.Styles;

namespace SharpMap.Tests.Rendering.Thematics
{
    [TestFixture]
    public class CategorialThemeTest
    {
        [Test]
        public void GetFillColor()
        {
            var theme = new CategorialTheme();
            var themeItem = new CategorialThemeItem {Style = new VectorStyle {Fill = new SolidBrush(Color.Red)}, Value = 1.0};
            theme.ThemeItems = new EventedList<IThemeItem>(new[] { themeItem });

            const int valueAsInt = 1;
            const float valueAsFloat = 1.0f;

            Assert.AreEqual(Color.Transparent, theme.GetFillColor(0.5));
            Assert.AreEqual(Color.Red, theme.GetFillColor(1.0));
            Assert.AreEqual(Color.Red, theme.GetFillColor(valueAsInt));
            Assert.AreEqual(Color.Red, theme.GetFillColor(valueAsFloat));
            Assert.AreEqual(Color.Red, theme.GetFillColor(new ConvertableObject()));
            Assert.AreEqual(Color.Transparent, theme.GetFillColor(1.5));
            Assert.AreEqual(Color.Transparent, theme.GetFillColor(new ComparableObject()));

            theme.ThemeItems = new EventedList<IThemeItem>();
            Assert.AreEqual(Color.Transparent, theme.GetFillColor(0.5));
            Assert.AreEqual(Color.Transparent, theme.GetFillColor(1.0));
            Assert.AreEqual(Color.Transparent, theme.GetFillColor(valueAsInt));
            Assert.AreEqual(Color.Transparent, theme.GetFillColor(valueAsFloat));
            Assert.AreEqual(Color.Transparent, theme.GetFillColor(new ConvertableObject()));
            Assert.AreEqual(Color.Transparent, theme.GetFillColor(1.5));
            Assert.AreEqual(Color.Transparent, theme.GetFillColor(new ComparableObject()));
        }

        #region Nested Type - ComparibleObject

        class ComparableObject : IComparable
        {
            public int CompareTo(object obj)
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Nested Type - ConvertableObject

        class ConvertableObject : IConvertible, IComparable
        {
            public TypeCode GetTypeCode()
            {
                throw new NotImplementedException();
            }

            public bool ToBoolean(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public char ToChar(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public sbyte ToSByte(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public byte ToByte(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public short ToInt16(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public ushort ToUInt16(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public int ToInt32(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public uint ToUInt32(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public long ToInt64(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public ulong ToUInt64(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public float ToSingle(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public double ToDouble(IFormatProvider provider)
            {
                return 1.0;
            }

            public decimal ToDecimal(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public DateTime ToDateTime(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public string ToString(IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public object ToType(Type conversionType, IFormatProvider provider)
            {
                throw new NotImplementedException();
            }

            public int CompareTo(object obj)
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}