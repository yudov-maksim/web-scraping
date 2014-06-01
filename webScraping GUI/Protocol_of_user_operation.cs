using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using diplomWorkTranslator;
namespace diplom_project {
    /// <summary>
    /// Реализует протоколирование действий пользователя
    /// </summary>
    public class Protocol_of_user_operation {
        Interpretator intrpr;//нужен для работы с элементами dom-дерева. Отдельно узлы не существуют.
        public HtmlElement elemAfterFocuse;
        //HtmlElement newElement;
        CommandHistory history;
        Dictionary<string, string> attr;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="hist_output">Окно вывода</param>
        /// <param name="wb1">Брвузер</param>
        public Protocol_of_user_operation(RichTextBox hist_output, Interpretator intrpr1 = null) {
            intrpr = intrpr1;
            history = new CommandHistory(hist_output);
            attr = new Dictionary<string, string>();
        }
        /// <summary>
        /// Сохраняет действие пользователя
        /// </summary>
        public void save_user_action() {
            switch (elemAfterFocuse.TagName.ToLower()) {
                case "input":
                    //если не checkbox и не radio
                    if (elemAfterFocuse.GetAttribute("type") == "checkbox") {
                        //MessageBox.Show(elemAfterFocuse.GetAttribute("checked"));
                        if (elemAfterFocuse.GetAttribute("checked") == "True") {
                            history.addCommand("set_check_box_elem", "elem", "true");
                        }
                        else {
                            history.addCommand("set_check_box_elem", "elem", "false");
                        }
                    }
                    else if (elemAfterFocuse.GetAttribute("value") != attr["value"]) {
                        history.addCommand("input_elem", "elem", "\"" + elemAfterFocuse.GetAttribute("value") + "\"");
                    }
                    break;
                case "select":
                    history.addCommand("set_options_elem", "elem", "\"" + intrpr.wb.getChoseOption(elemAfterFocuse) + "\"");
                    break;
                default:
                    break;
            }
            //print_command();
        }
        public void saveElem(HtmlElement el) {
            if (el != null) {
                elemAfterFocuse = el;
                attr.Clear();
                attr.Add("value", elemAfterFocuse.GetAttribute("value"));
                //attr.Add("checked", elemAfterFocuse.GetAttribute("checked"));
            }
        }
        /// <summary>
        /// Сохраняет клик по элементу
        /// </summary>
        public void save_user_click() {
            history.addCommand("click_elem", "elem");
        }
        /// <summary>
        /// Сохраняет путь к элементу в DOM-дереве
        /// </summary>
        /// <param name="elemAfterFocuse"></param>
        public void findElem(HtmlElement elemAfterFocuse) {
            string resVarName = "elemCollection";
            string tree_path = intrpr.wb.getTreeToElem(elemAfterFocuse);
            List<HtmlElement> elemList = intrpr.wb.get_tree(tree_path);
            if (elemList.Count == 0) {
                MessageBox.Show("Невозможно определить объект");
                return;
            }
            history.addCommand(resVarName + "=gettree", "\"" + tree_path + "\"");
            if (elemList.Count == 1) {
                //history.addCommand(resVarName + "=gettree", "\"" + tree_path + "\"");
                history.addCommand("elem = getElemWithNum", resVarName, "0");
            }
            else if (elemList.Count != 1) {
                //Если получили несколько элементов, то нужный еще надо найти
                int i = 0;
                foreach (HtmlElement elemInList in elemList) {
                    if (elemInList == elemAfterFocuse) {
                        history.addCommand("elem = getElemWithNum", resVarName, i.ToString());
                        return;
                    }
                    i++;
                }
            }
        }

    }
    enum commandType {
        _click,
        _input,
        _setOptions,
        _setCheckBoxFlag
    }
    /// <summary>
    /// Реализует хранение действийпользователя
    /// </summary>
    class CommandHistory {
        RichTextBox output;
        List<Command> history;
        public CommandHistory(RichTextBox outp) {
            history = new List<Command>();
            output = outp;
        }


        public void addCommand(string cmd, string ar1, string ar2="") {
            history.Add(new Command(cmd, ar1, ar2));
            print_last_command();
        }
        void print_last_command() {
            if (history.Count > 0) {
                output.Text += history[history.Count - 1].getCommand() + '\n';
                output.SelectionStart = output.Text.Length;
                output.ScrollToCaret();
            }
        }
    }


    /// <summary>
    /// Реализует хранение команды с аргументами
    /// </summary>
    class Command{
        string command;
        string arg1;
        string arg2;
        public Command(string cmd, string ar1, string ar2) {
            command = cmd;
            arg1 = ar1;
            arg2 = ar2;
        }
        public string getCommand(){
            if (arg2 != "") {
                return command + "(" + arg1 + ", " + arg2 + ");";
            }
            else {
                return command + "(" + arg1 + ");";
            }
        }
    }
}
