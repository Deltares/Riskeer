/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 03/11/2020
 * Time: 12:41
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

namespace AutomatedSystemTests.Modules.Calculation
{
    /// <summary>
    /// Description of CalculateNameOfCopiedCalculationFromNameOfOriginalCalculation.
    /// </summary>
    [TestModule("59C93BDA-2F66-419C-8C73-506536AC6587", ModuleType.UserCode, 1)]
    public class InsertPreffixInLastStepOfPath : ITestModule
    {
        
        string _originalPathItem = "";
        [TestVariable("49ba6196-e160-44ef-82e0-97affcff7e03")]
        public string originalPathItem
        {
            get { return _originalPathItem; }
            set { _originalPathItem = value; }
        }
        
        string _finalPathItem = "";
        [TestVariable("eb85040d-f099-45ee-8f59-9e8180d4b6cf")]
        public string finalPathItem
        {
            get { return _finalPathItem; }
            set { _finalPathItem = value; }
        }
        
        string _preffixToInsert = "";
        [TestVariable("2fc04d49-bfb1-41ea-99e2-6181637f22cb")]
        public string preffixToInsert
        {
            get { return _preffixToInsert; }
            set { _preffixToInsert = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public InsertPreffixInLastStepOfPath()
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
            
            int indexToInsertPreffix = originalPathItem.LastIndexOf('>');
            
            finalPathItem = originalPathItem.Substring(0, indexToInsertPreffix+1) + preffixToInsert + originalPathItem.Substring(indexToInsertPreffix + 1);
            Report.Log(ReportLevel.Info, "CalculatePathOfCopiedCalculationItemFromNameOfOriginalCalculationItem:::  pathOriginalCalculation: " + originalPathItem + " pathCopyCalculation: " + finalPathItem);
        }
    }
}
