﻿using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Shell.Core.Workflow;
using DelftTools.Utils.Collections;

using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Extensions;
using Ringtoets.Piping.Forms.Helpers;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Service;

namespace Ringtoets.Piping.Forms.NodePresenters
{
    /// <summary>
    /// This class presents the data on <see cref="PipingFailureMechanism"/> as a node in a 
    /// <see cref="ITreeView"/> and implements the way the user can interact with the node.
    /// </summary>
    public class PipingFailureMechanismNodePresenter : ITreeNodePresenter
    {
        public ITreeView TreeView { get; set; }

        public Type NodeTagType
        {
            get
            {
                return typeof(PipingFailureMechanism);
            }
        }

        /// <summary>
        /// Injection points for a method to cause an <see cref="IActivity"/> to be scheduled for execution.
        /// </summary>
        public Action<IActivity> RunActivityAction { private get; set; }

        public void UpdateNode(ITreeNode parentNode, ITreeNode node, object nodeData)
        {
            node.Text = Resources.PipingFailureMechanismDisplayName;
            node.Image = Resources.PipingIcon;
        }

        public IEnumerable GetChildNodeObjects(object parentNodeData, ITreeNode node)
        {
            PipingFailureMechanism failureMechanism = (PipingFailureMechanism) parentNodeData;
            yield return failureMechanism.SoilProfiles;
            yield return failureMechanism.SurfaceLines;

            foreach (var calculation in failureMechanism.Calculations)
            {
                yield return new PipingCalculationInputs
                {
                    PipingData = calculation,
                    AvailablePipingSurfaceLines = failureMechanism.SurfaceLines,
                    AvailablePipingSoilProfiles = failureMechanism.SoilProfiles
                };
            }
        }

        public bool CanRenameNode(ITreeNode node)
        {
            return false;
        }

        public bool CanRenameNodeTo(ITreeNode node, string newName)
        {
            return false;
        }

        public void OnNodeRenamed(object nodeData, string newName)
        {
            throw new InvalidOperationException(string.Format("Cannot rename tree node of type {0}.", GetType().Name));
        }

        public void OnNodeChecked(ITreeNode node) {}

        public DragOperations CanDrag(object nodeData)
        {
            return DragOperations.None;
        }

        public DragOperations CanDrop(object item, ITreeNode sourceNode, ITreeNode targetNode, DragOperations validOperations)
        {
            return DragOperations.None;
        }

        public bool CanInsert(object item, ITreeNode sourceNode, ITreeNode targetNode)
        {
            return false;
        }

        public void OnDragDrop(object item, object sourceParentNodeData, object targetParentNodeData, DragOperations operation, int position) {}

        public void OnNodeSelected(object nodeData) {}

        public ContextMenuStrip GetContextMenu(ITreeNode sender, object nodeData)
        {
            var rootMenu = new ContextMenuStrip();

            rootMenu.AddMenuItem(Resources.PipingFailureMechanism_Add_piping_calculation,
                                 Resources.PipingFailureMechanism_Add_piping_calculation_Tooltip,
                                 Resources.PipingIcon, (o, args) =>
                                 {
                                     var failureMechanism = (PipingFailureMechanism)nodeData;
                                     var pipingData = new PipingData
                                     {
                                         Name = NamingHelper.GetUniqueName(failureMechanism.Calculations, "Piping", pd => pd.Name)
                                     };
                                     failureMechanism.Calculations.Add(pipingData);
                                     failureMechanism.NotifyObservers();
                                 });
            rootMenu.AddMenuItem(Resources.Calculate,
                                 Resources.PipingFailureMechanism_Calculate_Tooltip,
                                 Resources.PlayAll, (o, args) =>
                                 {
                                     var failureMechanism = (PipingFailureMechanism)nodeData;
                                     foreach (var calc in failureMechanism.Calculations)
                                     {
                                         RunActivityAction(new PipingCalculationActivity(calc));
                                     }
                                 });

            return rootMenu;
        }

        public void OnPropertyChanged(object sender, ITreeNode node, PropertyChangedEventArgs e) {}

        public void OnCollectionChanged(object sender, NotifyCollectionChangingEventArgs e) {}

        public bool CanRemove(object parentNodeData, object nodeData)
        {
            return nodeData is PipingFailureMechanism;
        }

        public bool RemoveNodeData(object parentNodeData, object nodeData)
        {
            if (nodeData is PipingFailureMechanism)
            {
                var wtiProject = (WtiProject)parentNodeData;

                wtiProject.ClearPipingFailureMechanism();
                wtiProject.NotifyObservers();

                return true;
            }
            return false;
        }
    }
}