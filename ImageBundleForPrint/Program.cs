using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;


namespace ImageBundleForPrint
{
    class Program
    {
        //Image grid
        int image_cols = 3;
        int image_rows = 3;

        //Image list
        List<string> image_paths = new List<string>();

        //Program params
        bool verbose = false;
        string paperSize = "a4";
        int dpi = 96;

        static void Main(string[] args)
        {
            // The code provided will print ‘Hello World’ to the console.
            // Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.
            // Go to http://aka.ms/dotnet-get-started-console to continue learning how to build a console app! 

            Program p = new Program();
            p.ReadParams();
            p.ReadInput(args);
            if (p.image_paths.Count > 0)
            {
                p.CreatePrintImage();
            }
        }

        void ReadParams()
        {
            string exeName = Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location);
            string[] arrExeName = exeName.Replace(".exe", "").Split('-');
            foreach (string param in arrExeName)
            {
                string value = param.Trim();
                if (value.StartsWith("s "))
                {
                    value = value.Replace("s ", "").ToLower();
                    if (value.Equals("a3") ||
                        value.Equals("a4") ||
                        value.Equals("a5"))
                    {
                        paperSize = value;
                    }
                    else
                    {
                        output("Paper size allowed, A3, A4, A5");
                    }
                }

                if (value.Equals("v"))
                {
                    verbose = true;
                    value = value.Replace("v ", "").ToLower();
                    if (value.Length>0 && value.Equals("false"))
                    {
                        verbose = false;
                    }
                }

                if (value.StartsWith("dpi "))
                {
                    value = value.Replace("dpi ", "").ToLower();
                    int intValue = int.Parse(value);
                    if (intValue > 20 && intValue <= 300 )
                    {
                        dpi = intValue;
                    }
                    else
                    {
                        output("Dpi needs to be between 20 and 300");
                    }
                }
            }
        }

        void ReadInput(string[] args)
        {
            output("Reading input");

            //loop all images, check if jpg
            for (int i = 0; i < args.Length; i++)
            {
                string filepath = args[i];
                if (!filepath.ToLower().EndsWith(".jpg"))
                {
                    output("Skipping file nr " + (i + 1) + " bad fileformat.");
                    continue;
                }

                if (!File.Exists(filepath))
                {
                    output("Skipping file nr " + (i + 1) + " missing file.");
                    continue;
                }

                //save to list
                image_paths.Add(filepath);

            }

            output(image_paths.Count + " images found");
        }

        void CreatePrintImage()
        {
            int outputWidth = 1024;
            int outputHeight = 1024;
            Bitmap bitmap;
            Graphics g;

            if (paperSize.Equals("a3"))
            {
                //get paper size in pixels
                outputWidth = CmToPx(29.7);
                outputHeight = CmToPx(42.0);

                //calc cols/rows in output
                image_cols = Convert.ToInt32(Math.Ceiling((double)image_paths.Count * 0.3)); //1 - 29.7/42.0
                image_rows = Convert.ToInt32(Math.Ceiling((double)image_paths.Count / image_cols));
            }

            if (paperSize.Equals("a4"))
            {
                //get paper size in pixels
                outputWidth = CmToPx(21.0);
                outputHeight = CmToPx(29.7);

                //calc cols/rows in output
                image_cols = Convert.ToInt32(Math.Ceiling((double)image_paths.Count * 0.3)); //1 - 21.0/29.7
                image_rows = Convert.ToInt32(Math.Ceiling((double)image_paths.Count / image_cols));
            }

            if (paperSize.Equals("a5"))
            {
                //get paper size in pixels
                outputWidth = CmToPx(14.8);
                outputHeight = CmToPx(21.0);

                //calc cols/rows in output
                image_cols = Convert.ToInt32(Math.Ceiling((double)image_paths.Count * 0.3)); //1 - 14.8/21.0
                image_rows = Convert.ToInt32(Math.Ceiling((double)image_paths.Count / image_cols));
            }

            //create new image
            bitmap = new Bitmap(outputWidth, outputHeight);
            g = Graphics.FromImage(bitmap);
            g.Clear(Color.White);

            //size of every image box
            int box_width = (int)Math.Floor((double)outputWidth / image_cols);
            int box_height = (int)Math.Floor((double)outputHeight / image_rows);

            //loop all images to place
            int image_index = 0;
            int pos_x = 0;
            int pos_y = 0;
            //loop rows
            for (int r = 0; r < image_rows; r++)
            {
                //loop cols
                pos_x = 0;
                for (int c = 0; c < image_cols; c++)
                {
                    //still more images
                    if (image_index < image_paths.Count)
                    {
                        string filename = image_paths[image_index];
                        g.DrawImage(resizeImage(filename, box_width, box_height),
                            pos_x,
                            pos_y
                            );
                        image_index++;
                    }
                    pos_x += box_width;
                }
                pos_y += box_height;
            }

            try
            {
                bitmap.Save("output.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                output( paperSize.Substring(0,1).ToUpper() + paperSize.Substring(1) + " output image saved");
            }
            catch (Exception e)
            {
                output("Error when saving output image: " + e.ToString());
            }

            if (verbose)
            {
                System.Threading.Thread.Sleep(10 * 1000);
            }
        }

        Image resizeImage(string imageFilename, int width, int height)
        {
            Image orgImage = Image.FromFile(imageFilename);
            Bitmap newImage = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(newImage);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            float percent = 1;
            float percentW = 1;
            float percentH = 1;

            percentW = (float)width / (float)orgImage.Width;
            percentH = (float)height / (float)orgImage.Height;
            if (percentW < percentH)
            {
                percent = percentW;
            }
            else
            {
                percent = percentH;
            }

            //image is to large, resize
            if (orgImage.Width > width || orgImage.Height > height)
            {
                //resize image
                int newWidth = (int)((float)orgImage.Width * percent);
                int newHeight = (int)((float)orgImage.Height * percent);

                int margin_x = (width - newWidth) / 2;
                int margin_y = (height - newHeight) / 2;

                g.DrawImage(orgImage, 0 + margin_x, 0 + margin_y, newWidth, newHeight);
            }
            else
            {
                //dont resize image
                int margin_x = (width - orgImage.Width) / 2;
                int margin_y = (height - orgImage.Height) / 2;

                g.DrawImage(orgImage, 0 + margin_x, 0 + margin_y, orgImage.Width, orgImage.Height);
            }

            //delete temp img
            orgImage.Dispose();

            //return new centered image
            return newImage;
        }

        int CmToPx(double cm)
        {
            return int.Parse(Math.Ceiling(cm * dpi / 2.54).ToString());
        }

        void output(string text)
        {
            if (verbose)
            {
                Console.WriteLine(text);
            }
        }
    }
}

