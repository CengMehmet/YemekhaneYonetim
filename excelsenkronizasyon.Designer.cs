namespace RTEvents
{
    partial class excelsenkronizasyon
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.buttonExcelGuncelle = new System.Windows.Forms.Button();
            this.buttonExcelGoster = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(135, 1);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(874, 708);
            this.dataGridView1.TabIndex = 6;
            // 
            // buttonExcelGuncelle
            // 
            this.buttonExcelGuncelle.Location = new System.Drawing.Point(24, 83);
            this.buttonExcelGuncelle.Name = "buttonExcelGuncelle";
            this.buttonExcelGuncelle.Size = new System.Drawing.Size(89, 61);
            this.buttonExcelGuncelle.TabIndex = 10;
            this.buttonExcelGuncelle.Text = "Excel Güncelle";
            this.buttonExcelGuncelle.UseVisualStyleBackColor = true;
            this.buttonExcelGuncelle.Click += new System.EventHandler(this.buttonExcelGuncelle_Click);
            // 
            // buttonExcelGoster
            // 
            this.buttonExcelGoster.Location = new System.Drawing.Point(24, 12);
            this.buttonExcelGoster.Name = "buttonExcelGoster";
            this.buttonExcelGoster.Size = new System.Drawing.Size(89, 61);
            this.buttonExcelGoster.TabIndex = 9;
            this.buttonExcelGoster.Text = "Excel Görüntüle";
            this.buttonExcelGoster.UseVisualStyleBackColor = true;
            this.buttonExcelGoster.Click += new System.EventHandler(this.buttonExcelGoster_Click);
            // 
            // excelsenkronizasyon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1011, 704);
            this.Controls.Add(this.buttonExcelGuncelle);
            this.Controls.Add(this.buttonExcelGoster);
            this.Controls.Add(this.dataGridView1);
            this.Name = "excelsenkronizasyon";
            this.Text = "Excel Senkronizasyon";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button buttonExcelGuncelle;
        private System.Windows.Forms.Button buttonExcelGoster;
    }
}