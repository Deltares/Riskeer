/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 09/10/2021
 * Time: 17:37
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

namespace Ranorex_Automation_Helpers.UserCodeCollections
{
    /// <summary>
    /// Creates a Ranorex user code collection. A collection is used to publish user code methods to the user code library.
    /// </summary>
    [UserCodeCollection]
    public static class StringHelpers
    {
        // You can use the "Insert New User Code Method" functionality from the context menu,
        // to add a new method with the attribute [UserCodeMethod].
        
        
        /// <summary>
        /// The method <c>ToInvariantCulture</c> transforms the format of all numbers
        /// in the string from the CurrentCulture to InvariantCulture.
        /// Currently this transformation comprises decimal separator and group separator.
        /// </summary>
        [UserCodeMethod]
        public static string ToInvariantCulture(this string originalString)
        {
            return originalString.ToNewCulture(CultureInfo.CurrentCulture, CultureInfo.InvariantCulture);
        }
        
        /// <summary>
        /// The method <c>ToCurrentCulture</c> transforms the format of all numbers
        /// in the string from the InvariantCulture to CurrentCulture.
        /// Currently this transformation comprises decimal separator and group separator.
        /// </summary>
        [UserCodeMethod]
        public static string ToCurrentCulture(this string originalString)
        {
            return originalString.ToNewCulture(CultureInfo.InvariantCulture, CultureInfo.CurrentCulture);
        }
        
        /// <summary>
        /// The method <c>ToCurrentCultureDecimalSeparator</c> transforms the format of decimal separator for all numbers
        /// in the string from InvariantCulture to CurrentCulture.
        /// </summary>
        [UserCodeMethod]
        public static string ToCurrentCultureDecimalSeparator(this string originalString)
        {
            return originalString.ReplaceNumberSeparator(CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator, 
                                                         CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
        }
        
        /// <summary>
        /// The method <c>ToCurrentCultureGroupSeparator</c> transforms the format of group separator for all numbers
        /// in the string from InvariantCulture to CurrentCulture.
        /// </summary>
        [UserCodeMethod]
        public static string ToCurrentCultureGroupSeparator(this string originalString)
        {
            return originalString.ReplaceNumberSeparator(CultureInfo.InvariantCulture.NumberFormat.NumberGroupSeparator, 
                                                         CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator);
        }
        
        /// <summary>
        /// The method <c>ToInvariantCultureDecimalSeparator</c> transforms the format of decimal separator for all numbers
        /// in the string from CurrentCulture to InvariantCulture.
        /// </summary>
        [UserCodeMethod]
        public static string ToInvariantCultureDecimalSeparator(this string originalString)
        {
            return originalString.ReplaceNumberSeparator(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, 
                                                         CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);
        }
        
        /// <summary>
        /// The method <c>ToCurrentCultureGroupSeparator</c> transforms the format of group separator for all numbers
        /// in the string from CurrentCulture to InvariantCulture.
        /// </summary>
        [UserCodeMethod]
        public static string ToInvariantCultureGroupSeparator(this string originalString)
        {
            return originalString.ReplaceNumberSeparator(CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator,
                                                         CultureInfo.InvariantCulture.NumberFormat.NumberGroupSeparator);
        }
        
        /// <summary>
        /// The method <c>ToNoGroupSeparator</c> removes the group separator for all numbers
        /// in the string from CurrentCulture.
        /// </summary>
        [UserCodeMethod]
        public static string ToNoGroupSeparator(this string originalString)
        {
            string stringToRemove = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
            bool mustBeRun = Regex.IsMatch(originalString, @"(\d+)\" + stringToRemove + @"(\d+)");
            while (mustBeRun)
            {
                originalString = Regex.Replace(originalString, @"(\d+)\" + stringToRemove + @"(\d+)", "$1$2");
                mustBeRun = Regex.IsMatch(originalString, @"(\d+)\" + stringToRemove + @"(\d+)");
            }
            return originalString;
        }
        
        private static string ToNewCulture(this string originalString, CultureInfo currentCulture, CultureInfo newCulture)
        {
            return originalString.
                ReplaceNumberSeparator(currentCulture.NumberFormat.NumberGroupSeparator,
                                      newCulture.NumberFormat.NumberGroupSeparator).
                ReplaceNumberSeparator(currentCulture.NumberFormat.NumberDecimalSeparator,
                                      newCulture.NumberFormat.NumberDecimalSeparator);
        }
        
        private static string ReplaceNumberSeparator(this string originalString, string currentSeparator, string newSeparator)
        {
            return Regex.Replace(originalString, @"(\d+)\" + currentSeparator + @"(\d+)", "$1" + newSeparator + "$2");
        }
    }
}
