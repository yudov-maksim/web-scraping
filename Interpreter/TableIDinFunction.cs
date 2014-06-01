using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace diplomWorkTranslator {
    /// <summary>
    /// Реализует работу с таблицей идентификаторов функции
    /// </summary>
    public class TableIDinFunction {
        //(без вызываемых функций)
        List<LexID> tId; //таблица id функции
        string funcName;
        int num_param;
        Type_of_lex returnType;
        int curId;
        bool has_event;
        Stack<Type_of_lex> st_lex;
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="name">Имя функции</param>
        public TableIDinFunction(string name) {
            st_lex = new Stack<Type_of_lex>();
            tId = new List<LexID>();
            curId = -1;
            has_event = false;
            funcName = name;
            returnType = Type_of_lex._undefined;
        }
        //----------------
        public void initialize_all_vars() {
            for (int i = 0; i < tId.Count; i++) {
                switch (tId[i].getType()) {
                    case Type_of_lex._htmlelement:
                        tId[i].put_html_val((HtmlElement)null);
                        break;
                    case Type_of_lex._htmlelementcollect:
                        tId[i].put_html_val((List<HtmlElement>)null);
                        break;
                    default:
                        tId[i].putValue("");
                        break;
                }
            }
        }
        public void set_has_event() {
            has_event = true;
        }
        public bool get_has_event() {
            return has_event;
        }
        public void set_num_of_param(int num) {
            num_param = num;
        }
        public int get_num_of_param() {
            return num_param;
        }
        public bool isDeclare(Lexem lex) {
            if (getNumOfLex(lex.getName()) != -1) {
                return true;
            }
            else {
                return false;
            }
        }
        public string getValueOfLex(string name) {
            for (int i = 0; i < tId.Count; i++) {
                if (tId[i].getName() == name) {
                    return tId[i].getValue();
                }
            }
            return "";
        }
        public string getValueOfLex(int num) {
            return tId[num].getValue();
        }
        public int getNumOfLex(string name) {
            for (int i = 0; i < tId.Count; i++) {
                if (tId[i].getName() == name) {
                    return i;
                }
            }
            return -1;
        }
        public Type_of_lex getTypeOfLex(string name) {
            for (int i = 0; i < tId.Count; i++) {
                if (tId[i].getName() == name) {
                    return tId[i].getType();
                }
            }
            return Type_of_lex._void;
        }
        public Type_of_lex getTypeOfLex(int num) {
            return tId[num].getType();
        }
        public void putTypeOfLex(Lexem lex, Type_of_lex type) {
            for (int i = 0; i < tId.Count; i++) {
                if (tId[i].getName() == lex.getName()) {
                    tId[i].putType(type);
                }
            }
        }
        public void putTypeOfLex(string name, Type_of_lex type) {
            for (int i = 0; i < tId.Count; i++) {
                if (tId[i].getName() == name) {
                    tId[i].putType(type);
                }
            }
        }
        public void putTypeOfLex(int num, Type_of_lex type) {
            tId[num].putType(type);
        }
        public void putValOfLex(Lexem lex, string val) {
            for (int i = 0; i < tId.Count; i++) {
                if (tId[i].getName() == lex.getName()) {
                    tId[i].putValue(val);
                }
            }
        }
        public void putValOfLex(int i, string val, bool isHtml = false) {
            if (!isHtml) {
                tId[i].putValue(val);
            }
            else {
                int num = getNumOfLex(val);
                Type_of_lex type = getTypeOfLex(num);
                HtmlElement elem;
                List<HtmlElement> elem_collect;
                if (type == Type_of_lex._htmlelement) {
                    elem = tId[num].get_htmlElem_val();
                    tId[i].put_html_val(elem);
                }
                else {
                    elem_collect = tId[num].get_htmlElemCollection_val();
                    tId[i].put_html_val(elem_collect);
                }
            }
        }
        public void putHtmlElemValueOfLex(int lex_num, HtmlElement elem) {
            tId[lex_num].put_html_val(elem);
        }

        public void putHtmlElemCollectValueOfLex(int lex_num, List<HtmlElement> elem) {
            tId[lex_num].put_html_val(elem);
        }
        public System.Windows.Forms.HtmlElement get_htmlElem_of_lex(string name) {
            int num = getNumOfLex(name);
            return tId[num].get_htmlElem_val();
        }
        public List<HtmlElement> get_htmlElemCollect_of_lex(string name) {
            int num = getNumOfLex(name);
            return tId[num].get_htmlElemCollection_val();
        }
        public void addId(Lexem lex) {
            if (isDeclare(lex)) {
                throw new SystemException("повторное описание переменной " + lex.getName() + " в функции " + funcName);
            }
            tId.Add(new LexID(lex));
            curId++;
        }
        public void addId(LexID lex) {
            if (isDeclare(new Lexem(lex.getType(), lex.getName()))) {
                throw new SystemException("повторное описание переменной " + lex.getName() + " в функции " + funcName);
            }
            tId.Add(new LexID(lex));
            curId++;
        }
        public void putTypeOfFunc(Type_of_lex type) {
            returnType = type;
        }
        public Type_of_lex getTypeOffunc() {
            return returnType;
        }
        public string getName() {
            return funcName;
        }
        public void push(Type_of_lex type) {
            st_lex.Push(type);
        }
        public Type_of_lex pop() {
            return st_lex.Pop();
        }
        public int getStackSize() {
            return st_lex.Count;
        }
        public bool isStackEmpty() {
            if (st_lex.Count == 0) {
                return true;
            }
            else {
                return false;
            }
        }
    }
}
