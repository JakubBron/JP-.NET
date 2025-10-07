using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;


namespace JP_NET.Lab1
{
    public class TimetableElement
    {
        public string id { get; set; }
        public int? delayInSeconds { get; set; }
        public DateTime estimatedTime { get; set; }
        public string headsign { get; set; }
        public int routeId { get; set; }
        public string routeShortName { get; set; }
        public DateTime scheduledTripStartTime { get; set; }
        public int tripId { get; set; }
        public string status { get; set; }
        public DateTime theoreticalTime { get; set; }
        public DateTime timestamp { get; set; }
        public int trip { get; set; }
        public int? vehicleCode { get; set; }
        public int? vehicleId { get; set; }
        public string vehicleService { get; set; }
    }

    public class Timetable
    {
        public DateTime lastUpdate { get; set; }
        public List<TimetableElement> departures { get; set; }
    }



    class DataGetter()
    {
        private string url = "https://ckan2.multimediagdansk.pl/departures?stopId=";
        

        public string GetJSONFromWeb(int stopID)
        {
            using (var client = new HttpClient())
            {
                var response = client.GetStringAsync(url + stopID).Result;
                return response;
            }
        }
    }

    class JSONProcessor()
    {
        public Timetable[] ProcessJSON(string json)
        {
            var timetable = JsonSerializer.Deserialize<Timetable>(json);
            // adjust time from UTC to localtime
            timetable.lastUpdate = timetable.lastUpdate.ToLocalTime();
            foreach (var departure in timetable.departures)
            {
                departure.estimatedTime = departure.estimatedTime.ToLocalTime();
            }
            return new Timetable[] { timetable };
        }
    }

    class ProgramZTM
    {
        public static void Main()
        {
            DataGetter dg = new DataGetter();
            JSONProcessor jp = new JSONProcessor();

            int stopID = 0;

            // Get stop ID from user
            Console.Write("Enter stop ID: ");
            while (!int.TryParse(Console.ReadLine(), out stopID))
            {
                Console.Write("Invalid input. Enter a valid stop ID: ");
            }

            // Get JSON from web
            string json = dg.GetJSONFromWeb(stopID);
            if (string.IsNullOrEmpty(json))
            {
                Console.WriteLine("Incorrect stop ID, response is empty.");
                return;
            }

            // process JSON to Timetable[]
            Timetable[] timetables = jp.ProcessJSON(json);

            // Print the results
            foreach (var timetable in timetables)
            {
                Console.WriteLine($"Last Update: {timetable.lastUpdate}");
                Console.WriteLine($"{"Route name",-10} | {"Heading to",-30} | {"Departure at",-20}");
                Console.WriteLine(new string('-', 70));
                foreach (var departure in timetable.departures)
                {
                    Console.WriteLine(
                        $"{departure.routeShortName,-10} | {departure.headsign,-30} | {departure.estimatedTime,-20}"
                    );
                }
            }


        }
    }
}
