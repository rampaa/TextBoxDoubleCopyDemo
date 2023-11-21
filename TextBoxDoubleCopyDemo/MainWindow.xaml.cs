using TextBoxDoubleCopyDemo;
using System.Windows;
using System.Windows.Interop;

namespace TextBoxDoubleCopyDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WinApi? _winApi;
        private int _copyCount = 0;
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            _winApi = new WinApi();
            _winApi.SubscribeToWndProc(this);
            nint windowHandle = new WindowInteropHelper(this).Handle;
            WinApi.SubscribeToClipboardChanged(windowHandle);
            _winApi.ClipboardChanged += ClipboardChanged;
        }

        private void ClipboardChanged(object? sender, EventArgs? e)
        {
            bool gotTextFromClipboard = false;
            while (Clipboard.ContainsText() && !gotTextFromClipboard)
            {
                try
                {
                    // Can throw "System.Runtime.InteropServices.COMException (0x800401D0): OpenClipboard Failed (0x800401D0 (CLIPBRD_E_CANT_OPEN))"
                    // Hence the bizzare while loop to make sure we get the text.
                    string text = Clipboard.GetText();
                    gotTextFromClipboard = true;

                    ++_copyCount;
                    MainTextBox.Text += $"\nCopy count: {_copyCount}";
                }
                catch
                {
                }
            }
        }

        private void CopyTextToClipboard(object sender, RoutedEventArgs e)
        {
            string text = MainTextBox.SelectedText;
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            bool retry = true;
            do
            {
                try
                {
                    Clipboard.SetDataObject(text, false);
                    retry = false;
                }
                catch
                {
                }
            } while (retry);
        }
    }
}