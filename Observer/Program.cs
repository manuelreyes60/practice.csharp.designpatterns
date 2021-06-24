using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace Observer
{
    public class FallsIllEventArgs
    {
        public string Address;
    }
    public class Person
    {
        public void CatchCold()
        {
            FallsIll?.Invoke(this, new FallsIllEventArgs { Address = "123 London Road"});
        }
        public event EventHandler<FallsIllEventArgs> FallsIll;
    }
    
    // Weak Event Pattern
    public class Button
    {
        public event EventHandler Clicked;
        public void Fire()
        {
            Clicked?.Invoke(this, EventArgs.Empty);
        }
    }
    public class Window
    {
        public Window(Button button)
        {
            WeakEventManager<Button, EventArgs>
              .AddHandler(button, "Clicked", ButtonOnClicked);
        }
        private void ButtonOnClicked(Object sender, EventArgs eventArgs)
        {
            Console.WriteLine("Button clicked (Window handled)");
        }
        ~Window()
        {
            Console.WriteLine("Window finalized");
        }
    }

    // Obervable Special Interfaces
    public class Event
    {

    }
    public class FallsIllEvent : Event
    {
        public string Address;
    }
    public class Person2 : IObservable<Event>
    {
        private readonly HashSet<Subscription> subscriptions = new HashSet<Subscription>();
        public IDisposable Subscribe(IObserver<Event> observer)
        {
            var subscription = new Subscription(this, observer);
            subscriptions.Add(subscription);
            return subscription;
        }

        public void FallIll()
        {
            foreach(var s in subscriptions)
            {
                s.Observer.OnNext(new FallsIllEvent { Address = "456 New Road" });
            }
        }

        private class Subscription : IDisposable
        {
            private readonly Person2 person2;
            public readonly IObserver<Event> Observer;
            public Subscription(Person2 person2, IObserver<Event> observer)
            {
                this.person2 = person2;
                Observer = observer;
            }
            public void Dispose()
            {
                person2.subscriptions.Remove(this);
            }
        }
    }

    // Observable Collections
    public class Market
    {
        public BindingList<float> Prices = new BindingList<float>();

        public void AddPrice(float price)
        {
            Prices.Add(price);
        }
    }

    class Program : IObserver<Event>
    {
        static void Main(string[] args)
        {
            var person = new Person();

            person.FallsIll += CallDoctor;

            person.CatchCold();

            var btn = new Button();
            var window = new Window(btn);
            //var window = new Window(btn);
            var windowRef = new WeakReference(window);
            btn.Fire();

            Console.WriteLine("Setting window to null");
            window = null;

            FireGC();
            Console.WriteLine($"Is window alive after GC? {windowRef.IsAlive}");

            btn.Fire();

            Console.WriteLine("Setting button to null");
            btn = null;

            FireGC();

            new Program();

            var market = new Market();
            market.Prices.ListChanged += (sender, eventArgs) =>
            {
                if (eventArgs.ListChangedType == ListChangedType.ItemAdded)
                {
                    float price = ((BindingList<float>)sender)[eventArgs.NewIndex];
                    Console.WriteLine($"Binding list got a price of {price}");
                }
            };
            market.AddPrice(123);
        }

        public Program()
        {
            var person2 = new Person2();
            var sub = person2.Subscribe(this);
            person2.FallIll();
        }

        private static void FireGC()
        {
            Console.WriteLine("Starting GC");
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            Console.WriteLine("GC is done!");
        }

        private static void CallDoctor(object sender, FallsIllEventArgs eventArgs)
        {
            Console.WriteLine($"A doctor has been called to {eventArgs.Address}");
        }

        public void OnNext(Event value)
        {
            if(value is FallsIllEvent args)
            {
                Console.WriteLine($"A doctor is required at {args.Address}");
            }
        }

        public void OnError(Exception error){}

        public void OnCompleted(){ }
    }
}
