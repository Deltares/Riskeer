using System.ComponentModel;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.DuneErosion.Data;

namespace Ringtoets.DuneErosion.Forms.PropertyClasses
{
    public class DuneLocationContextProperties : ObjectProperties<DuneLocation>
    {
        public long Id
        {
            get
            {
                return data.Id;
            }
        }

        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        public int CoastalAreaId
        {
            get
            {
                return data.CoastalAreaId;
            }
        }

        public RoundedDouble Offset
        {
            get
            {
                return data.Offset;
            }
        }

        public Point2D Location
        {
            get
            {
                return data.Location;
            }
        }

        public RoundedDouble WaterLevel
        {
            get
            {
                return data.Output == null
                           ? RoundedDouble.NaN
                           : data.Output.WaterLevel;
            }
        }

        public RoundedDouble WaveHeight
        {
            get
            {
                return data.Output == null
                           ? RoundedDouble.NaN
                           : data.Output.WaveHeight;
            }
        }

        public RoundedDouble WavePeriod
        {
            get
            {
                return data.Output == null
                           ? RoundedDouble.NaN
                           : data.Output.WavePeriod;
            }
        }

        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double TargetProbability
        {
            get
            {
                return data.Output == null
                           ? double.NaN
                           : data.Output.TargetProbability;
            }
        }

        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble TargetReliability
        {
            get
            {
                return data.Output == null
                           ? RoundedDouble.NaN
                           : data.Output.TargetReliability;
            }
        }

        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double CalculatedProbability
        {
            get
            {
                return data.Output == null
                           ? double.NaN
                           : data.Output.CalculatedProbability;
            }
        }

        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble CalculatedReliability
        {
            get
            {
                return data.Output == null
                           ? RoundedDouble.NaN
                           : data.Output.CalculatedReliability;
            }
        }

        public string Convergence
        {
            get
            {
                return new EnumDisplayWrapper<CalculationConvergence>(data.Output == null
                                                                          ? CalculationConvergence.NotCalculated
                                                                          : data.Output.CalculationConvergence).DisplayName;
            }
        }
    }
}