using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Net;

namespace learn_dotnet_core
{
    class Program
    {
        static void Main(string[] args)
        {
            string html = "This is Test page " +
            "<a href='test.aspx'>test page</a> " +
            "<a href='/uploaded/test2.aspx'>test page</a> " +
            "<a href='/en'>test page</a> " +
            "<a href='en'>test page</a> " +
            "<a href='/workarea/test3.aspx'>test page</a> " +
            "<a href='example.com/test.aspx'>test page</a> " +
            "<a href='lsuc.on.ca/test.aspx'>test page</a> " +
            "<a href='lsuc.on.ca'>test page</a> " +
            "<a href='http://lso.ca'>test page</a> " +
            "<a href='/test.aspx'>test page</a> ";

            List<string> hrefs = FindHrefs(html);

            List<string> absoluteHrefs = GetAbsoluteHrefs(hrefs);

            List<string> relativeHrefs = GetRelativeHrefs(hrefs);

// test abs hrefs
            foreach (var href in absoluteHrefs)
            {
                Console.WriteLine("[Absolute] " + href +
                    (DoesAbsoluteHrefWork(href) ? " [works] " : " [broken] "));
            }

// test relative hrefs
            // foreach (var href in relativeHrefs)
            // {
            //     Console.WriteLine("[Relative] " + href +
            //         (DoesRelativeHrefWork(href) ? " [works] " : " [broken] "));
            // }
        }

        private static bool DoesAbsoluteHrefWork(string href)
        {
            try
            {
                Uri urlCheck;
                if (!href.StartsWith("http"))
                {
                    Console.WriteLine("appended--" +"http://" + href);
                    Uri.TryCreate("http://" + href, UriKind.Absolute, out urlCheck);
                }
                else
                {
                    Uri.TryCreate(href, UriKind.Absolute, out urlCheck);
                }
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlCheck);
                request.Timeout = 15000;
                HttpWebResponse response;

                response = (HttpWebResponse)request.GetResponse();
                //Console.WriteLine("Status Code: " + response.StatusCode.ToString());

                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (System.Exception)
            {
                //Console.WriteLine("ERROR");
                return false;
            }
        }

        private static bool DoesRelativeHrefWork(string href)
        {
            try
            {
                Uri urlCheck;
                if (!href.StartsWith("/"))
                {
                    Console.WriteLine("appended--" + "http://www.lso.ca/" + href);
                    
                    Uri.TryCreate("http://www.lso.ca/" + href, UriKind.Absolute, out urlCheck);
                }
                else
                {
                    Console.WriteLine("appended--" + "http://www.lso.ca" + href);
                    Uri.TryCreate("http://www.lso.ca" + href, UriKind.Absolute, out urlCheck);          
                }
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlCheck);
                request.Timeout = 15000;
                HttpWebResponse response;

                response = (HttpWebResponse)request.GetResponse();
                Console.WriteLine("Status Code: " + response.StatusCode.ToString());

                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (System.Exception)
            {
                Console.WriteLine("ERROR");
                return false;
            }
        }

        private static List<string> GetRelativeHrefs(List<string> hrefs)
        {
            return hrefs.Where(
                href =>
                    !href.StartsWith("http") &&
                    !href.StartsWith("www.") &&
                    !href.Contains(".com") &&
                    !href.Contains(".on.ca")).ToList();
        }

        private static List<string> GetAbsoluteHrefs(List<string> hrefs)
        {
            return hrefs.Where(
                href =>
                    href.StartsWith("http") ||
                    href.StartsWith("www.") ||
                    href.Contains(".on.ca")).ToList();
        }

        private static List<string> FindHrefs(string html)
        {
            List<string> hrefs = new List<string>();
            Match m;
            string HRefPattern = "href\\s*=\\s*(?:[\"'](?<1>[^\"']*)[\"']|(?<1>\\S+))";

            try
            {
                m = Regex.Match(html, HRefPattern,
                                RegexOptions.IgnoreCase | RegexOptions.Compiled,
                                TimeSpan.FromSeconds(1));
                while (m.Success)
                {
                    Console.WriteLine("Found href " + m.Groups[1] + " at "
                       + m.Groups[1].Index);
                    hrefs.Add(m.Groups[1].ToString());
                    m = m.NextMatch();
                }
            }
            catch (RegexMatchTimeoutException)
            {
                Console.WriteLine("The matching operation timed out.");
            }

            return hrefs;
        }
    }
}
