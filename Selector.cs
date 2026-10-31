using System.Text.RegularExpressions;

namespace HtmlSerializer
{
    /// <summary>
    /// מייצגת רמה אחת בשאילתת CSS. הרמות מקושרות זו לזו ב-Parent/Child
    /// (כל אבא מחזיק בן אחד בלבד - "עץ בינארי מנוון", בעצם רשימה מקושרת).
    /// </summary>
    public class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; } = new();

        public Selector Parent { get; set; }
        public Selector Child { get; set; }

        /// <summary>
        /// ממירה מחרוזת שאילתה (למשל "div #mydiv .class-name") לשרשרת אוביקטי Selector.
        /// מחזירה את הסלקטור העליון (השורש).
        /// </summary>
        public static Selector Parse(string query)
        {
            var levels = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            Selector root = null;
            Selector current = null;

            foreach (var level in levels)
            {
                var selector = new Selector();

                // פירוק לפי המפרידים # ו-. תוך שמירה על המפריד בתחילת כל חלק
                // "div#mydiv.a.b" → "div", "#mydiv", ".a", ".b"
                var parts = Regex.Split(level, @"(?=[#.])").Where(p => p.Length > 0);

                foreach (var part in parts)
                {
                    if (part.StartsWith("#"))
                        selector.Id = part.Substring(1);
                    else if (part.StartsWith("."))
                        selector.Classes.Add(part.Substring(1));
                    else if (HtmlHelper.Instance.IsHtmlTag(part))
                        selector.TagName = part.ToLower();
                }

                // חיבור לשרשרת
                if (root == null)
                    root = selector;
                else
                {
                    current.Child = selector;
                    selector.Parent = current;
                }
                current = selector;
            }

            return root;
        }

        /// <summary>
        /// בודקת אם אלמנט עונה על כל תנאי הרמה הנוכחית בסלקטור.
        /// </summary>
        public bool IsMatch(HtmlElement element)
        {
            if (TagName != null && element.Name != TagName)
                return false;
            if (Id != null && element.Id != Id)
                return false;
            if (Classes.Any(c => !element.Classes.Contains(c)))
                return false;
            return true;
        }
    }
}
