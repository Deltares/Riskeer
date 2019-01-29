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

using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Documents;
using Core.Components.GraphSharp.Forms.Extensions;
using NUnit.Framework;

namespace Core.Components.GraphSharp.Forms.Test.Extensions
{
    [TestFixture]
    public class TextBlockExtensionsTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            DependencyPropertyDescriptor dependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(TextBlockExtensions.FormattedTextProperty, typeof(TextBlock));

            // Assert
            Assert.AreEqual("FormattedText", dependencyPropertyDescriptor.DependencyProperty.Name);
            Assert.AreEqual(typeof(TextBlockExtensions), dependencyPropertyDescriptor.DependencyProperty.OwnerType);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void SetFormattedText_ValidXmlWithoutChildNodes_ConvertToInlines()
        {
            // Setup
            const string xmlToConvert = "<text>test</text>";
            var textBlock = new TextBlock();

            // Precondition
            CollectionAssert.IsEmpty(textBlock.Inlines);

            // Call
            TextBlockExtensions.SetFormattedText(textBlock, xmlToConvert);

            // Assert
            Assert.AreEqual(1, textBlock.Inlines.Count);
            var span = (Span) textBlock.Inlines.First();

            Assert.AreEqual(1, span.Inlines.Count);
            var run = (Run) span.Inlines.First();
            Assert.AreEqual("test", run.Text);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void SetFormattedText_ValidXmlWithSupportedChildNode_ConvertToInlines()
        {
            // Setup
            const string xmlToConvert = "<text>test <bold>bold text</bold></text>";
            var textBlock = new TextBlock();

            // Precondition
            CollectionAssert.IsEmpty(textBlock.Inlines);

            // Call
            TextBlockExtensions.SetFormattedText(textBlock, xmlToConvert);

            // Assert
            Assert.AreEqual(1, textBlock.Inlines.Count);
            var span = (Span) textBlock.Inlines.First();

            Inline[] spanInlines = span.Inlines.ToArray();

            Assert.AreEqual(2, spanInlines.Length);
            var run = (Run) spanInlines[0];
            Assert.AreEqual("test ", run.Text);

            var bold = (Bold) spanInlines[1];
            var boldSpan = (Span) bold.Inlines.First();

            Assert.AreEqual("bold text", ((Run) boldSpan.Inlines.First()).Text);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void SetFormattedText_ValidXmlWithUnsupportedChildNode_ConvertToInlines()
        {
            // Setup
            const string xmlToConvert = "<text>test <italic>unsupported node</italic></text>";
            var textBlock = new TextBlock();

            // Precondition
            CollectionAssert.IsEmpty(textBlock.Inlines);

            // Call
            TextBlockExtensions.SetFormattedText(textBlock, xmlToConvert);

            // Assert
            Assert.AreEqual(1, textBlock.Inlines.Count);
            var span = (Span) textBlock.Inlines.First();

            Inline[] spanInlines = span.Inlines.ToArray();

            Assert.AreEqual(2, spanInlines.Length);
            var run = (Run) spanInlines[0];
            Assert.AreEqual("test ", run.Text);

            var unsupportedNodeRun = (Run) spanInlines[1];
            Assert.AreEqual("unsupported node", unsupportedNodeRun.Text);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenTextBlockWithXml_WhenNewTextXmlNull_ThenInlinesCleared()
        {
            // Given
            const string xmlToConvert = "<text>test</text>";
            var textBlock = new TextBlock();

            TextBlockExtensions.SetFormattedText(textBlock, xmlToConvert);

            // Precondition
            Assert.AreEqual(1, textBlock.Inlines.Count);
            var span = (Span) textBlock.Inlines.First();

            Assert.AreEqual(1, span.Inlines.Count);
            var run = (Run) span.Inlines.First();
            Assert.AreEqual("test", run.Text);

            // When
            TextBlockExtensions.SetFormattedText(textBlock, null);

            // Then
            CollectionAssert.IsEmpty(textBlock.Inlines);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GetFormattedText_DefaultValue_ReturnNull()
        {
            // Setup
            var textBlock = new TextBlock();

            // Call
            string formattedText = TextBlockExtensions.GetFormattedText(textBlock);

            // Assert
            Assert.IsNull(formattedText);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GetFormattedText_TextSet_ReturnText()
        {
            // Setup
            const string xmlToConvert = "<text>test <bold>bold text</bold></text>";
            var textBlock = new TextBlock();

            TextBlockExtensions.SetFormattedText(textBlock, xmlToConvert);

            // Call
            string formattedText = TextBlockExtensions.GetFormattedText(textBlock);

            // Assert
            Assert.AreEqual(xmlToConvert, formattedText);
        }
    }
}