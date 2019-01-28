// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System;
using Core.Common.Base.Data;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class DesignVariablePropertiesTest
    {
        [Test]
        public void Constructor_DesignVariableNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new SimpleDesignVariableProperties(DistributionPropertiesReadOnly.None,
                                                                         null,
                                                                         handler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("designVariable", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var distribution = new LogNormalDistribution();
            var designVariable = new LogNormalDistributionDesignVariable(distribution);

            // Call
            var properties = new SimpleDesignVariableProperties(DistributionPropertiesReadOnly.All,
                                                                designVariable,
                                                                handler);

            // Assert
            Assert.IsInstanceOf<DistributionPropertiesBase<LogNormalDistribution>>(properties);
            Assert.AreEqual(designVariable.GetDesignValue(), properties.DesignValue);
            mockRepository.VerifyAll();
        }

        [Test]
        [SetCulture("nl-NL")]
        public void ToString_Always_ReturnDistributionName()
        {
            // Setup
            var mockRepository = new MockRepository();
            var handler = mockRepository.Stub<IObservablePropertyChangeHandler>();
            mockRepository.ReplayAll();

            var distribution = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) 1,
                StandardDeviation = (RoundedDouble) 2
            };
            var designVariable = new LogNormalDistributionDesignVariable(distribution);

            // Call
            var properties = new SimpleDesignVariableProperties(DistributionPropertiesReadOnly.None,
                                                                designVariable,
                                                                handler);

            // Call
            string propertyName = properties.ToString();

            // Assert
            Assert.AreEqual("0,45 (Verwachtingswaarde = 1,00, Standaardafwijking = 2,00)", propertyName);
        }

        private class SimpleDesignVariableProperties : DesignVariableProperties<LogNormalDistribution>
        {
            public SimpleDesignVariableProperties(DistributionPropertiesReadOnly propertiesReadOnly,
                                                  DesignVariable<LogNormalDistribution> designVariable,
                                                  IObservablePropertyChangeHandler handler)
                : base(propertiesReadOnly, designVariable, handler) {}

            public override string DistributionType { get; }
        }
    }
}