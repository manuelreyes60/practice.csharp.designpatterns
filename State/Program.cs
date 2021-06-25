using System;
using System.Collections.Generic;
using System.Text;

namespace State
{
    public enum State
    {
        OffHook,
        Connecting,
        Connected,
        OnHold
    }

    public enum Trigger
    {
        CallDialed,
        HungUp,
        CallConnected,
        PlacedOnHold,
        TakenOffHold,
        LeftMessage
    }

    enum LockState
    {
        Locked,
        Failed,
        Unlocked
    }

    class Program
    {
        private static Dictionary<State, List<(Trigger, State)>> rules = new Dictionary<State, List<(Trigger, State)>>
        {
            [State.OffHook] = new List<(Trigger, State)> { (Trigger.CallDialed, State.Connecting) },
            [State.Connecting] = new List<(Trigger, State)> 
            {
                (Trigger.HungUp, State.OffHook),
                (Trigger.CallConnected, State.Connected)
            },
            [State.Connected] = new List<(Trigger, State)> 
            {
                (Trigger.LeftMessage, State.OffHook),
                (Trigger.HungUp, State.OffHook),
                (Trigger.PlacedOnHold, State.OnHold)
            },
            [State.OnHold] = new List<(Trigger, State)>
            {
                (Trigger.TakenOffHold, State.Connected),
                (Trigger.HungUp, State.OffHook)
            }
        };

        static void Main(string[] args)
        {
/*            var state = State.OffHook;
            while (true)
            {
                Console.WriteLine($"The phone is currently {state}");
                Console.WriteLine("Select a trigger:");

                // foreach to for
                for (var i = 0; i < rules[state].Count; i++)
                {
                    var (t, _) = rules[state][i];
                    Console.WriteLine($"{i}. {t}");
                }

                int input = int.Parse(Console.ReadLine());

                var (_, s) = rules[state][input];
                state = s;
            }*/

            string code = "1234";
            var state = LockState.Locked;
            var entry = new StringBuilder();

            while (true)
            {
                switch (state)
                {
                    case LockState.Locked:
                        entry.Append(Console.ReadKey().KeyChar);

                        if (entry.ToString() == code)
                        {
                            state = LockState.Unlocked;
                            break;
                        }

                        if (!code.StartsWith(entry.ToString()))
                        {
                            // the code is blatantly wrong
                            state = LockState.Failed;
                        }
                        break;
                    case LockState.Failed:
                        Console.CursorLeft = 0;
                        Console.WriteLine("FAILED");
                        entry.Clear();
                        state = LockState.Locked;
                        break;
                    case LockState.Unlocked:
                        Console.CursorLeft = 0;
                        Console.WriteLine("UNLOCKED");
                        return;
                }
            }
        }
    }
}
