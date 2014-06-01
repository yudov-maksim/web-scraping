using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using webBrowser;

namespace diplomWorkTranslator {
    /// <summary>
    /// Отвечает за выполнение кода, содержащегося в ПОЛИЗе
    /// </summary>
    public class Executer {
        Lexem curLex;
        GlobalTableId tableId;
        Poliz prog;
        Stack<Lexem> args;
        int cur_step_poliz;
        System.Windows.Forms.RichTextBox out_stream;
        System.Windows.Forms.TextBox in_stream;
        WB wb;
        
        /// <summary>
        /// Контруктор
        /// </summary>
        /// <param name="table">Таблица идентификаторов</param>
        /// <param name="i">Ввод</param>
        /// <param name="o">Вывод</param>
        /// <param name="web">Браузер</param>
        public Executer(GlobalTableId table, System.Windows.Forms.TextBox i, System.Windows.Forms.RichTextBox o, WB web) {
            wb = web;
            out_stream = o;
            in_stream = i;
            cur_step_poliz = 0;
            tableId = new GlobalTableId();
            tableId = table;
            prog = tableId.get_poliz(tableId.get_curFunc());
            args = new Stack<Lexem>();
        }
        /// <summary>
        /// Создает новую таблицу идентификаторов. Необходима для реализации пошагового режима.
        /// </summary>
        /// <param name="table"></param>
        public void make_new_TableId(GlobalTableId table) {
            tableId = table;
            prog = tableId.get_poliz(tableId.get_curFunc());
            args = new Stack<Lexem>();
            cur_step_poliz = 0;
        }
        /// <summary>
        /// Реализация вывода
        /// </summary>
        /// <param name="arg"></param>
        public void system_print(string arg) {
            if (out_stream != null){
                out_stream.Text += arg + '\n';
                out_stream.SelectionStart = out_stream.Text.Length;
                out_stream.ScrollToCaret();
            }
        }

