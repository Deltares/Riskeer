using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using DelftTools.Utils;
using GeoAPI.Geometries;

namespace DelftTools.TestUtils
{
    public class TestReferenceHelper
    {
        public static IEnumerable<object> GetObjectsInTree(object graph)
        {
            var root = BuildReferenceTree(graph);

            var comparerNode = new ReferenceEqualsComparer<ReferenceNode>();
            var comparerObj = new ReferenceEqualsComparer<object>();

            var visitedNodes = new HashSet<ReferenceNode>(comparerNode);
            //var queue = new Queue<ReferenceNode>();
            //queue.Enqueue(root);
            return GetObjectsInTree(root, visitedNodes);
        }

        private static IEnumerable<object> GetObjectsInTree(ReferenceNode node, HashSet<ReferenceNode> visitedNodes)
        {
            //don't visit the same node twice.
            if (visitedNodes.Contains(node))
            {
                yield break;
            }
            visitedNodes.Add(node);

            yield return node.Object;

            foreach (var to in node.Links.Select(l => l.To))
            {
                foreach (var obj in GetObjectsInTree(to, visitedNodes))
                {
                    yield return obj;
                }
            }

        }

        

        public class ReferenceNode
        {
            public ReferenceNode(object o)
            {
                Object = o;

                try
                {
                    Name = Object.ToString();
                }
                catch (Exception)
                {
                    Name = "<exception>";
                }
                Links = new List<ReferenceLink>();
                Path = new List<ReferenceLink>();
            }

            public object Object { get; private set; }
            public string Name { get; private set; }
            public IList<ReferenceLink> Links { get; private set; }
            public IList<ReferenceLink> Path { get; set; }

            public string ToPathString()
            {
                StringBuilder pathString = new StringBuilder();

                if (Path.Count > 0)
                {
                    pathString.Append(String.Format("{0}", Path[0].From.Name));
                }
                foreach(var link in Path)
                {
                    pathString.Append(String.Format(".{0}", link.Name));
                }

                return pathString.ToString();
            }
        }

        public class ReferenceLink
        {
            public ReferenceLink(ReferenceNode from, ReferenceNode to, string name)
            {
                From = from;
                To = to;
                Name = name;
            }

            public ReferenceNode From { get; set; }
            public ReferenceNode To { get; set; }
            public string Name { get; set; }

            public string ToPathString()
            {
                var lastLink = From.Links.First(link => link.To == To);

                return From.ToPathString() + "." + lastLink.Name;
            }
        }

        public class ReferenceEqualsComparer<T> : IEqualityComparer<T>
        {
            public bool Equals(T x, T y)
            {
                return ReferenceEquals(x, y);
            }

            public int GetHashCode(T obj)
            {
                return RuntimeHelpers.GetHashCode(obj);
            }
        }

        private static ReferenceNode BuildReferenceTree(object graph)
        {
            var queue = new Queue<ReferenceNode>();
            var rootNode = new ReferenceNode(graph);
            var referenceMapping = new Dictionary<object, ReferenceNode>(new ReferenceEqualsComparer<object>());

            queue.Enqueue(rootNode); referenceMapping.Add(graph, rootNode);

            while (queue.Count > 0)
            {
                var activeNode = queue.Dequeue();

                var values = new List<Tuple<object, string>>();

                var objectType = activeNode.Object.GetType();

                var publicProperties = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var nonpublicProperties = objectType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (var propertyInfo in publicProperties.Concat(nonpublicProperties))
                {
                    object value = null;
                    try
                    {
                        if (propertyInfo.GetIndexParameters().Length == 0)
                        {
                            value = propertyInfo.GetValue(activeNode.Object, null);
                        }
                    }
                    catch (Exception)
                    {
                    }

                    if (value == null || value.GetType().IsValueType)
                    {
                        continue;
                    }
                    values.Add(new Tuple<object, string>(value, propertyInfo.Name));
                }

                if (activeNode.Object is IEnumerable)
                {
                    var enumerable = activeNode.Object as IEnumerable;

                    int count = 0;
                    foreach (var subobject in enumerable)
                    {
                        if (subobject == null || subobject.GetType().IsValueType)
                        {
                            break;
                        }
                        values.Add(new Tuple<object, string>(subobject, "[" + count + "]"));
                        count++;
                    }
                }

                foreach (var valueTuple in values)
                {
                    var value = valueTuple.First;
                    var valueAsCollection = value as ICollection;
                    var valueAsGeometry = value as IGeometry;

                    if (value == null //skip null values
                        || (valueAsCollection != null && valueAsCollection.Count == 0) //skip empty collections
                        || valueAsGeometry != null) //skip geometry
                    {
                        continue;
                    }

                    ReferenceNode referenceNode = null;
                    bool addPath = false;
                    
                    if (referenceMapping.ContainsKey(value))
                    {
                        referenceNode = referenceMapping[value];
                    }
                    else
                    {
                        referenceNode = new ReferenceNode(value);
                        referenceMapping.Add(value, referenceNode);
                        queue.Enqueue(referenceNode);
                        addPath = true;
                    }

                    var link = new ReferenceLink(activeNode, referenceNode, valueTuple.Second);
                    activeNode.Links.Add(link);

                    if (addPath)
                    {
                        var path = new List<ReferenceLink>(activeNode.Path);
                        path.Add(link);
                        referenceNode.Path = path;
                    }
                }
            }
            return rootNode;
        }

        /// <summary>
        /// UNTESTED!! Use at own risk! Should be used for debugging purposes only.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="graph"></param>
        /// <returns></returns>
        public static List<string> SearchObjectInObjectGraph(object target, object graph)
        {
            var list = new List<string>();
            
            var root = BuildReferenceTree(graph);
            
            var comparerNode = new ReferenceEqualsComparer<ReferenceNode>();
            var comparerObj = new ReferenceEqualsComparer<object>();

            var visitedNodes = new Dictionary<ReferenceNode, object>(comparerNode);
            var queue = new Queue<ReferenceNode>();
            queue.Enqueue(root);
            
            var uniqueFrom = new List<object>();
            
            while (queue.Count > 0)
            {
                var activeNode = queue.Dequeue();

                foreach (var link in activeNode.Links)
                {
                    if (!visitedNodes.ContainsKey(link.To))
                    {
                        queue.Enqueue(link.To);
                        visitedNodes.Add(link.To, null);
                    }

                    if (ReferenceEquals(link.To.Object, target))
                    {
                        if (!uniqueFrom.Contains(link.From.Object, comparerObj))
                        {
                            uniqueFrom.Add(link.From.Object);
                            list.Add(link.ToPathString());
                        }
                    }
                }
            }
            return list;
        }
    }
}
