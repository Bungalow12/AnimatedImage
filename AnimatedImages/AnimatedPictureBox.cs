using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;

namespace AnimatedImages
{
    /// <summary>
    /// Picture Box supporting animated PNGs
    /// </summary>
    public class AnimatedPictureBox : Control
    {
        private IAnimatedImage image = null;
        private int frameRate = 0;
        private readonly Timer animationTimer;
        private int curPlayCount = 0;
        private int currentFrame = -1;
        //private List<Bitmap> frames = new List<Bitmap>();
        private Bitmap curImage = null;
        private PictureBox pictureBox;

        /// <summary>
        /// The most recently drawn image.
        /// </summary>
        protected Bitmap Drawn = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatedImages.AnimatedPictureBox"/> class.*/
        /// </summary>
        public AnimatedPictureBox()
        {
            DoubleBuffered = true;
            pictureBox = new PictureBox();
            pictureBox.Size = Size;
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox.Dock = DockStyle.Fill;
            animationTimer = new Timer();
            animationTimer.Tick += HandleTick;
        }

        /// <summary>
        /// Performs the base operations for the painting of the control.
        /// </summary>
        /// <param name="e">E.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (curImage != null)
            {
                //Frame has incremented already.
                int curFrame = (currentFrame - 1 < 0) ? 0 : currentFrame - 1;

                Graphics g = e.Graphics;
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;

                if (Drawn == null)
                {
                    Drawn = new Bitmap(image.ViewSize.Width, image.ViewSize.Height);
                    Graphics render = Graphics.FromImage(Drawn);

                    render.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                    //render.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.GammaCorrected;
                    //render.Clear(Color.FromArgb(0x00000000));
                    render.DrawImage(curImage, 0, 0, Drawn.Width, Drawn.Height);
                }

                if (image.GetDisposeOperationFor(curFrame) == DisposeOps.APNGDisposeOpBackground)
                {
                    Graphics render = Graphics.FromImage(Drawn);
                    render.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                    //render.Clear(Color.FromArgb(0x00000000));
                    //g.Clear(Color.FromArgb(0x00000000));
                    //ClearBitmap(ref Drawn);

                }
                else if (image.GetDisposeOperationFor(curFrame) == DisposeOps.APNGDisposeOpPrevious)
                {
                    throw new NotImplementedException("Dispose operation previous has not been implemented yet");
                }

                //Blend operation
                if (image.GetBlendOperationFor(curFrame) == BlendOps.APNGBlendOpOver)
                {
                    GetCompositePixel(ref Drawn, curImage);
                }
                else if (image.GetBlendOperationFor(curFrame) == BlendOps.APNGBlendOpSource)
                {
                    //TODO: Fix this for bounce.png for some reason it the previous draw is not clearing. 
                    //(This works if every other frame. Not sure how to wait for the surface to have cleared...)
                    Graphics render = Graphics.FromImage(Drawn);

                    render.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                    g.Clear(Color.FromArgb(0x00000000));
                    render.Clear(Color.FromArgb(0x00000000));
                    render.DrawImage(curImage, 0, 0, Drawn.Width, Drawn.Height);
                    //g.Clear(Color.FromArgb(0x00000000));
                    //ClearBitmap(ref Drawn);
                    //DrawTo(ref Drawn, curImage);
                }
                g.DrawImage(Drawn, ClientRectangle);
            }
        }

        /// <summary>
        /// Clears the bitmap.
        /// </summary>
        /// <param name="image">Image.</param>
        protected void ClearBitmap(ref Bitmap image)
        {
            Color clearColor = Color.FromArgb(0x00000000);

            for (int y = 0; y < image.Height; ++y)
            {
                for (int x = 0; x < image.Width; ++x)
                {
                    image.SetPixel(x, y, clearColor);
                }
            }
        }

