namespace Outbreak.Client.Gui.Actions
{
    public interface IAction
    {
        // returns true if PerformAction will do anything sane.
        // this should return false for example, if you don't have the equipment
        // or materials ..
        bool CanPerformAction();

        // performs the action - whether it's opening a window saying
        // pick which action you want (eg build -> build barricade)
        // or actually telling the server we want to build a barricade.
        void PerformAction();

        // Returns the name of the image that is associated with this action.
        string GetImage();

        // returns the name of the action
        // eg Build Barricade
        string GetActionName();
    }
}
