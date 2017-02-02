using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
/**
 * さるの道具
 * **/
namespace luxintao
{

    // 鼠标状态类型
    public enum MouseStateType
    {
        MouseDown,
        MouseMove,
        MouseUp
    }
    class ToolUtils
    {
        //绘制多边形
        public static void DrawPolygon(Graphics g, Point[] p, Boolean isFill,Pen pen,Color c)
        {
            GraphicsPath tempGraphicsPath = new GraphicsPath();
            if (p.Length > 1)
            {
                tempGraphicsPath.AddLines(p.ToArray());
                if (isFill)
                {
                    g.FillPath(new SolidBrush(c), tempGraphicsPath);
                }
                g.DrawPath(pen, tempGraphicsPath);
            }
        }

        /**
         * 浮雕效果
         * **/
        public static Bitmap generate浮き彫(int Width,int Height,Bitmap oldBitmap){
            Bitmap newBitmap = new Bitmap(Width, Height);
            Color pixel1, pixel2;
            for (int x = 0; x < Width - 1; x++)
            {
                for (int y = 0; y < Height - 1; y++)
                {
                    int r = 0, g = 0, b = 0;
                    pixel1 = oldBitmap.GetPixel(x, y);
                    pixel2 = oldBitmap.GetPixel(x + 1, y + 1);
                    r = Math.Abs(pixel1.R - pixel2.R + 128);
                    g = Math.Abs(pixel1.G - pixel2.G + 128);
                    b = Math.Abs(pixel1.B - pixel2.B + 128);
                    if (r > 255)
                        r = 255;
                    if (r < 0)
                        r = 0;
                    if (g > 255)
                        g = 255;
                    if (g < 0)
                        g = 0;
                    if (b > 255)
                        b = 255;
                    if (b < 0)
                        b = 0;
                    newBitmap.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            return newBitmap;
        }

        /**
         * 黑白效果
         * **/
        public static Bitmap 白黒しゃしん(int temp_Width,int temp_Height, Bitmap oldBitmap){
            Bitmap newBitmap = new Bitmap(temp_Width, temp_Height);
            Color pixel;
            for (int x = 0; x < temp_Width; x++)
                for (int y = 0; y < temp_Height; y++)
                {
                    pixel = oldBitmap.GetPixel(x, y);
                    int r, g, b, Result = 0;
                    r = pixel.R;
                    g = pixel.G;
                    b = pixel.B;
                    //实例程序以加权平均值法产生黑白图像
                    int iType = 2;
                    switch (iType)
                    {
                        case 0://平均值法
                            Result = ((r + g + b) / 3);
                            break;
                        case 1://最大值法
                            Result = r > g ? r : g;
                            Result = Result > b ? Result : b;
                            break;
                        case 2://加权平均值法
                            Result = ((int)(0.7 * r) + (int)(0.2 * g) + (int)(0.1 * b));
                            break;
                    }
                    newBitmap.SetPixel(x, y, Color.FromArgb(Result, Result, Result));
                }
            return newBitmap;
        }

        /**
         * 柔化効果
         * **/
        public static Bitmap 柔化効果(int temp_Width, int temp_Height, Bitmap oldBitmap)
        {
            Bitmap bitmap = new Bitmap(temp_Width, temp_Height);
            Color pixel;
            //高斯模板
            int[] Gauss = { 1, 2, 1, 2, 4, 2, 1, 2, 1 };
            for (int x = 1; x < temp_Width - 1; x++)
                for (int y = 1; y < temp_Height - 1; y++)
                {
                    int r = 0, g = 0, b = 0;
                    int Index = 0;
                    for (int col = -1; col <= 1; col++)
                        for (int row = -1; row <= 1; row++)
                        {
                            pixel = oldBitmap.GetPixel(x + row, y + col);
                            r += pixel.R * Gauss[Index];
                            g += pixel.G * Gauss[Index];
                            b += pixel.B * Gauss[Index];
                            Index++;
                        }
                    r /= 16;
                    g /= 16;
                    b /= 16;
                    //处理颜色值溢出
                    r = r > 255 ? 255 : r;
                    r = r < 0 ? 0 : r;
                    g = g > 255 ? 255 : g;
                    g = g < 0 ? 0 : g;
                    b = b > 255 ? 255 : b;
                    b = b < 0 ? 0 : b;
                    bitmap.SetPixel(x - 1, y - 1, Color.FromArgb(r, g, b));
                }
            return bitmap;
        }
        /**
         * シャープ锐化
         * **/
        public static Bitmap シャープ(int temp_Width, int temp_Height, Bitmap oldBitmap)
        {
            Bitmap newBitmap = new Bitmap(temp_Width, temp_Height);
            Color pixel;
            //拉普拉斯模板
            int[] Laplacian = { -1, -1, -1, -1, 9, -1, -1, -1, -1 };
            for (int x = 1; x < temp_Width - 1; x++)
                for (int y = 1; y < temp_Height - 1; y++)
                {
                    int r = 0, g = 0, b = 0;
                    int Index = 0;
                    for (int col = -1; col <= 1; col++)
                        for (int row = -1; row <= 1; row++)
                        {
                            pixel = oldBitmap.GetPixel(x + row, y + col); r += pixel.R * Laplacian[Index];
                            g += pixel.G * Laplacian[Index];
                            b += pixel.B * Laplacian[Index];
                            Index++;
                        }
                    //处理颜色值溢出
                    r = r > 255 ? 255 : r;
                    r = r < 0 ? 0 : r;
                    g = g > 255 ? 255 : g;
                    g = g < 0 ? 0 : g;
                    b = b > 255 ? 255 : b;
                    b = b < 0 ? 0 : b;
                    newBitmap.SetPixel(x - 1, y - 1, Color.FromArgb(r, g, b));
                }
            return newBitmap;
        }

        /**
         * 灰度
         * **/
        public static Bitmap generate白黒階調(Bitmap currentBitmap)
        {
            Graphics g = Graphics.FromImage(currentBitmap);
            ImageAttributes ia = new ImageAttributes();
            float[][] colorMatrix =   {    
                new   float[]   {0.299f,   0.299f,   0.299f,   0,   0},
                new   float[]   {0.587f,   0.587f,   0.587f,   0,   0},
                new   float[]   {0.114f,   0.114f,   0.114f,   0,   0},
                new   float[]   {0,   0,   0,   1,   0},
                new   float[]   {0,   0,   0,   0,   1}
            };
            ColorMatrix cm = new ColorMatrix(colorMatrix);
            ia.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            g.DrawImage(currentBitmap, new Rectangle(0, 0, currentBitmap.Width, currentBitmap.Height), 0, 0, currentBitmap.Width, currentBitmap.Height, GraphicsUnit.Pixel, ia);
            return currentBitmap;
        }
        /**
         * 雾化效果
         * **/
        public static Bitmap generateふんむ(int temp_Width, int temp_Height, Bitmap oldBitmap)
        {
            Bitmap newBitmap = new Bitmap(temp_Width, temp_Height);
            Color pixel;
            for (int x = 1; x < temp_Width - 1; x++)
                for (int y = 1; y < temp_Height - 1; y++)
                {
                    System.Random MyRandom = new Random();
                    int k = MyRandom.Next(123456);
                    //像素块大小
                    int dx = x + k % 19;
                    int dy = y + k % 19;
                    if (dx >= temp_Width)
                        dx = temp_Width - 1;
                    if (dy >= temp_Height)
                        dy = temp_Height - 1;
                    pixel = oldBitmap.GetPixel(dx, dy);
                    newBitmap.SetPixel(x, y, pixel);
                }
            return newBitmap;
        }

        /**
         * 马赛克
         * **/
        public static Bitmap モザイクタイル(Bitmap m_PreImage, int val)
        {
            Bitmap MyBitmap = new Bitmap(m_PreImage);
            if (MyBitmap.Equals(null))
            {
                return null;
            }
            int iWidth = MyBitmap.Width;
            int iHeight = MyBitmap.Height;
            int stdR, stdG, stdB;
            stdR = 0;
            stdG = 0;
            stdB = 0;
            BitmapData srcData = MyBitmap.LockBits(new Rectangle(0, 0, iWidth, iHeight),
            ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* point = (byte*)srcData.Scan0.ToPointer();
                for (int i = 0; i < iHeight; i++)
                {
                    for (int j = 0; j < iWidth; j++)
                    {
                        if (i % val == 0)
                        {
                            if (j % val == 0)
                            {
                                stdR = point[2];
                                stdG = point[1];
                                stdB = point[0];
                            }
                            else
                            {
                                point[0] = (byte)stdB;
                                point[1] = (byte)stdG;
                                point[2] = (byte)stdR;
                            }
                        }
                        else
                        {  
                            byte* pTemp = point - srcData.Stride;
                            point[0] = (byte)pTemp[0];
                            point[1] = (byte)pTemp[1];
                            point[2] = (byte)pTemp[2];
                        }
                        point += 3;
                    }
                    point += srcData.Stride - iWidth * 3;
                }
                MyBitmap.UnlockBits(srcData);
            }
            return MyBitmap;
        } 



            

    }
}
