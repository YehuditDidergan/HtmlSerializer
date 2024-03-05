using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSDpracticod2
{
    public class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> CLasses { get; set; }=new List<string>();
        public Selector Parent { get; set; }
        public Selector Child { get; set; }
      

        public static Selector ParseSelector(string query)
        {
            // פיצול המחרוזת לפי רווחים
            string[] parts = query.Split(' ');

            // יצירת אובייקט שורש
            Selector root = new Selector();
            Selector current = root;

            // לולאה על חלקי המחרוזת
            foreach (string part in parts)
            {
                // פיצול חלק המחרוזת לפי # ו-.
                string[] subParts = part.Split('#', '.');

                // עדכון מאפייני הסלקטור הנוכחי
                if (subParts[0].StartsWith("#"))
                {
                    current.Id = subParts[0].Substring(1);
                }
                else if (subParts[0].StartsWith("."))
                {
                    current.CLasses.Add(subParts[0].Substring(1));
                }
                else if (HtmlHelper.IsValidTagName(subParts[0]))
                {
                    current.TagName = subParts[0];
                }

                // יצירת אובייקט Selector חדש והוספתו כבן
                Selector child = new Selector();
                current.Child = child;
                current = child;
            }

            return root;
        }


    }
}
