using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Drawing.Imaging;

namespace luxintao
{
    public partial class lxt_form1 : Form
    {
        /*  @start基本属性*/
        private Bitmap img = null;
        private string fullname = null;//图像文件完整路径
        private int tempWidth, tempHeight;//图像变化信息，宽高属性

        private Color c;//前景色
        Color c1;//纯色；或渐变色的始止颜色；或者纹理的前景色和背景色
        private Color startcolor, endcolor;//用户设置前景色与背景色
        DashStyle line_type; //线型
        LineCap StartCap, EndCap;//起始端点形状,结束端点形状
        int colortype;//1 纯色，2 渐变色，3 纹理，4 图片填充
        LinearGradientMode lgm;//渐变方向
        HatchStyle hs; //纹理类型
        Bitmap fill_img;//填充图片
        int lineheight;//线宽
        Pen pen;//钢笔
        SolidBrush sb;//纯色画笔
        LinearGradientBrush lgb;//渐变画笔
        HatchBrush hb;//阴影画笔，可画纹理效果 
        TextureBrush tb;//纹理画笔，可用于图片填充
        int drawselect = 0;//绘制图形选项，1直线，2曲线，3弧线 4空心矩形，5实心矩形，6 空心椭圆，7实心椭圆，8空心多边形，9实心多边形，10 文本
        Point startpoint;//绘制的起始点
        Point targetPoint;//目标点
        bool domousemove = false;// 判断标记，是否为绘制时的鼠标移动
        ArrayList arrayPoint = new ArrayList();//存放绘制过程中的多个点的动态数组
        string drawstring = "";  //绘制的文本
        Font myfont = new Font("宋体", 12);//绘制文本的字体


        //水平旋转
        public const int Rotate180FlipY = 0;
        //垂直旋转
        public const int Rotate180FlipX = 1;
        //顺时针
        public const int Rotate90FlipNone = 2;
        //逆时针旋转90
        public const int Rotate270FlipNone = 3;


        public HatchBrush HB
        {
            get { return hb; }
            set { hb = value; }
        }
        public LinearGradientBrush LGB
        { //渐变画刷 
            get { return lgb; }
            set { lgb = value; }
        }
        public int ImageWidth
        {//图片宽构造函数
            get { return img.Width; }
            set { tempWidth = value; }
        }

        public int ImageHeight
        {//图片构造函数
            get { return img.Height; }
            set { tempHeight = value; }
        }

        public Color startColor
        {//前景色属性设置函数
            get { return c; }
            set { c = value; }
        }

        public Color commonColor
        {//纯色；或渐变色的始止颜色；或者纹理的前景色和背景色
            get { return c1; }
            set { c1 = value; }
        }

        public int ColorType
        {
            get { return colortype; }
            set { colortype = value; }
        }

        public Color StartColor
        {//前景色
            get { return startcolor; }
            set { startcolor = value; }
        }

        public Color EndColor
        {//背景色
            get { return endcolor; }
            set { endcolor = value; }
        }

        public HatchStyle HS
        {//纹理样式
            get { return hs; }
            set { hs = value; }
        }
        /*  @end基本属性*/

        public lxt_form1()
        {
            InitializeComponent();
        }

        public lxt_form1(string a)
        {
            InitializeComponent();
            fullname = a;
        }


        //窗体初始化
        private void Form1_Load(object sender, EventArgs e)
        {
            c = Color.Blue;
            c1 = Color.Green;
            line_type = DashStyle.Solid;
            startcolor = Color.Yellow; endcolor = Color.Green;
            colortype = 1;
            lineheight = 1;
            StartCap = LineCap.NoAnchor;
            EndCap = LineCap.NoAnchor;
            for (int i = 1; i < 6; i++)
            {
                toolStripComboBox1.ComboBox.Items.Add(i);
            }
            lgm = LinearGradientMode.Horizontal;
            hs = HatchStyle.DashedHorizontal;
            img = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(img);
            sb = new SolidBrush(Color.White);
            g.FillRectangle(sb, 0, 0, pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = img;
            if (fullname != null && !fullname.Equals(""))
            {
                Bitmap tempImage = new Bitmap(fullname);
                img = new Bitmap(tempImage.Width, tempImage.Height);
                Graphics draw = Graphics.FromImage(img);
                draw.DrawImage(tempImage, 0, 0, tempImage.Width, tempImage.Height);
                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
                pictureBox1.Image = img;
                draw.Dispose();
                tempImage.Dispose();
            }
        }

        /**
         * 打开文件
         */
        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "bmp,jpg,gif,png,tiff,icon|*.bmp;*.jpg;*.gif;*.png;*.tiff;*.icon";
            dialog.Title = "选择图片";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Bitmap tempImage = new Bitmap(dialog.FileName);
                img = new Bitmap(tempImage.Width, tempImage.Height);
                Graphics draw = Graphics.FromImage(img);
                draw.DrawImage(tempImage, 0, 0, tempImage.Width, tempImage.Height);
                fullname = dialog.FileName.ToString();
                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
                pictureBox1.Image = img;
                draw.Dispose();
                tempImage.Dispose();
            }
        }

