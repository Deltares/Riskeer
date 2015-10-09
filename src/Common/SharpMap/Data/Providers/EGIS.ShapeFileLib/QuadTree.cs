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

        internal bool isPointData;

        public QuadTree(RectangleF bounds, int maxLevels, bool isPointData)
        {
            this.isPointData = isPointData;
            this.maxLevels = maxLevels;
            RootNode = new QTNode(bounds, 0, this);
        }

        public QTNode RootNode { get; set; }

        public void Insert(int recordIndex, RectangleF bounds)
        {
            if (isPointData)
            {
                RootNode.Insert(recordIndex, bounds);
            }
            else
            {
                RootNode.Insert(recordIndex, ref bounds);
            }
        }

        public IEnumerable<int> GetIndices(ref RectangleF rect, float maximumFeaturesSize)
        {
            if (RootNode.Bounds.IntersectsWith(rect))
            {
                List<int> indices = new List<int>();
                Dictionary<int, int> duplicates = new Dictionary<int, int>();
                RootNode.GetIndices(ref rect, indices, duplicates, maximumFeaturesSize);

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
        internal List<int> indexList;
        internal List<RectangleF> boundsList;

        //public static int MaxLevels = 7;
        //public static int MinLevels = 2;
        //public static int MaxIndicesPerNode = 16;

        private readonly int _level = 0;
        private readonly QuadTree tree;

        public QTNode(RectangleF bounds, int level, QuadTree tree)
        {
            this.tree = tree;
            Bounds = bounds;
            _level = level;
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

        public QTNode[] Children { get; private set; }

        public RectangleF Bounds { get; private set; }

        public int Level
        {
            get
            {
                return _level;
            }
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
                if (tree.isPointData)
                {
                    if (Children == null)
                    {
                        CreateChildren();
                    }

                    if (Children[TL].Bounds.Contains(bounds))
                    {
                        Children[TL].Insert(recordIndex, bounds);
                    }
                    else if (Children[TR].Bounds.Contains(bounds))
                    {
                        Children[TR].Insert(recordIndex, bounds);
                    }
                    else if (Children[BL].Bounds.Contains(bounds))
                    {
                        Children[BL].Insert(recordIndex, bounds);
                    }
                    else if (Children[BR].Bounds.Contains(bounds))
                    {
                        Children[BR].Insert(recordIndex, bounds);
                    }
                    else
                    {
                        throw new InvalidOperationException("point " + bounds + " is not contained in children bounds");
                    }
                }
                else
                {
                    if (Children == null)
                    {
                        CreateChildren();
                    }
                    int c = 0;
                    if (Children[TL].Bounds.IntersectsWith(bounds))
                    {
                        c++;
                        Children[TL].Insert(recordIndex, bounds);
                    }
                    if (Children[TR].Bounds.IntersectsWith(bounds))
                    {
                        c++;
                        Children[TR].Insert(recordIndex, bounds);
                    }
                    if (Children[BL].Bounds.IntersectsWith(bounds))
                    {
                        c++;
                        Children[BL].Insert(recordIndex, bounds);
                    }
                    if (Children[BR].Bounds.IntersectsWith(bounds))
                    {
                        c++;
                        Children[BR].Insert(recordIndex, bounds);
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
                if (tree.isPointData) {}
                else
                {
                    if (Children == null)
                    {
                        CreateChildren();
                    }
                    int c = 0;
                    if (Children[TL].Bounds.IntersectsWith(recBounds))
                    {
                        c++;
                        Children[TL].Insert(recordIndex, ref recBounds);
                    }
                    if (Children[TR].Bounds.IntersectsWith(recBounds))
                    {
                        c++;
                        Children[TR].Insert(recordIndex, ref recBounds);
                    }
                    if (Children[BL].Bounds.IntersectsWith(recBounds))
                    {
                        c++;
                        Children[BL].Insert(recordIndex, ref recBounds);
                    }
                    if (Children[BR].Bounds.IntersectsWith(recBounds))
                    {
                        c++;
                        Children[BR].Insert(recordIndex, ref recBounds);
                    }
                }
            }
        }

        internal void GetIndices(ref RectangleF rect, List<int> indices, Dictionary<int, int> foundIndicies, float maximumFeatureSize)
        {
            if (Children != null)
            {
                //check each child bounds
                if (Children[TL].Bounds.IntersectsWith(rect))
                {
                    Children[TL].GetIndices(ref rect, indices, foundIndicies, maximumFeatureSize);
                }
                if (Children[TR].Bounds.IntersectsWith(rect))
                {
                    Children[TR].GetIndices(ref rect, indices, foundIndicies, maximumFeatureSize);
                }
                if (Children[BL].Bounds.IntersectsWith(rect))
                {
                    Children[BL].GetIndices(ref rect, indices, foundIndicies, maximumFeatureSize);
                }
                if (Children[BR].Bounds.IntersectsWith(rect))
                {
                    Children[BR].GetIndices(ref rect, indices, foundIndicies, maximumFeatureSize);
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

        private void CreateChildren()
        {
            Children = new QTNode[4];
            Children[TL] = new QTNode(RectangleF.FromLTRB(Bounds.Left, Bounds.Top, 0.5f*(Bounds.Left + Bounds.Right), 0.5f*(Bounds.Top + Bounds.Bottom)), Level + 1, tree);
            Children[TR] = new QTNode(RectangleF.FromLTRB(0.5f*(Bounds.Left + Bounds.Right), Bounds.Top, Bounds.Right, 0.5f*(Bounds.Top + Bounds.Bottom)), Level + 1, tree);
            Children[BL] = new QTNode(RectangleF.FromLTRB(Bounds.Left, 0.5f*(Bounds.Top + Bounds.Bottom), 0.5f*(Bounds.Left + Bounds.Right), Bounds.Bottom), Level + 1, tree);
            Children[BR] = new QTNode(RectangleF.FromLTRB(0.5f*(Bounds.Left + Bounds.Right), 0.5f*(Bounds.Top + Bounds.Bottom), Bounds.Right, Bounds.Bottom), Level + 1, tree);
        }
    }
}