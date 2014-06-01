using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace diplomWorkTranslator {
    /// <summary>
    /// Реализует получение лексем из исходного текста программы
    /// </summary>
    public class Scanner {
        static Type_of_lex[] keyWordsTable={
            Type_of_lex.LEX_function,
            Type_of_lex.LEX_var,
            Type_of_lex.LEX_for,
            Type_of_lex.LEX_while,
            Type_of_lex.LEX_if,
            Type_of_lex.LEX_else,
            Type_of_lex.LEX_return,
            Type_of_lex.LEX_id,
            Type_of_lex.LEX_num,
            Type_of_lex.LEX_string,
            Type_of_lex.LEX_WRITE,
            Type_of_lex.LEX_READ,
            Type_of_lex.LEX_foreach,
            Type_of_lex.LEX_in,
            Type_of_lex.LEX_TRUE,
            Type_of_lex.LEX_FALSE
        };
        static string[] keyWordsNamesTable={
            "function",
            "var",
            "for",
            "while",
            "if",
            "else",
            "return",
            "identifier",//используется только для вывода
            "number",//используется только для вывода
            "string",//используется только для вывода
            "write",
            "read",
            "foreach",
            "in",
            "true",
            "false",
            null
        };
        static Type_of_lex[] delimTable={
            Type_of_lex.LEX_openRoundBracket, //(
            Type_of_lex.LEX_closeRoundBracket, //)
            Type_of_lex.LEX_comma, //,
            Type_of_lex.LEX_semicolon, //;
            Type_of_lex.LEX_openFigBracket, //{
            Type_of_lex.LEX_closeFigBracket, //}
            Type_of_lex.LEX_assign, //=
            Type_of_lex.LEX_dot, //.
            Type_of_lex.LEX_openSquareBracket, //[
            Type_of_lex.LEX_closeSquareBracket, //]
            Type_of_lex.LEX_question, //?
            Type_of_lex.LEX_colon, //:
            Type_of_lex.LEX_or, // ||
            Type_of_lex.LEX_and, //&&
            Type_of_lex.LEX_eq, //==
            Type_of_lex.LEX_neq, //!=
            Type_of_lex.LEX_bigger, // >
            Type_of_lex.LEX_biggerEq, // >=
            Type_of_lex.LEX_less, // <
            Type_of_lex.LEX_lessEq, //<=
            Type_of_lex.LEX_plus, //+
            Type_of_lex.LEX_minus, //-
            Type_of_lex.LEX_mul, //*
            Type_of_lex.LEX_slash, // /
            Type_of_lex.LEX_percent, // %
            Type_of_lex.LEX_not, //!
            Type_of_lex.LEX_plusPlus, //++
            Type_of_lex.LEX_minusMinus, //--
            Type_of_lex.LEX_quote, // "
            Type_of_lex.LEX_fin
        };
        static string[] delimNamesTable={
            "(",
            ")",
            ",",
            ";",
            "{",
            "}",
            "=",
            ".",
            "[",
            "]",
            "?",
            ":",
            "||",
            "&&",
            "==",
            "!=",
            ">",
            ">=",
            "<",
            "<=",
            "+",
            "-",
            "*",
            "/",
            "%",
            "!",
            "++",
            "--",
            "\"",
            "EOF",
            null
        };
        enum State { H, IDENT, NUMB, CMP_2_SYMB, DELIM, AND_OR, PLUSPLUS_MINUSMINUS, STRING};
        State curState;
        System.IO.StringReader file;
        string word;
        char curSymb ;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="program">Путь к файлу с программой или текст программы</param>
        /// <param name="prog_is_not_file_name">Является ли первый параметр программой</param>
        public Scanner(string program, bool prog_is_not_file_name=false){
            if (program == "") {
                file = new System.IO.StringReader("");
            }
            else if (!prog_is_not_file_name) {
                System.IO.StreamReader sr = new System.IO.StreamReader(program, Encoding.GetEncoding(1251));
                file = new System.IO.StringReader(sr.ReadToEnd());
                sr.Close();
            }
            else {
                file = new System.IO.StringReader(program);
            }

            curState = State.H;
            word = "";
            clear();
            gc();
        }

        void clear (){
            word = "";    
        }
        void add(){ 
            word += curSymb; 
        }
        //найти в таблице
        int look (string buf, string[] tbl){
            int i = 0;
            while (tbl[i] != null){
               // if (buf.Equals(tbl[i]))
                if(buf == tbl[i])
                    return i;
                ++i;
            }
            return -1;
        }
        void gc(){
            curSymb = (char)file.Read(); 
        }
        /// <summary>
        /// Возвращает лексему
        /// </summary>
        /// <returns></returns>
        public Lexem get_lex(){
            string numb_value = "";
            curState = State.H;
            int num_in_tables = 0;
            Lexem resLex = new Lexem();
            do {
                switch (curState) {
                    case State.H:
                        if (curSymb == ' ' || curSymb == '\n' || curSymb == '\r' || curSymb == '\t') {
                            gc();
                        } else if (Char.IsLetter(curSymb)) {
                            clear();
                            add();
                            gc();
                            curState = State.IDENT;
                        } else if (Char.IsDigit(curSymb)) {
                            //numb_value += curSymb;
                            curState = State.NUMB;
                        } else if (curSymb =='>' || curSymb =='<' || curSymb == '!' || curSymb == '=') {
                            clear();
                            add();
                            gc();
                            curState = State.CMP_2_SYMB;
                        } else if (curSymb == '"') {
                            clear();
                            gc();
                            curState = State.STRING;
                        } else if (curSymb == '&' || curSymb == '|') {
                            clear();
                            add();
                            gc();
                            curState = State.AND_OR;
                        } else if (curSymb == '+' || curSymb == '-') {
                            clear();
                            add();
                            gc();
                            curState = State.PLUSPLUS_MINUSMINUS;
                        } else if (file.Peek() == -1) {      
                            file.Close();
                            return new Lexem(Type_of_lex.LEX_fin);
                        } else curState = State.DELIM;
                        break;
                    case State.IDENT:
                        if (Char.IsLetterOrDigit(curSymb) || curSymb == '_') {
                            add();
                            gc();
                        } else if ((num_in_tables = look(word, keyWordsNamesTable)) != -1) {//ключевое слово
                            return new Lexem(keyWordsTable[num_in_tables], word);
                        } else {
                            return new Lexem(Type_of_lex.LEX_id, word);//идентификатор
                        }
                        break;
                    case State.NUMB:
                        if (Char.IsDigit(curSymb)) {
                            //d = d * 10 + (c - '0');
                            numb_value += curSymb;
                            gc();
                        } else return new Lexem(Type_of_lex.LEX_num, numb_value);
                        break;
                    case State.STRING:
                        if (curSymb == '"') {
                            gc();
                            curState = State.H;
                            return new Lexem(Type_of_lex.LEX_string, word);
                        } else {
                            add();
                            gc();
                        }
                        break;
                    case State.CMP_2_SYMB:
                        if (curSymb == '=') {
                            add();
                            gc();
                            num_in_tables = look(word, delimNamesTable);
                            return new Lexem(delimTable[num_in_tables]);
                        } else {
                            num_in_tables = look(word, delimNamesTable);
                            return new Lexem(delimTable[num_in_tables]);
                        }
                        //break;
                    case State.AND_OR:
                        if (curSymb == word[0]) {
                            add();
                            gc();
                            num_in_tables = look(word, delimNamesTable);
                            return new Lexem(delimTable[num_in_tables]);
                        } else {
                            throw new SystemException(curSymb.ToString());
                        }
                        //break;
                    case State.PLUSPLUS_MINUSMINUS:
                        if (curSymb == word[0]) {
                            add();
                            gc();
                            num_in_tables = look(word, delimNamesTable);
                            return new Lexem(delimTable[num_in_tables]);
                        } else {
                            num_in_tables = look(word, delimNamesTable);
                            return new Lexem(delimTable[num_in_tables]);
                        }
                        //break;
                    case State.DELIM:
                        clear();
                        add();
                        if ((num_in_tables = look(word, delimNamesTable)) != -1) {
                            gc();
                            return new Lexem(delimTable[num_in_tables]);
                        } else {
                            throw new SystemException(curSymb.ToString());//event описать
                        }
                        //break;
                    default:
                        return new Lexem(Type_of_lex.LEX_NULL);
                }
            } while (true);
        }
    }
    
}
