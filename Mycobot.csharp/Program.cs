using System;
using System.Threading;

namespace Mycobot.csharp
{
    class Test
    {
        static void Main(string[] args)
        {
            MyCobot mc = new MyCobot("COM14");//树莓派机械臂串口名称：/dev/ttyAMA0
            mc.Open();
            Thread.Sleep(5000);//windows打开串口后，需要等待5s，Windows打开串口底部basic会重启
            // int[] angles = new[] {100, 100, 100, 100, 100, 100};
            // mc.SendAngles(angles, 50);
            double[] a = { 0, 0, 90, 0, 0, 0 };
            mc.SendAngles(a, 70);

                Thread.Sleep(1000);
                float[] cdd = mc.GetAngles();
                foreach (var v in cdd)
                {
                    Console.WriteLine(v);
                }
            mc.SendOneCoord(1, -60, 70);
            Thread.Sleep(1000);
            mc.SendOneCoord(1, 20, 70);
            /*
                        mc.SendOneCoord(1,20, 70);
                        Thread.Sleep(1000);
                        var recv = mc.GetAngles();
                        foreach (var v in recv)
                        {
                           Console.WriteLine(v);
                        }
            */
            // int[] coords = new[] {160, 160, 160, 0, 0, 0};
            // mc.SendCoords(coords, 90, 1);
            // Thread.Sleep(5000);
            // var recv = mc.GetCoords();
            // foreach (var v in recv)
            // {
            // Console.WriteLine(v);
            // }
            /*           float[] b = { 0,0,0,0,0,0 };
                       mc.SendAngles(b, 70);
                       Thread.Sleep(1000);
                       mc.SendOneAngle(3, 90, 70);
                       Thread.Sleep(1000);
            */

            /*var angle = new int[6];
            angle = mc.GetAngles();
            foreach (var v in angle)
                Console.WriteLine(v);
            // byte[] setColor = {0xfe, 0xfe, 0x05, 0x6a, 0xff, 0x00, 0x00, 0xfa};*/

            //set basic output io
            /*mc.SetBasicOut(2, 1);
            Thread.Sleep(100);
            mc.SetBasicOut(5, 1);
            Thread.Sleep(100);
            mc.SetBasicOut(26, 1);
            Thread.Sleep(100);*/

            //get basic input io
            Console.WriteLine(mc.GetBasicIn(35));
            Thread.Sleep(100);
            Console.WriteLine(mc.GetBasicIn(36));
            Thread.Sleep(100);

            //set atom output io
            /*mc.SetDigitalOut(23, 0);
            Thread.Sleep(100);
            mc.SetDigitalOut(33, 0);
            Thread.Sleep(100);*/

            //get m5 input io
            /*Console.WriteLine(mc.GetDigitalIn(19));
            Thread.Sleep(100);
            Console.WriteLine(mc.GetDigitalIn(22));
            Thread.Sleep(100);*/

            //set gripper open or close 0--close 100-open max 0-100
            /*mc.setGripperValue(0, 10);
            Thread.Sleep(3000);
            mc.setGripperValue(50, 100);
            Thread.Sleep(3000);*/

            //get gripper state 0--close 1--open
            /*Console.WriteLine(mc.getGripperValue());*/
        }
    }
}