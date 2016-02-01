// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Core.Common.Gui.PropertyBag;

using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Forms.UITypeEditors
{
    /// <summary>
    /// This class provides a base implementation of <see cref="UITypeEditor"/> and defines a drop down list 
    /// edit-control used for piping input data.
    /// </summary>
    /// <typeparam name="T">The type of items that populate the list-edit control.</typeparam>
    public class PipingInputContextSelectionEditor<T> : UITypeEditor where T : class
    {
        private IWindowsFormsEditorService editorService;

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object originalValue)
        {
            if (provider != null)
            {
                editorService = (IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService));
            }

            if (editorService != null && context != null)
            {
                // Create editor:
                ListBox listBox = CreateSelectionControl(context);

                // Open editor for user to select an item:
                editorService.DropDownControl(listBox);

                // Return user selected object, or original value if user did not select anything:
                return listBox.SelectedItem ?? originalValue;
            }
            return base.EditValue(context, provider, originalValue);
        }

        /// <summary>
        /// Gets which member to show of <typeparamref name="T"/> in the dropdown editor.
        /// </summary>
        protected string DisplayMember { private get; set; }

        /// <summary>
        /// Gets the available options which populate the dropdown editor.
        /// </summary>
        /// <param name="context">The context on which to base the available options.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of objects of type <typeparamref name="T"/> which contains all the available options.</returns>
        protected virtual IEnumerable<T> GetAvailableOptions(ITypeDescriptorContext context)
        {
            return Enumerable.Empty<T>();
        }

        /// <summary>
        /// Gets the current option which should be selected in the dropdown editor.
        /// </summary>
        /// <param name="context">The context on which to base the current option.</param>
        /// <returns>The object of type <typeparamref name="T"/> which is currently selected.</returns>
        protected virtual T GetCurrentOption(ITypeDescriptorContext context)
        {
            return null;
        }

        /// <summary>
        /// Takes a context and from this, obtains the object which populates the dropdown editor.
        /// </summary>
        /// <param name="context">The context from which the object is obtained.</param>
        /// <returns>The object which' properties populates the dropdown editor.</returns>
        protected static PipingInputContextProperties GetPropertiesObject(ITypeDescriptorContext context)
        {
            return (PipingInputContextProperties)((DynamicPropertyBag)context.Instance).WrappedObject;
        }

        private ListBox CreateSelectionControl(ITypeDescriptorContext context)
        {
            var listBox = new ListBox
            {
                SelectionMode = SelectionMode.One,
                DisplayMember = DisplayMember
            };
            listBox.SelectedValueChanged += (sender, eventArgs) => editorService.CloseDropDown();

            foreach (T surfaceLine in GetAvailableOptions(context))
            {
                int index = listBox.Items.Add(surfaceLine);
                if (ReferenceEquals(GetCurrentOption(context), surfaceLine))
                {
                    listBox.SelectedIndex = index;
                }
            }
            return listBox;
        }
    }
}