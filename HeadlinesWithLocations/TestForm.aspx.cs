using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.VisualBasic.FileIO;
using System.IO;
using System.Text;

namespace HeadlinesWithLocations
{
    
    public partial class TestForm : System.Web.UI.Page
    {

        string source_csv_file_path = HttpContext.Current.Server.MapPath(@"~\csvFiles\abcnews-date-text.csv"),
               target_csv_file_path = HttpContext.Current.Server.MapPath(@"~\csvFiles\result.csv"),
               name_list_csv_file_path = HttpContext.Current.Server.MapPath(@"~\csvFiles\world-cities.csv");

        private HashSet<string> nameSet;
        private Hashtable nameHead;
        // nameSet stores names of cities and countries,
        // while nameHead holds the first word of the names(which are stored in the nameSet) as key, the lengths of the names as value(numbers in a list)
        // for example, there are three names : united states, united kingdom, united some country, then the word "united" will be the key, pointing to two numbers: 2,3.


        protected void Page_Load(object sender, EventArgs e)
        {
           
        }
        protected void ButtonClicked(object sender, EventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            Initialize();
            ProcessCSVFile(source_csv_file_path, IdentifyName);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            string message = String.Format("<h4>The process is finished in {0} seconds, please check the csv file named result.csv in the csvFiles folder</h4>", elapsedMs/1000);
            window.InnerHtml = message;
        }
        private void ProcessCSVFile(string file_path, Func<string[], int> ProcessRow)
        {
            using (TextFieldParser csvParser = new TextFieldParser(file_path))
            {
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;
                csvParser.ReadLine();

                while (!csvParser.EndOfData)
                {
                    string[] fields = csvParser.ReadFields();
                    ProcessRow(fields);
                }
            }
        }
        private int PopulateSetAndTable(string[] fields)
        {
            //city name : fields[0]
            //country of the city : fields[1]
            //subcountry of the country : fields[2]
            string[] lowerFields = new string[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                lowerFields[i] = fields[i].ToLower();
            }
            List<int> nameLengths;//keep values in nameLengths unic  
            for (int i = 0; i < 3; i++)
            {
                nameSet.Add(lowerFields[i]);
                string[] words = lowerFields[i].Split(' ');                            
                if (!nameHead.ContainsKey(words[0]))
                {
                    nameLengths = new List<int>();
                    nameLengths.Add(words.Count());
                    nameHead.Add(words[0], nameLengths);
                }
                else
                {
                    nameLengths = (List<int>)nameHead[words[0]];
                    bool isRecorded = false;
                    foreach (int record in nameLengths)
                    {
                        if (record == words.Count())
                        {
                            isRecorded = true;
                            break;
                        }
                    }
                    if (!isRecorded)
                    {
                        nameLengths.Add(words.Count());
                        nameHead[words[0]] = nameLengths;
                    }                    
                }
            }
            return 0;
        }
        private int IdentifyName(string[] fields)
        {
            int step = 1;
            //the step is for the i variable in the first for loop.
            //Given a headline, look up nameHead with every word of the headline,
            //if the word is not a name head, step =1,
            //if the word is a name head, step = max(nameLengths) + 1 (assume the following word(right after the name) will never be another name head).

            bool nameIdentified = false;
            string result = "";
            //fields[0]:published date, fields[1]:headline
            string[] wordsOfHeadline = fields[1].Split(' '),wordsOfHeadLineCopy = wordsOfHeadline;
            for(int i = 0; i < wordsOfHeadline.Count();)
            {
                string word = wordsOfHeadline[i];
                if (nameHead.ContainsKey(word))
                {
                    List<int> nameLengths = (List<int>)nameHead[word];
                    int maxNameLength = 1;
                    foreach (int length in nameLengths)
                    {
                        if (length + i > wordsOfHeadline.Count()||length < maxNameLength)
                            //if there is no space for the name or the length is shorter than max name length
                            continue;
                        else
                        {                            
                            string wordsToIdentify = "";
                            for (int j = i, count = 0; count < length; j++, count++)
                            {
                                if (count < length - 1)
                                {
                                    wordsToIdentify += (wordsOfHeadline[j] + " ");
                                }
                                else
                                {
                                    wordsToIdentify += wordsOfHeadline[j];
                                }
                            }
                            if (nameSet.Contains(wordsToIdentify))
                            {
                                nameIdentified = true;
                                for (int k = i, count = 0; count < length; k++, count++)
                                {
                                    wordsOfHeadLineCopy[k] = CapitalizeName(wordsOfHeadline[k]);
                                }
                                maxNameLength = length;
                                step = length + 1;
                            }

                        }
                    }
                }
                i += step;
            }
            if (nameIdentified)
            {
                result += fields[0] + ",";
                wordsOfHeadLineCopy[0] = CapitalizeName(wordsOfHeadline[0]);
                for (int i=0;i<wordsOfHeadLineCopy.Count();i++)
                {
                    if (i < wordsOfHeadLineCopy.Count() - 1)
                    {
                        result += wordsOfHeadLineCopy[i] + " ";
                    }
                    else
                    {
                        result += wordsOfHeadLineCopy[i];
                    }
                }
                
                WriteRowsToFile(target_csv_file_path, result);
            }
            return 0;
        }
        private void Initialize()
        {
            nameSet = new HashSet<string>();
            nameHead = new Hashtable();
            File.WriteAllText(target_csv_file_path, "publish_date,headline_text" + Environment.NewLine);
            ProcessCSVFile(name_list_csv_file_path, PopulateSetAndTable);
        }
        private string CapitalizeName(string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }
        private void WriteRowsToFile(string target_csv_file_path, string content)
        {
            if (content.Length > 0)
            {
                File.AppendAllText(target_csv_file_path, content+Environment.NewLine);
            }            
        }
    }
}