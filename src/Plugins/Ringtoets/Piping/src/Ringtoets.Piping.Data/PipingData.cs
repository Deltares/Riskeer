using System.Collections.Generic;

using DelftTools.Shell.Core;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// This class holds the information which can be made visible in the graphical interface of the application.
    /// </summary>
    public class PipingData : IObservable
    {
        private readonly IList<IObserver> observers = new List<IObserver>();

        /// <summary>
        /// Constructs a new instance of <see cref="PipingData"/> with default values set for some of the parameters.
        /// </summary>
        public PipingData()
        {
            Name = "Piping";

            // Defaults as they have been defined in the DikesPiping Kernel's Technical Documentation of 07 Oct 15
            BeddingAngle = 37.0;
            MeanDiameter70 = 2.08e-4;
            Gravity = 9.81;
            WaterKinematicViscosity = 1.33e-6;
            WhitesDragCoefficient = 0.25;
            SandParticlesVolumicWeight = 16.5;
            WaterVolumetricWeight = 9.81;
            SellmeijerReductionFactor = 0.3;
            DampingFactorExit = 1.0;
        }

        /// <summary>
        /// Gets or sets the name the user gave this this calculation object.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the damping factor at the exit point.
        /// </summary>
        public double DampingFactorExit { get; set; }

        /// <summary>
        /// Gets or sets the reduction factor Sellmeijer.
        /// </summary>
        public double SellmeijerReductionFactor { get; set; }

        /// <summary>
        /// Gets or sets the volumetric weight of water.
        /// [kN/m&#179;]
        /// </summary>
        public double WaterVolumetricWeight { get; set; }

        /// <summary>
        /// Gets or sets the (lowerbound) volumic weight of sand grain material of a sand layer under water.
        /// [kN/m&#179;]
        /// </summary>
        public double SandParticlesVolumicWeight { get; set; }

        /// <summary>
        /// Gets or sets the White's drag coefficient.
        /// </summary>
        public double WhitesDragCoefficient { get; set; }

        /// <summary>
        /// Gets or sets the kinematic viscosity of water at 10 degrees Celsius.
        /// [m&#178;/s]
        /// </summary>
        public double WaterKinematicViscosity { get; set; }

        /// <summary>
        /// Gets or sets the gravitational acceleration.
        /// [m/s&#178;]
        /// </summary>
        public double Gravity { get; set; }

        /// <summary>
        /// Gets or sets the mean diameter of small scale tests applied to different kinds of sand, on which the formula of Sellmeijer has been fit.
        /// [m]
        /// </summary>
        public double MeanDiameter70 { get; set; }

        /// <summary>
        /// Gets or sets the angle of the force balance representing the amount in which sand grains resist rolling.
        /// [&#176;]
        /// </summary>
        public double BeddingAngle { get; set; }

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
        /// Gets or sets the surface line.
        /// </summary>
        public RingtoetsPipingSurfaceLine SurfaceLine { get; set; }

        /// <summary>
        /// Gets or sets <see cref="PipingOutput"/>, which contains the results of a Piping calculation.
        /// </summary>
        public PipingOutput Output { get; set; }

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