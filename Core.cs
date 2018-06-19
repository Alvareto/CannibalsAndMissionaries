using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using MiniBus.Contracts;

namespace Core
{
    public class Message
    {
        /// <summary>
        /// In passenger request, represents passenger ID.
        /// </summary>
        public int PID { get; set; }
        /// <summary>
        /// In passenger request, represents whether passenger is missionary.
        /// </summary>
        public int Value { get; set; }
        //public string OrderNumber { get; set; }
        //public string Description { get; set; }
    }

    public class ConsoleLogger : ILogMessages
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }

    public enum PassengerType
    {
        Cannibal = 0,
        Missionary = 1
    }

    public static class Constants
    {
        public const int BOAT_CHECK_PERIOD = 4000;
        public const int MAX_NUMBER_OF_PASSENGERS_IN_BOAT = 3;
        public const int MIN_NUMBER_OF_PASSENGERS_IN_BOAT = 1;

        public static int N { get; set; }
        public static int M { get; set; }

        public static string MESSAGE_QUEUE_PATH = @".\Private$\NOS";
        public static string MESSAGE_QUEUE_LABEL = "NOS";

        public static string FILENAME_PASSENGER = "Passenger.exe";
        public static string FILENAME_BOAT = "Boat.exe";
        public static string ARGUMENT_PASSENGER_M_MISIONARA = Boolean.TrueString;
        public static string ARGUMENT_PASSENGER_N_KANIBALA = Boolean.FalseString;

        //public static string MESSAGE_LABEL_FROM_PASSENGER = "P";
        //public static string MESSAGE_LABEL_FROM_BOAT = "B";
        //public static string MESSAGE_LABEL_UNLOAD = "_UNLOAD";
        //public static string MESSAGE_LABEL_LOAD = "_LOAD";
    }

    //public interface IMessenger
    //{
    //    /// <summary>
    //    /// Identifier of process.
    //    /// </summary>
    //    int PID { get; }

    //    MessageQueue MessageQueue { get; }
    //}

    //public interface IBoat : IMessenger
    //{
    //    // -- void Accept();

    //    void InitPassengers();

    //    void CollectPassengers();

    //    /// <summary>
    //    /// 1. Unloads all passengers (sends messages to all passengers in boat)
    //    /// </summary>
    //    /// <returns></returns>
    //    void UnloadPassengers();

    //    List<IPassenger> PassengersInBoat { get; }
    //}

    //public interface IPassenger : IMessenger
    //{
    //    PassengerType Type { get; }

    //    /// <summary>
    //    /// 1. Asks to get passenger on the boat (sends message to boat)
    //    /// 2. Waits for the message answer if accepted
    //    /// </summary>
    //    /// <returns></returns>
    //    bool Load(IPassenger passenger);
    //}

    //public enum PassengerType
    //{
    //    Kanibal,
    //    Misionar
    //}
}
