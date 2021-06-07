using System;
using Application.Riskeer.API.Implementation;

namespace Application.Riskeer.API.ErrorHandling {
    public class RiskeerApiException : Exception
    {
        public RiskeerApiException(RiskeerApiExceptionType type, string msg, Exception innerException = null) : base(msg, innerException)
        {
            Type = type;
        }

        public RiskeerApiException(RiskeerApiExceptionType type, Exception innerException = null) : this(type,type.GetMessage(), innerException) {}

        public RiskeerApiExceptionType Type { get; }
    }
}