namespace DelftTools.Utils.Aop
{
    using System;

    [Serializable]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class NoChildEventBubblingAttribute : Attribute
    {
    }
}