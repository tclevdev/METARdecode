using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace METARdecode
{
    public class Decoder
    {
        // Regexes
        private static string icaoCode = "^[A-Z]{4}$";
        private static string dayTime = "([0-9]{2})([0-9]{2})([0-9]{2})Z";
        private static string wind = "([0-9]{3}|VRB)([0-9]{2,3})G?([0-9]{2,3})?(KT)";
        private static string visibility = "([0-9]{2,3,4})?(SM)";
        private static string weather = "^(VC)?(-|\\+)?(MI|PR|BC|DR|BL|SH|TS|FZ)?((DZ|RA|SN|SG|IC|PL|GR|GS|UP)+)?(BR|FG|FU|VA|DU|SA|HZ|PY)?(PO|SQ|FC|SS)?$";
        private static string cloudInfo = "^(VV|FEW|SCT|SKC|CLR||BKN|OVC)([0-9]{3}|///)(CU|CB|TCU|CI)?$";
        private static string tempAndDewPt = "^(M?[0-9]{2})/(M?[0-9]{2})?$";
        private static string altimiter = "A([0-9]{4})";

        public METAR ProcessMetar(string data)
        {
            METAR metar = new METAR();

            List<string> blocks = data.Split(' ').ToList();

            // If starts with METAR, just strip it off.
            if (blocks[0] == "METAR")
                blocks.RemoveAt(0);

            foreach (string block in blocks)
            {
                // ICAO Airport Code
                Regex regex = new Regex(icaoCode);
                Match regexMatch = regex.Match(block);

                if (regexMatch.Success)
                {
                    Console.WriteLine($"Found ICAO Code: {block}");
                    metar.IcaoCode = block;
                    continue;
                }

                // UTC Day and Time
                regex = new Regex(dayTime);
                regexMatch = regex.Match(block);

                if (regexMatch.Success)
                {
                    Console.WriteLine($"Found Day and Time: {block}");
                    metar.Day = block.Substring(0, 2);
                    string parsedTime = block.Substring(2, 4);
                    metar.Time = DateTime.ParseExact(parsedTime, "HHmm", null).TimeOfDay;

                    continue;
                }

                // Wind Direction/Speed. Optional Gust Speed.
                regex = new Regex(wind);
                regexMatch = regex.Match(block);

                if (regexMatch.Success)
                {
                    Console.WriteLine($"Found Wind: {block}");
                    metar.WindDirection = block.Substring(0, 3);
                    metar.WindSpeed = block.Substring(2, 2);

                    if (block.Contains("G"))
                        metar.GustSpeed = block.Substring(6, 2);

                    continue;
                }

                // Visibility
                regex = new Regex(visibility);
                regexMatch = regex.Match(block);

                if (regexMatch.Success)
                {
                    Console.WriteLine($"Found Visibility: {block}");
                    metar.Visibility = block;

                    continue;
                }

                // Weather Comments
                regex = new Regex(weather);
                regexMatch = regex.Match(block);

                if (regexMatch.Success)
                {
                    Console.WriteLine($"Found Weather: {block}");
                    metar.WeatherCodes = block;

                    continue;
                }

                // Cloud Details
                regex = new Regex(cloudInfo);
                regexMatch = regex.Match(block);

                if (metar.Clouds == null)
                    metar.Clouds = new List<string>();

                if (regexMatch.Success)
                {
                    Console.WriteLine($"Found Cloud Details: {block}");
                    metar.Clouds.Add(block);

                    continue;
                }

                // Temperature and Dew Point
                regex = new Regex(tempAndDewPt);
                regexMatch = regex.Match(block);

                if (regexMatch.Success)
                {
                    Console.WriteLine($"Found Temperature and Dew Point: {block}");
                    string[] parts = block.Split('/');
                    metar.Temperature = parts[0];
                    metar.Dewpoint = parts[1];

                    continue;
                }

                // Altimiter Setting
                regex = new Regex(altimiter);
                regexMatch = regex.Match(block);

                if (regexMatch.Success)
                {
                    Console.WriteLine($"Found Altimier Setting: {block}");
                    metar.Altimeter = block;

                    continue;
                }
            }

            // Parse any Remarks

            if (data.Contains("RMK"))
            {
                string remarks = data.Split("RMK ")[1];
                string[] remarkArray = remarks.Split(' ');

                if (metar.Remarks == null)
                    metar.Remarks = new List<string>();

                foreach (string remark in remarkArray)
                {
                    Console.WriteLine($"Found Remark: {remark}");
                    metar.Remarks.Add(remark);
                }
            }


            return metar;
        }
    }
}
