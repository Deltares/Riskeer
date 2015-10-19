namespace DelftTools.Controls
{
    //using own dialog result because don't want to reference windows.forms
    public interface IDialog
    {
        DelftDialogResult ShowModal();
    }
}