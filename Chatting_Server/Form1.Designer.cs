namespace Chatting_Server
{
    partial class Form1
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
            btn_server = new Button();
            lb_server = new Label();
            txtChatMsg = new TextBox();
            SuspendLayout();
            // 
            // btn_server
            // 
            btn_server.Location = new Point(291, 339);
            btn_server.Name = "btn_server";
            btn_server.Size = new Size(163, 77);
            btn_server.TabIndex = 0;
            btn_server.Text = "서버 시작";
            btn_server.UseVisualStyleBackColor = true;
            btn_server.Click += btn_server_Click;
            // 
            // lb_server
            // 
            lb_server.AutoSize = true;
            lb_server.Font = new Font("맑은 고딕", 20F, FontStyle.Regular, GraphicsUnit.Point);
            lb_server.Location = new Point(24, 361);
            lb_server.Name = "lb_server";
            lb_server.Size = new Size(192, 37);
            lb_server.TabIndex = 1;
            lb_server.Tag = "stop";
            lb_server.Text = "Server 중지 됨";
            // 
            // txtChatMsg
            // 
            txtChatMsg.Location = new Point(24, 20);
            txtChatMsg.Multiline = true;
            txtChatMsg.Name = "txtChatMsg";
            txtChatMsg.ScrollBars = ScrollBars.Vertical;
            txtChatMsg.Size = new Size(430, 300);
            txtChatMsg.TabIndex = 3;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(485, 450);
            Controls.Add(txtChatMsg);
            Controls.Add(lb_server);
            Controls.Add(btn_server);
            Name = "Form1";
            Text = "Form1";
            FormClosed += Form1_FormClosed;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btn_server;
        private Label lb_server;
        private TextBox txtChatMsg;
    }
}