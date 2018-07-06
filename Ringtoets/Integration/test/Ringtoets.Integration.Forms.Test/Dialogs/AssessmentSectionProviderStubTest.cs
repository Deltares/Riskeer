// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Integration.Forms.Dialogs;
using Ringtoets.Integration.Service.Merge;

namespace Ringtoets.Integration.Forms.Test.Dialogs
{
    [TestFixture]
    public class AssessmentSectionProviderStubTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.StrictMock<IWin32Window>();
            mocks.ReplayAll();

            // Call
            using (var provider = new AssessmentSectionProviderStub(dialogParent))
            {
                // Assert
                Assert.IsInstanceOf<DialogBase>(provider);
                Assert.IsInstanceOf<IAssessmentSectionProvider>(provider);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GetAssessmentSections_Always_ShowsDialog()
        {
            // Setup
            using (var dialogParent = new Form())
            using (var provider = new AssessmentSectionProviderStub(dialogParent))
            {
                // Call
                provider.GetAssessmentSections(null);

                // Assert
                var invalidProjectButtonSelect = (Button)new ButtonTester("invalidProjectButton", provider).TheObject;
                Assert.AreEqual("Selecteer fout project", invalidProjectButtonSelect.Text);
                Assert.IsTrue(invalidProjectButtonSelect.Enabled);

                var noMatchButtonSelect = (Button)new ButtonTester("noMatchButton", provider).TheObject;
                Assert.AreEqual("Selecteer project zonder overeenkomende trajecten", noMatchButtonSelect.Text);
                Assert.IsTrue(noMatchButtonSelect.Enabled);

                var matchButtonSelect = (Button)new ButtonTester("matchButton", provider).TheObject;
                Assert.AreEqual("Selecteer project met overeenkomend traject", matchButtonSelect.Text);
                Assert.IsTrue(matchButtonSelect.Enabled);

                var buttonCancel = (Button)new ButtonTester("cancelButton", provider).TheObject;
                Assert.AreEqual("Annuleren", buttonCancel.Text);

                Assert.AreEqual(1, provider.MinimumSize.Width);
                Assert.AreEqual(1, provider.MinimumSize.Height);
            }
        }
    }
}