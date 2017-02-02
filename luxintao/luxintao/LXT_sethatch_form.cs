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
 * 纹理设置
 * @Author さる
 * */
namespace luxintao
{
    public partial class LXT_sethatch_form : Form
    {
        private lxt_form1 mainForm;
        private HatchStyle[] hs;
        private HatchBrush hb;
        public LXT_sethatch_form()
        {
            InitializeComponent();
            hs = new HatchStyle[] { HatchStyle.BackwardDiagonal, HatchStyle.Cross, HatchStyle.DarkDownwardDiagonal,
                HatchStyle.DarkHorizontal, HatchStyle.DarkUpwardDiagonal, HatchStyle.DarkVertical,
                HatchStyle.DashedDownwardDiagonal, HatchStyle.DashedHorizontal, HatchStyle.DashedUpwardDiagonal,
                HatchStyle.DashedVertical, HatchStyle.DiagonalBrick, HatchStyle.DiagonalCross, HatchStyle.Divot, 
                HatchStyle.DottedDiamond, HatchStyle.DottedGrid, HatchStyle.ForwardDiagonal, HatchStyle.Horizontal,
                HatchStyle.HorizontalBrick, HatchStyle.LargeCheckerBoard, HatchStyle.Vertical, HatchStyle.Wave,
                HatchStyle.ZigZag };
        }

        private void LXT_sethatch_form_Load(object sender, EventArgs e)
        {
            mainForm = (lxt_form1)this.Owner;
            frontColorBT.BackColor = mainForm.StartColor;//前景色，需要对主界面修改
            backColorBT.BackColor = mainForm.EndColor;//背景色，需要对主界面修改
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            //为纹理类型下拉列表赋值，并确定选中项
            for (int i = 0; i < hs.Length;i++)
            {
                comboBox1.Items.Add(hs[i].ToString());
                if (mainForm.HS.ToString() == hs[i].ToString()) {
                    comboBox1.SelectedIndex = i;
                }
            }
        }

        


        private void preview() {
            Graphics g = CreateGraphics();
            Rectangle rec = new Rectangle(label4.Left+50,label4.Top-20,163,51);
            hb = new HatchBrush(hs[comboBox1.SelectedIndex],frontColorBT.BackColor,backColorBT.BackColor);
            g.FillEllipse(hb,rec);
        }

        /**
         *paint じけん
         *paint事件
         **/
        private void LXT_sethatch_form_Paint(object sender, PaintEventArgs e)
        {
            preview();
        }

        /**
         * 
         * 前色をクリックする
         * 前景色点击
         * **/
        private void frontColorBT_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            dialog.AllowFullOpen = true;
            dialog.Color = frontColorBT.BackColor;
            if(dialog.ShowDialog()==DialogResult.OK){
                frontColorBT.BackColor = dialog.Color;
                preview();//アップグレード
            }
        }
        /**
         * はいけい色をクリックする
         * 背景色
         * **/
        private void backColorBT_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            dialog.AllowFullOpen = true;
            dialog.Color = backColorBT.BackColor;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                backColorBT.BackColor = dialog.Color;
                preview();//アップグレード
            }
        }


        void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            preview();
        }

        /**
         * キャンセル
         * **/
        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /**
         * 確認(かくにん)
         * **/
        private void confirmBtn_Click(object sender, EventArgs e)
        {
            mainForm.StartColor = frontColorBT.BackColor;
            mainForm.EndColor = backColorBT.BackColor;
            mainForm.HS = hs[comboBox1.SelectedIndex];
            mainForm.HB = hb;
            mainForm.ColorType = 3;
            this.Close();
        }






    }
}
