/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 17/05/2022
 * Time: 17:35
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
using Newtonsoft.Json;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests.Modules.ActionsDocumentView
{
    /// <summary>
    /// Description of SeManualFailureProbabilityFMInResultView.
    /// </summary>
    [TestModule("11B4F29F-CD05-44C2-A71F-20FD99B75F3B", ModuleType.UserCode, 1)]
    public class SeManualFailureProbabilityFMInResultView : ITestModule
    {
        
        string _trajectAssessmentInformationString = "";
        [TestVariable("9779eb56-6b69-4555-8fbd-8dcb44167c7b")]
        public string trajectAssessmentInformationString
        {
            get { return _trajectAssessmentInformationString; }
            set { _trajectAssessmentInformationString = value; }
        }
        
        
        string _currentFMLabel = "";
        [TestVariable("e1a699f8-5384-403d-8cb2-e94036ed136d")]
        public string currentFMLabel
        {
            get { return _currentFMLabel; }
            set { _currentFMLabel = value; }
        }
        
        string _ManualFailureProbToSet = "";
        [TestVariable("c37e1b0c-bde7-43d9-9a00-eac5b401cd23")]
        public string ManualFailureProbToSet
        {
            get { return _ManualFailureProbToSet; }
            set { _ManualFailureProbToSet = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public SeManualFailureProbabilityFMInResultView()
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
            var failureProbText = AutomatedSystemTestsRepository.Instance.RiskeerMainWindow.ContainerMultipleViews.DocumentViewContainer.FM_ResultView.FailureProbabilityFM.Element;
            failureProbText.SetAttributeValue("AccessibleValue", ManualFailureProbToSet);
            var trajectResultInformation = TrajectResultInformation.BuildAssessmenTrajectInformation(trajectAssessmentInformationString);
            trajectResultInformation.ListFMsResultInformation.Where(fmItem=>fmItem.Label==currentFMLabel).FirstOrDefault().FailureProbability = ManualFailureProbToSet;
            failureProbText.As<Text>().PressKeys("{Return}");
            trajectAssessmentInformationString = JsonConvert.SerializeObject(trajectResultInformation, Formatting.Indented);
        }
    }
}
