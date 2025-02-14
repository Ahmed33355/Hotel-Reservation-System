using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelReservationSystem
{
    // Represents a hotel room
    public class Room
    {
        public int RoomNumber { get; set; }
        public string Type { get; set; }   // e.g., Single, Double, Suite
        public double Price { get; set; }

        public Room(int roomNumber, string type, double price)
        {
            RoomNumber = roomNumber;
            Type = type;
            Price = price;
        }

        public override string ToString()
        {
            return $"Room {RoomNumber} - {Type} - ${Price}";
        }
    }

    // Represents a guest booking a room
    public class Guest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public Guest(string name, string email, string phone)
        {
            Name = name;
            Email = email;
            Phone = phone;
        }
    }

    // Represents a reservation made by a guest
    public class Reservation
    {
        public Guid ReservationId { get; set; }
        public Room Room { get; set; }
        public Guest Guest { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }

        public Reservation(Room room, Guest guest, DateTime checkIn, DateTime checkOut)
        {
            ReservationId = Guid.NewGuid(); // Unique identifier for each reservation
            Room = room;
            Guest = guest;
            CheckIn = checkIn;
            CheckOut = checkOut;
        }

        public override string ToString()
        {
            return $"Reservation ID: {ReservationId}\n" +
                   $"Guest: {Guest.Name}\n" +
                   $"Room: {Room.RoomNumber} ({Room.Type})\n" +
                   $"Check-In: {CheckIn.ToShortDateString()}\n" +
                   $"Check-Out: {CheckOut.ToShortDateString()}";
        }
    }

    // Manages the list of rooms and reservations
    public class Hotel
    {
        public List<Room> Rooms { get; set; }
        public List<Reservation> Reservations { get; set; }

        public Hotel()
        {
            Rooms = new List<Room>();
            Reservations = new List<Reservation>();
            PopulateRooms();
        }

        // Adds some sample rooms to the hotel
        private void PopulateRooms()
        {
            Rooms.Add(new Room(101, "Single", 100));
            Rooms.Add(new Room(102, "Double", 150));
            Rooms.Add(new Room(103, "Suite", 250));
            Rooms.Add(new Room(104, "Single", 100));
            Rooms.Add(new Room(105, "Double", 150));
        }

        // Returns a list of rooms that are available for the given dates
        public List<Room> GetAvailableRooms(DateTime checkIn, DateTime checkOut)
        {
            var availableRooms = new List<Room>();

            foreach (var room in Rooms)
            {
                bool isBooked = Reservations.Any(res =>
                    res.Room.RoomNumber == room.RoomNumber &&
                    res.CheckIn < checkOut &&   // Overlap check
                    checkIn < res.CheckOut);

                if (!isBooked)
                {
                    availableRooms.Add(room);
                }
            }
            return availableRooms;
        }

        // Creates a reservation if the room is available
        public Reservation MakeReservation(Guest guest, int roomNumber, DateTime checkIn, DateTime checkOut)
        {
            var room = Rooms.FirstOrDefault(r => r.RoomNumber == roomNumber);
            if (room == null)
            {
                throw new Exception("Room not found.");
            }

            bool isBooked = Reservations.Any(res =>
                res.Room.RoomNumber == roomNumber &&
                res.CheckIn < checkOut &&
                checkIn < res.CheckOut);

            if (isBooked)
            {
                throw new Exception("Room is not available for the selected dates.");
            }

            Reservation reservation = new Reservation(room, guest, checkIn, checkOut);
            Reservations.Add(reservation);
            return reservation;
        }

        // Cancels a reservation by its unique Reservation ID
        public bool CancelReservation(Guid reservationId)
        {
            var reservation = Reservations.FirstOrDefault(r => r.ReservationId == reservationId);
            if (reservation != null)
            {
                Reservations.Remove(reservation);
                return true;
            }
            return false;
        }

        // Displays all reservations
        public void ListReservations()
        {
            if (Reservations.Count == 0)
            {
                Console.WriteLine("No reservations found.");
            }
            else
            {
                foreach (var res in Reservations)
                {
                    Console.WriteLine(res);
                    Console.WriteLine("-------------------------------------");
                }
            }
        }
    }

    // Main program with a simple text-based UI
    class Program
    {
        static Hotel hotel = new Hotel();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("=== Hotel Reservation System ===");
                Console.WriteLine("1. View Available Rooms");
                Console.WriteLine("2. Make Reservation");
                Console.WriteLine("3. Cancel Reservation");
                Console.WriteLine("4. List All Reservations");
                Console.WriteLine("5. Exit");
                Console.Write("Select an option: ");
                string choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            ViewAvailableRooms();
                            break;
                        case "2":
                            MakeReservation();
                            break;
                        case "3":
                            CancelReservation();
                            break;
                        case "4":
                            ListReservations();
                            break;
                        case "5":
                            return;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                Console.WriteLine();
            }
        }

        // Prompts user to enter dates and displays available rooms
        static void ViewAvailableRooms()
        {
            Console.Write("Enter Check-In Date (yyyy-mm-dd): ");
            DateTime checkIn = DateTime.Parse(Console.ReadLine());
            Console.Write("Enter Check-Out Date (yyyy-mm-dd): ");
            DateTime checkOut = DateTime.Parse(Console.ReadLine());

            var availableRooms = hotel.GetAvailableRooms(checkIn, checkOut);
            if (availableRooms.Count == 0)
            {
                Console.WriteLine("No available rooms for the selected dates.");
            }
            else
            {
                Console.WriteLine("Available Rooms:");
                foreach (var room in availableRooms)
                {
                    Console.WriteLine(room);
                }
            }
        }

        // Collects guest details and reservation dates, then makes a reservation
        static void MakeReservation()
        {
            Console.Write("Enter Guest Name: ");
            string name = Console.ReadLine();
            Console.Write("Enter Guest Email: ");
            string email = Console.ReadLine();
            Console.Write("Enter Guest Phone: ");
            string phone = Console.ReadLine();
            Guest guest = new Guest(name, email, phone);

            Console.Write("Enter Check-In Date (yyyy-mm-dd): ");
            DateTime checkIn = DateTime.Parse(Console.ReadLine());
            Console.Write("Enter Check-Out Date (yyyy-mm-dd): ");
            DateTime checkOut = DateTime.Parse(Console.ReadLine());

            var availableRooms = hotel.GetAvailableRooms(checkIn, checkOut);
            if (availableRooms.Count == 0)
            {
                Console.WriteLine("No available rooms for the selected dates.");
                return;
            }

            Console.WriteLine("Available Rooms:");
            foreach (var room in availableRooms)
            {
                Console.WriteLine(room);
            }

            Console.Write("Enter Room Number to reserve: ");
            int roomNumber = int.Parse(Console.ReadLine());

            Reservation reservation = hotel.MakeReservation(guest, roomNumber, checkIn, checkOut);
            Console.WriteLine("Reservation successful!");
            Console.WriteLine("Reservation Details:");
            Console.WriteLine(reservation);
        }

        // Cancels a reservation based on the Reservation ID provided by the user
        static void CancelReservation()
        {
            Console.Write("Enter Reservation ID to cancel: ");
            string input = Console.ReadLine();
            if (Guid.TryParse(input, out Guid reservationId))
            {
                bool result = hotel.CancelReservation(reservationId);
                if (result)
                {
                    Console.WriteLine("Reservation canceled successfully.");
                }
                else
                {
                    Console.WriteLine("Reservation not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid Reservation ID format.");
            }
        }

        // Lists all reservations
        static void ListReservations()
        {
            hotel.ListReservations();
        }
    }
}
