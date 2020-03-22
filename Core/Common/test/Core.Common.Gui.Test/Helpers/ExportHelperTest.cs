using System;
using Core.Common.Gui.Helpers;
using Core.Common.Util;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.Helpers
{
    [TestFixture]
    public class ExportHelperTest
    {
        [Test]
        public void GetFilePath_InquiryHelperNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ExportHelper.GetFilePath(null, new FileFilterGenerator());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("inquiryHelper", exception.ParamName);
        }

        [Test]
        public void GetFilePath_FileFilterGeneratorNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            // Call
            void Call() => ExportHelper.GetFilePath(inquiryHelper, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("fileFilterGenerator", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(null)]
        [TestCase("C:/test")]
        public void GetFilePath_Always_ReturnsSelectedFilePath (string expectedFilePath)
        {
            // Setup
            var  fileFilterGenerator = new FileFilterGenerator("testExtension", "testDescription");

            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            inquiryHelper.Expect(ih => ih.GetTargetFileLocation(fileFilterGenerator.Filter, null))
                         .Return(expectedFilePath);
            mocks.ReplayAll();

            // Call
            string filePath = ExportHelper.GetFilePath(inquiryHelper, fileFilterGenerator);

            // Assert
            Assert.AreEqual(expectedFilePath, filePath);
            mocks.VerifyAll();
        }
    }
}