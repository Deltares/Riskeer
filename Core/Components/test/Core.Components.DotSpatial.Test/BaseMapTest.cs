using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Core.Common.TestUtil;
using Core.Components.DotSpatial.Properties;
using DotSpatial.Controls;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Components.DotSpatial.Test
{
    [TestFixture]
    public class BaseMapTest
    {
        [Test]
        public void DefaultConstructor_PropertiesSet()
        {
            // Setup
            var mocks = new MockRepository();
            var map = mocks.StrictMock<Map>();
            mocks.ReplayAll();

            // Call
            var baseMap = new BaseMap();

            // Assert
            Assert.IsInstanceOf<Control>(baseMap);
            Assert.IsNotNull(baseMap.Data);
            Assert.IsNotNull(map);
            mocks.VerifyAll();
        }

        [Test]
        public void Data_NotNull_DataSet()
        {
            // Setup
            var map = new BaseMap();
            var fileData = new Collection<string> { "1", "2", "3" };

            // Call
            map.Data = fileData;

            //Assert
            CollectionAssert.AreEqual(fileData, map.Data);
        }

        [Test]
        public void Data_NotKnownFileType_HandlesApplicationExceptionAndWriteLog()
        {
            // Setup
            var map = new BaseMap();

            // Call / Assert
            Action action = () => map.Data = new Collection<string> { "test" };
            TestHelper.AssertLogMessageIsGenerated(action, Resources.BaseMap_LoadData_Cannot_open_file_extension);
        }

        [Test]
        public void Data_FileNotFound_HandlesFileNotFoundExceptionAndWriteLog()
        {
            // Setup
            var map = new BaseMap();
            var fileName = "test.shp";
            var completeFilePath = string.Format("{0}\\{1}", Environment.CurrentDirectory, fileName);
            var expectedLog = string.Format(Resources.BaseMap_LoadData_File_loading_failded__The_file__0__could_not_be_found, completeFilePath);

            // Call / Assert
            Action action = () => map.Data = new Collection<string> { fileName };
            TestHelper.AssertLogMessageIsGenerated(action, expectedLog);
        }
    }
}