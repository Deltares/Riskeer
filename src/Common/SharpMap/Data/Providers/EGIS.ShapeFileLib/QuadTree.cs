#region Copyright and License

/****************************************************************************
**
** Copyright (C) 2008 - 2011 Winston Fletcher.
** All rights reserved.
**
** This file is part of the EGIS.ShapeFileLib class library of Easy GIS .NET.
** 
** Easy GIS .NET is free software: you can redistribute it and/or modify
** it under the terms of the GNU Lesser General Public License version 3 as
** published by the Free Software Foundation and appearing in the file
** lgpl-license.txt included in the packaging of this file.
**
** Easy GIS .NET is distributed in the hope that it will be useful,
** but WITHOUT ANY WARRANTY; without even the implied warranty of
** MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
** GNU General Public License for more details.
**
** You should have received a copy of the GNU General Public License and
** GNU Lesser General Public License along with Easy GIS .NET.
** If not, see <http://www.gnu.org/licenses/>.
**
****************************************************************************/

#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SharpMap.Data.Providers.EGIS.ShapeFileLib
{
    public class QuadTree
    {
        internal int maxLevels;

        private QTNode rootNode;

        internal bool isPointData;

        public QuadTree(RectangleF bounds, int maxLevels, bool isPointData)
        {
            this.isPointData = isPointData;
            this.maxLevels = maxLevels;
            rootNode = new QTNode(bounds, 0, this);
        }

        public QTNode RootNode
        {
            get { return rootNode; }
            set { rootNode = value; }
        }

        public void Insert(int recordIndex, RectangleF bounds)
        {
            if (isPointData)
            {
                rootNode.Insert(recordIndex, bounds);
            }
            else
            {
                rootNode.Insert(recordIndex, ref bounds);
            }
        }

        public IEnumerable<int> GetIndices(ref RectangleF rect, float maximumFeaturesSize)
        {
            if (rootNode.Bounds.IntersectsWith(rect))
            {
                List<int> indices = new List<int>();
                Dictionary<int, int> duplicates = new Dictionary<int, int>();
                rootNode.GetIndices(ref rect, indices, duplicates, maximumFeaturesSize);

                return indices;
            }
            return Enumerable.Empty<int>();
        }
    }

    public class QTNode
    {
        private const int TL = 0;
        private const int TR = 1;
        private const int BL = 2;
        private const int BR = 3;

        //public static int MaxLevels = 7;
        //public static int MinLevels = 2;
        //public static int MaxIndicesPerNode = 16;


        private RectangleF _bounds;
        private QTNode[] children;
        private int _level = 0;
        internal List<int> indexList;
        internal List<RectangleF> boundsList;
        private QuadTree tree;

        public QTNode(RectangleF bounds, int level, QuadTree tree)
        {
            this.tree = tree;
            this._bounds = bounds;
            this._level = level;
            //if (level < MinLevels)
            //{
            //    //create children
            //}
            if (Level == tree.maxLevels)
            {                
                indexList = new List<int>();                
                boundsList = new List<RectangleF>();
            }
        }

        public QTNode[] Children
        {
            get { return children; }
        }

        public RectangleF Bounds
        {
            get
            {
                return _bounds;
            }            
        }

        public int Level
        {
            get
            {
                return _level;
            }
        }


        private void CreateChildren()
        {
            children = new QTNode[4];
            children[TL] = new QTNode(RectangleF.FromLTRB(_bounds.Left, _bounds.Top, 0.5f * (_bounds.Left + _bounds.Right), 0.5f * (_bounds.Top + _bounds.Bottom)), Level + 1, tree);
            children[TR] = new QTNode(RectangleF.FromLTRB(0.5f * (_bounds.Left + _bounds.Right), _bounds.Top, _bounds.Right, 0.5f * (_bounds.Top + _bounds.Bottom)), Level + 1, tree);
            children[BL] = new QTNode(RectangleF.FromLTRB(_bounds.Left, 0.5f * (_bounds.Top + _bounds.Bottom), 0.5f * (_bounds.Left + _bounds.Right), _bounds.Bottom), Level + 1, tree);
            children[BR] = new QTNode(RectangleF.FromLTRB(0.5f * (_bounds.Left + _bounds.Right), 0.5f * (_bounds.Top + _bounds.Bottom), _bounds.Right, _bounds.Bottom), Level + 1, tree);
        }

        public void Insert(int recordIndex, RectangleF bounds)
        {
            if (Level == tree.maxLevels)
            {                
                indexList.Add(recordIndex);
                boundsList.Add(bounds);
            }
            else
            {                
                if(tree.isPointData)
                {
                    if(children == null)
                    {
                        CreateChildren();
                    }

                    if (children[TL].Bounds.Contains(bounds))
                    {
                        children[TL].Insert(recordIndex, bounds);
                    }
                    else if (children[TR].Bounds.Contains(bounds))
                    {
                        children[TR].Insert(recordIndex, bounds);
                    }
                    else if (children[BL].Bounds.Contains(bounds))
                    {
                        children[BL].Insert(recordIndex, bounds);
                    }
                    else if (children[BR].Bounds.Contains(bounds))
                    {
                        children[BR].Insert(recordIndex, bounds);
                    }
                    else
                    {
                        throw new InvalidOperationException("point " + bounds + " is not contained in children bounds");
                    }                    
                }
                else
                {
                    if (children == null)
                    {
                        CreateChildren();
                    }
                    int c = 0;
                    if (children[TL].Bounds.IntersectsWith(bounds))
                    {
                        c++;
                        children[TL].Insert(recordIndex, bounds);
                    }
                    if (children[TR].Bounds.IntersectsWith(bounds))
                    {
                        c++;
                        children[TR].Insert(recordIndex, bounds);
                    }
                    if (children[BL].Bounds.IntersectsWith(bounds))
                    {
                        c++;
                        children[BL].Insert(recordIndex, bounds);
                    }
                    if (children[BR].Bounds.IntersectsWith(bounds))
                    {
                        c++;
                        children[BR].Insert(recordIndex, bounds);
                    }                    
                }
            }
        }

        internal void Insert(int recordIndex, ref RectangleF recBounds)
        {
            if (Level == tree.maxLevels)
            {
                indexList.Add(recordIndex);
                boundsList.Add(recBounds);
            }
            else
            {
                if (tree.isPointData)
                {
                }
                else
                {
                    if (children == null)
                    {
                        CreateChildren();
                    }
                    int c = 0;
                    if (children[TL].Bounds.IntersectsWith(recBounds))
                    {
                        c++;
                        children[TL].Insert(recordIndex, ref recBounds);
                    }
                    if (children[TR].Bounds.IntersectsWith(recBounds))
                    {
                        c++;
                        children[TR].Insert(recordIndex,ref recBounds);
                    }
                    if (children[BL].Bounds.IntersectsWith(recBounds))
                    {
                        c++;
                        children[BL].Insert(recordIndex,ref recBounds);
                    }
                    if (children[BR].Bounds.IntersectsWith(recBounds))
                    {
                        c++;
                        children[BR].Insert(recordIndex,ref recBounds);
                    }
                }
            }
        }

        internal void GetIndices(ref RectangleF rect, List<int> indices, System.Collections.Generic.Dictionary<int, int> foundIndicies, float maximumFeatureSize)
        {
            if (children != null)
            {
                //check each child bounds
                if (children[TL].Bounds.IntersectsWith(rect))
                {
                    children[TL].GetIndices(ref rect, indices, foundIndicies, maximumFeatureSize);
                }
                if (children[TR].Bounds.IntersectsWith(rect))
                {
                    children[TR].GetIndices(ref rect, indices, foundIndicies, maximumFeatureSize);
                }
                if (children[BL].Bounds.IntersectsWith(rect))
                {
                    children[BL].GetIndices(ref rect, indices, foundIndicies, maximumFeatureSize);
                }
                if (children[BR].Bounds.IntersectsWith(rect))
                {
                    children[BR].GetIndices(ref rect, indices, foundIndicies, maximumFeatureSize);
                }                
            }
            else
            {
                if (indexList != null)
                {
                    RectangleF e1, e2 = new RectangleF();

                    //assumes already checked node's Bounds intersect rect
                    //add the node'x indices, checking if it has already been added
                    //We need to check for duplicates as a shape may intersect more than 1 node
                    for (int n = indexList.Count - 1; n >= 0; --n)
                    {
                        var i = indexList[n];
                        if (maximumFeatureSize != 0)
                        {
                            if (!tree.isPointData)
                            {
                                var b = boundsList[n];
                                if (b.Width < maximumFeatureSize && b.Height < maximumFeatureSize)
                                {
                                    continue;
                                }
                            }
                            else // points
                            {
                                e1 = boundsList[n];
                                
                                if (n != indexList.Count - 1)
                                {
                                    if (Math.Abs(e1.X - e2.X) < maximumFeatureSize && Math.Abs(e1.Y - e2.Y) < maximumFeatureSize)
                                    {
                                        continue;
                                    }
                                }

                                e2 = e1;
                            }
                        }

                        if (!boundsList[n].IntersectsWith(rect))
                        {
                            continue;
                        }

                        if (!foundIndicies.ContainsKey(i))
                        {
                            indices.Add(i);
                            foundIndicies.Add(i, 0);
                        }
                    }
                }
            }
        }
    }
}
