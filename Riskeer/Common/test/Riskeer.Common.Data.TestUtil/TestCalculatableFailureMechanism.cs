﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Common.Base;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Common.Data.TestUtil
{
    /// <summary>
    /// Simple calculatable failure mechanism which can be used for testing.
    /// </summary>
    public class TestCalculatableFailureMechanism : FailureMechanismBase<TestFailureMechanismSectionResult>,
                                                    IHasGeneralInput, ICalculatableFailureMechanism
    {
        private const string defaultName = "Test failure mechanism";
        private const string defaultCode = "TFM";

        /// <summary>
        /// Creates a new instance of <see cref="TestCalculatableFailureMechanism"/> with a default name and code.
        /// </summary>
        public TestCalculatableFailureMechanism()
            : this(defaultName, defaultCode) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestCalculatableFailureMechanism"/> based on the input arguments.
        /// </summary>
        /// <param name="name">The name of the failure mechanism.</param>
        /// <param name="code">The code of the failure mechanism.</param>
        /// <exception cref="ArgumentException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="name"/> is <c>null</c> or empty.</item>
        /// <item><paramref name="code"/> is <c>null</c> or empty.</item>
        /// </list>
        /// </exception>
        public TestCalculatableFailureMechanism(string name, string code)
            : this(name, code, new List<ICalculation>()) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestCalculatableFailureMechanism"/> based on the input arguments.
        /// </summary>
        /// <param name="calculations">The collection of <see cref="ICalculation"/>.</param>
        public TestCalculatableFailureMechanism(IEnumerable<ICalculation> calculations)
            : this(defaultName, defaultCode, calculations) {}

        private TestCalculatableFailureMechanism(string name, string code, IEnumerable<ICalculation> calculations)
            : base(name, code)
        {
            CalculationsGroup = new CalculationGroup();
            CalculationsGroup.Children.AddRange(calculations);
            CalculationsInputComments = new Comment();
        }

        public CalculationGroup CalculationsGroup { get; }

        public Comment CalculationsInputComments { get; }

        public IEnumerable<ICalculation> Calculations => CalculationsGroup.GetCalculations();

        protected override void AddSectionDependentData(FailureMechanismSection section)
        {
            ((ObservableList<TestFailureMechanismSectionResult>) SectionResults).Add(new TestFailureMechanismSectionResult(section));
        }
    }
}