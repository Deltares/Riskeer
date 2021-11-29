// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.IO.FileImporters;

namespace Riskeer.Common.IO.Test.FileImporters
{
    [TestFixture]
    public class FailureMechanismSectionReplaceStrategyTest
    {
        [Test]
        public void Constructor_FailurePathNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new FailureMechanismSectionReplaceStrategy(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failurePath", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var failurePath = mocks.Stub<IFailurePath>();
            mocks.ReplayAll();

            // Call
            var importer = new FailureMechanismSectionReplaceStrategy(failurePath);

            // Assert
            Assert.IsInstanceOf<IFailureMechanismSectionUpdateStrategy>(importer);
            mocks.VerifyAll();
        }

        [Test]
        public void UpdateSectionsWithImportedData_ImportedFailureMechanismSectionsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failurePath = mocks.Stub<IFailurePath>();
            mocks.ReplayAll();

            // Call
            void Call() => new FailureMechanismSectionReplaceStrategy(failurePath).UpdateSectionsWithImportedData(null, "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("importedFailureMechanismSections", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void UpdateSectionsWithImportedData_SourcePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failurePath = mocks.Stub<IFailurePath>();
            mocks.ReplayAll();

            var failureMechanismSectionReplaceStrategy = new FailureMechanismSectionReplaceStrategy(failurePath);

            // Call
            void Call() => failureMechanismSectionReplaceStrategy.UpdateSectionsWithImportedData(Enumerable.Empty<FailureMechanismSection>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sourcePath", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenReplaceStrategyWithFailurePath_WhenUpdateSectionsWithImportedDataWithValidData_ThenSetsSectionsToFailureMechanismAndReturnsAffectedObjects()
        {
            // Given
            var failurePath = new TestFailurePath();
            var failureMechanismSectionReplaceStrategy = new FailureMechanismSectionReplaceStrategy(failurePath);
            string sourcePath = TestHelper.GetScratchPadPath();
            FailureMechanismSection[] sections =
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            };

            // When
            IEnumerable<IObservable> affectedObjects = failureMechanismSectionReplaceStrategy.UpdateSectionsWithImportedData(sections, sourcePath);

            // Then
            Assert.AreEqual(sourcePath, failurePath.FailureMechanismSectionSourcePath);
            Assert.AreEqual(sections.Single(), failurePath.Sections.Single());
            CollectionAssert.AreEqual(new[]
            {
                failurePath
            }, affectedObjects);
        }
        
        [Test]
        public void GivenReplaceStrategyWithFailureMechanismWithSectionResults_WhenUpdateSectionsWithImportedDataWithValidData_ThenSetsSectionsToFailureMechanismAndReturnsAffectedObjects()
        {
            // Given
            var failureMechanism = new TestFailureMechanism();
            var failureMechanismSectionReplaceStrategy = new FailureMechanismSectionReplaceStrategy(failureMechanism);
            string sourcePath = TestHelper.GetScratchPadPath();
            FailureMechanismSection[] sections =
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            };

            // When
            IEnumerable<IObservable> affectedObjects = failureMechanismSectionReplaceStrategy.UpdateSectionsWithImportedData(sections, sourcePath);

            // Then
            Assert.AreEqual(sourcePath, failureMechanism.FailureMechanismSectionSourcePath);
            Assert.AreEqual(sections.Single(), failureMechanism.Sections.Single());
            CollectionAssert.AreEqual(new IObservable[]
            {
                failureMechanism,
                failureMechanism.SectionResultsOld
            }, affectedObjects);
        }

        [Test]
        public void UpdateSectionsWithImportedData_WithInvalidSections_ThrowsUpdateDataException()
        {
            // Setup
            string sourcePath = TestHelper.GetScratchPadPath();

            var failurePath = new TestFailurePath();
            FailureMechanismSection failureMechanismSection1 = FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 5.0)
            });
            FailureMechanismSection failureMechanismSection2 = FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
            {
                new Point2D(10.0, 10.0),
                new Point2D(15.0, 15.0)
            });

            var failureMechanismSectionReplaceStrategy = new FailureMechanismSectionReplaceStrategy(failurePath);

            FailureMechanismSection[] sections =
            {
                failureMechanismSection1,
                failureMechanismSection2
            };

            // Call
            void Call() => failureMechanismSectionReplaceStrategy.UpdateSectionsWithImportedData(sections, sourcePath);

            // Assert
            var exception = Assert.Throws<UpdateDataException>(Call);
            Assert.IsInstanceOf<ArgumentException>(exception.InnerException);
            Assert.AreEqual(exception.InnerException.Message, exception.Message);
        }

        [Test]
        public void UpdateSectionsWithImportedData_WithEmptyData_ClearsSectionsAndUpdatesPathAndReturnsAffectedObjects()
        {
            // Setup
            const string oldSourcePath = "old/path";
            string sourcePath = TestHelper.GetScratchPadPath();

            var failurePath = new TestFailurePath();
            failurePath.SetSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            }, oldSourcePath);

            var failureMechanismSectionReplaceStrategy = new FailureMechanismSectionReplaceStrategy(failurePath);

            // Precondition
            IEnumerable<FailureMechanismSection> failureMechanismSections = failurePath.Sections;
            Assert.AreEqual(1, failureMechanismSections.Count());
            Assert.AreEqual(oldSourcePath, failurePath.FailureMechanismSectionSourcePath);

            // Call
            IEnumerable<IObservable> affectedObjects = failureMechanismSectionReplaceStrategy.UpdateSectionsWithImportedData(Enumerable.Empty<FailureMechanismSection>(), sourcePath);

            // Assert
            Assert.AreEqual(sourcePath, failurePath.FailureMechanismSectionSourcePath);
            Assert.IsEmpty(failureMechanismSections);
            CollectionAssert.AreEqual(new[]
            {
                failurePath
            }, affectedObjects);
        }


        [Test]
        public void DoPostUpdateActions_Always_ReturnsEmptyCollection()
        {
            // Setup
            var failurePath = new TestFailurePath();
            var failureMechanismSectionReplaceStrategy = new FailureMechanismSectionReplaceStrategy(failurePath);
            
            // Call
            IEnumerable<IObservable> affectedObjects = failureMechanismSectionReplaceStrategy.DoPostUpdateActions();
            
            // Assert
            CollectionAssert.IsEmpty(affectedObjects);
        }
    }
}