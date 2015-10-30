using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

using Core.Common.BaseDelftTools;
using Core.Common.Utils.PropertyBag.Dynamic;

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

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            IObservable observableParent = GetObservableOwnerOfDistribution(context);

            var propertyDescriptorCollection = TypeDescriptor.GetProperties(value);
            var properties = new PropertyDescriptor[Parameters.Length];
            for (int i = 0; i < Parameters.Length; i++)
            {
                properties[i] = CreatePropertyDescriptor(propertyDescriptorCollection, Parameters[i], observableParent);
            }

            return new PropertyDescriptorCollection(properties);
        }

        /// <summary>
        /// Gets the name of the distribution.
        /// </summary>
        protected abstract string DistributionName { get; }

        /// <summary>
        /// Gets all parameters available for the given distribution.
        /// </summary>
        protected abstract ParameterDefinition<T>[] Parameters { get; }

        private static PropertyDescriptor CreatePropertyDescriptor(PropertyDescriptorCollection originalProperties, ParameterDefinition<T> parameter, IObservable observableParent)
        {
            PropertyDescriptor originalMeanPropertyDescriptor = originalProperties.Find(parameter.PropertyName, false);
            return new TextPropertyDescriptorDecorator(originalMeanPropertyDescriptor,
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
            var pipingCalculationInputProperties = dynamicPropertyBag.WrappedObject as PipingCalculationInputsProperties;
            return pipingCalculationInputProperties != null ?
                       ((PipingCalculationInputs)pipingCalculationInputProperties.Data).PipingData :
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