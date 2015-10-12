﻿using System.Collections.Generic;
using DelftTools.Shell.Core;

namespace Wti.Data
{
    public class PipingData : IObservable
    {
        private readonly IList<IObserver> observers = new List<IObserver>();
        private PipingOutput output;

        // Defaults as they have been defined in the DikesPiping Kernel's Technical Documentation of 07 Oct 15
        private double dampingFactorExit = 1.0;
        private double sellmeijerReductionFactor = 0.3;
        private double waterVolumetricWeight = 9.81;
        private double sandParticlesVolumicWeight = 16.5;
        private double whitesDragCoefficient = 0.25;
        private double waterKinematicViscosity = 1.33e-6;
        private double gravity = 9.81;
        private double meanDiameter70 = 2.08e-4;
        private double beddingAngle = 37.0;

        /// <summary>
        /// Gets or sets the damping factor at the exit point.
        /// </summary>
        public double DampingFactorExit
        {
            get
            {
                return dampingFactorExit;
            }
            set
            {
                dampingFactorExit = value;
            }
        }

        /// <summary>
        /// Gets or sets the reduction factor Sellmeijer.
        /// </summary>
        public double SellmeijerReductionFactor
        {
            get
            {
                return sellmeijerReductionFactor;
            }
            set
            {
                sellmeijerReductionFactor = value;
            }
        }

        /// <summary>
        /// Gets or sets the volumetric weight of water.
        /// [kN/m&#179;]
        /// </summary>
        public double WaterVolumetricWeight
        {
            get
            {
                return waterVolumetricWeight;
            }
            set
            {
                waterVolumetricWeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the (lowerbound) volumic weight of sand grain material of a sand layer under water.
        /// [kN/m&#179;]
        /// </summary>
        public double SandParticlesVolumicWeight
        {
            get
            {
                return sandParticlesVolumicWeight;
            }
            set
            {
                sandParticlesVolumicWeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the White's drag coefficient.
        /// </summary>
        public double WhitesDragCoefficient
        {
            get
            {
                return whitesDragCoefficient;
            }
            set
            {
                whitesDragCoefficient = value;
            }
        }

        /// <summary>
        /// Gets or sets the kinematic viscosity of water at 10 degrees Celsius.
        /// [m&#178;/s]
        /// </summary>
        public double WaterKinematicViscosity
        {
            get
            {
                return waterKinematicViscosity;
            }
            set
            {
                waterKinematicViscosity = value;
            }
        }

        /// <summary>
        /// Gets or sets the gravitational acceleration.
        /// [m/s&#178;]
        /// </summary>
        public double Gravity
        {
            get
            {
                return gravity;
            }
            set
            {
                gravity = value;
            }
        }

        /// <summary>
        /// Gets or sets the mean diameter of small scale tests applied to different kinds of sand, on which the formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        public double MeanDiameter70
        {
            get
            {
                return meanDiameter70;
            }
            set
            {
                meanDiameter70 = value;
            }
        }

        /// <summary>
        /// Gets or sets the angle of the force balance representing the amount in which sand grains resist rolling.
        /// [&#176;]
        /// </summary>
        public double BeddingAngle
        {
            get
            {
                return beddingAngle;
            }
            set
            {
                beddingAngle = value;
            }
        }

        /// <summary>
        /// Gets or sets the calculation value used to account for uncertainty in the model for uplift.
        /// </summary>
        public double UpliftModelFactor { get; set; }

        /// <summary>
        /// Gets or sets the outside high water level.
        /// [m]
        /// </summary>
        public double AssessmentLevel { get; set; }

        /// <summary>
        /// Gets or sets the piezometric head at the exit point.
        /// [m]
        /// </summary>
        public double PiezometricHeadExit { get; set; }

        /// <summary>
        /// Gets or sets the phreatic level at the exit point.
        /// [m]
        /// </summary>
        public double PhreaticLevelExit { get; set; }

        /// <summary>
        /// Gets or sets the piezometric head in the hinterland.
        /// [m]
        /// </summary>
        public double PiezometricHeadPolder { get; set; }

        /// <summary>
        /// Gets or sets the critical exit gradient for heave.
        /// </summary>
        public double CriticalHeaveGradient { get; set; }

        /// <summary>
        /// Gets or sets the total thickness of the coverage layer at the exit point.
        /// [m]
        /// </summary>
        public double ThicknessCoverageLayer { get; set; }

        /// <summary>
        /// Gets or sets the calculation value used to account for uncertainty in the model for Sellmeijer.
        /// </summary>
        public double SellmeijerModelFactor { get; set; }

        /// <summary>
        /// Gets or sets the horizontal distance between entree and exit point.
        /// [m]
        /// </summary>
        public double SeepageLength { get; set; }

        /// <summary>
        /// Gets or sets the sieve size through which 70% fraction of the grains of the top part of the aquifer passes.
        /// [m]
        /// </summary>
        public double Diameter70 { get; set; }

        /// <summary>
        /// Gets or sets the Darcy-speed with which water flows through the aquifer layer.
        /// [m/s]
        /// </summary>
        public double DarcyPermeability { get; set; }

        /// <summary>
        /// Gets or sets the thickness of the aquifer layer.
        /// [m]
        /// </summary>
        public double ThicknessAquiferLayer { get; set; }

        /// <summary>
        /// Gets or sets the x coordinate of the exit point.
        /// [m]
        /// </summary>
        public double ExitPointXCoordinate { get; set; }

        /// <summary>
        /// Gets or sets <see cref="PipingOutput"/>, which contains the results of a Piping calculation.
        /// </summary>
        public PipingOutput Output
        {
            get
            {
                return output;
            }
            set
            {
                output = value;
                NotifyObservers();
            }
        }

        public void Attach(IObserver observer)
        {
            observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            observers.Remove(observer);
        }

        public void NotifyObservers()
        {
            foreach (var observer in observers)
            {
                observer.UpdateObserver();
            }
        }
    }
}