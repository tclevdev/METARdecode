using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

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

        public string BuildReport(METAR metar)
        {
            StringBuilder report = new StringBuilder();
            report.AppendLine("---------- METAR Report ----------");
            report.AppendLine();
            report.AppendLine($"ICAO Code: {metar.IcaoCode}");
            report.AppendLine($"Day of Month: {metar.Day}");
            report.AppendLine($"Time: {metar.Time} Zulu");
            report.AppendLine($"Wind Direction: {metar.WindDirection} Degrees");
            report.AppendLine($"Wind Speed: {metar.WindSpeed} KTs");

            if (!string.IsNullOrEmpty(metar.GustSpeed))
                report.AppendLine($"Wind Gust Speed: {metar.GustSpeed}");

            report.AppendLine($"Visibility: {metar.Visibility.Replace("SM", " Statute Miles")}");

            if (metar.WeatherCodes != null && metar.WeatherCodes.Any())
            {
                report.Append("Weather Codes: ");
                foreach (string wCode in metar.WeatherCodes)
                {
                    report.Append(wCode + " ");
                }
                report.AppendLine();
            }

            if (metar.Clouds != null && metar.Clouds.Any())
            {
                report.Append("Cloud Information: ");
                foreach (string cCode in metar.Clouds)
                {
                    string converted = cCode.Replace("FEW", "Few ").Replace("SCT", "Scattered ").Replace("BKN", "Broken ").Replace("OVC", "Overcast ");                  
                    report.Append(converted + "00 FT ");
                }
                report.AppendLine();
            }

            report.AppendLine($"Temperature: {metar.Temperature} C");
            report.AppendLine($"Dew Point: {metar.Dewpoint} C");
            report.AppendLine($"Altimiter: {metar.Altimeter.Replace("A","").Insert(2,".")} Hg");

            if (metar.Remarks != null && metar.Remarks.Any())
            {
                report.Append("Remarks: ");
                foreach (string r in metar.Remarks)
                {
                    report.Append(r + " ");
                }
                report.AppendLine();
            }

            return report.ToString();
        }

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
                    metar.IcaoCode = block;
                    continue;
                }

                // UTC Day and Time
                regex = new Regex(dayTime);
                regexMatch = regex.Match(block);

                if (regexMatch.Success)
                {
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
                    metar.Visibility = block;

                    continue;
                }

                // Weather Comments
                regex = new Regex(weather);
                regexMatch = regex.Match(block);

                if (metar.WeatherCodes == null)
                    metar.WeatherCodes = new List<string>();

                if (regexMatch.Success)
                {
                    metar.WeatherCodes.Add(block);

                    continue;
                }

                // Cloud Details
                regex = new Regex(cloudInfo);
                regexMatch = regex.Match(block);

                if (metar.Clouds == null)
                    metar.Clouds = new List<string>();

                if (regexMatch.Success)
                {
                    metar.Clouds.Add(block);

                    continue;
                }

                // Temperature and Dew Point
                regex = new Regex(tempAndDewPt);
                regexMatch = regex.Match(block);

                if (regexMatch.Success)
                {
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
                    metar.Remarks.Add(remark);
                }
            }


            return metar;
        }
    }
}
