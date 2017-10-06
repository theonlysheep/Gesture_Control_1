namespace streams.cs
{
    partial class MainForm
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
            renders[0].Dispose();
            renders[1].Dispose();

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
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.deviceMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.colorMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.depthMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.irMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.rgbImage = new System.Windows.Forms.PictureBox();
            this.depthImage = new System.Windows.Forms.PictureBox();
            this.resultImage = new System.Windows.Forms.PictureBox();
            this.messageBox = new System.Windows.Forms.RichTextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.radioClick = new System.Windows.Forms.RadioButton();
            this.radioOpen = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioFist = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.radioDepth = new System.Windows.Forms.RadioButton();
            this.radioIR = new System.Windows.Forms.RadioButton();
            this.radioColor = new System.Windows.Forms.RadioButton();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusStripLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rgbImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.depthImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.resultImage)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deviceMenu,
            this.colorMenu,
            this.depthMenu,
            this.irMenu});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(996, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // deviceMenu
            // 
            this.deviceMenu.Name = "deviceMenu";
            this.deviceMenu.Size = new System.Drawing.Size(54, 20);
            this.deviceMenu.Text = "Device";
            // 
            // colorMenu
            // 
            this.colorMenu.Name = "colorMenu";
            this.colorMenu.Size = new System.Drawing.Size(48, 20);
            this.colorMenu.Text = "Color";
            // 
            // depthMenu
            // 
            this.depthMenu.Name = "depthMenu";
            this.depthMenu.Size = new System.Drawing.Size(51, 20);
            this.depthMenu.Text = "Depth";
            // 
            // irMenu
            // 
            this.irMenu.Name = "irMenu";
            this.irMenu.Size = new System.Drawing.Size(29, 20);
            this.irMenu.Text = "IR";
            // 
            // rgbImage
            // 
            this.rgbImage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.rgbImage.Location = new System.Drawing.Point(13, 28);
            this.rgbImage.Name = "rgbImage";
            this.rgbImage.Size = new System.Drawing.Size(320, 240);
            this.rgbImage.TabIndex = 1;
            this.rgbImage.TabStop = false;
            // 
            // depthImage
            // 
            this.depthImage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.depthImage.Location = new System.Drawing.Point(339, 28);
            this.depthImage.Name = "depthImage";
            this.depthImage.Size = new System.Drawing.Size(320, 240);
            this.depthImage.TabIndex = 2;
            this.depthImage.TabStop = false;
            // 
            // resultImage
            // 
            this.resultImage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.resultImage.Location = new System.Drawing.Point(665, 28);
            this.resultImage.Name = "resultImage";
            this.resultImage.Size = new System.Drawing.Size(320, 240);
            this.resultImage.TabIndex = 3;
            this.resultImage.TabStop = false;
            // 
            // messageBox
            // 
            this.messageBox.Location = new System.Drawing.Point(13, 275);
            this.messageBox.Name = "messageBox";
            this.messageBox.Size = new System.Drawing.Size(646, 164);
            this.messageBox.TabIndex = 4;
            this.messageBox.Text = "";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.radioClick, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.radioOpen, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.radioButton3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.radioFist, 0, 3);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(665, 291);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(200, 100);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // radioClick
            // 
            this.radioClick.AutoSize = true;
            this.radioClick.Location = new System.Drawing.Point(3, 3);
            this.radioClick.Name = "radioClick";
            this.radioClick.Size = new System.Drawing.Size(48, 17);
            this.radioClick.TabIndex = 0;
            this.radioClick.TabStop = true;
            this.radioClick.Text = "Click";
            this.radioClick.UseVisualStyleBackColor = true;
            // 
            // radioOpen
            // 
            this.radioOpen.AutoSize = true;
            this.radioOpen.Location = new System.Drawing.Point(3, 28);
            this.radioOpen.Name = "radioOpen";
            this.radioOpen.Size = new System.Drawing.Size(78, 17);
            this.radioOpen.TabIndex = 1;
            this.radioOpen.TabStop = true;
            this.radioOpen.Text = "Hand open";
            this.radioOpen.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(3, 53);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(85, 17);
            this.radioButton3.TabIndex = 2;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "Hand closed";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioFist
            // 
            this.radioFist.AutoSize = true;
            this.radioFist.Location = new System.Drawing.Point(3, 78);
            this.radioFist.Name = "radioFist";
            this.radioFist.Size = new System.Drawing.Size(41, 17);
            this.radioFist.TabIndex = 3;
            this.radioFist.TabStop = true;
            this.radioFist.Text = "Fist";
            this.radioFist.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(665, 275);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Gesture recognition selection";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.buttonStop, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.buttonStart, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(902, 374);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(83, 70);
            this.tableLayoutPanel2.TabIndex = 8;
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(3, 38);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(75, 23);
            this.buttonStop.TabIndex = 1;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(3, 3);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 0;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.radioDepth, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.radioIR, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.radioColor, 0, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(902, 291);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(83, 77);
            this.tableLayoutPanel3.TabIndex = 10;
            this.tableLayoutPanel3.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel3_Paint);
            // 
            // radioDepth
            // 
            this.radioDepth.AutoSize = true;
            this.radioDepth.Location = new System.Drawing.Point(3, 53);
            this.radioDepth.Name = "radioDepth";
            this.radioDepth.Size = new System.Drawing.Size(54, 17);
            this.radioDepth.TabIndex = 2;
            this.radioDepth.TabStop = true;
            this.radioDepth.Text = "Depth";
            this.radioDepth.UseVisualStyleBackColor = true;
            // 
            // radioIR
            // 
            this.radioIR.AutoSize = true;
            this.radioIR.Location = new System.Drawing.Point(3, 28);
            this.radioIR.Name = "radioIR";
            this.radioIR.Size = new System.Drawing.Size(36, 17);
            this.radioIR.TabIndex = 1;
            this.radioIR.TabStop = true;
            this.radioIR.Text = "IR";
            this.radioIR.UseVisualStyleBackColor = true;
            // 
            // radioColor
            // 
            this.radioColor.AutoSize = true;
            this.radioColor.Location = new System.Drawing.Point(3, 3);
            this.radioColor.Name = "radioColor";
            this.radioColor.Size = new System.Drawing.Size(49, 17);
            this.radioColor.TabIndex = 0;
            this.radioColor.TabStop = true;
            this.radioColor.Text = "Color";
            this.radioColor.UseVisualStyleBackColor = true;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusStripLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 447);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(996, 22);
            this.statusStrip.TabIndex = 11;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusStripLabel
            // 
            this.statusStripLabel.Name = "statusStripLabel";
            this.statusStripLabel.Size = new System.Drawing.Size(39, 17);
            this.statusStripLabel.Text = "Ready";
            this.statusStripLabel.Click += new System.EventHandler(this.toolStripStatusLabel1_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(996, 469);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.messageBox);
            this.Controls.Add(this.resultImage);
            this.Controls.Add(this.depthImage);
            this.Controls.Add(this.rgbImage);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rgbImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.depthImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.resultImage)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem deviceMenu;
        private System.Windows.Forms.ToolStripMenuItem colorMenu;
        private System.Windows.Forms.ToolStripMenuItem depthMenu;
        private System.Windows.Forms.ToolStripMenuItem irMenu;
        private System.Windows.Forms.PictureBox rgbImage;
        private System.Windows.Forms.PictureBox depthImage;
        private System.Windows.Forms.PictureBox resultImage;
        private System.Windows.Forms.RichTextBox messageBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.RadioButton radioClick;
        private System.Windows.Forms.RadioButton radioOpen;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioFist;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.RadioButton radioColor;
        private System.Windows.Forms.RadioButton radioDepth;
        private System.Windows.Forms.RadioButton radioIR;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusStripLabel;
    }
}