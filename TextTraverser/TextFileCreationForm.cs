﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;

namespace TextTraverser
{
    public partial class TextFileCreationForm : Form
    {
        MainWindow main;

        public TextFileCreationForm(MainWindow masterScreen)
        {
            InitializeComponent();
            main = masterScreen;
        }

        private void TextFileCreationForm_Load(object sender, EventArgs e)
        {
            textBox2.Text = "C:\\";
            label1.Font = new Font("/TextTraverser;component/#Fixedsys Excelsior 3.01", 16.0f);
        }

        private void button1_Click(object sender, EventArgs e)//create new text file based on the text field inputs
        {
            GenerateTextFile();
            this.Close();

        }

        private void GenerateTextFile()
        {
            Process CMD = new Process();
            string extention = textBox1.Text;//gets text
            string path = textBox2.Text;
            string fileName = textBox3.Text;
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;//finds the current base directory of the program

            extention = Regex.Replace(extention, @"[^0-9a-zA-Z]+", "");//makes sure there is only alpha-numeric input for the extention

            string arguments;

            arguments = "/C dir " + path + "*." + extention + " /b /on /s > " + currentDirectory + fileName + ".txt";//concatenating the argument to be passed to cmd

            CMD.StartInfo.UseShellExecute = true;
            CMD.StartInfo.FileName = "cmd.exe";
            CMD.StartInfo.CreateNoWindow = true;
            CMD.StartInfo.Arguments = arguments;
            CMD.Start();
            //CMD.Start("cmd.exe", arguments);
            CMD.WaitForExit();//waits for cmd to finish
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.tadupd02);
            player.Play();
            try
            {
                main.changePath(currentDirectory + fileName + ".txt");
            }
            catch (IOException)
            {
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
