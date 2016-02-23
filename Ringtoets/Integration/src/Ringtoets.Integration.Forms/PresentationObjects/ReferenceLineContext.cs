﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Core.Common.Base;

using Ringtoets.Common.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.Properties;

namespace Ringtoets.Integration.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for <see cref="ReferenceLine"/> instances.
    /// </summary>
    public class ReferenceLineContext : Observable, IEquatable<ReferenceLineContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceLineContext"/> class.
        /// </summary>
        /// <param name="parent">The parent owner of the data represented by the presentation object.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parent"/> is <c>null</c>.</exception>
        public ReferenceLineContext(AssessmentSectionBase parent)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent", Resources.ReferenceLineContext_Parent_assessment_section_cannot_be_null);
            }
            Parent = parent;
        }

        /// <summary>
        /// The reference line data wrapped by this presentation object.
        /// </summary>
        public ReferenceLine WrappedData
        {
            get
            {
                return Parent.ReferenceLine;
            }
        }

        /// <summary>
        /// The assessment section owning <see cref="WrappedData"/>.
        /// </summary>
        public AssessmentSectionBase Parent { get; private set; }

        #region Implementation: IEquatable<ReferenceLineContext>

        public bool Equals(ReferenceLineContext other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Parent.Equals(other.Parent);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((ReferenceLineContext)obj);
        }

        public override int GetHashCode()
        {
            return Parent.GetHashCode();
        }

        #endregion
    }
}