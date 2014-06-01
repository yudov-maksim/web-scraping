using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diplom_project {
    /// <summary>
    /// Позволяет работать с историей команд в пошаговом режиме.
    /// (Сохранение, показ предыдущей и следующей  команды)
    /// </summary>
    public class Command_history {
        List<string> history;
        int cur_cmd;
        /// <summary>
        /// Конструктор
        /// </summary>
        public Command_history() {
            history = new List<string>();
            cur_cmd = -1;
        }
        /// <summary>
        /// Сохраняет команду
        /// </summary>
        /// <param name="cmd"></param>
        public void add(string cmd) {
            history.Add(cmd);
            cur_cmd = history.Count - 1;
        }
        /// <summary>
        /// Возвращает предыдущую команду
        /// </summary>
        /// <returns></returns>
        public string getPrev() {
            if (history.Count == 0) {
                return "";
            }
            if (cur_cmd > 0) {
                return history[cur_cmd--];
            }
            else {
                return history[0];
            }
        }
        /// <summary>
        /// Возвращает следующую команду
        /// </summary>
        /// <returns></returns>
        public string getNext() {
            if (history.Count == 0) {
                return "";
            }
            if (cur_cmd < history.Count - 1) {
                return history[cur_cmd++];
            }
            else {
                return history[history.Count - 1];
            }
        }
        void set_init() {
            cur_cmd = history.Count - 1;
        }
    }
}
