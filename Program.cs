using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace BackgroundEasier
{
    class Program
    {
        private string basePath;
        Program(string basePath)
        {
                this.basePath = basePath;
                if (this.basePath[this.basePath.Length - 1] != '/' && this.basePath[this.basePath.Length - 1] != '\\')
                {
                    this.basePath = this.basePath + "/";
                }
        }

        static void Main(string[] args)
    {
            try
            {
                foreach (string arg in args)
                {
                    new Program(arg).ProcFolder(arg, true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        void ProcFolder(string inFolder, bool noFiles = false)
        {
            string[] folders = Directory.GetDirectories(inFolder);
            foreach(string folder in folders)
            {
                ProcFolder(folder);
            }
            if (!noFiles)
            {
                string[] files = Directory.GetFiles(inFolder);
                foreach(string file in files)
                {
                    ProcFile(file);
                }
                folders = Directory.GetDirectories(inFolder);
                files = Directory.GetFiles(inFolder);
                if (folders.Length + files.Length == 0)
                {
                    Directory.Delete(inFolder);
                }
            }
        }


        SolidBrush brush = new SolidBrush(Color.Transparent);
        void ProcFile(string file)
        {
            // It will not try to load anything that is not a png, jpg, jpeg, gif
            string ext = Path.GetExtension(file).ToLower();
            if (ext == ".png" ||
                ext == ".jpg" ||
                ext == ".jpeg" ||
                ext == ".gif")
            {
                string targetLoc = basePath + Path.GetFileNameWithoutExtension(file) + ".png";
                if (!File.Exists(targetLoc))
                {
                    //try and load as image, return if failed to.
                    Bitmap bitmap = null;
                    using (Bitmap lbitmap = new Bitmap(file))
                    {
                        bitmap = new Bitmap(lbitmap);
                    }

                    //separate path into lowercase no-spaces commands (split(new array[] {'/','\'})).
                    string[] commands = Path.GetDirectoryName(file).Substring(basePath.Length).ToLower().Split(new char[] { '/', '\\' });

                    //for each command (taking them from the end first) apply it to the image
                    for (int i = commands.Length - 1; i >= 0; i--)
                    {
                        string command = commands[i].Replace(" ", string.Empty);
                        string[] commSplit = command.Split('-');
                        switch (commSplit[0])
                        {
                            #region stretch
                            case "stretch":
                                int w = bitmap.Width;
                                int h = bitmap.Height;
                                bool max = false;
                                for (int j = 1; j < commSplit.Length; j++)
                                {
                                    if (commSplit[j] == "max")
                                    {
                                        max = true;
                                    }
                                    else
                                    {
                                        switch (commSplit[j][0])
                                        {
                                            case 'w':
                                                w = Convert.ToInt32(commSplit[j].Substring(1));
                                                break;
                                            case 'h':
                                                h = Convert.ToInt32(commSplit[j].Substring(1));
                                                break;
                                        }
                                    }
                                }

                                double width = w;
                                double height = h;
                                double scale = Math.Min(width / bitmap.Width, height / bitmap.Height);
                                if (max)
                                {
                                    scale = Math.Max(width / bitmap.Width, height / bitmap.Height);
                                }
                                int scaleWidth = (int)(bitmap.Width * scale);
                                int scaleHeight = (int)(bitmap.Height * scale);

                                Bitmap newBitmap = new Bitmap(scaleWidth, scaleHeight);
                                using (Graphics graphics = Graphics.FromImage(newBitmap))
                                {
                                    graphics.InterpolationMode = InterpolationMode.High;
                                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                                    graphics.Clear(Color.Transparent);
                                    graphics.DrawImage(bitmap, 0, 0, scaleWidth, scaleHeight);
                                }
                                bitmap = newBitmap;
                                break;
                            #endregion
                            #region underlay
                            case "underlay":
                                int R = 255;
                                int G = 255;
                                int B = 255;
                                for (int j = 1; j < commSplit.Length; j++)
                                {
                                    switch (commSplit[j][0])
                                    {
                                        case 't':
                                            w = Convert.ToInt32(commSplit[j].Substring(1));
                                            break;
                                        case 'r':
                                            R = Convert.ToInt32(commSplit[j].Substring(1));
                                            break;
                                        case 'g':
                                            G = Convert.ToInt32(commSplit[j].Substring(1));
                                            break;
                                        case 'b':
                                            B = Convert.ToInt32(commSplit[j].Substring(1));
                                            break;
                                    }
                                }

                                newBitmap = new Bitmap(bitmap.Width,bitmap.Height);
                                using (Graphics graphics = Graphics.FromImage(newBitmap))
                                {
                                    graphics.Clear(Color.FromArgb(R,G,B));
                                    graphics.DrawImageUnscaled(bitmap, new Point(0,0));
                                }
                                bitmap = newBitmap;
                                break;
                            #endregion
                            #region expand
                            case "expand":
                                w = bitmap.Width;
                                h = bitmap.Height;
                                bool t = false;
                                bool b = false;
                                bool l = false;
                                bool r = false;
                                for (int j = 1; j < commSplit.Length; j++)
                                {
                                    switch (commSplit[j][0])
                                    {
                                        case 'w':
                                            w = Convert.ToInt32(commSplit[j].Substring(1));
                                            break;
                                        case 'h':
                                            h = Convert.ToInt32(commSplit[j].Substring(1));
                                            break;
                                        case 't':
                                            t = true;
                                            break;
                                        case 'b':
                                            b = true;
                                            break;
                                        case 'l':
                                            l = true;
                                            break;
                                        case 'r':
                                            r = true;
                                            break;
                                    }
                                }

                                int y = 0;
                                if (t)
                                {
                                    y = 0;
                                }
                                else if (b)
                                {
                                    y = h - bitmap.Height;
                                }
                                else
                                {
                                    y = (h - bitmap.Height) / 2;
                                }

                                int x = 0;
                                if (l)
                                {
                                    x = 0;
                                }
                                else if (r)
                                {
                                    x = w - bitmap.Width;
                                }
                                else
                                {
                                    x = (w - bitmap.Width) / 2;
                                }

                                newBitmap = new Bitmap(w, h);
                                using (Graphics graphics = Graphics.FromImage(newBitmap))
                                {
                                    graphics.Clear(Color.Transparent);
                                    graphics.DrawImageUnscaled(bitmap, new Point(x,y));
                                }
                                bitmap = newBitmap;
                                break;
                            #endregion
                            #region border
                            case "border":
                                w = 3;
                                R = 0;
                                G = 0;
                                B = 0;
                                for (int j = 1; j < commSplit.Length; j++)
                                {
                                    switch (commSplit[j][0])
                                    {
                                        case 't':
                                            w = Convert.ToInt32(commSplit[j].Substring(1));
                                            break;
                                        case 'r':
                                            R = Convert.ToInt32(commSplit[j].Substring(1));
                                            break;
                                        case 'g':
                                            G = Convert.ToInt32(commSplit[j].Substring(1));
                                            break;
                                        case 'b':
                                            B = Convert.ToInt32(commSplit[j].Substring(1));
                                            break;
                                    }
                                }

                                newBitmap = new Bitmap(w*2+bitmap.Width, w*2+bitmap.Height);
                                using (Graphics graphics = Graphics.FromImage(newBitmap))
                                {
                                    graphics.Clear(Color.Transparent);
                                    graphics.DrawImageUnscaled(bitmap, new Point(w, w));
                                }
                                bitmap = newBitmap;

                                newBitmap = new Bitmap(bitmap.Width, bitmap.Height);
                                using (Graphics graphics = Graphics.FromImage(newBitmap))
                                {
                                    graphics.Clear(Color.FromArgb(R, G, B));
                                    graphics.DrawImageUnscaled(bitmap, new Point(0, 0));
                                }
                                bitmap = newBitmap;
                                break;
                                #endregion
                        }
                    }

                    //Try and save the image to the output directory without overwriting.
                    bitmap.Save(targetLoc, System.Drawing.Imaging.ImageFormat.Png);

                    //If successful, delete file.
                    File.Delete(file);
                }

            }
        }
    }
}
