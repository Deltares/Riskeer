// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

namespace Riskeer.Piping.KernelWrapper.SubCalculator
{
    /// <summary>
    /// Factory which creates the sub calculators from the piping kernel.
    /// </summary>
    public class PipingSubCalculatorFactory : IPipingSubCalculatorFactory
    {
        private static IPipingSubCalculatorFactory instance;

        private PipingSubCalculatorFactory() {}

        /// <summary>
        /// Gets or sets an instance of <see cref="IPipingSubCalculatorFactory"/>.
        /// </summary>
        public static IPipingSubCalculatorFactory Instance
        {
            get
            {
                return instance ?? (instance = new PipingSubCalculatorFactory());
            }
            set
            {
                instance = value;
            }
        }

        public IUpliftCalculator CreateUpliftCalculator()
        {
            return new UpliftCalculator();
        }

        public IHeaveCalculator CreateHeaveCalculator()
        {
            return new HeaveCalculator();
        }

        public ISellmeijerCalculator CreateSellmeijerCalculator()
        {
            return new SellmeijerCalculator();
        }

        public IEffectiveThicknessCalculator CreateEffectiveThicknessCalculator()
        {
            return new EffectiveThicknessCalculator();
        }

        public IPiezoHeadCalculator CreatePiezometricHeadAtExitCalculator()
        {
            return new PiezoHeadCalculator();
        }

        public IPipingProfilePropertyCalculator CreatePipingProfilePropertyCalculator()
        {
            return new PipingProfilePropertyCalculator();
        }
    }
}