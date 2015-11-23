using System;
using System.Collections.Generic;
using System.Drawing;
using Core.Common.Base;
using Core.Common.Gui;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Tests.Gui
{
    [TestFixture]
    public class GuiExportHandlerTest
    {
        [Test]
        public void GetSupportedExporters()
        {
            var handler = new GuiExportHandler(FileExportersGetter, null);

            CollectionAssert.IsNotEmpty(handler.GetSupportedExportersForItem("string item"));
        }

        [Test]
        public void GetSupportedExportersChecksForCanExport()
        {
            var mocks = new MockRepository();
            var exporter = mocks.StrictMock<IFileExporter>();

            exporter.Expect(e => e.SourceTypes()).Return(new[]
            {
                typeof(string)
            });
            exporter.Expect(e => e.CanExportFor(null)).IgnoreArguments().Return(false);

            mocks.ReplayAll();

            var handler = new GuiExportHandler(o => new[]
            {
                exporter
            }, null);
            CollectionAssert.IsEmpty(handler.GetSupportedExportersForItem("string"));

            mocks.BackToRecordAll();

            exporter.Expect(e => e.SourceTypes()).Return(new[]
            {
                typeof(string)
            });
            exporter.Expect(e => e.CanExportFor(null)).IgnoreArguments().Return(true);

            mocks.ReplayAll();

            CollectionAssert.IsNotEmpty(handler.GetSupportedExportersForItem("string"));

            mocks.VerifyAll();
        }

        private static IEnumerable<IFileExporter> FileExportersGetter(object item)
        {
            yield return new TestFileExporter();
        }

        public class TestFileExporter : IFileExporter
        {
            public string Name { get; private set; }

            public string Category { get; private set; }

            public string FileFilter { get; private set; }

            public Bitmap Icon { get; private set; }

            public bool Export(object item, string path)
            {
                return true;
            }

            public IEnumerable<Type> SourceTypes()
            {
                yield return typeof(string);
            }

            public bool CanExportFor(object item)
            {
                return true;
            }
        }
    }
}