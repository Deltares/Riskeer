// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;

namespace Ringtoets.Common.Data.TestUtil
{
    /// <summary>
    /// A simple test calculation that has input parameters containing a <see cref="ForeshoreProfile"/>
    /// property.
    /// </summary>
    public class TestCalculationWithForeshoreProfile : CloneableObservable,
                                                       ICalculation<TestCalculationWithForeshoreProfile.TestCalculationInputWithForeshoreProfile>
    {
        private TestCalculationWithForeshoreProfile(bool hasOutput)
        {
            InputParameters = new TestCalculationInputWithForeshoreProfile();
            HasOutput = hasOutput;
        }

        public string Name { get; set; }

        public bool ShouldCalculate
        {
            get
            {
                return !HasOutput;
            }
        }

        public bool HasOutput { get; private set; }

        public Comment Comments { get; }

        /// <summary>
        /// Gets the input parameters of the calculation.
        /// </summary>
        public TestCalculationInputWithForeshoreProfile InputParameters { get; }

        /// <summary>
        /// Create a calculation with a <see cref="ForeshoreProfile"/> and output.
        /// </summary>
        /// <param name="foreshoreProfile">The foreshore profile assigned to the calculation.</param>
        /// <returns>A <see cref="TestCalculationWithForeshoreProfile"/> with a foreshore and 
        /// calculation output.</returns>
        public static TestCalculationWithForeshoreProfile CreateCalculationWithOutput(ForeshoreProfile foreshoreProfile)
        {
            return new TestCalculationWithForeshoreProfile(true)
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfile
                }
            };
        }

        /// <summary>
        /// Create a calculation with a <see cref="ForeshoreProfile"/> and
        /// without output.
        /// </summary>
        /// <param name="foreshoreProfile">The foreshore profile assigned to the calculation.</param>
        /// <returns>A <see cref="TestCalculationWithForeshoreProfile"/> with a foreshore and 
        /// without calculation output.</returns>
        public static TestCalculationWithForeshoreProfile CreateCalculationWithoutOutput(ForeshoreProfile foreshoreProfile)
        {
            return new TestCalculationWithForeshoreProfile(false)
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfile
                }
            };
        }

        /// <summary>
        /// Creates a default calculation without a <see cref="ForeshoreProfile"/>
        /// and calculation output.
        /// </summary>
        /// <returns>A <see cref="TestCalculationWithForeshoreProfile"/> without 
        /// a foreshore and calculation output.</returns>
        public static TestCalculationWithForeshoreProfile CreateDefaultCalculation()
        {
            return new TestCalculationWithForeshoreProfile(false);
        }

        public void ClearOutput()
        {
            HasOutput = false;
        }

        /// <summary>
        /// A simple input class with a <see cref="ForeshoreProfile"/>.
        /// </summary>
        public class TestCalculationInputWithForeshoreProfile : ICalculationInput, IUseForeshore, IHasForeshoreProfile
        {
            public IEnumerable<IObserver> Observers
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public ForeshoreProfile ForeshoreProfile { get; set; }

            public bool IsForeshoreProfileInputSynchronized { get; }

            public bool UseForeshore { get; set; }

            public RoundedPoint2DCollection ForeshoreGeometry
            {
                get
                {
                    return ForeshoreProfile?.Geometry;
                }
            }

            public void Attach(IObserver observer)
            {
                throw new NotImplementedException();
            }

            public void Detach(IObserver observer)
            {
                throw new NotImplementedException();
            }

            public void NotifyObservers()
            {
                throw new NotImplementedException();
            }

            public object Clone()
            {
                throw new NotImplementedException();
            }

            public void SynchronizeForeshoreProfileInput()
            {
                throw new NotImplementedException();
            }
        }
    }
}