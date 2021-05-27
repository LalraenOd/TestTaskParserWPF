using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestTaskParserWPF.Core
{
    internal class ProxyWorker
    {
        internal static int requestsWithProxy = 0;
        internal static int selectedProxy = 0;
        internal static int totalProxies = 0;
        internal static int checkedProxies = 0;
        internal static int workingProxiesCount = 0;
        internal static List<string> workingProxies = new List<string>();
        internal static string proxyFilePath = @"proxy\proxy.txt";

        /// <summary>
        /// Updates proxy info in mainWindow each second
        /// </summary>
        internal static void ProxyInfoUpdate()
        {
            while (true)
            {
                try
                {
                    MainWindow.AppWindow.Dispatcher.Invoke(() =>
                    {
                        MainWindow.AppWindow.TextBlockTotalProxies.Text = $"Total proxies: {totalProxies}";
                        MainWindow.AppWindow.TextBlockCurrentProxy.Text = $"Current proxy: {selectedProxy}";
                        MainWindow.AppWindow.TextBlockProxiesChecked.Text = $"Checked: {checkedProxies}";
                        MainWindow.AppWindow.TextBlockWorkingProxies.Text = $"Working proxies: {workingProxiesCount}";
                        MainWindow.AppWindow.TextBlockRequestCount.Text = $"Requests with current proxy: {requestsWithProxy}";
                        if (workingProxiesCount > 0)
                        {
                            MainWindow.AppWindow.CheckBoxProxyAval.IsChecked = true;
                        }
                    });
                }
                catch (TaskCanceledException)
                {
                }
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Gets proxy file from githyb source and starts checker
        /// </summary>
        internal static void ProxyGetter()
        {
            if (!Directory.Exists("proxy"))
                Directory.CreateDirectory("proxy");
            if (File.Exists(proxyFilePath))
                File.Delete(proxyFilePath);
            //downloading proxylist from github source
            string proxyList = "https://sunny9577.github.io/proxy-scraper/proxies.txt";
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(proxyList, proxyFilePath);
            }
            string[] proxies = File.ReadAllLines(proxyFilePath);
            totalProxies = proxies.Length;
            //spliting proxy array to 5 lists
            List<string> proxies1 = new List<string>();
            List<string> proxies2 = new List<string>();
            List<string> proxies3 = new List<string>();
            List<string> proxies4 = new List<string>();
            List<string> proxies5 = new List<string>();
            int proxiesPart = proxies.Length / 5;
            for (int i = 0; i < proxiesPart; i++)
            {
                proxies1.Add(proxies[i]);
                proxies2.Add(proxies[i + proxiesPart]);
                proxies3.Add(proxies[i + proxiesPart * 2]);
                proxies4.Add(proxies[i + proxiesPart * 3]);
                proxies5.Add(proxies[i + proxiesPart * 4]);
            }
            //launching different threads for 5 proxy list parts
            Thread ThreadProxyFounder1 = new Thread(new ParameterizedThreadStart(ProxyChecker));
            ThreadProxyFounder1.IsBackground = true;
            ThreadProxyFounder1.Start(proxies1);
            Thread ThreadProxyFounder2 = new Thread(new ParameterizedThreadStart(ProxyChecker));
            ThreadProxyFounder2.IsBackground = true;
            ThreadProxyFounder2.Start(proxies2);
            Thread ThreadProxyFounder3 = new Thread(new ParameterizedThreadStart(ProxyChecker));
            ThreadProxyFounder3.IsBackground = true;
            ThreadProxyFounder3.Start(proxies3);
            Thread ThreadProxyFounder4 = new Thread(new ParameterizedThreadStart(ProxyChecker));
            ThreadProxyFounder4.IsBackground = true;
            ThreadProxyFounder4.Start(proxies4);
            Thread ThreadProxyFounder5 = new Thread(new ParameterizedThreadStart(ProxyChecker));
            ThreadProxyFounder5.IsBackground = true;
            ThreadProxyFounder5.Start(proxies5);
        }

        /// <summary>
        /// Selects working proxy and add it to a list.
        /// </summary>
        private static void ProxyChecker(object proxiesObj)
        {
            List<string> proxies = new List<string>();
            //Cast obj to a proxy list
            if (proxiesObj is IEnumerable)
            {
                var enumerator = ((IEnumerable)proxiesObj).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    proxies.Add(enumerator.Current.ToString());
                }
            }
            //Check if proxy works. Takes a lot of time for unknown reason
            foreach (string proxy in proxies)
            {
                //splitting proxy ip and port
                string proxyIP = proxy.Split(':')[0];
                int proxyPort = int.Parse(proxy.Split(':')[1]);
                using (WebClient webClient = new WebClient())
                {
                    //trying to get page using proxy
                    webClient.Proxy = new WebProxy(proxyIP, proxyPort);
                    webClient.Encoding = Encoding.UTF8;
                    string webPage = "";
                    try
                    {
                        webPage = webClient.DownloadString("https://www.ilcats.ru");
                    }
                    catch (WebException)
                    {
                        //Go to next proxy if fail
                        continue;
                    }
                    //also checking if site blocks this proxy?
                    HtmlParser parser = new HtmlParser();
                    IHtmlDocument htmlDocument = parser.ParseDocument(webPage);
                    IHtmlCollection<IElement> body = htmlDocument.QuerySelectorAll("div.ifButtonsSetBody");
                    if (body.Length > 0)
                    {
                        workingProxies.Add(proxy);
                        workingProxiesCount++;
                    }
                    else
                    {
                        continue;
                    }
                    checkedProxies++;
                }
            }
            //Closing threads after all proxy checking
            Thread.CurrentThread.Abort();
        }
    }
}