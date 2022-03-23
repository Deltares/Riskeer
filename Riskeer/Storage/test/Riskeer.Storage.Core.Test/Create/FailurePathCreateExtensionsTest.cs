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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.FailurePath;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create
{
    [TestFixture]
    public class FailurePathCreateExtensionsTest
    {
        # region FailureMechanism

        [Test]
        public void CreateForFailureMechanism_RegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailurePath>();
            var failureMechanismType = random.NextEnumValue<FailureMechanismType>();

            // Call
            void Call() => failureMechanism.Create(failureMechanismType, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
        }

        [Test]
        public void CreateForFailureMechanism_PropertiesSet_ReturnExpectedEntity()
        {
            // Setup
            var random = new Random(21);
            var failureMechanismType = random.NextEnumValue<FailureMechanismType>();

            var failureMechanism = new TestFailureMechanism
            {
                InAssembly = random.NextBoolean(),
                AssemblyResult =
                {
                    ProbabilityResultType = random.NextEnumValue<FailurePathAssemblyProbabilityResultType>(),
                    ManualFailurePathAssemblyProbability = random.NextDouble()
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(failureMechanismType, registry);

            // Assert
            Assert.AreEqual(Convert.ToByte(failureMechanismType), entity.FailureMechanismType);

            Assert.IsNull(entity.FailureMechanismSectionCollectionSourcePath);
            CollectionAssert.IsEmpty(entity.FailureMechanismSectionEntities);

            Assert.AreEqual(Convert.ToByte(failureMechanism.InAssembly), entity.InAssembly);

            Assert.IsNull(entity.InAssemblyInputComments);
            Assert.IsNull(entity.InAssemblyOutputComments);
            Assert.IsNull(entity.NotInAssemblyComments);

            FailurePathAssemblyResult assemblyResult = failureMechanism.AssemblyResult;
            Assert.AreEqual(Convert.ToByte(assemblyResult.ProbabilityResultType), entity.FailurePathAssemblyProbabilityResultType);
            Assert.AreEqual(assemblyResult.ManualFailurePathAssemblyProbability, entity.ManualFailurePathAssemblyProbability);
        }

        [Test]
        public void CreateForFailureMechanism_WithNaNValues_ReturnExpectedEntity()
        {
            // Setup
            var random = new Random(21);
            var failureMechanismType = random.NextEnumValue<FailureMechanismType>();

            var failureMechanism = new TestFailureMechanism();
            var registry = new PersistenceRegistry();

            // Precondition
            FailurePathAssemblyResult assemblyResult = failureMechanism.AssemblyResult;
            Assert.IsNaN(assemblyResult.ManualFailurePathAssemblyProbability);

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(failureMechanismType, registry);

            // Assert
            Assert.IsNull(entity.ManualFailurePathAssemblyProbability);
        }

        [Test]
        public void CreateForFailureMechanism_WithSections_ReturnsExpectedEntity()
        {
            // Setup
            var random = new Random(21);
            var failureMechanismType = random.NextEnumValue<FailureMechanismType>();

            const string specificFailurePathSectionsSourcePath = "File\\Path";
            var failureMechanism = new TestFailureMechanism();

            failureMechanism.SetSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0)
                }),
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 0)
                })
            }, specificFailurePathSectionsSourcePath);

            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(failureMechanismType, registry);

            // Assert
            Assert.AreEqual(failureMechanism.Sections.Count(), entity.FailureMechanismSectionEntities.Count);
        }

        [Test]
        public void CreateForFailureMechanism_StringPropertiesDoNotShareReference()
        {
            // Setup
            var random = new Random(21);
            var failureMechanismType = random.NextEnumValue<FailureMechanismType>();
            IFailurePath failureMechanism = new TestFailureMechanism("a", "cool")
            {
                InAssemblyInputComments =
                {
                    Body = "Some input text"
                },
                InAssemblyOutputComments =
                {
                    Body = "Some output text"
                },
                NotInAssemblyComments =
                {
                    Body = "Really not in assembly"
                },
                CalculationsInputComments =
                {
                    Body = "Some calculation text"
                }
            };
            failureMechanism.SetSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            }, "File\\Path");

            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(failureMechanismType, registry);

            // Assert
            TestHelper.AssertAreEqualButNotSame(failureMechanism.InAssemblyInputComments.Body, entity.InAssemblyInputComments);
            TestHelper.AssertAreEqualButNotSame(failureMechanism.InAssemblyOutputComments.Body, entity.InAssemblyOutputComments);
            TestHelper.AssertAreEqualButNotSame(failureMechanism.NotInAssemblyComments.Body, entity.NotInAssemblyComments);
            Assert.IsNull(entity.CalculationsInputComments);
            TestHelper.AssertAreEqualButNotSame(failureMechanism.FailureMechanismSectionSourcePath, entity.FailureMechanismSectionCollectionSourcePath);
        }

        #endregion

        # region CalculatableFailureMechanism

        [Test]
        public void CreateForCalculatableFailureMechanism_RegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<ICalculatableFailureMechanism>();
            var failureMechanismType = random.NextEnumValue<FailureMechanismType>();

            // Call
            void Call() => failureMechanism.Create(failureMechanismType, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
        }

        [Test]
        public void CreateForCalculatableFailureMechanism_PropertiesSet_ReturnExpectedEntity()
        {
            // Setup
            var random = new Random(21);
            var failureMechanismType = random.NextEnumValue<FailureMechanismType>();

            var failureMechanism = new TestFailureMechanism
            {
                InAssembly = random.NextBoolean(),
                AssemblyResult =
                {
                    ProbabilityResultType = random.NextEnumValue<FailurePathAssemblyProbabilityResultType>(),
                    ManualFailurePathAssemblyProbability = random.NextDouble()
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(failureMechanismType, registry);

            // Assert
            Assert.AreEqual(Convert.ToByte(failureMechanismType), entity.FailureMechanismType);

            Assert.IsNull(entity.FailureMechanismSectionCollectionSourcePath);
            CollectionAssert.IsEmpty(entity.FailureMechanismSectionEntities);

            Assert.AreEqual(Convert.ToByte(failureMechanism.InAssembly), entity.InAssembly);

            Assert.IsNull(entity.InAssemblyInputComments);
            Assert.IsNull(entity.InAssemblyOutputComments);
            Assert.IsNull(entity.NotInAssemblyComments);

            FailurePathAssemblyResult assemblyResult = failureMechanism.AssemblyResult;
            Assert.AreEqual(Convert.ToByte(assemblyResult.ProbabilityResultType), entity.FailurePathAssemblyProbabilityResultType);
            Assert.AreEqual(assemblyResult.ManualFailurePathAssemblyProbability, entity.ManualFailurePathAssemblyProbability);
        }

        [Test]
        public void CreateForCalculatableFailureMechanism_WithNaNValues_ReturnExpectedEntity()
        {
            // Setup
            var random = new Random(21);
            var failureMechanismType = random.NextEnumValue<FailureMechanismType>();

            var failureMechanism = new TestFailureMechanism();
            var registry = new PersistenceRegistry();

            // Precondition
            FailurePathAssemblyResult assemblyResult = failureMechanism.AssemblyResult;
            Assert.IsNaN(assemblyResult.ManualFailurePathAssemblyProbability);

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(failureMechanismType, registry);

            // Assert
            Assert.IsNull(entity.ManualFailurePathAssemblyProbability);
        }

        [Test]
        public void CreateForCalculatableFailureMechanism_WithSections_ReturnsExpectedEntity()
        {
            // Setup
            var random = new Random(21);
            var failureMechanismType = random.NextEnumValue<FailureMechanismType>();

            const string specificFailurePathSectionsSourcePath = "File\\Path";
            var failureMechanism = new TestFailureMechanism();

            failureMechanism.SetSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0)
                }),
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 0)
                })
            }, specificFailurePathSectionsSourcePath);

            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(failureMechanismType, registry);

            // Assert
            Assert.AreEqual(failureMechanism.Sections.Count(), entity.FailureMechanismSectionEntities.Count);
        }

        [Test]
        public void CreateForCalculatableFailureMechanism_StringPropertiesDoNotShareReference()
        {
            // Setup
            var random = new Random(21);
            var failureMechanismType = random.NextEnumValue<FailureMechanismType>();
            var failureMechanism = new TestFailureMechanism("a", "cool")
            {
                InAssemblyInputComments =
                {
                    Body = "Some input text"
                },
                InAssemblyOutputComments =
                {
                    Body = "Some output text"
                },
                NotInAssemblyComments =
                {
                    Body = "Really not in assembly"
                },
                CalculationsInputComments =
                {
                    Body = "Some calculation text"
                }
            };
            failureMechanism.SetSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            }, "File\\Path");

            var registry = new PersistenceRegistry();

            // Call
            FailureMechanismEntity entity = failureMechanism.Create(failureMechanismType, registry);

            // Assert
            TestHelper.AssertAreEqualButNotSame(failureMechanism.InAssemblyInputComments.Body, entity.InAssemblyInputComments);
            TestHelper.AssertAreEqualButNotSame(failureMechanism.InAssemblyOutputComments.Body, entity.InAssemblyOutputComments);
            TestHelper.AssertAreEqualButNotSame(failureMechanism.NotInAssemblyComments.Body, entity.NotInAssemblyComments);
            TestHelper.AssertAreEqualButNotSame(failureMechanism.CalculationsInputComments.Body, entity.CalculationsInputComments);
            TestHelper.AssertAreEqualButNotSame(failureMechanism.FailureMechanismSectionSourcePath, entity.FailureMechanismSectionCollectionSourcePath);
        }

        #endregion

        #region FailurePath

        [Test]
        public void CreateForFailurePath_RegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var failurePath = new SpecificFailurePath();

            // Call
            void Call() => failurePath.Create(null, 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
        }

        [Test]
        public void CreateForFailurePath_PropertiesSet_ReturnExpectedEntity()
        {
            // Setup
            var random = new Random(21);
            int order = random.Next();

            var failurePath = new SpecificFailurePath
            {
                GeneralInput =
                {
                    N = random.NextRoundedDouble(1, 20),
                    ApplyLengthEffectInSection = random.NextBoolean()
                },
                InAssembly = random.NextBoolean(),
                AssemblyResult =
                {
                    ProbabilityResultType = random.NextEnumValue<FailurePathAssemblyProbabilityResultType>(),
                    ManualFailurePathAssemblyProbability = random.NextDouble()
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            SpecificFailurePathEntity entity = failurePath.Create(registry, order);

            // Assert
            Assert.AreEqual(order, entity.Order);
            Assert.AreEqual(failurePath.GeneralInput.N, entity.N);
            Assert.AreEqual(Convert.ToByte(failurePath.GeneralInput.ApplyLengthEffectInSection), entity.ApplyLengthEffectInSection);

            Assert.IsNull(entity.FailureMechanismSectionCollectionSourcePath);
            CollectionAssert.IsEmpty(entity.FailureMechanismSectionEntities);

            Assert.AreEqual(Convert.ToByte(failurePath.InAssembly), entity.InAssembly);

            Assert.IsNull(entity.InAssemblyInputComments);
            Assert.IsNull(entity.InAssemblyOutputComments);
            Assert.IsNull(entity.NotInAssemblyComments);

            FailurePathAssemblyResult assemblyResult = failurePath.AssemblyResult;
            Assert.AreEqual(Convert.ToByte(assemblyResult.ProbabilityResultType), entity.FailurePathAssemblyProbabilityResultType);
            Assert.AreEqual(assemblyResult.ManualFailurePathAssemblyProbability, entity.ManualFailurePathAssemblyProbability);
        }

        [Test]
        public void CreateForFailurePath_WithNaNValues_ReturnExpectedEntity()
        {
            // Setup
            var failurePath = new SpecificFailurePath();
            var registry = new PersistenceRegistry();

            // Precondition
            FailurePathAssemblyResult assemblyResult = failurePath.AssemblyResult;
            Assert.IsNaN(assemblyResult.ManualFailurePathAssemblyProbability);

            // Call
            SpecificFailurePathEntity entity = failurePath.Create(registry, 0);

            // Assert
            Assert.IsNull(entity.ManualFailurePathAssemblyProbability);
        }

        [Test]
        public void CreateForFailurePath_WithSections_ReturnsExpectedEntity()
        {
            // Setup
            const string specificFailurePathSectionsSourcePath = "File\\Path";
            var failurePath = new SpecificFailurePath();

            failurePath.SetSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0)
                }),
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 0)
                })
            }, specificFailurePathSectionsSourcePath);

            var registry = new PersistenceRegistry();

            // Call
            SpecificFailurePathEntity entity = failurePath.Create(registry, 0);

            // Assert
            int nrOfFailurePathSections = failurePath.Sections.Count();
            Assert.AreEqual(nrOfFailurePathSections, entity.FailureMechanismSectionEntities.Count);
            Assert.AreEqual(nrOfFailurePathSections, entity.FailureMechanismSectionEntities.SelectMany(fms => fms.NonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntities).Count());
            TestHelper.AssertAreEqualButNotSame(specificFailurePathSectionsSourcePath, entity.FailureMechanismSectionCollectionSourcePath);
        }

        [Test]
        public void CreateForFailurePath_StringPropertiesDoNotShareReference()
        {
            // Setup
            var failurePath = new SpecificFailurePath
            {
                Name = "Just a Name",
                Code = "FAALMECHANISME",
                InAssemblyInputComments =
                {
                    Body = "Some input text"
                },
                InAssemblyOutputComments =
                {
                    Body = "Some output text"
                },
                NotInAssemblyComments =
                {
                    Body = "Really not in assembly"
                }
            };
            failurePath.SetSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            }, "File\\Path");

            var registry = new PersistenceRegistry();

            // Call
            SpecificFailurePathEntity entity = failurePath.Create(registry, 0);

            // Assert
            TestHelper.AssertAreEqualButNotSame(failurePath.Name, entity.Name);
            TestHelper.AssertAreEqualButNotSame(failurePath.Code, entity.Code);
            TestHelper.AssertAreEqualButNotSame(failurePath.InAssemblyInputComments.Body, entity.InAssemblyInputComments);
            TestHelper.AssertAreEqualButNotSame(failurePath.InAssemblyOutputComments.Body, entity.InAssemblyOutputComments);
            TestHelper.AssertAreEqualButNotSame(failurePath.NotInAssemblyComments.Body, entity.NotInAssemblyComments);
            TestHelper.AssertAreEqualButNotSame(failurePath.FailureMechanismSectionSourcePath, entity.FailureMechanismSectionCollectionSourcePath);
        }

        #endregion
    }
}