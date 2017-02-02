using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
/**
 * @Author さる
 * 渐变色
 * **/
namespace luxintao
{
    public partial class LXT_SetGradient_Form : Form
    {
        private lxt_form1 mainForm;
        private Color startColor;
        private Color endColor;
        
        private int gradientTypeIndex;

        public const int yszx = 0;
        public const int zsyx = 1;
        public const int czdy = 2;
        public const int csdx = 3;

        public LXT_SetGradient_Form()
        {
            InitializeComponent();
        }

        private void LXT_SetGradient_Form_Load(object sender, EventArgs e)
        {
            mainForm = (lxt_form1)this.Owner;
            startColor = startColorBtn.BackColor;
            endColor = endColorBtn.BackColor;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            comboBox1.SelectedIndex = 0;

        }
        /**
         * 取消按钮
         * **/
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /**
         * 确认按钮
         * **/
        private void button1_Click(object sender, EventArgs e)
        {
            mainForm.ColorType=2;
            mainForm.StartColor = startColor;
            mainForm.EndColor = endColor;
            this.Close();
        }

        /**
         * 条目选中改变事件
         * */
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            previewEffect();
        }

        private void previewEffect() {
            gradientTypeIndex = comboBox1.SelectedIndex;
            Rectangle ractangle = new Rectangle(pictureBox1.Left, pictureBox1.Top, pictureBox1.Width, pictureBox1.Height);
            LinearGradientBrush brush = null;
            switch (gradientTypeIndex)
            {
                case yszx:
                    {
                        brush = new LinearGradientBrush(ractangle, startColor, endColor, LinearGradientMode.BackwardDiagonal);
                        break;
                    }
                case zsyx:
                    {
                        brush = new LinearGradientBrush(ractangle, startColor, endColor, LinearGradientMode.ForwardDiagonal);
                        break;
                    }
                case czdy:
                    {
                        brush = new LinearGradientBrush(ractangle, startColor, endColor, LinearGradientMode.Horizontal);
                        break;
                    }
                case csdx:
                    {
                        brush = new LinearGradientBrush(ractangle, startColor, endColor, LinearGradientMode.Vertical);
                        break;
                    }
            }
            mainForm.LGB = brush;
            Bitmap tempImage = new Bitmap(ractangle.Width, ractangle.Height);
            Graphics g = CreateGraphics();
            g.FillRectangle(brush, ractangle);
            g.DrawImage(tempImage, ractangle);
            brush.Dispose();
        }

        private void LXT_SetGradient_Form_Paint(object sender, PaintEventArgs e)
        {
            previewEffect();
        }


        private void startColorBtn_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            dialog.AllowFullOpen = true;
            dialog.Color = startColorBtn.BackColor;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                startColorBtn.BackColor = dialog.Color;
                startColor = dialog.Color;
                previewEffect();//アップグレード
            }
        }

        private void endColorBtn_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            dialog.AllowFullOpen = true;
            dialog.Color = endColorBtn.BackColor;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                endColorBtn.BackColor = dialog.Color;
                endColor = dialog.Color;
                previewEffect();//アップグレード
            }
        }


    }
}
