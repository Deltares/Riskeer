using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Controls;
using Core.Common.Controls.Swf.TreeViewControls;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Plugins.ProjectExplorer.Properties;

namespace Core.Plugins.ProjectExplorer.NodePresenters
{
    public class ProjectNodePresenter : TreeViewNodePresenterBase<Project>
    {
        private readonly IContextMenuBuilderProvider contextMenuBuilderProvider;
        private readonly IGuiCommandHandler commandHandler;

        /// <summary>
        /// Creates a new instance of <see cref="ProjectNodePresenter"/>, which uses the 
        /// <paramref name="contextMenuBuilderProvider"/> and <paramref name="commandHandler"/> 
        /// to create and bind its <see cref="ContextMenuStrip"/>.
        /// </summary>
        /// <param name="contextMenuBuilderProvider">The <see cref="IContextMenuBuilderProvider"/> 
        /// to use for  building a <see cref="ContextMenuStrip"/>.</param>
        /// <param name="commandHandler">The <see cref="IGuiCommandHandler"/> to defer the add
        /// item action to.</param>
        public ProjectNodePresenter(IContextMenuBuilderProvider contextMenuBuilderProvider, IGuiCommandHandler commandHandler)
        {
            if (contextMenuBuilderProvider == null)
            {
                throw new ArgumentNullException("contextMenuBuilderProvider", Common.Gui.Properties.Resources.NodePresenter_ContextMenuBuilderProvider_required);
            }
            if (commandHandler == null)
            {
                throw new ArgumentNullException("commandHandler", Common.Gui.Properties.Resources.NodePresenter_CommandHandler_required);
            }
            this.contextMenuBuilderProvider = contextMenuBuilderProvider;
            this.commandHandler = commandHandler;
        }

        public override IEnumerable GetChildNodeObjects(Project project)
        {
            return project.Items;
        }

        public override void UpdateNode(ITreeNode parentNode, ITreeNode node, Project project)
        {
            node.Text = project.Name;
            Image image = Resources.Project;
            node.Image = image;

            node.Tag = project;
        }

        public override ContextMenuStrip GetContextMenu(ITreeNode node, object nodeData)
        {
            var addItem = new StrictContextMenuItem(
                Resources.AddItem,
                null,
                Resources.plus,
                (s, e) => commandHandler.AddNewItem(nodeData));

            return contextMenuBuilderProvider
                .Get(node)
                .AddCustomItem(addItem)
                .AddSeparator()
                .AddImportItem()
                .AddExportItem()
                .AddSeparator()
                .AddExpandAllItem()
                .AddCollapseAllItem()
                .AddSeparator()
                .AddPropertiesItem()
                .Build();
        }

        public override DragOperations CanDrag(Project nodeData)
        {
            return DragOperations.None;
        }


        protected override void OnPropertyChanged(Project item, ITreeNode node, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                node.Text = item.Name;
            }
        }
    }
}