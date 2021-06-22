using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using static System.Console;

namespace Mediator
{
    public class Person
    {
        public string Name;
        public ChatRoom Room;
        private List<string> chatLog = new List<string>();

        public Person(string name)
        {
            Name = name;
        }

        public void Receive(string sender, string message)
        {
            string s = $"{sender}: '{message}'";
            WriteLine($"[{Name}'s chat session] {s}");
            chatLog.Add(s);
        }

        public void Say(string message)
        {
            Room.Broadcast(Name, message);
        }

        public void PrivateMessage(string who, string message)
        {
            Room.Message(Name, who, message);
        }
    }

    public class ChatRoom
    {
        private List<Person> people = new List<Person>();

        public void Broadcast(string source, string message)
        {
            foreach (var p in people)
                if (p.Name != source)
                    p.Receive(source, message);
        }

        public void Join(Person p)
        {
            string joinMsg = $"{p.Name} joins the chat";
            Broadcast("room", joinMsg);

            p.Room = this;
            people.Add(p);
        }

        public void Message(string source, string destination, string message)
        {
            people.FirstOrDefault(p => p.Name == destination)?.Receive(source, message);
        }
    }

    // Event Broker
    public class Actor
    {
        protected EventBroker broker;

        public Actor(EventBroker broker)
        {
            this.broker = broker ?? throw new ArgumentNullException(paramName: nameof(broker));
        }
    }

    public class FootballCoach : Actor
    {
        public FootballCoach(EventBroker broker) : base(broker)
        {
            broker.OfType<PlayerScoredEvent>()
              .Subscribe(
                ps =>
                {
                    if (ps.GoalsScored < 3)
                        Console.WriteLine($"Coach: well done, {ps.Name}!");
                }
              );

            broker.OfType<PlayerSentOffEvent>()
              .Subscribe(
                ps =>
                {
                    if (ps.Reason == "violence")
                        Console.WriteLine($"Coach: How could you, {ps.Name}?");
                });
        }
    }

    public class Ref : Actor
    {
        public Ref(EventBroker broker) : base(broker)
        {
            broker.OfType<PlayerEvent>()
              .Subscribe(e =>
              {
                  if (e is PlayerScoredEvent scored)
                      Console.WriteLine($"REF: player {scored.Name} has scored his {scored.GoalsScored} goal.");
                  if (e is PlayerSentOffEvent sentOff)
                      Console.WriteLine($"REF: player {sentOff.Name} sent off due to {sentOff.Reason}.");
              });
        }
    }

    public class FootballPlayer : Actor
    {
        private IDisposable sub;
        public string Name { get; set; } = "Unknown Player";
        public int GoalsScored { get; set; } = 0;

        public void Score()
        {
            GoalsScored++;
            broker.Publish(new PlayerScoredEvent { Name = Name, GoalsScored = GoalsScored });
        }

        public void AssaultReferee()
        {
            broker.Publish(new PlayerSentOffEvent { Name = Name, Reason = "violence" });

        }

        public FootballPlayer(EventBroker broker, string name) : base(broker)
        {
            if (name == null)
            {
                throw new ArgumentNullException(paramName: nameof(name));
            }
            Name = name;

            broker.OfType<PlayerScoredEvent>()
              .Where(ps => !ps.Name.Equals(name))
              .Subscribe(ps => Console.WriteLine($"{name}: Nicely scored, {ps.Name}! It's your {ps.GoalsScored} goal!"));

            sub = broker.OfType<PlayerSentOffEvent>()
              .Where(ps => !ps.Name.Equals(name))
              .Subscribe(ps => Console.WriteLine($"{name}: See you in the lockers, {ps.Name}."));
        }
    }

    public class PlayerEvent
    {
        public string Name { get; set; }
    }

    public class PlayerScoredEvent : PlayerEvent
    {
        public int GoalsScored { get; set; }
    }

    public class PlayerSentOffEvent : PlayerEvent
    {
        public string Reason { get; set; }
    }

    public class EventBroker : IObservable<PlayerEvent>
    {
        private readonly Subject<PlayerEvent> subscriptions = new Subject<PlayerEvent>();
        public IDisposable Subscribe(IObserver<PlayerEvent> observer)
        {
            return subscriptions.Subscribe(observer);
        }

        public void Publish(PlayerEvent pe)
        {
            subscriptions.OnNext(pe);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var room = new ChatRoom();

            var john = new Person("John");
            var jane = new Person("Jane");

            room.Join(john);
            room.Join(jane);

            john.Say("hi room");
            jane.Say("oh, hey john");

            var simon = new Person("Simon");
            room.Join(simon);
            simon.Say("hi everyone!");

            jane.PrivateMessage("Simon", "glad you could join us!");


            var cb = new ContainerBuilder();
            cb.RegisterType<EventBroker>().SingleInstance();
            cb.RegisterType<FootballCoach>();
            cb.RegisterType<Ref>();
            cb.Register((c, p) => new FootballPlayer(c.Resolve<EventBroker>(), p.Named<string>("name")));

            using (var c = cb.Build())
            {
                var referee = c.Resolve<Ref>(); // order matters here!
                var coach = c.Resolve<FootballCoach>();
                var player1 = c.Resolve<FootballPlayer>(new NamedParameter("name", "John"));
                var player2 = c.Resolve<FootballPlayer>(new NamedParameter("name", "Chris"));
                player1.Score();
                player1.Score();
                player1.Score(); // only 2 notifications
                player1.AssaultReferee();
                player2.Score();
            }
        }
    }
}
