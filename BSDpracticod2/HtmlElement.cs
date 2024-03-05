using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BSDpracticod2
{
    public class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public List<string> Classes { get; set; } = new List<string>();
        public string InnerHtml { get; set; } = "";
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; } = new List<HtmlElement>();
        public override string ToString()
        {
            string s = "";
            if (Name != null) s += "Name: " + Name;
            if (Id != null) s += " Id: " + Id;
            if (Classes.Count > 0)
            {
                s += " Classes: ";
                foreach (var c in Classes)
                    s += c + " ";
            }
            return s;
        }
        public IEnumerable<HtmlElement> Descendants()
        {
            // יצירת תור
            Queue<HtmlElement> queue = new Queue<HtmlElement>();

            // דחיפת האלמנט הנוכחי לתור
            queue.Enqueue(this);

            // לולאה כל עוד התור לא ריק
            while (queue.Count > 0)
            {
                // שליפת האלמנט הראשון מהתור
                HtmlElement element = queue.Dequeue();

                // הוספת האלמנט לרשימה
                yield return element;

                // הוספת כל הבנים של האלמנט לתור
                foreach (HtmlElement child in element.Children)
                {
                    queue.Enqueue(child);
                }
            }
        }
        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement parent = this.Parent;

            // כל עוד קיים הורה
            while (parent != null)
            {
                // הוספת ההורה לרשימה
                yield return parent;

                // מעבר להורה הבא
                parent = parent.Parent;
            }
        }


        public IEnumerable<HtmlElement> FindElementsBySelector(Selector selector)
        {
            return FindElementsBySelector(selector, new HashSet<HtmlElement>());
        }

        private IEnumerable<HtmlElement> FindElementsBySelector(Selector selector, HashSet<HtmlElement> results)
        {
            // תנאי עצירה: הגענו לסוף הסלקטור
            if (selector == null)
            {
                return results;
            }

            // מציאת כל הצאצאים שעונים לקריטריונים של הסלקטור הנוכחי
            IEnumerable<HtmlElement> descendants = this.Descendants().Where(MatchesSelector(selector)).ToHashSet();


            // ריקורסיה על הרשימה המסוננת עם הסלקטור הבא
            foreach (HtmlElement element in descendants)
            {
                results.Add(element);

                results = element.FindElementsBySelector(selector.Child, results).ToList();
            }

            // אם הגענו לסוף הסלקטור, החזרת התוצאות
            if (selector.Child == null)
            {
                return results;
            }

            return results;
        }

        private bool MatchesSelector(Selector selector)
        {
            // בדיקת שם התגית
            if (selector.TagName != null && selector.TagName != this.Name)
            {
                return false;
            }

            // בדיקת Id
            if (selector.Id != null && selector.Id != this.Id)
            {
                return false;
            }

            // בדיקת Classes
            if (selector.CLasses.Count > 0 && !selector.CLasses.All(this.Classes.Contains))
            {
                return false;
            }

            return true;
        }


        public override int GetHashCode()
        {
            int hash = 17;

            if (Name != null)
            {
                hash = hash * 23 + Name.GetHashCode();
            }

            if (Id != null)
            {
                hash = hash * 23 + Id.GetHashCode();
            }

            foreach (string className in Classes)
            {
                hash = hash * 23 + className.GetHashCode();
            }

            return hash;
        }

    }

}