        string system_read() {
            if (in_stream != null)
                return in_stream.Text;
            else
                return "";
        }
        Lexem args_pop() {
            return args.Pop();
        }
        void setCurFunc(int num) {
            tableId.set_curFunc(num);
        }
        void putValueOfLex(int num, string name) {
            tableId.putValueOfLex(num, name);
        }
        void putTypeofLex(int num, Type_of_lex type) {
            tableId.putTypeofLex(num, type);
        }
        string add_new_uniq_lexId(Type_of_lex type, HtmlElement elem, List<HtmlElement> collect) {
            string name = "/hlp_html_var";
            while (tableId.getNumOfLex(name) != -1) {
                name += "/";
            }
            tableId.addId(new LexID(new Lexem(type, name), elem, collect));
            return name;
        }
        List<HtmlElement> ret_list_htmlElem_from_collection(HtmlElementCollection collect) {
            List<HtmlElement> list_htmlElement = new List<HtmlElement>();
            for (int i = 0; i < collect.Count; i++) {
                list_htmlElement.Add(collect[i]);
            }
            return list_htmlElement;
        }
        /// <summary>
        /// Реализует выполнение встроенных функций
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool run_inet_func(string name) {
            HtmlElement htmlElem = null;
            List<HtmlElement> htmlElem_list=null;
            string res="";
            bool bl;
            
            int num_params = tableId.get_num_of_param(name);
            Lexem[] lex_arr = new Lexem[num_params];
            //в массиве лежат либо константы, либо переменные

            for (int i = num_params - 1; i >= 0; i--) {
                lex_arr[i] = args.Pop();
            }

            //switch по функциям
            switch (name) {
                case "click_by_id":
                    wb.click_by_id(lex_arr[0].getName());
                    break;
                case "check_by_id":
                    bl = wb.check_by_id(lex_arr[0].getName());
                    res = getValueFromBool(bl);
                    break;
                case "input_by_id":
                    wb.input_by_id(lex_arr[0].getName(), lex_arr[1].getName());
                    break;
                case "go":
                    wb.go(lex_arr[0].getName());
                    break;
                case "check_by_text":
                    bl = wb.check_by_text(lex_arr[0].getName());
                    res = getValueFromBool(bl);
                    break;
                case "set_options":
                    wb.set_options(lex_arr[0].getName());
                    break;
                case "click_href":
                    wb.click_href(lex_arr[0].getName());
                    break;
                case "click_by_text":
                    wb.click_by_text(lex_arr[0].getName());
                    break;
                case "set_check_box_flag":
                    wb.set_check_box_flag(lex_arr[0].getName(), getBoolFromValue(lex_arr[1]));
                    break;
                case "click_check_box_flag":
                    wb.click_check_box_flag(lex_arr[0].getName());
                    break;
                case "click_elem":
                    wb.click_elem(tableId.get_htmlElem_of_lex(lex_arr[0].getName()));
                    break;
                    //----------------------
                case "get_tree":
                    htmlElem_list = wb.get_tree(lex_arr[0].getName());
                    break;
                case "get_elements_by_tag":
                    htmlElem_list = wb.get_elements_by_tag(lex_arr[0].getName());
                    break;
                case "get_elements_by_attr_value":
                    htmlElem_list = wb.get_elements_by_attr_value(lex_arr[0].getName(), lex_arr[1].getName());
                    
                    break;
                case "save":
                    htmlElem = tableId.get_htmlElem_of_lex(lex_arr[0].getName());
                    //res = wb.save(htmlElem, lex_arr[1].getName());
                    wb.save(htmlElem, lex_arr[1].getName());
                    break;
                case "saveHTML":
                    htmlElem = tableId.get_htmlElem_of_lex(lex_arr[0].getName());
                    //res = wb.saveHTML(htmlElem, lex_arr[1].getName());
                    wb.saveHTML(htmlElem, lex_arr[1].getName());
                    break;
                case "save_images":
                    htmlElem = tableId.get_htmlElem_of_lex(lex_arr[0].getName());
                    wb.save_images(htmlElem, lex_arr[0].getName());
                    break;
                //-------------------------
                case "get_element_with_num":
                    htmlElem = tableId.get_htmlElemCollect_of_lex(lex_arr[0].getName())[Convert.ToInt32(lex_arr[1].getName())];
                    break;
                case "get_length":
                    res = tableId.get_htmlElemCollect_of_lex(lex_arr[0].getName()).Count.ToString();
                    break;
                case "input_elem":
                    wb.input_elem(tableId.get_htmlElem_of_lex(lex_arr[0].getName()), lex_arr[1].getName());
                    break;
                case "set_check_box_elem":
                    wb.set_check_box_elem(tableId.get_htmlElem_of_lex(lex_arr[0].getName()), getBoolFromValue(lex_arr[1]));
                    break;
                case "set_options_elem":
                    wb.set_options_elem(tableId.get_htmlElem_of_lex(lex_arr[0].getName()), lex_arr[1].getName());
                    break;
                case "wait":
                    wb.wait(Convert.ToInt32(lex_arr[0].getName()));
                    break;
                case "go_back":
                    wb.go_back();
                    break;
                default:
                    break;
            }
            Type_of_lex type = tableId.getTypeOfFunc(name);
            if (type != Type_of_lex._void) {                 
                if (type == Type_of_lex._htmlelement || type == Type_of_lex._htmlelementcollect){
                    res = add_new_uniq_lexId(type, htmlElem, htmlElem_list);
                } 
                args.Push(new Lexem(type, res));  
            }
            //cur_step_poliz++;
            return tableId.get_has_event_func(name);
        }
        /// <summary>
        /// Реализует выполнение описанных пользователем функций
        /// </summary>
        /// <param name="func_name"></param>
        public void run_func(string func_name) {
            int old_curFunc = tableId.get_curFunc();
            int num_of_func = tableId.getNumOfFunc(func_name);
            int num_params = tableId.get_num_of_param(func_name);
            int old_csp = cur_step_poliz;
            HtmlElement helem;
            List<HtmlElement> helemCollect, collect_to_run_func;
            Lexem[] lex_arr = new Lexem[num_params];
            //в массиве лежат либо константы, либо переменные
            
            //создать новый экземпляр класса executer и вызвать из него выполнение проги
            setCurFunc(num_of_func);
            //Да, ничего не скопирует, но рекурсия станет возможной
            Executer func_exec = new Executer(tableId, in_stream, out_stream, wb);

            //передача параметров в вызываемую функцию
            for (int i = num_params - 1; i >= 0; i--) {
                lex_arr[i] = args.Pop();
            }
            //обнуление всех переменных внутри функции
            //func_exec.tableId.initialize_all_vars();
            for (int i = 0; i < num_params; i++){
                if (lex_arr[i].get_type() == Type_of_lex._htmlelement || lex_arr[i].get_type() == Type_of_lex._htmlelementcollect) {
                    //func_exec.tableId.putValueOfLex(i, lex_arr[i].getName(), true);
                    setCurFunc(old_curFunc);
                    if (lex_arr[i].get_type() == Type_of_lex._htmlelement) {
                        helem = func_exec.tableId.get_htmlElem_of_lex(lex_arr[i].getName());
                        setCurFunc(num_of_func);
                        func_exec.tableId.putHtmlElemValueOfLex(i, helem);
                    }
                    else {
                        helemCollect = tableId.get_htmlElemCollect_of_lex(lex_arr[i].getName());
                        setCurFunc(num_of_func);

                        collect_to_run_func = new List<HtmlElement>(helemCollect.Count);
                        for (int j = 0; j < helemCollect.Count; j++) {
                            collect_to_run_func.Add(helemCollect[j]);
                        }
                            //тут нужно передать не ссылку, а новый список
                        func_exec.tableId.putHtmlElemCollectValueOfLex(i, collect_to_run_func);
                    }
                }
                else {
                    func_exec.tableId.putValueOfLex(i, lex_arr[i].getName());
                } 
                if (lex_arr[i].get_type() != Type_of_lex._undefined) {
                    func_exec.putTypeofLex(i, lex_arr[i].get_type());
                }
            }
            func_exec.execute();
            Lexem lex = null;
            if (func_exec.args.Count > 0) {
                lex = func_exec.args_pop();
            }
            
            //для добавления вспомогательной переменной
            if (lex != null) {                
                Type_of_lex type = tableId.getTypeOfFunc(func_name);                
                if (type == Type_of_lex._htmlelement || type == Type_of_lex._htmlelementcollect) {
                    string res_name = "";
                    HtmlElement htmlElem = null;
                    List<HtmlElement> htmlElemCollect = null;
                    if (type == Type_of_lex._htmlelement) {
                        htmlElem = tableId.get_htmlElem_of_lex(lex.getName());
                        htmlElemCollect = null;
                    }
                    else {
                        htmlElemCollect = tableId.get_htmlElemCollect_of_lex(lex.getName());
                        htmlElem = null;
                    }    
                    setCurFunc(old_curFunc);
                    res_name = add_new_uniq_lexId(type, htmlElem, htmlElemCollect);
                    lex.putName(res_name);
                }
                args.Push(lex);
            }
            setCurFunc(old_curFunc);
            cur_step_poliz = old_csp;
        }
        bool id_is_func(Lexem lex) {
            int num = tableId.getNumOfFunc(lex.getName());
            if (num >= 0) {
                return true;
            }
            else {
                return false;
            }
        }
        /// <summary>
        /// Реализует выполнение ПОЛИЗа
        /// </summary>
        public void execute(){ 
            Lexem lex, lex1, lex2;
            bool b;
            string val, hlp_str;
            int i, j, size = prog.get_free(), num;
            Type_of_lex t;
            if (wb.exit_run > 0) {
                wb.exit_run--;
                cur_step_poliz++;
            }
            while (cur_step_poliz < size){ 
                curLex = prog.arr[cur_step_poliz]; 
                switch (curLex.get_type()){ 
                    case Type_of_lex._int: 
                    case Type_of_lex._double:
                    case Type_of_lex._string:
                    case Type_of_lex._bool:
                    case Type_of_lex.POLIZ_ADDR: 
                    case Type_of_lex.POLIZ_LABEL: 
                        args.Push(curLex); 
                        break; 
                    case Type_of_lex.LEX_id:
                        //Проверили, не вызов ли функции
                        if (!id_is_func(curLex)){
                            lex = new Lexem(tableId.get_type_of_lex(curLex.getName()), tableId.get_value_of_lex(curLex.getName()));
                            args.Push(lex);
                        }else{
                            if (tableId.getNumOfFunc(curLex.getName()) < tableId.get_inet_func_num()) {
                                bool need_exit_in_wait_loop = run_inet_func(curLex.getName());
                                 
                                if (need_exit_in_wait_loop) {
                                    system_print(">>>exit in main wait loop<<<");
                                    wb.exit_run--;  
                                    cur_step_poliz++;
                                    return;
                                }
                            }
                            else{
                                run_func(curLex.getName());
                            }
                        }
                        break;
                    case Type_of_lex.LEX_ElCcnt:
                        lex = args.Pop();
                        args.Push(new Lexem(Type_of_lex._int, tableId.get_htmlElemCollect_of_lex(lex.getName()).Count.ToString()));
                        break;
                    case Type_of_lex.LEX_ElCbyNum:
                        lex = args.Pop();//iterator
                        lex1 = args.Pop();//elem_collection
                        lex2 = args.Pop();//elem

                        HtmlElement hc = tableId.get_htmlElemCollect_of_lex(lex1.getName())[Convert.ToInt32(lex.getName())];
                        tableId.putHtmlElemValueOfLex(tableId.getNumOfLex(lex2), hc);
                        break;
                    case Type_of_lex.LEX_WRITE:
                        //System.Console.WriteLine(args.Pop().getName());
                        system_print(args.Pop().getName());
                        break;
                    case Type_of_lex.LEX_READ:
                        lex = args.Pop();
                        //val = System.Console.ReadLine();
                        val = system_read();
                        tableId.putValueOfLex(Convert.ToInt32(lex.getName()), val);
                        break;
                    case Type_of_lex.LEX_not:
                        lex = args.Pop();
                        args.Push(new Lexem(lex.get_type(), getValueFromBool(!getBoolFromValue(lex))));
                        break;  
                    case Type_of_lex.LEX_or:
                        lex1 = args.Pop();
                        lex2 = args.Pop();
                        b = getBoolFromValue(lex1) || getBoolFromValue(lex2);
                        args.Push(new Lexem(Type_of_lex._bool, getValueFromBool(b)));
                        break;
                    case Type_of_lex.LEX_and:
                        lex1 = args.Pop();
                        lex2 = args.Pop();
                        b = getBoolFromValue(lex1) && getBoolFromValue(lex2);
                        args.Push(new Lexem(Type_of_lex._bool, getValueFromBool(b)));
                        break;
                    case Type_of_lex.POLIZ_GO:
                        cur_step_poliz = Convert.ToInt32(args.Pop().getName()) - 1;
                        break;
                    case Type_of_lex.POLIZ_FGO:
                        lex1 = args.Pop();
                        lex2 = args.Pop();
                        if (!getBoolFromValue(lex2)) {
                            cur_step_poliz = Convert.ToInt32(lex1.getName()) - 1;
                        }
                        break;
                    case Type_of_lex.LEX_UPLUS:
                        break;
                    case Type_of_lex.LEX_UMINUS:
                        lex1 = args.Pop();
                        val = "-" + lex1.getName();
                        args.Push(new Lexem(lex1.get_type(), val));
                        break;
                    case Type_of_lex.LEX_minusMinus:
                    case Type_of_lex.LEX_MINUSMINUSright:
                        lex1 = args.Pop();
                        num = Convert.ToInt32(lex1.getName());
                        t = tableId.getTypeofLex(num);
                        hlp_str = tableId.get_value_of_lex(num);
                        switch (t) {
                            case Type_of_lex._string:
                                val = hlp_str.Remove(hlp_str.Length - 1);
                                break;
                            case Type_of_lex._bool:
                                bool a1 = getBoolFromValue(new Lexem(Type_of_lex._bool, hlp_str));
                                b = !a1;
                                val = getValueFromBool(b);
                                break;
                            case Type_of_lex._double:
                                val = (Convert.ToDouble(hlp_str) - 1).ToString();
                                break;
                            case Type_of_lex._int:
                                val = (Convert.ToInt32(hlp_str) - 1).ToString();
                                break;
                            default:
                                val = hlp_str.Remove(hlp_str.Length - 1);
                                break;
                        }
                        tableId.putValueOfLex(Convert.ToInt32(lex1.getName()), val);
                        if (curLex.get_type() == Type_of_lex.LEX_minusMinus) {
                            args.Push(new Lexem(t, val));
                        }
                        else {
                            args.Push(new Lexem(t, hlp_str));
                        }
                        break;
                    case Type_of_lex.LEX_plusPlus:
                    case Type_of_lex.LEX_PLUSPLUSright:
                        lex1 = args.Pop();
                        num = Convert.ToInt32(lex1.getName());
                        t = tableId.getTypeofLex(num);
                        hlp_str = tableId.get_value_of_lex(num);
                        switch (t) {
                            case Type_of_lex._string:
                                val = hlp_str + " ";
                                break;
                            case Type_of_lex._bool:
                                bool a1 = getBoolFromValue(new Lexem(Type_of_lex._bool, hlp_str));
                                b = !a1;
                                val = getValueFromBool(b);
                                break;
                            case Type_of_lex._double:
                                val = (Convert.ToDouble(hlp_str) + 1).ToString();
                                break;
                            case Type_of_lex._int:
                                val = (Convert.ToInt32(hlp_str) + 1).ToString();
                                break;
                            default:
                                val = hlp_str + " ";
                                break;
                        }
                        tableId.putValueOfLex(Convert.ToInt32(lex1.getName()), val);
                        if (curLex.get_type() == Type_of_lex.LEX_plusPlus) {
                            args.Push(new Lexem(t, val));
                        }
                        else {
                            args.Push(new Lexem(t, hlp_str));
                        }
                        break;
                    case Type_of_lex.LEX_plus:
                        lex1 = args.Pop();
                        lex2 = args.Pop();
                        t = get_real_type(lex1, lex2);
                        switch (t) {
                            case Type_of_lex._string:
                                val = lex1.getName() + lex2.getName();
                                break;
                            case Type_of_lex._bool:
                                bool a1=getBoolFromValue(lex1), a2=getBoolFromValue(lex2);
                                b = !a1 && a2 || a1 && !a2;
                                val = getValueFromBool(b);
                                break;
                            case Type_of_lex._double:
                                val = (Convert.ToDouble(lex1.getName()) + Convert.ToDouble(lex2.getName())).ToString();
                                break;
                            case Type_of_lex._int:
                                val = (Convert.ToInt32(lex1.getName()) + Convert.ToInt32(lex2.getName())).ToString();
                                break;
                            default:
                                val = lex1.getName() + lex2.getName();
                                break;
                        }
                        args.Push(new Lexem(t, val));
                        break;
                    case Type_of_lex.LEX_mul:
                        lex1 = args.Pop();
                        lex2 = args.Pop();
                        t = get_real_type(lex1, lex2);
                        switch (t) {
                            case Type_of_lex._string:
                                val = lex1.getName() + lex2.getName();
                                break;
                            case Type_of_lex._bool:
                                bool a1=getBoolFromValue(lex1), a2=getBoolFromValue(lex2);
                                b = a1 && a2;
                                val = getValueFromBool(b);
                                break;
                            case Type_of_lex._double:
                                val = (Convert.ToDouble(lex1.getName()) * Convert.ToDouble(lex2.getName())).ToString();
                                break;
                            case Type_of_lex._int:
                                val = (Convert.ToInt32(lex1.getName()) * Convert.ToInt32(lex2.getName())).ToString();
                                break;
                            default:
                                val = lex1.getName() + lex2.getName();
                                break;
                        }
                        args.Push(new Lexem(t, val));
                        break;
                    case Type_of_lex.LEX_minus:
                        lex1 = args.Pop();
                        lex2 = args.Pop();
                        t = get_real_type(lex1, lex2);
                        switch (t) {
                            case Type_of_lex._string:
                                val = "";
                                break;
                            case Type_of_lex._bool:
                                bool a1=getBoolFromValue(lex1), a2=getBoolFromValue(lex2);
                                b = !a1 && a2 || a1 && !a2;
                                val = getValueFromBool(b);
                                break;
                            case Type_of_lex._double:
                                val = (Convert.ToDouble(lex2.getName()) - Convert.ToDouble(lex1.getName())).ToString();
                                break;
                            case Type_of_lex._int:
                                val = (Convert.ToInt32(lex2.getName()) - Convert.ToInt32(lex1.getName())).ToString();
                                break;
                            default:
                                val = "";
                                break;
                        }
                        args.Push(new Lexem(t, val));
                        break;
                    case Type_of_lex.LEX_slash:
                        lex1 = args.Pop();
                        lex2 = args.Pop();
                        t = get_real_type(lex1, lex2);
                        switch (t) {
                            case Type_of_lex._string:
                                val = "";
                                break;
                            case Type_of_lex._bool:
                                val = getValueFromBool(false);
                                break;
                            case Type_of_lex._double:
                                val = (Convert.ToDouble(lex2.getName()) / Convert.ToDouble(lex1.getName())).ToString();
                                break;
                            case Type_of_lex._int:
                                val = (Convert.ToInt32(lex2.getName()) / Convert.ToInt32(lex1.getName())).ToString();
                                break;
                            default:
                                val = "";
                                break;
                        }
                        args.Push(new Lexem(t, val));
                        break;
                    case Type_of_lex.LEX_less:
                        lex1 = args.Pop();
                        lex2 = args.Pop();
                        t = get_real_type(lex1, lex2);
                        switch (t) {
                            case Type_of_lex._bool:
                                val = "";
                                break;
                            case Type_of_lex._double:
                                val = (Convert.ToDouble(lex2.getName()) < Convert.ToDouble(lex1.getName())).ToString();
                                break;
                            case Type_of_lex._int:
                                val = (Convert.ToInt32(lex2.getName()) < Convert.ToInt32(lex1.getName())).ToString();
                                break;
                            case Type_of_lex._string:
                            default:
                                i = String.Compare(lex2.getName(), lex1.getName());
                                if (i < 0) val = "true";
                                else val = "false";
                                break;
                        }
                        args.Push(new Lexem(Type_of_lex._bool, val));
                        break;
                    case Type_of_lex.LEX_bigger:
                        lex1 = args.Pop();
                        lex2 = args.Pop();
                        t = get_real_type(lex1, lex2);
                        switch (t) {
                            case Type_of_lex._bool:
                                val = "";
                                break;
                            case Type_of_lex._double:
                                val = (Convert.ToDouble(lex2.getName()) > Convert.ToDouble(lex1.getName())).ToString();
                                break;
                            case Type_of_lex._int:
                                val = (Convert.ToInt32(lex2.getName()) > Convert.ToInt32(lex1.getName())).ToString();
                                break;
                            case Type_of_lex._string:
                            default:
                                i = String.Compare(lex2.getName(), lex1.getName());
                                if (i > 0) val = "true";
                                else val = "false";
                                break;
                        }
                        args.Push(new Lexem(Type_of_lex._bool, val));
                        break;
                    case Type_of_lex.LEX_lessEq:
                        lex1 = args.Pop();
                        lex2 = args.Pop();
                        t = get_real_type(lex1, lex2);
                        switch (t) {
                            case Type_of_lex._bool:
                                val = "";
                                break;
                            case Type_of_lex._double:
                                val = (Convert.ToDouble(lex2.getName()) <= Convert.ToDouble(lex1.getName())).ToString();
                                break;
                            case Type_of_lex._int:
                                val = (Convert.ToInt32(lex2.getName()) <= Convert.ToInt32(lex1.getName())).ToString();
                                break;
                            case Type_of_lex._string:
                            default:
                                i = String.Compare(lex2.getName(), lex1.getName());
                                if (i <= 0) val = "true";
                                else val = "false";
                                break;
                        }
                        args.Push(new Lexem(Type_of_lex._bool, val));
                        break;
                    case Type_of_lex.LEX_biggerEq:
                        lex1 = args.Pop();
                        lex2 = args.Pop();
                        t = get_real_type(lex1, lex2);
                        switch (t) {
                            case Type_of_lex._bool:
                                val = "";
                                break;
                            case Type_of_lex._double:
                                val = (Convert.ToDouble(lex2.getName()) >= Convert.ToDouble(lex1.getName())).ToString();
                                break;
                            case Type_of_lex._int:
                                val = (Convert.ToInt32(lex2.getName()) >= Convert.ToInt32(lex1.getName())).ToString();
                                break;
                            case Type_of_lex._string:
                            default:
                                i = String.Compare(lex2.getName(), lex1.getName());
                                if (i >= 0) val = "true";
                                else val = "false";
                                break;
                        }
                        args.Push(new Lexem(Type_of_lex._bool, val));
                        break;
                    case Type_of_lex.LEX_neq:
                        val = (args.Pop().getName() != args.Pop().getName()).ToString();
                        args.Push(new Lexem(Type_of_lex._bool, val));
                        break;
                    case Type_of_lex.LEX_eq:
                        val = (args.Pop().getName() == args.Pop().getName()).ToString();
                        args.Push(new Lexem(Type_of_lex._bool, val));
                        break;
                    case Type_of_lex.LEX_assign:
                        lex1 = args.Pop();//что присваиваем
                        lex2 = args.Pop();//куда присваиваем
                        val = lex1.getName();
                        if (lex1.get_type() != Type_of_lex._htmlelement && lex1.get_type() != Type_of_lex._htmlelementcollect) {
                            tableId.putValueOfLex(Convert.ToInt32(lex2.getName()), val);
                        }
                        //в вычислителе лежало имя, а реальное значение надо взять из таблицы идентификаторов
                        else {
                            tableId.putValueOfLex(Convert.ToInt32(lex2.getName()), val, true);
                        }
                        break;
                }
                cur_step_poliz++;
            }//end of while
            if (cur_step_poliz >= size) {
                if (wb.isConsoleVersion) {
                    Application.Exit();
                }
            }
        }
        public string getValueFromBool(bool b) {
            if (b)
                return "true";
            else
                return "false";
        }
        bool getBoolFromValue(Lexem lex) {
            string val = lex.getName();
            switch (lex.get_type()) {
                case Type_of_lex._string:
                    if (val == "")
                        return false;
                    else
                        return true;
                case Type_of_lex._bool:
                    if (val.ToLower() == "false")
                        return false;
                    else
                        return true;
                case Type_of_lex._double:
                    if (Convert.ToDouble(val) == 0)
                        return false;
                    else
                        return true;
                case Type_of_lex._int:
                    if (Convert.ToInt32(val) == 0)
                        return false;
                    else
                        return true;
                default:
                    return false;
            }
        }
        Type_of_lex get_real_type(Lexem l1, Lexem l2) {
            //на момент работы анализаторов типы одинаковы. Во время выполнения могут уточняться
            if (l1.get_type() == Type_of_lex._undefined) {
                return l2.get_type();
            }
            else {
                return l1.get_type();
            }
        }
    }
}
