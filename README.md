# Html Serializer

כלי ב-C# (.NET 8) שטוען דף אינטרנט, ממיר את ה-HTML שלו לעץ של אוביקטים, ומאפשר לחפש בו אלמנטים לפי CSS Selector.

## חלקי הפרויקט

| קובץ | תפקיד |
|---|---|
| `HtmlHelper.cs` | Singleton שטוען את רשימות התגיות מקבצי ה-JSON |
| `HtmlElement.cs` | צומת בעץ, כולל `Descendants()` (עם Queue) ו-`Ancestors()` |
| `HtmlParser.cs` | טעינת הדף ב-HttpClient, פירוק ב-Regex ובניית העץ בלולאה |
| `Selector.cs` | המרת מחרוזת שאילתה לשרשרת סלקטורים |
| `HtmlElementExtensions.cs` | חיפוש רקורסיבי בעץ לפי סלקטור, עם HashSet למניעת כפילויות |
| `Files/*.json` | רשימת כל תגיות ה-HTML ורשימת התגיות שלא דורשות סגירה |

## שאילתות נתמכות

- `div` – לפי שם תגית
- `#mydiv` – לפי id
- `.class-name` – לפי class
- `div#mydiv.class-name` – כל התנאים על אותו אלמנט
- `div #mydiv .class-name` – רווח מציין צאצא (לא בהכרח ישיר)

## הרצה

```bash
dotnet run
```

## דוגמת שימוש

```csharp
var parser = new HtmlParser();
var root = await parser.Parse("https://example.com");
var results = root.FindElements("div p.class-name");
```
