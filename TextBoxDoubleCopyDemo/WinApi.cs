using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using static TextBoxDoubleCopyDemo.WinApi.NativeMethods;

namespace TextBoxDoubleCopyDemo;

internal sealed partial class WinApi
{
    internal static partial class NativeMethods
    {
        internal const int WM_CLIPBOARDUPDATE = 0x031D;

        [LibraryImport("user32.dll", EntryPoint = "AddClipboardFormatListener", SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool AddClipboardFormatListener(nint hwnd);
    }

    public event EventHandler? ClipboardChanged;

    public void SubscribeToWndProc(Window windowSource)
    {
        if (PresentationSource.FromVisual(windowSource) is not HwndSource source)
        {
            throw new ArgumentException(
                "Window source MUST be initialized first, such as in the Window's OnSourceInitialized handler.",
                nameof(windowSource));
        }

        source.AddHook(WndProc);
    }

    public static void SubscribeToClipboardChanged(nint windowHandle)
    {
        _ = AddClipboardFormatListener(windowHandle);
    }

    private void OnClipboardChanged()
    {
        ClipboardChanged?.Invoke(this, EventArgs.Empty);
    }

    private nint WndProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled)
    {
        switch (msg)
        {
            case WM_CLIPBOARDUPDATE:
                OnClipboardChanged();
                handled = true;
                break;
        }

        return 0;
    }
}
