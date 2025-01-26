using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainChecker
{
    public class ApiData
    {
        public class Rootobject
        {
            public double latitude { get; set; }
            public double longitude { get; set; }
            public double generationtime_ms { get; set; }
            public int utc_offset_seconds { get; set; }
            public string timezone { get; set; }
            public string timezone_abbreviation { get; set; }
            public double elevation { get; set; }
            public Current_Units current_units { get; set; }
            public Current current { get; set; }
            public Hourly_Units hourly_units { get; set; }
            public Hourly hourly { get; set; }
            public Daily_Units daily_units { get; set; }
            public Daily daily { get; set; }
        }

        public class Current_Units
        {
            public string time { get; set; }
            public string interval { get; set; }
            public string temperature_2m { get; set; }
            public string rain { get; set; }
            public string weather_code { get; set; }
        }

        public class Current
        {
            public string time { get; set; }
            public int interval { get; set; }
            public double temperature_2m { get; set; }
            public double rain { get; set; }
            public int weather_code { get; set; }
        }

        public class Hourly_Units
        {
            public string time { get; set; }
            public string weather_code { get; set; }
        }

        public class Hourly
        {
            public string[] time { get; set; }
            public int[] weather_code { get; set; }
        }

        public class Daily_Units
        {
            public string time { get; set; }
            public string weather_code { get; set; }
            public string temperature_2m_max { get; set; }
            public string temperature_2m_min { get; set; }
        }

        public class Daily
        {
            public string[] time { get; set; }
            public int[] weather_code { get; set; }
            public double[] temperature_2m_max { get; set; }
            public double[] temperature_2m_min { get; set; }
        }

    }
}
