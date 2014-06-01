using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using diplomWorkTranslator;

namespace diplom_project {
    public partial class Step_execution_form : Form {
        //Form father_form;
        //WB wb;
        Interpretator intrpr;
        //WebBrowser wb_on_form;
        Command_history cmd_history;
        bool isFocuseHandled;
        Protocol_of_user_operation protocol;
        string prevElemTag;
        public Step_execution_form() {
            isFocuseHandled = false;
            prevElemTag = "";
            InitializeComponent();
            //father_form = form;
            
            //wb = new WB("");
            output.Text = "";
            cmd_history = new Command_history();         
            //initialize_browser();
            
            this.FormClosing += new FormClosingEventHandler(window_close);
            //this.input.KeyPress += new KeyPressEventHandler(key_on_input_was_pressed);
            input.KeyDown += new KeyEventHandler(key_on_input_was_pressed);
            runScript.Hide();
        }
        //---------------------------
        private void initialize_browser(string prog) {
            //wb = new WB(prog);
            intrpr = new Interpretator(prog);
            groupBox1.Controls.Add(intrpr.wb.webBrowser);
            intrpr.wb.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            intrpr.wb.webBrowser.TabIndex = 1;
            if (intrpr.wb.webBrowser.Document == null) {
                intrpr.wb.go("about:blank");
            }
            intrpr.wb.webBrowser.Document.Click += new HtmlElementEventHandler(click_handle);
            intrpr.wb.webBrowser.Navigating += new WebBrowserNavigatingEventHandler(webBrowser_navigating);
            protocol = new Protocol_of_user_operation(history_out, intrpr);
        }
        private void webBrowser_navigating(object sender, WebBrowserNavigatingEventArgs e) {
            //MessageBox.Show(wb.webBrowser.TabIndex.ToString());

            if (intrpr.wb.firstNavigating) {
                if (protocol.elemAfterFocuse != null) {
                    protocol.save_user_click();
                }
                if (isFocuseHandled) {
                    intrpr.wb.webBrowser.Document.LosingFocus -= new HtmlElementEventHandler(lose_focus);
                    isFocuseHandled = false;
                }
                intrpr.wb.firstNavigating = false;
            }
        }
        void click_handle(Object sender, HtmlElementEventArgs e) {
            if (intrpr.wb.webBrowser.Document != null) {
                HtmlElement elem = intrpr.wb.webBrowser.Document.GetElementFromPoint(e.ClientMousePosition);
                //print_in_output(wb.getTreeToElem(elem) + '\n');
                //если клик произошел после выбораэлемента select, то его надо не учитывать
                if (prevElemTag == "select") {
                    prevElemTag = elem.TagName.ToLower();
                    return;
                }
                if (elem != null) {
                    protocol.findElem(elem);
                    protocol.saveElem(elem);
                    prevElemTag = elem.TagName.ToLower();
                    //protocol.save_user_click();
                }

            }
            if (!isFocuseHandled) {
                intrpr.wb.webBrowser.Document.LosingFocus += new HtmlElementEventHandler(lose_focus);
                isFocuseHandled = true;
            }
        }
        void lose_focus(Object sender, HtmlElementEventArgs e) {
            //MessageBox.Show("focus was loosed");
            //HtmlElement elemFrom = e.FromElement ;//= wb.webBrowser.Document.GetElementFromPoint(e.ClientMousePosition);
            protocol.save_user_action();
            
/*            HtmlElement elemTo = e.ToElement;
            if (elemTo != null) {
                elemTo.InvokeMember("click");
            }
 */
        }

        //---------------------------
        private void window_close(object sender, FormClosingEventArgs e) {
            //father_form.Show();
        }
        private void step_execution_form_Load(object sender, EventArgs e) {
        }

        void key_on_input_was_pressed(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter){// && e.Control) {
                string prog = input.Text;
                string add = " ";
                input.Text = "";
                if (prog != "") {
                    cmd_history.add(prog);
                    if (prog[prog.Length - 1] != ';' && prog[prog.Length - 1] != ' ') {
                        add = "; ";
                    }
                    prog += add;
                    print_in_output("<<< " + prog + '\n');
                }
                try {
                    if (intrpr != null) {
                        intrpr.step_execute(prog);
                    }
                    else {
                        initialize_browser("");
                        intrpr.interpretation(input, output);
                        intrpr.step_execute(prog);
                    }
                }
                catch (Exception ex) {
                    print_in_output("error: " + ex.Message + '\n');
                }
            }
            else if (e.KeyCode == Keys.Up) {
                input.Text = cmd_history.getPrev();
            }
            else if (e.KeyCode == Keys.Down) {
                input.Text = cmd_history.getNext();
            }
        }

        void print_in_output(string str) {
            output.Text += str;
            output.SelectionStart = output.Text.Length;
            output.ScrollToCaret();
        }

        private void loadScript_Click(object sender, EventArgs e) {
            OpenFileDialog fileName = new OpenFileDialog();
            if (fileName.ShowDialog() == DialogResult.OK) {
                scriptPath.Text = fileName.FileName;
                runScript.Show();
            }
        }

        private void runScript_Click(object sender, EventArgs e) {
            System.IO.StreamReader sr = new System.IO.StreamReader(scriptPath.Text, Encoding.GetEncoding(1251));
            System.IO.StringReader file = new System.IO.StringReader(sr.ReadToEnd());
            string prog = file.ReadToEnd();
            sr.Close();
            file.Close();
            try {
                if (intrpr != null) {
                    intrpr.step_execute(prog);
                }
                else {
                    initialize_browser(scriptPath.Text);
                    intrpr.interpretation(input, output);
                    //wb.step_execute(prog);
                }
            }
            catch (Exception ex) {
                print_in_output("error: " + ex.Message + '\n');
            }
        }
    }
}
