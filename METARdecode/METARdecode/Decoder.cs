using System;
using System.Collections.Generic;
using System.Linq;
namespace METARdecode
{
    public class Decoder
    {
        private List<string> cloudTypes = new List<string>() { "SKC", "FEW", "SCT", "BKN", "OCV" };

        public METAR ProcessMetar(string data)
        {
            METAR metar = new METAR();

            List<string> blocks = data.Split(' ').ToList();

            if (isError(blocks, out string errMsg))
            {
                Console.WriteLine($"Error: {errMsg}");
                return metar;
            }

            // Block 0 - METAR

            // Block 1 - ICAO Code
            metar.IcaoCode = blocks[1];

            // Block 2 - UTC DateTime (DDTTTTZ)
            parseDateTime(metar, blocks[2]);

            // Block 3 - Optional - May be AUTO or COR
            if (blocks[3] == "AUTO" || blocks[3] == "COR")
                blocks.RemoveAt(3);

            // Block 3 - Wind Speed (WWWDDGWWKT)
            parseWindSpeed(metar, blocks[3]);

            // Block 4 - Optional - Variable wind direction (DDDVDDD)
            if (blocks[4].Contains("V"))
            {
                metar.VariableWindDirections = blocks[4];
                blocks.RemoveAt(4);
            }

            // Block 4 - Visibility
            metar.Visibility = blocks[4];

            // Block 5 - Optional - Runway visual range
            if (blocks[5].EndsWith("FT"))
            {
                parseRunwayVisualRange(metar, blocks[5]);
                blocks.RemoveAt(5);
            }

            // Block 5 - Weather Codes OR cloud cover
            if (cloudTypes.Any(s => blocks[5].Contains(s)))
            {
                // clouds
                metar.Clouds = blocks[5];
            }
            else
            {
                // weather codes
                metar.WeatherCodes = blocks[5];
            }

            // Block 6 - Optional - Still may be cloud codes if 5 was weather codes...
            if (!string.IsNullOrEmpty(metar.WeatherCodes) && !blocks[6].Contains("/"))
            {
                metar.Clouds = blocks[6];
                blocks.RemoveAt(6);
            }

            // Block 7 - Temperature and dew point.
            parseTempDewPoint(metar, blocks[6]);

            // Block 8 - Altimeter setting
            metar.Altimeter = blocks[7];

            // The rest - remarks
            if (blocks[8] == "RMK")
            {
                metar.Remarks = new List<string>();

                for (int i = 8; i < blocks.Count; i++)
                {
                    metar.Remarks.Add(blocks[i]);
                }
            }

            return metar;
        }

        private void parseTempDewPoint(METAR metar, string tempBlock)
        {
            string[] parts = tempBlock.Split('/');
            metar.Temperature = parts[0];
            metar.Dewpoint = parts[1];
        }

        private void parseRunwayVisualRange(METAR metar, string rvrBlock)
        {
            metar.Runway = rvrBlock.Substring(0, 4);
            metar.RunwayRange = rvrBlock.Substring(3);
        }

        private void parseWindSpeed(METAR metar, string windSpeedBlock)
        {
            metar.WindDirection = windSpeedBlock.Substring(0, 3);
            metar.WindSpeed = windSpeedBlock.Substring(2, 2);

            if (windSpeedBlock.Contains("G"))
            {
                metar.GustSpeed = windSpeedBlock.Substring(6, 2);
            }
        }

        private void parseDateTime(METAR metar, string dateTimeBlock)
        {
            metar.Day = dateTimeBlock.Substring(0, 2);
            string parsedTime = dateTimeBlock.Substring(2, 4);
            metar.Time = DateTime.ParseExact(parsedTime, "HHmm", null).TimeOfDay;
        }

        private bool isError(List<string> blocks, out string errMsg)
        {
            if (blocks[0] != "METAR")
            {
                errMsg = "Invalid METAR string. Please check input. Did you put it in double quotes?";
                return true;
            }

            if (blocks.Count < 10)
            {
                errMsg = "Invalid METAR string. Not enough elements. Is this a METAR?";
                return true;
            }

            errMsg = string.Empty;
            return false;
        }
    }
}
