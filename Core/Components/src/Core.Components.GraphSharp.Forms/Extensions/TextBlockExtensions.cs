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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml;

namespace Core.Components.GraphSharp.Forms.Extensions
{
    /// <summary>
    /// Class that defines dependency properties that can be used on a <see cref="TextBlock"/>.
    /// </summary>
    public static class TextBlockExtensions
    {
        public static readonly DependencyProperty FormattedTextProperty = DependencyProperty.RegisterAttached(
            "FormattedText", typeof(string), typeof(TextBlockExtensions), new UIPropertyMetadata(default(string), FormattedTextChanged));

        public static void SetFormattedText(DependencyObject element, string value)
        {
            element.SetValue(FormattedTextProperty, value);
        }

        public static string GetFormattedText(DependencyObject element)
        {
            return (string) element.GetValue(FormattedTextProperty);
        }

        private static void FormattedTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            var textBlock = sender as TextBlock;
            if (textBlock == null)
            {
                return;
            }

            textBlock.Inlines.Clear();

            var value = eventArgs.NewValue as string;
            if (value != null)
            {
                textBlock.Inlines.Add(Convert(value));
            }
        }

        /// <summary>
        /// Converts an XML string to an <see cref="Inline"/> element.
        /// </summary>
        /// <param name="value">The XML string to convert.</param>
        /// <exception cref="XmlException">Thrown when there is a load or parse error in the XML.</exception>
        /// <returns>An <see cref="Inline"/> element.</returns>
        private static Inline Convert(string value)
        {
            var doc = new XmlDocument
            {
                XmlResolver = null
            };
            doc.LoadXml(value);

            var span = new Span();
            ConvertNode(span, doc.ChildNodes[0]);

            return span;
        }

        private static void ConvertNode(Span span, XmlNode xmlNode)
        {
            foreach (XmlNode child in xmlNode)
            {
                if (child is XmlText)
                {
                    span.Inlines.Add(new Run(child.InnerText));
                }
                else if (child is XmlElement)
                {
                    switch (child.Name.ToUpper())
                    {
                        case "BOLD":
                            var boldSpan = new Span();
                            ConvertNode(boldSpan, child);
                            var bold = new Bold(boldSpan);
                            span.Inlines.Add(bold);
                            break;
                        default:
                            span.Inlines.Add(new Run(child.InnerText));
                            break;
                    }
                }
            }
        }
    }
}