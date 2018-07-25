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
using System.Globalization;
using Core.Common.Base;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.FailureMechanism
{
    /// <summary>
    /// This class is the base implementation for a failure mechanism. Classes which want
    /// to implement <see cref="IFailureMechanism"/> can and should most likely inherit
    /// from this class.
    /// </summary>
    public abstract class FailureMechanismBase : Observable, IFailureMechanism
    {
        private static readonly Range<double> contributionValidityRange = new Range<double>(0, 100);
        private readonly FailureMechanismSectionCollection sectionCollection;
        private double contribution;

        /// <summary>
        /// Creates a new instance of the <see cref="FailureMechanismBase"/> class.
        /// </summary>
        /// <param name="name">The name of the failure mechanism.</param>
        /// <param name="failureMechanismCode">The code of the failure mechanism.</param>
        /// <param name="group">The group that this failure mechanism belongs to.</param>
        /// <exception cref="ArgumentException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="name"/> is <c>null</c> or empty.</item>
        /// <item><paramref name="failureMechanismCode"/> is <c>null</c> or empty.</item>
        /// </list>
        /// </exception>
        protected FailureMechanismBase(string name, string failureMechanismCode, int group)
        {
            ValidateParameters(name, failureMechanismCode);

            Name = name;
            Code = failureMechanismCode;
            Group = group;
            sectionCollection = new FailureMechanismSectionCollection();
            IsRelevant = true;
            InputComments = new Comment();
            OutputComments = new Comment();
            NotRelevantComments = new Comment();
        }

        public double Contribution
        {
            get
            {
                return contribution;
            }
            set
            {
                if (!contributionValidityRange.InRange(value))
                {
                    string message = string.Format(Resources.Contribution_Value_should_be_in_Range_0_,
                                                   contributionValidityRange.ToString(FormattableConstants.ShowAtLeastOneDecimal, CultureInfo.CurrentCulture));
                    throw new ArgumentOutOfRangeException(nameof(value), message);
                }

                contribution = value;
            }
        }

        public string Name { get; }

        public string Code { get; }

        public int Group { get; }

        public abstract IEnumerable<ICalculation> Calculations { get; }

        public IEnumerable<FailureMechanismSection> Sections
        {
            get
            {
                return sectionCollection;
            }
        }

        public Comment InputComments { get; }

        public Comment OutputComments { get; }

        public Comment NotRelevantComments { get; }

        public bool IsRelevant { get; set; }

        public void AddSections(IEnumerable<FailureMechanismSection> sections)
        {
            sectionCollection.AddRange(sections, string.Empty);

            foreach (FailureMechanismSection failureMechanismSection in sections)
            {
                AddSectionResult(failureMechanismSection);
            }
        }

        public virtual void ClearAllSections()
        {
            sectionCollection.Clear();
        }

        protected virtual void AddSectionResult(FailureMechanismSection section) {}

        private static void ValidateParameters(string failureMechanismName, string failureMechanismCode)
        {
            const string parameterIsRequired = "Parameter is required.";
            if (string.IsNullOrEmpty(failureMechanismName))
            {
                throw new ArgumentException(parameterIsRequired, nameof(failureMechanismName));
            }

            if (string.IsNullOrEmpty(failureMechanismCode))
            {
                throw new ArgumentException(parameterIsRequired, nameof(failureMechanismCode));
            }
        }
    }
}