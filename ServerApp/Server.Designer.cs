    namespace ServerApp
    {
        partial class Server
        {
            /// <summary>
            ///  Required designer variable.
            /// </summary>
            private System.ComponentModel.IContainer components = null;

            /// <summary>
            ///  Clean up any resources being used.
            /// </summary>
            /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
            protected override void Dispose(bool disposing)
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                base.Dispose(disposing);
            }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtLogs = new RichTextBox();
            label3 = new Label();
            SuspendLayout();
            // 
            // txtLogs
            // 
            txtLogs.Location = new Point(284, 228);
            txtLogs.Name = "txtLogs";
            txtLogs.Size = new Size(445, 95);
            txtLogs.TabIndex = 6;
            txtLogs.Text = "";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(150, 265);
            label3.Name = "label3";
            label3.Size = new Size(50, 25);
            label3.TabIndex = 8;
            label3.Text = "Logs";
            // 
            // Server
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1061, 616);
            Controls.Add(label3);
            Controls.Add(txtLogs);
            Name = "Server";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private RichTextBox txtLogs;
            private Label label3;
        }
    }
