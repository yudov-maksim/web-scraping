using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
namespace diplomWorkTranslator {
    /// <summary>
    /// Реализует преобразование программы в ПОЛИЗ и проверку контекстных условий
    /// </summary>
    public class Parser {
        Scanner scan;
        //Poliz prog;
        Lexem curLex;
        GlobalTableId tableId;
        Lexem bufLexem;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="program">путь к файлу с программой</param>
        public Parser(string program) {
            bufLexem = null;
            scan = new Scanner(program);
            tableId = new GlobalTableId();
            //prog = new Poliz();
        }
        /// <summary>
        /// Возвращает таблицу идентификаторов
        /// </summary>
        /// <returns></returns>
        public GlobalTableId getTableId() {
            return tableId;
        }
         void getBufLexem() {
            curLex = bufLexem;
            bufLexem = null;
        }
         void chngBufANDcurLexem () {
            Lexem hlp = curLex;
            curLex = bufLexem;
            bufLexem = hlp;
            
        }
         void putBufLexem(Lexem ar) {
            bufLexem = ar;
        }
         void getLex() {
            if (bufLexem != null) {//возьмем из буфера предыдущую лексему
                getBufLexem();
                return;
            }
            curLex = scan.get_lex();
        }
//--------------------------
        void checkId(bool add=true) {
            if (!tableId.checkId(curLex)){
                 throw new SystemException(curLex.getName() + " not declare");
            }
            if (add) {
                tableId.poliz_put_lex(curLex);
            }
        }
        void check_op() {
            if (!tableId.check_op()) {
                throw new SystemException(curLex.getName() + " wrong types in operator");
            }
        }
        void check_not() {
            if (!tableId.check_not()) {
                throw new SystemException(curLex.getName() + " wrong type in not");
            }
        }
        void check_assign(Lexem lex) {
            if (!tableId.check_assign(lex)) {
                throw new SystemException(curLex.getName() + " wrong types in assign");
            }
            tableId.poliz_put_lex(new Lexem(Type_of_lex.LEX_assign));
        }
        void check_unary_right_op() {
            if (!tableId.check_unary_right()) {
                throw new SystemException(curLex.getName() + " wrong types right unary operator");
            }
        }
        void check_unary_left_op() {
            if (!tableId.check_unary_left()) {
                throw new SystemException(curLex.getName() + " wrong types left unary operator");
            }
        }
        void push(Lexem lex) {
            tableId.push(lex);
        }
        Type_of_lex pop() {
            return tableId.pop();
        }
        int getStackSize() {
            return tableId.getStackSize();
        }
        void putTypeOfFunc(Type_of_lex type) {
            tableId.putTypeOfFunc(type);
        }
        void checkStackTypeLex() {
            if (!tableId.isStackTypeLexEmpty()) {
                pop();
                if (!tableId.isStackTypeLexEmpty()) {
                    throw new SystemException("Bad Stack type of lex");
                }
            }

        }
        //-----------Добавим встроенные функции------------------
        void initialize_internal_functions() {
            //В принципе, список функций должен браться из dll , вызываться тоже из dll.
            //Пока берем список из xml, а описаны в коде
            string name="", ret_type="";
            int num_param=0, has_event=0;
            int cnt = 0;
            XmlTextReader reader = new XmlTextReader("../../../webBrowser/internal_functions.xml");
            //reader.Read();
            while (reader.Read()) {
                if (reader.NodeType == XmlNodeType.Element) {
                    while (reader.MoveToNextAttribute()) {
                        switch (reader.Name) {
                            case "name":
                                name = reader.Value;
                                break;
                            case "num_param":
                                num_param = Convert.ToInt32(reader.Value);
                                break;
                            case "ret_type":
                                ret_type = reader.Value;
                                break;
                            case "event":
                                has_event = Convert.ToInt32(reader.Value);
                                break;
                            default:
                                break;
                        }
                    }
                    if (name != "") {
                        tableId.incCnt(name);
                        tableId.set_num_of_param(num_param);
                        switch (ret_type) {
                            case "bool":
                                tableId.putTypeOfFunc(Type_of_lex._bool);
                                break;
                            case "string":
                                tableId.putTypeOfFunc(Type_of_lex._string);
                                break;
                            case "htmlelement":
                                tableId.putTypeOfFunc(Type_of_lex._htmlelement);
                                break;
                            case "htmlelementcollect":
                                tableId.putTypeOfFunc(Type_of_lex._htmlelementcollect);
                                break;
                            case "int":
                                tableId.putTypeOfFunc(Type_of_lex._int);
                                break;
                            default:
                                tableId.putTypeOfFunc(Type_of_lex._void);
                                break;
                        }
                        name = "";
                        cnt++;
                        if (has_event == 1) {
                            tableId.set_has_event_func();
                        }
                    }
                }
            }
            tableId.set_inet_func_num(cnt);
        }
        //--------------------------
        public void step_analyze(string prog) {
            scan = new Scanner(prog, true);
            tableId.clear_cur_poliz();
            analyze(true);
        }
        public void analyze(bool need_initialize_inernal_func = false) {
            bufLexem = null;
            if (!need_initialize_inernal_func) {
                initialize_internal_functions();
                getLex();
                S();
            }
            else {
                getLex();
                while (curLex.get_type() != Type_of_lex.LEX_fin) {
                    statement();
                }
            }
            //System.Console.WriteLine("Success!");
            //tableId.poliz_print_all_array();
            //System.Console.WriteLine();
        }

