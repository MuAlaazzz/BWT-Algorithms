using System;
using B_wheeler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace B_wheeler
{
    public partial class Form1 : Form
    {
        public string selectedFilePath;
        private Program program = new Program();
        private RichTextBox outputBox;

        public Form1()
        {
            InitializeComponent();
            outputBox = richTextBox1;



        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectedFilePath = openFileDialog.FileName;
                MessageBox.Show("Selected file: " + selectedFilePath);
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath))
            {
                MessageBox.Show("Please select a file first!");
                return;
            }

            string inputEnc;
            try
            {
                using (StreamReader reader = new StreamReader(selectedFilePath, Encoding.ASCII))
                {
                    inputEnc = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading file: " + ex.Message);
                return;
            }

            if (radioButton1.Checked)
            {
                program.Compression(inputEnc, outputBox);
                MessageBox.Show("Compression Applied");

            }
            else if (radioButton2.Checked)
            {
                program.Decompression(selectedFilePath, outputBox);
                MessageBox.Show("Decompression Applied");
            }
        }
    }
}



