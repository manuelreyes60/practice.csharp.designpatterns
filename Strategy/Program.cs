using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strategy
{
    public enum OutputFormat
    {
        Markdown,
        Html
    }

    public interface IListStrategy
    {
        void Start(StringBuilder sb);
        void End(StringBuilder sb);
        void AddListItem(StringBuilder sb, string item);
    }

    public class MarkdownListStrategy : IListStrategy
    {
        public void Start(StringBuilder sb)
        {
            // markdown doesn't require a list preamble
        }

        public void End(StringBuilder sb)
        {

        }

        public void AddListItem(StringBuilder sb, string item)
        {
            sb.AppendLine($" * {item}");
        }
    }

    public class HtmlListStrategy : IListStrategy
    {
        public void Start(StringBuilder sb)
        {
            sb.AppendLine("<ul>");
        }

        public void End(StringBuilder sb)
        {
            sb.AppendLine("</ul>");
        }

        public void AddListItem(StringBuilder sb, string item)
        {
            sb.AppendLine($"  <li>{item}</li>");
        }
    }

    public class TextProcessorDynamic
    {
        private StringBuilder sb = new StringBuilder();
        private IListStrategy listStrategy;

        public void SetOutputFormat(OutputFormat format)
        {
            switch (format)
            {
                case OutputFormat.Markdown:
                    listStrategy = new MarkdownListStrategy();
                    break;
                case OutputFormat.Html:
                    listStrategy = new HtmlListStrategy();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }

        public void AppendList(IEnumerable<string> items)
        {
            listStrategy.Start(sb);
            foreach (var item in items)
                listStrategy.AddListItem(sb, item);
            listStrategy.End(sb);
        }

        public StringBuilder Clear()
        {
            return sb.Clear();
        }

        public override string ToString()
        {
            return sb.ToString();
        }
    }

    public class TextProcessorStatic<LS> where LS : IListStrategy, new()
    {
        private StringBuilder sb = new StringBuilder();
        private IListStrategy listStrategy = new LS();

        public void AppendList(IEnumerable<string> items)
        {
            listStrategy.Start(sb);
            foreach (var item in items)
                listStrategy.AddListItem(sb, item);
            listStrategy.End(sb);
        }

        public override string ToString()
        {
            return sb.ToString();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var tpd = new TextProcessorDynamic();
            tpd.SetOutputFormat(OutputFormat.Markdown);
            tpd.AppendList(new[] { "foo", "bar", "baz" });
            Console.WriteLine(tpd);

            tpd.Clear();
            tpd.SetOutputFormat(OutputFormat.Html);
            tpd.AppendList(new[] { "foo", "bar", "baz" });
            Console.WriteLine(tpd);

            var tps = new TextProcessorStatic<MarkdownListStrategy>();
            tps.AppendList(new[] { "foo", "bar", "baz" });
            Console.WriteLine(tps);

            var tps2 = new TextProcessorStatic<HtmlListStrategy>();
            tps2.AppendList(new[] { "foo", "bar", "baz" });
            Console.WriteLine(tps2);
        }
    }
}
