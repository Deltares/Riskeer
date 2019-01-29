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

using Riskeer.Piping.KernelWrapper.SubCalculator;

namespace Riskeer.Piping.KernelWrapper.TestUtil.SubCalculator
{
    /// <summary>
    /// This class allows for retrieving the created sub calculators, so that
    /// tests can be performed upon them.
    /// </summary>
    public class TestPipingSubCalculatorFactory : IPipingSubCalculatorFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestPipingSubCalculatorFactory"/>.
        /// </summary>
        public TestPipingSubCalculatorFactory()
        {
            LastCreatedUpliftCalculator = new UpliftCalculatorStub();
            LastCreatedHeaveCalculator = new HeaveCalculatorStub();
            LastCreatedSellmeijerCalculator = new SellmeijerCalculatorStub();
            LastCreatedEffectiveThicknessCalculator = new EffectiveThicknessCalculatorStub();
            LastCreatedPiezometricHeadAtExitCalculator = new PiezoHeadCalculatorStub();
            LastCreatedPipingProfilePropertyCalculator = new PipingProfilePropertyCalculatorStub();
        }

        /// <summary>
        /// Gets the last created <see cref="EffectiveThicknessCalculatorStub"/>.
        /// </summary>
        public EffectiveThicknessCalculatorStub LastCreatedEffectiveThicknessCalculator { get; }

        /// <summary>
        /// Gets the last created <see cref="UpliftCalculatorStub"/>.
        /// </summary>
        public UpliftCalculatorStub LastCreatedUpliftCalculator { get; }

        /// <summary>
        /// Gets the last created <see cref="SellmeijerCalculatorStub"/>.
        /// </summary>
        public SellmeijerCalculatorStub LastCreatedSellmeijerCalculator { get; }

        /// <summary>
        /// Gets the last created <see cref="HeaveCalculatorStub"/>.
        /// </summary>
        public HeaveCalculatorStub LastCreatedHeaveCalculator { get; }

        /// <summary>
        /// Gets the last created <see cref="PiezoHeadCalculatorStub"/>.
        /// </summary>
        public PiezoHeadCalculatorStub LastCreatedPiezometricHeadAtExitCalculator { get; }

        /// <summary>
        /// Gets the last created <see cref="PipingProfilePropertyCalculatorStub"/>.
        /// </summary>
        public PipingProfilePropertyCalculatorStub LastCreatedPipingProfilePropertyCalculator { get; }

        public IUpliftCalculator CreateUpliftCalculator()
        {
            return LastCreatedUpliftCalculator;
        }

        public IHeaveCalculator CreateHeaveCalculator()
        {
            return LastCreatedHeaveCalculator;
        }

        public ISellmeijerCalculator CreateSellmeijerCalculator()
        {
            return LastCreatedSellmeijerCalculator;
        }

        public IEffectiveThicknessCalculator CreateEffectiveThicknessCalculator()
        {
            return LastCreatedEffectiveThicknessCalculator;
        }

        public IPiezoHeadCalculator CreatePiezometricHeadAtExitCalculator()
        {
            return LastCreatedPiezometricHeadAtExitCalculator;
        }

        public IPipingProfilePropertyCalculator CreatePipingProfilePropertyCalculator()
        {
            return LastCreatedPipingProfilePropertyCalculator;
        }
    }
}