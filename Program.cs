using HtmlSerializer;
using System.Text;

Console.OutputEncoding = Encoding.UTF8; // כדי שעברית תוצג נכון בקונסול

var parser = new HtmlParser();

// ---- בדיקה 1: HTML מקומי (הדוגמא מתוך ההנחיות) ----
var sampleHtml = @"
<html>
  <body>
    <div id=""main"" class=""container wide"">
      <div>
        <p class=""class-name"">שלום</p>
      </div>
      <img src=""a.png"" />
      <br>
      <span class=""class-name"">טקסט</span>
    </div>
  </body>
</html>";

var root = parser.ParseHtml(sampleHtml);

Console.WriteLine("=== div p.class-name ===");
foreach (var el in root.FindElements("div p.class-name"))
    Console.WriteLine($"{el}  InnerHtml: {el.InnerHtml}");
// p יופיע פעם אחת בלבד למרות שיש לו שני אבות מסוג div - בזכות ה-HashSet

Console.WriteLine("\n=== #main .class-name ===");
foreach (var el in root.FindElements("#main .class-name"))
    Console.WriteLine(el);

Console.WriteLine("\n=== Ancestors של p ===");
var p = root.FindElements("p").First();
foreach (var a in p.Ancestors())
    Console.WriteLine(a);

// ---- בדיקה 2: דף אינטרנט אמיתי ----
Console.Write("\nהקלד כתובת אתר (או Enter לדילוג): ");
var url = Console.ReadLine();
if (!string.IsNullOrWhiteSpace(url))
{
    Console.Write("הקלד סלקטור: ");
    var query = Console.ReadLine();

    var webRoot = await parser.Parse(url);
    var results = webRoot.FindElements(query);

    Console.WriteLine($"נמצאו {results.Count} אלמנטים:");
    foreach (var el in results)
        Console.WriteLine(el);
}
