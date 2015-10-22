﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using Wti.Data;
using Wti.IO.Builders;

namespace Wti.IO
{
    /// <summary>
    /// This class is responsible for reading an array of bytes and interpret this as a XML document, which contains information about
    /// the geometry of a <see cref="PipingSoilLayer"/>.
    /// </summary>
    internal class PipingSoilLayer2DReader
    {
        private const string outerLoopElementName = "OuterLoop";
        private const string innerLoopElementName = "InnerLoop";
        private const string endPointElementName = "EndPoint";
        private const string headPointElementName = "HeadPoint";
        private const string xElementName = "X";
        private const string yElementName = "Y";
        private const string zElementName = "Z";

        private readonly XmlTextReader xmlTextReader;

        /// <summary>
        /// Constructs a new <see cref="PipingSoilLayer2DReader"/>, which uses the <paramref name="geometry"/> as the source of the 
        /// geometry for a <see cref="PipingSoilLayer"/>.
        /// </summary>
        /// <param name="geometry">An array of <see cref="byte"/> which contains the information of a <see cref="PipingSoilLayer"/>
        /// in an XML document.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="geometry"/> is null.</exception>
        internal PipingSoilLayer2DReader(byte[] geometry)
        {
            xmlTextReader = new XmlTextReader(new MemoryStream(geometry));
        }

        /// <summary>
        /// Reads the XML document and from this obtains the required information and constructs a <see cref="SoilLayer2D"/> based
        /// on this information.
        /// </summary>
        /// <returns>A new <see cref="SoilLayer2D"/> with information taken from the XML document.</returns>
        /// <exception cref="XmlException">When reading from the XML document of the <see cref="PipingSoilLayer2DReader"/> failed.</exception>
        internal SoilLayer2D Read()
        {
            var pipingSoilLayer = new SoilLayer2D();

            while (xmlTextReader.Read())
            {
                HashSet<Point3D> outerLoop;
                HashSet<Point3D> innerLoop;
                if (TryParseLoop(outerLoopElementName, out outerLoop))
                {
                    pipingSoilLayer.OuterLoop = outerLoop;
                }
                if (TryParseLoop(innerLoopElementName, out innerLoop))
                {
                    pipingSoilLayer.InnerLoops.Add(innerLoop);
                }
            }

            return pipingSoilLayer;
        }

        /// <summary>
        /// Tries to parse the element with the given <paramref name="elementName"/>, which the reader should be currently pointing at, as a loop.
        /// </summary>
        /// <param name="elementName">The name of the element which the reader should be currently pointing at.</param>
        /// <param name="loop">The result of parsing the element as a loop. <c>null</c> if the current element's name does not match <paramref name="elementName"/>.</param>
        /// <returns>True if the reader currently points to an element with name <paramref name="elementName"/>. False otherwise.</returns>
        private bool TryParseLoop(String elementName, out HashSet<Point3D> loop)
        {
            loop = null;

            if (IsElementWithName(elementName))
            {
                loop = new HashSet<Point3D>();

                if (!IsEmptyElement())
                {
                    while (xmlTextReader.Read() && !IsEndElementWithName(elementName))
                    {
                        Point3D parsedPoint;
                        if (TryParsePoint(out parsedPoint))
                        {
                            loop.Add(parsedPoint);
                        }
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Finds out whether the element which the reader is currently pointing at is empty.
        /// </summary>
        /// <returns>True if the element is empty. False otherwise.</returns>
        private bool IsEmptyElement()
        {
            return xmlTextReader.IsEmptyElement;
        }

        /// <summary>
        /// Tries to parse the element which the reader is currently pointing at as a point.
        /// </summary>
        /// <param name="point">The result of parsing the element as a point. <c>null</c> if current element is not a head or end point.</param>
        /// <returns>True if the reader currently points to an element with name <see cref="headPointElementName"/> or <see cref="endPointElementName"/>. False otherwise.</returns>
        private bool TryParsePoint(out Point3D point)
        {
            point = null;

            if (IsElementWithName(headPointElementName) || IsElementWithName(endPointElementName))
            {
                var pointValues = ReadChildValues();
                point = new Point3D
                {
                    X = double.Parse(pointValues[xElementName], CultureInfo.InvariantCulture),
                    Y = double.Parse(pointValues[yElementName], CultureInfo.InvariantCulture),
                    Z = double.Parse(pointValues[zElementName], CultureInfo.InvariantCulture)
                };
                return true;
            }
            return false;
        }

        /// <summary>
        /// Reads the name and values for the children of the current element and puts them in a name indexed dictionary.
        /// </summary>
        /// <returns>A <see cref="Dictionary{TKey,TValue}"/>. For each entry, key is equal to child element name and value is equal to the value of the child element's text node.</returns>
        private Dictionary<string, string> ReadChildValues()
        {
            string elementName = xmlTextReader.Name;
            var nodeSibblings = new Dictionary<string, string>();

            while (xmlTextReader.Read() && !IsEndElementWithName(elementName))
            {
                if (xmlTextReader.NodeType == XmlNodeType.Element)
                {
                    nodeSibblings[xmlTextReader.Name] = xmlTextReader.ReadString();
                }
            }

            return nodeSibblings;
        }

        /// <summary>
        /// Checks whether the element the reader is currently pointing at is of <see cref="XmlNodeType.Element"/> type and has a name equal to <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name which the element should have.</param>
        /// <returns>True if the current element has type <see cref="XmlNodeType.Element"/> and its name is equal to <paramref name="name"/>.</returns>
        private bool IsElementWithName(string name)
        {
            var isElement = xmlTextReader.NodeType == XmlNodeType.Element;
            var isPoint = xmlTextReader.Name == name;

            return isElement && isPoint;
        }
        /// <summary>
        /// Checks whether the element the reader is currently pointing at is of <see cref="XmlNodeType.EndElement"/> type and has a name equal to <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name which the end element should have.</param>
        /// <returns>True if the current element has type <see cref="XmlNodeType.EndElement"/> and its name is equal to <paramref name="name"/>.</returns>
        private bool IsEndElementWithName(string name)
        {
            var isElement = xmlTextReader.NodeType == XmlNodeType.EndElement;
            var isPoint = xmlTextReader.Name == name;

            return isElement && isPoint;
        }
    }
}