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
                IsRelevant = random.NextBoolean(),
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

            Assert.AreEqual(Convert.ToByte(specificFailurePath.IsRelevant), entity.IsRelevant);

            Assert.IsNull(entity.InputComments);
            Assert.IsNull(entity.OutputComments);
            Assert.IsNull(entity.NotRelevantComments);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string name = "newName";
            const string originalInput = "Some input text";
            const string originalOutput = "Some output text";
            const string originalNotRelevantText = "Really not relevant";
            const string specificFailurePathSectionsSourcePath = "File\\Path";
            var specificFailurePath = new SpecificFailurePath
            {
                Name = name,
                InputComments =
                {
                    Body = originalInput
                },
                OutputComments =
                {
                    Body = originalOutput
                },
                NotRelevantComments =
                {
                    Body = originalNotRelevantText
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
            TestHelper.AssertAreEqualButNotSame(specificFailurePath.InputComments.Body, entity.InputComments);
            TestHelper.AssertAreEqualButNotSame(specificFailurePath.OutputComments.Body, entity.OutputComments);
            TestHelper.AssertAreEqualButNotSame(specificFailurePath.NotRelevantComments.Body, entity.NotRelevantComments);
            TestHelper.AssertAreEqualButNotSame(specificFailurePath.FailureMechanismSectionSourcePath, entity.FailureMechanismSectionCollectionSourcePath);
        }
    }
}