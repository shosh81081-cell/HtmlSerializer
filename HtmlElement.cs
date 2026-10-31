namespace HtmlSerializer
{
    /// <summary>
    /// מייצגת תגית HTML אחת בעץ.
    /// </summary>
    public class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new();
        public List<string> Classes { get; set; } = new();
        public string InnerHtml { get; set; } = "";

        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; } = new();

        /// <summary>
        /// מחזירה את האלמנט עצמו ואת כל צאצאיו מכל הדורות.
        /// ממומשת עם Queue (סריקה לרוחב) ולא בריקורסיה, כדי לא להעמיס על ה-Stack.
        /// </summary>
        public IEnumerable<HtmlElement> Descendants()
        {
            var queue = new Queue<HtmlElement>();
            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                yield return current;

                foreach (var child in current.Children)
                    queue.Enqueue(child);
            }
        }

        /// <summary>
        /// מחזירה את כל אבות האלמנט - מהאבא הישיר ועד השורש.
        /// </summary>
        public IEnumerable<HtmlElement> Ancestors()
        {
            var current = Parent;
            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }

        public override string ToString()
        {
            var id = string.IsNullOrEmpty(Id) ? "" : $"#{Id}";
            var classes = Classes.Count == 0 ? "" : "." + string.Join(".", Classes);
            return $"<{Name}{id}{classes}>";
        }
    }
}