        public void S() {
            while (curLex.get_type() == Type_of_lex.LEX_function) {
                function();
            }
            tableId.incCnt("main_body");
            while (curLex.get_type() != Type_of_lex.LEX_fin) {
                statement();
            }
            
        }
        public void function() {
            function_head();
            block();
        }
        public void function_head() {
            if (curLex.get_type() != Type_of_lex.LEX_function) {
                throw new SystemException(curLex.ToString());
            }
            getLex();
            if (curLex.get_type() != Type_of_lex.LEX_id) {
                throw new SystemException(curLex.ToString());
            }
            //добавить имя функции в таблицу функций
            tableId.incCnt(curLex.getName());
            getLex();
            function_params();           
        }
        public void function_params() {
            if (curLex.get_type() != Type_of_lex.LEX_openRoundBracket) {
                throw new SystemException(curLex.ToString());
            }
            getLex();
            int num = param_names();
            if (curLex.get_type() != Type_of_lex.LEX_closeRoundBracket) {
                throw new SystemException(curLex.ToString());
            }
            tableId.set_num_of_param(num);
            getLex();
        }
        public int param_names() {
            int num_param = 0;
            if (curLex.get_type() == Type_of_lex.LEX_closeRoundBracket) {
                return num_param;
            }
            while (true) {
                if (curLex.get_type() != Type_of_lex.LEX_id) {
                    throw new SystemException(curLex.ToString());
                }
                tableId.addId(curLex);
                num_param++;
                getLex();
                if ((curLex.get_type() != Type_of_lex.LEX_comma) && (curLex.get_type() != Type_of_lex.LEX_closeRoundBracket)) {
                    throw new SystemException(curLex.ToString());
                }
                else if (curLex.get_type() == Type_of_lex.LEX_comma) {
                    getLex();
                }
                else {
                    //getLex();
                    break;
                }
            }
            return num_param;
        }
//------------------
        public void block() {
            if (curLex.get_type() != Type_of_lex.LEX_openFigBracket) {
                throw new SystemException(curLex.ToString());
            }
            getLex();
            while (curLex.get_type() != Type_of_lex.LEX_closeFigBracket) {
                statement();
                if (curLex.get_type() == Type_of_lex.LEX_fin) {
                    throw new SystemException(curLex.ToString());
                }
            }
            getLex();
        }

