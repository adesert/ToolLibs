using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;


using SimpleJSON;

public class CutImageTools
{
    public CutImageTools()
    {
    }

    // 文件所属的文件夹 
    private string _fileDirectory;
    public void LayaCutImageByConfigJsonAndAltas()
    {
        string path3 = System.IO.Directory.GetCurrentDirectory();
        path3 += "\\img";
        string currentDir = "img";
        //this._fileDirectory = Path.GetDirectoryName(currentDir);
        this._fileDirectory += currentDir;
        if (!Directory.Exists(this._fileDirectory))
        {
            Directory.CreateDirectory(this._fileDirectory);
        }
        
        string[] files1 = Directory.GetFiles(path3, "*.json", SearchOption.AllDirectories);
        string[] files2 = Directory.GetFiles(path3, "*.altas", SearchOption.AllDirectories);
        string[] files = files1.Concat(files2).ToArray();
        int len = files.Length;
        for(int i = 0; i < len; i++)
        {
            string filePath = files[i];
            string txt = File.ReadAllText(filePath);
            string currentFile = filePath.Replace("\\", "/");
            string fileName = Path.GetFileNameWithoutExtension(currentFile);
            string currentDirPath = Path.GetDirectoryName(currentFile);
            string fileDir = currentDirPath;
            fileDir = fileDir + "\\" + fileName;
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }

            fileDir += "\\";

            //string imgPath = currentDirPath + "\\" +  fileName + ".png";
            //Image img = Image.FromFile(imgPath);
            //int imgWidth = img.Width;
            //int imgHeight = img.Height;

            JSONNode m = JSON.Parse(txt);
            JSONNode meta = m["meta"];
            float scale = meta["scale"];
            string imageName = meta["image"];
            string[] imgNameArr = imageName.Split(',');
            List<Image> imgList = new List<Image>();
            for(int k = 0; k < imgNameArr.Length; k++)
            {
                string p = currentDirPath + "\\" + imgNameArr[k];
                Image image = Image.FromFile(p);
                imgList.Add(image);
            }

            JSONNode n = m["frames"];
            JSONNode.KeyEnumerator keys = n.Keys;
            while (true)
            {
                if (!keys.MoveNext())
                {
                    break;
                }
                string CurrentKey = keys.Current;
                JSONNode node = n[CurrentKey];
                int x = node["frame"]["x"];
                int y = node["frame"]["y"];
                int w = node["frame"]["w"];
                int h = node["frame"]["h"];
                int idx = node["frame"]["idx"];
                int spriteSourceSizeX = node["spriteSourceSize"]["x"];
                int spriteSourceSizeY = node["spriteSourceSize"]["y"];
                int sourceSizeW = node["sourceSize"]["w"];
                int sourceSizeH = node["sourceSize"]["h"];

                Image srcImg = imgList[idx];

                Rectangle rect = new Rectangle(x, y, w, h);
                Bitmap newBmp = new Bitmap(sourceSizeW, sourceSizeH, PixelFormat.Format32bppArgb);
                //Bitmap newBmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
                Graphics newBmpGraphics = Graphics.FromImage(newBmp);
                ////设置高质量插值法      
                newBmpGraphics.InterpolationMode = InterpolationMode.High;
                ////设置高质量,低速度呈现平滑程度  
                newBmpGraphics.SmoothingMode = SmoothingMode.HighQuality;
                //int vx = (sourceSizeW - rect.Width) / 2;
                //int vy = (sourceSizeH - rect.Height) / 2;
                int vx = spriteSourceSizeX;
                int vy = spriteSourceSizeY;
                newBmpGraphics.DrawImage(srcImg, new Rectangle(vx, vy, rect.Width, rect.Height), rect, GraphicsUnit.Pixel);
                //newBmpGraphics.DrawImage(img, new Rectangle(vx,vy, rect.Width, rect.Height), rect, GraphicsUnit.Pixel);
                //newBmpGraphics.DrawImage(img, new Rectangle(0, 0, rect.Width, rect.Height), rect, GraphicsUnit.Pixel);
                newBmpGraphics.Save();
                string outPath = fileDir + CurrentKey;
                newBmp.Save(outPath, ImageFormat.Png);
            }
        }
    }
}
