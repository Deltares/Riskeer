﻿using System;
using System.ComponentModel;
using System.Globalization;

using Core.Common.BaseDelftTools;
using Core.Common.Utils.PropertyBag.Dynamic;
using Core.Common.Utils.Reflection;

using Ringtoets.Piping.Data.Probabilistics;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Forms.TypeConverters
{
    /// <summary>
    /// A <see cref="TypeConverter"/> implementation for <see cref="ShiftedLognormalDistribution"/>
    /// properties.
    /// </summary>
    /// <remarks>This class has been designed to be used in <see cref="PipingCalculationInputsProperties"/>.
    /// If its reused somewhere else, change notification might not work properly.</remarks>
    public class ShiftedLognormalDistributionTypeConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var distribution = (ShiftedLognormalDistribution)value;
                return String.Format("Verschoven lognormale verdeling (\u03BC = {0}, \u03C3 = {1}, Verschuiving = {2})",
                                     distribution.Mean.ToString(culture),
                                     distribution.StandardDeviation.ToString(culture),
                                     distribution.Shift);
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
            var properties = new PropertyDescriptor[3];
            properties[0] = CreateMeanPropertyDescriptor(propertyDescriptorCollection, observableParent);
            properties[1] = CreateStandardDeviationPropertyDescriptor(propertyDescriptorCollection, observableParent);
            properties[2] = CreateShiftPropertyDescriptor(propertyDescriptorCollection, observableParent);

            return new PropertyDescriptorCollection(properties);
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

        private PropertyDescriptor CreateMeanPropertyDescriptor(PropertyDescriptorCollection originalProperties, IObservable observableParent)
        {
            string propertyName = TypeUtils.GetMemberName<ShiftedLognormalDistribution>(nd => nd.Mean);
            PropertyDescriptor originalMeanPropertyDescriptor = originalProperties.Find(propertyName, false);
            return new TextPropertyDescriptorDecorator(originalMeanPropertyDescriptor,
                                                       "\u03BC",
                                                       "De gemiddelde waarde van de lognormale verdeling.")
            {
                ObservableParent = observableParent
            };
        }

        private PropertyDescriptor CreateStandardDeviationPropertyDescriptor(PropertyDescriptorCollection originalProperties, IObservable observableParent)
        {
            string propertyName = TypeUtils.GetMemberName<ShiftedLognormalDistribution>(nd => nd.StandardDeviation);
            PropertyDescriptor originalStandardDeviationPropertyDescriptor = originalProperties.Find(propertyName, false);
            return new TextPropertyDescriptorDecorator(originalStandardDeviationPropertyDescriptor,
                                                       "\u03C3",
                                                       "De standaardafwijking van de lognormale verdeling.")
            {
                ObservableParent = observableParent
            };
        }

        private PropertyDescriptor CreateShiftPropertyDescriptor(PropertyDescriptorCollection originalProperties, IObservable observableParent)
        {
            string propertyName = TypeUtils.GetMemberName<ShiftedLognormalDistribution>(nd => nd.Shift);
            PropertyDescriptor originalShiftPropertyDescriptor = originalProperties.Find(propertyName, false);
            return new TextPropertyDescriptorDecorator(originalShiftPropertyDescriptor,
                                                       "Verschuiving",
                                                       "De verschuiving van de lognormale verdeling.")
            {
                ObservableParent = observableParent
            };
        }
    }
}