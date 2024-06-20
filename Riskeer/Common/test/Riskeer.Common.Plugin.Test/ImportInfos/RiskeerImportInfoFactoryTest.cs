// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using Core.Common.Util;
using Core.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.Properties;
using Riskeer.Common.IO.FileImporters;
using Riskeer.Common.Plugin.ImportInfos;

namespace Riskeer.Common.Plugin.Test.ImportInfos
{
    [TestFixture]
    public class RiskeerImportInfoFactoryTest
    {
        [Test]
        public void CreateCalculationConfigurationImportInfo_CreateFileImporterFuncNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => RiskeerImportInfoFactory.CreateCalculationConfigurationImportInfo<ICalculationContext<CalculationGroup, ICalculatableFailureMechanism>>(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("createFileImporterFunc", exception.ParamName);
        }

        [Test]
        public void CreateCalculationConfigurationImportInfo_WithArguments_ExpectedPropertiesSet()
        {
            // Setup
            var mocks = new MockRepository();
            var fileImporter = mocks.Stub<IFileImporter>();
            mocks.ReplayAll();

            Func<ICalculationContext<CalculationGroup, ICalculatableFailureMechanism>, string, IFileImporter> createFileImporter = (context, s) => fileImporter;

            // Call
            ImportInfo<ICalculationContext<CalculationGroup, ICalculatableFailureMechanism>> importInfo = RiskeerImportInfoFactory.CreateCalculationConfigurationImportInfo(createFileImporter);

            // Assert
            Assert.AreSame(createFileImporter, importInfo.CreateFileImporter);
            Assert.AreEqual("Riskeer berekeningenconfiguratie", importInfo.Name);
            Assert.AreEqual("Algemeen", importInfo.Category);

            FileFilterGenerator fileFilterGenerator = importInfo.FileFilterGenerator;
            Assert.AreEqual("Riskeer berekeningenconfiguratie (*.xml)|*.xml", fileFilterGenerator.Filter);

            TestHelper.AssertImagesAreEqual(Resources.GeneralFolderIcon, importInfo.Image);
            Assert.IsTrue(importInfo.IsEnabled(null));

            mocks.VerifyAll();
        }

        [Test]
        public void CreateFailureMechanismSectionsImportInfo_CreateSectionReplaceStrategyFuncNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => RiskeerImportInfoFactory.CreateFailureMechanismSectionsImportInfo<FailureMechanismSectionsContext>(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("createSectionReplaceStrategyFunc", exception.ParamName);
        }

        [Test]
        public void CreateFailureMechanismSectionsImportInfo_WithArgument_ExpectedPropertiesSet()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism<TestFailureMechanismSectionResult>>();
            mocks.ReplayAll();

            var replaceStrategy = new FailureMechanismSectionReplaceStrategy(failureMechanism);

            // Call
            ImportInfo<FailureMechanismSectionsContext> importInfo =
                RiskeerImportInfoFactory.CreateFailureMechanismSectionsImportInfo<FailureMechanismSectionsContext>(c => replaceStrategy);

            // Assert
            Assert.AreEqual("Vakindeling", importInfo.Name);
            Assert.AreEqual("Algemeen", importInfo.Category);

            FileFilterGenerator fileFilterGenerator = importInfo.FileFilterGenerator;
            Assert.AreEqual("Shapebestand (*.shp)|*.shp", fileFilterGenerator.Filter);

            TestHelper.AssertImagesAreEqual(Resources.SectionsIcon, importInfo.Image);
            Assert.IsNull(importInfo.VerifyUpdates);
        }

        [Test]
        public void GivenCreatedImportInfo_WhenIsEnabledCalledWithReferenceLineWithGeometry_ThenReturnsFalse()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(new ReferenceLine());
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism();
            failureMechanism.SetSections(Enumerable.Empty<FailureMechanismSection>(), "path/to/sections");

            var replaceStrategy = new FailureMechanismSectionReplaceStrategy(failureMechanism);

            ImportInfo<FailureMechanismSectionsContext> importInfo =
                RiskeerImportInfoFactory.CreateFailureMechanismSectionsImportInfo<FailureMechanismSectionsContext>(c => replaceStrategy);

            // When
            var context = new FailureMechanismSectionsContext(failureMechanism, assessmentSection);
            bool isEnabled = importInfo.IsEnabled(context);

            // Then
            Assert.IsFalse(isEnabled);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenCreatedImportInfo_WhenIsEnabledCalledWithReferenceLineWithGeometry_ThenReturnsTrue()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(ReferenceLineTestFactory.CreateReferenceLineWithGeometry());
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism();
            failureMechanism.SetSections(Enumerable.Empty<FailureMechanismSection>(), "path/to/sections");

            var replaceStrategy = new FailureMechanismSectionReplaceStrategy(failureMechanism);

            ImportInfo<FailureMechanismSectionsContext> importInfo =
                RiskeerImportInfoFactory.CreateFailureMechanismSectionsImportInfo<FailureMechanismSectionsContext>(c => replaceStrategy);

            // When
            var context = new FailureMechanismSectionsContext(failureMechanism, assessmentSection);
            bool isEnabled = importInfo.IsEnabled(context);

            // Then
            Assert.IsTrue(isEnabled);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenCreatedImportInfo_WhenCreatingFileImporter_ThenReturnFileImporter()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(ReferenceLineTestFactory.CreateReferenceLineWithGeometry());
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism();
            failureMechanism.SetSections(Enumerable.Empty<FailureMechanismSection>(), "path/to/sections");

            var replaceStrategy = new FailureMechanismSectionReplaceStrategy(failureMechanism);

            ImportInfo<FailureMechanismSectionsContext> importInfo =
                RiskeerImportInfoFactory.CreateFailureMechanismSectionsImportInfo<FailureMechanismSectionsContext>(c => replaceStrategy);

            // When
            var context = new FailureMechanismSectionsContext(failureMechanism, assessmentSection);
            IFileImporter importer = importInfo.CreateFileImporter(context, "");

            // Then
            Assert.IsInstanceOf<FailureMechanismSectionsImporter>(importer);
            mocks.VerifyAll();
        }
    }
}