using DelftTools.Controls;

namespace DelftTools.Shell.Gui
{
    public interface IPartitionDialog : IConfigureDialog
    {
        int CoreCount { get; }
        DelftDialogResult ShowPartitionModal();

        void ConfigurePartition(object model);
    }
}