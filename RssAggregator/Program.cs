using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

class RssItem
{
    public string Title { get; set; } = "";
    public string Link { get; set; } = "";
    public string Description { get; set; } = "";
    public string Publisher { get; set; } = "";
    public DateTimeOffset PublishedAt { get; set; }
}

class Program
{
    static async Task Main()
    {
        string[] feedUrls =
        {
            "https://www.err.ee/rss",
            "https://www.postimees.ee/rss",
            "https://online.le.ee/feed/"
        };

        using var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(20);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("RssHomework/1.0");

        var allNews = new List<RssItem>();

        // Loeme kõik uudisvood ja kogume uudised ühte loendisse.
        foreach (string url in feedUrls)
        {
            try
            {
                string xml = await client.GetStringAsync(url);
                List<RssItem> feedNews = ReadFeed(xml, url);
                allNews.AddRange(feedNews);
                Console.WriteLine($"{url} — {feedNews.Count} uudist");
            }
            catch (Exception error) when (error is HttpRequestException
                || error is OperationCanceledException || error is XmlException)
            {
                Console.WriteLine($"Voo lugemine ebaõnnestus ({url}): {error.Message}");
            }
        }

        if (allNews.Count == 0)
        {
            Console.WriteLine("Ühtegi uudist ei õnnestunud lugeda.");
            return;
        }

        // Valime 20 kõige uuemat uudist kogu loendist.
        var latestNews = allNews.OrderByDescending(news => news.PublishedAt).Take(20);
        string outputPath = Path.GetFullPath("uudised.html");
        SaveHtml(outputPath, latestNews);
        Console.WriteLine("Ava brauseris: " + outputPath);
    }

    static List<RssItem> ReadFeed(string xml, string url)
    {
        XDocument document = XDocument.Parse(xml);
        XElement channel = document.Root?.Element("channel")
            ?? throw new XmlException("RSS-i channel-element puudub.");
        string publisher = channel.Element("title")?.Value.Trim() ?? new Uri(url).Host;

        var news = new List<RssItem>();
        foreach (XElement item in channel.Elements("item"))
        {
            string title = item.Element("title")?.Value.Trim() ?? "";
            string link = item.Element("link")?.Value.Trim() ?? "";
            string description = item.Element("description")?.Value ?? "";
            string date = item.Element("pubDate")?.Value.Trim() ?? "";

            // Ühtlustame RSS-i ajavööndi kuju, näiteks +0200 → +02:00.
            date = Regex.Replace(date, @"([+-]\d{2})(\d{2})$", "$1:$2");

            if (title.Length == 0
                || !Uri.TryCreate(new Uri(url), link, out Uri? articleUrl)
                || link.Length == 0
                || (articleUrl.Scheme != "http" && articleUrl.Scheme != "https")
                || !DateTimeOffset.TryParse(date, CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal, out DateTimeOffset publishedAt))
            {
                continue;
            }

            news.Add(new RssItem
            {
                Title = title,
                Link = articleUrl.AbsoluteUri,
                Description = WebUtility.HtmlDecode(Regex.Replace(description, "<[^>]*>", " ")).Trim(),
                Publisher = publisher,
                PublishedAt = publishedAt
            });
        }

        return news;
    }

    static void SaveHtml(string path, IEnumerable<RssItem> news)
    {
        using var file = new FileStream(path, FileMode.Create, FileAccess.Write);
        using var writer = new StreamWriter(file, Encoding.UTF8);

        writer.WriteLine("<!doctype html>");
        writer.WriteLine("<html lang=\"et\"><head><meta charset=\"utf-8\"><title>Uudised</title></head><body>");
        writer.WriteLine("<h1>Viimased uudised</h1><ul>");

        foreach (RssItem item in news)
        {
            string link = WebUtility.HtmlEncode(item.Link);
            string title = WebUtility.HtmlEncode(item.Title);
            string publisher = WebUtility.HtmlEncode(item.Publisher);
            string description = WebUtility.HtmlEncode(item.Description);

            writer.WriteLine("<li>");
            writer.WriteLine($"<a href=\"{link}\">{title}</a><br>");
            writer.WriteLine($"{item.PublishedAt:dd.MM.yyyy HH:mm zzz} | {publisher}<br>");
            writer.WriteLine(description);
            writer.WriteLine("</li>");
        }

        writer.WriteLine("</ul></body></html>");
    }
}



