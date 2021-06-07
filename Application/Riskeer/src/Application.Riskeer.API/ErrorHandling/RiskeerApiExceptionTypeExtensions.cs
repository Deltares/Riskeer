using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Riskeer.API.Implementation;

namespace Application.Riskeer.API.ErrorHandling
{
    public static class RiskeerApiExceptionTypeExtensions
    {
        public static string GetMessage(this RiskeerApiExceptionType type)
        {
            switch (type)
            {
                case RiskeerApiExceptionType.InvalidRiskeerFile:
                    return "No valid Riskeer file";
                default:
                    return "Unknown exception";
            }
        }
    }
}