        public void statement() {
            //tableId.prog.clear();
            switch (curLex.get_type()) {
                case Type_of_lex.LEX_openFigBracket:
                    block();
                    break;
                case Type_of_lex.LEX_var:
                    var();
                    if (curLex.get_type() != Type_of_lex.LEX_semicolon) {
                        throw new SystemException(curLex.ToString());
                    }
                    getLex();
                    break;
                case Type_of_lex.LEX_for:
                    _for();
                    break;
                case Type_of_lex.LEX_while:
                    _while();
                    break;
                case Type_of_lex.LEX_if:
                    _if();
                    break;
                case Type_of_lex.LEX_foreach:
                    _foreach();
                    break;
                case Type_of_lex.LEX_return:
                    _return();
                    putTypeOfFunc(pop());
                    if (curLex.get_type() != Type_of_lex.LEX_semicolon) {
                        throw new SystemException(curLex.ToString());
                    }
                    getLex();
                    break;
               default:
                    expression();
                    checkStackTypeLex();
                    
                    if (curLex.get_type() != Type_of_lex.LEX_semicolon) {
                        throw new SystemException(curLex.ToString());
                    }
                    
                    getLex();                    
                    break;
            }
        }

        public void var() {
            while (curLex.get_type() == Type_of_lex.LEX_var) {
                getLex();
                var_block();       
            }
        }
        public void var_block() {
            while (true) {
                variable_list_item();
                //getLex();
                if ((curLex.get_type() != Type_of_lex.LEX_comma) && (curLex.get_type() != Type_of_lex.LEX_semicolon)) {
                    throw new SystemException(curLex.ToString());
                }
                else if (curLex.get_type() == Type_of_lex.LEX_comma) {
                    getLex();
                }
                else {
                    //getLex();
                    break;
                }
            }

        }
        public void variable_list_item() {
            if (curLex.get_type() != Type_of_lex.LEX_id) {
                throw new SystemException(curLex.ToString());
            }
            //тут id
            tableId.addId(curLex);
            curLex.put_type(Type_of_lex._undefined);            
            Lexem hlp = curLex;
            
            getLex();
            if (curLex.get_type() == Type_of_lex.LEX_assign) {
                push(hlp);
                push(curLex);
                getLex();
                tableId.poliz_put_lex(new Lexem(Type_of_lex.POLIZ_ADDR, tableId.getNumOfLex(hlp).ToString()));
                rvalue();
                check_assign(hlp);
            }
            else {
                return;
            }
        }

        //for -> "for" "(" expression ";" expression ";" expression ")" block
        public void _for(){
            getLex(); // текущая - "for"
            if (curLex.get_type() != Type_of_lex.LEX_openRoundBracket) {
                throw new SystemException(curLex.ToString());
            }
            getLex();
            for (int i = 0; i < 2; i++) {
                expression();
                checkStackTypeLex();
                if (curLex.get_type() != Type_of_lex.LEX_semicolon) {
                    throw new SystemException(curLex.ToString());
                }
                getLex();
            }
            expression();
            //pop();
            if (curLex.get_type() != Type_of_lex.LEX_closeRoundBracket) {
                throw new SystemException(curLex.ToString());
            }
            getLex();
            block();
        }


