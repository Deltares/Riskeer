/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 18/12/2020
 * Time: 18:47
 * 
 * To change this template use Tools > Options > Coding > Edit standard headers.
 */
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using WinForms = System.Windows.Forms;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests.Modules.Validation.Abstract
{
    /// <summary>
    /// Description of ValidateCombinedApproxFailureProbabilityAndBeta.
    /// </summary>
    [TestModule("C58BB999-6267-4CDB-B506-9567A6BD06A2", ModuleType.UserCode, 1)]
    public class ValidateCombinedApproxFailureProbabilityAndBeta : ITestModule
    {
        
        
        string _betaUplift = "";
        [TestVariable("deab0d92-23b4-4e6c-9e83-33a30e0ecd46")]
        public string betaUplift
        {
            get { return _betaUplift; }
            set { _betaUplift = value; }
        }
        
        string _probUplift = "";
        [TestVariable("9069fa4a-c94d-432f-bb5c-82aa7d96c55f")]
        public string probUplift
        {
            get { return _probUplift; }
            set { _probUplift = value; }
        }
        
        
        string _betaHeave = "";
        [TestVariable("be5f7cf3-2536-4bf1-9876-6cb71505a180")]
        public string betaHeave
        {
            get { return _betaHeave; }
            set { _betaHeave = value; }
        }
        
        string _probHeave = "";
        [TestVariable("2de942d5-1e24-463c-bc72-f741da714108")]
        public string probHeave
        {
            get { return _probHeave; }
            set { _probHeave = value; }
        }
        
        string _betaSellmeijer = "";
        [TestVariable("1a943d9c-3098-4672-9e40-f694caedba9d")]
        public string betaSellmeijer
        {
            get { return _betaSellmeijer; }
            set { _betaSellmeijer = value; }
        }
        
        string _probSellmeijer = "";
        [TestVariable("404e1eb2-9994-4d82-b10f-96182d7ab7a1")]
        public string probSellmeijer
        {
            get { return _probSellmeijer; }
            set { _probSellmeijer = value; }
        }
        
        string _betaCombined = "";
        [TestVariable("ecd11fb5-b634-49aa-83f7-048c430100fc")]
        public string betaCombined
        {
            get { return _betaCombined; }
            set { _betaCombined = value; }
        }
        
        string _approxProbCombined = "";
        [TestVariable("0701eaa4-e25a-430c-b9e8-a8842cb7ff8b")]
        public string approxProbCombined
        {
            get { return _approxProbCombined; }
            set { _approxProbCombined = value; }
        }
        
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ValidateCombinedApproxFailureProbabilityAndBeta()
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
            Delay.SpeedFactor = 0.0;
            
            System.Globalization.CultureInfo currentCulture = CultureInfo.CurrentCulture;

            double minProbNumeric = 1;
            string minProb = "";
            string betaMinProb = "";
            double probUpliftNumeric = NumericValueProbability(probUplift, currentCulture);
            if (probUpliftNumeric<minProbNumeric) {
                minProbNumeric = probUpliftNumeric;
                minProb = probUplift;
                betaMinProb = betaUplift;
            }
            double probHeaveNumeric = NumericValueProbability(probHeave, currentCulture);
            if (probHeaveNumeric < minProbNumeric) {
                minProbNumeric = probHeaveNumeric;
                minProb = probHeave;
                betaMinProb = betaHeave;
            }
            double probSellmeijerNumeric = NumericValueProbability(probSellmeijer, currentCulture);
            if (probSellmeijerNumeric<minProbNumeric) {
                minProbNumeric = probSellmeijerNumeric;
                minProb = probSellmeijer;
                betaMinProb = betaSellmeijer;
            }
            
            double approxProbCombinedNumeric = NumericValueProbability(approxProbCombined, currentCulture);
            
            Report.Info("Validating that approx. failure probability (" + approxProbCombined+") is equal to minimum of Uplift probability(" + probUplift + "), Heave probability (" + probHeave + ") and Sellmeijer probability(" + probSellmeijer + ").");
            Validate.AreEqual(approxProbCombined, minProb);
            Report.Info("Validating that the corresponding reliability index (" + betaCombined + ") is equal to the one of the minimum probability(" + betaMinProb + ").");
            Validate.AreEqual(betaCombined, betaMinProb);
        }
        
        private double NumericValueProbability(string probabilityString, System.Globalization.CultureInfo currentCulture)
        {
            double numerator = Double.Parse(probabilityString.Substring(2, probabilityString.Length-2), currentCulture);
            return 1.0 / numerator;
        }
    }
}
