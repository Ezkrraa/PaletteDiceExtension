using Microsoft.CommandPalette.Extensions.Toolkit;
using System;
using System.Runtime.InteropServices;


namespace PaletteDiceExtension;

internal sealed partial class ShowMessageCommand : InvokableCommand
{
    public override string Name => "Show message";
    public override IconInfo Icon => new("\uE8A7");

    public override CommandResult Invoke()
    {
        new ToastStatusMessage("This is a test message").Show();
        return CommandResult.KeepOpen();
    }


    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);
}