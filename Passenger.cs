using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;
using Core;
using MiniBus;
using MiniBus.Contracts;
using Message = Core.Message;

namespace Passenger
{
    class Program
    {
        static void Main(string[] args)
        {
            var pid = Convert.ToInt32(args[0]);
            var type = Convert.ToBoolean(args[1]);

            Passenger p = new Passenger(pid, type);
            //Passenger p = new Passenger(type == Constants.ARGUMENT_PASSENGER_M_MISIONARA ? PassengerType.Misionar : PassengerType.Kanibal);

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }
    }

    public class Passenger
    {
        public PassengerType _TYPE_ { get; set; }
        public int PID { get; set; }


        public Passenger(int _pid, bool _isMissionary) // TODO: call this from main with data received from args[]
        {
            this.PID = _pid;
            this._TYPE_ = _isMissionary ? PassengerType.Missionary : PassengerType.Cannibal;

            // create a bus for sending messages
            IBus bus = new BusBuilder()
                .WithLogging(new ConsoleLogger())
                .DefineErrorQueue("Passenger.errors")
                .DefineWriteQueue("Boat")
                .CreateLocalQueuesAutomatically()
                .JsonSerialization()

                .CreateBus();
            
            // create your message type
            var msg = new Message { PID = this.PID, Value = (int)_TYPE_ };
            // send it
            bus.Send(msg);

            // create a bus for receiving messages
            bus = new BusBuilder()
                .WithLogging(new ConsoleLogger())
                .DefineErrorQueue("Passenger.errors")
                .DefineReadQueue("Passenger." + PID)
                //.DefineWriteQueue("Boat")
                .CreateLocalQueuesAutomatically()
                .JsonSerialization()
                .CreateBus();

            // register one or more message handlers
            bus.RegisterHandler<bool>(new ResponseHandler());
            // process messages on the read queue synchronously
            bus.Receive<bool>();

            // process messages on the read queue asynchronously
            //bus.ReceiveAsync<PlaceOrder>();

            // DIE in handler
        }

        public class ResponseHandler : IHandleMessage<bool>
        {
            public void Handle(bool msg)
            {
                // process the message
                if (true)
                    Process.GetCurrentProcess().Kill();
            }
        }
    }

    //public class Passenger : IPassenger
    //{
    //    public PassengerType Type { get; set; }

    //    public Passenger(PassengerType t)
    //    {
    //        Type = t;

    //        Load(this);
    //    }

    //    public bool Load(IPassenger passenger)
    //    {
    //        MessageQueue.Send(passenger, Constants.MESSAGE_LABEL_FROM_PASSENGER + Constants.MESSAGE_LABEL_LOAD);

    //        while (Wait()) ;



    //        return true;
    //    }


    //    private bool Wait()
    //    {
    //        // creates an enumerator for all messages in queue
    //        var messages = MessageQueue.GetMessageEnumerator2();

    //        while (messages.MoveNext()) // move next IF one available
    //        {
    //            var m = messages.Current;
    //            if (m?.Label == Constants.MESSAGE_LABEL_FROM_BOAT + Constants.MESSAGE_LABEL_UNLOAD)
    //            {
    //                var passenger = (IPassenger)m?.Body;
    //                if (passenger.PID == this.PID)
    //                {
    //                    m = messages.RemoveCurrent();

    //                    return false;
    //                }
    //            }
    //        }

    //        // no message is available
    //        return true;
    //    }


    //    public int PID => Process.GetCurrentProcess().Id;
    //    public MessageQueue MessageQueue =>
    //        new MessageQueue(Constants.MESSAGE_QUEUE_PATH)
    //        {
    //            Label = Constants.MESSAGE_QUEUE_LABEL,
    //            Formatter = new XmlMessageFormatter(new Type[] { typeof(IPassenger) })
    //        };
    //}
}
