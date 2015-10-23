namespace SecuruStik.UIDesign
{
    partial class ShareForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param pathFullName="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button_OK = new DevComponents.DotNetBar.ButtonX();
            this.textBoxX_Email = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.labelX_Email = new DevComponents.DotNetBar.LabelX();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // button_OK
            // 
            this.button_OK.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.button_OK.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.button_OK.Location = new System.Drawing.Point(198, 80);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new System.Drawing.Size(75, 23);
            this.button_OK.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.button_OK.TabIndex = 0;
            this.button_OK.Text = "OK";
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // textBoxX_Email
            // 
            this.textBoxX_Email.BackColor = System.Drawing.Color.White;
            // 
            // 
            // 
            this.textBoxX_Email.Border.Class = "TextBoxBorder";
            this.textBoxX_Email.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.textBoxX_Email.ButtonCustom.Tooltip = "";
            this.textBoxX_Email.ButtonCustom2.Tooltip = "";
            this.textBoxX_Email.DisabledBackColor = System.Drawing.Color.White;
            this.textBoxX_Email.ForeColor = System.Drawing.Color.Black;
            this.textBoxX_Email.Location = new System.Drawing.Point(21, 41);
            this.textBoxX_Email.Name = "textBoxX_Email";
            this.textBoxX_Email.PreventEnterBeep = true;
            this.textBoxX_Email.Size = new System.Drawing.Size(252, 22);
            this.textBoxX_Email.TabIndex = 1;
            // 
            // button_Cancel
            // 
            this.button_Cancel.BackColor = System.Drawing.Color.White;
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Cancel.ForeColor = System.Drawing.Color.Black;
            this.button_Cancel.Location = new System.Drawing.Point(117, 80);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(75, 23);
            this.button_Cancel.TabIndex = 3;
            this.button_Cancel.Text = "Cancel";
            this.button_Cancel.UseVisualStyleBackColor = false;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // labelX_Email
            // 
            this.labelX_Email.BackColor = System.Drawing.Color.White;
            // 
            // 
            // 
            this.labelX_Email.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX_Email.ForeColor = System.Drawing.Color.Black;
            this.labelX_Email.Location = new System.Drawing.Point(21, 12);
            this.labelX_Email.Name = "labelX_Email";
            this.labelX_Email.Size = new System.Drawing.Size(42, 23);
            this.labelX_Email.TabIndex = 4;
            this.labelX_Email.Text = "<b>Email :</b>";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // ShareForm
            // 
            this.AcceptButton = this.button_OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button_Cancel;
            this.ClientSize = new System.Drawing.Size(298, 113);
            this.ControlBox = false;
            this.Controls.Add(this.labelX_Email);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.textBoxX_Email);
            this.Controls.Add(this.button_OK);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ShareForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Share file";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ShareForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.ButtonX button_OK;
        private DevComponents.DotNetBar.Controls.TextBoxX textBoxX_Email;
        private System.Windows.Forms.Button button_Cancel;
        private DevComponents.DotNetBar.LabelX labelX_Email;
        private System.Windows.Forms.ErrorProvider errorProvider1;
    }
}