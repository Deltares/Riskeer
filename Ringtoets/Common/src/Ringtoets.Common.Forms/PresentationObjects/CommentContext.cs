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
using Ringtoets.Common.Data;

namespace Ringtoets.Common.Forms.PresentationObjects
{
    /// <summary>
    /// This class is a presentation object for the comment of <see cref="T"/>.
    /// </summary>
    public class CommentContext<T> where T : ICommentable
    {
        /// <summary>
        /// Creates a new instance of <see cref="CommentContext{T}"/>.
        /// </summary>
        /// <param name="commentContainer">The container to wrap.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="commentContainer"/> is <c>null</c>.</exception>
        public CommentContext(T commentContainer)
        {
            if (commentContainer == null)
            {
                throw new ArgumentNullException("commentContainer");
            }
            CommentContainer = commentContainer;
        }

        /// <summary>
        /// Gets the wrapped comment container.
        /// </summary>
        public T CommentContainer { get; private set; }
    }
}
