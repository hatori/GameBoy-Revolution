namespace GameBoy_Revolution
{
    partial class Debugger
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
            this.btnClose = new System.Windows.Forms.Button();
            this.lblPrevOpcode = new System.Windows.Forms.Label();
            this.txtPrevOpcode = new System.Windows.Forms.TextBox();
            this.txtCurrOpcode = new System.Windows.Forms.TextBox();
            this.lblCurrOpcode = new System.Windows.Forms.Label();
            this.txtCurrCycles = new System.Windows.Forms.TextBox();
            this.lblCurrCucles = new System.Windows.Forms.Label();
            this.txtAF = new System.Windows.Forms.TextBox();
            this.lblAF = new System.Windows.Forms.Label();
            this.txtBC = new System.Windows.Forms.TextBox();
            this.lblBC = new System.Windows.Forms.Label();
            this.txtDE = new System.Windows.Forms.TextBox();
            this.lblDE = new System.Windows.Forms.Label();
            this.txtHL = new System.Windows.Forms.TextBox();
            this.lblHL = new System.Windows.Forms.Label();
            this.txtPC = new System.Windows.Forms.TextBox();
            this.lblPC = new System.Windows.Forms.Label();
            this.txtSP = new System.Windows.Forms.TextBox();
            this.lblSP = new System.Windows.Forms.Label();
            this.lblCPUStatus = new System.Windows.Forms.Label();
            this.btnStep = new System.Windows.Forms.Button();
            this.lblBreakpoint = new System.Windows.Forms.Label();
            this.txtBreakpoint = new System.Windows.Forms.TextBox();
            this.btnRun = new System.Windows.Forms.Button();
            this.lblIF = new System.Windows.Forms.Label();
            this.txtIF = new System.Windows.Forms.TextBox();
            this.lblIE = new System.Windows.Forms.Label();
            this.txtIE = new System.Windows.Forms.TextBox();
            this.chkZ = new System.Windows.Forms.CheckBox();
            this.chkN = new System.Windows.Forms.CheckBox();
            this.chkH = new System.Windows.Forms.CheckBox();
            this.chkC = new System.Windows.Forms.CheckBox();
            this.lblIMEStatus = new System.Windows.Forms.Label();
            this.chkIMEEnabled = new System.Windows.Forms.CheckBox();
            this.lblMemory = new System.Windows.Forms.Label();
            this.txtMemory = new System.Windows.Forms.TextBox();
            this.cmbMemoryType = new System.Windows.Forms.ComboBox();
            this.lblMemoryType = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.btnClose.Location = new System.Drawing.Point(721, 319);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblPrevOpcode
            // 
            this.lblPrevOpcode.AutoSize = true;
            this.lblPrevOpcode.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.lblPrevOpcode.Location = new System.Drawing.Point(12, 9);
            this.lblPrevOpcode.Name = "lblPrevOpcode";
            this.lblPrevOpcode.Size = new System.Drawing.Size(126, 14);
            this.lblPrevOpcode.TabIndex = 37;
            this.lblPrevOpcode.Text = "Previous Opcode: ";
            // 
            // txtPrevOpcode
            // 
            this.txtPrevOpcode.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.txtPrevOpcode.Location = new System.Drawing.Point(144, 6);
            this.txtPrevOpcode.Name = "txtPrevOpcode";
            this.txtPrevOpcode.Size = new System.Drawing.Size(169, 20);
            this.txtPrevOpcode.TabIndex = 36;
            // 
            // txtCurrOpcode
            // 
            this.txtCurrOpcode.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.txtCurrOpcode.Location = new System.Drawing.Point(444, 6);
            this.txtCurrOpcode.Name = "txtCurrOpcode";
            this.txtCurrOpcode.Size = new System.Drawing.Size(169, 20);
            this.txtCurrOpcode.TabIndex = 34;
            // 
            // lblCurrOpcode
            // 
            this.lblCurrOpcode.AutoSize = true;
            this.lblCurrOpcode.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.lblCurrOpcode.Location = new System.Drawing.Point(319, 9);
            this.lblCurrOpcode.Name = "lblCurrOpcode";
            this.lblCurrOpcode.Size = new System.Drawing.Size(119, 14);
            this.lblCurrOpcode.TabIndex = 35;
            this.lblCurrOpcode.Text = "Current Opcode: ";
            // 
            // txtCurrCycles
            // 
            this.txtCurrCycles.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.txtCurrCycles.Location = new System.Drawing.Point(144, 33);
            this.txtCurrCycles.Name = "txtCurrCycles";
            this.txtCurrCycles.Size = new System.Drawing.Size(100, 20);
            this.txtCurrCycles.TabIndex = 30;
            // 
            // lblCurrCucles
            // 
            this.lblCurrCucles.AutoSize = true;
            this.lblCurrCucles.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.lblCurrCucles.Location = new System.Drawing.Point(12, 35);
            this.lblCurrCucles.Name = "lblCurrCucles";
            this.lblCurrCucles.Size = new System.Drawing.Size(119, 14);
            this.lblCurrCucles.TabIndex = 31;
            this.lblCurrCucles.Text = "Current Cycles: ";
            // 
            // txtAF
            // 
            this.txtAF.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.txtAF.Location = new System.Drawing.Point(590, 130);
            this.txtAF.Name = "txtAF";
            this.txtAF.Size = new System.Drawing.Size(44, 20);
            this.txtAF.TabIndex = 17;
            // 
            // lblAF
            // 
            this.lblAF.AutoSize = true;
            this.lblAF.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.lblAF.Location = new System.Drawing.Point(549, 133);
            this.lblAF.Name = "lblAF";
            this.lblAF.Size = new System.Drawing.Size(35, 14);
            this.lblAF.TabIndex = 18;
            this.lblAF.Text = "AF: ";
            // 
            // txtBC
            // 
            this.txtBC.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.txtBC.Location = new System.Drawing.Point(590, 156);
            this.txtBC.Name = "txtBC";
            this.txtBC.Size = new System.Drawing.Size(44, 20);
            this.txtBC.TabIndex = 15;
            // 
            // lblBC
            // 
            this.lblBC.AutoSize = true;
            this.lblBC.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.lblBC.Location = new System.Drawing.Point(549, 159);
            this.lblBC.Name = "lblBC";
            this.lblBC.Size = new System.Drawing.Size(35, 14);
            this.lblBC.TabIndex = 16;
            this.lblBC.Text = "BC: ";
            // 
            // txtDE
            // 
            this.txtDE.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.txtDE.Location = new System.Drawing.Point(590, 182);
            this.txtDE.Name = "txtDE";
            this.txtDE.Size = new System.Drawing.Size(44, 20);
            this.txtDE.TabIndex = 13;
            // 
            // lblDE
            // 
            this.lblDE.AutoSize = true;
            this.lblDE.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.lblDE.Location = new System.Drawing.Point(549, 185);
            this.lblDE.Name = "lblDE";
            this.lblDE.Size = new System.Drawing.Size(35, 14);
            this.lblDE.TabIndex = 14;
            this.lblDE.Text = "DE: ";
            // 
            // txtHL
            // 
            this.txtHL.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.txtHL.Location = new System.Drawing.Point(682, 130);
            this.txtHL.Name = "txtHL";
            this.txtHL.Size = new System.Drawing.Size(44, 20);
            this.txtHL.TabIndex = 9;
            // 
            // lblHL
            // 
            this.lblHL.AutoSize = true;
            this.lblHL.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.lblHL.Location = new System.Drawing.Point(641, 133);
            this.lblHL.Name = "lblHL";
            this.lblHL.Size = new System.Drawing.Size(35, 14);
            this.lblHL.TabIndex = 10;
            this.lblHL.Text = "HL: ";
            // 
            // txtPC
            // 
            this.txtPC.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.txtPC.Location = new System.Drawing.Point(682, 182);
            this.txtPC.Name = "txtPC";
            this.txtPC.Size = new System.Drawing.Size(44, 20);
            this.txtPC.TabIndex = 5;
            // 
            // lblPC
            // 
            this.lblPC.AutoSize = true;
            this.lblPC.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.lblPC.Location = new System.Drawing.Point(641, 185);
            this.lblPC.Name = "lblPC";
            this.lblPC.Size = new System.Drawing.Size(35, 14);
            this.lblPC.TabIndex = 6;
            this.lblPC.Text = "PC: ";
            // 
            // txtSP
            // 
            this.txtSP.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.txtSP.Location = new System.Drawing.Point(682, 156);
            this.txtSP.Name = "txtSP";
            this.txtSP.Size = new System.Drawing.Size(44, 20);
            this.txtSP.TabIndex = 7;
            // 
            // lblSP
            // 
            this.lblSP.AutoSize = true;
            this.lblSP.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.lblSP.Location = new System.Drawing.Point(641, 159);
            this.lblSP.Name = "lblSP";
            this.lblSP.Size = new System.Drawing.Size(35, 14);
            this.lblSP.TabIndex = 8;
            this.lblSP.Text = "SP: ";
            // 
            // lblCPUStatus
            // 
            this.lblCPUStatus.AutoSize = true;
            this.lblCPUStatus.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.lblCPUStatus.Location = new System.Drawing.Point(546, 84);
            this.lblCPUStatus.Name = "lblCPUStatus";
            this.lblCPUStatus.Size = new System.Drawing.Size(91, 14);
            this.lblCPUStatus.TabIndex = 25;
            this.lblCPUStatus.Text = "CPU Status: ";
            // 
            // btnStep
            // 
            this.btnStep.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.btnStep.Location = new System.Drawing.Point(721, 32);
            this.btnStep.Name = "btnStep";
            this.btnStep.Size = new System.Drawing.Size(75, 23);
            this.btnStep.TabIndex = 26;
            this.btnStep.Text = "Step";
            this.btnStep.UseVisualStyleBackColor = true;
            this.btnStep.Click += new System.EventHandler(this.btnStep_Click);
            // 
            // lblBreakpoint
            // 
            this.lblBreakpoint.AutoSize = true;
            this.lblBreakpoint.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.lblBreakpoint.Location = new System.Drawing.Point(640, 9);
            this.lblBreakpoint.Name = "lblBreakpoint";
            this.lblBreakpoint.Size = new System.Drawing.Size(91, 14);
            this.lblBreakpoint.TabIndex = 33;
            this.lblBreakpoint.Text = "Breakpoint: ";
            // 
            // txtBreakpoint
            // 
            this.txtBreakpoint.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.txtBreakpoint.Location = new System.Drawing.Point(737, 6);
            this.txtBreakpoint.Name = "txtBreakpoint";
            this.txtBreakpoint.Size = new System.Drawing.Size(59, 20);
            this.txtBreakpoint.TabIndex = 32;
            // 
            // btnRun
            // 
            this.btnRun.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.btnRun.Location = new System.Drawing.Point(640, 32);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(75, 23);
            this.btnRun.TabIndex = 27;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // lblIF
            // 
            this.lblIF.AutoSize = true;
            this.lblIF.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.lblIF.Location = new System.Drawing.Point(549, 211);
            this.lblIF.Name = "lblIF";
            this.lblIF.Size = new System.Drawing.Size(35, 14);
            this.lblIF.TabIndex = 12;
            this.lblIF.Text = "IF: ";
            // 
            // txtIF
            // 
            this.txtIF.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.txtIF.Location = new System.Drawing.Point(590, 208);
            this.txtIF.Name = "txtIF";
            this.txtIF.Size = new System.Drawing.Size(44, 20);
            this.txtIF.TabIndex = 11;
            // 
            // lblIE
            // 
            this.lblIE.AutoSize = true;
            this.lblIE.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.lblIE.Location = new System.Drawing.Point(641, 211);
            this.lblIE.Name = "lblIE";
            this.lblIE.Size = new System.Drawing.Size(35, 14);
            this.lblIE.TabIndex = 4;
            this.lblIE.Text = "IE: ";
            // 
            // txtIE
            // 
            this.txtIE.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.txtIE.Location = new System.Drawing.Point(682, 208);
            this.txtIE.Name = "txtIE";
            this.txtIE.Size = new System.Drawing.Size(44, 20);
            this.txtIE.TabIndex = 3;
            // 
            // chkZ
            // 
            this.chkZ.AutoSize = true;
            this.chkZ.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.chkZ.Location = new System.Drawing.Point(643, 83);
            this.chkZ.Name = "chkZ";
            this.chkZ.Size = new System.Drawing.Size(33, 18);
            this.chkZ.TabIndex = 24;
            this.chkZ.Text = "Z";
            this.chkZ.UseVisualStyleBackColor = true;
            // 
            // chkN
            // 
            this.chkN.AutoSize = true;
            this.chkN.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.chkN.Location = new System.Drawing.Point(682, 83);
            this.chkN.Name = "chkN";
            this.chkN.Size = new System.Drawing.Size(33, 18);
            this.chkN.TabIndex = 23;
            this.chkN.Text = "N";
            this.chkN.UseVisualStyleBackColor = true;
            // 
            // chkH
            // 
            this.chkH.AutoSize = true;
            this.chkH.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.chkH.Location = new System.Drawing.Point(721, 83);
            this.chkH.Name = "chkH";
            this.chkH.Size = new System.Drawing.Size(33, 18);
            this.chkH.TabIndex = 22;
            this.chkH.Text = "H";
            this.chkH.UseVisualStyleBackColor = true;
            // 
            // chkC
            // 
            this.chkC.AutoSize = true;
            this.chkC.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.chkC.Location = new System.Drawing.Point(760, 83);
            this.chkC.Name = "chkC";
            this.chkC.Size = new System.Drawing.Size(33, 18);
            this.chkC.TabIndex = 21;
            this.chkC.Text = "C";
            this.chkC.UseVisualStyleBackColor = true;
            // 
            // lblIMEStatus
            // 
            this.lblIMEStatus.AutoSize = true;
            this.lblIMEStatus.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.lblIMEStatus.Location = new System.Drawing.Point(546, 106);
            this.lblIMEStatus.Name = "lblIMEStatus";
            this.lblIMEStatus.Size = new System.Drawing.Size(91, 14);
            this.lblIMEStatus.TabIndex = 20;
            this.lblIMEStatus.Text = "IME Status: ";
            // 
            // chkIMEEnabled
            // 
            this.chkIMEEnabled.AutoSize = true;
            this.chkIMEEnabled.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.chkIMEEnabled.Location = new System.Drawing.Point(643, 106);
            this.chkIMEEnabled.Name = "chkIMEEnabled";
            this.chkIMEEnabled.Size = new System.Drawing.Size(75, 18);
            this.chkIMEEnabled.TabIndex = 19;
            this.chkIMEEnabled.Text = "Enabled";
            this.chkIMEEnabled.UseVisualStyleBackColor = false;
            // 
            // lblMemory
            // 
            this.lblMemory.AutoSize = true;
            this.lblMemory.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.lblMemory.Location = new System.Drawing.Point(12, 67);
            this.lblMemory.Name = "lblMemory";
            this.lblMemory.Size = new System.Drawing.Size(84, 14);
            this.lblMemory.TabIndex = 2;
            this.lblMemory.Text = "Memory Type";
            // 
            // txtMemory
            // 
            this.txtMemory.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.txtMemory.Location = new System.Drawing.Point(15, 84);
            this.txtMemory.Multiline = true;
            this.txtMemory.Name = "txtMemory";
            this.txtMemory.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtMemory.Size = new System.Drawing.Size(525, 258);
            this.txtMemory.TabIndex = 1;
            this.txtMemory.WordWrap = false;
            // 
            // cmbMemoryType
            // 
            this.cmbMemoryType.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.cmbMemoryType.FormattingEnabled = true;
            this.cmbMemoryType.Location = new System.Drawing.Point(394, 33);
            this.cmbMemoryType.Name = "cmbMemoryType";
            this.cmbMemoryType.Size = new System.Drawing.Size(121, 22);
            this.cmbMemoryType.TabIndex = 28;
            this.cmbMemoryType.SelectedIndexChanged += new System.EventHandler(this.cmbMemoryType_SelectedIndexChanged);
            // 
            // lblMemoryType
            // 
            this.lblMemoryType.AutoSize = true;
            this.lblMemoryType.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.lblMemoryType.Location = new System.Drawing.Point(269, 36);
            this.lblMemoryType.Name = "lblMemoryType";
            this.lblMemoryType.Size = new System.Drawing.Size(91, 14);
            this.lblMemoryType.TabIndex = 29;
            this.lblMemoryType.Text = "Memory Type:";
            // 
            // Debugger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(808, 354);
            this.Controls.Add(this.cmbMemoryType);
            this.Controls.Add(this.lblMemoryType);
            this.Controls.Add(this.lblMemory);
            this.Controls.Add(this.txtMemory);
            this.Controls.Add(this.chkC);
            this.Controls.Add(this.chkH);
            this.Controls.Add(this.chkN);
            this.Controls.Add(this.chkIMEEnabled);
            this.Controls.Add(this.chkZ);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.txtBreakpoint);
            this.Controls.Add(this.lblBreakpoint);
            this.Controls.Add(this.lblIMEStatus);
            this.Controls.Add(this.btnStep);
            this.Controls.Add(this.lblCPUStatus);
            this.Controls.Add(this.lblIE);
            this.Controls.Add(this.txtIE);
            this.Controls.Add(this.lblIF);
            this.Controls.Add(this.txtIF);
            this.Controls.Add(this.lblSP);
            this.Controls.Add(this.txtSP);
            this.Controls.Add(this.lblPC);
            this.Controls.Add(this.txtPC);
            this.Controls.Add(this.lblHL);
            this.Controls.Add(this.txtHL);
            this.Controls.Add(this.lblDE);
            this.Controls.Add(this.txtDE);
            this.Controls.Add(this.lblBC);
            this.Controls.Add(this.txtBC);
            this.Controls.Add(this.lblAF);
            this.Controls.Add(this.txtAF);
            this.Controls.Add(this.txtCurrCycles);
            this.Controls.Add(this.lblCurrCucles);
            this.Controls.Add(this.txtCurrOpcode);
            this.Controls.Add(this.lblCurrOpcode);
            this.Controls.Add(this.txtPrevOpcode);
            this.Controls.Add(this.lblPrevOpcode);
            this.Controls.Add(this.btnClose);
            this.DoubleBuffered = true;
            this.Name = "Debugger";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gameboy Debugger";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblPrevOpcode;
        private System.Windows.Forms.TextBox txtPrevOpcode;
        private System.Windows.Forms.TextBox txtCurrOpcode;
        private System.Windows.Forms.Label lblCurrOpcode;
        private System.Windows.Forms.TextBox txtCurrCycles;
        private System.Windows.Forms.Label lblCurrCucles;
        private System.Windows.Forms.TextBox txtAF;
        private System.Windows.Forms.Label lblAF;
        private System.Windows.Forms.TextBox txtBC;
        private System.Windows.Forms.Label lblBC;
        private System.Windows.Forms.TextBox txtDE;
        private System.Windows.Forms.Label lblDE;
        private System.Windows.Forms.TextBox txtHL;
        private System.Windows.Forms.Label lblHL;
        private System.Windows.Forms.TextBox txtPC;
        private System.Windows.Forms.Label lblPC;
        private System.Windows.Forms.TextBox txtSP;
        private System.Windows.Forms.Label lblSP;
        private System.Windows.Forms.Label lblCPUStatus;
        private System.Windows.Forms.Button btnStep;
        private System.Windows.Forms.Label lblBreakpoint;
        private System.Windows.Forms.TextBox txtBreakpoint;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Label lblIF;
        private System.Windows.Forms.TextBox txtIF;
        private System.Windows.Forms.Label lblIE;
        private System.Windows.Forms.TextBox txtIE;
        private System.Windows.Forms.CheckBox chkZ;
        private System.Windows.Forms.CheckBox chkN;
        private System.Windows.Forms.CheckBox chkH;
        private System.Windows.Forms.CheckBox chkC;
        private System.Windows.Forms.Label lblIMEStatus;
        private System.Windows.Forms.CheckBox chkIMEEnabled;
        private System.Windows.Forms.Label lblMemory;
        private System.Windows.Forms.TextBox txtMemory;
        private System.Windows.Forms.ComboBox cmbMemoryType;
        private System.Windows.Forms.Label lblMemoryType;
    }
}