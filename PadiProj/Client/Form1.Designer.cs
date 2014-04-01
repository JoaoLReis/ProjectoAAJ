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
            this.padIntBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.createPadInt = new System.Windows.Forms.Button();
            this.idPadIntBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.transactionWrite = new System.Windows.Forms.Button();
            this.transactionRead = new System.Windows.Forms.Button();
            this.transactionSend = new System.Windows.Forms.Button();
            this.receiveMessages = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.valueBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
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
            this.Connection.Location = new System.Drawing.Point(22, 113);
            this.Connection.Name = "Connection";
            this.Connection.Size = new System.Drawing.Size(338, 24);
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
            this.label2.Location = new System.Drawing.Point(20, 58);
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
            // padIntBox
            // 
            this.padIntBox.Location = new System.Drawing.Point(23, 312);
            this.padIntBox.Name = "padIntBox";
            this.padIntBox.Size = new System.Drawing.Size(100, 20);
            this.padIntBox.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 288);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Create PadInt";
            // 
            // createPadInt
            // 
            this.createPadInt.Location = new System.Drawing.Point(141, 309);
            this.createPadInt.Name = "createPadInt";
            this.createPadInt.Size = new System.Drawing.Size(75, 23);
            this.createPadInt.TabIndex = 7;
            this.createPadInt.Text = "Create";
            this.createPadInt.UseVisualStyleBackColor = true;
            this.createPadInt.Click += new System.EventHandler(this.createPadInt_Click);
            // 
            // idPadIntBox
            // 
            this.idPadIntBox.Location = new System.Drawing.Point(25, 184);
            this.idPadIntBox.Name = "idPadIntBox";
            this.idPadIntBox.Size = new System.Drawing.Size(75, 20);
            this.idPadIntBox.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 165);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Id PadInt";
            // 
            // transactionWrite
            // 
            this.transactionWrite.Location = new System.Drawing.Point(25, 211);
            this.transactionWrite.Name = "transactionWrite";
            this.transactionWrite.Size = new System.Drawing.Size(75, 23);
            this.transactionWrite.TabIndex = 10;
            this.transactionWrite.Text = "Write";
            this.transactionWrite.UseVisualStyleBackColor = true;
            this.transactionWrite.Click += new System.EventHandler(this.transactionWrite_Click);
            // 
            // transactionRead
            // 
            this.transactionRead.Location = new System.Drawing.Point(106, 211);
            this.transactionRead.Name = "transactionRead";
            this.transactionRead.Size = new System.Drawing.Size(75, 23);
            this.transactionRead.TabIndex = 11;
            this.transactionRead.Text = "Read";
            this.transactionRead.UseVisualStyleBackColor = true;
            this.transactionRead.Click += new System.EventHandler(this.transactionRead_Click);
            // 
            // transactionSend
            // 
            this.transactionSend.Location = new System.Drawing.Point(25, 240);
            this.transactionSend.Name = "transactionSend";
            this.transactionSend.Size = new System.Drawing.Size(75, 23);
            this.transactionSend.TabIndex = 12;
            this.transactionSend.Text = "Send";
            this.transactionSend.UseVisualStyleBackColor = true;
            this.transactionSend.Click += new System.EventHandler(this.transactionSend_Click);
            // 
            // receiveMessages
            // 
            this.receiveMessages.Location = new System.Drawing.Point(245, 184);
            this.receiveMessages.Multiline = true;
            this.receiveMessages.Name = "receiveMessages";
            this.receiveMessages.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.receiveMessages.Size = new System.Drawing.Size(115, 148);
            this.receiveMessages.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(242, 165);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Messages";
            // 
            // valueBox
            // 
            this.valueBox.Location = new System.Drawing.Point(107, 184);
            this.valueBox.Name = "valueBox";
            this.valueBox.Size = new System.Drawing.Size(74, 20);
            this.valueBox.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(107, 165);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Value";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(372, 344);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.valueBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.receiveMessages);
            this.Controls.Add(this.transactionSend);
            this.Controls.Add(this.transactionRead);
            this.Controls.Add(this.transactionWrite);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.idPadIntBox);
            this.Controls.Add(this.createPadInt);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.padIntBox);
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
        private System.Windows.Forms.TextBox padIntBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button createPadInt;
        private System.Windows.Forms.TextBox idPadIntBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button transactionWrite;
        private System.Windows.Forms.Button transactionRead;
        private System.Windows.Forms.Button transactionSend;
        private System.Windows.Forms.TextBox receiveMessages;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox valueBox;
        private System.Windows.Forms.Label label6;
    }
}

