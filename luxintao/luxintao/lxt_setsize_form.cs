using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace luxintao
{
    public partial class lxt_setsize_form : Form
    {
        private lxt_form1 sizeForm;

        public lxt_setsize_form()
        {
            InitializeComponent();

        }
        /**
         * 取消按钮点击事件
         * **/
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lxt_setsize_form_Load(object sender, EventArgs e)
        {
            sizeForm =(lxt_form1)this.Owner;
            currWidthLabel.Text = sizeForm.ImageWidth.ToString();
            currHeightLabel.Text = sizeForm.ImageHeight.ToString();
        }

        /**
         * 确认按钮点击事件
         * */
        private void button1_Click(object sender, EventArgs e)
        {
            if(myWidthTB.Text.Equals("")||myHeightTB.Text.Equals("")){
                MessageBox.Show("高度与宽度不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            sizeForm.ImageWidth = int.Parse(myWidthTB.Text.ToString());
            sizeForm.ImageHeight = int.Parse(myHeightTB.Text.ToString());
            this.Close();
        }




    }
}
