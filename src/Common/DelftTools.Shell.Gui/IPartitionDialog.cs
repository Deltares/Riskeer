using DelftTools.Controls;

namespace DelftTools.Shell.Gui
{
    public interface IPartitionDialog: IConfigureDialog
    {
        DelftDialogResult ShowPartitionModal();

        void ConfigurePartition(object model);

        int CoreCount { get; }
    }
}
