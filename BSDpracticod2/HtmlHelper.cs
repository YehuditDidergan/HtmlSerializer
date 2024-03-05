using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BSDpracticod2
{
    public class HtmlHelper
    {  
        private readonly static HtmlHelper _instance = new HtmlHelper();
        //החזרת המופע לשימוש בנתוני אובייקט המחלקה
        public static HtmlHelper Instance => _instance;
        public string[] HtmlTags { get; set; }
        public string[] HtmlOneTags { get; set; }

        private HtmlHelper()
        {
            string jsonHtmlTags = File.ReadAllText("jsonHelper/HtmlTags.json");
            if (jsonHtmlTags != null)
            {
                HtmlTags = JsonSerializer.Deserialize<string[]>(jsonHtmlTags);
            }

            string jsonHtmlOneTags = File.ReadAllText("jsonHelper/HtmlOneTags.json");
            if (jsonHtmlOneTags != null)
            {
                HtmlOneTags = JsonSerializer.Deserialize<string[]>(jsonHtmlOneTags);
            }
        }
        public static void PrintHtmlTree(HtmlElement element, string indentation)
        {
            Console.WriteLine($"{indentation}{element}");
            foreach (var child in element.Children)
                PrintHtmlTree(child, indentation + "  ");
        }
        public static HtmlElement Serialize(HtmlElement rootElement, List<string> htmlLines)
        {
            HtmlElement currentElement = rootElement;

            foreach (string line in htmlLines)
            {
                string tagName = line.Split(' ')[0];
                // טיפול בתגיות מיוחדות
                if (tagName == "/html")
                {
                    break;
                }
                else if (line.StartsWith("/"))
                {
                    currentElement = currentElement.Parent;
                    continue;
                }
                // טיפול בטקסט פנימי
                if (line.Length > 1 && !HtmlHelper.Instance.HtmlTags.Contains(tagName))
                {
                    currentElement.InnerHtml += line;
                    continue;
                }

                //  יצירת אובייקט חדש עבור תגית חדשה
                HtmlElement newElement = CreateChild(tagName, currentElement, line);
                currentElement.Children.Add(newElement);

                // טיפול בתגיות סוגרות
                //אם זה תגית לא סוגרת האובייקט הנוכחי הופך להיות אבא
                if (!HtmlHelper.Instance.HtmlOneTags.Contains(tagName) && !line.EndsWith("/"))
                {
                    currentElement = newElement;
                }
            }
            return rootElement;
        }
        //יצירת אובייקט חדש והוספתו כילד לאובייקט האב
        public static HtmlElement CreateChild(string tagName, HtmlElement currentElement, string line)
        {
            HtmlElement child = new HtmlElement { Name = tagName, Parent = currentElement };
            //attributesמציאת  
            var attributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(line);
            foreach (var attr in attributes)
            {
                //הפרדת attr לname ולvalue
                string attributeName = attr.ToString().Split('=')[0];
                string attributeValue = attr.ToString().Split('=')[1].Replace("\"", "");
                //השמת נתוני האובייקט
                if (attributeName.ToLower() == "class")
                    child.Classes.AddRange(attributeValue.Split(' '));
                else if (attributeName.ToLower() == "id")
                    child.Id = attributeValue;
                else child.Attributes.Add(attributeName, attributeValue);
            }
            return child;
        }
        public static bool IsValidTagName(string tagName)
        {
            if (string.IsNullOrEmpty(tagName))
            {
                return false;
            }

            if (!char.IsLetter(tagName[0]))
            {
                return false;
            }

            for (int i = 1; i < tagName.Length; i++)
            {
                if (!char.IsLetterOrDigit(tagName[i]) && tagName[i] != '_')
                {
                    return false;
                }
            }

            return true;
        }
    }
}
