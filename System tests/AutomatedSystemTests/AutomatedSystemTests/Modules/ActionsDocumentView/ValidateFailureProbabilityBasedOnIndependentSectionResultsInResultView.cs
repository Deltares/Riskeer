/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 12/05/2022
 * Time: 17:40
 * 
 * To change this template use Tools > Options > Coding > Edit standard headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;using System.Linq;
using WinForms = System.Windows.Forms;
using Newtonsoft.Json;
using Ranorex_Automation_Helpers.UserCodeCollections;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests.Modules.ActionsDocumentView
{
    /// <summary>
    /// Description of ValidateFailureProbabilityFMInResultView.
    /// </summary>
    [TestModule("624D9FA1-26B3-40F8-BFC7-47503BA11A9D", ModuleType.UserCode, 1)]
    public class ValidateFailureProbabilityBasedOnIndependentSectionResultsInResultView : ITestModule
    {
        
        string _labelFM = "";
        [TestVariable("d0d431e3-1478-40d4-b8fa-f84bbc8597e7")]
        public string labelFM
        {
            get { return _labelFM; }
            set { _labelFM = value; }
        }
        
        string _trajectAssessmentInformationString = "";
        [TestVariable("83cc88df-0bcd-4b5a-99dd-5ddb8f35a27d")]
        public string trajectAssessmentInformationString
        {
            get { return _trajectAssessmentInformationString; }
            set { _trajectAssessmentInformationString = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ValidateFailureProbabilityBasedOnIndependentSectionResultsInResultView()
        {
            // Do not delete - a parameterless constructor is required!
        }

        /// <summary>
        /// Performs the playback of actions in this module.
        /// </summary>
        /// <remarks>You should not call this method directly, instead pass the module
        /// instance to the <see cref="TestModuleRunner.Run(ITestModule)"/> method
        /// that will in turn invoke this method.</remarks>
        void ITestModule.Run()
        {
            Mouse.DefaultMoveTime = 0;
            Keyboard.DefaultKeyPressTime = 0;
            Delay.SpeedFactor = 0;
            var trajectResultInformation = BuildAssessmenTrajectInformation(trajectAssessmentInformationString);
            var currentFMResultInformation = trajectResultInformation.ListFMsResultInformation.Where(fmItem=>fmItem.Label==labelFM).FirstOrDefault();
            var actualFailureProbFM = currentFMResultInformation.FailureProbability.ToNoGroupSeparator();
            var expectedDoubleFailureProbFM = CalculateExpectedFailureProbFM(currentFMResultInformation);
            var expectedDenominatorFractionFailureProbFM = Convert.ToInt32(1.0/expectedDoubleFailureProbFM);
            var expectedFailureProbFM  = "1/" + expectedDenominatorFractionFailureProbFM.ToString();
            var comparison = actualFailureProbFM == expectedFailureProbFM;
            

        }
        
            private TrajectResultInformation BuildAssessmenTrajectInformation(string trajectAssessmentInformationString)
        {
            TrajectResultInformation trajectAssessmentInformation;
            if (trajectAssessmentInformationString=="") {
                trajectAssessmentInformation = new TrajectResultInformation();
            } else {
                var error = false;
                trajectAssessmentInformation = JsonConvert.DeserializeObject<TrajectResultInformation>(trajectAssessmentInformationString, new JsonSerializerSettings
                {
                    Error = (s, e) =>
                    {
                        error = true;
                        e.ErrorContext.Handled = true;
                    }
                }
            );
                if (error==true) {
                    
                    Report.Log(ReportLevel.Error, "error unserializing json string for trajectAssessmentInformationString: " + trajectAssessmentInformationString);
                }
                
            }
            return trajectAssessmentInformation;
        }
            
            private double CalculateExpectedFailureProbFM(FailureMechanismResultInformation fmResultInfo)
            {
                return PCombin1(fmResultInfo);
            }
            
            private double PCombin1(FailureMechanismResultInformation fmResultInfo)
            {
                double productInverseFailureProbability = 1.0;
                foreach (var section in fmResultInfo.SectionList) {
                    var denominator = section.CalculationFailureProbPerSection.Substring(2).ToNoGroupSeparator();
                    if (denominator!="Oneindig") {
                        productInverseFailureProbability = productInverseFailureProbability * (1.0 - 1.0/Double.Parse(denominator));
                    }
                }
                double failureProbFMMech1 = 1.0-productInverseFailureProbability;
                return failureProbFMMech1;
            }

    }
}