        public void _if() {
            int pl2, pl3;
            getLex();
            condition();
            pl2 = tableId.poliz_get_free();
            tableId.poliz_blank();
            tableId.poliz_put_lex(new Lexem(Type_of_lex.POLIZ_FGO));
            block();
            pl3 = tableId.poliz_get_free();
            tableId.poliz_blank();
            tableId.poliz_put_lex(new Lexem(Type_of_lex.POLIZ_GO));
            int hlp = tableId.poliz_get_free();
            tableId.poliz_put_lex(new Lexem(Type_of_lex.POLIZ_LABEL, hlp.ToString()), pl2);
            if (curLex.get_type() == Type_of_lex.LEX_else) {
                getLex();
                if (curLex.get_type() == Type_of_lex.LEX_if) {
                    _if();
                }
                else {
                    block();
                }
                tableId.poliz_put_lex(new Lexem(Type_of_lex.POLIZ_LABEL, tableId.poliz_get_free().ToString()), pl3);
            }
            else {
                //tableId.poliz_put_lex(new Lexem(Type_of_lex.POLIZ_LABEL, tableId.poliz_get_free().ToString()), pl3);
                tableId.poliz_delLast2();
                tableId.poliz_put_lex(new Lexem(Type_of_lex.POLIZ_LABEL, (hlp-2).ToString()), pl2);
            }
        }
        public void condition() {
            if (curLex.get_type() != Type_of_lex.LEX_openRoundBracket) {
                throw new SystemException(curLex.ToString());
            }
            getLex();
            expression();
            if (curLex.get_type() != Type_of_lex.LEX_closeRoundBracket) {
                throw new SystemException(curLex.ToString());
            }
            getLex();
            pop();
        }


        public void _while(){
            int pl0, pl1, pl2;
            getLex();
            pl0 = tableId.poliz_get_free();
            condition();
            pl1 = tableId.poliz_get_free();
            tableId.poliz_blank();
            tableId.poliz_put_lex(new Lexem(Type_of_lex.POLIZ_FGO));
            block();
            tableId.poliz_put_lex(new Lexem(Type_of_lex.POLIZ_LABEL, pl0.ToString()));
            tableId.poliz_put_lex(new Lexem(Type_of_lex.POLIZ_GO));
            tableId.poliz_put_lex(new Lexem(Type_of_lex.POLIZ_LABEL, tableId.poliz_get_free().ToString()), pl1);
        }

        //--------------------встроенные функции начало
        //internal_functions -> write | read
        public void internal_functions() {
            switch (curLex.get_type()) {
                case Type_of_lex.LEX_WRITE:
                    _write();
                    break;
                case Type_of_lex.LEX_READ:
                    _read();
                    break;
                default:
                    throw new SystemException(curLex.ToString());
            }

        }
        public bool is_internal_function() {
            switch (curLex.get_type()) {
                case Type_of_lex.LEX_WRITE:
                case Type_of_lex.LEX_READ:
                    return true;
                default:
                    return false;
            }
        }
        public void _write() {
            function_call();
        }
        public void _read() {
            getLex();
            if (curLex.get_type() == Type_of_lex.LEX_openRoundBracket) {
                getLex();
                if (curLex.get_type() != Type_of_lex.LEX_id) {
                    throw new SystemException(curLex.ToString());
                }
                tableId.poliz_put_lex(new Lexem(Type_of_lex.POLIZ_ADDR, tableId.getNumOfLex(curLex).ToString()));
                tableId.poliz_put_lex(new Lexem(Type_of_lex.LEX_READ, "read"));
                getLex();

                if (curLex.get_type() != Type_of_lex.LEX_closeRoundBracket) {
                    throw new SystemException(curLex.ToString());
                }
                getLex();
                
            }
            else {
                throw new SystemException(curLex.ToString());
            }
        }
        
        //--------------------встроенные функции конец

