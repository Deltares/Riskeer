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

using System;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// Properties class for implementations of <see cref="IDistribution"/>.
    /// </summary>
    public abstract class DistributionPropertiesBase<T> : ObjectProperties<T> where T : IDistribution
    {
        private readonly string meanPropertyName;
        private readonly string standardDeviationPropertyName;
        private readonly bool isMeanReadOnly;
        private readonly bool isStandardDeviationReadOnly;
        private readonly IObservable observable;
        private readonly IChangeHandler changeHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributionPropertiesBase{T}"/> class.
        /// </summary>
        /// <param name="propertiesReadOnly">Indicates which properties, if any, should be
        /// marked as read-only.</param>
        /// <param name="observable">The object to be notified of changes to properties.
        /// Can be null if all properties are marked as read-only by <paramref name="propertiesReadOnly"/>.</param>
        /// <param name="handler">The handler that is used to handle property changes.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="observable"/>
        /// is <c>null</c> and any number of properties in this class is editable.</exception>
        protected DistributionPropertiesBase(
            DistributionPropertiesReadOnly propertiesReadOnly,
            IObservable observable,
            IChangeHandler handler)
        {
            if (observable == null && !propertiesReadOnly.HasFlag(DistributionPropertiesReadOnly.All))
            {
                throw new ArgumentException(@"Observable must be specified unless no property can be set.", "observable");
            }

            isMeanReadOnly = propertiesReadOnly.HasFlag(DistributionPropertiesReadOnly.Mean);
            isStandardDeviationReadOnly = propertiesReadOnly.HasFlag(DistributionPropertiesReadOnly.StandardDeviation);

            meanPropertyName = TypeUtils.GetMemberName<DistributionPropertiesBase<T>>(rd => rd.Mean);
            standardDeviationPropertyName = TypeUtils.GetMemberName<DistributionPropertiesBase<T>>(rd => rd.StandardDeviation);

            this.observable = observable;
            changeHandler = handler;
        }

        [PropertyOrder(1)]
        [ResourcesDisplayName(typeof(Resources), "Distribution_DistributionType_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Distribution_DistributionType_Description")]
        public abstract string DistributionType { get; }

        [PropertyOrder(2)]
        [DynamicReadOnly]
        [ResourcesDisplayName(typeof(Resources), "NormalDistribution_Mean_DisplayName")]
        public virtual RoundedDouble Mean
        {
            get
            {
                return data.Mean;
            }
            set
            {
                if (isMeanReadOnly)
                {
                    throw new ArgumentException("Mean is set to be read-only.");
                }
                data.Mean = value;
                NotifyPropertyChanged();
            }
        }

        [PropertyOrder(3)]
        [DynamicReadOnly]
        [ResourcesDisplayName(typeof(Resources), "NormalDistribution_StandardDeviation_DisplayName")]
        public virtual RoundedDouble StandardDeviation
        {
            get
            {
                return data.StandardDeviation;
            }
            set
            {
                if (isStandardDeviationReadOnly)
                {
                    throw new ArgumentException("StandardDeviation is set to be read-only.");
                }
                data.StandardDeviation = value;
                NotifyPropertyChanged();
            }
        }

        [DynamicReadOnlyValidationMethod]
        public bool DynamicReadOnlyValidationMethod(string propertyName)
        {
            if (propertyName == meanPropertyName)
            {
                return isMeanReadOnly;
            }
            if (propertyName == standardDeviationPropertyName)
            {
                return isStandardDeviationReadOnly;
            }
            return false;
        }

        public override string ToString()
        {
            return data == null ? string.Empty :
                       string.Format("{0} ({1} = {2})",
                                     Mean, Resources.NormalDistribution_StandardDeviation_DisplayName, StandardDeviation);
        }

        /// <summary>
        /// Sends notifications due to a change of a property.
        /// </summary>
        protected void NotifyPropertyChanged()
        {
            if (changeHandler != null)
            {
                changeHandler.PropertyChanged();
            }
            observable.NotifyObservers();
        }

        /// <summary>
        /// Interface defining the operations of handling a change of <see cref="DistributionPropertiesBase{T}"/>.
        /// </summary>
        public interface IChangeHandler
        {
            /// <summary>
            /// Defines the action that is executed after a property of <see cref="DistributionPropertiesBase{T}"/>
            /// has been changed.
            /// </summary>
            void PropertyChanged();
        }
    }
}