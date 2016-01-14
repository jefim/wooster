namespace Wooster.Lib
{
    public interface IPromptService
    {
        string AskForText(string text, string caption);
        void ShowDialog(string text, string caption);
        bool Confirm(string text, string caption);
    }
}