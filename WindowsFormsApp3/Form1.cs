﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.Title = "请选择压缩文件";
            fileDialog.Filter = "所有文件(*.*)|*.*";
            huff.HUFFCOMPRESS test = new huff.HUFFCOMPRESS();
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string[] names = fileDialog.FileNames;
                foreach (string file in names)
                {

                    //MessageBox.Show("已选择文件:" + file, "选择文件提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    test.CreateCompressedFile(file);
                    MessageBox.Show("已完成压缩");
              
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.Title = "请选择解压文件";
            fileDialog.Filter = "所有文件(*.*)|*.*";
            dehuff.HUFFDECOMPRESS test2 = new dehuff.HUFFDECOMPRESS();
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string[] names = fileDialog.FileNames;
                foreach (string file in names)
                {
                    int index = file.LastIndexOf(".");
                    if (file.Substring(index) != ".huff")
                    {
                        MessageBox.Show("文件格式错误");
                    }
                    else
                    {
                        //MessageBox.Show("已选择文件:" + file, "选择文件提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        test2.decodeFile(file);
                        MessageBox.Show("已完成解压");
                    }

                }
            }

        
        }
    }
}
