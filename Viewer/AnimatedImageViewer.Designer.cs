namespace Viewer
{
    partial class AnimatedImageViewer
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
            this.picApng = new AnimatedImages.AnimatedPictureBox();
            //((System.ComponentModel.ISupportInitialize)(this.picApng)).BeginInit();
            this.SuspendLayout();
            // 
            // picGif
            // 
            this.picApng.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picApng.Location = new System.Drawing.Point(12, 12);
            this.picApng.Name = "picApng";
            this.picApng.Size = new System.Drawing.Size(260, 238);
            this.picApng.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picApng.TabIndex = 0;
            this.picApng.TabStop = false;
            // 
            // GifViewer
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.picApng);
            this.DoubleBuffered = true;
            this.Name = "APNGViewer";
            this.Text = "APNG Viewer";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.AnimatedImageViewer_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.AnimatedImageViewer_DragEnter);
            //((System.ComponentModel.ISupportInitialize)(this.picApng)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AnimatedImages.AnimatedPictureBox picApng;
    }
}

