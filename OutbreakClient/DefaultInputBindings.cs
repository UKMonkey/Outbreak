using Outbreak.Client.KeyBindings;
using Psy.Core.Input;

namespace Outbreak.Client
{
    public static class DefaultInputBindings
    {
        public static void Populate(IInputBinder inputBinder)
        {
            inputBinder.Bind(InputActions.MoveUp.ToString(), Key.Up, Key.W);
            inputBinder.Bind(InputActions.MoveDown.ToString(), Key.Down, Key.S);
            inputBinder.Bind(InputActions.MoveLeft.ToString(), Key.Left, Key.A);
            inputBinder.Bind(InputActions.MoveRight.ToString(), Key.Right, Key.D);
            inputBinder.Bind(InputActions.Walk.ToString(), Key.LeftCtrl);

            inputBinder.BindDown(InputActions.Reload.ToString(), Key.R, Key.Insert);
            inputBinder.Bind(InputActions.Fire.ToString(), Key.MouseLeft);
            inputBinder.Bind(InputActions.Interact.ToString(), Key.Space);

            inputBinder.BindDown(InputActions.ZoomIn.ToString(), Key.MouseWheelUp);
            inputBinder.BindDown(InputActions.Zoomout.ToString(), Key.MouseWheelDown);
            inputBinder.Bind(InputActions.Inventory.ToString(), Key.I);
            inputBinder.Bind(InputActions.DebugView.ToString(), Key.F3);

            inputBinder.Bind(InputActions.Chat.ToString(), Key.T);

            inputBinder.Bind(InputActions.HotAction1.ToString(), Key.D1);
            inputBinder.Bind(InputActions.HotAction2.ToString(), Key.D2);
            inputBinder.Bind(InputActions.HotAction3.ToString(), Key.D3);
            inputBinder.Bind(InputActions.HotAction4.ToString(), Key.D4);
            inputBinder.Bind(InputActions.HotAction5.ToString(), Key.D5);
            inputBinder.Bind(InputActions.HotAction6.ToString(), Key.D6);
            inputBinder.Bind(InputActions.HotAction7.ToString(), Key.D7);
            inputBinder.Bind(InputActions.HotAction8.ToString(), Key.D8);
            inputBinder.Bind(InputActions.HotAction9.ToString(), Key.D9);
        }
    }
}