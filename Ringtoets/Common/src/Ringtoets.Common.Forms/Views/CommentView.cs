﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Controls.TextEditor;
using Core.Common.Controls.Views;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// This class represents a simple view with a rich text editor, to which data can be added. 
    /// </summary>
    public partial class CommentView : UserControl, IView
    {
        private IComment data;
        private RichTextBoxControl richTextEditor;

        /// <summary>
        /// Creates a new instance of <see cref="CommentView"/>.
        /// </summary>
        public CommentView()
        {
            InitializeComponent();

            InitializeRichTextEditor();
        }

        /// <summary>
        /// Gets and sets the assessment section the <see cref="CommentView"/> belongs to.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; set; }

        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value as IComment;

                if (data != null)
                {
                    richTextEditor.Rtf = data.Comments;
                }
            }
        }

        private void InitializeRichTextEditor()
        {
            richTextEditor = new RichTextBoxControl
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(richTextEditor);

            richTextEditor.TextBoxValueChanged += OnTextBoxValueChanged;
        }

        private void OnTextBoxValueChanged(object sender, EventArgs eventArgs)
        {
            data.Comments = richTextEditor.Rtf;
        }
    }
}