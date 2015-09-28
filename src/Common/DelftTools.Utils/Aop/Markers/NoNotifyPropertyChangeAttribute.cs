using System;
using PostSharp.Extensibility;

namespace DelftTools.Utils.Aop
{
    /// <summary>
    /// Apply this attribute to properties you do not want to be intercepted
    /// by NotifyPropertyChangeAttribute. 
    /// </summary>
    [Serializable, MulticastAttributeUsage(MulticastTargets.Property|MulticastTargets.Field)]
    public class NoNotifyPropertyChangeAttribute: Attribute
    {
    }
}