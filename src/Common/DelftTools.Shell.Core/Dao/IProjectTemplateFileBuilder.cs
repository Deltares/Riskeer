namespace DelftTools.Shell.Core.Dao
{
    public interface IProjectTemplateFileBuilder
    {
        string ProjectTemplateFilePath { get; }
        bool ProjectTemplateFileExistsAndCorrect(string projectFileFormatVersion);

        void CopyFromTemplateFile(string targetProjectFilePath);

        void CopyProjectAsTemplateFile(string sourceProjectFilePath, string sourceProjectFileVersion);
    }
}