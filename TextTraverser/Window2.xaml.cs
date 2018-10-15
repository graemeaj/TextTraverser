using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TextTraverser
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        MainWindow main;
        public Window2(MainWindow masterScreen)
        {
            InitializeComponent();
            main = masterScreen;
        }

        private void GenerateTextFile()
        {
            Process CMD = new Process();
            string extention = textBox.Text;//gets text
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

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.BUTTON1);
            player.Play();
            GenerateTextFile();
            this.Close();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.BUZZ);
            player.Play();
            this.Close();
        }
    }
}
