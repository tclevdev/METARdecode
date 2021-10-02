using System;
using System.Collections.Generic;
namespace METARdecode
{
    public class METAR
    {
        public string IcaoCode { get; set; }
        public string Day { get; set; }
        public TimeSpan Time { get; set; }
        public string WindDirection { get; set; }
        public string WindSpeed { get; set; }
        public string GustSpeed { get; set; }
        public string VariableWindDirections { get; set; }
        public string Visibility { get; set; }
        public string Runway { get; set; }
        public string RunwayRange { get; set; }
        public string WeatherCodes { get; set; }
        public string Clouds { get; set; }
        public string Temperature { get; set; }
        public string Dewpoint { get; set; }
        public string Altimeter { get; set; }
        public List<string> Remarks { get; set; }
    }
}
