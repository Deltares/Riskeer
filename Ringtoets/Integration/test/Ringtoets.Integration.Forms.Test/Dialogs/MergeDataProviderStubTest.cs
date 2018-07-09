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
using Ringtoets.Integration.Forms.Merge;

namespace Ringtoets.Integration.Forms.Test.Dialogs
{
    [TestFixture]
    public class MergeDataProviderStubTest : NUnitFormTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var dialogParent = mocks.StrictMock<IWin32Window>();
            mocks.ReplayAll();

            // Call
            using (var provider = new MergeDataProviderStub(dialogParent))
            {
                // Assert
                Assert.IsInstanceOf<DialogBase>(provider);
                Assert.IsInstanceOf<IMergeDataProvider>(provider);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void SelectData_Always_ShowsDialog()
        {
            // Setup
            Button cancelButton = null;

            DialogBoxHandler = (name, wnd) =>
            {
                using (new FormTester(name))
                {
                    var button = new ButtonTester("cancelButton", name);
                    cancelButton = (Button) button.TheObject;
                    button.Click();
                }
            };

            using (var dialogParent = new Form())
            using (var provider = new MergeDataProviderStub(dialogParent))
            {
                // Call
                provider.SelectData(null);

                // Assert
                var okButton = (Button)new ButtonTester("okButton", provider).TheObject;
                Assert.AreEqual("Ok", okButton.Text);
                Assert.IsTrue(okButton.Enabled);

                Assert.AreEqual("Annuleren", cancelButton.Text);
                Assert.AreEqual(cancelButton, provider.CancelButton);

                Assert.AreEqual(1, provider.MinimumSize.Width);
                Assert.AreEqual(1, provider.MinimumSize.Height);

                Assert.AreEqual(1, provider.MinimumSize.Width);
                Assert.AreEqual(1, provider.MinimumSize.Height);
            }
        }

        [Test]
        public void GivenDialog_WhenCancelPressed_ThenReturnFalse()
        {
            // Given
            DialogBoxHandler = (name, wnd) =>
            {
                using (new FormTester(name))
                {
                    var button = new ButtonTester("cancelButton", name);
                    button.Click();
                }
            };

            using (var dialogParent = new Form())
            using (var provider = new MergeDataProviderStub(dialogParent))
            {
                // When
                bool dataSelected= provider.SelectData(null);

                // Then
                Assert.IsFalse(dataSelected);
            }
        }

        [Test]
        public void GivenDialog_WhenOkPressed_ThenReturnTrue()
        {
            // Given
            DialogBoxHandler = (name, wnd) =>
            {
                using (new FormTester(name))
                {
                    var button = new ButtonTester("okButton", name);
                    button.Click();
                }
            };

            using (var dialogParent = new Form())
            using (var provider = new MergeDataProviderStub(dialogParent))
            {
                // When
                bool dataSelected = provider.SelectData(null);

                // Then
                Assert.IsTrue(dataSelected);
            }
        }

        [Test]
        public void GivenDialog_WhenOkPressed_ThenSelectionPropertiesSet()
        {
            // Given
            DialogBoxHandler = (name, wnd) =>
            {
                using (new FormTester(name))
                {
                    var button = new ButtonTester("okButton", name);
                    button.Click();
                }
            };

            using (var dialogParent = new Form())
            using (var provider = new MergeDataProviderStub(dialogParent))
            {
                // Precondition
                Assert.IsNull(provider.SelectedAssessmentSection);
                Assert.IsNull(provider.SelectedAssessmentSection);

                // When
                provider.SelectData(null);

                // Then
                Assert.IsNotNull(provider.SelectedAssessmentSection);
                CollectionAssert.AreEqual(new[]
                {
                    provider.SelectedAssessmentSection.Piping
                }, provider.SelectedFailureMechanisms);
            }
        }
    }
}