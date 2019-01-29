// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Drawing;
using Core.Common.Controls.Views;

namespace Core.Common.Gui.Plugin
{
    /// <summary>
    /// Information for creating a view for a particular data object.
    /// </summary>
    public class ViewInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewInfo"/> class.
        /// </summary>
        public ViewInfo()
        {
            CreateInstance = o => (IView) Activator.CreateInstance(ViewType);
        }

        /// <summary>
        /// Gets or sets the data type associated with this view info.
        /// </summary>
        public Type DataType { get; set; }

        /// <summary>
        /// Gets or sets the type of data used for the view.
        /// </summary>
        public Type ViewDataType { get; set; }

        /// <summary>
        /// Gets or sets the type of the view.
        /// </summary>
        public Type ViewType { get; set; }

        /// <summary>
        /// Gets or sets the description of the view.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the method used to determine the name for the view. Function arguments:
        /// <list type="number">
        ///     <item>The view to get a name for.</item>
        ///     <item>The data corresponding to this view info.</item>
        ///     <item>out - The name of the view.</item>
        /// </list>
        /// </summary>
        public Func<IView, object, string> GetViewName { get; set; }

        /// <summary>
        /// Gets or sets the icon of the view.
        /// </summary>
        public Image Image { get; set; }

        /// <summary>
        /// Gets or sets the optional method for checking if this view info object can be 
        /// used for a given data object. Function arguments:
        /// <list type="number">
        ///     <item>Data for the view.</item>
        ///     <item>out - <c>true</c> is this view info can be used for the data, or <c>false</c> otherwise.</item>
        /// </list>
        /// </summary>
        public Func<object, bool> AdditionalDataCheck { get; set; }

        /// <summary>
        /// Gets or sets the optional method used to convert data from the type defined by 
        /// <see cref="DataType"/> to the type defined by <see cref="ViewDataType"/>. Function Arguments:
        /// <list type="number">
        ///     <item>Original data.</item>
        ///     <item>out - The converted data to be used in the view.</item>
        /// </list>
        /// </summary>
        public Func<object, object> GetViewData { get; set; }

        /// <summary>
        /// Gets or sets the optional method that performs additional actions after the
        /// view has been created. Function arguments:
        /// <list type="number">
        ///     <item>The created view instance.</item>
        ///     <item>The data corresponding to this view info.</item>
        /// </list>
        /// </summary>
        public Action<IView, object> AfterCreate { get; set; }

        /// <summary>
        /// Gets or sets the optional method that allows for actions to be performed  to 
        /// see if the view should be closed. Function arguments:
        /// <list type="number">
        ///     <item>View to close.</item>
        ///     <item>Data of the view.</item>
        ///     <item>out - <c>true</c> if the closing action was successful, <c>false</c> otherwise.</item>
        /// </list>
        /// </summary>
        public Func<IView, object, bool> CloseForData { get; set; }

        /// <summary>
        /// Gets or sets the optional method that allows for the construction of the view.
        /// Function arguments:
        /// <list type="number">
        ///     <item>The data corresponding to this view info.</item>
        ///     <item>The view to create.</item>
        /// </list>
        /// </summary>
        /// <remarks>This property needs to be set if no default constructor is available
        /// for the view type.</remarks>
        public Func<object, IView> CreateInstance { get; set; }

