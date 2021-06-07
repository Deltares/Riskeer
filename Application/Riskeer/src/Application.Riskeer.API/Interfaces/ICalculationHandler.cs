namespace Application.Riskeer.API.Interfaces
{
    public interface ICalculationHandler<TCalculation> where TCalculation : ICalculationApi
    {
        TCalculation FindCalculation(IFailureMechanismApi failureMechanismApi, string calculationTitle);

        bool PerformCalculation(TCalculation calculation);

        bool ClearOutput(TCalculation calculation);
    }
}