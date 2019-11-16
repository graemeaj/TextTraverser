using System;
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
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace TextTraverser
{
    public partial class TextFileCreationForm : Form
    {
        MainWindow main;
        Font fixedSys;

        public TextFileCreationForm(MainWindow masterScreen)
        {
            InitializeComponent();
            main = masterScreen;
        }

        private void TextFileCreationForm_Load(object sender, EventArgs e)
        {
            textBox2.Text = "C:\\";
            fixedSys = new Font(InitializeFont().Families[0], 16.0f);

            label1.Font = fixedSys;
            label2.Font = fixedSys;
            label3.Font = fixedSys;
            label4.Font = fixedSys;
            //textBox1.Font = fixedSys;
            //textBox2.Font = fixedSys;
            //textBox3.Font = fixedSys;
            button1.Font = fixedSys;
            button2.Font = fixedSys;
        }

        PrivateFontCollection InitializeFont()
        {
            //Create your private font collection object.
            PrivateFontCollection pfc = new PrivateFontCollection();

            //Select your font from the resources.
            //My font here is "Digireu.ttf"
            int fontLength = Properties.Resources.FSEX300.Length;

            // create a buffer to read in to
            byte[] fontdata = Properties.Resources.FSEX300;

            // create an unsafe memory block for the font data
            System.IntPtr data = Marshal.AllocCoTaskMem(fontLength);

            // copy the bytes to the unsafe memory block
            Marshal.Copy(fontdata, 0, data, fontLength);

            // pass the font to the font collection
            pfc.AddMemoryFont(data, fontLength);

            Marshal.FreeCoTaskMem(data);

            return pfc;
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
            string printIntro =
                "/t:{28|4} /k echo \"*********************************\ntext\n*********************************\" &&"
                ;

            arguments = printIntro + "dir " + path + "*." + extention + " /b /on /s > " + currentDirectory + fileName + ".txt";//concatenating the argument to be passed to cmd

            Debug.Write(arguments + " big butts");
            CMD.StartInfo.FileName = "cmd.exe";
            //CMD.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            CMD.StartInfo.Arguments = arguments;
            CMD.Start();
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
