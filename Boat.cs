using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;
using System.Threading;
using Core;
using MiniBus;
using MiniBus.Contracts;
using Message = Core.Message;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Boat
{
    public class Program
    {
        /// <summary>
        /// In this process, we have to create new passengers
        /// start listening every 4 seconds and then collect all messages
        /// </summary>
        static void Main(string[] args)
        {
            Constants.N = 2; // Convert.ToInt32(args[0]);
            Constants.M = 1; // Convert.ToInt32(args[1]);

            Boat m = new Boat(Constants.M, Constants.N);

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }
    }

    public class Boat
    {
        private string pre = "Passenger.";
        private List<Process> childProcesses = new List<Process>();

        private static Timer aTimer;

        //static void Main(string[] args)
        public Boat(int M, int N)
        {
            string[] ms = new string[M + N];
            var processInfo = new ProcessStartInfo
            {
                //UseShellExecute = false, // redirect all input/output streams to parent's console
                FileName = Constants.FILENAME_PASSENGER,
                Arguments = Constants.ARGUMENT_PASSENGER_M_MISIONARA
            };
            for (int i = 0; i < M; i++)
            {
                // create missionary
                var pid = i;
                //new Passenger(pid, false); // TO DO: start process
                processInfo.Arguments = pid + " " + Constants.ARGUMENT_PASSENGER_M_MISIONARA;
                childProcesses.Add(Process.Start(processInfo));

                ms.Append(pre + pid);
            }

            for (int i = 0; i < N; i++)
            {
                // create cannibal
                var pid = i + M;
                //new Passenger(pid, false); // TO DO: start process
                processInfo.Arguments = pid + " " + Constants.ARGUMENT_PASSENGER_M_MISIONARA;
                childProcesses.Add(Process.Start(processInfo));

                ms.Append(pre + pid);
            }


            // create a bus for receiving messages
            IBus bus = new BusBuilder()
                .WithLogging(new ConsoleLogger())
                .DefineErrorQueue("Boat.errors")
                .DefineReadQueue("Boat")
                //.DefineWriteQueues(ms)
                .CreateLocalQueuesAutomatically()
                .JsonSerialization()

                .CreateBus();

            // register one or more message handlers
            bus.RegisterHandler<Message>(new PassengerHandler());
            // process messages on the read queue synchronously
            bus.ReceiveAsync<Message>(); // bus.ReceiveAsync<PlaceOrder>();

            aTimer = new Timer(Constants.BOAT_CHECK_PERIOD);

            aTimer.Elapsed += (sender, args) =>
            {
                //bus.StopReceiving();
                if (!ProcessPassengers())
                {
                    //    bus.Receive<Message>();
                    //}
                    //else
                    //{
                    // die
                    aTimer.Enabled = false;
                    Console.WriteLine("Press enter to exit...");
                    Console.ReadLine();
                }
            };

            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private bool ProcessPassengers()
        {
            // after 4 seconds, check if CONDITION_CrossRiver
            if (CONDITION_CrossRiver)
            {
                List<String> temp = new List<string>();
                foreach (var p in PassengersInBoat)
                {
                    temp.Add(pre + p.PID);
                }

                var tempBus = new BusBuilder()
                    .WithLogging(new ConsoleLogger())
                    .DefineErrorQueue("Boat.errors")
                    .DefineWriteQueues(temp.ToArray())
                    .CreateLocalQueuesAutomatically()
                    .JsonSerialization()
                    .CreateBus();


                // send it to all passengers in boat that crossed the river (:
                tempBus.Send(true);

                PassengersInBoat.Clear();
                // transfer passengers from waiting list to boat
                var removeList = new List<int>();
                foreach (var pr in PassengerRequests)
                {
                    if (CONDITION_AddPassengerToBoat)
                    {
                        PassengersInBoat.Add(pr);
                        removeList.Add(pr.PID);
                    }
                }
                PassengerRequests.RemoveAll(r => removeList.Any(a => a == r.PID));

                return true; // keep going
            }

            // die
            return false;
        }

        public static List<Message> PassengersInBoat = new List<Message>(); //public IPassenger[] Passengers = new IPassenger[3];
        public static List<Message> PassengerRequests = new List<Message>();

        private static int CountKanibal => PassengersInBoat.Count(o => o.Value == (int)PassengerType.Cannibal);
        private static int CountMisionar => PassengersInBoat.Count(o => o.Value == (int)PassengerType.Missionary);
        private static bool CONDITION_CrossRiver => PassengersInBoat.Count >= Constants.MIN_NUMBER_OF_PASSENGERS_IN_BOAT &&
                                             PassengersInBoat.Count <= Constants.MAX_NUMBER_OF_PASSENGERS_IN_BOAT;
        private static bool CONDITION_AddPassengerToBoat => CountKanibal <= CountMisionar && PassengersInBoat.Count < Constants.MAX_NUMBER_OF_PASSENGERS_IN_BOAT;

        class PassengerHandler : IHandleMessage<Message>
        {

            public void Handle(Message msg)
            {
                // receive message with PID: passenger ID
                // and value if passenger is missionary
                if (CONDITION_AddPassengerToBoat)
                {
                    PassengersInBoat.Add(msg);
                }
                else
                {
                    PassengerRequests.Add(msg);
                }

                // we collect passenger requests and put them into boat if there is room
            }
        }


    }




    //public class Program
    //{
    //    private List<Process> childProcesses = new List<Process>();

    //    public void Boat()
    //    {
    //        //new Thread(InitPassengers).Start();

    //        Thread.Sleep(Constants.BOAT_CHECK_PERIOD);
    //    }

    //    /// <summary>
    //    /// In this process, we have to create new passengers
    //    /// start listening every 4 seconds and then collect all messages
    //    /// </summary>
    //    /// <param name="args"></param>
    //    static void Main(string[] args)
    //    {
    //        Constants.N = Convert.ToInt32(args[0]);
    //        Constants.M = Convert.ToInt32(args[1]);

    //        Boat m = new Boat(Constants.M, Constants.N);

    //        Console.WriteLine("Press enter to exit...");
    //        Console.ReadLine();
    //    }

    //private void SendTo(IPassenger p)
    //{
    //    Message msg = new Message();
    //    msg.Label = Constants.MESSAGE_LABEL_FROM_BOAT + Constants.MESSAGE_LABEL_UNLOAD;
    //    msg.Body = p;

    //    MessageQueue.Send(msg);
    //}

    //public void CollectPassengers()
    //{
    //    var messages = MessageQueue.GetMessageEnumerator2();

    //    while (messages.MoveNext())
    //    {
    //        var m = messages.Current;
    //        if (m?.Label == Constants.MESSAGE_LABEL_FROM_PASSENGER + Constants.MESSAGE_LABEL_LOAD)
    //        {
    //            m = messages.RemoveCurrent();
    //            //message.Formatter = new XmlMessageFormatter(new Type[] { typeof(IPassenger) });

    //            var passenger = (IPassenger)m?.Body;

    //            if (this.AddPassengerToBoat(passenger))
    //            {
    //                if (CONDITION_CrossRiver)
    //                {
    //                    UnloadPassengers();
    //                }
    //            }
    //        }
    //    }
    //}

    //private bool AddPassengerToBoat(IPassenger p)
    //{
    //    if (!CONDITION_AddPassengerToBoat)
    //    {
    //        return false;
    //    }

    //    this.PassengersInBoat.Add(p);

    //    return true;
    //}

    //public void InitPassengers()
    //{
    //    Console.WriteLine("Start child processes");

    //    var processInfo = new ProcessStartInfo
    //    {
    //        UseShellExecute = false, // redirect all input/output streams to parent's console
    //        FileName = Constants.FILENAME_PASSENGER,
    //        Arguments = Constants.ARGUMENT_PASSENGER_M_MISIONARA
    //    };

    //    for (int i = 0; i < Constants.M; i++)
    //    {
    //        childProcesses.Add(Process.Start(processInfo));
    //    }

    //    processInfo.Arguments = Constants.ARGUMENT_PASSENGER_N_KANIBALA;

    //    for (int i = 0; i < Constants.N; i++)
    //    {
    //        childProcesses.Add(Process.Start(processInfo));
    //    }

    //    //using (var process = Process.Start(processInfo))
    //    //{
    //    //    process.WaitForExit();
    //    //}
    //}

    //public void UnloadPassengers()
    //{
    //    foreach (var p in PassengersInBoat)
    //    {
    //        MessageQueue.Send(p, Constants.MESSAGE_LABEL_FROM_BOAT + Constants.MESSAGE_LABEL_UNLOAD);
    //    }
    //    PassengersInBoat.Clear();
    //}


    //public int PID => Process.GetCurrentProcess().Id;

    //public MessageQueue MessageQueue =>
    //    new MessageQueue(Constants.MESSAGE_QUEUE_PATH)
    //    {
    //        Label = Constants.MESSAGE_QUEUE_LABEL,
    //        Formatter = new XmlMessageFormatter(new Type[] { typeof(IPassenger) })
    //    };
    //}
}
