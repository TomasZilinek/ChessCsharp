using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessPB069
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            
            ProgramLogic programLogic = new ProgramLogic();
            MainForm mainForm = new MainForm();

            programLogic.Initialize(mainForm);

            Application.Run(mainForm);
        }
    }
}
