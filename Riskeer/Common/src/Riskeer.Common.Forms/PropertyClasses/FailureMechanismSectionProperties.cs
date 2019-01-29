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
using System.ComponentModel;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="FailureMechanismSection"/> for properties panel.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class FailureMechanismSectionProperties : ObjectProperties<FailureMechanismSection>
    {
        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionProperties"/>.
        /// </summary>
        /// <param name="section">The section to show the properties for.</param>
        /// <param name="sectionStart">The start of the section from the beginning
        /// of the reference line in meters.</param>
        /// <param name="sectionEnd">The end of the section from the beginning of
        /// the reference line in meters.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/>
        /// is <c>null</c>.</exception>
        public FailureMechanismSectionProperties(FailureMechanismSection section, double sectionStart, double sectionEnd)
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            data = section;
            SectionStart = new RoundedDouble(2, sectionStart);
            SectionEnd = new RoundedDouble(2, sectionEnd);
        }

        [PropertyOrder(1)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismSection_Name_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.FailureMechanismSection_Name_Description))]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.SectionStart_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.SectionStart_Description))]
        public RoundedDouble SectionStart { get; }

        [PropertyOrder(3)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.SectionEnd_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.SectionEnd_Description))]
        public RoundedDouble SectionEnd { get; }

        [PropertyOrder(4)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismSection_Length_Rounded_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.FailureMechanismSection_Length_Rounded_Description))]
        public RoundedDouble Length
        {
            get
            {
                return new RoundedDouble(2, data.Length);
            }
        }

        [PropertyOrder(5)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismSection_StartPoint_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.FailureMechanismSection_StartPoint_Description))]
        public Point2D StartPoint
        {
            get
            {
                return data.StartPoint;
            }
        }

        [PropertyOrder(6)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismSection_EndPoint_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.FailureMechanismSection_EndPoint_Description))]
        public Point2D EndPoint
        {
            get
            {
                return data.EndPoint;
            }
        }

        public override string ToString()
        {
            return data.Name;
        }
    }
}