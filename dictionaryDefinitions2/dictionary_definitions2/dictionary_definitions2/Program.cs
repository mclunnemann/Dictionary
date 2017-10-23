using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace dictionaryDefinitions
{
    class Program
    {
        static void Main(string[] args)
        {
            string word = findWord();
            string htmlSTR = getHTML(word);

            Console.Read();
        }
        public static string findWord()
        {
            string URL1 = "http://www-01.sil.org/linguistics/wordlists/english/wordlist/wordsEn.txt";

            ArrayList input1 = new ArrayList();

            WebResponse response1;
            WebRequest request1 = HttpWebRequest.Create(URL1);
            response1 = request1.GetResponse();

            StreamReader sr1 = new StreamReader(response1.GetResponseStream());

            while (!sr1.EndOfStream)
            {
                input1.Add(sr1.ReadLine());
            }

            sr1.Close();
            response1.Close();
            Random rnd = new Random();

            int rand = rnd.Next(0, input1.Count);

            string word = (string)input1[rand];

            //Console.WriteLine(word);
            return word;
        }

        public static string getHTML(string word)
        {
            //word = "caps";
            Console.WriteLine(word);            
            //string URL2 = "http://www.dictionary.com/browse/" + "escars" + "?s=t";
            string URL2 = "http://www.dictionary.com/browse/" + word + "?s=t";
            try
            {
                HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
                HtmlAgilityPack.HtmlDocument doc = web.Load(URL2);
                string extractedHTML_DEF = extractDef(doc);
            }
            catch (WebException e)
            {
                word = findWord();
                getHTML(word);
            }
            return "";
        }

        public static string extractDef(HtmlAgilityPack.HtmlDocument doc)
        {
            string resultText = "";
            HtmlAgilityPack.HtmlNode node = doc.DocumentNode.SelectSingleNode("//div[@class='def-list']");

            foreach (HtmlAgilityPack.HtmlNode node2 in node.SelectNodes(".//section[@class='def-pbk ce-spot']"))
            {
                foreach (HtmlAgilityPack.HtmlNode node3 in node2.SelectNodes(".//header[@class='luna-data-header']"))
                {
                    foreach (HtmlAgilityPack.HtmlNode node4 in node3.SelectNodes(".//span[@class='dbox-pg']"))
                    {
                        Regex regex1 = new Regex("(<.*?>\\s*)+", RegexOptions.Singleline);
                        string tempText_node4 = node4.OuterHtml;
                        tempText_node4 = regex1.Replace(tempText_node4, " ").Trim();
                        resultText += "\n" + tempText_node4.Trim() + "\n";
                    }                   
                }
                foreach (HtmlAgilityPack.HtmlNode node5 in node2.SelectNodes(".//div[@class='def-set']"))
                {
                    Regex regex1 = new Regex("(<.*?>\\s*)+", RegexOptions.Singleline);
                    Regex regex2 = new Regex("(.?<div class=\"def-block def-inline-example\">?.*?</div>)+", RegexOptions.Singleline);
                    Regex regex3 = new Regex(":?", RegexOptions.Singleline);
                    string tempText_node5 = node5.OuterHtml;
                    tempText_node5 = regex2.Replace(tempText_node5, "").Trim();
                    tempText_node5 = regex1.Replace(tempText_node5, " ").Trim();
                    tempText_node5 = regex3.Replace(tempText_node5, "").Trim();
                    resultText += tempText_node5.Trim() + "\n ";

                }
            }

            Console.WriteLine(resultText + "\n");
            return resultText;
        }
    }
}
