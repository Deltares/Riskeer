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

using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Forms.PresentationObjects;

namespace Ringtoets.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class CommentContextTest
    {
        [Test]
        public void Constuctor_DefaultValues()
        {
            // Setup
            var mocks = new MockRepository();
            var commentMock = mocks.StrictMock<ICommentable>();
            mocks.ReplayAll();

            // Call
            var context = new CommentContext<ICommentable>(commentMock);

            // Assert
            Assert.IsInstanceOf<WrappedObjectContextBase<ICommentable>>(context);
            Assert.AreSame(commentMock, context.WrappedData);

            mocks.VerifyAll();
        }
    }
}