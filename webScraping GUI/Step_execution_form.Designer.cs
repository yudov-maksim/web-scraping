namespace diplom_project {
    partial class Step_execution_form {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.output = new System.Windows.Forms.RichTextBox();
            this.input = new System.Windows.Forms.TextBox();
            this.history_out = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.scriptPath = new System.Windows.Forms.TextBox();
            this.loadScript = new System.Windows.Forms.Button();
            this.runScript = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1012, 293);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Web Browser";
            // 
            // output
            // 
            this.output.Location = new System.Drawing.Point(12, 374);
            this.output.Name = "output";
            this.output.ReadOnly = true;
            this.output.Size = new System.Drawing.Size(526, 144);
            this.output.TabIndex = 2;
            this.output.Text = "";
            // 
            // input
            // 
            this.input.Location = new System.Drawing.Point(12, 542);
            this.input.Name = "input";
            this.input.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.input.Size = new System.Drawing.Size(526, 20);
            this.input.TabIndex = 0;
            // 
            // history_out
            // 
            this.history_out.Location = new System.Drawing.Point(544, 374);
            this.history_out.Name = "history_out";
            this.history_out.ReadOnly = true;
            this.history_out.Size = new System.Drawing.Size(481, 143);
            this.history_out.TabIndex = 3;
            this.history_out.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 358);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Вывод приложения";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(541, 358);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(168, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Запись действий пользователя";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 526);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Окно ввода команд";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 315);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Скрипт";
            // 
            // scriptPath
            // 
            this.scriptPath.Location = new System.Drawing.Point(13, 335);
            this.scriptPath.Name = "scriptPath";
            this.scriptPath.ReadOnly = true;
            this.scriptPath.Size = new System.Drawing.Size(386, 20);
            this.scriptPath.TabIndex = 8;
            // 
            // loadScript
            // 
            this.loadScript.Location = new System.Drawing.Point(431, 328);
            this.loadScript.Name = "loadScript";
            this.loadScript.Size = new System.Drawing.Size(113, 26);
            this.loadScript.TabIndex = 9;
            this.loadScript.Text = "Загрузить";
            this.loadScript.UseVisualStyleBackColor = true;
            this.loadScript.Click += new System.EventHandler(this.loadScript_Click);
            // 
            // runScript
            // 
            this.runScript.Location = new System.Drawing.Point(572, 328);
            this.runScript.Name = "runScript";
            this.runScript.Size = new System.Drawing.Size(136, 25);
            this.runScript.TabIndex = 10;
            this.runScript.Text = "Выполнить";
            this.runScript.UseVisualStyleBackColor = true;
            this.runScript.Click += new System.EventHandler(this.runScript_Click);
            // 
            // Step_execution_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1036, 574);
            this.Controls.Add(this.runScript);
            this.Controls.Add(this.loadScript);
            this.Controls.Add(this.scriptPath);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.history_out);
            this.Controls.Add(this.input);
            this.Controls.Add(this.output);
            this.Controls.Add(this.groupBox1);
            this.Name = "Step_execution_form";
            this.Load += new System.EventHandler(this.step_execution_form_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox output;
        private System.Windows.Forms.TextBox input;
        private System.Windows.Forms.RichTextBox history_out;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox scriptPath;
        private System.Windows.Forms.Button loadScript;
        private System.Windows.Forms.Button runScript;
    }
}