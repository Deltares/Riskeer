/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 18/12/2020
 * Time: 19:28
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
    /// Description of ValidateSafetyFactorSTPH.
    /// </summary>
    [TestModule("644BC5EB-6772-4741-A29D-4CA34AF52F36", ModuleType.UserCode, 1)]
    public class ValidateSafetyFactorSTPH : ITestModule
    {
        
        
        string _betaProbRequirement = "";
        [TestVariable("1fb58f34-12d0-4f72-989e-4e1daf4176b5")]
        public string betaProbRequirement
        {
            get { return _betaProbRequirement; }
            set { _betaProbRequirement = value; }
        }
        
        
        string _betaApproxProb = "";
        [TestVariable("081afe7a-315e-48bb-9c4f-b14c755344df")]
        public string betaApproxProb
        {
            get { return _betaApproxProb; }
            set { _betaApproxProb = value; }
        }
        
        
        string _actualSafetyFactor = "";
        [TestVariable("546b6f94-8b56-4a63-889a-da1526aeebd4")]
        public string actualSafetyFactor
        {
            get { return _actualSafetyFactor; }
            set { _actualSafetyFactor = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ValidateSafetyFactorSTPH()
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
            
            double betaProbRequirementNumeric = Double.Parse(betaProbRequirement, currentCulture);
            double betaApproxProbNumeric = Double.Parse(betaApproxProb, currentCulture);
            double actualSafetyFactorNumeric = Double.Parse(actualSafetyFactor, currentCulture);
            double expectedSafetyFactor = betaApproxProbNumeric / betaProbRequirementNumeric;
            Report.Info("Validating that deviation of actual safety factor (" + actualSafetyFactorNumeric + ") from the expected one (" + betaApproxProbNumeric + "/" + betaProbRequirementNumeric + "=" + expectedSafetyFactor + ") is less than the current precision.");
            double deviation = Math.Abs(expectedSafetyFactor-actualSafetyFactorNumeric);
            Validate.IsTrue(deviation<0.001);
        }
    }
}
