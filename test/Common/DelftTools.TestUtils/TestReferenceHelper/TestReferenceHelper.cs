using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using DelftTools.Utils.Aop;
using GeoAPI.Geometries;
using NUnit.Framework;

namespace DelftTools.TestUtils.TestReferenceHelper
{
    // TODO: move (compose if needed) this class into TestHelper - our test utility class.
    // TODO: aybe something to combine with GetAllItemsRecursive
    public static class TestReferenceHelper 
    {
        private class ReferenceEqualsComparer<T> : IEqualityComparer<T>
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
        public static IEnumerable<ReferenceNode> GetReferenceNodesInTree(object graph)
        {
            var root = BuildReferenceTree(graph);

            var comparerNode = new ReferenceEqualsComparer<ReferenceNode>();

            var visitedNodes = new HashSet<ReferenceNode>(comparerNode);

            //var queue = new Queue<ReferenceNode>();
            //queue.Enqueue(root);
            return  GetReferenceNodesInTree(root, visitedNodes);
        }

        private static IEnumerable<ReferenceNode> GetReferenceNodesInTree(ReferenceNode node, HashSet<ReferenceNode> visitedNodes)
        {
            //don't visit the same node twice.
            if (visitedNodes.Contains(node))
            {
                yield break;
            }
            visitedNodes.Add(node);

            yield return node;
            
            foreach (var to in node.Links.Select(l=>l.To))
            {
                foreach (var obj in GetReferenceNodesInTree(to,visitedNodes))
                {
                    yield return obj;
                }
            }
            
        }

