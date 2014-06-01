using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using diplomWorkTranslator;

namespace webScraping_Console {
    class Program {
        [STAThread]
        static void Main(string[] args) {
            System.Console.WriteLine(args[0]);
            Interpretator intrpr = new Interpretator(args[0], true);
            
            //wb = new WB(fileName.FileName);
            System.Windows.Forms.Application.DoEvents();
            try {
                intrpr.interpretation(null, (System.Windows.Forms.RichTextBox)null);
                System.Windows.Forms.Application.Run();
            }
            catch (Exception exc) {
                System.IO.File.WriteAllText("error.txt", "error: " + exc.Message);
            }
        }
    }
}
