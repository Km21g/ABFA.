using System;
using System.Windows.Forms;

namespace SecureFileAccess
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());  // Start with Login Form
        }
    }
}

