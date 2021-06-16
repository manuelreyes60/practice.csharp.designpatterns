using Autofac;
using Autofac.Features.Metadata;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Adapter
{
    public class Point
    {
        public int X, Y;
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class Line
    {
        public Point Start, End;
        public Line(Point start, Point end)
        {
            Start = start;
            End = end;
        }
    }

    public class Vector : Collection<Line>
    {

    }

    public class Rectangle : Vector
    {
        public Rectangle(int x, int y, int width, int height)
        {
            Add(new Line(new Point(x, y), new Point(x + width, y)));
            Add(new Line(new Point(x + width, y), new Point(x + width, y + height)));
            Add(new Line(new Point(x, y), new Point(x, y + height)));
            Add(new Line(new Point(x, y + height), new Point(x + width, y + height)));
        }
    }

    public class LineToPointAdapter : Collection<Point>
    {
        private static int count;

        public LineToPointAdapter(Line line)
        {
            Console.WriteLine($"{++count}: Generating points in line [{line.Start.X}, {line.Start.Y}] - [{line.End.X}, {line.End.Y}]");

            int left = Math.Min(line.Start.X, line.End.X);
            int right = Math.Max(line.Start.X, line.End.X);
            int top = Math.Min(line.Start.Y, line.End.Y);
            int bottom = Math.Max(line.Start.Y, line.End.Y);

            int dx = right - left;
            int dy = line.End.Y - line.Start.Y;

            if (dx == 0)
            {
                for(int y = top; y <= bottom; ++y)
                {
                    Add(new Point(left, y));
                }
            }
            else if(dy == 0)
            {
                for(int x = left; x <= right; ++x)
                {
                    Add(new Point(x, top));
                }
            }
        }
    }

    // With Autofac
    public interface ICommand
    {
        void Execute();
    }

    public class SaveCommand : ICommand
    {
        public void Execute() => Console.WriteLine("Saving current file");
    }

    public class OpenCommand : ICommand
    {
        public void Execute() => Console.WriteLine("Opening a file");
    }

    public class Button
    {
        private ICommand command;
        private string name;

        public Button(ICommand command, string name)
        {
            this.command = command;
            this.name = name;
        }

        public void Click() => command.Execute();

        public void PrintMe() => Console.WriteLine($"I am a button called {name}");
    }

    public class Editor
    {
        private readonly IEnumerable<Button> buttons;

        public IEnumerable<Button> Buttons => buttons;

        public Editor(IEnumerable<Button> buttons) => this.buttons = buttons;

        public void ClickAll()
        {
            foreach (var btn in buttons)
            {
                btn.Click();
            }
        }
    }

    class Program
    {
        private static readonly List<Vector> vectors = new List<Vector>
        {
            new Rectangle(1, 1, 10, 10),
            new Rectangle(3, 3, 6, 6)
        };

        public static void DrawPoint(Point p)
        {
            Console.WriteLine(".");
        }

        static void Main(string[] args)
        {
            foreach (var vector in vectors)
            {
                foreach (var line in vector)
                {
                    var adapter = new LineToPointAdapter(line);
                    adapter.ForEach(DrawPoint);
                }
            }

            // for each ICommand, a ToolbarButton is created to wrap it, and all
            // are passed to the editor
            var b = new ContainerBuilder();
            b.RegisterType<OpenCommand>().As<ICommand>().WithMetadata("Name", "Open");
            b.RegisterType<SaveCommand>().As<ICommand>().WithMetadata("Name", "Save");
            /* b.RegisterType<Button>();*/
            /*b.RegisterAdapter<ICommand, Button>(cmd => new Button(cmd, ""));*/
            b.RegisterAdapter<Meta<ICommand>, Button>(cmd => new Button(cmd.Value, (string)cmd.Metadata["Name"]));
            b.RegisterType<Editor>();
            using (var c = b.Build())
            {
                var editor = c.Resolve<Editor>();
                editor.ClickAll();

                // problem: only one button

                foreach (var btn in editor.Buttons)
                    btn.PrintMe();


            }
        }
    }
}
