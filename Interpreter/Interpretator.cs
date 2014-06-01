using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading;
using webBrowser;
namespace diplomWorkTranslator {
    /// <summary>
    /// Реализует интерпретацию программы
    /// Работает с экземплярами классов Parser и Executer
    /// </summary>
    public class Interpretator {
        Parser pars;
        Executer exec;
        public WB wb;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="progPath">путь к файлу с программой</param>
        
        public Interpretator(string progPath, bool isConsole=false) {
            wb = new WB(isConsole);
            pars = new Parser(progPath);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en-US");
            wb.Raise_contin_exec_event+=wb_Raise_contin_exec_event;
        }
        void wb_Raise_contin_exec_event(object sender, EventArgs e) {
            continue_execute();
        }

        /// <summary>
        /// Осуществлет интерпретацию программы
        /// </summary>
        /// <param name="web">Браузер</param>
        /// <param name="input">Окно ввода</param>
        /// <param name="output">Окно вывода</param>
        /// init run
        public void interpretation(System.Windows.Forms.TextBox input, System.Windows.Forms.RichTextBox output) {
            pars.analyze();
            exec = new Executer(pars.getTableId(), input, output, wb);
            exec.execute();
        }
        /// <summary>
        /// Продожает выполнение программы
        /// </summary>
        public void continue_execute() {
            if (exec != null) {
                exec.execute();
            }
        }
        public void print(string str) {
            exec.system_print(str);
        }
        /// <summary>
        /// Реализует выполнение части программы. Полезно в пошаговом режиме.
        /// </summary>
        /// <param name="prog">Скрипт для выполнения</param>
        public void step_execute(string prog) {
            if (prog == " ") {
                return;
            }
            pars.step_analyze(prog);
            exec.make_new_TableId(pars.getTableId());
            exec.execute();
        }
    }
}
