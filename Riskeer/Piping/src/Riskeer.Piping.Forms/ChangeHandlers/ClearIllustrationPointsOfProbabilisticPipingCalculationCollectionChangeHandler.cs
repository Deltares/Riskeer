using System;
using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Gui.Helpers;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Util;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Forms.ChangeHandlers
{
    public class ClearIllustrationPointsOfProbabilisticPipingCalculationCollectionChangeHandler : ClearIllustrationPointsOfCalculationCollectionChangeHandlerBase
    {
        private readonly IEnumerable<ProbabilisticPipingCalculationScenario> calculations;

        /// <summary>
        /// Creates a new instance of <see cref="ClearIllustrationPointsOfProbabilisticPipingCalculationCollectionChangeHandler"/>.
        /// </summary>
        /// <param name="inquiryHelper">Object responsible for inquiring confirmation.</param>
        /// <param name="calculations">The calculations for which the illustration points should be cleared for.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ClearIllustrationPointsOfProbabilisticPipingCalculationCollectionChangeHandler(IInquiryHelper inquiryHelper,
                                                                                              IEnumerable<ProbabilisticPipingCalculationScenario> calculations)
            : base(inquiryHelper)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            this.calculations = calculations;
        }

        public override IEnumerable<IObservable> ClearIllustrationPoints()
        {
            var affectedObjects = new List<IObservable>();
            foreach (var calculation in calculations)
            {
                if (ProbabilisticPipingIllustrationPointsHelper.HasIllustrationPoints(calculation))
                {
                    affectedObjects.Add(calculation);
                    calculation.ClearIllustrationPoints();
                }
            }

            return affectedObjects;
        }

        protected override string GetConfirmationMessage()
        {
            return RiskeerCommonFormsResources.ClearIllustrationPointsCalculationCollection_ConfirmationMessage;
        }
    }
}