        //"foreach" "(" identifier "in" identifier ")" block
        public void _foreach() {
            List<Lexem> lex_list = new List<Lexem>(3);
            Lexem lex_iter;
            string iterator_name="";
            getLex();
            if (curLex.get_type() == Type_of_lex.LEX_openRoundBracket) {
                for (int i = 0; i < 3; i++) {
                    getLex();
                    lex_list.Add(new Lexem (curLex)); 
                }
                if (lex_list[1].get_type() != Type_of_lex.LEX_in || lex_list[0].get_type() != Type_of_lex.LEX_id
                    || lex_list[2].get_type() != Type_of_lex.LEX_id) {
                        throw new SystemException(curLex.ToString());
                }
                getLex();
                if (curLex.get_type() != Type_of_lex.LEX_closeRoundBracket) {
                    throw new SystemException(curLex.ToString());
                }
                getLex();
            }
            else {
                throw new SystemException(curLex.ToString());
            }
            //аналог condition
            int pl0, pl1;
            iterator_name = add_new_uniq_lexId(Type_of_lex._int, null, null);
            lex_iter = new Lexem(Type_of_lex._int, iterator_name);
            //tableId.putValueOfLex(lex_iter, "0");
            tableId.poliz_put_lex(new Lexem(Type_of_lex.POLIZ_ADDR, tableId.getNumOfLex(lex_iter).ToString()));
            tableId.poliz_put_lex(new Lexem(Type_of_lex._int, "0"));
            tableId.poliz_put_lex(new Lexem(Type_of_lex.LEX_assign));


            tableId.addId(new LexID(new Lexem(Type_of_lex._htmlelement, lex_list[0].getName()), null, null));
            tableId.putValueOfLex(new Lexem(Type_of_lex._htmlelement, lex_list[0].getName()), lex_list[0].getName());

            pl0 = tableId.poliz_get_free();
          //condition start
            tableId.poliz_put_lex(new Lexem(Type_of_lex.LEX_id, iterator_name));
            tableId.poliz_put_lex(new Lexem(Type_of_lex.LEX_id, lex_list[2].getName()));
            tableId.poliz_put_lex(new Lexem(Type_of_lex.LEX_ElCcnt));
            tableId.poliz_put_lex(new Lexem(Type_of_lex.LEX_less));
          //condition end
            pl1 = tableId.poliz_get_free();
            tableId.poliz_blank();
            tableId.poliz_put_lex(new Lexem(Type_of_lex.POLIZ_FGO));
          //---------block start
            //в начале блока надо сделать elem = elemCollect[i]
            //tableId.poliz_put_lex(new Lexem(Type_of_lex.POLIZ_ADDR, tableId.getNumOfLex(lex_list[0]).ToString()));
            tableId.poliz_put_lex(new Lexem(Type_of_lex.LEX_id, lex_list[0].getName()));
            tableId.poliz_put_lex(new Lexem(Type_of_lex.LEX_id, lex_list[2].getName()));
            tableId.poliz_put_lex(new Lexem(Type_of_lex.LEX_id, iterator_name));
            tableId.poliz_put_lex(new Lexem(Type_of_lex.LEX_ElCbyNum));
            //tableId.poliz_put_lex(new Lexem(Type_of_lex.LEX_assign));
            block(); 
            //реализует i++ в while
            tableId.poliz_put_lex(new Lexem(Type_of_lex.POLIZ_ADDR, tableId.getNumOfLex(lex_iter).ToString()));
            tableId.poliz_put_lex(new Lexem(Type_of_lex.LEX_plusPlus));
            //в конце i++
          //---------block end
            tableId.poliz_put_lex(new Lexem(Type_of_lex.POLIZ_LABEL, pl0.ToString()));
            tableId.poliz_put_lex(new Lexem(Type_of_lex.POLIZ_GO));
            tableId.poliz_put_lex(new Lexem(Type_of_lex.POLIZ_LABEL, tableId.poliz_get_free().ToString()), pl1);
            

        }
        string add_new_uniq_lexId(Type_of_lex type, HtmlElement elem, List<HtmlElement> collect) {
            string name = "/hlp_iter_var";
            while (tableId.getNumOfLex(name) != -1) {
                name += "/";
            }
            tableId.addId(new LexID(new Lexem(type, name), elem, collect));
            return name;
        }
        public void _return() {
            getLex();
            lexpression();
        }
        //@lexpression -> function_call | identifier
        public void lexpression() {
            //если вызов встроенных фцнкций
            if (curLex.get_type() == Type_of_lex.LEX_semicolon) {
                return;
            }
            else {
                rvalue();
            }
        }
        public void function_call() {
            int initStackSize, endStackSize;
            Lexem hlp = curLex;
            getLex();
            if (curLex.get_type() == Type_of_lex.LEX_openRoundBracket) {
                getLex();
                initStackSize = getStackSize();
                int num = call_params();
                if (tableId.get_num_of_param(hlp.getName()) != num) {
                    throw new SystemException("wrong num params in function " + hlp.getName() + " call");
                }
                endStackSize = getStackSize();
                //убрать изстекалексем оставшееся после работы с параметрами вызова
                for (int i = 0; i < endStackSize - initStackSize; i++) {
                    pop();
                }
                if (curLex.get_type() != Type_of_lex.LEX_closeRoundBracket) {
                    throw new SystemException(curLex.ToString());
                }
                getLex();
                tableId.poliz_put_lex(hlp);
            }
            else {
                throw new SystemException(curLex.ToString());
            }

        }
        public int call_params() {
            int num = 0;
            //пустой список параметров
            if (curLex.get_type() == Type_of_lex.LEX_closeRoundBracket) {
                return num;
            }
            rvalue();
            num++;
            while (curLex.get_type() == Type_of_lex.LEX_comma) {
                getLex();
                rvalue();
                num++;
            }
            return num;
        }
        
