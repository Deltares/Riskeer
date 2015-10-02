using System.Collections.Generic;
using DelftTools.Shell.Core;

namespace Wti.Data
{
    public class PipingData : IObservable
    {
        private IList<IObserver> observers = new List<IObserver>();
        private PipingOutput output;

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

        public double WaterVolumetricWeight { get; set; }
        public double UpliftModelFactor { get; set; }
        public double AssessmentLevel { get; set; } 
        public double PiezometricHeadExit { get; set; }
        public double DampingFactorExit { get; set; }
        public double PhreaticLevelExit { get; set; }
        public double PiezometricHeadPolder { get; set; }
        public double CriticalHeaveGradient { get; set; }
        public double ThicknessCoverageLayer { get; set; }
        public double SellmeijerModelFactor { get; set; }
        public double SellmeijerReductionFactor { get; set; }
        public double SeepageLength { get; set; }
        public double SandParticlesVolumicWeight { get; set; }
        public double WhitesDragCoefficient { get; set; }
        public double Diameter70 { get; set; }
        public double DarcyPermeability { get; set; }
        public double WaterKinematicViscosity { get; set; }
        public double Gravity { get; set; }
        public double ThicknessAquiferLayer { get; set; }
        public double MeanDiameter70 { get; set; }
        public double BeddingAngle { get; set; }
        public double ExitPointXCoordinate { get; set; }
    }
}