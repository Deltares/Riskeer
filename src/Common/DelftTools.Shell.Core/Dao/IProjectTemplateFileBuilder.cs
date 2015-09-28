namespace DelftTools.Shell.Core.Dao
{
    public interface IProjectTemplateFileBuilder
    {
        bool ProjectTemplateFileExistsAndCorrect(string projectFileFormatVersion);
            
        void CopyFromTemplateFile(string targetProjectFilePath);
            
        void CopyProjectAsTemplateFile(string sourceProjectFilePath, string sourceProjectFileVersion);

        string ProjectTemplateFilePath { get;  }
    }
}