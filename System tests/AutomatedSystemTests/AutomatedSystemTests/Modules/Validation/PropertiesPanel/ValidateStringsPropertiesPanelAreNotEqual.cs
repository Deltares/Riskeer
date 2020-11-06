/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 06/11/2020
 * Time: 10:54
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

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests.Modules.Validation.PropertiesPanel
{
    /// <summary>
    /// Description of ValidateStringsPropertiesPanelAreNotEqual.
    /// </summary>
    [TestModule("070F7C1D-5E44-4F97-B699-43136D3EB165", ModuleType.UserCode, 1)]
    public class ValidateStringsPropertiesPanelAreNotEqual : ITestModule
    {
        
        string _string1PropertiesPanel = "";
        [TestVariable("f045d1da-ec05-4010-abc8-12f45a6c5454")]
        public string string1PropertiesPanel
        {
            get { return _string1PropertiesPanel; }
            set { _string1PropertiesPanel = value; }
        }
        
        string _string2PropertiesPanel = "";
        [TestVariable("8815906b-7b8d-4d77-997d-fdd21318d340")]
        public string string2PropertiesPanel
        {
            get { return _string2PropertiesPanel; }
            set { _string2PropertiesPanel = value; }
        }
        
        string _maximumNumberOfRowsToValidate = "";
        [TestVariable("c38cab0a-bb41-499f-9bd1-ff8267861b86")]
        public string maximumNumberOfRowsToValidate
        {
            get { return _maximumNumberOfRowsToValidate; }
            set { _maximumNumberOfRowsToValidate = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ValidateStringsPropertiesPanelAreNotEqual()
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
            
            var list1 = string1PropertiesPanel.Substring(1,string1PropertiesPanel.Length-2).Split(new[] { "];[" }, StringSplitOptions.None);
            var list2 = string2PropertiesPanel.Substring(1,string2PropertiesPanel.Length-2).Split(new[] { "];[" }, StringSplitOptions.None);
            int maxIndex = Math.Min(list1.Length, list2.Length);
            if (maximumNumberOfRowsToValidate!="") {
                maxIndex = Math.Min(maxIndex, Int32.Parse(maximumNumberOfRowsToValidate));
            }
            bool differencesFound = false;
            List<int> listOfDifferencesFound = new List<int>();
            for (int idx = 0; idx < maxIndex; idx++) {
                var elements1 = list1[idx].Split(';');
                var elements2 = list2[idx].Split(';');
                if (elements1[1]==elements2[1]) {
                    Report.Log(ReportLevel.Info, "Comparing values for row = " + elements1[0] + ", parameter name = " + elements1[1] + ". Values found: " + elements1[2] + " <-> " + elements2[2]);
                    if (elements1[2]!=elements2[2]) {
                        differencesFound = true;
                        listOfDifferencesFound.Add(int.Parse(elements1[0]));
                        }
                    } else {
                        Report.Log(ReportLevel.Info, "Comparing parameter names for row = " + elements1[0] + ". Values found: " + elements1[1] + " <-> " + elements2[1]);
                        differencesFound = true;
                        listOfDifferencesFound.Add(int.Parse(elements1[0]));
                    }
                }
            Validate.AreEqual(true, differencesFound, "Validating that differences have been found. Expected: {0}. Found:{1}.");
            Report.Log(ReportLevel.Success, listOfDifferencesFound.Count + " differences found.");
            foreach (int index in listOfDifferencesFound) {
                Validate.IsFalse(list1[index ]==list2[index ], "Difference found: Row " + list1[index ] + " <-> Row " + list2[index]);
            }
        }
    }
}
