using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Configuration;
using System.Reflection;

namespace TextTraverser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TextSearch searcher;
        DateTime latestTime;
        Configuration config;
        Config settings;

        string versionNumber;
        string buildTime;

        public MainWindow()
        {

            //meta information
            versionNumber = "0.61";
            buildTime = Assembly.GetExecutingAssembly().GetLinkerTime().ToString();


            InitializeComponent();
            searcher = new TextSearch();    
            latestTime = DateTime.Now;
            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            settings = new Config();
            searcher.getText(config.AppSettings.Settings["previousPath"].Value, config);
            System.Windows.Application.Current.MainWindow.Height = Convert.ToDouble(config.AppSettings.Settings["windowHeight"].Value);
            System.Windows.Application.Current.MainWindow.Width = Convert.ToDouble(config.AppSettings.Settings["windowWidth"].Value); 

            System.Console.Write("the height is " + Convert.ToDouble(config.AppSettings.Settings["windowHeight"].Value));
            System.Console.Write("the width is " + Convert.ToDouble(config.AppSettings.Settings["windowWidth"].Value));



            System.Windows.Application.Current.MainWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            Left = Convert.ToDouble(config.AppSettings.Settings["windowLocationX"].Value); //settings.windowLocationX;
            Top = Convert.ToDouble(config.AppSettings.Settings["windowLocationY"].Value); //settings.windowLocationY;

            
            this.Loaded += new RoutedEventHandler(windowLoaded);
        }

        public void windowLoaded(object sender, RoutedEventArgs e)
        {
            ResetPathText();
            updatePreviousPathsInMenu();

        }

        public void ResetPathText()
        {
            if (config.AppSettings.Settings["previousPath"].Value != null)
            {
                ConfigurationManager.RefreshSection("appSettings");
                textBoxPath.Text = config.AppSettings.Settings["previousPath"].Value.ToString();
                System.Console.Write("\n" + config.AppSettings.Settings["previousPath"].Value.ToString() + " is the previous path");
                System.Media.SystemSounds.Beep.Play();
            }
        }

        public void updatePreviousPathsInMenu()
        {
            mostRecent.Header = config.AppSettings.Settings["previousPath"].Value;
            second.Header = config.AppSettings.Settings["2ndPath"].Value;
            third.Header = config.AppSettings.Settings["3rdPath"].Value;
            fourth.Header = config.AppSettings.Settings["4thPath"].Value;
            fifth.Header = config.AppSettings.Settings["5thPath"].Value;
            sixth.Header = config.AppSettings.Settings["6thPath"].Value;
        }

        public void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DateTime threadStartingTime = DateTime.Now;
            latestTime = DateTime.Now;
            listBox.Items.Clear();
            listBox.Items.Add("Searching...");
            listBox.Items.Refresh();
            

            string searchBarText = textBox.GetLineText(0);

            List<String> searchResults;


            BackgroundWorker bw = new BackgroundWorker();


            // this allows our worker to report progress during work
            bw.WorkerReportsProgress = true;

            // what to do in the background thread
            bw.DoWork += new DoWorkEventHandler(
            delegate (object o, DoWorkEventArgs args)
            {
                lock (listBox)
                {
                    if (searchBarText != "")
                    {
                        searchResults = searcher.ListSearch(searchBarText, listBox, matchesLabel);//single search as backup


                        Console.WriteLine(searchResults.Count + " items in the list");
                        /*if (listBox.Items != null)
                        {
                            listBox.Items.RemoveAt(0);
                        }*/
                        Dispatcher.Invoke((Action)(() => listBox.Items.Clear()));

                        args.Result = searchResults;
                        //matchesLabel.Content = searchResults.Count;
                    }
                    else
                    {
                        Dispatcher.Invoke((Action)(() => listBox.Items.Clear()));
                        Dispatcher.Invoke((Action)(() => matchesLabel.Content = searcher.textList.Count));
                    }
                }
                
            });

            // what to do when progress changed (update the progress bar for example)
            bw.ProgressChanged += new ProgressChangedEventHandler(
            delegate (object o, ProgressChangedEventArgs args)
            {
                //listBox.Items.Add("Searching...");
            });

            // what to do when worker completes its task (notify the user)
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
            delegate (object o, RunWorkerCompletedEventArgs args)
            {
                Console.WriteLine(latestTime.Millisecond + "is the latest time \n" + threadStartingTime.Millisecond + " is the thread starting time");
                if (latestTime == threadStartingTime)
                {
                    List<String> completedSearchResults = args.Result as List<String>;
                    if (completedSearchResults != null)
                    {
                        Dispatcher.Invoke((Action)(() => matchesLabel.Content = completedSearchResults.Count()));//Console.WriteLine(line.Count());
                        foreach (string line in completedSearchResults)
                        {


                            Dispatcher.Invoke((Action)(() => listBox.Items.Add(line))); //invokes the changing of the listbox ui within the thread

                        }

                    }
                    else
                    {
                        Dispatcher.Invoke((Action)(() => matchesLabel.Content = searcher.textList.Count));
                    }
                }

            });
        
            bw.RunWorkerAsync();



        }

        public void list_item_clicked(object sender, RoutedEventArgs e)
        {
            if (this.listBox.SelectedItem != null)//checks if the listbox item selected is not null
            {
                string path = this.listBox.SelectedItem.ToString();//makes value into sting
                path = TextManipulate.CleanUpString(path);//cleans up the string with a custom function to ensure the path will read
                searcher.OpenFile(path);//opens path
            }
            else
            {
                //if the clicked listbox contains nothing
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string pathBarText = textBoxPath.GetLineText(0);
            changePath(pathBarText);
        }

        private void changePath(string s)
        {
            if (s != null && System.IO.File.Exists(s) == true)
            {
                searcher.getText(s, config);
                ResetPathText();
                notificationLabel.Content = "Success! Path \"" + s + "\" has been loaded";
                updatePreviousPathsInMenu();
            }
            else
            {
                notificationLabel.Content = "Failure. Path \"" + s + "\" could not be found";
                textBoxPath.Text = config.AppSettings.Settings["previousPath"].Value.ToString();//returns the textbox to the previous successful query
                System.Media.SystemSounds.Hand.Play();
            }
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void File_Click(object sender, RoutedEventArgs e)
        {

        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            string aboutInfo;
            aboutInfo = "V" + versionNumber + " TextTraverser\n";
            aboutInfo += "current build time: " + buildTime + "\n";
            aboutInfo += "Author: Graeme Judkins\n\ngraeme@judkins.ca\n\n©2017";
            System.Windows.Forms.MessageBox.Show(aboutInfo);
        }

        private void File_Associations_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);

        }
        
        private void recordNewSize(object sender, RoutedEventArgs e)
        {
            settings.changeSetting("windowWidth", Convert.ToString(System.Windows.Application.Current.MainWindow.Width), config);
            settings.changeSetting("windowHeight", Convert.ToString(System.Windows.Application.Current.MainWindow.Height), config);
            //System.Console.Write("\n" + "size changed  " + "\n");
        }

        public void recordNewWindowLocation(object sender, System.EventArgs e)
        {
            settings.changeSetting("windowLocationX", Convert.ToString(System.Windows.Application.Current.MainWindow.Left), config);
            settings.changeSetting("windowLocationY", Convert.ToString(System.Windows.Application.Current.MainWindow.Top), config);
        }

        private void PreviousPathsClick(object sender, RoutedEventArgs e)
        {
            //make them open the files
            System.Windows.Controls.MenuItem mI = sender as System.Windows.Controls.MenuItem;
            string path = mI.Header.ToString();
            changePath(path);
        }

        private void textBoxPath_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                string pathBarText = textBoxPath.GetLineText(0);
                changePath(pathBarText);
            }
        }
    }
}