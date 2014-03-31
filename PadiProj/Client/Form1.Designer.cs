namespace Client
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.URLMaster = new System.Windows.Forms.TextBox();
            this.Connection = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ClientPort = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // URLMaster
            // 
            this.URLMaster.Location = new System.Drawing.Point(23, 30);
            this.URLMaster.Name = "URLMaster";
            this.URLMaster.Size = new System.Drawing.Size(337, 20);
            this.URLMaster.TabIndex = 0;
            this.URLMaster.TextChanged += new System.EventHandler(this.URLMaster_TextChanged);
            // 
            // Connection
            // 
            this.Connection.Location = new System.Drawing.Point(146, 57);
            this.Connection.Name = "Connection";
            this.Connection.Size = new System.Drawing.Size(101, 37);
            this.Connection.TabIndex = 1;
            this.Connection.Text = "Start Connection";
            this.Connection.UseVisualStyleBackColor = true;
            this.Connection.Click += new System.EventHandler(this.Connection_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Url Master";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Client Port";
            // 
            // ClientPort
            // 
            this.ClientPort.Location = new System.Drawing.Point(23, 74);
            this.ClientPort.Name = "ClientPort";
            this.ClientPort.Size = new System.Drawing.Size(100, 20);
            this.ClientPort.TabIndex = 4;
            this.ClientPort.TextChanged += new System.EventHandler(this.ClientPort_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(372, 344);
            this.Controls.Add(this.ClientPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Connection);
            this.Controls.Add(this.URLMaster);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox URLMaster;
        private System.Windows.Forms.Button Connection;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ClientPort;
    }
}

