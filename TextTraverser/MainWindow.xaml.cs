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

namespace TextTraverser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TextSearch searcher;
        DateTime latestTime;
        Config settings;
        
        public MainWindow()
        {
            InitializeComponent();
            searcher = new TextSearch();
            searcher.getText("C:\\TEMP\\GRAEME.TXT");
            latestTime = DateTime.Now;
            settings = new Config();
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
            string path = this.listBox.SelectedItem.ToString();
            path = TextManipulate.CleanUpString(path);
            searcher.OpenFile(path);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string pathBarText = textBoxPath.GetLineText(0);
            searcher.getText(pathBarText);
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void File_Click(object sender, RoutedEventArgs e)
        {

        }

        private void File_Associations_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);

        }
    }
}
