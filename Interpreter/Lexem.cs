using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace diplomWorkTranslator{
    /// <summary>
    /// Представляет все типы лексем
    /// </summary>
    public enum Type_of_lex {
        LEX_NULL, //0
	    LEX_function,
        LEX_var,
        LEX_for,
        LEX_while,
        LEX_if,
        LEX_else,
        LEX_return,
        LEX_id,
        LEX_num,
        LEX_string,
        LEX_openRoundBracket, //(
        LEX_closeRoundBracket, //)
        LEX_comma, //,
        LEX_semicolon, //;
        LEX_openFigBracket, //{
        LEX_closeFigBracket, //}
        LEX_assign, //=
        LEX_dot, //.
        LEX_openSquareBracket, //[
        LEX_closeSquareBracket, //]
        LEX_question, //?
        LEX_colon, //:
        LEX_or, // ||
        LEX_and, //&&
        LEX_eq, //==
        LEX_neq, //!=
        LEX_bigger, // >
        LEX_biggerEq, // >=
        LEX_less, // <
        LEX_lessEq, //<=
        LEX_plus, //+
        LEX_minus, //-
        LEX_mul, //*
        LEX_slash, // /
        LEX_percent, // %
        LEX_not, //!
        LEX_plusPlus, //++
        LEX_minusMinus, //--
        LEX_quote, //"
        LEX_fin,
        _int,
        _double,
        _undefined,
        _string,
        _void,
        _bool,
        _htmlelement,
        _htmlelementcollect,
        POLIZ_GO,
        POLIZ_FGO,
        POLIZ_LABEL,
        POLIZ_ADDR,
        LEX_TRUE,
        LEX_FALSE,
        LEX_WRITE,
        LEX_READ,
        LEX_UPLUS,
        LEX_UMINUS,
        LEX_MINUSMINUSright,
        LEX_PLUSPLUSright,
        LEX_foreach,
        LEX_in,
        LEX_ElCcnt,
        LEX_ElCbyNum
    };
    /// <summary>
    /// Реализует работу с лексемой
    /// </summary>
    public class Lexem {
        Type_of_lex t_lex;
        string name;//оно же значение
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="t">Тип лексемы</param>
        /// <param name="v">Значение лексемы(имя)</param>
        public Lexem (Type_of_lex t = Type_of_lex.LEX_NULL, string v = ""){
            t_lex = t;
            name = v;
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="lex">Инициализирует экземпляр класса существующей лексемой</param>
        public Lexem(Lexem lex) {
            t_lex = lex.get_type();
            name = lex.getName();
        }
        /// <summary>
        /// Получает имя лексемы
        /// </summary>
        /// <returns></returns>
        public string getName() {
            return name;
        }
        /// <summary>
        /// Задает имя лексемы
        /// </summary>
        /// <param name="val"></param>
        public void putName(string val) {
            name = val;
        }
        /// <summary>
        /// Получает тип лексемы
        /// </summary>
        /// <param name="type"></param>
        public void put_type(Type_of_lex type) {
            t_lex = type;
        }
        /// <summary>
        /// Задает тип лексемы
        /// </summary>
        /// <returns></returns>
        public Type_of_lex get_type (){ 
            return t_lex; 
        }
    }
}
