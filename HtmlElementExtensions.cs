namespace HtmlSerializer
{
    /// <summary>
    /// פונקציות הרחבה לחיפוש אלמנטים בעץ לפי Selector.
    /// </summary>
    public static class HtmlElementExtensions
    {
        public static HashSet<HtmlElement> FindElements(this HtmlElement element, Selector selector)
        {
            // HashSet מונע כפילויות - אלמנט שעונה לתנאי כמה פעמים ייכנס רק פעם אחת
            var result = new HashSet<HtmlElement>();
            FindRecursive(element, selector, result);
            return result;
        }

        public static HashSet<HtmlElement> FindElements(this HtmlElement element, string query) =>
            element.FindElements(Selector.Parse(query));

        private static void FindRecursive(HtmlElement element, Selector selector, HashSet<HtmlElement> result)
        {
            // כל הצאצאים של האלמנט הנוכחי (Skip(1) - בלי האלמנט עצמו)
            var matches = element.Descendants().Skip(1).Where(selector.IsMatch);

            // תנאי עצירה: הגענו לסלקטור האחרון
            if (selector.Child == null)
            {
                foreach (var match in matches)
                    result.Add(match);
                return;
            }

            // אחרת - ממשיכים לחפש מתוך כל אלמנט שנמצא, עם הסלקטור הבא
            foreach (var match in matches)
                FindRecursive(match, selector.Child, result);
        }
    }
}
