﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Riskeer.Common.Data;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// This class represents a simple view with a rich text editor, to which data can be added.
    /// </summary>
    public partial class CommentView : UserControl, IView
    {
        private Comment data;

        /// <summary>
        /// Creates a new instance of <see cref="CommentView"/>.
        /// </summary>
        public CommentView()
        {
            InitializeComponent();

            InitializeRichTextEditor();
        }

        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value as Comment;

                if (data != null)
                {
                    richTextBoxControl.Rtf = data.Body;
                }
            }
        }

        private void InitializeRichTextEditor()
        {
            richTextBoxControl.TextBoxValueChanged += OnTextBoxValueChanged;
        }

        private void OnTextBoxValueChanged(object sender, EventArgs eventArgs)
        {
            if (data != null)
            {
                data.Body = richTextBoxControl.Rtf;
            }
        }
    }
}