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
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Core.Common.Base;
using Core.Common.Gui.PropertyBag;

using Ringtoets.Piping.Data.Probabilistics;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;
using Ringtoets.Piping.Forms.TypeConverters.PropertyDescriptors;

using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.TypeConverters
{
    /// <summary>
    /// Base class for shared implementation of <see cref="TypeConverter"/> to provide probabilistic
    /// distributions to the property editor.
    /// </summary>
    /// <typeparam name="T">Type of distribution</typeparam>
    public abstract class DesignVariableTypeConverter<T> : TypeConverter where T:IDistribution
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var designVariable = (DesignVariable<T>)value;
                var variablesText = string.Join(", ", Parameters.Select(p => p.GetSummary(designVariable.Distribution, culture)));
                return String.Format("{0} ({1})", designVariable.GetDesignValue(), variablesText);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            IObservable observableParent = GetObservableOwnerOfDistribution(context);

            var designVariable = (DesignVariable<T>)value;
            PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptor.GetProperties(designVariable.Distribution);
            var properties = new PropertyDescriptor[Parameters.Length+2];
            properties[0] = new SimpleReadonlyPropertyDescriptorItem(PipingFormsResources.DesignVariableTypeConverter_DestributionType_DisplayName,
                                                                     PipingFormsResources.DesignVariableTypeConverter_DistributionType_Description,
                                                                     "DistributionType",
                                                                     DistributionShortName);
            for (int i = 0; i < Parameters.Length; i++)
            {
                properties[i+1] = CreatePropertyDescriptor(propertyDescriptorCollection, Parameters[i], observableParent);
            }
            properties[Parameters.Length + 1] = new SimpleReadonlyPropertyDescriptorItem(PipingFormsResources.DesignVariableTypeConverter_DesignValue_DisplayName,
                                                                                         PipingFormsResources.DesignVariableTypeConverter_DesignValue_Description,
                                                                                         "DesignValue",
                                                                                         designVariable.GetDesignValue());

            return new PropertyDescriptorCollection(properties);
        }

        /// <summary>
        /// Gets the full name of the distribution.
        /// </summary>
        protected abstract string DistributionName { get; }

        /// <summary>
        /// Gets the short name of the distribution.
        /// </summary>
        protected abstract string DistributionShortName { get; }

        /// <summary>
        /// Gets all parameters available for the given distribution.
        /// </summary>
        protected abstract ParameterDefinition<T>[] Parameters { get; }

        private static PropertyDescriptor CreatePropertyDescriptor(PropertyDescriptorCollection originalProperties, ParameterDefinition<T> parameter, IObservable observableParent)
        {
            PropertyDescriptor originalMeanPropertyDescriptor = originalProperties.Find(parameter.PropertyName, false);
            var reroutedPropertyDescriptor = new RoutedPropertyDescriptor(originalMeanPropertyDescriptor, o => ((DesignVariable<T>)o).Distribution);
            return new TextPropertyDescriptorDecorator(reroutedPropertyDescriptor,
                                                       parameter.Symbol,
                                                       parameter.Description)
            {
                ObservableParent = observableParent
            };
        }

        private static IObservable GetObservableOwnerOfDistribution(ITypeDescriptorContext context)
        {
            if (context == null)
            {
                return null;
            }
            // Sadly, we need this hack in order to update the correct class
            var dynamicPropertyBag = context.Instance as DynamicPropertyBag;
            if (dynamicPropertyBag == null)
            {
                return null;
            }

            // Note: If this type converter is going to be reused for other classes, we 
            //       might want to reconsider how we want to propagate IObservable updates!
            var inputParameterContextProperties = dynamicPropertyBag.WrappedObject as PipingInputContextProperties;
            return inputParameterContextProperties != null ?
                       ((PipingInputContext)inputParameterContextProperties.Data).WrappedData:
                       null;
        }

        /// <summary>
        /// A generic parameter description class.
        /// </summary>
        /// <typeparam name="DistributionType">Type of object which as the parameter.</typeparam>
        protected class ParameterDefinition<DistributionType>
        {
            /// <summary>
            /// Instantiates a new instance of <see cref="ParameterDefinition{T}"/> for a
            /// given parameter.
            /// </summary>
            /// <param name="expression">The parameter expression.</param>
            public ParameterDefinition(Expression<Func<DistributionType, double>> expression)
            {
                PropertyName = ((MemberExpression)expression.Body).Member.Name;
                GetValue = expression.Compile();
            }

            /// <summary>
            /// The symbol of name of the parameter.
            /// </summary>
            public string Symbol { get; set; }

            /// <summary>
            /// Name of the property holding the parameter
            /// </summary>
            public string PropertyName { get; private set; }

            /// <summary>
            /// Method to retrieve the value of the parameter from a distribution.
            /// </summary>
            public Func<DistributionType, double> GetValue { get; private set; }

            /// <summary>
            /// Description text of the parameter.
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Retrieves the 'symbol = value' text.
            /// </summary>
            /// <param name="distribution">The distribution of evaluate.</param>
            /// <param name="culture">The culture used to print text in.</param>
            /// <returns>The summay text of the parameter.</returns>
            public string GetSummary(DistributionType distribution, CultureInfo culture)
            {
                return String.Format("{0} = {1}",
                                     Symbol,
                                     GetValue(distribution).ToString(culture));
            }
        }
    }
}