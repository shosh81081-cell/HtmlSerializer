using System.Text.Json;

namespace HtmlSerializer
{
    /// <summary>
    /// מחלקת עזר שמחזיקה את רשימות תגיות ה-HTML.
    /// ממומשת כ-Singleton כדי שקבצי ה-JSON ייטענו פעם אחת בלבד.
    /// </summary>
    public class HtmlHelper
    {
        // המופע היחיד - נוצר פעם אחת בלבד, בפעם הראשונה שניגשים אליו
        private static readonly HtmlHelper _instance = new HtmlHelper();
        public static HtmlHelper Instance => _instance;

        public string[] HtmlTags { get; private set; }
        public string[] HtmlVoidTags { get; private set; }

        // Constructor פרטי - אי אפשר ליצור מופע מבחוץ
        private HtmlHelper()
        {
            var basePath = AppContext.BaseDirectory;

            var tagsJson = File.ReadAllText(Path.Combine(basePath, "Files", "HtmlTags.json"));
            HtmlTags = JsonSerializer.Deserialize<string[]>(tagsJson);

            var voidTagsJson = File.ReadAllText(Path.Combine(basePath, "Files", "HtmlVoidTags.json"));
            HtmlVoidTags = JsonSerializer.Deserialize<string[]>(voidTagsJson);
        }

        public bool IsHtmlTag(string tagName) =>
            HtmlTags.Contains(tagName.ToLower());

        public bool IsVoidTag(string tagName) =>
            HtmlVoidTags.Contains(tagName.ToLower());
    }
}
