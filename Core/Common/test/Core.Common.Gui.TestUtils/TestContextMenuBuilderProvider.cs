using System;
using Core.Common.Controls;
using Core.Common.Gui.ContextMenu;
using Rhino.Mocks;

namespace Core.Common.Gui.TestUtils
{
    /// <summary>
    /// Class can be used to create a default implementation for a <see cref="IContextMenuBuilderProvider"/>.
    /// </summary>
    public static class TestContextMenuBuilderProvider
    {
        /// <summary>
        /// Creates a new instance of <see cref="IContextMenuBuilderProvider"/>. Makes use of mocks.
        /// </summary>
        /// <param name="mocks">The <see cref="MockRepository"/> to add the mocks to.</param>
        /// <param name="node">The <see cref="ITreeNode"/> to pass to the <see cref="ContextMenuBuilder"/>.</param>
        /// <param name="itemsEnabled">Value indicating whether following option should be available.
        /// <list type="bullet">
        /// <item>export</item>
        /// <item>import</item>
        /// <item>show properties</item>
        /// <item>open view</item>
        /// </list></param>
        /// <returns>A new <see cref="IContextMenuBuilderProvider"/> mock.</returns>
        public static IContextMenuBuilderProvider Create(MockRepository mocks, ITreeNode node, bool itemsEnabled = false)
        {
            if (mocks == null || node == null)
            {
                throw new ArgumentNullException();
            }
            var menuBuilderProviderMock = mocks.StrictMock<IContextMenuBuilderProvider>();
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();

            commandHandlerMock.Expect(ch => ch.CanExportFromGuiSelection()).Return(itemsEnabled).Repeat.Any();
            commandHandlerMock.Expect(ch => ch.CanImportToGuiSelection()).Return(itemsEnabled).Repeat.Any();
            commandHandlerMock.Expect(ch => ch.CanShowPropertiesForGuiSelection()).Return(itemsEnabled).Repeat.Any();
            commandHandlerMock.Expect(ch => ch.CanOpenDefaultViewForSelection()).Return(itemsEnabled).Repeat.Any();

            menuBuilderProviderMock.Expect(mbp => mbp.Get(null)).IgnoreArguments().Return(new ContextMenuBuilder(commandHandlerMock, node));
            return menuBuilderProviderMock;
        }
    }
}