using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Core.Common.Base;
using Core.Common.Utils.PropertyBag.Dynamic;

using Ringtoets.Piping.Data.Probabilistics;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;
using Ringtoets.Piping.Forms.TypeConverters.PropertyDescriptors;

namespace Ringtoets.Piping.Forms.TypeConverters
{
    /// <summary>
    /// Base class for shared implementation of <see cref="TypeConverter"/> to provide probabilistic
    /// distributions to the property editor.
    /// </summary>
    /// <typeparam name="T">Type of distributionci</typeparam>
    public abstract class DesignVariableTypeConverter<T> : TypeConverter where T:IDistribution
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var designVariable = (DesignVariable)value;
                var variablesText = string.Join(", ", Parameters.Select(p => p.GetSummary((T)designVariable.Distribution, culture)));
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

            var designVariable = (DesignVariable)value;
            PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptor.GetProperties(designVariable.Distribution);
            var properties = new PropertyDescriptor[Parameters.Length+2];
            properties[0] = new SimpleReadonlyPropertyDescriptorItem("Type verdeling", "De soort kansverdeling waarin deze parameter in gedefiniëerd wordt.", "DistributionType", DistributionName);
            for (int i = 0; i < Parameters.Length; i++)
            {
                properties[i+1] = CreatePropertyDescriptor(propertyDescriptorCollection, Parameters[i], observableParent);
            }
            properties[Parameters.Length + 1] = new SimpleReadonlyPropertyDescriptorItem("Rekenwaarde", "De representatieve waarde die gebruikt wordt door de berekening.", "DesignValue", designVariable.GetDesignValue());

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
            var reroutedPropertyDescriptor = new RoutedPropertyDescriptor(originalMeanPropertyDescriptor, o => ((DesignVariable)o).Distribution);
            return new TextPropertyDescriptorDecorator(reroutedPropertyDescriptor,
                                                       parameter.Symbol,
                                                       parameter.Description)
            {
                ObservableParent = observableParent,
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