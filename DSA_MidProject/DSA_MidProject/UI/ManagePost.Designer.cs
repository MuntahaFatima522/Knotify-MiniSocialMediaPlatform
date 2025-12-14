namespace DSA_MidProject.UI
{
    partial class ManagePost
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManagePost));
            panel1 = new Panel();
            pictureBox1 = new PictureBox();
            label1 = new Label();
            label6 = new Label();
            panel2 = new Panel();
            label9 = new Label();
            label8 = new Label();
            label7 = new Label();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label10 = new Label();
            undoButton = new Button();
            panel3 = new Panel();
            postsPanel = new Panel();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.Teal;
            panel1.Controls.Add(pictureBox1);
            panel1.Controls.Add(label1);
            panel1.Location = new Point(54, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1011, 59);
            panel1.TabIndex = 16;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(89, 59);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 3;
            pictureBox1.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(137, 16);
            label1.Name = "label1";
            label1.Size = new Size(0, 32);
            label1.TabIndex = 6;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.BackColor = Color.White;
            label6.Font = new Font("Segoe UI Black", 16F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.ForeColor = Color.Black;
            label6.Location = new Point(2, 4);
            label6.Name = "label6";
            label6.Size = new Size(49, 45);
            label6.TabIndex = 18;
            label6.Text = "◀";
            label6.Click += label6_Click;
            // 
            // panel2
            // 
            panel2.BackColor = Color.Teal;
            panel2.Controls.Add(label9);
            panel2.Controls.Add(label8);
            panel2.Controls.Add(label7);
            panel2.Controls.Add(label5);
            panel2.Controls.Add(label4);
            panel2.Controls.Add(label3);
            panel2.Controls.Add(label2);
            panel2.Location = new Point(0, 56);
            panel2.Name = "panel2";
            panel2.Size = new Size(54, 590);
            panel2.TabIndex = 17;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label9.ForeColor = Color.White;
            label9.Location = new Point(1, 301);
            label9.Name = "label9";
            label9.Size = new Size(64, 38);
            label9.TabIndex = 14;
            label9.Text = "👥 ";
            label9.Click += label9_Click;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label8.ForeColor = Color.White;
            label8.Location = new Point(0, 357);
            label8.Name = "label8";
            label8.Size = new Size(56, 38);
            label8.TabIndex = 13;
            label8.Text = "👤";
            label8.Click += label8_Click;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label7.ForeColor = Color.Black;
            label7.Location = new Point(6, 249);
            label7.Name = "label7";
            label7.Size = new Size(41, 38);
            label7.TabIndex = 12;
            label7.Text = "...";
            label7.Click += label7_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.ForeColor = Color.White;
            label5.Location = new Point(6, 76);
            label5.Name = "label5";
            label5.Size = new Size(43, 38);
            label5.TabIndex = 10;
            label5.Text = "☰";
            label5.Click += label5_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.ForeColor = Color.White;
            label4.Location = new Point(1, 136);
            label4.Name = "label4";
            label4.Size = new Size(56, 38);
            label4.TabIndex = 9;
            label4.Text = "🔍";
            label4.Click += label4_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.White;
            label3.Location = new Point(1, 198);
            label3.Name = "label3";
            label3.Size = new Size(56, 38);
            label3.TabIndex = 8;
            label3.Text = "➕";
            label3.Click += label3_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.White;
            label2.Location = new Point(2, 22);
            label2.Name = "label2";
            label2.Size = new Size(56, 38);
            label2.TabIndex = 7;
            label2.Text = "🏠";
            label2.Click += label2_Click;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI Black", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label10.Location = new Point(7, -7);
            label10.Name = "label10";
            label10.Size = new Size(145, 38);
            label10.TabIndex = 20;
            label10.Text = "My Posts";
            // 
            // undoButton
            // 
            undoButton.BackColor = Color.White;
            undoButton.Font = new Font("Segoe UI Black", 10F);
            undoButton.ForeColor = Color.LightSeaGreen;
            undoButton.Location = new Point(919, -3);
            undoButton.Name = "undoButton";
            undoButton.Size = new Size(92, 37);
            undoButton.TabIndex = 25;
            undoButton.Text = "Undo";
            undoButton.UseVisualStyleBackColor = false;
            undoButton.Click += UndoButton_Click;
            // 
            // panel3
            // 
            panel3.BackColor = Color.LightSeaGreen;
            panel3.Controls.Add(undoButton);
            panel3.Controls.Add(label10);
            panel3.Location = new Point(54, 56);
            panel3.Name = "panel3";
            panel3.Size = new Size(1010, 31);
            panel3.TabIndex = 29;
            // 
            // postsPanel
            // 
            postsPanel.Location = new Point(53, 90);
            postsPanel.Name = "postsPanel";
            postsPanel.Size = new Size(1012, 553);
            postsPanel.TabIndex = 30;
            // 
            // ManagePost
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1065, 646);
            Controls.Add(postsPanel);
            Controls.Add(panel3);
            Controls.Add(panel1);
            Controls.Add(label6);
            Controls.Add(panel2);
            MaximumSize = new Size(1087, 702);
            MinimumSize = new Size(1087, 702);
            Name = "ManagePost";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ManagePost";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private PictureBox pictureBox1;
        private Label label1;
        private Label label6;
        private Panel panel2;
        private Label label9;
        private Label label8;
        private Label label7;
        private Label label5;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label10;
        private Button undoButton;
        private Panel panel3;
        private Panel postsPanel;
    }
}