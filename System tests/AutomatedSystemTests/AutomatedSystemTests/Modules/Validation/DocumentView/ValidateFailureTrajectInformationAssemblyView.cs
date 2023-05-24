/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 16/05/2022
 * Time: 17:05
 * 
 * To change this template use Tools > Options > Coding > Edit standard headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using WinForms = System.Windows.Forms;
using Ranorex_Automation_Helpers.UserCodeCollections;
using System.Linq;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;
using Ranorex.Core.Repository;

namespace AutomatedSystemTests.Modules.Validation.DocumentView
{
    /// <summary>
    /// Description of ValidateFailureTrajectInformationAssemblyView.
    /// </summary>
    [TestModule("443FFA80-4CC7-4849-B6F7-51C8AF392630", ModuleType.UserCode, 1)]
    public class ValidateFailureTrajectInformationAssemblyView : ITestModule
    {
        
        string _trajectAssessmentInformationString = "";
        [TestVariable("55734f6c-666a-41d9-be67-e33bccb5c946")]
        public string trajectAssessmentInformationString
        {
            get { return _trajectAssessmentInformationString; }
            set { _trajectAssessmentInformationString = value; }
        }
        
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ValidateFailureTrajectInformationAssemblyView()
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
            var assemblyViewRepo = AutomatedSystemTests.AutomatedSystemTestsRepository.Instance.RiskeerMainWindow.ContainerMultipleViews.DocumentViewContainer.SecurityAssemblyView;
//            var actualFailureProbTraject = assemblyViewRepo.Summary.AssessmentFailureProbability.TextValue.ToNoGroupSeparator();
//            var actualAssessmentTrajectLabel = assemblyViewRepo.Summary.SecurityAssessment.TextValue;
//            var trajectResultInformation = TrajectResultInformation.BuildAssessmenTrajectInformation(trajectAssessmentInformationString);
//            var expectedFailureProbTraject = CalculateFailureProbTraject(trajectResultInformation);
//            var expectedAssessmentLabelTraject = CalculateAssessmentLabelTraject(trajectResultInformation.UpperLimitsSecurityBoundaries, expectedFailureProbTraject);
//            Validate.AreEqual(actualFailureProbTraject, expectedFailureProbTraject, "Validating failure probability traject: actual = " + actualFailureProbTraject + ", expected = " + expectedFailureProbTraject, false);
//            Validate.AreEqual(actualAssessmentTrajectLabel, expectedAssessmentLabelTraject, "Validating label traject: actual = " + actualAssessmentTrajectLabel + ", expected = " + expectedAssessmentLabelTraject, false);
//            ValidateCellBackgroundColorLabelAssessmentTraject(expectedAssessmentLabelTraject);
            ValidateCellBackgroundColorLabelAssessmentTraject("B");
        }
        
        private string CalculateFailureProbTraject(TrajectResultInformation trajectResultInformation)
        {
            double productInverseProbs = 1.0;
            foreach (var fmInfo in trajectResultInformation.ListFMsResultInformation) {
                var denomFailureProb = fmInfo.FailureProbability.ToNoGroupSeparator().Substring(2);
                productInverseProbs = productInverseProbs * (1.0 - 1.0 / Double.Parse(denomFailureProb));
            }
            var probTraject = 1- productInverseProbs;
            var denomProbTraject = Convert.ToInt32(1/probTraject).ToString();
            return "1/" + denomProbTraject;
        }
        
        private string CalculateAssessmentLabelTraject(List<string> upperLimits, string failureProb)
        {
            var lowestUpperLimit = upperLimits.Where(ul=>ProbabilityToDouble(failureProb)<ProbabilityToDouble(ul)).FirstOrDefault();
            var allLabels = new List<string> {
                "A+",
                "A",
                "B",
                "C",
                "D"
            };
            var label = allLabels[upperLimits.IndexOf(lowestUpperLimit)];
            return label;
        }
        
        private double ProbabilityToDouble(string prob)
        {
            return 1.0 / Double.Parse(prob.Substring(2));
        }
        
        private void ValidateCellBackgroundColorLabelAssessmentTraject(string label)
        {
            Report.Info("Validating background color of security assessment label field...");
            Validate.AttributeEqual(AutomatedSystemTests.AutomatedSystemTestsRepository.Instance.RiskeerMainWindow.ContainerMultipleViews.DocumentViewContainer.SecurityAssemblyView.Summary.GroupLabelInfo, "BackColor", GetColorCodeForLabel(label));
            return;
        }
        
        private string GetColorCodeForLabel(string label)
        {
            switch (label) {
                case "A+":
                    return "0, 255, 0";
                case "A":
                    return "118, 147, 60";
                case "B":
                    return "255, 255, 0";
                case "C":
                    return "255, 153, 0";
                case "D":
                    return "255, 0, 0";
                default:
                    return "Error!!! Assessment label " + label + "cannot be recognized!";
            }
        }
    }
}