        /// <summary>
        /// Draws over the destination bitmap.
        /// </summary>
        /// <param name="dest">Destination.</param>
        /// <param name="src">Source.</param>
        protected void DrawTo(ref Bitmap dest, Bitmap src)
        {
            for (int y = 0; y < dest.Height; ++y)
            {
                for (int x = 0; x < dest.Width; ++x)
                {
                    dest.SetPixel(x, y, src.GetPixel(x, y));
                }
            }
        }

        /// <summary>
        /// Gets the composite bitmap.
        /// </summary>
        /// <param name="dest">Destination bitmap.</param>
        /// <param name="src">Source bitmap.</param>
        protected void GetCompositePixel(ref Bitmap dest, Bitmap src)
        {
            for (int y = 0; y < dest.Height; ++y)
            {
                for (int x = 0; x < dest.Width; ++x)
                {
                    Color destPixel = dest.GetPixel(x, y);
                    Color srcPixel = src.GetPixel(x, y);
                    Color finalPixel;

                    //Src is transparent here. Use the background
                    if(srcPixel.A == 0)
                    {
                        finalPixel = Color.FromArgb(destPixel.ToArgb());
                    }
                    //Src is solid. Use the Src.
                    else if(srcPixel.A == 255)
                    {
                        finalPixel = Color.FromArgb(srcPixel.ToArgb());
                    }
                    else //Compositing needed. Algorithm from: http://pmt.sourceforge.net/specs/png-1.2-pdg.html#D.Alpha-channel-processing
                    {
                        //Get floating point alpha and it's compliment.
                        float alpha = srcPixel.A / 255;
                        float compAlpha = 1.0f - alpha;

                        //Convert each pixel to floating point.
                        float rSrc, gSrc, bSrc;
                        rSrc = srcPixel.R / 255;
                        gSrc = srcPixel.G / 255;
                        bSrc = srcPixel.B / 255;

                        float rDest, gDest, bDest;
                        rDest = destPixel.R / 255;
                        gDest = destPixel.G / 255;
                        bDest = destPixel.B / 255;

                        finalPixel = Color.FromArgb(
                            srcPixel.A,
                            (int)((rSrc * alpha + rDest * compAlpha) * 255),
                            (int)((gSrc * alpha + gDest * compAlpha) * 255),
                            (int)((bSrc * alpha + bDest * compAlpha) * 255)
                        );
                    }
                    dest.SetPixel(x, y, finalPixel);
                }
            }

        }

        /// <summary>
        /// Updates the frame.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event args.</param>
        protected void HandleTick (object sender, EventArgs e)
        {
            if (image != null)
            {
                if (currentFrame >= image.FrameCount)
                {
                    Drawn = null;
                    if(image.PlayCount > 0)
                    {
                        if (++curPlayCount > image.PlayCount)
                        {
                            curImage = image.GetDefaultImage();
                            animationTimer.Stop();
                            return;
                        }
                    }
                    currentFrame = 0;
                }

                curImage = image[currentFrame++];
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>The image.</value>
        public IAnimatedImage Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
                FrameRate = value.FrameRate;
                curPlayCount = value.PlayCount;

                //Get all frames preloaded for better display.
                for (int i = 0; i < image.FrameCount; ++i)
                {

                }

                animationTimer.Interval = frameRate;
                animationTimer.Start();
            }
        }

        /// <summary>
        /// Gets or sets the frame rate in milliseconds.
        /// </summary>
        /// <value>The frame rate in milliseconds.</value>
        public int FrameRate
        {
            get
            {
                return frameRate;
            }

            set
            {
                frameRate = value;
                curPlayCount = image.PlayCount;
                animationTimer.Interval = frameRate;
                animationTimer.Start();
            }
        }

        /// <summary>
        /// Gets or sets the size mode. (Not yet complete. StretchImage only)
        /// </summary>
        /// <value>The size mode.</value>
        public PictureBoxSizeMode SizeMode
        {
            get;
            set;
        }
    }
}

