using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace diplomWorkTranslator {
    //массив для полиза. Для вспомогательных действий будем использовать стек из TableIdFunc
    /// <summary>
    /// Реализует работу с ПОЛИЗом программы
    /// </summary>
    public class Poliz {
        public List <Lexem> arr;         
        public Poliz (){
            arr = new List<Lexem>();
        }
        public void put_lex(Lexem lex) {
            arr.Add(new Lexem(lex));
        }
        public void put_lex(Lexem lex, int num) {
            arr[num] = lex;
        }
        public void blank() {
            arr.Add(null);
        }
        public int get_free() {
            return arr.Count;
        }

        public void clear() {
            arr.Clear();
        }
        public void delLast2() {
            arr.RemoveRange(arr.Count - 2, 2);
        }
    }
}
