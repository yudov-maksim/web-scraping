using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace diplomWorkTranslator {
    /// <summary>
    /// Реализует работу с таблицами идентификаторов всей программы.
    /// Содержит список таблиц идентификаторов всех функций и список ПОЛИЗов функций.
    /// Выполняет роль обертки над классом TableIDinFunction
    /// </summary>
    public class GlobalTableId {
        List <TableIDinFunction> funcIdTable;//по записи на каждую функцию
        //cписок идентификаторов основного блока (без вызываемых функций) идет поледним
        List <Poliz> prog;
        int curFunc;
        int inet_func_num;
        /// <summary>
        /// Конструктор
        /// </summary>
        public GlobalTableId(){
            funcIdTable = new List<TableIDinFunction>();
            prog = new List<Poliz>();
            curFunc=-1;
            inet_func_num = 0;
        }

        public void clear_cur_poliz() {
            prog[curFunc].clear();
        }

        public int get_curFunc() {
            return curFunc;
        }

        public void set_curFunc(int newVal) {
            curFunc = newVal;
        }
        public void set_inet_func_num(int num) {
            inet_func_num = num;
        }
        public int get_inet_func_num() {
            return inet_func_num;
        }
        public void incCnt(string funcName){//curFunc++
            if (isFuncDeclare(funcName)) {
                throw new SystemException("повторное описание функции " + funcName);
            }
            curFunc++;
            funcIdTable.Add(new TableIDinFunction(funcName));
            prog.Add(new Poliz());
        }
        public int get_num_of_param(string name) {
            int i = getNumOfFunc(name);
            if (i >= 0) {
                return funcIdTable[i].get_num_of_param();
            }
            else {
                return 1;//для write, read
            }
        }
        public void set_num_of_param(string name, int num) {
            int i = getNumOfFunc(name);
            funcIdTable[i].set_num_of_param(num);
        }
        public void set_num_of_param(int num) {
            funcIdTable[curFunc].set_num_of_param(num);
        }
        bool isFuncDeclare(string name) {
            if (getNumOfFunc(name) != -1) {
                return true;
            }
            else {
                return false;
            }
        }
        public int getNumOfFunc(string name){
            for (int i = 0; i < funcIdTable.Count; i++) {
                if (funcIdTable[i].getName() == name) {
                    return i;
                }
            }
            return -1;
        }
        public Type_of_lex getTypeOfFunc(string name) {
            for (int i = 0; i < funcIdTable.Count; i++) {
                if (funcIdTable[i].getName() == name) {
                    return funcIdTable[i].getTypeOffunc();
                }
            }
            return Type_of_lex._undefined;
        }
        //------------------------------
        //обертка для TableIDinFunction
        //public void initialize_all_vars() {
        //    funcIdTable[curFunc].initialize_all_vars(); 
        //}
        public void putTypeOfFunc(Type_of_lex type) {
            funcIdTable[curFunc].putTypeOfFunc(type);
        }
        
        public void addId(Lexem lex) {
            funcIdTable[curFunc].addId(lex);
        }
        public void addId(LexID lex) {
            funcIdTable[curFunc].addId(lex);
        }
        public bool checkId(Lexem lex) {
            if (funcIdTable[curFunc].isDeclare(lex)) {
                int num = funcIdTable[curFunc].getNumOfLex(lex.getName());
                Type_of_lex type = funcIdTable[curFunc].getTypeOfLex(lex.getName());

                funcIdTable[curFunc].push(type);
                return true;
            }
            else if (isFuncDeclare(lex.getName())){
                Type_of_lex type = getTypeOfFunc(lex.getName());
                funcIdTable[curFunc].push(type);
                return true;
            }
            else {
                return false;
            }
        }
        public bool check_assign(Lexem lex) {
            Type_of_lex t1, op, t2;
            t2 = funcIdTable[curFunc].pop();
            op = funcIdTable[curFunc].pop();
            t1 = funcIdTable[curFunc].pop();
            
            if (t1 == Type_of_lex._undefined) {
                funcIdTable[curFunc].putTypeOfLex(lex, t2);
                return true;
            }
            else  if (t1 == t2) {
                return true;
            }

            else {
                return false;
            }
        }
        //++id --id
        public bool check_unary_left() {
            Type_of_lex type, op;
            bool fl = false;
            Lexem tmpLex;
            type = funcIdTable[curFunc].pop();
            op = funcIdTable[curFunc].pop();
            if (type == Type_of_lex._htmlelement || type == Type_of_lex._htmlelementcollect) {
                return false;
            }
            if (type == Type_of_lex._undefined) {
                type = Type_of_lex._int;
                fl = true;
            }
            if (type == Type_of_lex._int || type == Type_of_lex._double) {
                funcIdTable[curFunc].push(type);
                switch (op) {
                    case Type_of_lex.LEX_plus:
                        op = Type_of_lex.LEX_UPLUS;
                        return true;
                    case Type_of_lex.LEX_minus:
                        op = Type_of_lex.LEX_UMINUS;
                        tmpLex = poliz_get_last();
                        tmpLex.putName("-" + tmpLex.getName());
                        poliz_del1();
                        poliz_put_lex(tmpLex);
                        return true;
                    default:
                        break;
                }
                tmpLex = poliz_get_last();
                if (fl) {
                    putTypeofLex(getNumOfLex(tmpLex), type);
                }
                poliz_del1();
                poliz_put_lex(new Lexem(Type_of_lex.POLIZ_ADDR, getNumOfLex(tmpLex).ToString()));
                poliz_put_lex(new Lexem(op));
                return true;
            }
            else {
                return false;
            }
        }
        //id++ id-- 
        public bool check_unary_right() {
            Type_of_lex type, op;
            bool fl = false;
            op = funcIdTable[curFunc].pop();
            type = funcIdTable[curFunc].pop();
            if (type == Type_of_lex._htmlelement || type == Type_of_lex._htmlelementcollect) {
                return false;
            }
            if (type == Type_of_lex._undefined) {
                type = Type_of_lex._int;
                fl = true;
            }
            if (type == Type_of_lex._int || type == Type_of_lex._double) {
                funcIdTable[curFunc].push(type);
                switch (op) {
                    case Type_of_lex.LEX_plusPlus:
                        op = Type_of_lex.LEX_PLUSPLUSright;
                        break;
                    case Type_of_lex.LEX_minusMinus:
                        op = Type_of_lex.LEX_MINUSMINUSright;
                        break;
                    default:
                        break;
                }
                Lexem tmpLex = poliz_get_last();
                if (fl) {
                    putTypeofLex(getNumOfLex(tmpLex), type);
                }
                poliz_del1();
                poliz_put_lex(new Lexem(Type_of_lex.POLIZ_ADDR, getNumOfLex(tmpLex).ToString()));
                poliz_put_lex(new Lexem(op));
                return true;
            }
            else {
                return false;
            }
        }
        public bool check_op() {
            Type_of_lex t1, op, t2, type=Type_of_lex._undefined;
            t2 = funcIdTable[curFunc].pop();
            op = funcIdTable[curFunc].pop();
            t1 = funcIdTable[curFunc].pop();
            if (t1 == Type_of_lex._htmlelement || t1 == Type_of_lex._htmlelementcollect ||
                t2 == Type_of_lex._htmlelement || t2 == Type_of_lex._htmlelementcollect) {
                    return false;
            }
            if ((op == Type_of_lex.LEX_plus) || (op == Type_of_lex.LEX_minus) || (op == Type_of_lex.LEX_mul) || (op == Type_of_lex.LEX_slash)) {
                type = t1;
            }
            else if ((op == Type_of_lex.LEX_or) || (op == Type_of_lex.LEX_and) || (op == Type_of_lex.LEX_eq) || (op == Type_of_lex.LEX_neq) ||
                (op == Type_of_lex.LEX_bigger) || (op == Type_of_lex.LEX_biggerEq) || (op == Type_of_lex.LEX_less) || 
                (op == Type_of_lex.LEX_lessEq)) {
                
                type = Type_of_lex._bool;
            }
            //prog.put_lex(new Lexem(t1));
            poliz_put_lex(new Lexem(op));
            //prog.put_lex(new Lexem(t2));
            string nameId;
            if (t1 == t2){
                //if (t1 != Type_of_lex._undefined) {
                    funcIdTable[curFunc].push(type);
                    return true;
                //}
            }
            else if (t1 == Type_of_lex._undefined) {
                funcIdTable[curFunc].push(t2);
                nameId = poliz_get_num_of_id(poliz_get_free() - 3);
                funcIdTable[curFunc].putTypeOfLex(nameId, t2);
                return true;
            }
            else if (t2 == Type_of_lex._undefined) {
                funcIdTable[curFunc].push(t1);
                nameId = poliz_get_num_of_id(poliz_get_free() - 2);
                funcIdTable[curFunc].putTypeOfLex(nameId, t1);
                return true;
            }
            else {
                return false;
            }
        }

        public bool check_not() {
            funcIdTable[curFunc].pop();
            funcIdTable[curFunc].push(Type_of_lex._bool);
            poliz_put_lex(new Lexem(Type_of_lex.LEX_not));
            return true;
        }
        public void push(Lexem lex) {
            funcIdTable[curFunc].push(lex.get_type());
        }
        public Type_of_lex pop() {
            return funcIdTable[curFunc].pop();
        }
        public int getStackSize() {
            return funcIdTable[curFunc].getStackSize();
        }
        public bool isStackTypeLexEmpty() {
            return funcIdTable[curFunc].isStackEmpty();
        }

        public string get_value_of_lex(string name) {
            return funcIdTable[curFunc].getValueOfLex(name);
        }
        public string get_value_of_lex(int num) {
            return funcIdTable[curFunc].getValueOfLex(num);
        }
        public Type_of_lex get_type_of_lex(string name) {
            return funcIdTable[curFunc].getTypeOfLex(name);
        }
        public void putValueOfLex(Lexem lex, string val) {
            funcIdTable[curFunc].putValOfLex(lex, val);
        }
        public void putValueOfLex(int i, string val, bool isHtml=false) {
            funcIdTable[curFunc].putValOfLex(i, val, isHtml);
        }
        public void putHtmlElemValueOfLex(int lex_num, HtmlElement elem) {
            funcIdTable[curFunc].putHtmlElemValueOfLex(lex_num, elem);
        }
        public void putHtmlElemCollectValueOfLex(int lex_num, List<HtmlElement> elem) {
            funcIdTable[curFunc].putHtmlElemCollectValueOfLex(lex_num, elem);
        }
        public void putTypeofLex(int num, Type_of_lex type) {
            funcIdTable[curFunc].putTypeOfLex(num, type);
        }
        
        public Type_of_lex getTypeofLex(int num) {
            return funcIdTable[curFunc].getTypeOfLex(num);
        }
        public int getNumOfLex(Lexem lex) {
            return funcIdTable[curFunc].getNumOfLex(lex.getName());
        }
        public int getNumOfLex(string name) {
            return funcIdTable[curFunc].getNumOfLex(name);
        }
        public HtmlElement get_htmlElem_of_lex(string name) {
            return funcIdTable[curFunc].get_htmlElem_of_lex(name);
        }
        public List<HtmlElement> get_htmlElemCollect_of_lex(string name) {
            return funcIdTable[curFunc].get_htmlElemCollect_of_lex(name);
        }
        public void set_has_event_func() {
            funcIdTable[curFunc].set_has_event();
        }
        public bool get_has_event_func(int num) {
            return funcIdTable[num].get_has_event();
        }
        public bool get_has_event_func(string name){
            int i = getNumOfFunc(name);
            return get_has_event_func(i);
        }
        //---------poliz-------
        public void poliz_put_lex(Lexem lex) {
            prog[curFunc].arr.Add(new Lexem(lex));
        }
        public void poliz_put_lex(Lexem lex, int num) {
            prog[curFunc].arr[num] = lex;
        }
        public int poliz_get_free() {
            return prog[curFunc].arr.Count;
        }
        public void poliz_blank() {
            prog[curFunc].arr.Add(null);
        }
        public string poliz_get_num_of_id(int cnt) {
            return prog[curFunc].arr[cnt].getName();
        }
        public void poliz_delLast2() {
            prog[curFunc].delLast2();
        }
        public void poliz_del1() {
            prog[curFunc].arr.RemoveAt(prog[curFunc].arr.Count - 1);
        }
        public Lexem poliz_get_last() {
            return prog[curFunc].arr[prog[curFunc].arr.Count - 1];
        }
        public Poliz get_poliz(int num) {
            return prog[num];
        }
    }
}
