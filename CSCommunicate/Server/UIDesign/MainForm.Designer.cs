namespace Server
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param pathFullName="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.button_Listenning = new DevComponents.DotNetBar.ButtonX();
            this.listBox_ClientList = new System.Windows.Forms.ListBox();
            this.groupBox_Client = new System.Windows.Forms.GroupBox();
            this.listBox_Log = new System.Windows.Forms.ListBox();
            this.buttonX2 = new DevComponents.DotNetBar.ButtonX();
            this.groupBox_Log = new System.Windows.Forms.GroupBox();
            this.groupBox_Client.SuspendLayout();
            this.groupBox_Log.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_Listenning
            // 
            this.button_Listenning.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.button_Listenning.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.button_Listenning.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button_Listenning.FocusCuesEnabled = false;
            this.button_Listenning.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Listenning.Location = new System.Drawing.Point(0, 483);
            this.button_Listenning.Name = "button_Listenning";
            this.button_Listenning.Size = new System.Drawing.Size(658, 42);
            this.button_Listenning.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.button_Listenning.Symbol = "";
            this.button_Listenning.SymbolSize = 12F;
            this.button_Listenning.TabIndex = 0;
            this.button_Listenning.Text = "Start";
            this.button_Listenning.Click += new System.EventHandler(this.button_Begin_Stop_Listening_Click);
            // 
            // listBox_ClientList
            // 
            this.listBox_ClientList.BackColor = System.Drawing.Color.White;
            this.listBox_ClientList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox_ClientList.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox_ClientList.ForeColor = System.Drawing.Color.Black;
            this.listBox_ClientList.FormattingEnabled = true;
            this.listBox_ClientList.ItemHeight = 15;
            this.listBox_ClientList.Location = new System.Drawing.Point(3, 16);
            this.listBox_ClientList.Name = "listBox_ClientList";
            this.listBox_ClientList.Size = new System.Drawing.Size(629, 162);
            this.listBox_ClientList.TabIndex = 1;
            // 
            // groupBox_Client
            // 
            this.groupBox_Client.BackColor = System.Drawing.Color.White;
            this.groupBox_Client.Controls.Add(this.listBox_ClientList);
            this.groupBox_Client.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox_Client.ForeColor = System.Drawing.Color.Black;
            this.groupBox_Client.Location = new System.Drawing.Point(12, 4);
            this.groupBox_Client.Name = "groupBox_Client";
            this.groupBox_Client.Size = new System.Drawing.Size(635, 181);
            this.groupBox_Client.TabIndex = 2;
            this.groupBox_Client.TabStop = false;
            this.groupBox_Client.Text = "Client";
            // 
            // listBox_Log
            // 
            this.listBox_Log.BackColor = System.Drawing.Color.White;
            this.listBox_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox_Log.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox_Log.ForeColor = System.Drawing.Color.Black;
            this.listBox_Log.FormattingEnabled = true;
            this.listBox_Log.HorizontalScrollbar = true;
            this.listBox_Log.ItemHeight = 15;
            this.listBox_Log.Location = new System.Drawing.Point(3, 16);
            this.listBox_Log.Name = "listBox_Log";
            this.listBox_Log.Size = new System.Drawing.Size(629, 228);
            this.listBox_Log.TabIndex = 3;
            // 
            // buttonX2
            // 
            this.buttonX2.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonX2.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonX2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonX2.FocusCuesEnabled = false;
            this.buttonX2.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonX2.Location = new System.Drawing.Point(3, 244);
            this.buttonX2.Name = "buttonX2";
            this.buttonX2.Size = new System.Drawing.Size(629, 39);
            this.buttonX2.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonX2.Symbol = "";
            this.buttonX2.SymbolSize = 12F;
            this.buttonX2.TabIndex = 0;
            this.buttonX2.Text = "Clear";
            this.buttonX2.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // groupBox_Log
            // 
            this.groupBox_Log.BackColor = System.Drawing.Color.White;
            this.groupBox_Log.Controls.Add(this.listBox_Log);
            this.groupBox_Log.Controls.Add(this.buttonX2);
            this.groupBox_Log.ForeColor = System.Drawing.Color.Black;
            this.groupBox_Log.Location = new System.Drawing.Point(12, 191);
            this.groupBox_Log.Name = "groupBox_Log";
            this.groupBox_Log.Size = new System.Drawing.Size(635, 286);
            this.groupBox_Log.TabIndex = 4;
            this.groupBox_Log.TabStop = false;
            this.groupBox_Log.Text = "Log";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(658, 525);
            this.Controls.Add(this.groupBox_Log);
            this.Controls.Add(this.button_Listenning);
            this.Controls.Add(this.groupBox_Client);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TitleText = "Server";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox_Client.ResumeLayout(false);
            this.groupBox_Log.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.ButtonX button_Listenning;
        private System.Windows.Forms.ListBox listBox_ClientList;
        private System.Windows.Forms.GroupBox groupBox_Client;
        private System.Windows.Forms.ListBox listBox_Log;
        private DevComponents.DotNetBar.ButtonX buttonX2;
        private System.Windows.Forms.GroupBox groupBox_Log;
    }
}