        /**
         * 保存图片文件
         */
        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fullname == null || fullname.Equals(""))
            {//如果是第一次保存
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Jpeg图片|*.jpg|位图文件|*.bmp|Gif动图|*.gif|png|*.png|tiff|*.tiff|icon|*.icon";
                sfd.OverwritePrompt = true;
                sfd.Title = "保存图像";
                sfd.ValidateNames = true;
                sfd.RestoreDirectory = true;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    fullname = sfd.FileName;
                    switch (sfd.FilterIndex)
                    {
                        case 1: { img.Save(sfd.FileName, ImageFormat.Jpeg); break; }
                        case 2: { img.Save(sfd.FileName, ImageFormat.Bmp); break; }
                        case 3: { img.Save(sfd.FileName, ImageFormat.Gif); break; }
                        case 4: { img.Save(sfd.FileName, ImageFormat.Png); break; }
                        case 5: { img.Save(sfd.FileName, ImageFormat.Tiff); break; }
                        case 6: { img.Save(sfd.FileName, ImageFormat.Icon); break; }

                    }
                }
            }
            else
            {//文件已经存在
                img.Save(fullname);//直接保存
            }
        }

        /**
         *另存为功能 
         **/
        private void 另存为ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Jpeg图片|*.jpg|位图文件|*.bmp|Gif动图|*.gif|png|*.png|tiff|*.tiff|icon|*.icon";
            sfd.OverwritePrompt = true;
            sfd.Title = "另存为图像";
            sfd.ValidateNames = true;
            sfd.RestoreDirectory = true;
            sfd.InitialDirectory = "C:\\";//初始化目录
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                switch (sfd.FilterIndex)
                {
                    case 1: { img.Save(sfd.FileName, ImageFormat.Jpeg); break; }
                    case 2: { img.Save(sfd.FileName, ImageFormat.Bmp); break; }
                    case 3: { img.Save(sfd.FileName, ImageFormat.Gif); break; }
                    case 4: { img.Save(sfd.FileName, ImageFormat.Png); break; }
                    case 5: { img.Save(sfd.FileName, ImageFormat.Tiff); break; }
                    case 6: { img.Save(sfd.FileName, ImageFormat.Icon); break; }

                }
                fullname = sfd.FileName;
            }
        }

        /**
         * 新建事件
         * **/
        private void 新建ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            img = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(img);
            sb = new SolidBrush(Color.White);
            g.FillRectangle(sb, 0, 0, pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = img;
        }

        /**
         * 实际大小点击事件
         * **/
        private void 实际大小ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
        }

        /**
         * 放大事件
         * **/
        private void 放大ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Height = (int)Math.Ceiling(pictureBox1.Height * 1.1);
            pictureBox1.Width = (int)Math.Ceiling(pictureBox1.Width * 1.1);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }

        /**
         * 缩小点击事件
         * **/
        private void 缩小ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Height = (int)Math.Ceiling(pictureBox1.Height * 0.9);
            pictureBox1.Width = (int)Math.Ceiling(pictureBox1.Width * 0.9);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }

        /**
         * 图像大小设置
         * **/
        private void 图像大小设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
            {
                MessageBox.Show("未打开任何图片", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            tempWidth = img.Width;//赋值给宽的全局变量
            tempHeight = img.Height;//赋值给高的全局变量
            lxt_setsize_form sizeForm = new lxt_setsize_form();
            sizeForm.Owner = this;
            sizeForm.ShowDialog();
            if (tempHeight != img.Height || tempWidth != img.Width)
            {
                Bitmap tempImage = new Bitmap(tempWidth, tempHeight);
                Graphics draw = Graphics.FromImage(tempImage);
                draw.DrawImage(img, 0, 0, tempImage.Width, tempImage.Height);
                img = tempImage;
                pictureBox1.Image = img;
                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            }
        }

        /**
         * 水平翻转事件
         * */
        private void 水平翻转ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.convertBitmap(Rotate180FlipY);
        }

        /**
         * 旋转处理
         **/
        private void convertBitmap(int type)
        {
            if (img == null)
            {
                MessageBox.Show("未打开任何图片", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            tempWidth = img.Width;//赋值给宽的全局变量
            tempHeight = img.Height;//赋值给高的全局变量
            Bitmap tempImage = new Bitmap(tempWidth, tempHeight);
            Graphics draw = Graphics.FromImage(tempImage);
            switch (type)
            {
                case Rotate180FlipY:
                    {//水平旋转
                        img.RotateFlip(RotateFlipType.Rotate180FlipY);
                        break;
                    }
                case Rotate180FlipX:
                    { //垂直翻转
                        img.RotateFlip(RotateFlipType.Rotate180FlipX);
                        break;
                    }
                case Rotate90FlipNone:
                    { //顺时针翻转
                        img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    }
                case Rotate270FlipNone:
                    { //逆时针翻转
                        img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                    }
            }
            draw.DrawImage(img, 0, 0, tempImage.Width, tempImage.Height);
            img = tempImage;
            pictureBox1.Image = img;
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
        }
        /**
         *垂直翻转 
         **/
        private void 垂直翻转ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.convertBitmap(Rotate180FlipX);
        }
        /**
         * 顺时针旋转
         * **/
        private void 顺时针ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.convertBitmap(Rotate90FlipNone);
        }
        /**
         *逆时针旋转 
         **/
        private void 逆时针旋转90度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.convertBitmap(Rotate270FlipNone);
        }

        /**
         * 反色
         * **/
        private void 反色ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
            {
                MessageBox.Show("未打开任何图片", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            // 准备颜色变换矩阵的元素
            float[][] colorMatrixElements = {
              new float[] {-1, 0, 0, 0, 0},
              new float[] {0, -1, 0, 0, 0},
              new float[] {0, 0, -1, 0, 0},
              new float[] {0, 0, 0, 1, 0},
              new float[] {1, 1, 1, 0, 1}
             };
            // 为 ImageAttributes 设置颜色变换矩阵
            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.SetColorMatrix(colorMatrix,
                 ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            // 将 ImageAttributes 应用于绘制
            tempWidth = img.Width;//赋值给宽的全局变量
            tempHeight = img.Height;//赋值给高的全局变量
            Bitmap tempImage = new Bitmap(tempWidth, tempHeight);
            Graphics draw = Graphics.FromImage(tempImage);
            draw.DrawImage(img, new Rectangle(0, 0, tempWidth, tempHeight),
                 0, 0, tempWidth, tempHeight, GraphicsUnit.Pixel, imageAttributes);
            img = tempImage;
            pictureBox1.Image = img;
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
        }

        /**
         * 浮雕功能
         * **/
        private void 浮雕ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
            {
                MessageBox.Show("未打开任何图片", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //以浮雕效果显示图像
            try
            {
                int Height = this.pictureBox1.Image.Height;
                int Width = this.pictureBox1.Image.Width;
                Bitmap oldBitmap = (Bitmap)this.pictureBox1.Image;
                Bitmap newBitmap = ToolUtils.generate浮き彫(Width, Height, oldBitmap);
                img = newBitmap;
                this.pictureBox1.Image = newBitmap;
                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        /**
         * 黑い白い
         * **/
        private void 黑白ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
            {
                MessageBox.Show("未打开任何图片", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                int temp_Width = this.pictureBox1.Image.Width;
                int temp_Height = this.pictureBox1.Image.Height;
                Bitmap oldBitmap = (Bitmap)this.pictureBox1.Image;
                Bitmap newBitmap = ToolUtils.白黒しゃしん(temp_Width, temp_Height, oldBitmap);
                img = newBitmap;
                this.pictureBox1.Image = newBitmap;
                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        /**
         * 柔化
         * **/
        private void 柔化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
            {
                MessageBox.Show("未打开任何图片", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                int temp_Width = this.pictureBox1.Image.Width;
                int temp_Height = this.pictureBox1.Image.Height;
                Bitmap oldBitmap = (Bitmap)this.pictureBox1.Image;
                Bitmap newBitmap = ToolUtils.柔化効果(temp_Width, temp_Height, oldBitmap);
                img = newBitmap;
                this.pictureBox1.Image = newBitmap;
                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        /**
         * 锐化
         * **/
        private void 锐化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
            {
                MessageBox.Show("未打开任何图片", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                int temp_Width = this.pictureBox1.Image.Width;
                int temp_Height = this.pictureBox1.Image.Height;
                Bitmap oldBitmap = (Bitmap)this.pictureBox1.Image;
                Bitmap newBitmap = ToolUtils.シャープ(temp_Width, temp_Height, oldBitmap);
                img = newBitmap;
                this.pictureBox1.Image = newBitmap;
                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        /**
         * 灰度化
         * **/
        private void 灰度化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
            {
                MessageBox.Show("未打开任何图片", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                int temp_Width = this.pictureBox1.Image.Width;
                int temp_Height = this.pictureBox1.Image.Height;
                Bitmap oldBitmap = (Bitmap)this.pictureBox1.Image;
                Bitmap newBitmap = ToolUtils.generate白黒階調(oldBitmap);
                img = newBitmap;
                this.pictureBox1.Image = newBitmap;
                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        /**
         * 雾化
         * **/
        private void 雾化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
            {
                MessageBox.Show("未打开任何图片", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                int temp_Width = this.pictureBox1.Image.Width;
                int temp_Height = this.pictureBox1.Image.Height;
                Bitmap oldBitmap = (Bitmap)this.pictureBox1.Image;
                Bitmap newBitmap = ToolUtils.generateふんむ(temp_Width, temp_Height, oldBitmap);
                img = newBitmap;
                this.pictureBox1.Image = newBitmap;
                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        /**
         * 马赛克效果
         * **/
        private void 马赛克效果ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
            {
                MessageBox.Show("未打开任何图片", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                int temp_Width = this.pictureBox1.Image.Width;
                int temp_Height = this.pictureBox1.Image.Height;
                Bitmap oldBitmap = (Bitmap)this.pictureBox1.Image;
                Bitmap newBitmap = ToolUtils.モザイクタイル(oldBitmap, 6);
                img = newBitmap;
                this.pictureBox1.Image = newBitmap;
                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        /**
         * 线型设置
         * **/
        private void dashToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            string currentName = item.Name;
            switch (currentName)
            {
                case "dashToolStripMenuItem":
                    {//dash
                        line_type = DashStyle.Dash;
                        break;
                    }
                case "dashDotToolStripMenuItem":
                    {//DashDot
                        line_type = DashStyle.DashDot;
                        break;
                    }
                case "dashDotDotToolStripMenuItem":
                    {//dashdotdot
                        line_type = DashStyle.DashDotDot;
                        break;
                    }
                case "solidToolStripMenuItem":
                    {//Solid
                        line_type = DashStyle.Solid;
                        break;
                    }
                case "dotToolStripMenuItem":
                    {
                        line_type = DashStyle.Dot;
                        break;
                    }
            }
        }
        /**
         按钮线型
         */
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            string currentName = item.Name;
            switch (currentName)
            {
                case "toolStripMenuItem2":
                    {//dash
                        line_type = DashStyle.Dash;
                        break;
                    }
                case "dashToolStripMenuItem1":
                    {//DashDot
                        line_type = DashStyle.DashDot;
                        break;
                    }
                case "dashDotDotToolStripMenuItem1":
                    {//dashdotdot
                        line_type = DashStyle.DashDotDot;
                        break;
                    }
                case "solidToolStripMenuItem1":
                    {//Solid
                        line_type = DashStyle.Solid;
                        break;
                    }
                case "dotToolStripMenuItem1":
                    {
                        line_type = DashStyle.Dot;
                        break;
                    }
            }
        }
        /**
         *起始端点事件
         **/
        private void arrowAnchorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            string currentName = item.Name;
            switch (currentName)
            {
                case "dashToolStripMenuItem":
                    {//ArrowAnchor
                        StartCap = LineCap.ArrowAnchor;
                        break;
                    }
                case "diamondAnchorToolStripMenuItem":
                    {
                        StartCap = LineCap.DiamondAnchor;
                        break;
                    }
                case "squareAnchorToolStripMenuItem":
                    {
                        StartCap = LineCap.SquareAnchor;
                        break;
                    }
                case "triangleToolStripMenuItem":
                    {
                        StartCap = LineCap.Triangle;
                        break;
                    }
                case "roundAnchorToolStripMenuItem":
                    {
                        StartCap = LineCap.RoundAnchor;
                        break;
                    }
                case "noanchorToolStripMenuItem1":
                    {
                        StartCap = LineCap.NoAnchor;
                        break;
                    }
            }
        }
        /**
         *终点端点事件 
         **/
        private void arrowAnchorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            string currentName = item.Name;
            switch (currentName)
            {
                case "arrowAnchorToolStripMenuItem1":
                    {
                        EndCap = LineCap.ArrowAnchor;
                        break;
                    }
                case "diamondAnchorToolStripMenuItem1":
                    {
                        EndCap = LineCap.DiamondAnchor;
                        break;
                    }
                case "squareAnchorToolStripMenuItem1":
                    {
                        EndCap = LineCap.SquareAnchor;
                        break;
                    }
                case "triangleToolStripMenuItem1":
                    {
                        EndCap = LineCap.Triangle;
                        break;
                    }
                case "roundAnchorToolStripMenuItem1":
                    {
                        EndCap = LineCap.RoundAnchor;
                        break;
                    }
                case "noanchorToolStripMenuItem":
                    {
                        EndCap = LineCap.NoAnchor;
                        break;
                    }
            }
        }
        /**
         * 無地事件
         *纯色事件 
         **/
        private void 纯色ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            dialog.AllowFullOpen = true;
            dialog.Color = this.color_show.BackColor;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                c = dialog.Color;
                this.color_show.BackColor = c;
                this.color_show.Text = "";
                colortype = 1;
            }
        }
        /**
         * グラデーション色設定
         *渐变色设置 
         **/
        private void 渐变色ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LXT_SetGradient_Form grand = new LXT_SetGradient_Form();
            grand.Owner = this;
            grand.ShowDialog();
            if (colortype == 2)
            {
                this.color_show.Text = "渐变";
                color_show.BackColor = Color.White;
            }
        }
        /**
         *木目ボタンのクリック 件
         * 纹理按钮点击事件
         * **/
        private void 纹理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LXT_sethatch_form sh = new LXT_sethatch_form();
            sh.Owner = this;
            sh.ShowDialog();
            if (colortype == 3)
            {
                this.color_show.Text = "纹理";
                color_show.BackColor = Color.White;
            }
        }
        /**
         * しやしん
         * 图片填充
         * **/
        private void 图片填充ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "bmp,jpg,gif,png,tiff,icon|*.bmp;*.jpg;*.gif;*.png;*.tiff;*.icon";
            dialog.Title = "选择图片";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                fill_img = (Bitmap)Image.FromFile(dialog.FileName);
            }
        }
        /**
         * 绘制直线
         * **/
        private void toolStripButton18_Click(object sender, EventArgs e)
        {
            drawselect = 1;
            this.Cursor = Cursors.Cross;
        }
        /**
         * 取消绘图
         * **/
        private void toolStripButton17_Click(object sender, EventArgs e)
        {
            drawselect = 0;
            this.Cursor = Cursors.Default;
        }
        /**
         * 鼠标按下
         * **/
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (drawselect == 0) return;
            startpoint = new Point(e.X, e.Y);
            targetPoint = new Point(e.X, e.Y);
            arrayPoint.Add(startpoint);//起点加入点阵数组
            domousemove = true;//绘制图形开始
            pictureBox1.Image = (Bitmap)this.img.Clone();
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            switch (drawselect)
            {
                case 10:
                    {
                        if (alphaTextBox.Visible)
                        {
                            for (int i = 0; i < alphaTextBox.Lines.Length; ++i)
                            {
                                drawstring += alphaTextBox.Lines[i];
                                Point newLocation = new Point(alphaTextBox.Location.X, alphaTextBox.Location.Y + i * (alphaTextBox.Font.Height - 5));
                                g.DrawString(alphaTextBox.Lines[i], alphaTextBox.Font, new SolidBrush(Color.FromArgb(255, alphaTextBox.ForeColor)), newLocation);
                            }
                            alphaTextBox.Text = "";
                            alphaTextBox.Visible = false;
                        }
                        else
                        {
                            alphaTextBox.Location = startpoint;
                            alphaTextBox.ForeColor = c;
                            alphaTextBox.Font = new Font(alphaTextBox.Font.FontFamily, 22, alphaTextBox.Font.Style);
                            alphaTextBox.Visible = true;
                        }
                        break;
                    }
            }
        }
        /**
         * 鼠标移动事件
         * **/
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (domousemove == false) return;
            pictureBox1.Image = (Bitmap)this.img.Clone();
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            switch (drawselect)
            {
                case 1:
                    { //直线
                        pen = new Pen(c, float.Parse(toolStripComboBox1.Text));//颜色与线宽
                        pen.DashStyle = line_type;//设置线型
                        pen.StartCap = StartCap;//起点形状
                        pen.EndCap = EndCap;//终点形状
                        g.DrawLine(pen, startpoint, new Point(e.X, e.Y));
                        break;
                    }
                case 2:
                    {//曲线 
                        pen = new Pen(c, float.Parse(toolStripComboBox1.Text));//颜色与线宽
                        pen.DashStyle = line_type;//设置线型
                        pen.StartCap = StartCap;//起点形状
                        pen.EndCap = EndCap;//终点形状
                        Point[] drawPoint = new Point[arrayPoint.Count + 1];
                        int i = 0;
                        foreach (Point p in arrayPoint) drawPoint[i++] = p;
                        drawPoint[i] = new Point(e.X, e.Y);//记录当前坐标点
                        g.DrawCurve(pen, drawPoint);
                        break;
                    }
                case 3:
                    {//弧线
                        pen = new Pen(c, float.Parse(toolStripComboBox1.Text));//颜色与线宽
                        pen.DashStyle = line_type;//设置线型
                        pen.StartCap = StartCap;//起点形状
                        pen.EndCap = EndCap;//终点形状
                        Point[] drawPoint = new Point[arrayPoint.Count + 1];
                        int i = 0;
                        foreach (Point p in arrayPoint) drawPoint[i++] = p;
                        drawPoint[i] = new Point(e.X, e.Y);//记录当前坐标点
                        g.DrawCurve(pen, drawPoint);
                        break;
                    }
                case 4:
                    { //空心矩形
                        pen = new Pen(c, float.Parse(toolStripComboBox1.Text));//颜色与线宽
                        pen.DashStyle = line_type;//设置线型
                        pen.StartCap = StartCap;//起点形状
                        pen.EndCap = EndCap;//终点形状
                        Point staticPoint = new Point((startpoint.X < e.X) ? startpoint.X : e.X, (startpoint.Y < e.Y) ? startpoint.Y : e.Y);
                        targetPoint.X = Math.Abs(e.X - startpoint.X);
                        targetPoint.Y = Math.Abs(e.Y - startpoint.Y);
                        g.DrawRectangle(pen, staticPoint.X, staticPoint.Y, targetPoint.X, targetPoint.Y);
                        break;
                    }
                case 5:
                    {//实心矩形
                        pen = new Pen(c, float.Parse(toolStripComboBox1.Text));//颜色与线宽
                        pen.DashStyle = line_type;//设置线型
                        pen.StartCap = StartCap;//起点形状
                        pen.EndCap = EndCap;//终点形状
                        Brush brush = new SolidBrush(c);
                        Point staticPoint = new Point((startpoint.X < e.X) ? startpoint.X : e.X, (startpoint.Y < e.Y) ? startpoint.Y : e.Y);
                        targetPoint.X = Math.Abs(e.X - startpoint.X);
                        targetPoint.Y = Math.Abs(e.Y - startpoint.Y);
                        if (hb != null) { g.FillRectangle(hb, new Rectangle(staticPoint.X, staticPoint.Y, targetPoint.X, targetPoint.Y)); }
                        else
                        {
                            g.FillRectangle(brush, new Rectangle(staticPoint.X, staticPoint.Y, targetPoint.X, targetPoint.Y));
                        }
                        g.DrawRectangle(pen, staticPoint.X, staticPoint.Y, targetPoint.X, targetPoint.Y);
                        break;
                    }
                case 6:
                    {//空心椭圆
                        pen = new Pen(c, float.Parse(toolStripComboBox1.Text));//颜色与线宽
                        pen.DashStyle = line_type;//设置线型
                        pen.StartCap = StartCap;//起点形状
                        pen.EndCap = EndCap;//终点形状
                        Point staticPoint = new Point((startpoint.X < e.X) ? startpoint.X : e.X, (startpoint.Y < e.Y) ? startpoint.Y : e.Y);
                        targetPoint.X = Math.Abs(e.X - startpoint.X);
                        targetPoint.Y = Math.Abs(e.Y - startpoint.Y);
                        g.DrawEllipse(pen, new Rectangle(staticPoint.X, staticPoint.Y, targetPoint.X, targetPoint.Y));
                        break;
                    }
                case 7:
                    {//实心椭圆
                        pen = new Pen(c, float.Parse(toolStripComboBox1.Text));//颜色与线宽
                        pen.DashStyle = line_type;//设置线型
                        pen.StartCap = StartCap;//起点形状
                        pen.EndCap = EndCap;//终点形状
                        Brush brush = new SolidBrush(c);
                        Point staticPoint = new Point((startpoint.X < e.X) ? startpoint.X : e.X, (startpoint.Y < e.Y) ? startpoint.Y : e.Y);
                        targetPoint.X = Math.Abs(e.X - startpoint.X);
                        targetPoint.Y = Math.Abs(e.Y - startpoint.Y);
                        g.FillEllipse(brush, new Rectangle(staticPoint.X, staticPoint.Y, targetPoint.X, targetPoint.Y));
                        g.DrawEllipse(pen, new Rectangle(staticPoint.X, staticPoint.Y, targetPoint.X, targetPoint.Y));
                        break;
                    }
                case 8:
                    {//空心多边形
                        pen = new Pen(c, float.Parse(toolStripComboBox1.Text));//颜色与线宽
                        pen.DashStyle = line_type;//设置线型
                        pen.StartCap = StartCap;//起点形状
                        pen.EndCap = EndCap;//终点形状
                        Point[] drawPoint = new Point[arrayPoint.Count + 1];
                        int i = 0;
                        foreach (Point p in arrayPoint) drawPoint[i++] = p;
                        drawPoint[i] = new Point(e.X, e.Y);
                        g.DrawLines(pen, drawPoint);
                        break;
                    }
                case 9:
                    {//实心多边形
                        pen = new Pen(c, float.Parse(toolStripComboBox1.Text));//颜色与线宽
                        pen.DashStyle = line_type;//设置线型
                        pen.StartCap = StartCap;//起点形状
                        pen.EndCap = EndCap;//终点形状
                        Point[] drawPoint = new Point[arrayPoint.Count + 1];
                        int i = 0;
                        foreach (Point p in arrayPoint) drawPoint[i++] = p;
                        drawPoint[i] = new Point(e.X, e.Y);
                        g.DrawLines(pen, drawPoint);
                        break;
                    }
                case 10:
                    {//文字添加
                        Point staticPoint = new Point((startpoint.X < e.X) ? startpoint.X : e.X, (startpoint.Y < e.Y) ? startpoint.Y : e.Y);
                        targetPoint.X = Math.Abs(e.X - startpoint.X);
                        targetPoint.Y = Math.Abs(e.Y - startpoint.Y);
                        if (alphaTextBox.Visible)
                        {
                            alphaTextBox.Location = new Point((staticPoint.X < targetPoint.X) ? staticPoint.X : targetPoint.X, (staticPoint.Y < targetPoint.Y) ? staticPoint.Y : targetPoint.Y);
                            alphaTextBox.Size = new Size(targetPoint.X, targetPoint.Y);
                        }
                        break;
                    }
            }
            g.Dispose();
        }

        /**
         * mouse up じけん
         * **/
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (domousemove == false) return;
            pictureBox1.Image = this.img;
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            switch (drawselect)
            {
                case 1:
                    {//直线
                        pen = new Pen(c, float.Parse(toolStripComboBox1.Text));
                        pen.DashStyle = line_type;
                        pen.StartCap = StartCap;
                        pen.EndCap = EndCap;
                        g.DrawLine(pen, startpoint, new Point(e.X, e.Y));
                        arrayPoint.Clear();
                        domousemove = false;
                        drawselect = 0;
                        this.Cursor = Cursors.Default;
                        break;
                    }
                case 4:
                    {//空心矩形
                        pen = new Pen(c, float.Parse(toolStripComboBox1.Text));
                        pen.DashStyle = line_type;
                        pen.StartCap = StartCap;
                        pen.EndCap = EndCap;
                        Point staticPoint = new Point((startpoint.X < e.X) ? startpoint.X : e.X, (startpoint.Y < e.Y) ? startpoint.Y : e.Y);
                        targetPoint.X = Math.Abs(e.X - startpoint.X);
                        targetPoint.Y = Math.Abs(e.Y - startpoint.Y);
                        g.DrawRectangle(pen, staticPoint.X, staticPoint.Y, targetPoint.X, targetPoint.Y);
                        arrayPoint.Clear();
                        domousemove = false;
                        drawselect = 0;
                        this.Cursor = Cursors.Default;
                        break;
                    }
                case 5:
                    {//实心矩形
                        pen = new Pen(c, float.Parse(toolStripComboBox1.Text));
                        pen.DashStyle = line_type;
                        pen.StartCap = StartCap;
                        pen.EndCap = EndCap;
                        Brush brush = new SolidBrush(c);
                        Point staticPoint = new Point((startpoint.X < e.X) ? startpoint.X : e.X, (startpoint.Y < e.Y) ? startpoint.Y : e.Y);
                        targetPoint.X = Math.Abs(e.X - startpoint.X);
                        targetPoint.Y = Math.Abs(e.Y - startpoint.Y);
                        if (hb != null) { g.FillRectangle(hb, new Rectangle(staticPoint.X, staticPoint.Y, targetPoint.X, targetPoint.Y)); }
                        else
                        {
                            g.FillRectangle(brush, new Rectangle(staticPoint.X, staticPoint.Y, targetPoint.X, targetPoint.Y));
                        }
                        g.DrawRectangle(pen, staticPoint.X, staticPoint.Y, targetPoint.X, targetPoint.Y);
                        arrayPoint.Clear();
                        domousemove = false;
                        drawselect = 0;
                        this.Cursor = Cursors.Default;
                        break;
                    }
                case 6:
                    {//空心椭圆
                        pen = new Pen(c, float.Parse(toolStripComboBox1.Text));
                        pen.DashStyle = line_type;
                        pen.StartCap = StartCap;
                        pen.EndCap = EndCap;
                        Point staticPoint = new Point((startpoint.X < e.X) ? startpoint.X : e.X, (startpoint.Y < e.Y) ? startpoint.Y : e.Y);
                        targetPoint.X = Math.Abs(e.X - startpoint.X);
                        targetPoint.Y = Math.Abs(e.Y - startpoint.Y);
                        g.DrawEllipse(pen, staticPoint.X, staticPoint.Y, targetPoint.X, targetPoint.Y);
                        arrayPoint.Clear();
                        domousemove = false;
                        drawselect = 0;
                        this.Cursor = Cursors.Default;
                        break;
                    }
                case 7:
                    {//实心椭圆
                        pen = new Pen(c, float.Parse(toolStripComboBox1.Text));
                        pen.DashStyle = line_type;
                        pen.StartCap = StartCap;
                        pen.EndCap = EndCap;
                        Brush brush = new SolidBrush(c);
                        Point staticPoint = new Point((startpoint.X < e.X) ? startpoint.X : e.X, (startpoint.Y < e.Y) ? startpoint.Y : e.Y);
                        targetPoint.X = Math.Abs(e.X - startpoint.X);
                        targetPoint.Y = Math.Abs(e.Y - startpoint.Y);
                        g.FillEllipse(brush, new Rectangle(staticPoint.X, staticPoint.Y, targetPoint.X, targetPoint.Y));
                        g.DrawEllipse(pen, new Rectangle(staticPoint.X, staticPoint.Y, targetPoint.X, targetPoint.Y));
                        arrayPoint.Clear();
                        domousemove = false;
                        drawselect = 0;
                        this.Cursor = Cursors.Default;
                        break;
                    }
                case 10:
                    {//文字
                        startpoint = Point.Empty;
                        domousemove = false;
                        //drawselect = 0;
                        this.Cursor = Cursors.Default;
                        break;
                    }
            }
            g.Dispose();
        }
        /**
         * 鼠标双击事件
         * **/
        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (domousemove == false) return;
            pictureBox1.Image = this.img;
            Graphics g = Graphics.FromImage(pictureBox1.Image);

            pen = new Pen(c, float.Parse(toolStripComboBox1.Text));
            pen.DashStyle = line_type;
            pen.StartCap = StartCap;
            pen.EndCap = EndCap;
            switch (drawselect)
            {
                case 2:
                    {//曲线
                        arrayPoint.RemoveAt(arrayPoint.Count - 1);
                        Point[] drawPoint = new Point[arrayPoint.Count];
                        int i = 0;
                        foreach (Point p in arrayPoint) drawPoint[i++] = p;
                        g.DrawCurve(pen, drawPoint);
                        break;
                    }
                case 3:
                    {//弧线
                        if (arrayPoint.Count < 4) return;
                        arrayPoint.RemoveAt(arrayPoint.Count - 1);
                        Point[] drawPoint = new Point[arrayPoint.Count];
                        int i = 0;
                        foreach (Point p in arrayPoint) drawPoint[i++] = p;
                        g.DrawBezier(pen, drawPoint[0], drawPoint[1], drawPoint[2], drawPoint[3]);
                        break;
                    }
                case 8:
                    {//空心多边形
                        arrayPoint.RemoveAt(arrayPoint.Count - 1);
                        Point[] drawPoint = new Point[arrayPoint.Count];
                        int i = 0;
                        foreach (Point p in arrayPoint) drawPoint[i++] = p;
                        ToolUtils.DrawPolygon(g, drawPoint, false, pen, c);
                        break;
                    }
                case 9:
                    {//实心多边形
                        arrayPoint.RemoveAt(arrayPoint.Count - 1);
                        Point[] drawPoint = new Point[arrayPoint.Count];
                        int i = 0;
                        foreach (Point p in arrayPoint) drawPoint[i++] = p;
                        ToolUtils.DrawPolygon(g, drawPoint, true, pen, c);
                        break;
                    }
            }
            domousemove = false;
            drawselect = 0;
            this.Cursor = Cursors.Default;
            arrayPoint.Clear();
            g.Dispose();
        }


        /**
         * 绘制曲线
         * **/
        private void toolStripButton19_Click(object sender, EventArgs e)
        {
            drawselect = 2;
            this.Cursor = Cursors.Cross;
        }
        /**
         * 绘制弧线
         * **/
        private void toolStripButton20_Click(object sender, EventArgs e)
        {
            drawselect = 3;
            this.Cursor = Cursors.Cross;
        }

        /**
         * 空心矩形
         * **/
        private void toolStripButton21_Click(object sender, EventArgs e)
        {
            drawselect = 4;
            this.Cursor = Cursors.Cross;
        }
        /**
         * 实心矩形
         * **/
        private void toolStripButton22_Click(object sender, EventArgs e)
        {
            drawselect = 5;
            this.Cursor = Cursors.Cross;
        }
        /**
         * 空心椭圆
         * **/
        private void toolStripButton23_Click(object sender, EventArgs e)
        {
            drawselect = 6;
            this.Cursor = Cursors.Cross;
        }
        /**
         * 实心椭圆
         * **/
        private void toolStripButton24_Click(object sender, EventArgs e)
        {
            drawselect = 7;
            this.Cursor = Cursors.Cross;
        }
        /**
         * 空心多边形
         * **/
        private void toolStripButton25_Click(object sender, EventArgs e)
        {
            drawselect = 8;
            this.Cursor = Cursors.Cross;
        }
        /**
         * 实心多边形
         * **/
        private void toolStripButton26_Click(object sender, EventArgs e)
        {
            drawselect = 9;
            this.Cursor = Cursors.Cross;
        }
        /**
         * 文字をついか
         * **/
        private void toolStripButton27_Click(object sender, EventArgs e)
        {
            drawselect = 10;
            this.Cursor = Cursors.Cross;
        }

        /**
         * 退出功能
         * **/
        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (null != img)
            {
                DialogResult result = MessageBox.Show("正在编辑，是否保存", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (fullname == null || fullname.Equals(""))
                    {//如果是第一次保存
                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.Filter = "Jpeg图片|*.jpg|位图文件|*.bmp|Gif动图|*.gif|png|*.png|tiff|*.tiff|icon|*.icon";
                        sfd.OverwritePrompt = true;
                        sfd.Title = "保存图像";
                        sfd.ValidateNames = true;
                        sfd.RestoreDirectory = true;
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            fullname = sfd.FileName;
                            switch (sfd.FilterIndex)
                            {
                                case 1: { img.Save(sfd.FileName, ImageFormat.Jpeg); break; }
                                case 2: { img.Save(sfd.FileName, ImageFormat.Bmp); break; }
                                case 3: { img.Save(sfd.FileName, ImageFormat.Gif); break; }
                                case 4: { img.Save(sfd.FileName, ImageFormat.Png); break; }
                                case 5: { img.Save(sfd.FileName, ImageFormat.Tiff); break; }
                                case 6: { img.Save(sfd.FileName, ImageFormat.Icon); break; }

                            }
                        }
                    }
                    else
                    {//文件已经存在
                        img.Save(fullname);//直接保存
                    }
                }
                else if (result == DialogResult.Cancel)
                {
                    this.Close();
                }
                this.Close();
            }
            else
            {
                this.Close();
            }
        }
        /**
         * 线宽改变
         * **/
        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            lineheight = int.Parse(toolStripComboBox1.ComboBox.SelectedItem.ToString());
        }

        private void lxt_form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }

        private void lxt_form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            int i;
            for (i = 0; i < s.Length; i++)
            {
                if (s[i].Trim() != "")
                {
                    fullname = s[i];
                    Bitmap tempImage = new Bitmap(s[i]);
                    img = new Bitmap(tempImage.Width, tempImage.Height);
                    Graphics draw = Graphics.FromImage(img);
                    draw.DrawImage(tempImage, 0, 0, tempImage.Width, tempImage.Height);
                    pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
                    pictureBox1.Image = img;
                    draw.Dispose();
                    tempImage.Dispose();
                }

            }
        }
    }
}