        public void expression() {
            if (curLex.get_type() != Type_of_lex.LEX_id) {
                rvalue();
            }
            else {
                Lexem hlp = curLex;
                getLex();
                putBufLexem(hlp);
                if (curLex.get_type() == Type_of_lex.LEX_assign) {
                    chngBufANDcurLexem();
                    Lexem hlp1 = curLex;
                    checkId(false);
                    chngBufANDcurLexem();
                    push(curLex);
                    bufLexem = null;
                    //chngBufANDcurLexem();
                    //i=expr   curlex- i
                    tableId.poliz_put_lex(new Lexem(Type_of_lex.POLIZ_ADDR, tableId.getNumOfLex(hlp).ToString())); //Перенес в Check_id
                    getLex();
                    rvalue();
                    check_assign(hlp1);
                }//потом сюда добавится кусок кода для массивов (индексация)
                else {
                    chngBufANDcurLexem();
                    rvalue();
                }
            }
            
        }

        //lvalue -> identifier  //| identifier indexing
        public void lvalue() {
            if (curLex.get_type() != Type_of_lex.LEX_id) {
                throw new SystemException(curLex.ToString());
            }
            checkId();
            getLex();
            /*            if (curLex.get_type() == Type_of_lex.LEX_openSquareBracket) {
                            indexing();
                        }*/
        }
        /*        //indexing -> { "[" lvalue "]" } | "[" lvalue "]"
                public void indexing() {
                }
        */

        public void rvalue() {
            _or();
            while (curLex.get_type() == Type_of_lex.LEX_or) {
                push(curLex);
                getLex();
                _or();
                check_op();
            }
            
        }

