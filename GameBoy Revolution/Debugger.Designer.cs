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
            this.txtA = new System.Windows.Forms.TextBox();
            this.lblA = new System.Windows.Forms.Label();
            this.txtB = new System.Windows.Forms.TextBox();
            this.lblB = new System.Windows.Forms.Label();
            this.txtC = new System.Windows.Forms.TextBox();
            this.lblC = new System.Windows.Forms.Label();
            this.txtD = new System.Windows.Forms.TextBox();
            this.lblD = new System.Windows.Forms.Label();
            this.txtE = new System.Windows.Forms.TextBox();
            this.lblE = new System.Windows.Forms.Label();
            this.txtH = new System.Windows.Forms.TextBox();
            this.lblH = new System.Windows.Forms.Label();
            this.txtL = new System.Windows.Forms.TextBox();
            this.lblL = new System.Windows.Forms.Label();
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
            this.txtCPUStatus = new System.Windows.Forms.TextBox();
            this.lblCPUStatus = new System.Windows.Forms.Label();
            this.lblCPUStatus2 = new System.Windows.Forms.Label();
            this.btnStep = new System.Windows.Forms.Button();
            this.lblBreakpoint = new System.Windows.Forms.Label();
            this.txtBreakpoint = new System.Windows.Forms.TextBox();
            this.btnRun = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(825, 427);
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
            this.lblPrevOpcode.Location = new System.Drawing.Point(12, 9);
            this.lblPrevOpcode.Name = "lblPrevOpcode";
            this.lblPrevOpcode.Size = new System.Drawing.Size(95, 13);
            this.lblPrevOpcode.TabIndex = 1;
            this.lblPrevOpcode.Text = "Previous Opcode: ";
            // 
            // txtPrevOpcode
            // 
            this.txtPrevOpcode.Location = new System.Drawing.Point(113, 6);
            this.txtPrevOpcode.Name = "txtPrevOpcode";
            this.txtPrevOpcode.Size = new System.Drawing.Size(100, 20);
            this.txtPrevOpcode.TabIndex = 2;
            // 
            // txtCurrOpcode
            // 
            this.txtCurrOpcode.Location = new System.Drawing.Point(313, 6);
            this.txtCurrOpcode.Name = "txtCurrOpcode";
            this.txtCurrOpcode.Size = new System.Drawing.Size(100, 20);
            this.txtCurrOpcode.TabIndex = 4;
            // 
            // lblCurrOpcode
            // 
            this.lblCurrOpcode.AutoSize = true;
            this.lblCurrOpcode.Location = new System.Drawing.Point(219, 9);
            this.lblCurrOpcode.Name = "lblCurrOpcode";
            this.lblCurrOpcode.Size = new System.Drawing.Size(88, 13);
            this.lblCurrOpcode.TabIndex = 3;
            this.lblCurrOpcode.Text = "Current Opcode: ";
            // 
            // txtCurrCycles
            // 
            this.txtCurrCycles.Location = new System.Drawing.Point(113, 32);
            this.txtCurrCycles.Name = "txtCurrCycles";
            this.txtCurrCycles.Size = new System.Drawing.Size(100, 20);
            this.txtCurrCycles.TabIndex = 6;
            // 
            // lblCurrCucles
            // 
            this.lblCurrCucles.AutoSize = true;
            this.lblCurrCucles.Location = new System.Drawing.Point(12, 35);
            this.lblCurrCucles.Name = "lblCurrCucles";
            this.lblCurrCucles.Size = new System.Drawing.Size(81, 13);
            this.lblCurrCucles.TabIndex = 5;
            this.lblCurrCucles.Text = "Current Cycles: ";
            // 
            // txtA
            // 
            this.txtA.Location = new System.Drawing.Point(38, 83);
            this.txtA.Name = "txtA";
            this.txtA.Size = new System.Drawing.Size(44, 20);
            this.txtA.TabIndex = 8;
            // 
            // lblA
            // 
            this.lblA.AutoSize = true;
            this.lblA.Location = new System.Drawing.Point(12, 86);
            this.lblA.Name = "lblA";
            this.lblA.Size = new System.Drawing.Size(20, 13);
            this.lblA.TabIndex = 7;
            this.lblA.Text = "A: ";
            // 
            // txtB
            // 
            this.txtB.Location = new System.Drawing.Point(38, 109);
            this.txtB.Name = "txtB";
            this.txtB.Size = new System.Drawing.Size(44, 20);
            this.txtB.TabIndex = 10;
            // 
            // lblB
            // 
            this.lblB.AutoSize = true;
            this.lblB.Location = new System.Drawing.Point(12, 112);
            this.lblB.Name = "lblB";
            this.lblB.Size = new System.Drawing.Size(20, 13);
            this.lblB.TabIndex = 9;
            this.lblB.Text = "B: ";
            // 
            // txtC
            // 
            this.txtC.Location = new System.Drawing.Point(38, 135);
            this.txtC.Name = "txtC";
            this.txtC.Size = new System.Drawing.Size(44, 20);
            this.txtC.TabIndex = 12;
            // 
            // lblC
            // 
            this.lblC.AutoSize = true;
            this.lblC.Location = new System.Drawing.Point(12, 138);
            this.lblC.Name = "lblC";
            this.lblC.Size = new System.Drawing.Size(20, 13);
            this.lblC.TabIndex = 11;
            this.lblC.Text = "C: ";
            // 
            // txtD
            // 
            this.txtD.Location = new System.Drawing.Point(38, 161);
            this.txtD.Name = "txtD";
            this.txtD.Size = new System.Drawing.Size(44, 20);
            this.txtD.TabIndex = 14;
            // 
            // lblD
            // 
            this.lblD.AutoSize = true;
            this.lblD.Location = new System.Drawing.Point(12, 164);
            this.lblD.Name = "lblD";
            this.lblD.Size = new System.Drawing.Size(21, 13);
            this.lblD.TabIndex = 13;
            this.lblD.Text = "D: ";
            // 
            // txtE
            // 
            this.txtE.Location = new System.Drawing.Point(38, 187);
            this.txtE.Name = "txtE";
            this.txtE.Size = new System.Drawing.Size(44, 20);
            this.txtE.TabIndex = 16;
            // 
            // lblE
            // 
            this.lblE.AutoSize = true;
            this.lblE.Location = new System.Drawing.Point(12, 190);
            this.lblE.Name = "lblE";
            this.lblE.Size = new System.Drawing.Size(20, 13);
            this.lblE.TabIndex = 15;
            this.lblE.Text = "E: ";
            // 
            // txtH
            // 
            this.txtH.Location = new System.Drawing.Point(38, 213);
            this.txtH.Name = "txtH";
            this.txtH.Size = new System.Drawing.Size(44, 20);
            this.txtH.TabIndex = 18;
            // 
            // lblH
            // 
            this.lblH.AutoSize = true;
            this.lblH.Location = new System.Drawing.Point(12, 216);
            this.lblH.Name = "lblH";
            this.lblH.Size = new System.Drawing.Size(21, 13);
            this.lblH.TabIndex = 17;
            this.lblH.Text = "H: ";
            // 
            // txtL
            // 
            this.txtL.Location = new System.Drawing.Point(38, 239);
            this.txtL.Name = "txtL";
            this.txtL.Size = new System.Drawing.Size(44, 20);
            this.txtL.TabIndex = 20;
            // 
            // lblL
            // 
            this.lblL.AutoSize = true;
            this.lblL.Location = new System.Drawing.Point(12, 242);
            this.lblL.Name = "lblL";
            this.lblL.Size = new System.Drawing.Size(19, 13);
            this.lblL.TabIndex = 19;
            this.lblL.Text = "L: ";
            // 
            // txtAF
            // 
            this.txtAF.Location = new System.Drawing.Point(120, 83);
            this.txtAF.Name = "txtAF";
            this.txtAF.Size = new System.Drawing.Size(44, 20);
            this.txtAF.TabIndex = 22;
            // 
            // lblAF
            // 
            this.lblAF.AutoSize = true;
            this.lblAF.Location = new System.Drawing.Point(87, 86);
            this.lblAF.Name = "lblAF";
            this.lblAF.Size = new System.Drawing.Size(26, 13);
            this.lblAF.TabIndex = 21;
            this.lblAF.Text = "AF: ";
            // 
            // txtBC
            // 
            this.txtBC.Location = new System.Drawing.Point(120, 112);
            this.txtBC.Name = "txtBC";
            this.txtBC.Size = new System.Drawing.Size(44, 20);
            this.txtBC.TabIndex = 24;
            // 
            // lblBC
            // 
            this.lblBC.AutoSize = true;
            this.lblBC.Location = new System.Drawing.Point(87, 112);
            this.lblBC.Name = "lblBC";
            this.lblBC.Size = new System.Drawing.Size(27, 13);
            this.lblBC.TabIndex = 23;
            this.lblBC.Text = "BC: ";
            // 
            // txtDE
            // 
            this.txtDE.Location = new System.Drawing.Point(120, 138);
            this.txtDE.Name = "txtDE";
            this.txtDE.Size = new System.Drawing.Size(44, 20);
            this.txtDE.TabIndex = 26;
            // 
            // lblDE
            // 
            this.lblDE.AutoSize = true;
            this.lblDE.Location = new System.Drawing.Point(87, 138);
            this.lblDE.Name = "lblDE";
            this.lblDE.Size = new System.Drawing.Size(28, 13);
            this.lblDE.TabIndex = 25;
            this.lblDE.Text = "DE: ";
            // 
            // txtHL
            // 
            this.txtHL.Location = new System.Drawing.Point(120, 161);
            this.txtHL.Name = "txtHL";
            this.txtHL.Size = new System.Drawing.Size(44, 20);
            this.txtHL.TabIndex = 28;
            // 
            // lblHL
            // 
            this.lblHL.AutoSize = true;
            this.lblHL.Location = new System.Drawing.Point(87, 164);
            this.lblHL.Name = "lblHL";
            this.lblHL.Size = new System.Drawing.Size(27, 13);
            this.lblHL.TabIndex = 27;
            this.lblHL.Text = "HL: ";
            // 
            // txtPC
            // 
            this.txtPC.Location = new System.Drawing.Point(120, 187);
            this.txtPC.Name = "txtPC";
            this.txtPC.Size = new System.Drawing.Size(44, 20);
            this.txtPC.TabIndex = 30;
            // 
            // lblPC
            // 
            this.lblPC.AutoSize = true;
            this.lblPC.Location = new System.Drawing.Point(87, 190);
            this.lblPC.Name = "lblPC";
            this.lblPC.Size = new System.Drawing.Size(27, 13);
            this.lblPC.TabIndex = 29;
            this.lblPC.Text = "PC: ";
            // 
            // txtSP
            // 
            this.txtSP.Location = new System.Drawing.Point(120, 216);
            this.txtSP.Name = "txtSP";
            this.txtSP.Size = new System.Drawing.Size(44, 20);
            this.txtSP.TabIndex = 32;
            // 
            // lblSP
            // 
            this.lblSP.AutoSize = true;
            this.lblSP.Location = new System.Drawing.Point(87, 219);
            this.lblSP.Name = "lblSP";
            this.lblSP.Size = new System.Drawing.Size(27, 13);
            this.lblSP.TabIndex = 31;
            this.lblSP.Text = "SP: ";
            // 
            // txtCPUStatus
            // 
            this.txtCPUStatus.Location = new System.Drawing.Point(493, 6);
            this.txtCPUStatus.Name = "txtCPUStatus";
            this.txtCPUStatus.Size = new System.Drawing.Size(105, 20);
            this.txtCPUStatus.TabIndex = 34;
            // 
            // lblCPUStatus
            // 
            this.lblCPUStatus.AutoSize = true;
            this.lblCPUStatus.Location = new System.Drawing.Point(419, 9);
            this.lblCPUStatus.Name = "lblCPUStatus";
            this.lblCPUStatus.Size = new System.Drawing.Size(68, 13);
            this.lblCPUStatus.TabIndex = 33;
            this.lblCPUStatus.Text = "CPU Status: ";
            // 
            // lblCPUStatus2
            // 
            this.lblCPUStatus2.AutoSize = true;
            this.lblCPUStatus2.Location = new System.Drawing.Point(495, 29);
            this.lblCPUStatus2.Name = "lblCPUStatus2";
            this.lblCPUStatus2.Size = new System.Drawing.Size(103, 13);
            this.lblCPUStatus2.TabIndex = 35;
            this.lblCPUStatus2.Text = "Z  N  H  C  0  0  0  0";
            // 
            // btnStep
            // 
            this.btnStep.Location = new System.Drawing.Point(825, 67);
            this.btnStep.Name = "btnStep";
            this.btnStep.Size = new System.Drawing.Size(75, 23);
            this.btnStep.TabIndex = 36;
            this.btnStep.Text = "Step";
            this.btnStep.UseVisualStyleBackColor = true;
            this.btnStep.Click += new System.EventHandler(this.btnStep_Click);
            // 
            // lblBreakpoint
            // 
            this.lblBreakpoint.AutoSize = true;
            this.lblBreakpoint.Location = new System.Drawing.Point(730, 15);
            this.lblBreakpoint.Name = "lblBreakpoint";
            this.lblBreakpoint.Size = new System.Drawing.Size(64, 13);
            this.lblBreakpoint.TabIndex = 37;
            this.lblBreakpoint.Text = "Breakpoint: ";
            // 
            // txtBreakpoint
            // 
            this.txtBreakpoint.Location = new System.Drawing.Point(800, 12);
            this.txtBreakpoint.Name = "txtBreakpoint";
            this.txtBreakpoint.Size = new System.Drawing.Size(100, 20);
            this.txtBreakpoint.TabIndex = 38;
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(825, 38);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(75, 23);
            this.btnRun.TabIndex = 39;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // Debugger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(912, 462);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.txtBreakpoint);
            this.Controls.Add(this.lblBreakpoint);
            this.Controls.Add(this.btnStep);
            this.Controls.Add(this.lblCPUStatus2);
            this.Controls.Add(this.txtCPUStatus);
            this.Controls.Add(this.lblCPUStatus);
            this.Controls.Add(this.txtSP);
            this.Controls.Add(this.lblSP);
            this.Controls.Add(this.txtPC);
            this.Controls.Add(this.lblPC);
            this.Controls.Add(this.txtHL);
            this.Controls.Add(this.lblHL);
            this.Controls.Add(this.txtDE);
            this.Controls.Add(this.lblDE);
            this.Controls.Add(this.txtBC);
            this.Controls.Add(this.lblBC);
            this.Controls.Add(this.txtAF);
            this.Controls.Add(this.lblAF);
            this.Controls.Add(this.txtL);
            this.Controls.Add(this.lblL);
            this.Controls.Add(this.txtH);
            this.Controls.Add(this.lblH);
            this.Controls.Add(this.txtE);
            this.Controls.Add(this.lblE);
            this.Controls.Add(this.txtD);
            this.Controls.Add(this.lblD);
            this.Controls.Add(this.txtC);
            this.Controls.Add(this.lblC);
            this.Controls.Add(this.txtB);
            this.Controls.Add(this.lblB);
            this.Controls.Add(this.txtA);
            this.Controls.Add(this.lblA);
            this.Controls.Add(this.txtCurrCycles);
            this.Controls.Add(this.lblCurrCucles);
            this.Controls.Add(this.txtCurrOpcode);
            this.Controls.Add(this.lblCurrOpcode);
            this.Controls.Add(this.txtPrevOpcode);
            this.Controls.Add(this.lblPrevOpcode);
            this.Controls.Add(this.btnClose);
            this.Name = "Debugger";
            this.Text = "Debugger";
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
        private System.Windows.Forms.TextBox txtA;
        private System.Windows.Forms.Label lblA;
        private System.Windows.Forms.TextBox txtB;
        private System.Windows.Forms.Label lblB;
        private System.Windows.Forms.TextBox txtC;
        private System.Windows.Forms.Label lblC;
        private System.Windows.Forms.TextBox txtD;
        private System.Windows.Forms.Label lblD;
        private System.Windows.Forms.TextBox txtE;
        private System.Windows.Forms.Label lblE;
        private System.Windows.Forms.TextBox txtH;
        private System.Windows.Forms.Label lblH;
        private System.Windows.Forms.TextBox txtL;
        private System.Windows.Forms.Label lblL;
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
        private System.Windows.Forms.TextBox txtCPUStatus;
        private System.Windows.Forms.Label lblCPUStatus;
        private System.Windows.Forms.Label lblCPUStatus2;
        private System.Windows.Forms.Button btnStep;
        private System.Windows.Forms.Label lblBreakpoint;
        private System.Windows.Forms.TextBox txtBreakpoint;
        private System.Windows.Forms.Button btnRun;
    }
}