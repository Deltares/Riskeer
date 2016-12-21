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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.KernelWrapper.TestUtil
{
    /// <summary>
    /// Class that holds a configuration that can be changed and used to create a <see cref="PipingCalculatorInput"/>
    /// with.
    /// </summary>
    public class TestPipingInput
    {
        private readonly PipingCalculatorInput.ConstructionProperties constructionProperties = new PipingCalculatorInput.ConstructionProperties();

        private readonly Random random = new Random(22);

        public double WaterVolumetricWeight
        {
            get
            {
                return constructionProperties.WaterVolumetricWeight;
            }
            set
            {
                constructionProperties.WaterVolumetricWeight = value;
            }
        }

        public double SaturatedVolumicWeightOfCoverageLayer
        {
            get
            {
                return constructionProperties.SaturatedVolumicWeightOfCoverageLayer;
            }
            set
            {
                constructionProperties.SaturatedVolumicWeightOfCoverageLayer = value;
            }
        }
        public double UpliftModelFactor
        {
            get
            {
                return constructionProperties.UpliftModelFactor;
            }
            set
            {
                constructionProperties.UpliftModelFactor = value;
            }
        }
        public double AssessmentLevel
        {
            get
            {
                return constructionProperties.AssessmentLevel;
            }
            set
            {
                constructionProperties.AssessmentLevel = value;
            }
        }
        public double PiezometricHeadExit
        {
            get
            {
                return constructionProperties.PiezometricHeadExit;
            }
            set
            {
                constructionProperties.PiezometricHeadExit = value;
            }
        }
        public double DampingFactorExit
        {
            get
            {
                return constructionProperties.DampingFactorExit;
            }
            set
            {
                constructionProperties.DampingFactorExit = value;
            }
        }
        public double PhreaticLevelExit
        {
            get
            {
                return constructionProperties.PhreaticLevelExit;
            }
            set
            {
                constructionProperties.PhreaticLevelExit = value;
            }
        }
        public double CriticalHeaveGradient
        {
            get
            {
                return constructionProperties.CriticalHeaveGradient;
            }
            set
            {
                constructionProperties.CriticalHeaveGradient = value;
            }
        }
        public double ThicknessCoverageLayer
        {
            get
            {
                return constructionProperties.ThicknessCoverageLayer;
            }
            set
            {
                constructionProperties.ThicknessCoverageLayer = value;
            }
        }
        public double EffectiveThicknessCoverageLayer
        {
            get
            {
                return constructionProperties.EffectiveThicknessCoverageLayer;
            }
            set
            {
                constructionProperties.EffectiveThicknessCoverageLayer = value;
            }
        }
        public double SellmeijerModelFactor
        {
            get
            {
                return constructionProperties.SellmeijerModelFactor;
            }
            set
            {
                constructionProperties.SellmeijerModelFactor = value;
            }
        }
        public double SellmeijerReductionFactor
        {
            get
            {
                return constructionProperties.SellmeijerReductionFactor;
            }
            set
            {
                constructionProperties.SellmeijerReductionFactor = value;
            }
        }
        public double SeepageLength
        {
            get
            {
                return constructionProperties.SeepageLength;
            }
            set
            {
                constructionProperties.SeepageLength = value;
            }
        }
        public double SandParticlesVolumicWeight
        {
            get
            {
                return constructionProperties.SandParticlesVolumicWeight;
            }
            set
            {
                constructionProperties.SandParticlesVolumicWeight = value;
            }
        }
        public double WhitesDragCoefficient
        {
            get
            {
                return constructionProperties.WhitesDragCoefficient;
            }
            set
            {
                constructionProperties.WhitesDragCoefficient = value;
            }
        }
        public double Diameter70
        {
            get
            {
                return constructionProperties.Diameter70;
            }
            set
            {
                constructionProperties.Diameter70 = value;
            }
        }
        public double DarcyPermeability
        {
            get
            {
                return constructionProperties.DarcyPermeability;
            }
            set
            {
                constructionProperties.DarcyPermeability = value;
            }
        }
        public double WaterKinematicViscosity
        {
            get
            {
                return constructionProperties.WaterKinematicViscosity;
            }
            set
            {
                constructionProperties.WaterKinematicViscosity = value;
            }
        }
        public double Gravity
        {
            get
            {
                return constructionProperties.Gravity;
            }
            set
            {
                constructionProperties.Gravity = value;
            }
        }
        public double ExitPointXCoordinate
        {
            get
            {
                return constructionProperties.ExitPointXCoordinate;
            }
            set
            {
                constructionProperties.ExitPointXCoordinate = value;
            }
        }
        public double BeddingAngle
        {
            get
            {
                return constructionProperties.BeddingAngle;
            }
            set
            {
                constructionProperties.BeddingAngle = value;
            }
        }
        public double MeanDiameter70
        {
            get
            {
                return constructionProperties.MeanDiameter70;
            }
            set
            {
                constructionProperties.MeanDiameter70 = value;
            }
        }
        public double ThicknessAquiferLayer
        {
            get
            {
                return constructionProperties.ThicknessAquiferLayer;
            }
            set
            {
                constructionProperties.ThicknessAquiferLayer = value;
            }
        }
        public RingtoetsPipingSurfaceLine SurfaceLine
        {
            get
            {
                return constructionProperties.SurfaceLine;
            }
            set
            {
                constructionProperties.SurfaceLine = value;
            }
        }
        public PipingSoilProfile SoilProfile
        {
            get
            {
                return constructionProperties.SoilProfile;
            }
            set
            {
                constructionProperties.SoilProfile = value;
            }
        }

        private double last;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestPipingInput"/> class with all
        /// parameters relevant for <see cref="PipingCalculatorInput"/> set.
        /// </summary>
        public TestPipingInput()
        {
            WaterVolumetricWeight = NextIncrementalDouble();
            SaturatedVolumicWeightOfCoverageLayer = NextIncrementalDouble();
            UpliftModelFactor = NextIncrementalDouble();
            AssessmentLevel = NextIncrementalDouble();
            PiezometricHeadExit = NextIncrementalDouble();
            PhreaticLevelExit = NextIncrementalDouble();
            DampingFactorExit = NextIncrementalDouble();
            CriticalHeaveGradient = NextIncrementalDouble();
            ThicknessCoverageLayer = NextIncrementalDouble();
            EffectiveThicknessCoverageLayer = NextIncrementalDouble();
            SellmeijerModelFactor = NextIncrementalDouble();
            SellmeijerReductionFactor = NextIncrementalDouble();
            SeepageLength = NextIncrementalDouble();
            SandParticlesVolumicWeight = NextIncrementalDouble();
            WhitesDragCoefficient = NextIncrementalDouble();
            Diameter70 = NextIncrementalDouble();
            DarcyPermeability = NextIncrementalDouble();
            WaterKinematicViscosity = NextIncrementalDouble();
            Gravity = NextIncrementalDouble();
            ExitPointXCoordinate = 0.5;
            BeddingAngle = NextIncrementalDouble();
            MeanDiameter70 = NextIncrementalDouble();
            ThicknessAquiferLayer = NextIncrementalDouble();
            SurfaceLine = CreateValidSurfaceLine();
            SoilProfile = CreateValidSoilProfile();
        }

        /// <summary>
        /// Returns the current set value as a <see cref="PipingCalculatorInput"/>
        /// </summary>
        public PipingCalculatorInput AsRealInput()
        {
            return new PipingCalculatorInput(constructionProperties);
        }

        private PipingSoilProfile CreateValidSoilProfile()
        {
            return new PipingSoilProfile(string.Empty, -2, new[]
            {
                new PipingSoilLayer(9),
                new PipingSoilLayer(4)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(2),
                new PipingSoilLayer(-1),
            }, SoilProfileType.SoilProfile1D, 1234L);
        }

        private RingtoetsPipingSurfaceLine CreateValidSurfaceLine()
        {
            var ringtoetsPipingSurfaceLine = new RingtoetsPipingSurfaceLine();
            ringtoetsPipingSurfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 2),
                new Point3D(1, 0, 8),
                new Point3D(2, 0, -1)
            });
            return ringtoetsPipingSurfaceLine;
        }

        /// <summary>
        /// The returned double is sure to be different from the last time it was called.
        /// </summary>
        private double NextIncrementalDouble()
        {
            return last += random.NextDouble() + 1e-6;
        }
    }
}