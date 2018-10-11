using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using StringExtensions;
using System.Configuration;

namespace TextTraverser
{
    class TextSearch
    {
        string text;
        public List<String> textList = new List<string>();

        public object New { get; private set; }

        public String getText(String textFileLocation, Configuration config, Label notificationLabel)//method to aquire text content and put it into a string
        {
            
            if(System.IO.File.Exists(textFileLocation) == true)//if the file exists
            {
                System.IO.StreamReader objReader;//make reader
                objReader = new System.IO.StreamReader(textFileLocation);

                text = objReader.ReadToEnd();//read the whole file and dump it into string structure
                objReader.Close();//close the reader
                textList.Clear();
                EnlistString(text);//put the string into an organised list

                Config.UpdatePreviousPaths(textFileLocation, config);
                notificationLabel.Content = "Success! Path \"" + config.AppSettings.Settings["previousPath"].Value + "\" has been loaded at " + DateTime.Now;
            }
            else
            {
                MessageBox.Show("No such file called \"" + textFileLocation + "\"", "Missing Text Search File", MessageBoxButton.OK, MessageBoxImage.Warning);//if the load fails, report it
                notificationLabel.Content = "Failure. Path \"" + config.AppSettings.Settings["previousPath"].Value + "\" has failed to load at " + DateTime.Now + " because it does not exist";
            }
            
            return text;//returns the payload
        }

        public void EnlistString(String incomingString)//generates an internal list object for search manipulation
        {
            string line = "";//storage string for line information
            int beginningNewLineOffset = 0;//records the newline offset
            int currentIndex = 0;//current working index
            bool endOfLineReached = false;
            bool endOfStringReached = false;

            while (endOfStringReached == false)
            {
                //System.Console.Write(currentIndex + " of " + text.Length + "\n");
                while (endOfLineReached == false && endOfStringReached == false)//repeat until the next newline is found
                {
                    
                    if ((currentIndex - beginningNewLineOffset >= 0) && currentIndex < text.Length)//checks to see if the index is less than 0
                    {
                        if ((text[(currentIndex - beginningNewLineOffset)].ToString() != "\n"))//if the current character is a newline
                        {
                            line += text[(currentIndex - beginningNewLineOffset)];//add the character to the line string
                            currentIndex++;
                        }
                        else
                        {
                            endOfLineReached = true;//flag that the end of the line has been found
                            currentIndex++;
                        }
                    }
                    else if(currentIndex >= text.Length)//flag condition if the end of the string is reached
                    {
                        endOfStringReached = true;
                    }
                    else
                    {
                        System.Diagnostics.Debug.Write("\nfailed because index before start of text file\n");
                    }
                }
                if (line != "" && currentIndex <= text.Length)
                {
                    textList.Add(line);//add the line to the list
                }
                line = "";//clear the line variable for reuse
                endOfLineReached = false;
            }
        }

        

        public void OpenFile(string query)
        {
            OpenFileDialog choofdlog = new OpenFileDialog();
            if (File.Exists(query))
            {
                System.Diagnostics.Debug.Write(query);
                System.Diagnostics.Debug.Write("\nsuccess");
                System.Diagnostics.Process.Start(query);
            }
            else
            {
                System.Diagnostics.Debug.Write(query);
                System.Diagnostics.Debug.Write("\nfailed");
                MessageBox.Show("no such file called \"" + query + "\"", "No Text Search File", MessageBoxButton.OK, MessageBoxImage.Warning);

            }
        }

        public List<String> SingleSearch(string query, ListBox list, Label matchesLabel)//test search to search the txt string
        {
            List<String> results = new List<string>();

            int matchesCounter = 0;
            if (text != null && text != "")//if the string has contents
            {
                int location = text.IndexOf(query);
                string line = "";
                query = Regex.Escape(query);//make the regex to find the desired string
                if (query != "")//if the query is not empty
                {
                    
                    foreach (Match match in Regex.Matches(text, query, RegexOptions.IgnoreCase))//for every match of the query in the text file string
                    {

                        //return match.Index;
                        bool beginningOfLineReached = false;//initialize the boolean for the beginning endline
                        bool failed = false;
                        int counter = 0;//counts characters back the newline is in the loop
                        int beginningNewLineOffset = 0;//records the newline offset

                        matchesCounter++;

                        if (match.Index >= 0)
                        {
                            while (beginningOfLineReached == false)//go to the beginning of the line by finding the previous newline character
                            {
                                if(match.Index - counter <= 0)
                                {
                                    beginningOfLineReached = true;
                                    failed = true;
                                }
                                else if (text[match.Index - counter].ToString() == "\n")
                                {
                                    beginningNewLineOffset = counter;
                                    beginningOfLineReached = true;
                                }
                                else
                                {
                                    counter++;
                                }
                            }
                            counter = 1;
                            bool endOfLineReached = false;
                            if (failed == false)
                            {
                                while (endOfLineReached == false)//repeat until the next newline is found
                                {
                                    if (match.Index - beginningNewLineOffset + counter >= 0)
                                    {
                                        if ((text[(match.Index - beginningNewLineOffset) + counter].ToString() != "\n") /*&& (text[(match.Index - beginningNewLineOffset) + counter] <= text.Length) && (text[(match.Index - beginningNewLineOffset) + counter] >= 0)*/)
                                        {
                                            line += text[(match.Index - beginningNewLineOffset) + counter];
                                            counter++;
                                        }
                                        else
                                        {
                                            endOfLineReached = true;
                                        }
                                    }
                                    else
                                    {
                                        System.Diagnostics.Debug.Write("\nfailed because index before start of text file\n");
                                    }
                                }
                            }

                            
                            if (line != "")
                            {
                                results.Add(line);//add the line to the list
                            }
                            else
                            {
                                matchesCounter--;
                            }
                            line = "";//clear the line variable
                        }
                    }
                }
                else//if the query is not valid put a non null value in line for the return
                {
                    results.Clear();//clears list if the query is not valid
                }

                return results;
            }
            else
            {
                results.Clear();
                return results; // "file does not exist";
            }
        }


        public List<String> ListSearch(string query, ListBox list, Label matchesLabel)//test search to search the txt string
        {
            List<String> results = new List<string>();
            StringComparison comp = StringComparison.OrdinalIgnoreCase;    

            int listIndex = 0;
            if (text != null && text != "")//if the string has contents
            {
                int location = text.IndexOf(query);

                if (query != "")//if the query is not empty
                {

                    while(listIndex < textList.Count)//finds all instances of the query in the list
                    {
                        if(textList[listIndex].Contains(query, comp))
                        {
                            results.Add(textList[listIndex]);
                        }
                        listIndex++;
                    }
                }
                else//if the query is not valid put a non null value in line for the return
                {
                    results.Clear();//clears list if the query is not valid
                }

                return results;
            }
            else
            {
                results.Clear();
                return results; // "file does not exist";
            }
        }

    }
}
