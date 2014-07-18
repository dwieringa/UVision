// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace NetCams
{

    public class ColorAnalysisInstance : ToolInstance
    {
        public ColorAnalysisInstance(ColorAnalysisDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
        {
            if (theDefinition.SourceImage == null) throw new ArgumentException(Name + " doesn't have SourceImage defined.");
            mSourceImage = testExecution.ImageRegistry.GetObject(theDefinition.SourceImage.Name);

            if (theDefinition.ROI == null) throw new ArgumentException(Name + " doesn't have ROI defined.");
            mROI = testExecution.ROIRegistry.GetObject(theDefinition.ROI.Name);

            if (theDefinition.H_Average != null)
            {
                mH_Average = new GeneratedValueInstance(theDefinition.H_Average, testExecution);
            }
            if (theDefinition.H_Min != null)
            {
                mH_Min = new GeneratedValueInstance(theDefinition.H_Min, testExecution);
            }
            if (theDefinition.H_Max != null)
            {
                mH_Max = new GeneratedValueInstance(theDefinition.H_Max, testExecution);
            }
            if (theDefinition.H_StdDev != null)
            {
                mH_StdDev = new GeneratedValueInstance(theDefinition.H_StdDev, testExecution);
            }

            if (theDefinition.S_Average != null)
            {
                mS_Average = new GeneratedValueInstance(theDefinition.S_Average, testExecution);
            }
            if (theDefinition.S_Min != null)
            {
                mS_Min = new GeneratedValueInstance(theDefinition.S_Min, testExecution);
            }
            if (theDefinition.S_Max != null)
            {
                mS_Max = new GeneratedValueInstance(theDefinition.S_Max, testExecution);
            }
            if (theDefinition.S_StdDev != null)
            {
                mS_StdDev = new GeneratedValueInstance(theDefinition.S_StdDev, testExecution);
            }

            if (theDefinition.I_Average != null)
            {
                mI_Average = new GeneratedValueInstance(theDefinition.I_Average, testExecution);
            }
            if (theDefinition.I_Min != null)
            {
                mI_Min = new GeneratedValueInstance(theDefinition.I_Min, testExecution);
            }
            if (theDefinition.I_Max != null)
            {
                mI_Max = new GeneratedValueInstance(theDefinition.I_Max, testExecution);
            }
            if (theDefinition.I_StdDev != null)
            {
                mI_StdDev = new GeneratedValueInstance(theDefinition.I_StdDev, testExecution);
            }

            if (theDefinition.R_Average != null)
            {
                mR_Average = new GeneratedValueInstance(theDefinition.R_Average, testExecution);
            }
            if (theDefinition.R_Min != null)
            {
                mR_Min = new GeneratedValueInstance(theDefinition.R_Min, testExecution);
            }
            if (theDefinition.R_Max != null)
            {
                mR_Max = new GeneratedValueInstance(theDefinition.R_Max, testExecution);
            }
            if (theDefinition.R_StdDev != null)
            {
                mR_StdDev = new GeneratedValueInstance(theDefinition.R_StdDev, testExecution);
            }

            if (theDefinition.G_Average != null)
            {
                mG_Average = new GeneratedValueInstance(theDefinition.G_Average, testExecution);
            }
            if (theDefinition.G_Min != null)
            {
                mG_Min = new GeneratedValueInstance(theDefinition.G_Min, testExecution);
            }
            if (theDefinition.G_Max != null)
            {
                mG_Max = new GeneratedValueInstance(theDefinition.G_Max, testExecution);
            }
            if (theDefinition.G_StdDev != null)
            {
                mG_StdDev = new GeneratedValueInstance(theDefinition.G_StdDev, testExecution);
            }

            if (theDefinition.B_Average != null)
            {
                mB_Average = new GeneratedValueInstance(theDefinition.B_Average, testExecution);
            }
            if (theDefinition.B_Min != null)
            {
                mB_Min = new GeneratedValueInstance(theDefinition.B_Min, testExecution);
            }
            if (theDefinition.B_Max != null)
            {
                mB_Max = new GeneratedValueInstance(theDefinition.B_Max, testExecution);
            }
            if (theDefinition.B_StdDev != null)
            {
                mB_StdDev = new GeneratedValueInstance(theDefinition.B_StdDev, testExecution);
            }

            if (theDefinition.Grey_Average != null)
            {
                mGrey_Average = new GeneratedValueInstance(theDefinition.Grey_Average, testExecution);
            }
            if (theDefinition.Grey_Min != null)
            {
                mGrey_Min = new GeneratedValueInstance(theDefinition.Grey_Min, testExecution);
            }
            if (theDefinition.Grey_Max != null)
            {
                mGrey_Max = new GeneratedValueInstance(theDefinition.Grey_Max, testExecution);
            }
            if (theDefinition.Grey_StdDev != null)
            {
                mGrey_StdDev = new GeneratedValueInstance(theDefinition.Grey_StdDev, testExecution);
            }

        }

        private ImageInstance mSourceImage = null;
        [CategoryAttribute("Input")]
        public ImageInstance SourceImage
        {
            get { return mSourceImage; }
        }

        private ROIInstance mROI;
        [CategoryAttribute("Parameters"),
        DescriptionAttribute("ROI to test over")]
        public ROIInstance ROI
        {
            get { return mROI; }
        }

        private GeneratedValueInstance mH_Average = null;
        [CategoryAttribute("Output : HSI : H")]
        public GeneratedValueInstance H_Average
        {
            get { return mH_Average; }
        }

        private GeneratedValueInstance mH_Min = null;
        [CategoryAttribute("Output : HSI : H")]
        public GeneratedValueInstance H_Min
        {
            get { return mH_Min; }
        }

        private GeneratedValueInstance mH_Max = null;
        [CategoryAttribute("Output : HSI : H")]
        public GeneratedValueInstance H_Max
        {
            get { return mH_Max; }
        }

        private GeneratedValueInstance mH_StdDev = null;
        [CategoryAttribute("Output : HSI : H")]
        public GeneratedValueInstance H_StdDev
        {
            get { return mH_StdDev; }
        }

        private GeneratedValueInstance mS_Average = null;
        [CategoryAttribute("Output : HSI : S")]
        public GeneratedValueInstance S_Average
        {
            get { return mS_Average; }
        }

        private GeneratedValueInstance mS_Min = null;
        [CategoryAttribute("Output : HSI : S")]
        public GeneratedValueInstance S_Min
        {
            get { return mS_Min; }
        }

        private GeneratedValueInstance mS_Max = null;
        [CategoryAttribute("Output : HSI : S")]
        public GeneratedValueInstance S_Max
        {
            get { return mS_Max; }
        }

        private GeneratedValueInstance mS_StdDev = null;
        [CategoryAttribute("Output : HSI : S")]
        public GeneratedValueInstance S_StdDev
        {
            get { return mS_StdDev; }
        }

        private GeneratedValueInstance mI_Average = null;
        [CategoryAttribute("Output : HSI : I")]
        public GeneratedValueInstance I_Average
        {
            get { return mI_Average; }
        }

        private GeneratedValueInstance mI_Min = null;
        [CategoryAttribute("Output : HSI : I")]
        public GeneratedValueInstance I_Min
        {
            get { return mI_Min; }
        }

        private GeneratedValueInstance mI_Max = null;
        [CategoryAttribute("Output : HSI : I")]
        public GeneratedValueInstance I_Max
        {
            get { return mI_Max; }
        }

        private GeneratedValueInstance mI_StdDev = null;
        [CategoryAttribute("Output : HSI : I")]
        public GeneratedValueInstance I_StdDev
        {
            get { return mI_StdDev; }
        }

        private GeneratedValueInstance mR_Average = null;
        [CategoryAttribute("Output : RGB : R")]
        public GeneratedValueInstance R_Average
        {
            get { return mR_Average; }
        }

        private GeneratedValueInstance mR_Min = null;
        [CategoryAttribute("Output : RGB : R")]
        public GeneratedValueInstance R_Min
        {
            get { return mR_Min; }
        }

        private GeneratedValueInstance mR_Max = null;
        [CategoryAttribute("Output : RGB : R")]
        public GeneratedValueInstance R_Max
        {
            get { return mR_Max; }
        }

        private GeneratedValueInstance mR_StdDev = null;
        [CategoryAttribute("Output : RGB : R")]
        public GeneratedValueInstance R_StdDev
        {
            get { return mR_StdDev; }
        }

        private GeneratedValueInstance mG_Average = null;
        [CategoryAttribute("Output : RGB : G")]
        public GeneratedValueInstance G_Average
        {
            get { return mG_Average; }
        }

        private GeneratedValueInstance mG_Min = null;
        [CategoryAttribute("Output : RGB : G")]
        public GeneratedValueInstance G_Min
        {
            get { return mG_Min; }
        }

        private GeneratedValueInstance mG_Max = null;
        [CategoryAttribute("Output : RGB : G")]
        public GeneratedValueInstance G_Max
        {
            get { return mG_Max; }
        }

        private GeneratedValueInstance mG_StdDev = null;
        [CategoryAttribute("Output : RGB : G")]
        public GeneratedValueInstance G_StdDev
        {
            get { return mG_StdDev; }
        }

        private GeneratedValueInstance mB_Average = null;
        [CategoryAttribute("Output : RGB : B")]
        public GeneratedValueInstance B_Average
        {
            get { return mB_Average; }
        }

        private GeneratedValueInstance mB_Min = null;
        [CategoryAttribute("Output : RGB : B")]
        public GeneratedValueInstance B_Min
        {
            get { return mB_Min; }
        }

        private GeneratedValueInstance mB_Max = null;
        [CategoryAttribute("Output : RGB : B")]
        public GeneratedValueInstance B_Max
        {
            get { return mB_Max; }
        }

        private GeneratedValueInstance mB_StdDev = null;
        [CategoryAttribute("Output : RGB : B")]
        public GeneratedValueInstance B_StdDev
        {
            get { return mB_StdDev; }
        }

        private GeneratedValueInstance mGrey_Average = null;
        [CategoryAttribute("Output : HSI : H")]
        public GeneratedValueInstance Grey_Average
        {
            get { return mGrey_Average; }
        }

        private GeneratedValueInstance mGrey_Min = null;
        [CategoryAttribute("Output : HSI : H")]
        public GeneratedValueInstance Grey_Min
        {
            get { return mGrey_Min; }
        }

        private GeneratedValueInstance mGrey_Max = null;
        [CategoryAttribute("Output : HSI : H")]
        public GeneratedValueInstance Grey_Max
        {
            get { return mGrey_Max; }
        }

        private GeneratedValueInstance mGrey_StdDev = null;
        [CategoryAttribute("Output : HSI : H")]
        public GeneratedValueInstance Grey_StdDev
        {
            get { return mGrey_StdDev; }
        }

        private bool mIsComplete = false;
        public override bool IsComplete() { return mIsComplete; }

        public override void DoWork()
        {
            TestExecution().LogMessageWithTimeFromTrigger(Name + " started");
            Stopwatch watch = new Stopwatch();
            watch.Start();

            long numPixels = 0;
            int H_current;
            double H_avg = -1;
            int H_min = 999999;
            int H_max = -999999;
            double H_stddev = -1;
            long H_sum = 0;

            int S_current;
            double S_avg = -1;
            int S_min = 999999;
            int S_max = -999999;
            double S_stddev = -1;
            long S_sum = 0;

            int I_current;
            double I_avg = -1;
            int I_min = 999999;
            int I_max = -999999;
            double I_stddev = -1;
            long I_sum = 0;

            int R_current;
            double R_avg = -1;
            int R_min = 999999;
            int R_max = -999999;
            double R_stddev = -1;
            long R_sum = 0;

            int G_current;
            double G_avg = -1;
            int G_min = 999999;
            int G_max = -999999;
            double G_stddev = -1;
            long G_sum = 0;

            int B_current;
            double B_avg = -1;
            int B_min = 999999;
            int B_max = -999999;
            double B_stddev = -1;
            long B_sum = 0;

            int Grey_current;
            double Grey_avg = -1;
            int Grey_min = 999999;
            int Grey_max = -999999;
            double Grey_stddev = -1;
            long Grey_sum = 0;

            if (mPrerequisite != null && !mPrerequisite.ValueAsBoolean())
            {
                TestExecution().LogMessageWithTimeFromTrigger(Name + ": prerequisites not met. Skipping.");
            }
            else
            {
                if (true)
                {
                    if (mSourceImage != null && mSourceImage.Bitmap != null)
                    {
                        Point currentPoint = new Point(-1, -1);
                        mROI.GetFirstPointOnXAxis(mSourceImage, ref currentPoint);

                        Color color;
                        while (currentPoint.X > -1 && currentPoint.Y > -1)
                        {
                            numPixels++;

                            color = mSourceImage.GetColor(currentPoint.X, currentPoint.Y);

                            H_current = (int)(color.GetHue());
                            S_current = (int)(color.GetSaturation() * 100);
                            I_current = (int)(color.GetBrightness() * 100);
                            Grey_current = (int)(0.3 * color.R + 0.59 * color.G + 0.11 * color.B); // Then, add 30% of the red value, 59% of the green value, and 11% of the blue value, together. .... These percentages are chosen due to the different relative sensitivity of the normal human eye to each of the primary colors (less sensitive to green, more to blue).
                            R_current = color.R;
                            G_current = color.G;
                            B_current = color.B;

                            H_sum += H_current;
                            if (H_current < H_min) H_min = H_current;
                            if (H_current > H_max) H_max = H_current;

                            S_sum += S_current;
                            if (S_current < S_min) S_min = S_current;
                            if (S_current > S_max) S_max = S_current;

                            I_sum += I_current;
                            if (I_current < I_min) I_min = I_current;
                            if (I_current > I_max) I_max = I_current;

                            R_sum += R_current;
                            if (R_current < R_min) R_min = R_current;
                            if (R_current > R_max) R_max = R_current;

                            G_sum += G_current;
                            if (G_current < G_min) G_min = G_current;
                            if (G_current > G_max) G_max = G_current;

                            B_sum += B_current;
                            if (B_current < B_min) B_min = B_current;
                            if (B_current > B_max) B_max = B_current;

                            Grey_sum += Grey_current;
                            if (Grey_current < Grey_min) Grey_min = Grey_current;
                            if (Grey_current > Grey_max) Grey_max = Grey_current;

                            mROI.GetNextPointOnXAxis(mSourceImage, ref currentPoint);
                        }

                        if (numPixels > 0)
                        {
                            H_avg = ((double)H_sum) / numPixels;
                            S_avg = ((double)S_sum) / numPixels;
                            I_avg = ((double)I_sum) / numPixels;
                            R_avg = ((double)R_sum) / numPixels;
                            G_avg = ((double)G_sum) / numPixels;
                            B_avg = ((double)B_sum) / numPixels;
                            Grey_avg = ((double)Grey_sum) / numPixels;
                        }
                        else
                        {
                            TestExecution().LogErrorWithTimeFromTrigger("ColorAnalysis " + Name + " didn't analyze any pixels -- check ROI size.");
                        }
                    } // if image not null
                }
                else // if not new array pixel access, then use old pointer access
                {
                    Bitmap sourceBitmap = SourceImage.Bitmap;
                    if (sourceBitmap != null)
                    {
                        // for LockBits see http://www.bobpowell.net/lockingbits.htm & http://www.codeproject.com/csharp/quickgrayscale.asp?df=100&forumid=293759&select=2214623&msg=2214623
                        BitmapData sourceBitmapData = null;
                        try
                        {
                            sourceBitmapData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                            const int pixelByteWidth = 4; // determined by PixelFormat.Format32bppArgb
                            int stride = sourceBitmapData.Stride;
                            int strideOffset = stride - (sourceBitmapData.Width * pixelByteWidth);

                            Point currentPoint = new Point(-1, -1);
                            mROI.GetFirstPointOnXAxis(mSourceImage, ref currentPoint);

                            unsafe // see http://www.codeproject.com/csharp/quickgrayscale.asp?df=100&forumid=293759&select=2214623&msg=2214623
                            {
                                byte* sourcePointer;

                                Color color;
                                while (currentPoint.X > -1 && currentPoint.Y > -1)
                                {
                                    numPixels++;

                                    sourcePointer = (byte*)sourceBitmapData.Scan0; // init to first byte of image
                                    sourcePointer += (currentPoint.Y * stride) + (currentPoint.X * pixelByteWidth); // adjust to current point
                                    color = Color.FromArgb(sourcePointer[3], sourcePointer[2], sourcePointer[1], sourcePointer[0]); // Array index 0 is blue, 1 is green, 2 is red, 0 is alpha

                                    H_current = (int)(color.GetHue());
                                    S_current = (int)(color.GetSaturation() * 100);
                                    I_current = (int)(color.GetBrightness() * 100);
                                    Grey_current = (int)(0.3 * color.R + 0.59 * color.G + 0.11 * color.B); // Then, add 30% of the red value, 59% of the green value, and 11% of the blue value, together. .... These percentages are chosen due to the different relative sensitivity of the normal human eye to each of the primary colors (less sensitive to green, more to blue).
                                    R_current = color.R;
                                    G_current = color.G;
                                    B_current = color.B;

                                    H_sum += H_current;
                                    if (H_current < H_min) H_min = H_current;
                                    if (H_current > H_max) H_max = H_current;

                                    S_sum += S_current;
                                    if (S_current < S_min) S_min = S_current;
                                    if (S_current > S_max) S_max = S_current;

                                    I_sum += I_current;
                                    if (I_current < I_min) I_min = I_current;
                                    if (I_current > I_max) I_max = I_current;

                                    R_sum += R_current;
                                    if (R_current < R_min) R_min = R_current;
                                    if (R_current > R_max) R_max = R_current;

                                    G_sum += G_current;
                                    if (G_current < G_min) G_min = G_current;
                                    if (G_current > G_max) G_max = G_current;

                                    B_sum += B_current;
                                    if (B_current < B_min) B_min = B_current;
                                    if (B_current > B_max) B_max = B_current;

                                    Grey_sum += Grey_current;
                                    if (Grey_current < Grey_min) Grey_min = Grey_current;
                                    if (Grey_current > Grey_max) Grey_max = Grey_current;

                                    mROI.GetNextPointOnXAxis(mSourceImage, ref currentPoint);
                                }

                                if (numPixels > 0)
                                {
                                    H_avg = ((double)H_sum) / numPixels;
                                    S_avg = ((double)S_sum) / numPixels;
                                    I_avg = ((double)I_sum) / numPixels;
                                    R_avg = ((double)R_sum) / numPixels;
                                    G_avg = ((double)G_sum) / numPixels;
                                    B_avg = ((double)B_sum) / numPixels;
                                    Grey_avg = ((double)Grey_sum) / numPixels;
                                }
                                else
                                {
                                    TestExecution().LogErrorWithTimeFromTrigger("ColorAnalysis " + Name + " didn't analyze any pixels -- check ROI size.");
                                }

                            } // end unsafe block
                        }
                        finally
                        {
                            sourceBitmap.UnlockBits(sourceBitmapData);
                        }
                    } // if bitmap
                } // pixel access type
            } // if prereqs met
            if (mH_Average != null)
            {
                mH_Average.SetValue(H_avg);
                mH_Average.SetIsComplete();
            }
            if (mH_Min != null)
            {
                mH_Min.SetValue(H_min);
                mH_Min.SetIsComplete();
            }
            if (mH_Max != null)
            {
                mH_Max.SetValue(H_max);
                mH_Max.SetIsComplete();
            }
            if (mH_StdDev != null)
            {
                mH_StdDev.SetValue(H_stddev);
                mH_StdDev.SetIsComplete();
            }

            if (mS_Average != null)
            {
                mS_Average.SetValue(S_avg);
                mS_Average.SetIsComplete();
            }
            if (mS_Min != null)
            {
                mS_Min.SetValue(S_min);
                mS_Min.SetIsComplete();
            }
            if (mS_Max != null)
            {
                mS_Max.SetValue(S_max);
                mS_Max.SetIsComplete();
            }
            if (mS_StdDev != null)
            {
                mS_StdDev.SetValue(S_stddev);
                mS_StdDev.SetIsComplete();
            }

            if (mI_Average != null)
            {
                mI_Average.SetValue(I_avg);
                mI_Average.SetIsComplete();
            }
            if (mI_Min != null)
            {
                mI_Min.SetValue(I_min);
                mI_Min.SetIsComplete();
            }
            if (mI_Max != null)
            {
                mI_Max.SetValue(I_max);
                mI_Max.SetIsComplete();
            }
            if (mI_StdDev != null)
            {
                mI_StdDev.SetValue(I_stddev);
                mI_StdDev.SetIsComplete();
            }

            if (mR_Average != null)
            {
                mR_Average.SetValue(R_avg);
                mR_Average.SetIsComplete();
            }
            if (mR_Min != null)
            {
                mR_Min.SetValue(R_min);
                mR_Min.SetIsComplete();
            }
            if (mR_Max != null)
            {
                mR_Max.SetValue(R_max);
                mR_Max.SetIsComplete();
            }
            if (mR_StdDev != null)
            {
                mR_StdDev.SetValue(R_stddev);
                mR_StdDev.SetIsComplete();
            }

            if (mG_Average != null)
            {
                mG_Average.SetValue(G_avg);
                mG_Average.SetIsComplete();
            }
            if (mG_Min != null)
            {
                mG_Min.SetValue(G_min);
                mG_Min.SetIsComplete();
            }
            if (mG_Max != null)
            {
                mG_Max.SetValue(G_max);
                mG_Max.SetIsComplete();
            }
            if (mG_StdDev != null)
            {
                mG_StdDev.SetValue(G_stddev);
                mG_StdDev.SetIsComplete();
            }

            if (mB_Average != null)
            {
                mB_Average.SetValue(B_avg);
                mB_Average.SetIsComplete();
            }
            if (mB_Min != null)
            {
                mB_Min.SetValue(B_min);
                mB_Min.SetIsComplete();
            }
            if (mB_Max != null)
            {
                mB_Max.SetValue(B_max);
                mB_Max.SetIsComplete();
            }
            if (mB_StdDev != null)
            {
                mB_StdDev.SetValue(B_stddev);
                mB_StdDev.SetIsComplete();
            }

            if (mGrey_Average != null)
            {
                mGrey_Average.SetValue(Grey_avg);
                mGrey_Average.SetIsComplete();
            }
            if (mGrey_Min != null)
            {
                mGrey_Min.SetValue(Grey_min);
                mGrey_Min.SetIsComplete();
            }
            if (mGrey_Max != null)
            {
                mGrey_Max.SetValue(Grey_max);
                mGrey_Max.SetIsComplete();
            }
            if (mGrey_StdDev != null)
            {
                mGrey_StdDev.SetValue(Grey_stddev);
                mGrey_StdDev.SetIsComplete();
            }
            
            mIsComplete = true;

            watch.Stop();
            TestExecution().LogMessageWithTimeFromTrigger(Name + " took " + watch.ElapsedMilliseconds + "ms  ("+ watch.ElapsedTicks + " ticks for " + numPixels + " pixels)");
        }
    }
}

