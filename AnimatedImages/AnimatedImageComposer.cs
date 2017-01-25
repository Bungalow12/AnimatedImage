using System;
using System.Drawing;

namespace AnimatedImages
{
    public class AnimatedImageComposer
    {
        private IAnimatedImage image;
        private Bitmap curImage = null;

        public AnimatedImageComposer(IAnimatedImage animatedImage)
        {
            this.image = animatedImage;
        }

        public Bitmap Render(int frame)
        {
            if (this.image == null || this.curImage == null)
            {
                return null;
            }

            // Get Start Image that would be the closest to current frame index with APNGDisposeOpBackground
            int startIndex = frame;
            while (startIndex > 0 && this.image.GetDisposeOperationFor(startIndex) != DisposeOps.APNGDisposeOpBackground)
            {
                --startIndex;
            }

            Bitmap latestImage = new Bitmap(this.image.ViewSize.Width, this.image.ViewSize.Height);

            //Draw all frames up to requested frame
            for (int i = startIndex; i <= frame; ++i)
            {
                curImage = this.image[i];
                if (image.GetDisposeOperationFor(frame) == DisposeOps.APNGDisposeOpBackground || i == 1)
                {
                    Graphics render = Graphics.FromImage(latestImage);
                    render.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                    render.Clear(Color.FromArgb(0x00000000));
                    //render.DrawImage(curImage, 0, 0, latestImage.Width, latestImage.Height);
                    //g.Clear(Color.FromArgb(0x00000000));
                    //ClearBitmap(ref latestImage);

                }
                else if (image.GetDisposeOperationFor(frame) == DisposeOps.APNGDisposeOpPrevious)
                {
                    //This is handled by default with this method of drawing.
                    //throw new NotImplementedException("Dispose operation previous has not been implemented yet");
                }

                //Blend operation
                if (image.GetBlendOperationFor(frame) == BlendOps.APNGBlendOpOver)
                {
                    GetCompositePixel(ref latestImage, curImage);
                }
                else if (image.GetBlendOperationFor(frame) == BlendOps.APNGBlendOpSource)
                {
                    //TODO: Fix this for bounce.png for some reason it the previous draw is not clearing. 
                    //(This works if every other frame. Not sure how to wait for the surface to have cleared...)
                    Graphics render = Graphics.FromImage(latestImage);

                    render.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                    render.Clear(Color.FromArgb(0x00000000));
                    render.DrawImage(curImage, 0, 0, latestImage.Width, latestImage.Height);
                    //g.Clear(Color.FromArgb(0x00000000));
                    //ClearBitmap(ref latestImage);
                    //DrawTo(ref latestImage, curImage);
                }
            }

            return latestImage;
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
                    if (srcPixel.A == 0)
                    {
                        finalPixel = Color.FromArgb(destPixel.ToArgb());
                    }
                    //Src is solid. Use the Src.
                    else if (srcPixel.A == 255)
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
    }
}
