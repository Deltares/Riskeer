using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

using Core.Common.BaseDelftTools;
using Core.Common.Utils.PropertyBag.Dynamic;
using Core.Common.Utils.Reflection;

using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Forms.TypeConverters
{
    /// <summary>
    /// Base class for shared implementation of <see cref="TypeConverter"/> to provide probabilistic
    /// distributions to the property editor.
    /// </summary>
    /// <typeparam name="T">Type of distributionci</typeparam>
    public abstract class ProbabilisticDistributionTypeConverter<T> : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var distribution = (T)value;
                var variablesText = string.Join(", ", Parameters.Select(p => p.GetSummary(distribution, culture)));
                return String.Format("{0} ({1})", DistributionName, variablesText);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Gets all parameters available for the given distribution.
        /// </summary>
        protected abstract ParameterDefinition<T>[] Parameters { get; }

        /// <summary>
        /// Gets the name of the distribution.
        /// </summary>
        protected abstract string DistributionName { get; }

        protected static IObservable GetObservableOwnerOfDistribution(ITypeDescriptorContext context)
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
            var pipingCalculationInputProperties = dynamicPropertyBag.WrappedObject as PipingCalculationInputsProperties;
            return pipingCalculationInputProperties != null ?
                       ((PipingCalculationInputs)pipingCalculationInputProperties.Data).PipingData :
                       null;
        }

        protected static PropertyDescriptor CreatePropertyDescriptor(PropertyDescriptorCollection originalProperties, Expression<Func<T, object>> propertyExpression, string customDisplayName, string customDescription, IObservable observableParent)
        {
            string propertyName = TypeUtils.GetMemberName(propertyExpression);
            PropertyDescriptor originalMeanPropertyDescriptor = originalProperties.Find(propertyName, false);
            return new TextPropertyDescriptorDecorator(originalMeanPropertyDescriptor,
                                                       customDisplayName,
                                                       customDescription)
            {
                ObservableParent = observableParent
            };
        }

        /// <summary>
        /// A generic parameter description class.
        /// </summary>
        /// <typeparam name="DistributionType">Type of object which as the parameter.</typeparam>
        protected class ParameterDefinition<DistributionType>
        {
            /// <summary>
            /// The symbol of name of the parameter.
            /// </summary>
            public string Symbol { get; set; }

            /// <summary>
            /// Method to retrieve the value of the parameter from a distribution.
            /// </summary>
            public Func<DistributionType, double> GetValue { get; set; }

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