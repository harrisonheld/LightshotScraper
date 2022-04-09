using System;
using System.Net;
using System.IO;
using System.Xml;

namespace LightshotScraper
{
    static class Program
    {
        static WebClient webClient = new WebClient();
        static void Main(string[] args)
        {
            webClient.Headers.Add("User-Agent: Other");

            Console.WriteLine("Hello, World!");

            for (int i = 1; i < 1000; i++)
            {
                string id = IndexToID(i);
                string url = "https://prnt.sc/" + id;
                string html = UrlToHtml(url);

                Console.Write($"[i={i}, id={id}] ");

                // extract a tag such as the following
                // <img class="no-click screenshot-image" src="https://i.imgur.com/LbpzD7m.png" ... 
                string key = "img class=\"no-click screenshot-image\" src=\"";
                int idxStart = html.IndexOf(key) + key.Length;
                int idxEnd = html.IndexOf(".png", idxStart);
                string imageUrl = html.Substring(idxStart, idxEnd - idxStart + 4);

                // if not the length of a typical imgur link
                if (imageUrl.Length != 31)
                {
                    Console.WriteLine("Failed to retrieve image.");
                    continue;
                }

                Console.Write(imageUrl);
                webClient.DownloadFile(imageUrl, $@"C:\Users\johnd\Desktop\LightShotScraping\{id}.png");
                Console.WriteLine(", Finished downloading.");
            }

            Console.ReadKey();
        }

        // Not correct, but correct enough.
        private static string IndexToID(int idx)
        {
            // 0th ID should aaaaaa
            // 1st ID should be aaaaab
            // 25th ID should aaaaaz
            // 26th should be aaaaba
            string result = "";
            int RADIX = 26;

            while (idx > 0)
            {
                int charCode = idx % RADIX;
                idx /= RADIX;

                char c = (char)('a' + charCode);
                result += c;
            }

            result = result.PadLeft(6, 'a');

            return result;
        }
        private static string UrlToHtml(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.UserAgent = "Mozilla/5.0";
            WebResponse response = request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream(),
                System.Text.Encoding.UTF8);
            string result = sr.ReadToEnd();
            sr.Close();
            response.Close();

            return result;
        }


    }
}
