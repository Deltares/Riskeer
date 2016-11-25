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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
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

namespace Core.Common.Gui.UITypeEditors
{
    /// <summary>
    /// This class provides a base implementation of <see cref="UITypeEditor"/> and defines a drop down list 
    /// edit-control used for calculation input data.
    /// </summary>
    /// <typeparam name="TInternalPresentationType">The type of items that populate the list-edit control.</typeparam>
    /// <typeparam name="TPropertiesClass">The type which properties populates the dropdown editor.</typeparam>
    public class SelectionEditor<TPropertiesClass, TInternalPresentationType> :
        SelectionEditor<TPropertiesClass, TInternalPresentationType, TInternalPresentationType>
        where TInternalPresentationType : class
        where TPropertiesClass : IObjectProperties
    {
        protected override ListBox CreateSelectionControl(ITypeDescriptorContext context)
        {
            var listBox = new ListBox
            {
                SelectionMode = SelectionMode.One,
                DisplayMember = DisplayMember
            };
            listBox.SelectedValueChanged += (sender, eventArgs) => EditorService.CloseDropDown();

            TInternalPresentationType currentOption = GetCurrentOption(context);
            if (NullItem != null)
            {
                int index = listBox.Items.Add(NullItem);
                if (currentOption == null)
                {
                    listBox.SelectedIndex = index;
                }
            }

            foreach (TInternalPresentationType option in GetAvailableOptions(context))
            {
                int index = listBox.Items.Add(option);
                if (ReferenceEquals(currentOption, option))
                {
                    listBox.SelectedIndex = index;
                }
            }
            return listBox;
        }
    }

    /// <summary>
    /// This class provides a base implementation of <see cref="UITypeEditor"/> and defines a drop down list 
    /// edit-control used for calculation input data.
    /// </summary>
    /// <typeparam name="TInternalPresentationType">The type of items that populate the list-edit control.</typeparam>
    /// <typeparam name="TDomainType">The type of item the list should return once selected.</typeparam>
    /// <typeparam name="TPropertiesClass">The type which properties populates the dropdown editor.</typeparam>
    public class SelectionEditor<TPropertiesClass, TDomainType, TInternalPresentationType> : UITypeEditor
        where TDomainType : class
        where TInternalPresentationType : class
        where TPropertiesClass : IObjectProperties
    {
        protected IWindowsFormsEditorService EditorService;

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object originalValue)
        {
            if (provider != null)
            {
                EditorService = (IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService));
            }

            if (EditorService != null && context != null)
            {
                // Create editor:
                ListBox listBox = CreateSelectionControl(context);

                // Open editor for user to select an item:
                EditorService.DropDownControl(listBox);

                // Return user selected object, or original value if user did not select anything:
                if (listBox.SelectedItem == null)
                {
                    return originalValue;
                }

                if (ReferenceEquals(listBox.SelectedItem, NullItem))
                {
                    return null;
                }

                return ConvertToDomainType(listBox.SelectedItem);
            }
            return base.EditValue(context, provider, originalValue);
        }

        /// <summary>
        /// Sets which member to show of <typeparamref name="TInternalPresentationType"/> in the dropdown editor.
        /// </summary>
        protected string DisplayMember { get; set; }

        /// <summary>
        /// Gets or sets the item to show which represents a null value.
        /// </summary>
        protected TInternalPresentationType NullItem { get; set; }

        /// <summary>
        /// Converts an <see cref="object"/> to a <see cref="TDomainType"/> object.
        /// </summary>
        /// <param name="selectedItem">The item to convert.</param>
        /// <returns>A converted item to <see cref="TDomainType"/>.</returns>
        protected virtual TDomainType ConvertToDomainType(object selectedItem)
        {
            return (TDomainType) selectedItem;
        }

        /// <summary>
        /// Gets the available options which populate the dropdown editor.
        /// </summary>
        /// <param name="context">The context on which to base the available options.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of objects of type <typeparamref name="TInternalPresentationType"/> which contains all the available options.</returns>
        protected virtual IEnumerable<TInternalPresentationType> GetAvailableOptions(ITypeDescriptorContext context)
        {
            return Enumerable.Empty<TInternalPresentationType>();
        }

        /// <summary>
        /// Gets the current option which should be selected in the dropdown editor.
        /// </summary>
        /// <param name="context">The context on which to base the current option.</param>
        /// <returns>The object of type <typeparamref name="TInternalPresentationType"/> which is currently selected.</returns>
        protected virtual TInternalPresentationType GetCurrentOption(ITypeDescriptorContext context)
        {
            return null;
        }

        /// <summary>
        /// Takes a context and from this, obtains the object which populates the dropdown editor.
        /// </summary>
        /// <param name="context">The context from which the object is obtained.</param>
        /// <returns>The object which' properties populates the dropdown editor.</returns>
        protected static TPropertiesClass GetPropertiesObject(ITypeDescriptorContext context)
        {
            return (TPropertiesClass) ((DynamicPropertyBag) context.Instance).WrappedObject;
        }

        protected virtual ListBox CreateSelectionControl(ITypeDescriptorContext context)
        {
            var listBox = new ListBox
            {
                SelectionMode = SelectionMode.One,
                DisplayMember = DisplayMember
            };
            listBox.SelectedValueChanged += (sender, eventArgs) => EditorService.CloseDropDown();

            TInternalPresentationType currentOption = GetCurrentOption(context);
            if (NullItem != null)
            {
                int index = listBox.Items.Add(NullItem);
                if (currentOption == null)
                {
                    listBox.SelectedIndex = index;
                }
            }

            foreach (TInternalPresentationType option in GetAvailableOptions(context))
            {
                int index = listBox.Items.Add(option);
                if (Equals(currentOption, option))
                {
                    listBox.SelectedIndex = index;
                }
            }
            return listBox;
        }
    }
}