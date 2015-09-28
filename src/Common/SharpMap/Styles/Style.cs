// Copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using SharpMap.Api;

namespace SharpMap.Styles
{
	/// <summary>
	/// Defines a style used for for defining layer styles
	/// </summary>
	[Serializable]
	public abstract class Style : IStyle
	{
		double _minVisible;
		double _maxVisible;
		private bool _Enabled;
	    private long id;

	    /// <summary>
		/// Initializes a style as sets Min=0, Max=double.MaxValue and Visible=true
		/// </summary>
		public Style()
		{
			_minVisible = 0;
			_maxVisible = double.MaxValue;
			_Enabled = true;
		}

	    public Style(IStyle another)
	    {
            _minVisible = another.MinVisible;
            _maxVisible = another.MaxVisible;
            _Enabled = another.Enabled;
	    }

	    public long Id
	    {
	        get { return id; }
	        set { id = value; }
	    }

	    /// <summary>
		/// Gets or sets the minimum zoom value where the style is applied
		/// </summary>
		public virtual double MinVisible
		{
			get { return _minVisible; }
			set { _minVisible = value; }
		}
		/// <summary>
		/// Gets or sets the maximum zoom value where the style is applied
		/// </summary>
		public virtual double MaxVisible
		{
			get { return _maxVisible; }
			set { _maxVisible = value; }
		}

		/// <summary>
		/// Specified whether style is rendered or not
		/// </summary>
		public virtual bool Enabled
		{
			get { return _Enabled; }
			set { _Enabled = value; }
		}

        /// <summary>
        /// Clones the Style
        /// </summary>
        /// <returns>cloned object</returns>
        public abstract object Clone();
	}
}
