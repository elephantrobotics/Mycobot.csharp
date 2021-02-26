using System;

namespace Mycobot.csharp
{
    class Test 
    {
        static void Main(string[] args)
        {
            MyCobot mc = new MyCobot("/dev/ttyUSB0");
            mc.Open();
            // int[] angles = new[] {100, 100, 100, 100, 100, 100};
            // mc.SendAngles(angles, 50);
            // Thread.Sleep(5000);
            // var recv = mc.GetAngles();
            // foreach (var v in recv)
            // {
                // Console.WriteLine(v);
            // }
            
            // int[] coords = new[] {160, 160, 160, 0, 0, 0};
            // mc.SendCoords(coords, 90, 1);
            // Thread.Sleep(5000);
            // var recv = mc.GetCoords();
            // foreach (var v in recv)
            // {
                // Console.WriteLine(v);
            // }
            
            mc.SendOneAngle(1, 100,70);

            // byte[] setColor = {0xfe, 0xfe, 0x05, 0x6a, 0xff, 0x00, 0x00, 0xfa};
            mc.Close();
        }
    }
}