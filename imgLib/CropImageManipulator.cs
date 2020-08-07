using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.IO;

namespace imgLib
{
    public class CropImageManipulator
    {
        public CropImageManipulator()
        {
        }

        // 不含扩展名的文件名 
        private string _fileNameWithoutExtension;
        // 文件扩展名 
        private string _fileExtension;
        // 文件所属的文件夹 
        private string _fileDirectory;
        public void Cropping(string inputImgPath, int cropWidth, int cropHeight)
        {
            this._fileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputImgPath);
            this._fileExtension = Path.GetExtension(inputImgPath);
            this._fileDirectory = Path.GetDirectoryName(inputImgPath);

            string folderPath = this._fileDirectory + "img/";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            this._fileDirectory = folderPath;

            // 装载要分隔的图片 
            Image inputImg = Image.FromFile(inputImgPath);
            int imgWidth = inputImg.Width;
            int imgHeight = inputImg.Height;

            // 计算要分几格 
            int widthCount = (int)Math.Ceiling((imgWidth * 1.00) / (cropWidth * 1.00));
            int heightCount = (int)Math.Ceiling((imgHeight * 1.00) / (cropHeight * 1.00));
            //---------------------------------------------------------------------- 
            ArrayList areaList = new ArrayList();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<table cellpadding='0' cellspacing='0' border='[$border]'>");
            sb.Append(System.Environment.NewLine);

            int i = 0;
            for (int iHeight = 0; iHeight < heightCount; iHeight++)
            {
                sb.Append("<tr>");
                sb.Append(System.Environment.NewLine);
                for (int iWidth = 0; iWidth < widthCount; iWidth++)
                {
                    string fileName = string.Format("<img src='http://localhost/SRcommBeijingFile/{0}_{1}{2}'  />", this._fileNameWithoutExtension, i, this._fileExtension);
                    sb.Append("<td>" + fileName + "</td>");
                    sb.Append(System.Environment.NewLine);


                    int pointX = iWidth * cropWidth;
                    int pointY = iHeight * cropHeight;
                    int areaWidth = ((pointX + cropWidth) > imgWidth) ? (imgWidth - pointX) : cropWidth;
                    int areaHeight = ((pointY + cropHeight) > imgHeight) ? (imgHeight - pointY) : cropHeight;
                    string s = string.Format("{0};{1};{2};{3}", pointX, pointY, areaWidth, areaHeight);

                    Rectangle rect = new Rectangle(pointX, pointY, areaWidth, areaHeight);
                    areaList.Add(rect);
                    i++;
                }
                sb.Append("</tr>");
                sb.Append(System.Environment.NewLine);
            }

            sb.Append("</table>");


            //----------------------------------------------------------------------     

            for (int iLoop = 0; iLoop < areaList.Count; iLoop++)
            {
                Rectangle rect = (Rectangle)areaList[iLoop];
                string fileName = this._fileDirectory + "" + this._fileNameWithoutExtension + "_" + iLoop.ToString() + this._fileExtension;
                Bitmap newBmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
                Graphics newBmpGraphics = Graphics.FromImage(newBmp);
                //newBmpGraphics.Clear(Color.Transparent);//背景色正常为黑色

                ////设置高质量插值法      
                newBmpGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                ////设置高质量,低速度呈现平滑程度  
                newBmpGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                newBmpGraphics.DrawImage(inputImg, new Rectangle(0, 0, rect.Width, rect.Height), rect, GraphicsUnit.Pixel);
                newBmpGraphics.Save();

                switch (this._fileExtension.ToLower())
                {
                    case ".jpg":
                    case ".jpeg":
                        newBmp.Save(fileName, ImageFormat.Jpeg);
                        break;
                    case "gif":
                        newBmp.Save(fileName, ImageFormat.Gif);
                        break;
                    case ".png":
                        newBmp.Save(fileName, ImageFormat.Png);
                        break;
                    default:
                        newBmp.Save(fileName, ImageFormat.Png);
                        break;
                }

                Console.Write(fileName);
                Console.WriteLine();
            }
            inputImg.Dispose();
            string html = sb.ToString();
            //return html;
        }

    }

}