        public override string ToString()
        {
            return DataType + " : " + ViewDataType + " : " + ViewType;
        }
    }

    /// <summary>
    /// Information for creating a view for a particular data object.
    /// </summary>
    /// <typeparam name="TData">Data type associated with this view info.</typeparam>
    /// <typeparam name="TViewData">Type of data used for the view.</typeparam>
    /// <typeparam name="TView">Type of the view.</typeparam>
    public class ViewInfo<TData, TViewData, TView> where TView : IView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewInfo{TData, TViewData, TView}"/> class.
        /// </summary>
        public ViewInfo()
        {
            CreateInstance = o => (TView) Activator.CreateInstance(ViewType);
        }

        /// <summary>
        /// Gets the data type associated with this view info.
        /// </summary>
        public Type DataType
        {
            get
            {
                return typeof(TData);
            }
        }

        /// <summary>
        /// Gets the type of data used for the view.
        /// </summary>
        public Type ViewDataType
        {
            get
            {
                return typeof(TViewData);
            }
        }

        /// <summary>
        /// Gets the type of the view.
        /// </summary>
        public Type ViewType
        {
            get
            {
                return typeof(TView);
            }
        }

        /// <summary>
        /// Gets or sets the description of the view.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the method used to determine the name for the view. Function arguments:
        /// <list type="number">
        ///     <item>The view to get a name for.</item>
        ///     <item>The data corresponding to this view info.</item>
        ///     <item>out - The name of the view.</item>
        /// </list>
        /// </summary>
        public Func<TView, TData, string> GetViewName { get; set; }

        /// <summary>
        /// Gets or sets the icon of the view.
        /// </summary>
        public Image Image { get; set; }

        /// <summary>
        /// Gets or sets the optional method for checking if this view info object can be 
        /// used for a given data object. Function arguments:
        /// <list type="number">
        ///     <item>Data for the view.</item>
        ///     <item>out - <c>true</c> is this view info can be used for the data, or <c>false</c> otherwise.</item>
        /// </list>
        /// </summary>
        public Func<TData, bool> AdditionalDataCheck { get; set; }

        /// <summary>
        /// Gets or sets the optional method used to convert data from the type defined by 
        /// <see cref="DataType"/> to the type defined by <see cref="ViewDataType"/>. Function Arguments:
        /// <list type="number">
        ///     <item>Original data.</item>
        ///     <item>out - The converted data to be used in the view.</item>
        /// </list>
        /// </summary>
        public Func<TData, TViewData> GetViewData { get; set; }

        /// <summary>
        /// Gets or sets the optional method that performs additional actions after the
        /// view has been created. Function arguments:
        /// <list type="number">
        ///     <item>The created view instance.</item>
        ///     <item>The data corresponding to this view info.</item>
        /// </list>
        /// </summary>
        public Action<TView, TData> AfterCreate { get; set; }

        /// <summary>
        /// Gets or sets the optional method that allows for actions to be performed  to 
        /// see if the view should be closed. Function arguments:
        /// <list type="number">
        ///     <item>View to close.</item>
        ///     <item>Data of the view.</item>
        ///     <item>out - <c>true</c> if the closing action was successful, <c>false</c> otherwise.</item>
        /// </list>
        /// </summary>
        public Func<TView, object, bool> CloseForData { get; set; }

        /// <summary>
        /// Gets or sets the optional method that allows for the construction of the view.
        /// Function arguments:
        /// <list type="number">
        ///     <item>The data corresponding to this view info.</item>
        ///     <item>The view to create.</item>
        /// </list>
        /// </summary>
        /// <remarks>This property needs to be set if no default constructor is available
        /// for the view type.</remarks>
        public Func<TData, TView> CreateInstance { get; set; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="ViewInfo{TData, TViewData, TView}"/> to <see cref="ViewInfo"/>.
        /// </summary>
        /// <param name="viewInfo">The view information to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator ViewInfo(ViewInfo<TData, TViewData, TView> viewInfo)
        {
            return new ViewInfo
            {
                DataType = viewInfo.DataType,
                ViewDataType = viewInfo.ViewDataType,
                ViewType = viewInfo.ViewType,
                Description = viewInfo.Description,
                Image = viewInfo.Image,
                AdditionalDataCheck = o => viewInfo.AdditionalDataCheck == null || viewInfo.AdditionalDataCheck((TData) o),
                GetViewData = o => viewInfo.GetViewData != null ? viewInfo.GetViewData((TData) o) : o,
                CloseForData = (v, o) => viewInfo.CloseForData != null && viewInfo.CloseForData((TView) v, o),
                AfterCreate = (v, o) => viewInfo.AfterCreate?.Invoke((TView) v, (TData) o),
                GetViewName = (v, o) => viewInfo.GetViewName?.Invoke((TView) v, (TData) o),
                CreateInstance = o => viewInfo.CreateInstance((TData) o)
            };
        }

        public override string ToString()
        {
            return DataType + " : " + ViewDataType + " : " + ViewType;
        }
    }

    /// <summary>
    /// Information for creating a view for a particular data object.
    /// </summary>
    /// <typeparam name="TData">Data type associated with this view info.</typeparam>
    /// <typeparam name="TView">Type of the view.</typeparam>
    public class ViewInfo<TData, TView> : ViewInfo<TData, TData, TView> where TView : IView {}
}