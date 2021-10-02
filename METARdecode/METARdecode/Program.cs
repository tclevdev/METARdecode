using System;

namespace METARdecode
{
    class Program
    {
        static void Main(string[] args)
        {
            Decoder decoder = new Decoder();
            decoder.ProcessMetar("METAR KSFO 020656Z 29006KT 10SM CLR 16/12 A2999 RMK AO2 SLP156 T01610122");
        }
    }
}
