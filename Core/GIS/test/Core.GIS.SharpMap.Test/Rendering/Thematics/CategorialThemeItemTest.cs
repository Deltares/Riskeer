﻿using System.ComponentModel;
using Core.GIS.SharpMap.Api;
using Core.GIS.SharpMap.Rendering.Thematics;
using Core.GIS.SharpMap.Styles;
using NUnit.Framework;

namespace Core.GIS.SharpMap.Test.Rendering.Thematics
{
    [TestFixture]
    public class CategorialThemeItemTest
    {
        [Test]
        public void Style_PropertyHasChanged_EventBubbledByCategorialThemeItem()
        {
            var counter = 0;
            var categorialThemeItem = new CategorialThemeItem("", new VectorStyle());

            ((INotifyPropertyChanged) categorialThemeItem).PropertyChanged += (sender, e) =>
            {
                if (sender is IStyle)
                {
                    counter++;
                }
            };

            ((VectorStyle) categorialThemeItem.Style).EnableOutline = true;

            Assert.AreEqual(1, counter);
        }
    }
}
