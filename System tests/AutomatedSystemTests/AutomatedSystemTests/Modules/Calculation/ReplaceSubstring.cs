/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 10/11/2020
 * Time: 09:27
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
    /// Description of ReplaceSubstring.
    /// </summary>
    [TestModule("92030A75-8205-438C-9C04-F777A02A0B87", ModuleType.UserCode, 1)]
    public class ReplaceSubstring : ITestModule
    {
        
        string _originalString = "";
        [TestVariable("666d8948-5ddb-4a96-976c-bc610318d8eb")]
        public string originalString
        {
            get { return _originalString; }
            set { _originalString = value; }
        }
        
        string _substringToBeReplaced = "";
        [TestVariable("be7bb929-358e-4961-8102-ba536ad20ae5")]
        public string substringToBeReplaced
        {
            get { return _substringToBeReplaced; }
            set { _substringToBeReplaced = value; }
        }
        
        string _substringToReplaceWith = "";
        [TestVariable("b1ca51cd-28fb-439f-b68e-18e10d9c5bc8")]
        public string substringToReplaceWith
        {
            get { return _substringToReplaceWith; }
            set { _substringToReplaceWith = value; }
        }
        
        string _finalString = "";
        [TestVariable("de4c6e03-fcc5-4286-99a5-5964c488d96a")]
        public string finalString
        {
            get { return _finalString; }
            set { _finalString = value; }
        }
        
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ReplaceSubstring()
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
            finalString = originalString.Replace(substringToBeReplaced, substringToReplaceWith);
        }
    }
}
