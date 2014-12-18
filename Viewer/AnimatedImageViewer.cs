using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AnimatedImages;

namespace Viewer
{
    public partial class AnimatedImageViewer : Form
    {
		public AnimatedImageViewer(string[] args)
        {
            InitializeComponent();
            if (args != null && args.Length > 0)
            {
                picApng.Image = APNG.FromFile(args[args.Length - 1]);
                this.Width = picApng.Image.ViewSize.Width;
                this.Height = picApng.Image.ViewSize.Height;
            }
        }

        private void AnimatedImageViewer_DragDrop(object sender, DragEventArgs e)
        {
            string filename = ((string[]) e.Data.GetData(DataFormats.FileDrop))[0];
            picApng.Image = APNG.FromFile(filename);
            this.Width = picApng.Image.ViewSize.Width;
            this.Height = picApng.Image.ViewSize.Height;
        }

        private void AnimatedImageViewer_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                e.Effect = DragDropEffects.All;
            }  
        }
    }
}
