using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace diplomWorkTranslator {
    /// <summary>
    /// Реализует работу с лексемой-идентификатором.
    /// Содержит имя, тип и значение лексемы.
    /// </summary>
    public class LexID {
        string name;
        Type_of_lex type;
        string value;
        //если тип HTMLElement|HTMLElementCollection, значение хранится тут
        HtmlElement html_elem;
        List<HtmlElement> html_elem_collect;
        //Element elem_val;//Если тип переменной - Element
        //может номер в новой таблице, которая в webBrowser?
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="lex">лексема</param>
        /// <param name="elem">значение HtmlElement</param>
        /// <param name="collect">значение HtmlElementCollection</param>
        public LexID(Lexem lex, HtmlElement elem = null, List<HtmlElement> collect = null) {
            html_elem = elem;
            html_elem_collect = collect;//Не копирование, а просто присваивание ссылки
            value = "";
            name = lex.getName();

            if (elem != null) {
                type = Type_of_lex._htmlelement;
                value = name;
            }
            else if (collect != null) {
                type = Type_of_lex._htmlelementcollect;
                value = name;
            }
            else if (lex.get_type() != Type_of_lex.LEX_id) {
                type = lex.get_type();
            }
            else {
                type = Type_of_lex._undefined;
            }
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="lex">Лексема-идентификатор</param>
        public LexID(LexID lex) {
            name = lex.name;
            type = lex.type;
            value = lex.value;
            html_elem = lex.html_elem;
            html_elem_collect = lex.html_elem_collect;
        }
        //методы
        public string getName() {
            return name;
        }
        public string getValue() {
            return value;
        }
        public Type_of_lex getType() {
            return type;
        }
        public void putType(Type_of_lex val) {
            type = val;
            if (type == Type_of_lex._htmlelement || type == Type_of_lex._htmlelementcollect) {
                value = name;
            }
        }
        public void putValue(string val) {
            value = val;
        }
        public HtmlElement get_htmlElem_val() {
            return html_elem;
        }
        public List<HtmlElement> get_htmlElemCollection_val() {
            return html_elem_collect;
        }
        public void put_html_val(HtmlElement elem) {
            html_elem = elem;
        }
        public void put_html_val(List<HtmlElement> elem_collect) {
            html_elem_collect = elem_collect;
        }

    }
}
