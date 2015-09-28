using System.ComponentModel;
using System.Drawing;
using DelftTools.TestUtils;
using log4net;
using NUnit.Framework;

namespace DelftTools.Utils.Tests.Reflection
{
    [TestFixture]
    public class TypeConverterTest
    {
        private static readonly ILog log = LogManager.GetLogger(typeof (TypeConverterTest));
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
        /// http://jfoscoding.blogspot.com/2007/11/easy-mistake-using-typeconverter.html
        /// </summary>
        [Test]
        
        public void test1()
        {
            Rectangle rect = new Rectangle(10, 12, 14, 16);

            // Wrong
            System.ComponentModel.TypeConverter baseConverter = new System.ComponentModel.TypeConverter();
            string sample1 = baseConverter.ConvertToString(rect);

            // Right
            System.ComponentModel.TypeConverter rectSpecificConverter = TypeDescriptor.GetConverter(rect);
            string sample2 = rectSpecificConverter.ConvertToString(rect);

            log.DebugFormat("From new TypeConverter() {0}\r\n", sample1);
            log.DebugFormat("From TypeDescriptor.GetConverter() {0}\r\n", sample2);
            log.DebugFormat("From rect.ToString() {0}\r\n", rect.ToString());
        }
    }
}