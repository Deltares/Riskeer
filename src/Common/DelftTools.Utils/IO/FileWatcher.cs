using System;
using System.IO;

namespace DelftTools.Utils.IO
{
    /// <summary>
    /// DataItem that references a file.
    /// 
    /// TODO: why it is called Watcher, it looks more like a service, should be FileReference probably
    /// </summary>
    public class FileWatcher : ICloneable
    {
        private readonly FileSystemWatcher fileWatcher;
        protected string filePath;
        //todo consider making projectlocation static property.
        private static string projectLocation;

        /// <summary>
        /// Default constructor.
        /// </summary>
        
        public FileWatcher()
        {
            fileWatcher = FileUtils.CreateWatcher();
            fileWatcher.Created += OnFileChanged;
            fileWatcher.Deleted += OnFileChanged;
            fileWatcher.Changed += OnFileChanged;
        }

        /// <summary>
        /// Reference to the full path. 
        /// </summary>
        public virtual string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        /// <summary>
        /// Reference relative to the project location, only use this when loading/saving 
        /// project.
        /// </summary>
        public virtual string RelativePath
        {
            get
            {
                if (filePath!=null && FileUtils.IsSubdirectory(projectLocation, filePath))
                {
                    return FileUtils.GetRelativePath(projectLocation, filePath);
                }
                else
                {
                    return filePath;
                }
            }
            set
            {
                // combine the relativepath and projectlocation to form the full
                //path.
                if (projectLocation != null && FileUtils.PathIsRelative(value))
                {
                    filePath = Path.Combine(projectLocation, value);
                }
                else
                {
                    //use the full path.
                    filePath = value;
                }
            }
        }


        /// <summary>
        /// Projectlocation can be set from the outside indicating that the filePath should be relative. <para/>
        /// Warning: only use this when saving/loading a project.
        /// 
        /// HACK: remove this property! 
        /// </summary>
        public static string ProjectLocation
        {
            set { projectLocation = Path.GetFullPath(value); }
            get { return projectLocation; }
        }

        #region ICloneable Members

        public virtual object Clone()
        {
            FileWatcher item = new FileWatcher();

            item.FilePath = FilePath;

            return item;
        }

        #endregion

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            // TODO:DataItem
            // FireChangedEvent(new DataItemChangeEventArgs(DataItemChangeAction.Value, null));
        }
    }
}