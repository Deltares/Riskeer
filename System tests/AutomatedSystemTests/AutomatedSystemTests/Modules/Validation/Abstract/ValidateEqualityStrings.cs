/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 25/03/2022
 * Time: 13:08
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

namespace AutomatedSystemTests.Modules.Validation.Abstract
{
    /// <summary>
    /// Description of ValidateEqualityStrings.
    /// </summary>
    [TestModule("BE733B7A-096B-4716-91C9-93590425588B", ModuleType.UserCode, 1)]
    public class ValidateEqualityStrings : ITestModule
    {
        
        string _string1 = "";
        [TestVariable("35edb599-3a82-403c-a68d-d122782e2bd2")]
        public string string1
        {
            get { return _string1; }
            set { _string1 = value; }
        }
        
        
        string _string2 = "";
        [TestVariable("70f1e163-3004-4b25-9a49-22f777e8aae1")]
        public string string2
        {
            get { return _string2; }
            set { _string2 = value; }
        }
        
        
        string _equalityIsExpected = "";
        [TestVariable("e91a369c-1da2-47ba-9ad5-0524ec4a94dc")]
        public string equalityIsExpected
        {
            get { return _equalityIsExpected; }
            set { _equalityIsExpected = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ValidateEqualityStrings()
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
            if (equalityIsExpected=="true") {
                Report.Info("Validating that string1ToCompare = " + string1 + " and string2ToCompare = " + string2+ " are equal.");
                Validate.AreEqual(string1, string2);
            } else 
            {
                Report.Info("Validating that string1ToCompare = " + string1 + " and string2ToCompare = " + string2+ " are unequal.");
                Validate.IsFalse(string1==string2);
            }
        }
    }
}
