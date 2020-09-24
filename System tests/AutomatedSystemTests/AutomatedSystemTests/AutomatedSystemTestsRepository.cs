﻿///////////////////////////////////////////////////////////////////////////////
//
// This file was automatically generated by RANOREX.
// DO NOT MODIFY THIS FILE! It is regenerated by the designer.
// All your modifications will be lost!
// http://www.ranorex.com
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Repository;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests
{
#pragma warning disable 0436 //(CS0436) The type 'type' in 'assembly' conflicts with the imported type 'type2' in 'assembly'. Using the type defined in 'assembly'.
    /// <summary>
    /// The class representing the AutomatedSystemTestsRepository element repository.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("Ranorex", global::Ranorex.Core.Constants.CodeGenVersion)]
    [RepositoryFolder("40cb8eed-98dc-4b16-a5ad-7ef93b4f444c")]
    public partial class AutomatedSystemTestsRepository : RepoGenBaseFolder
    {
        static AutomatedSystemTestsRepository instance = new AutomatedSystemTestsRepository();
        AutomatedSystemTestsRepositoryFolders.RiskeerMainWindowAppFolder _riskeermainwindow;
        AutomatedSystemTestsRepositoryFolders.CloseProjectDialogAppFolder _closeprojectdialog;

        /// <summary>
        /// Gets the singleton class instance representing the AutomatedSystemTestsRepository element repository.
        /// </summary>
        [RepositoryFolder("40cb8eed-98dc-4b16-a5ad-7ef93b4f444c")]
        public static AutomatedSystemTestsRepository Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Repository class constructor.
        /// </summary>
        public AutomatedSystemTestsRepository() 
            : base("AutomatedSystemTestsRepository", "/", null, 0, false, "40cb8eed-98dc-4b16-a5ad-7ef93b4f444c", ".\\RepositoryImages\\AutomatedSystemTestsRepository40cb8eed.rximgres")
        {
            _riskeermainwindow = new AutomatedSystemTestsRepositoryFolders.RiskeerMainWindowAppFolder(this);
            _closeprojectdialog = new AutomatedSystemTestsRepositoryFolders.CloseProjectDialogAppFolder(this);
        }

#region Variables

#endregion

        /// <summary>
        /// The Self item info.
        /// </summary>
        [RepositoryItemInfo("40cb8eed-98dc-4b16-a5ad-7ef93b4f444c")]
        public virtual RepoItemInfo SelfInfo
        {
            get
            {
                return _selfInfo;
            }
        }

        /// <summary>
        /// The RiskeerMainWindow folder.
        /// </summary>
        [RepositoryFolder("d918d6a6-a4c6-4c01-a295-0360a81bad96")]
        public virtual AutomatedSystemTestsRepositoryFolders.RiskeerMainWindowAppFolder RiskeerMainWindow
        {
            get { return _riskeermainwindow; }
        }

        /// <summary>
        /// The CloseProjectDialog folder.
        /// </summary>
        [RepositoryFolder("0c304014-01d3-44c4-9578-fb43d28c00b3")]
        public virtual AutomatedSystemTestsRepositoryFolders.CloseProjectDialogAppFolder CloseProjectDialog
        {
            get { return _closeprojectdialog; }
        }
    }

    /// <summary>
    /// Inner folder classes.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("Ranorex", global::Ranorex.Core.Constants.CodeGenVersion)]
    public partial class AutomatedSystemTestsRepositoryFolders
    {
        /// <summary>
        /// The RiskeerMainWindowAppFolder folder.
        /// </summary>
        [RepositoryFolder("d918d6a6-a4c6-4c01-a295-0360a81bad96")]
        public partial class RiskeerMainWindowAppFolder : RepoGenBaseFolder
        {
            AutomatedSystemTestsRepositoryFolders.ProjectExplorerFolder _projectexplorer;

            /// <summary>
            /// Creates a new RiskeerMainWindow  folder.
            /// </summary>
            public RiskeerMainWindowAppFolder(RepoGenBaseFolder parentFolder) :
                    base("RiskeerMainWindow", "/form[@automationid='RiskeerMainWindow']", parentFolder, 30000, null, true, "d918d6a6-a4c6-4c01-a295-0360a81bad96", "")
            {
                _projectexplorer = new AutomatedSystemTestsRepositoryFolders.ProjectExplorerFolder(this);
            }

            /// <summary>
            /// The Self item.
            /// </summary>
            [RepositoryItem("d918d6a6-a4c6-4c01-a295-0360a81bad96")]
            public virtual Ranorex.Form Self
            {
                get
                {
                    return _selfInfo.CreateAdapter<Ranorex.Form>(true);
                }
            }

            /// <summary>
            /// The Self item info.
            /// </summary>
            [RepositoryItemInfo("d918d6a6-a4c6-4c01-a295-0360a81bad96")]
            public virtual RepoItemInfo SelfInfo
            {
                get
                {
                    return _selfInfo;
                }
            }

            /// <summary>
            /// The ProjectExplorer folder.
            /// </summary>
            [RepositoryFolder("f5ed0b8b-7145-426f-982e-b89248da6d53")]
            public virtual AutomatedSystemTestsRepositoryFolders.ProjectExplorerFolder ProjectExplorer
            {
                get { return _projectexplorer; }
            }
        }

        /// <summary>
        /// The ProjectExplorerFolder folder.
        /// </summary>
        [RepositoryFolder("f5ed0b8b-7145-426f-982e-b89248da6d53")]
        public partial class ProjectExplorerFolder : RepoGenBaseFolder
        {
            AutomatedSystemTestsRepositoryFolders.ProjectRootNodeFolder _projectrootnode;

            /// <summary>
            /// Creates a new ProjectExplorer  folder.
            /// </summary>
            public ProjectExplorerFolder(RepoGenBaseFolder parentFolder) :
                    base("ProjectExplorer", ".//container[@controlname='ProjectExplorer']//tree[@controlname='treeView']/tree[@accessiblerole='Outline']", parentFolder, 30000, null, true, "f5ed0b8b-7145-426f-982e-b89248da6d53", "")
            {
                _projectrootnode = new AutomatedSystemTestsRepositoryFolders.ProjectRootNodeFolder(this);
            }

            /// <summary>
            /// The Self item.
            /// </summary>
            [RepositoryItem("f5ed0b8b-7145-426f-982e-b89248da6d53")]
            public virtual Ranorex.Tree Self
            {
                get
                {
                    return _selfInfo.CreateAdapter<Ranorex.Tree>(true);
                }
            }

            /// <summary>
            /// The Self item info.
            /// </summary>
            [RepositoryItemInfo("f5ed0b8b-7145-426f-982e-b89248da6d53")]
            public virtual RepoItemInfo SelfInfo
            {
                get
                {
                    return _selfInfo;
                }
            }

            /// <summary>
            /// The ProjectRootNode folder.
            /// </summary>
            [RepositoryFolder("b466899e-e209-4d83-a46c-0533f333cea5")]
            public virtual AutomatedSystemTestsRepositoryFolders.ProjectRootNodeFolder ProjectRootNode
            {
                get { return _projectrootnode; }
            }
        }

        /// <summary>
        /// The ProjectRootNodeFolder folder.
        /// </summary>
        [RepositoryFolder("b466899e-e209-4d83-a46c-0533f333cea5")]
        public partial class ProjectRootNodeFolder : RepoGenBaseFolder
        {

            /// <summary>
            /// Creates a new ProjectRootNode  folder.
            /// </summary>
            public ProjectRootNodeFolder(RepoGenBaseFolder parentFolder) :
                    base("ProjectRootNode", "treeitem[1]", parentFolder, 30000, null, true, "b466899e-e209-4d83-a46c-0533f333cea5", "")
            {
            }

            /// <summary>
            /// The Self item.
            /// </summary>
            [RepositoryItem("b466899e-e209-4d83-a46c-0533f333cea5")]
            public virtual Ranorex.TreeItem Self
            {
                get
                {
                    return _selfInfo.CreateAdapter<Ranorex.TreeItem>(true);
                }
            }

            /// <summary>
            /// The Self item info.
            /// </summary>
            [RepositoryItemInfo("b466899e-e209-4d83-a46c-0533f333cea5")]
            public virtual RepoItemInfo SelfInfo
            {
                get
                {
                    return _selfInfo;
                }
            }
        }

        /// <summary>
        /// The CloseProjectDialogAppFolder folder.
        /// </summary>
        [RepositoryFolder("0c304014-01d3-44c4-9578-fb43d28c00b3")]
        public partial class CloseProjectDialogAppFolder : RepoGenBaseFolder
        {
            RepoItemInfo _buttonnoInfo;

            /// <summary>
            /// Creates a new CloseProjectDialog  folder.
            /// </summary>
            public CloseProjectDialogAppFolder(RepoGenBaseFolder parentFolder) :
                    base("CloseProjectDialog", "/form[@title='Project afsluiten']", parentFolder, 30000, null, true, "0c304014-01d3-44c4-9578-fb43d28c00b3", "")
            {
                _buttonnoInfo = new RepoItemInfo(this, "ButtonNo", "button[@text='&No']", 30000, null, "2229a024-7306-4b0d-a6ab-e43f6a29c1bd");
            }

            /// <summary>
            /// The Self item.
            /// </summary>
            [RepositoryItem("0c304014-01d3-44c4-9578-fb43d28c00b3")]
            public virtual Ranorex.Form Self
            {
                get
                {
                    return _selfInfo.CreateAdapter<Ranorex.Form>(true);
                }
            }

            /// <summary>
            /// The Self item info.
            /// </summary>
            [RepositoryItemInfo("0c304014-01d3-44c4-9578-fb43d28c00b3")]
            public virtual RepoItemInfo SelfInfo
            {
                get
                {
                    return _selfInfo;
                }
            }

            /// <summary>
            /// The ButtonNo item.
            /// </summary>
            [RepositoryItem("2229a024-7306-4b0d-a6ab-e43f6a29c1bd")]
            public virtual Ranorex.Button ButtonNo
            {
                get
                {
                    return _buttonnoInfo.CreateAdapter<Ranorex.Button>(true);
                }
            }

            /// <summary>
            /// The ButtonNo item info.
            /// </summary>
            [RepositoryItemInfo("2229a024-7306-4b0d-a6ab-e43f6a29c1bd")]
            public virtual RepoItemInfo ButtonNoInfo
            {
                get
                {
                    return _buttonnoInfo;
                }
            }
        }

    }
#pragma warning restore 0436
}
