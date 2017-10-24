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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.deviceMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.rgbImage = new System.Windows.Forms.PictureBox();
            this.depthImage = new System.Windows.Forms.PictureBox();
            this.resultImage = new System.Windows.Forms.PictureBox();
            this.messageBox = new System.Windows.Forms.RichTextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.radioDepth = new System.Windows.Forms.RadioButton();
            this.radioIR = new System.Windows.Forms.RadioButton();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusStripLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.gestureListBox = new System.Windows.Forms.CheckedListBox();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rgbImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.depthImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.resultImage)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deviceMenu});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(996, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // deviceMenu
            // 
            this.deviceMenu.Name = "deviceMenu";
            this.deviceMenu.Size = new System.Drawing.Size(54, 20);
            this.deviceMenu.Text = "Device";
            // 
            // rgbImage
            // 
            this.rgbImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rgbImage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.rgbImage.Location = new System.Drawing.Point(3, 3);
            this.rgbImage.Name = "rgbImage";
            this.rgbImage.Size = new System.Drawing.Size(318, 235);
            this.rgbImage.TabIndex = 1;
            this.rgbImage.TabStop = false;
            // 
            // depthImage
            // 
            this.depthImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.depthImage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.depthImage.Location = new System.Drawing.Point(327, 3);
            this.depthImage.Name = "depthImage";
            this.depthImage.Size = new System.Drawing.Size(318, 235);
            this.depthImage.TabIndex = 2;
            this.depthImage.TabStop = false;
            // 
            // resultImage
            // 
            this.resultImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resultImage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.resultImage.Location = new System.Drawing.Point(651, 3);
            this.resultImage.Name = "resultImage";
            this.resultImage.Size = new System.Drawing.Size(318, 235);
            this.resultImage.TabIndex = 3;
            this.resultImage.TabStop = false;
            // 
            // messageBox
            // 
            this.messageBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.messageBox.Location = new System.Drawing.Point(13, 275);
            this.messageBox.Name = "messageBox";
            this.messageBox.Size = new System.Drawing.Size(646, 169);
            this.messageBox.TabIndex = 4;
            this.messageBox.Text = "";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(665, 275);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Gesture recognition selection";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
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
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
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
            this.tableLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.radioDepth, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.radioIR, 0, 1);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(902, 291);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(83, 53);
            this.tableLayoutPanel3.TabIndex = 10;
            this.tableLayoutPanel3.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel3_Paint);
            // 
            // radioDepth
            // 
            this.radioDepth.AutoSize = true;
            this.radioDepth.Location = new System.Drawing.Point(3, 3);
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
            this.radioIR.Location = new System.Drawing.Point(3, 29);
            this.radioIR.Name = "radioIR";
            this.radioIR.Size = new System.Drawing.Size(36, 17);
            this.radioIR.TabIndex = 1;
            this.radioIR.TabStop = true;
            this.radioIR.Text = "IR";
            this.radioIR.UseVisualStyleBackColor = true;
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
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.rgbImage, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.depthImage, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.resultImage, 2, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(13, 28);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(972, 241);
            this.tableLayoutPanel1.TabIndex = 12;
            // 
            // gestureListBox
            // 
            this.gestureListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.gestureListBox.FormattingEnabled = true;
            this.gestureListBox.Location = new System.Drawing.Point(668, 291);
            this.gestureListBox.Name = "gestureListBox";
            this.gestureListBox.Size = new System.Drawing.Size(141, 154);
            this.gestureListBox.TabIndex = 13;
            this.gestureListBox.Visible = false;
            this.gestureListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.gestureListBox_ItemCheck);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(996, 469);
            this.Controls.Add(this.gestureListBox);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.messageBox);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rgbImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.depthImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.resultImage)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem deviceMenu;
        private System.Windows.Forms.PictureBox rgbImage;
        private System.Windows.Forms.PictureBox depthImage;
        private System.Windows.Forms.PictureBox resultImage;
        private System.Windows.Forms.RichTextBox messageBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.RadioButton radioDepth;
        private System.Windows.Forms.RadioButton radioIR;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusStripLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckedListBox gestureListBox;
    }
}