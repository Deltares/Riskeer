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
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.Data.FailurePath;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.Create.SpecificFailurePaths;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create.SpecificFailurePaths
{
    [TestFixture]
    public class SpecificFailurePathCreateExtensionsTest
    {
        [Test]
        public void Create_RegistryIsNull_ThrowArgumentNullException()
        {
            // Setup
            var specificFailurePath = new SpecificFailurePath();

            // Call
            void Call() => specificFailurePath.Create(null, 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
        }

        [Test]
        public void Create_WithRegistryAndPropertiesSet_ReturnsExpectedEntity()
        {
            // Setup
            var random = new Random(21);
            int order = random.Next();
            var specificFailurePath = new SpecificFailurePath
            {
                InAssembly = random.NextBoolean(),
                Input =
                {
                    N = random.NextRoundedDouble(1.0, 20.0)
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            SpecificFailurePathEntity entity = specificFailurePath.Create(registry, order);

            // Assert
            SpecificFailurePathInput expectedInput = specificFailurePath.Input;
            Assert.AreEqual(expectedInput.N, entity.N, expectedInput.N.GetAccuracy());
            CollectionAssert.IsEmpty(entity.FailureMechanismSectionEntities);

            Assert.AreEqual(Convert.ToByte(specificFailurePath.InAssembly), entity.InAssembly);

            Assert.IsNull(entity.InAssemblyInputComments);
            Assert.IsNull(entity.InAssemblyOutputComments);
            Assert.IsNull(entity.NotInAssemblyComments);
        }

        [Test]
        public void Create_WithSections_ReturnsExpectedEntity()
        {
            // Setup
            var random = new Random(21);
            int order = random.Next();
            const string specificFailurePathSectionsSourcePath = "File\\Path";
            var specificFailurePath = new SpecificFailurePath
            {
                Input =
                {
                    N = random.NextRoundedDouble(1.0, 20.0)
                }
            };

            specificFailurePath.SetSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new []
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0)
                }),
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new []
                {
                    new Point2D(1, 0),
                    new Point2D(2, 0)
                })                
            }, specificFailurePathSectionsSourcePath);

            var registry = new PersistenceRegistry();

            // Call
            SpecificFailurePathEntity entity = specificFailurePath.Create(registry, order);

            // Assert
            Assert.AreEqual(specificFailurePath.Sections.Count(), entity.FailureMechanismSectionEntities.Count);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string name = "newName";
            const string originalInAssemblyInputComments = "Some input text";
            const string originalInAssemblyOutputComments = "Some output text";
            const string originalNotInAssemblyComments = "Really not in assembly";
            const string specificFailurePathSectionsSourcePath = "File\\Path";
            var specificFailurePath = new SpecificFailurePath
            {
                Name = name,
                InAssemblyInputComments =
                {
                    Body = originalInAssemblyInputComments
                },
                InAssemblyOutputComments =
                {
                    Body = originalInAssemblyOutputComments
                },
                NotInAssemblyComments =
                {
                    Body = originalNotInAssemblyComments
                }
            };
            specificFailurePath.SetSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            }, specificFailurePathSectionsSourcePath);

            var registry = new PersistenceRegistry();

            // Call
            SpecificFailurePathEntity entity = specificFailurePath.Create(registry, 0);

            // Assert
            TestHelper.AssertAreEqualButNotSame(specificFailurePath.Name, entity.Name);
            TestHelper.AssertAreEqualButNotSame(specificFailurePath.InAssemblyInputComments.Body, entity.InAssemblyInputComments);
            TestHelper.AssertAreEqualButNotSame(specificFailurePath.InAssemblyOutputComments.Body, entity.InAssemblyOutputComments);
            TestHelper.AssertAreEqualButNotSame(specificFailurePath.NotInAssemblyComments.Body, entity.NotInAssemblyComments);
            TestHelper.AssertAreEqualButNotSame(specificFailurePath.FailureMechanismSectionSourcePath, entity.FailureMechanismSectionCollectionSourcePath);
        }
    }
}