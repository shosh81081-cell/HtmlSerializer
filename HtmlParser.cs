using System.Text.RegularExpressions;

namespace HtmlSerializer
{
    /// <summary>
    /// טוענת דף אינטרנט וממירה את ה-HTML שלו לעץ של HtmlElement.
    /// </summary>
    public class HtmlParser
    {
        private static readonly HttpClient _client = new HttpClient();

        // שלב 1: קריאה לדף אינטרנט
        public async Task<string> Load(string url)
        {
            var response = await _client.GetAsync(url);
            var html = await response.Content.ReadAsStringAsync();
            return html;
        }

        // שלב 2: פירוק המחרוזת לתגיות ולטקסטים
        public List<string> SplitToParts(string html)
        {
            // מחיקת הערות HTML ותוכן של script/style (שם יכולים להופיע תווים < > שיבלבלו את הפירוק)
            html = Regex.Replace(html, @"<!--.*?-->", "", RegexOptions.Singleline);
            html = Regex.Replace(html, @"<(script|style)\b[^>]*>.*?</\1>", "<$1></$1>",
                                 RegexOptions.Singleline | RegexOptions.IgnoreCase);

            // פיצול לפי תבנית של תגית. הסוגריים בתבנית גורמים לכך שגם התגית עצמה נשמרת בתוצאה
            var parts = Regex.Split(html, @"(<[^>]+>)");

            // ניקוי ירידות שורה ורווחים מיותרים, והשמטת מחרוזות ריקות
            return parts
                .Select(p => Regex.Replace(p, @"\s+", " ").Trim())
                .Where(p => p.Length > 0)
                .ToList();
        }

        // שלב 3: בניית העץ - בלולאה, ללא ריקורסיה
        public HtmlElement BuildTree(List<string> parts)
        {
            var helper = HtmlHelper.Instance;
            var root = new HtmlElement { Name = "root" };
            var current = root;

            foreach (var part in parts)
            {
                // טקסט פנימי (לא תגית)
                if (!part.StartsWith("<"))
                {
                    current.InnerHtml += part;
                    continue;
                }

                // הסרת < ו-> מהקצוות
                var content = part.Substring(1, part.Length - 2).Trim();

                // הצהרות כמו <!DOCTYPE html> - מדלגים
                if (content.StartsWith("!") || content.StartsWith("?"))
                    continue;

                // חיתוך המילה הראשונה
                var firstWord = content.Split(' ')[0];

                if (firstWord == "/html")
                    break; // הגענו לסוף ה-HTML

                if (firstWord.StartsWith("/"))
                {
                    // תגית סוגרת - עולים רמה אחת בעץ
                    if (current.Parent != null)
                        current = current.Parent;
                    continue;
                }

                var tagName = firstWord.TrimEnd('/').ToLower();

                if (helper.IsHtmlTag(tagName))
                {
                    var element = new HtmlElement { Name = tagName, Parent = current };
                    current.Children.Add(element);

                    // פירוק ה-Attributes מהמשך המחרוזת
                    var rest = content.Substring(firstWord.Length);
                    ParseAttributes(element, rest);

                    // האם התגית סוגרת את עצמה?
                    bool selfClosing = content.EndsWith("/") || helper.IsVoidTag(tagName);

                    // אם לא - ייתכן שיש לה תוכן פנימי, לכן היא הופכת לנוכחית
                    if (!selfClosing)
                        current = element;
                }
                else
                {
                    // משהו שנראה כמו תגית אך אינו תגית HTML מוכרת - מתייחסים אליו כטקסט
                    current.InnerHtml += part;
                }
            }

            return root;
        }

        private void ParseAttributes(HtmlElement element, string attributesString)
        {
            // תבנית ל: name="value" / name='value' / name=value / name
            var regex = new Regex(@"([^\s=/]+)(?:\s*=\s*(?:""([^""]*)""|'([^']*)'|([^\s""'>]+)))?");

            foreach (Match match in regex.Matches(attributesString))
            {
                var name = match.Groups[1].Value.ToLower();
                var value = match.Groups[2].Success ? match.Groups[2].Value
                          : match.Groups[3].Success ? match.Groups[3].Value
                          : match.Groups[4].Value;

                element.Attributes[name] = value;
            }

            if (element.Attributes.TryGetValue("id", out var id))
                element.Id = id;

            if (element.Attributes.TryGetValue("class", out var classes))
                element.Classes = classes.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        // פונקציה מרכזת: URL → עץ
        public async Task<HtmlElement> Parse(string url)
        {
            var html = await Load(url);
            return ParseHtml(html);
        }

        public HtmlElement ParseHtml(string html) => BuildTree(SplitToParts(html));
    }
}
