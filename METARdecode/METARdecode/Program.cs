using System;

namespace METARdecode
{
    class Program
    {
        static void Main(string[] args)
        {
            Decoder decoder = new Decoder();
            decoder.ProcessMetar("KAUS 021553Z 09004KT 10SM SCT013 BKN027 OVC250 24/22 A3008 RMK AO2 SLP178 T02440217");
        }
    }
}
