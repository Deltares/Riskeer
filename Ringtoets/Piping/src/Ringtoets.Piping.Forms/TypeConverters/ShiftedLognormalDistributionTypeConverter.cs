using System;
using System.ComponentModel;

using Core.Common.BaseDelftTools;

using Ringtoets.Piping.Data.Probabilistics;
using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Forms.TypeConverters
{
    /// <summary>
    /// A <see cref="TypeConverter"/> implementation for <see cref="ShiftedLognormalDistribution"/>
    /// properties.
    /// </summary>
    /// <remarks>This class has been designed to be used in <see cref="PipingCalculationInputsProperties"/>.
    /// If its reused somewhere else, change notification might not work properly.</remarks>
    public class ShiftedLognormalDistributionTypeConverter : ProbabilisticDistributionTypeConverter<ShiftedLognormalDistribution>
    {
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            IObservable observableParent = GetObservableOwnerOfDistribution(context);

            var propertyDescriptorCollection = TypeDescriptor.GetProperties(value);
            var properties = new PropertyDescriptor[3];
            properties[0] = CreateMeanPropertyDescriptor(propertyDescriptorCollection, observableParent);
            properties[1] = CreateStandardDeviationPropertyDescriptor(propertyDescriptorCollection, observableParent);
            properties[2] = CreateShiftPropertyDescriptor(propertyDescriptorCollection, observableParent);

            return new PropertyDescriptorCollection(properties);
        }

        protected override ParameterDefinition<ShiftedLognormalDistribution>[] Parameters
        {
            get
            {
                return new[]
                {
                    new ParameterDefinition<ShiftedLognormalDistribution>
                    {
                        Symbol = "\u03BC", GetValue = distribution => distribution.Mean
                    },
                    new ParameterDefinition<ShiftedLognormalDistribution>
                    {
                        Symbol = "\u03C3", GetValue = distribution => distribution.StandardDeviation
                    },
                    new ParameterDefinition<ShiftedLognormalDistribution>
                    {
                        Symbol = "Verschuiving", GetValue = distribution => distribution.Shift
                    },
                };
            }
        }

        protected override string DistributionName
        {
            get
            {
                return "Verschoven lognormale verdeling";
            }
        }

        private PropertyDescriptor CreateMeanPropertyDescriptor(PropertyDescriptorCollection originalProperties, IObservable observableParent)
        {
            return CreatePropertyDescriptor(originalProperties, sld => sld.Mean, "\u03BC", "De gemiddelde waarde van de verschoven lognormale verdeling.", observableParent);
        }

        private PropertyDescriptor CreateStandardDeviationPropertyDescriptor(PropertyDescriptorCollection originalProperties, IObservable observableParent)
        {
            return CreatePropertyDescriptor(originalProperties, sld => sld.StandardDeviation, "\u03C3", "De standaardafwijking van de verschoven lognormale verdeling.", observableParent);
        }

        private PropertyDescriptor CreateShiftPropertyDescriptor(PropertyDescriptorCollection originalProperties, IObservable observableParent)
        {
            return CreatePropertyDescriptor(originalProperties, sld => sld.Shift, "Verschuiving", "De verschuiving van de lognormale verdeling.", observableParent);
        }
    }
}