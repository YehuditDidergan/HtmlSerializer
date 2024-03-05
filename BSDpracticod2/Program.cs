// See https://aka.ms/new-console-template for more information
using BSDpracticod2;
using System.Text.RegularExpressions;

Console.WriteLine("Hello, World!");
var html = await Load("https://mail.google.com/mail/u/0/#inbox");

// הסרת רווחים מיותרים
html = new Regex("[\\r\\n\\t]").Replace(new Regex("\\s{2,}").Replace(html, ""), "");

// פיצול לפי תגיות
var htmlLines = Regex.Split(html, "<(.*?)>").Where(s => !string.IsNullOrEmpty(s)).ToList();

// יצירת אובייקט שורש
HtmlElement rootElement = HtmlHelper.CreateChild(htmlLines[1].Split(' ')[0], null, htmlLines[1]);
//בניית עץ אלמנטים
HtmlHelper.Serialize(rootElement, htmlLines.Skip(2).ToList());
//הדפסת עץ האלמנטים
HtmlHelper.PrintHtmlTree(rootElement, "");

async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}
//בניית עץ אלמנטים