        public static ReferenceNode BuildReferenceTree(object graph)
        {
            var queue = new Queue<ReferenceNode>();
            var rootNode = new ReferenceNode(graph);
            var referenceMapping = new Dictionary<object, ReferenceNode>(new ReferenceEqualsComparer<object>());

            queue.Enqueue(rootNode); referenceMapping.Add(graph, rootNode);

            while (queue.Count > 0)
            {
                var activeNode = queue.Dequeue();

                var values = new List<Utils.Tuple<object, string>>();
                
                foreach (var propertyInfo in GetAllProperties(activeNode.Object))
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

                    if (!ShouldInvestigateValueFurther(value)) 
                        continue;

                    values.Add(new Utils.Tuple<object, string>(value, propertyInfo.Name));
                }

                if (activeNode.Object is IEnumerable)
                {
                    var enumerable = activeNode.Object as IEnumerable;

                    var index = 0;
                    foreach (var subobject in enumerable)
                    {
                        if (!ShouldInvestigateValueFurther(subobject))
                            break; //not the kind of list we want to enumerate.. (right?)
                        
                        values.Add(new Utils.Tuple<object, string>(subobject, string.Format("[{0}]", index)));
                        index++;
                    }
                }

                foreach (var valueTuple in values)
                {
                    var value = valueTuple.First;
                    var valueAsCollection = value as ICollection;
                    var valueAsGeometry = value as IGeometry;

                    if (value == null //skip null values
                        || (valueAsCollection != null && valueAsCollection.Count == 0) //skip empty collections
                        || valueAsGeometry != null) //skip geometry && datetime
                    {
                        continue;
                    }

                    ReferenceNode referenceNode;
                    var addPath = false;
                    
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
                        var path = new List<ReferenceLink>(activeNode.Path) {link};
                        referenceNode.Path = path;
                    }
                }
            }
            return rootNode;
        }

        private static bool ShouldInvestigateValueFurther(object value)
        {
            if (value == null || value is Type || value is MemberInfo || value is Assembly || value.GetType().IsValueType)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// UNTESTED!! Use at own risk! Should be used for debugging purposes only.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="graph"></param>
        /// <returns></returns>
        public static List<string> SearchObjectInObjectGraph(object target, object graph)
        {
            var root = BuildReferenceTree(graph);
            return SearchObjectInObjectGraph(target, root);
        }

        public static List<string> SearchObjectInObjectGraph(object target, ReferenceNode root)
        {
            var list = new List<string>();
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

        public static void AssertStringRepresentationOfGraphIsEqual(object network, object clone)
        {
            var objectsInReal = GetReferenceNodesInTree(network).ToList();
            var objectsInClone = GetReferenceNodesInTree(clone).ToList();
            
            for (var i = 0; i < objectsInReal.Count && i < objectsInClone.Count; i++)
            {
                var expected = objectsInReal[i];
                var actual = objectsInClone[i];

                if (!Equals(expected.Object.ToString(), actual.Object.ToString()))
                {
                    Assert.Fail("Unexpected object at index {0}:\n {1}\n{2}\n", i, expected, actual);
                }
            }

            if(objectsInReal.Count != objectsInClone.Count)
            {
                for (var i = 0; i < objectsInReal.Count || i < objectsInClone.Count; i++)
                {
                    var expected = objectsInReal[i].ToString();
                    var actual = objectsInClone[i].ToString();

                    if (expected != actual)
                    {
                        Assert.Fail("Unexpected object:\n {0}\n{1}\n", expected, actual);
                    }
                }

                Assert.Fail("Number of objects in the original object tree ({0}) is not equal to the number of objects in the cloned tree ({1})", objectsInReal.Count, objectsInClone.Count);
            }
        }

        /// <summary>
        /// Searches for event subscriptions on the given object (only its own events). Works with normal events and PostSharp events. 
        /// See FindEventSubscriptionsAdvanced to check entire object graphs. This call is equivalent to FindEventSubscriptionsAdvanced 
        /// with depth 0.
        /// </summary>
        /// <param name="target">The object to check the number of event subscribers for.</param>
        /// <param name="printSubscriptions">optional: print subscriptions to console</param>
        /// <returns></returns>
        public static int FindEventSubscriptions(object target, bool printSubscriptions=false)
        {
            var subscriptions = new List<string>();
            var count = FindEventSubscriptionsAdvanced(target, subscriptions, 0);
            
            if (printSubscriptions)
            {
                Console.WriteLine("===");
                subscriptions.ForEach(Console.WriteLine);
            }

            return count;
        }

        private static readonly HashSet<object> VisitedObjects = new HashSet<object>();
        /// <summary>
        /// NOTE: No guarantees!
        /// </summary>
        /// <param name="target">Object (graph) to find subscriptions on</param>
        /// <param name="subscriptions">A list which will be filled with subscriptions (debug info, can be put in file compare to quickly see difference in subscriptions in subsequent calls)</param>
        /// <param name="depth">The maximum depth for which the object graph is traversed looking for subscriptions (0 only searches the target, 1 searches the target and its properties, etc)</param>
        /// <returns>Number of event subscriptions (which differs from subscriptions.Count, because multiple subscriptions are aggregated on one line there)</returns>
        public static int FindEventSubscriptionsAdvanced(object target, IList<string> subscriptions=null, int depth=6)
        {
            VisitedObjects.Clear();

            if (subscriptions == null)
            {
                subscriptions = new List<string>();
            }
            subscriptions.Clear();

            return FindEventSubscriptionsCore("", target, subscriptions, depth);
        }

        private static int FindEventSubscriptionsCore(string path, object target, IList<string> subscriptions, int depth)
        {
            if (VisitedObjects.Contains(target))
            {
                return 0;
            }
            VisitedObjects.Add(target);

            var allEvents = GetAllEventNames(target);

            int count = 0;

            foreach(var eventName in allEvents)
            {
                var invokeList = GetEventSubscribers(target, eventName, target.GetType());
                var invokeListLength = invokeList.Length;
                count += invokeListLength;
                if (invokeListLength > 0)
                {
                    var numListeners = (invokeListLength > 1 ? "(x" + invokeListLength + ")" : "");

                    var listeners = String.Join("; ",
                                                invokeList.Select(
                                                    i =>
                                                    string.Format("{0} [{1}] - {2}", i.Target,
                                                                  i.Target != null ? i.Target.GetType().Name : "<null>",
                                                                  i.Method.Name)).ToArray());

                    var subscrib = string.Format("{0}.{1} {2}", path, eventName, numListeners);
                    subscrib = string.Format("{0,-80} {1}", subscrib, listeners);
                    subscriptions.Add(subscrib);
                }
            }

            count += FindPostSharpEvents(path, target, subscriptions);

            if (depth == 0)
                return count;

            foreach (var propertyInfo in GetAllProperties(target))
            {
                try
                {
                    if (propertyInfo.GetIndexParameters().Length == 0)
                    {
                        var value = propertyInfo.GetValue(target, null);

                        var values = new List<object>();

                        if (value is IEnumerable)
                        {
                            count += FindEventSubscriptionsCore(path + "." + propertyInfo.Name, value, subscriptions, 0); //depth 0, only search direct events
                            var enumerable = value as IEnumerable;

                            values.AddRange(enumerable.OfType<object>());
                        }
                        else
                        {
                            values.Add(value);
                        }

                        foreach (var item in values)
                        {
                            if (item != null && !(item is IGeometry))
                            {
                                var newPath = path + "." + propertyInfo.Name;
                                if (values.Count > 1)
                                {
                                    newPath += "[]";
                                }

                                count+=FindEventSubscriptionsCore(newPath, item, subscriptions, depth - 1);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            return count;
        }

        private static int FindPostSharpEvents(string path, object target, IList<string> subscriptions)
        {
            var type = target.GetType();
            var count = 0;

            while (type != null)
            {
                var postSharpEventImpl = GetPostSharpEventImpl(target, type);
                count += postSharpEventImpl != null
                             ? FindEventSubscriptionsCore(path + "{PostSharp}", postSharpEventImpl, subscriptions, 0)
                             : 0;
                type = type.BaseType;
            }
            return count;
        }

        private static object GetPostSharpEventImpl(object target, Type typeLevel)
        {
            var field = typeLevel.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                                 .FirstOrDefault(f => f.FieldType == typeof (EntityAttribute));

            return (field != null)
                       ? field.GetValue(target)
                       : null;
        }

        private static IEnumerable<string> GetAllEventNames(object target)
        {
            return target.GetType().GetEvents().Select(e => e.Name).Distinct().ToList();
        }

        //copyright bob powell, http://www.bobpowell.net/eventsubscribers.htm
        private static Delegate[] GetEventSubscribers(object target, string eventName, Type type)
        {
            do
            {
                var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
                
                foreach (FieldInfo fi in fields)
                {
                    if (fi.Name == eventName)
                    {
                        var d = fi.GetValue(target) as Delegate;
                        if (d != null)
                        {
                            return d.GetInvocationList();
                        }

                    }
                }
                type = type.BaseType;
            } 
            while (type != null);

            return new Delegate[] { };
        }

        private static IEnumerable<PropertyInfo> GetAllProperties(object target)
        {
            var objectType = target.GetType();
            var publicProperties = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            var nonpublicProperties = objectType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            return publicProperties.Concat(nonpublicProperties);
        }
    }
}
