// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace Core.Common.Gui
{
    /// <summary>
    /// Use this type in combination with the Editor Attribute on properties in property classes which you want to 
    /// edit with a Ringtoets view.
    /// 
    /// The property grid will display an ellipsis button (...). Clicking on the button will open the default view
    /// for the data object in the central tabbed document area of Ringtoets. The view will remain open until 
    /// closed by the user and is not modal.
    /// 
    /// </summary>
    /// <example>
    /// Usage (for example):
    /// <code>
    /// [Editor(typeof(ViewPropertyEditor), typeof(UITypeEditor))]
    /// public TimeSeries TimeSeries
    /// {
    ///     get { return data.TimeSeries; }
    ///     set { data.TimeSeries = value; } //not called
    /// }
    /// </code>
    /// This would typically open a FunctionView to edit the time series.
    /// </example>
    public class ViewPropertyEditor : UITypeEditor
    {
        public static IViewCommands ViewCommands { get; set; } //static: injected in RingtoetsGui

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            ViewCommands.OpenView(value);
            return value;
        }
    }
}