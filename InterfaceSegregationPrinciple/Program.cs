using System;

namespace InterfaceSegregationPrinciple
{
    public class Document
    {
        public Document() { }
    }

    public class Photocopier : IPrinter, IScanner
    {
        public void Print(Document d)
        {
            Console.WriteLine("Printing...");
        }

        public void Scan(Document d)
        {
            Console.WriteLine("Scanning...");
        }
    }

    public class MultiFunctionDevice : IMultiFunctionDevice
    {
        private IPrinter printer;
        private IScanner scanner;

        public MultiFunctionDevice(IPrinter printer, IScanner scanner)
        {
            this.printer = printer;
            this.scanner = scanner;
        }

        public void Print(Document d)
        {
            printer.Print(d);
        }

        public void Scan(Document d)
        {
            scanner.Scan(d);
        }
    }

    public interface IMultiFunctionDevice: IScanner, IPrinter { }

    public interface IPrinter
    {
        void Print(Document d);
    }

    public interface IScanner
    {
        void Scan(Document d);
    }

    public interface IFax
    {
        void Fax(Document d);
    }

    class Program
    {
        static void Main(string[] args)
        {
            Photocopier p = new Photocopier();
            p.Scan(new Document());
            p.Print(new Document());

            MultiFunctionDevice device = new MultiFunctionDevice(p, p);
            device.Print(new Document());
            device.Scan(new Document());
        }
    }
}
