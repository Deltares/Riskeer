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
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.IO.FileImporters;

namespace Riskeer.Common.IO.Test.FileImporters
{
    [TestFixture]
    public class FailureMechanismSectionUpdateStrategyTest
    {
        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var sectionResultUpdateStrategy = mocks.Stub<IFailureMechanismSectionResultUpdateStrategy<TestFailureMechanismSectionResult>>();
            mocks.ReplayAll();

            // Call
            void Call() => new FailureMechanismSectionUpdateStrategy<TestFailureMechanismSectionResult>(null, sectionResultUpdateStrategy);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_SectionResultUpdateStrategyNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IHasSectionResults<TestFailureMechanismSectionResult>>();
            mocks.ReplayAll();

            // Call
            void Call() => new FailureMechanismSectionUpdateStrategy<TestFailureMechanismSectionResult>(failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResultUpdateStrategy", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IHasSectionResults<TestFailureMechanismSectionResult>>();
            var sectionResultUpdateStrategy = mocks.Stub<IFailureMechanismSectionResultUpdateStrategy<TestFailureMechanismSectionResult>>();
            mocks.ReplayAll();

            // Call
            var importer = new FailureMechanismSectionUpdateStrategy<TestFailureMechanismSectionResult>(failureMechanism, sectionResultUpdateStrategy);

            // Assert
            Assert.IsInstanceOf<IFailureMechanismSectionUpdateStrategy>(importer);
            mocks.VerifyAll();
        }

        [Test]
        public void UpdateSectionsWithImportedData_ImportedFailureMechanismSectionsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IHasSectionResults<TestFailureMechanismSectionResult>>();
            var sectionResultUpdateStrategy = mocks.Stub<IFailureMechanismSectionResultUpdateStrategy<TestFailureMechanismSectionResult>>();
            mocks.ReplayAll();

            // Call
            void Call() => new FailureMechanismSectionUpdateStrategy<TestFailureMechanismSectionResult>(failureMechanism, sectionResultUpdateStrategy).UpdateSectionsWithImportedData(null, "");

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
            var failureMechanism = mocks.Stub<IHasSectionResults<TestFailureMechanismSectionResult>>();
            var sectionResultUpdateStrategy = mocks.Stub<IFailureMechanismSectionResultUpdateStrategy<TestFailureMechanismSectionResult>>();
            mocks.ReplayAll();

            var failureMechanismSectionUpdateStrategy = new FailureMechanismSectionUpdateStrategy<TestFailureMechanismSectionResult>(failureMechanism, sectionResultUpdateStrategy);

