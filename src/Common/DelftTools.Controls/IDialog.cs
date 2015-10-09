namespace DelftTools.Controls
{
    //using own dialog result because don't want to reference windows.forms
    public interface IDialog
    {
        string Title { get; set; }

        DelftDialogResult ShowModal();
        DelftDialogResult ShowModal(object owner);
    }
}