using System.Runtime.InteropServices;
using System.Windows;

namespace CustomCircleUserControl
{
    public partial class App : Application
    {
#if DEBUG
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();
#endif
    }
}
