# METARdecode


METARdecode is a simple command line .NET core application to decode METeorological Aerodrome Reports (METARs). METAR is a format for providing weather information in aviation.

METARdecode can decode any USA METAR by simplying supplying the METAR as an argument via command line. A report will be displayed of the details.

Input:

METARdecode "KAUS 021553Z 09004KT 10SM SCT013 BKN027 OVC250 24/22 A3008 RMK AO2 SLP178 T02440217"

Output:

-------- METAR Report --------

ICAO Code: KAUS
Day of Month: 02
Time: 15:53:00 Zulu
Wind Direction: 090 Degrees
Wind Speed: 00 KTs
Visibility: 10 Statute Miles
Cloud Information: Scattered 01300 FT Broken 02700 FT Overcast 25000 FT 
Temperature: 24 C
Dew Point: 22 C
Altimiter: 30.08 Hg
Remarks: AO2 SLP178 T02440217



To-Do:

* Better error handling
* More descriptive explanations of each parsed section
* GUI interface
* If possible, retrieve METAR via API by entering an airport code.
