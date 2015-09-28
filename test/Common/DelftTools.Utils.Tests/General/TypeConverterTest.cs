using System;
using System.ComponentModel;
using System.Globalization;
using NUnit.Framework;

namespace DelftTools.Utils.Tests.General
{
    [TestFixture]
    public class TypeConverterTest
    {
        [Test]
        public void ConversionTest()
        {
            Assert.AreEqual(12, TypeConverter.ConvertValueToTargetType<int>("12"));

            IFormatProvider providerEN = new CultureInfo("en-US", true);
            IFormatProvider providerTR = new CultureInfo("tr-TR", true);

            //    DateTime x = TConverter.ConvertValueToTargetType<DateTime>("12.10.2007", providerTR); // ddMMyyyy
            //    DateTime y = TConverter.ConvertValueToTargetType<DateTime>("12.10.2007", providerEN); // MMddyyyy

            TypeConverter.RegisterTypeConverter<Version, VersionConverter>();

            Assert.AreEqual(123, TypeConverter.ConvertValueToTargetType<int?>("123"));

            DateTime dt = TypeConverter.ConvertValueToTargetType<DateTime>("20:33");
            Assert.AreEqual(20, dt.Hour);
            Assert.AreEqual(33, dt.Minute);

            //char c = TConverter.ConvertValueToTargetType<char>("x");
            Assert.AreEqual('x', TypeConverter.ConvertValueToTargetType<char>("x"));

            Guid g = TypeConverter.ConvertValueToTargetType<Guid>("{32F92EEB-A703-4eb7-A9F8-62E09F87D03F}");
            

            Version v = TypeConverter.ConvertValueToTargetType<Version>("1.2.3.4");
            Assert.AreEqual(1, v.Major);
            Assert.AreEqual(2, v.Minor);
            Assert.AreEqual(3, v.Build);
            Assert.AreEqual(4, v.Revision);

            Assert.IsNull(TypeConverter.ConvertValueToTargetType<DateTime?>(null));
        }
    }

    public class VersionConverter : System.ComponentModel.TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string strvalue = value as string;

            if (strvalue != null)
            {
                return new Version(strvalue);
            }

            else
            {
                return new Version();
            }
        }
    }
}