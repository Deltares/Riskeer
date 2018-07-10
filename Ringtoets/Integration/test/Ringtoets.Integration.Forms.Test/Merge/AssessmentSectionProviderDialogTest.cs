using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.Merge;

namespace Ringtoets.Integration.Forms.Test.Merge
{
    [TestFixture]
    public class AssessmentSectionProviderDialogTest
    {
        [Test]
        public void Constructor_AssessmentSectionsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new AssessmentSectionProviderDialog(null, dialogParent);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSections", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_DialogParentNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AssessmentSectionProviderDialog(Enumerable.Empty<AssessmentSection>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("dialogParent", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.Stub<IWin32Window>();
            mocks.ReplayAll();

            IEnumerable<AssessmentSection> assessmentSections = Enumerable.Empty<AssessmentSection>();

            // Call
            using (var dialog = new AssessmentSectionProviderDialog(assessmentSections, dialogParent))
            {
                // Assert
                Assert.IsInstanceOf<DialogBase>(dialog);
                Assert.IsInstanceOf<IMergeDataProvider>(dialog);

                Icon icon = BitmapToIcon(Resources.SelectionDialogIcon);
                Bitmap expectedImage = icon.ToBitmap();
                Bitmap actualImage = dialog.Icon.ToBitmap();
                TestHelper.AssertImagesAreEqual(expectedImage, actualImage);
            }

            mocks.VerifyAll();
        }

        private static Icon BitmapToIcon(Bitmap icon)
        {
            return Icon.FromHandle(icon.GetHicon());
        }
    }
}