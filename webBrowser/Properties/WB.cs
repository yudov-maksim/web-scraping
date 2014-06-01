using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace webBrowser {
    /// <summary>
    /// Реализует работу с интернет-страницами. Управляет работой прриложения в целом.
    /// </summary>
    public class WB {
        public event EventHandler Raise_contin_exec_event;
        public WebBrowser webBrowser;
        //diplomWorkTranslator.Interpretator intrpr;
        public bool firstNavigating;
        SHDocVw.WebBrowser_V1 Web_V1;
        public int exit_run;
        //bool new_title;
        NeedExec needExec;
        public bool isConsoleVersion;

        public WB(bool isConsole=false) {
            firstNavigating = true;
            isConsoleVersion = isConsole;
            //new_title = true;
            needExec = new NeedExec();
            webBrowser = new WebBrowser();
            webBrowser.ScriptErrorsSuppressed = true;
            //intrpr = new diplomWorkTranslator.Interpretator(path_to_intrpr_prog);
            webBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(this.webBrowser_documentCompleted);
            //webBrowser.Navigating += new WebBrowserNavigatingEventHandler(this.webBrowser_navigating);
            webBrowser.Navigated += new WebBrowserNavigatedEventHandler(this.webBrowser_navigated);
            webBrowser.DocumentTitleChanged += new EventHandler(title_changed);
            //webBrowser.NewWindow += new System.ComponentModel.CancelEventHandler(cancel_load_new_window);
            Web_V1 = (SHDocVw.WebBrowser_V1)this.webBrowser.ActiveXInstance;
            Web_V1.NewWindow += new SHDocVw.DWebBrowserEvents_NewWindowEventHandler(Web_V1_NewWindow);
            exit_run = 0;
        }
        public void init_run(System.Windows.Forms.TextBox input, System.Windows.Forms.RichTextBox output) {
            //intrpr.interpretation(this, input, output);

        }
        public void step_execute(string prog) {
            //intrpr.step_execute(prog);
        }
//------------------------основные функции------------------------
//-----------------------------------------------------------------
//сбор данных
        void save_all_images() {
            //HtmlElementCollection collect= webBrowser.Document.Images;
            //System.Net.WebClient web_client = new System.Net.WebClient();
            //web_client.DownloadFile(collect[0].GetAttribute("src"), @"C:\Users\asus\Desktop\Diplom\image");
            mshtml.IHTMLDocument2 doc = (mshtml.IHTMLDocument2)webBrowser.Document.DomDocument;
            mshtml.IHTMLControlRange imgRange = (mshtml.IHTMLControlRange)((mshtml.HTMLBody)doc.body).createControlRange();
            int cnt = 0;
            string str;
            foreach (mshtml.IHTMLImgElement img in doc.images) {
                imgRange.add((mshtml.IHTMLControlElement)img);
                imgRange.execCommand("Copy", false, null);

                
                using (System.Drawing.Bitmap bmp = (System.Drawing.Bitmap)Clipboard.GetDataObject().GetData(DataFormats.Bitmap)) {
                    if (bmp != null) {
                        str = img.nameProp;
                        bmp.Save(cnt.ToString());
                        cnt++;
                    }
                }    
            }
        }

        void save_image_from_elemCollection(HtmlElementCollection collect){
            foreach(HtmlElement elem in collect){
                save_images(elem);
            }
        }

        public void save_images(HtmlElement elem, string addr_dir = "") {
            mshtml.IHTMLDocument2 doc = (mshtml.IHTMLDocument2)webBrowser.Document.DomDocument;
            mshtml.IHTMLControlRange imgRange = (mshtml.IHTMLControlRange)((mshtml.HTMLBody)doc.body).createControlRange();
            mshtml.IHTMLImgElement img = (mshtml.IHTMLImgElement) elem.DomElement;

            imgRange.add((mshtml.IHTMLControlElement)img);
            imgRange.execCommand("Copy", false, null);


            using (System.Drawing.Bitmap bmp = (System.Drawing.Bitmap)Clipboard.GetDataObject().GetData(DataFormats.Bitmap)) {
                if (bmp != null) {
                    if (addr_dir == "") {
                        bmp.Save(@"C:\" + "aaaa");
                    }
                    else {
                        bmp.Save(addr_dir + img.nameProp);
                    }
                }
            }    

        }

        List<string> get_normalize_path(string path) {
            string hlp = "";
            int i = 0;
            List<string> res = new List<string>();

            while (i < path.Length) {
                if (path[i] != '/') {
                    hlp += path[i];
                }
                else {
                    res.Add(hlp);
                    hlp = "";
                }
                i++;
            }
            res.Add(hlp);
            return res;
        }

        List<HtmlElement> gettree_recursive(List<string> norm_path, int cur_num, HtmlElementCollection collect) {
            List<HtmlElement> res = new List<HtmlElement>();
            foreach (HtmlElement elem in collect) {
                if (elem.TagName.ToLower() == norm_path[cur_num].ToLower()) {
                    if (cur_num == norm_path.Count - 1) {
                        res.Add(elem);
                    }
                    else {
                        res.AddRange(gettree_recursive(norm_path, cur_num + 1, elem.Children));
                    }
                }
            }
            return res;
        }

        public List<HtmlElement> get_tree(string path) {
            List<string> norm_path = get_normalize_path(path);
            HtmlElementCollection collect = webBrowser.Document.GetElementsByTagName(norm_path[0]);
            return gettree_recursive(norm_path, 0, collect);
        }


        public List<HtmlElement> get_elements_by_tag(string tag_name) {
            return ret_list_htmlElem_from_collection(webBrowser.Document.GetElementsByTagName(tag_name));
        }
        public string getATtrValue(HtmlElement elem, string attr) {
            if (attr == "class") {
                attr = "className";
            }
            return elem.GetAttribute(attr);
        }
        public List<HtmlElement> get_elements_by_attr_value(string attr_name, string attr_value, HtmlElementCollection collect = null) {
            if (collect == null) {
                collect = webBrowser.Document.All;
            }
            if (attr_name == "class"){
                attr_name = "className";
            }
            List<HtmlElement> res = new List<HtmlElement>();
            foreach (HtmlElement elem in collect){ 
                if (elem.GetAttribute(attr_name) == attr_value) {
                    res.Add(elem);
                }
            }
            return res;
        }

        public void save(HtmlElement elem, string file_name) {
            System.IO.File.AppendAllText(file_name, elem.OuterText);
            //return elem.OuterText;
        }
        public void saveHTML(HtmlElement elem, string file_name) {
            System.IO.File.AppendAllText(file_name, elem.OuterHtml);
            //return elem.OuterHtml;
        }
//-----------------------------------------------------------------
//навигация
        public bool check_by_id(string elem_id) {
            HtmlElement elem;
            elem = webBrowser.Document.GetElementById(elem_id);
            if (elem != null)
                return true;
            else
                return false;
        }
        public bool check_by_text(string inner_text) {
            List<string> tagList = new List<string>();
            List<HtmlElement> elemList = new List<HtmlElement>();
            tagList.Add("a");
            tagList.Add("input");
            //tagList.Add("select");
            //tagList.Add("textarea");
            foreach (string tag in tagList) {
                elemList = ret_elements_by_innerText(tag, inner_text);
                if (elemList.Count > 0) {
                    return true;
                }
            }
            return false;
        }
//---------
        //click_by_id
        public void click_by_id(string elem_id) {
            HtmlElement elem;
            elem = webBrowser.Document.GetElementById(elem_id);
            if (elem != null)
                elem.InvokeMember("click");
        }
        public void click_elem(HtmlElement elem) {
            if (elem != null) {
                if (elem.TagName.ToLower() == "a") {
                    string addr = elem.GetAttribute("href");
                    if (!addr.StartsWith("http")) {
                        addr = elem.Document.Url.OriginalString + addr;                        
                    }
                    go(addr);
                }
                else {
                    elem.InvokeMember("click");
                }
            }
        }
        public void click_href(string inner_text) {
            HtmlElementCollection hec = webBrowser.Document.GetElementsByTagName("a");
            foreach (HtmlElement he in hec) {
                if (he.InnerText != null && he.InnerText.Equals(inner_text)) {
                    go(he.GetAttribute("href"));
                }
            }
        }
        public void click_by_text(string inner_text) {
            HtmlElement elem;
            HtmlElementCollection hec;
            List<HtmlElement> retElements = new List<HtmlElement>();
            string tag = "a";
            //поиск элемента с таким отображаемым текстом среди ссылок(тег a)
            retElements = ret_elements_by_innerText(tag, inner_text);
            elem = retElements[0]; //если возможно несколько вариантов, возьмем первый
            if (elem != null) {
                //go(elem.GetAttribute("href"));
                click_elem(elem);
                return;
            }
            //поиск среди кнопок
            else {
                tag = "input";
                hec = webBrowser.Document.GetElementsByTagName(tag);
                foreach (HtmlElement he in hec) {
                    if ((he.GetAttribute("type") == "button" || he.GetAttribute("type") == "submit") && he.GetAttribute("value").Equals(inner_text)) {
                        if (he.Id != null)
                            click_by_id(he.Id);
                        else
                            he.InvokeMember("click");
                        break;
                    }
                }
            }
        }
        public void go_back() {
            webBrowser.GoBack();
        }
//--------
        public void input_by_id(string dest_elem_id, string value) {
            HtmlElement elem;
            elem = webBrowser.Document.GetElementById(dest_elem_id);
            elem.SetAttribute("value", value);

        }
        public void input_elem(HtmlElement elem, string value) {
            elem.SetAttribute("value", value);
        }
        public void search(string search_field_id, string search_button_id, string search) {
            if (check_by_id(search_field_id)) {
                input_by_id(search_field_id, search);
                if (check_by_id(search_button_id)) {
                    click_by_id(search_button_id);
                }
            }

        }
//----------------------OPTIONS----------------------
        public void set_options_elem(HtmlElement elem, string option_name) {
            HtmlElementCollection optionCollection = elem.GetElementsByTagName("option");
            foreach (HtmlElement OptionElem in optionCollection) {
                if (OptionElem.InnerText == option_name) {
                    OptionElem.SetAttribute("selected", "selected");
                    break;
                }
            }
            optionCollection[0].SetAttribute("selected", null);
        }
        public string getChoseOption(HtmlElement elem) {
            HtmlElementCollection optionCollection = elem.GetElementsByTagName("option");
            foreach (HtmlElement OptionElem in optionCollection) {
                if (OptionElem.GetAttribute("selected") == "True") {
                    return OptionElem.GetAttribute("value");
                }
            }
            return "";
        }
        public void set_options(string inner_text) {
            HtmlElementCollection collect = webBrowser.Document.GetElementsByTagName("select");
            foreach (HtmlElement selectElem in collect) {
                if (selectElem.InnerHtml.IndexOf(inner_text) > 0) {
                    HtmlElementCollection optionCollection = selectElem.GetElementsByTagName("option");
                    foreach (HtmlElement OptionElem in optionCollection) {
                        if (OptionElem.InnerText == inner_text) {
                            OptionElem.SetAttribute("selected", "selected");
                            break;
                        }
                    }
                    optionCollection[0].SetAttribute("selected", null);
                    break;
                }
            }
        }
//--------------------CHECKBOX---------------
        public void set_check_box_elem(HtmlElement elem, bool flag) {
            if (elem.TagName != "input" && elem.GetAttribute("type") != "checkbox") {
                return;
            }
            if (flag) {
                elem.SetAttribute("checked", "True");
            }
            else {
                elem.SetAttribute("checked", null);
            }
        }
        public void set_check_box_flag(string inner_text, bool flag) {
            HtmlElementCollection collect = webBrowser.Document.GetElementsByTagName("input");
            foreach (HtmlElement elem in collect) {
                if (elem.GetAttribute("type") == "checkbox") {
                    if (elem.Parent.InnerHtml.IndexOf(inner_text) > 0) {
                        if (flag) {
                            elem.SetAttribute("checked", "True");
                        }
                        else {
                            elem.SetAttribute("checked", "False");
                            elem.SetAttribute("checked", null);
                        }
                        break;
                    }
                }
            }
        }
        public void click_check_box_flag(string inner_text) {
            HtmlElementCollection collect = webBrowser.Document.GetElementsByTagName("input");
            foreach (HtmlElement elem in collect) {
                if (elem.GetAttribute("type") == "checkbox" && elem.InnerText == inner_text) {
                    if (elem.GetAttribute("checked") == "True") {
                        elem.SetAttribute("checked", "False");
                    }
                    else {
                        elem.SetAttribute("checked", "True");
                    }
                }
            }
        }
        public void wait(int msec) {
            System.Threading.Thread.Sleep(msec);
        }
//-----------------------------------------------------
        public void go(string addr) {
            webBrowser.Navigate(addr);
        }

//--------
        public HtmlElementCollection getElemCollectByPath(string path) {
            HtmlElementCollection collect = null;
            return collect;
        }
//-----------------------------------------------------------------------
//------------------------вспомогательные функции------------------------
        string retCurAddress() {
            if (webBrowser.Document != null)
                return webBrowser.Url.OriginalString;
            else return "";
        }

        List<HtmlElement> ret_elements_by_innerText(string tag, string inner_text) {
            List<HtmlElement> retElements = new List<HtmlElement>();
            HtmlElementCollection hec = webBrowser.Document.GetElementsByTagName(tag);
            foreach (HtmlElement he in hec) {
                if (he.InnerText != null && he.InnerText.Equals(inner_text)) {
                    retElements.Add(he);
                }
            }
            return retElements;
        }
        List<HtmlElement> ret_list_htmlElem_from_collection(HtmlElementCollection collect) {
            List<HtmlElement> list_htmlElement = new List<HtmlElement>();
            for (int i = 0; i < collect.Count; i++) {
                list_htmlElement.Add(collect[i]);
            }
            return list_htmlElement;
        }
//-------------функции для протоколирования действий пользователя---------
        public string getTreeToElem(HtmlElement elem) {
            if (elem != null) {
                if (elem.TagName.ToLower() == "body" || elem.TagName.ToLower() == "html") {
                    return elem.TagName;
                }
                return getTreeToElem(elem.Parent) + "/" + elem.TagName;
            }
            else {
                return "";
            }
        }
//-------------обработчики событий начало---------------------------------        
        void contin_exec() {
            needExec.setFalse();
            firstNavigating = true;
            //this.intrpr.print(">>> execution continue <<<");              
            exit_run++;
            Raise_contin_exec_event(this, null);
            //intrpr.continue_execute();
        }
        private void webBrowser_documentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
            //MessageBox.Show("documentCompleted");
            if (e.Url.AbsolutePath != (sender as WebBrowser).Url.AbsolutePath ||
                webBrowser.ReadyState != WebBrowserReadyState.Complete) {// ||
                //webBrowser.StatusText.ToLower() != "готово") {
                    
                    return;
                //Application.DoEvents();
            }
            //while (webBrowser.StatusText.ToLower() != "готово" || new_title != true) {
            //    Application.DoEvents();
            //}
            if (e.Url.AbsolutePath != "blank") {
                needExec.page_downloaded = true;
            }
            if (needExec.allDone()) {
                contin_exec();
            }
            if (e.Url.AbsolutePath != "blank") {
                
/*                firstNavigating = true;
                this.intrpr.print(">>> execution continue <<<");
                //MessageBox.Show(">>> execution continue <<<");               
                exit_run++;
                intrpr.continue_execute();
*/            }
            else {//Document только инициализировался
                //Отключим cookie
                if (webBrowser.Document.Cookie!=null)
                    webBrowser.Document.Cookie.Remove(0, webBrowser.Document.Cookie.Length);
                
            }
            
        }
        void title_changed(Object sender, EventArgs e) {
            if (webBrowser.DocumentTitle == "") {
                return;
            }
            needExec.new_title = true;
            if (needExec.allDone()) {
                contin_exec();
            }
            //MessageBox.Show(this.webBrowser.DocumentTitle);
        }

        private void webBrowser_navigating(object sender, WebBrowserNavigatingEventArgs e) {
            //MessageBox.Show("navigating is ok");

        }
        private void webBrowser_navigated(object sender, WebBrowserNavigatedEventArgs e) {
            //MessageBox.Show("navigated is ok");
            //need_invalidate = false;

        }
        private void Web_V1_NewWindow(string URL, int Flags, string TargetFrameName, ref object PostData, string Headers, ref bool Processed) {
            Processed = true; //Stop event from being processed
            //Code to open in same window
            this.webBrowser.Navigate(URL);
            MessageBox.Show(URL);
        }
        //-------------обработчики событий конец--------------------------------- 

    }
    class NeedExec {
        public bool new_title;
        public bool page_downloaded;
        public NeedExec() {
            new_title = false;
            page_downloaded = false;
        }
        public bool allDone() {
            return new_title && page_downloaded;
        }
        public void setFalse(){
            new_title = page_downloaded = false;
        }
    }
}
