using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Forms;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Windows.Media;

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

        private SolidColorBrush buttonBrush;

        public MainWindow()
        {

            //meta information
            versionNumber = "0.955";
            buildTime = Assembly.GetExecutingAssembly().GetLinkerTime().ToString();


            InitializeComponent();
            searcher = new TextSearch();    
            latestTime = DateTime.Now;
            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            settings = new Config();
            searcher.getText(config.AppSettings.Settings["previousPath"].Value, config, notificationLabel);
            System.Windows.Application.Current.MainWindow.Height = Convert.ToDouble(config.AppSettings.Settings["windowHeight"].Value);
            System.Windows.Application.Current.MainWindow.Width = Convert.ToDouble(config.AppSettings.Settings["windowWidth"].Value); 

            System.Console.Write("the height is " + Convert.ToDouble(config.AppSettings.Settings["windowHeight"].Value));
            System.Console.Write("the width is " + Convert.ToDouble(config.AppSettings.Settings["windowWidth"].Value));

            System.Windows.Application.Current.MainWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            Left = Convert.ToDouble(config.AppSettings.Settings["windowLocationX"].Value); //settings.windowLocationX;
            Top = Convert.ToDouble(config.AppSettings.Settings["windowLocationY"].Value); //settings.windowLocationY;

            //listBox.KeyDown += HandleKeyUpSelected;

            textBox.Focus();

            this.Loaded += new RoutedEventHandler(windowLoaded);
        }



        public void windowLoaded(object sender, RoutedEventArgs e)
        {
            ResetPathText();
            updatePreviousPathsInMenu();

            SetValuesOnSubItems(this.File.Items.OfType<ToolStripMenuItem>().ToList());

            buttonBrush = (SolidColorBrush)button.Background;

            //notificationLabel.Content = "Success! Path \"" + config.AppSettings.Settings["previousPath"].Value + "\" has been loaded at " + DateTime.Now;

            textBox.Focus();

        }

        private void SetValuesOnSubItems(List<ToolStripMenuItem> items)
        {
            items.ForEach(item =>
            {
                var dropdown = (ToolStripDropDownMenu)item.DropDown;
                if (dropdown != null)
                {
                    dropdown.ShowImageMargin = false;
                    SetValuesOnSubItems(item.DropDownItems.OfType<ToolStripMenuItem>().ToList());
                }
            });
        }

        public void ResetPathText()//i think this just gets the last saved path
        {
            if (config.AppSettings.Settings["previousPath"].Value != null)
            {
                ConfigurationManager.RefreshSection("appSettings");
                textBoxPath.Text = config.AppSettings.Settings["previousPath"].Value.ToString();
                System.Console.Write("\n" + config.AppSettings.Settings["previousPath"].Value.ToString() + " is the previous path");
                System.Media.SystemSounds.Beep.Play();
                Search();
            }
        }

        public void updatePreviousPathsInMenu()
        {
            int truncateFactor = 13;
            mostRecent.Header = Truncate.TruncateString(config.AppSettings.Settings["previousPath"].Value, truncateFactor);
            second.Header = Truncate.TruncateString(config.AppSettings.Settings["2ndPath"].Value, truncateFactor);
            third.Header = Truncate.TruncateString(config.AppSettings.Settings["3rdPath"].Value, truncateFactor);
            fourth.Header = Truncate.TruncateString(config.AppSettings.Settings["4thPath"].Value, truncateFactor);
            fifth.Header = Truncate.TruncateString(config.AppSettings.Settings["5thPath"].Value, truncateFactor);
            sixth.Header = Truncate.TruncateString(config.AppSettings.Settings["6thPath"].Value, truncateFactor);
        }

        public void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search();
        }

        public void Search()
        {
            if (listBox != null)
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
        }

        public void list_item_clicked(object sender, RoutedEventArgs e)
        {
            if (this.listBox.SelectedItem != null)//checks if the listbox item selected is not null
            {
                string path = this.listBox.SelectedItem.ToString();//makes value into sting
                path = TextManipulate.CleanUpString(path);//cleans up the string with a custom function to ensure the path will read
                searcher.OpenFile(path);//opens path
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.BUTTON1);
                player.Play();
            }
            else
            {
                //if the clicked listbox contains nothing
            }
        }


        private void button_Click(object sender, RoutedEventArgs e)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.BUTTON1);
            player.Play();
            Refresh();
        }

        private void Refresh()
        {
            string pathBarText = textBoxPath.GetLineText(0);
            changePath(pathBarText);
        }

        public void changePath(string s)
        {
            if (s != null && System.IO.File.Exists(s) == true)
            {
                searcher.getText(s, config, notificationLabel);
                ResetPathText();
                //textBox.Text = "";
                notificationLabel.Content = "Success! Path \"" + s + "\" has been loaded at " + DateTime.Now;
                updatePreviousPathsInMenu();
                Search();
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
            /*
            this.listBox.SelectedItem = "";
            System.Windows.Controls.TextBox textboxInstance = new System.Windows.Controls.TextBox();
            textboxInstance.Text = this.listBox.SelectedItem.ToString();
            textboxInstance.IsReadOnly = true;
            this.listBox.Items.Add(textboxInstance);
            System.Console.Write("\nthis has occurred\n");
            */
        }

        private void File_Click(object sender, RoutedEventArgs e)
        {

        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.BUTTON1);
            player.Play();
            string aboutInfo;
            aboutInfo = "V" + versionNumber + " TextTraverser\n";
            aboutInfo += "Built on: " + buildTime + "\n";
            aboutInfo += "Author: Graeme Judkins\n\ngraeme@judkins.ca\n\n©2018";
            //System.Windows.Forms.MessageBox.Show(aboutInfo);
            Window1 about = new Window1(aboutInfo);
            about.Show();

        }

        private void File_Associations_Click(object sender, RoutedEventArgs e)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.BUTTON1);
            player.Play();
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
            //string path = mI.Header.ToString();
            string path = "";
            switch (mI.Name)
            {
                case "mostRecent":
                    path = config.AppSettings.Settings["previousPath"].Value.ToString();
                    break;
                case "second":
                    path = config.AppSettings.Settings["2ndPath"].Value.ToString();
                    break;
                case "third":
                    path = config.AppSettings.Settings["3rdPath"].Value.ToString();
                    break;
                case "fourth":
                    path = config.AppSettings.Settings["4thPath"].Value.ToString();
                    break;
                case "fifth":
                    path = config.AppSettings.Settings["5thPath"].Value.ToString();
                    break;
                case "sixth":
                    path = config.AppSettings.Settings["6thPath"].Value.ToString();
                    break;
                default:

                    break;
            }
            if (path != null)
            {
                changePath(path);
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.BUTTON1);
                player.Play();
            }
        }

        private void textBoxPath_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                string pathBarText = textBoxPath.GetLineText(0);
                changePath(pathBarText);
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.BUTTON1);
                player.Play();
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.BUTTON1);
            player.Play();
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == System.Windows.Forms.DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                try
                {
                    changePath(file);
                }
                catch (IOException)
                {
                }
            }
            else//if user closes the prompt
            {
                //player = new System.Media.SoundPlayer(Properties.Resources.BUZZ);
                //player.Play();
            }
        }

        private void listBox_Selected(object sender, RoutedEventArgs e)
        {
            /*
            this.listBox.SelectedItem = "";
            System.Windows.Controls.TextBox textboxInstance = new System.Windows.Controls.TextBox();
            if (this.listBox.SelectedItem.ToString() != null)
            {
                textboxInstance.Text = this.listBox.SelectedItem.ToString();
            }
            textboxInstance.IsReadOnly = true;
            this.listBox.Items.Add(textboxInstance);
            System.Console.Write("\nthis has occurred\n");
            */
        }

        private void CreateTextFile_Click(object sender, RoutedEventArgs e)
        {
            //TextFileCreationForm TFCF = new TextFileCreationForm(this);
            //TFCF.Show();
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.BUTTON1);
            player.Play();
            Window2 TFCF = new Window2(this);
            TFCF.Show();

        }


        private void listBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (this.listBox.SelectedItem != null)//checks if the listbox item selected is not null
                {
                    System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.BUTTON1);
                    player.Play();
                    string path = this.listBox.SelectedItem.ToString();//makes value into sting
                    path = TextManipulate.CleanUpString(path);//cleans up the string with a custom function to ensure the path will read
                    searcher.OpenFile(path);//opens path
                }
                else
                {
                    //if the clicked listbox contains nothing
                }
            }
        }

        private void textBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                listBox.Focus();
                //you might need to select one value to allow arrow keys
                listBox.SelectedIndex = 0;
            }
        }

        private void button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            SolidColorBrush redBrush = (SolidColorBrush)new BrushConverter().ConvertFromString("Red");

            this.button.Background = redBrush;
            this.button.InvalidateVisual();
        }

        private void button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.button.Background = buttonBrush;
        }

        private void textBoxPath_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            /*if (e.Key == Key.Enter)
            {
                Refresh();
            }*/
        }

        private void listBox_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = VisualTreeHelper.HitTest(listBox, Mouse.GetPosition(listBox)).VisualHit;
            int maxLength = 70;
            // find ListViewItem (or null)
            while (item != null && !(item is ListBoxItem))
                item = VisualTreeHelper.GetParent(item);

            if (item != null)
            {
                int i = listBox.Items.IndexOf(((ListBoxItem)item).DataContext);
                System.Diagnostics.Debug.Write("Right clicked item " + i.ToString());
                var hitListItem = listBox.Items.GetItemAt(i);

                if (hitListItem != null)
                {
                    if (hitListItem.ToString().Length > maxLength)
                    {
                        CopyPath.Header = Truncate.TruncateString(CopyFilePath(hitListItem.ToString()), 34);
                        if(CopyFileNameOnly(hitListItem.ToString()).Length < maxLength)
                        {
                            CopyNameOnly.Header = CopyFileNameOnly(hitListItem.ToString());
                        }
                        else
                        {
                            CopyNameOnly.Header = Truncate.TruncateString(CopyFileNameOnly(hitListItem.ToString()), 34);
                        }
                        
                        if(CopyFileNameAndExtension(hitListItem.ToString()).Length < maxLength)
                        {
                            CopyNameExtension.Header = CopyFileNameAndExtension(hitListItem.ToString());
                        }
                        else
                        {
                            CopyNameExtension.Header = Truncate.TruncateString(CopyFileNameAndExtension(hitListItem.ToString()), 34);
                        }
                        
                    }
                    else
                    {
                        CopyPath.Header = CopyFilePath(hitListItem.ToString());
                        CopyNameOnly.Header = CopyFileNameOnly(hitListItem.ToString());
                        CopyNameExtension.Header = CopyFileNameAndExtension(hitListItem.ToString());
                    }
                }
                else
                {
                    System.Diagnostics.Debug.Write("Item is null! ");
                }
            }

            

        }

        public void CopyStringToClipBoard(String path)
        {
            System.Windows.Clipboard.SetData(System.Windows.DataFormats.Text, path);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)//Copy File Path
        {
            try
            {
                if (listBox.SelectedItem.ToString() != "")
                {
                    String path = listBox.SelectedItem.ToString();
                    path = CopyFilePath(path);
                    CopyStringToClipBoard(path);
                    notificationLabel.Content = "\"" + path + "\"" + " Copied to Clipboard";
                    System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.BUTTON1);
                    player.Play();
                }
            }
            catch
            {

            }

        }

        private string CopyFilePath(string s)
        {
            string path = s.Replace("\r", "");
            return path;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)//Copy File Name Only //minus drive
        {
            try
            {
                if (listBox.SelectedItem.ToString() != "")
                {
                    string path = @listBox.SelectedItem.ToString();
                    path = CopyFileNameOnly(path);
                    CopyStringToClipBoard(path);
                    notificationLabel.Content = "\"" + path + "\"" + " Copied to Clipboard";
                    System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.BUTTON1);
                    player.Play();
                }
            }
            catch
            {

            }

        }

        private string CopyFileNameOnly(string s)
        {
            string path = s.Replace("\r", "");
            path = Path.GetFullPath(path); // GetFileNameWithoutExtension(path);
            path = path.Remove(0, 3);
            path = path.Substring(0, path.Length - 4);
            return path;
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)//Copy File Name And Extension
        {
            try
            {
                if (listBox.SelectedItem.ToString() != "")
                {
                    string path = listBox.SelectedItem.ToString();
                    path = CopyFileNameAndExtension(path);
                    CopyStringToClipBoard(path);
                    notificationLabel.Content = "\"" + path + "\"" + " Copied to Clipboard";
                    System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.BUTTON1);
                    player.Play();
                }
            }
            catch
            {

            }
        }

        private string CopyFileNameAndExtension(string s)
        {
            string path = s.Replace("\r", "");
            path = Path.GetFileName(path);
            return path;
        }

        //stolen from the stud, Jimmy T https://stackoverflow.com/questions/5014825/triple-mouse-click-in-c
        private int _clicks = 0;
        private Timer _timer = new Timer();
        private System.Windows.Controls.TextBox TB = new System.Windows.Controls.TextBox();
        private void TextBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Debug.Write("Click up");
            try
            {
                TB = (System.Windows.Controls.TextBox)sender;
            }
            catch
            {
                System.Diagnostics.Debug.Write("Error: NOT A VALID LISTBOX");
                return;
            }
            _timer.Stop();
            _clicks++;
            if (_clicks == 3)
            {
                // this means the trip click happened - do something
                TB.SelectAll();
                _clicks = 0;
                System.Diagnostics.Debug.Write("THE TRIPLE CLICK");
            }
            if (_clicks < 3)
            {
                _timer.Interval = SystemInformation.DoubleClickTime;
                _timer.Start();
                _timer.Tick += (s, t) =>
                {
                    _timer.Stop();
                    _clicks = 0;
                };
            }
        }
    }
}