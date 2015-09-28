using System;
using System.Collections.Generic;
using System.ComponentModel;
using DelftTools.Controls.Swf.DataEditorGenerator.FromType;
using DelftTools.Utils.Aop;

namespace DelftTools.Tests.Controls.Swf.DataEditorGenerator.Domain
{
    [Entity]
    public class Car
    {
        public Car()
        {
            IntegerArray = new[] {1, 2, 3, 4, 5, 6, 7};
            DoubleArray = new[] {0.0, 0.1, 0.2};
            DateTimeArray = new[] {new DateTime(1999, 12, 31, 23, 59, 58), new DateTime(1999, 12, 31, 23, 59, 59)};
            Name = "";
        }

        public string Name { get; set; }
        
        [Description("Miles per gallon")]
        public double MilesPerGallon { get; set; }

        [Description("Kilometers per liter")]
        [DependentProperty]
        public double KilometersPerLiter
        {
            get { return MilesPerGallon/15; }
        }

        [Description("Car type")]
        public CarTypes CarType { get; set; }

        public enum CarTypes
        {
            Sedan,
            Hatchback,
            [Description("Four wheel drive")] FourWheelDrive,
        }

        [Description("Array of integers")]
        public IList<int> IntegerArray { get; set; }

        [Description("Array of doubles")]
        public IList<double> DoubleArray { get; set; }

        [Description("Array of times")]
        public IList<DateTime> DateTimeArray { get; set; }
    }
}