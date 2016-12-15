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

using Ringtoets.Piping.KernelWrapper.SubCalculator;

namespace Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator
{
    /// <summary>
    /// This class allows for retrieving the created sub calculators, so that
    /// tests can be performed upon them.
    /// </summary>
    public class TestPipingSubCalculatorFactory : IPipingSubCalculatorFactory
    {
        public TestPipingSubCalculatorFactory()
        {
            LastCreatedUpliftCalculator = new UpliftCalculatorStub();
            LastCreatedHeaveCalculator = new HeaveCalculatorStub();
            LastCreatedSellmeijerCalculator = new SellmeijerCalculatorStub();
            LastCreatedEffectiveThicknessCalculator = new EffectiveThicknessCalculatorStub();
            LastCreatedPiezometricHeadAtExitCalculator = new PiezoHeadCalculatorStub();
            LastCreatedPipingProfilePropertyCalculator = new PipingProfilePropertyCalculatorStub();
        }

        public EffectiveThicknessCalculatorStub LastCreatedEffectiveThicknessCalculator { get; private set; }
        public UpliftCalculatorStub LastCreatedUpliftCalculator { get; private set; }
        public SellmeijerCalculatorStub LastCreatedSellmeijerCalculator { get; private set; }
        public HeaveCalculatorStub LastCreatedHeaveCalculator { get; private set; }
        public PiezoHeadCalculatorStub LastCreatedPiezometricHeadAtExitCalculator { get; private set; }
        public PipingProfilePropertyCalculatorStub LastCreatedPipingProfilePropertyCalculator { get; private set; }

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