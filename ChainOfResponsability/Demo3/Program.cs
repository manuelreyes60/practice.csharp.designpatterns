using System;

using static System.Console;

namespace Demo3
{
    // Method Chain
    public class Creature
    {
        public string Name;
        public int Attack, Defense;

        public Creature(string name, int attack, int defense)
        {
            Name = name;
            Attack = attack;
            Defense = defense;
        }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Attack)}: {Attack}, {nameof(Defense)}: {Defense}";
        }
    }

    public class CreatureModifier
    {
        protected Creature creature;
        protected CreatureModifier next;

        public CreatureModifier(Creature creature)
        {
            this.creature = creature;
        }

        public void Add(CreatureModifier cm)
        {
            if (next != null) next.Add(cm);
            else next = cm;
        }

        public virtual void Handle() => next?.Handle();
    }

    public class NoBonusesModifier : CreatureModifier
    {
        public NoBonusesModifier(Creature creature) : base(creature)
        {

        }

        public override void Handle()
        {
            // Do nothing
        }
    }

    public class DoubleAttackModifier : CreatureModifier
    {
        public DoubleAttackModifier(Creature creature) : base(creature)
        {

        }

        public override void Handle()
        {
            WriteLine($"Doubling {creature.Name} Attack");
            creature.Attack *= 2;
            base.Handle();
        }
    }

    public class IncreaseDefenseModifier : CreatureModifier
    {
        public IncreaseDefenseModifier(Creature creature) : base(creature)
        {

        }

        public override void Handle()
        {
            WriteLine($"Increase {creature.Name} Defense");
            creature.Defense += 3;
            base.Handle();
        }
    }

    // BrokerChain (Better Approach to this example)
    public class Game
    {
        public event EventHandler<Query> Queries;
        public void PerformQuery(object sender, Query q)
        {
            Queries?.Invoke(sender, q);
        }
    }

    public class Query
    {
        public string CreatureName;
        public enum Argument
        {
            Attack, Defense
        }
        public Argument WhatToQuery;
        public int Value;

        public Query(string creatureName, Argument whatToQuery, int value)
        {
            CreatureName = creatureName;
            WhatToQuery = whatToQuery;
            Value = value;   
        }
    }

    public class Creature2
    {
        private Game game;
        public string Name;
        private int attack, defense;

        public Creature2(Game game, string name, int attack, int defense)
        {
            this.game = game;
            Name = name;
            this.attack = attack;
            this.defense = defense;
        }

        public int Attack
        {
            get
            {
                var q = new Query(Name, Query.Argument.Attack, attack);
                game.PerformQuery(this, q);
                return q.Value;
            }
        }

        public int Defense
        {
            get
            {
                var q = new Query(Name, Query.Argument.Defense, defense);
                game.PerformQuery(this, q);
                return q.Value;
            }
        }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Attack)}: {Attack}, {nameof(Defense)}: {Defense}";
        }
    }

    public abstract class CreatureModifier2 : IDisposable
    {
        protected Game game;
        protected Creature2 creature2;

        protected CreatureModifier2(Game game, Creature2 creature2)
        {
            this.game = game;
            this.creature2 = creature2;
            game.Queries += Handle;
        }

        protected abstract void Handle(object sender, Query q);
        public void Dispose()
        {
            game.Queries -= Handle;
        }
    }

    public class DoubleAttackModifier2 : CreatureModifier2
    {
        public DoubleAttackModifier2(Game game, Creature2 creature2) : base(game, creature2)
        {

        }

        protected override void Handle(object sender, Query q)
        {
            if(q.CreatureName == creature2.Name && q.WhatToQuery == Query.Argument.Attack)
            {
                q.Value *= 2;
            }
        }
    }

    public class IncreaseDefenseModifier2 : CreatureModifier2
    {
        public IncreaseDefenseModifier2(Game game, Creature2 creature2) : base(game, creature2)
        {

        }

        protected override void Handle(object sender, Query q)
        {
            if (q.CreatureName == creature2.Name && q.WhatToQuery == Query.Argument.Defense)
            {
                q.Value += 2;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var goblin = new Creature("Goblin", 2, 2);

            var root = new CreatureModifier(goblin);

            root.Add(new NoBonusesModifier(goblin));

            WriteLine("Lets double attack");
            root.Add(new DoubleAttackModifier(goblin));
            WriteLine("Lets Increase defense");
            root.Add(new IncreaseDefenseModifier(goblin));

            root.Handle();

            WriteLine(goblin);

            var game = new Game();
            var goblin2 = new Creature2(game, "Strong Goblin", 3, 3);
            WriteLine(goblin2);

            using(new DoubleAttackModifier2(game, goblin2))
            {
                WriteLine(goblin2);
                using (new IncreaseDefenseModifier2(game, goblin2))
                {
                    WriteLine(goblin2);
                }
            }

            WriteLine(goblin2);
        }
    }
}