            // Call
            void Call() => failureMechanismSectionUpdateStrategy.UpdateSectionsWithImportedData(Enumerable.Empty<FailureMechanismSection>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sourcePath", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void UpdateSectionsWithImportedData_WithValidData_SetsSectionsToFailureMechanismAndCopiesPropertiesOfEqualSectionsAndReturnsAffectedObjects()
        {
            // Setup
            string sourcePath = TestHelper.GetScratchPadPath();

            var failureMechanism = new TestFailureMechanism();
            FailureMechanismSection failureMechanismSection1 = FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 5.0)
            });
            FailureMechanismSection failureMechanismSection2 = FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
            {
                new Point2D(5.0, 5.0),
                new Point2D(10.0, 10.0)
            });
            failureMechanism.SetSections(new[]
            {
                failureMechanismSection1,
                failureMechanismSection2
            }, sourcePath);

            IObservableEnumerable<FailureMechanismSectionResult> failureMechanismSectionResults = failureMechanism.SectionResults;
            FailureMechanismSectionResult oldSectionResult = failureMechanismSectionResults.First();

            var sectionResultUpdateStrategy = new TestUpdateFailureMechanismSectionResultUpdateStrategy();
            var failureMechanismSectionUpdateStrategy = new FailureMechanismSectionUpdateStrategy<FailureMechanismSectionResult>(failureMechanism, sectionResultUpdateStrategy);

            FailureMechanismSection[] sections =
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(5.0, 5.0)
                }),
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(5.0, 5.0),
                    new Point2D(15.0, 15.0)
                })
            };

            // Call
            IEnumerable<IObservable> affectedObjects = failureMechanismSectionUpdateStrategy.UpdateSectionsWithImportedData(sections, sourcePath);

            // Assert
            Assert.AreEqual(sourcePath, failureMechanism.FailureMechanismSectionSourcePath);

            IEnumerable<FailureMechanismSection> failureMechanismSections = failureMechanism.Sections;
            Assert.AreEqual(2, failureMechanismSections.Count());
            CollectionAssert.AreEqual(sections, failureMechanismSections);
            Assert.AreSame(oldSectionResult, sectionResultUpdateStrategy.Origin);
            Assert.AreSame(failureMechanismSectionResults.First(), sectionResultUpdateStrategy.Target);
            CollectionAssert.AreEqual(new IObservable[]
            {
                failureMechanism,
                failureMechanism.SectionResults
            }, affectedObjects);
        }

        [Test]
        public void UpdateSectionsWithImportedData_WithInvalidSections_ThrowsUpdateDataException()
        {
            // Setup
            string sourcePath = TestHelper.GetScratchPadPath();

            var failureMechanism = new TestFailureMechanism();
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

            var sectionResultUpdateStrategy = new TestUpdateFailureMechanismSectionResultUpdateStrategy();
            var failureMechanismSectionUpdateStrategy = new FailureMechanismSectionUpdateStrategy<FailureMechanismSectionResult>(failureMechanism, sectionResultUpdateStrategy);

            FailureMechanismSection[] sections =
            {
                failureMechanismSection1,
                failureMechanismSection2
            };

            // Call
            void Call() => failureMechanismSectionUpdateStrategy.UpdateSectionsWithImportedData(sections, sourcePath);

            // Assert
            var exception = Assert.Throws<UpdateDataException>(Call);
            Assert.IsInstanceOf<ArgumentException>(exception.InnerException);
            Assert.AreEqual(exception.InnerException.Message, exception.Message);
            Assert.IsFalse(sectionResultUpdateStrategy.Updated);
        }

        [Test]
        public void UpdateSectionsWithImportedData_WithEmptyData_ClearsSectionsAndUpdatesPathAndReturnsAffectedObjects()
        {
            // Setup
            const string oldSourcePath = "old/path";
            string sourcePath = TestHelper.GetScratchPadPath();

            var failureMechanism = new TestFailureMechanism();
            failureMechanism.SetSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            }, oldSourcePath);

            var sectionResultUpdateStrategy = new TestUpdateFailureMechanismSectionResultUpdateStrategy();
            var failureMechanismSectionUpdateStrategy = new FailureMechanismSectionUpdateStrategy<FailureMechanismSectionResult>(
                failureMechanism, sectionResultUpdateStrategy);

            // Precondition
            IEnumerable<FailureMechanismSection> failureMechanismSections = failureMechanism.Sections;
            Assert.AreEqual(1, failureMechanismSections.Count());
            Assert.AreEqual(oldSourcePath, failureMechanism.FailureMechanismSectionSourcePath);

            // Call
            IEnumerable<IObservable> affectedObjects = failureMechanismSectionUpdateStrategy.UpdateSectionsWithImportedData(
                Enumerable.Empty<FailureMechanismSection>(), sourcePath);

            // Assert
            Assert.AreEqual(sourcePath, failureMechanism.FailureMechanismSectionSourcePath);
            Assert.IsEmpty(failureMechanismSections);
            Assert.IsFalse(sectionResultUpdateStrategy.Updated);
            CollectionAssert.AreEqual(new IObservable[]
            {
                failureMechanism,
                failureMechanism.SectionResults
            }, affectedObjects);
        }

        [Test]
        public void DoPostUpdateActions_Always_ReturnsEmptyCollection()
        {
            // Setup
            var failureMechanism = new TestFailurePath();
            var failureMechanismSectionReplaceStrategy = new FailureMechanismSectionReplaceStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = failureMechanismSectionReplaceStrategy.DoPostUpdateActions();

            // Assert
            CollectionAssert.IsEmpty(affectedObjects);
        }

        private class TestUpdateFailureMechanismSectionResultUpdateStrategy : IFailureMechanismSectionResultUpdateStrategy<FailureMechanismSectionResult>
        {
            public bool Updated { get; private set; }
            public FailureMechanismSectionResult Origin { get; private set; }
            public FailureMechanismSectionResult Target { get; private set; }

            public void UpdateSectionResult(FailureMechanismSectionResult origin, FailureMechanismSectionResult target)
            {
                Updated = true;
                Origin = origin;
                Target = target;
            }
        }
    }
}