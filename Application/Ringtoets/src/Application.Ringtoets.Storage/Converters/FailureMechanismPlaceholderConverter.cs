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

using Application.Ringtoets.Storage.DbContext;

using Ringtoets.Integration.Data.Placeholders;

namespace Application.Ringtoets.Storage.Converters
{
    /// <summary>
    /// Converter for <see cref="FailureMechanismEntity"/> to <see cref="FailureMechanismPlaceholder"/> 
    /// and <see cref="FailureMechanismPlaceholder"/> to <see cref="FailureMechanismEntity"/>.
    /// </summary>
    public class FailureMechanismPlaceholderConverter : FailureMechanismConverterBase<FailureMechanismPlaceholder>
    {
        private readonly FailureMechanismType type;

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureMechanismPlaceholderConverter"/> class.
        /// </summary>
        /// <param name="type">The type of failure mechanism where the placeholder is used for.</param>
        public FailureMechanismPlaceholderConverter(FailureMechanismType type)
        {
            this.type = type;
        }

        protected override FailureMechanismPlaceholder ConstructFailureMechanism()
        {
            return new FailureMechanismPlaceholder(GetFailureMechanismType().ToString());
        }

        protected override FailureMechanismType GetFailureMechanismType()
        {
            return type;
        }
    }
}