// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Application.Ringtoets.Storage.Converters;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;
using Ringtoets.Integration.Data.Placeholders;

namespace Application.Ringtoets.Storage.Test.Converters
{
    [TestFixture]
    public class FailureMechanismPlaceholderConverterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var converter = new FailureMechanismPlaceholderConverter(FailureMechanismType.StrengthAndStabilityParallelConstruction);

            // Assert
            Assert.IsInstanceOf<FailureMechanismConverterBase<FailureMechanismPlaceholder>>(converter);
        }

        [Test]
        [TestCase(984753874, true)]
        [TestCase(123, false)]
        public void ConvertEntityToModel_ProperEntity_ReturnNewlyCreatedFailureMechanism(
            long id, bool isRelevant)
        {
            // Setup
            var type = FailureMechanismType.StructureHeight;

            var entity = new FailureMechanismEntity
            {
                FailureMechanismType = (short) type,
                FailureMechanismEntityId = id,
                IsRelevant = isRelevant ? (byte) 1 : (byte) 0
            };

            var converter = new FailureMechanismPlaceholderConverter(type);

            // Call
            FailureMechanismPlaceholder failureMechanism = converter.ConvertEntityToModel(entity);

            // Assert
            Assert.AreEqual(id, failureMechanism.StorageId);
            Assert.AreEqual(isRelevant, failureMechanism.IsRelevant);
        }

        [Test]
        [TestCase(98546309845, FailureMechanismType.GrassRevetmentSlidingOutwards, true)]
        [TestCase(1, FailureMechanismType.StabilityStoneRevetment, false)]
        public void ConvertModelToEntity_InitializedFailureMechanism_ChangeValuesForEntity(
            long id, FailureMechanismType type, bool isRelevant)
        {
            // Setup

            var converter = new FailureMechanismPlaceholderConverter(type);

            var failureMechanism = new FailureMechanismPlaceholder("A")
            {
                StorageId = id,
                IsRelevant = isRelevant
            };

            var entity = new FailureMechanismEntity();

            // Call
            converter.ConvertModelToEntity(failureMechanism, entity);

            // Assert
            Assert.AreEqual(id, entity.FailureMechanismEntityId);
            Assert.AreEqual((short) type, entity.FailureMechanismType);
            var expectedIsRelevantValue = isRelevant ?
                                              (byte) 1 :
                                              (byte) 0;
            Assert.AreEqual(expectedIsRelevantValue, entity.IsRelevant);
        }
    }
}