        public void _or() {
            _and();
            while (curLex.get_type() == Type_of_lex.LEX_and) {
                push(curLex);
                getLex();
                _and();
                check_op();
            }
        }
        public void _and() {
            _equal();
            while ((curLex.get_type() == Type_of_lex.LEX_eq) || (curLex.get_type() == Type_of_lex.LEX_neq)) {
                push(curLex);
                getLex();
                _equal();
                check_op();
            }
        }
        public void _equal() {
            _inequal();
            while ((curLex.get_type() == Type_of_lex.LEX_bigger) || (curLex.get_type() == Type_of_lex.LEX_biggerEq) ||
                    (curLex.get_type() == Type_of_lex.LEX_less) || (curLex.get_type() == Type_of_lex.LEX_lessEq)) {
                push(curLex);
                getLex();
                _inequal();
                check_op();
            }
        }
        public void _inequal() {
            _sum();
            while ((curLex.get_type() == Type_of_lex.LEX_plus) || (curLex.get_type() == Type_of_lex.LEX_minus)) {
                push(curLex);
                getLex();
                _sum();
                check_op();
            }
        }
        public void _sum() {
            _mul();
            while ((curLex.get_type() == Type_of_lex.LEX_mul) || (curLex.get_type() == Type_of_lex.LEX_slash) ||
                    (curLex.get_type() == Type_of_lex.LEX_percent) ) {
                push(curLex);
                getLex();
                _mul();
                check_op();
            }
        }
        public void _mul() {
            switch (curLex.get_type()) {
                case Type_of_lex.LEX_plus:
                    push(curLex);
                    getLex();
                    _unary();
                    check_unary_left_op();
                    break;
                case Type_of_lex.LEX_minus:
                    push(curLex);
                    getLex();
                    _unary();
                    check_unary_left_op();
                    break;
                case Type_of_lex.LEX_not:
                    push(curLex);
                    getLex();
                    _unary();
                    check_not();
                    break;
                default:
                    _unary();
                    break;
            }
        }
        public void _unary() {
            switch (curLex.get_type()) {
                case Type_of_lex.LEX_openRoundBracket:
                    getLex();
                    expression();
                    if (curLex.get_type() != Type_of_lex.LEX_closeRoundBracket) {
                        throw new SystemException(curLex.ToString());
                    }
                    else {
                        getLex();
                    }
                    break;
                case Type_of_lex.LEX_plusPlus:
                    push(curLex);
                    getLex();
                    lvalue();
                    check_unary_left_op();
                    break;
                case Type_of_lex.LEX_minusMinus:
                    push(curLex);
                    getLex();
                    lvalue();
                    check_unary_left_op();
                    break;
                case Type_of_lex.LEX_id:
                    Lexem hlp = curLex;
                    getLex();
                    putBufLexem(hlp);
                    //если это вызов функции, причем описанной в коде, не встроенной
                    if (curLex.get_type() == Type_of_lex.LEX_openRoundBracket) {
                        chngBufANDcurLexem();
                        checkId(false);
                        function_call();
                    }
                    else {
                        chngBufANDcurLexem();
                        lvalue();
                        if (curLex.get_type() == Type_of_lex.LEX_plusPlus) {
                            push(curLex);
                            check_unary_right_op();
                            getLex();
                        }
                        if (curLex.get_type() == Type_of_lex.LEX_minusMinus) {
                            push(curLex);
                            check_unary_right_op();
                            getLex();
                        }
                    }    

                    break;
                default:
                    if (is_internal_function()) {
                        internal_functions();
                    }
                    else{
                        _const();
                    }
                    break;
            }
        }
        public void _const() {
            switch (curLex.get_type()) {
                case Type_of_lex.LEX_string:
                    curLex.put_type(Type_of_lex._string);
                    push(curLex);
                    tableId.poliz_put_lex(curLex);
                    getLex();
                    break;
                case Type_of_lex.LEX_num:
                    curLex.put_type(Type_of_lex._int);
                    //push(curLex);
                    Lexem hlp = curLex;
                    getLex();
                    if (curLex.get_type() == Type_of_lex.LEX_dot) {//float
                        getLex();
                        if (curLex.get_type() != Type_of_lex.LEX_num) {
                            throw new SystemException(curLex.ToString());
                        }
                        else {
                            //сделать лексему типа double
                            string value = hlp.getName() + "." + curLex.getName();
                            hlp.put_type(Type_of_lex._double);
                            hlp.putName(value);                            
                            getLex();
                        }
                    }
                    push(hlp);
                    tableId.poliz_put_lex(hlp);
                    break;
                case Type_of_lex.LEX_FALSE:
                    curLex.put_type(Type_of_lex._bool);
                    curLex.putName("false");
                    push(curLex);
                    tableId.poliz_put_lex(curLex);
                    getLex();
                    break;
                case Type_of_lex.LEX_TRUE:
                    curLex.put_type(Type_of_lex._bool);
                    curLex.putName("true");
                    push(curLex);
                    tableId.poliz_put_lex(curLex);
                    getLex();
                    break;
                default:
                    throw new SystemException(curLex.ToString());
            }
        }
    }
}
