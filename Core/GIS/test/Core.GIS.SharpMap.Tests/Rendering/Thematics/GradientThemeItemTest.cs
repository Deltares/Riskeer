﻿using System.ComponentModel;
using Core.GIS.SharpMap.Api;
using Core.GIS.SharpMap.Rendering.Thematics;
using Core.GIS.SharpMap.Styles;
using NUnit.Framework;

namespace Core.GIS.SharpMap.Tests.Rendering.Thematics
{
    [TestFixture]
    public class GradientThemeItemTest
    {
        [Test]
        public void GradientThemeItemBubblesPropertyChangesOfStyle()
        {
            var counter = 0;
            var gradientThemeItem = new GradientThemeItem(new VectorStyle(), "", "");

            ((INotifyPropertyChanged)gradientThemeItem).PropertyChanged += (sender, e) =>
            {
                if (sender is IStyle)
                {
                    counter++;
                }
            };

            ((VectorStyle) gradientThemeItem.Style).EnableOutline = true;

            Assert.AreEqual(1, counter);
        }
    }
}
