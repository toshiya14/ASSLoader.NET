using ASSLoader.AutoEffects;
using System.IO;

namespace ASSLoader.NET.ConsoleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            TestFunc2();
        }

        static void TestFunc1()
        {
            var ass = new ASSSubtitle();
            ass.Load(@"D:\demo.ass");
            ass.Events.Add(new ASSEvent
            {
                Type = ASSEventType.Dialogue,
                Layer = 0,
                Name = "Shaya",
                Start = new ASSEventTime("0:00:20.00"),
                End = new ASSEventTime("0:00:30.00"),
                Style = "Uta",
                Text = "Sayonara"
            });
            ass.Save(@"D:\demo_.ass");
        }

        static void TestFunc2()
        {
            var ass = new ASSSubtitle();
            var template = new Template();
            ass.Load(@"D:\vegas\200419_F7RMS_01.ass");
            template.Load(File.ReadAllText(@"D:\vegas\ass_f7rms_tpl.ast"));
            var result = template.Run(ass.Events);
            ass.Events = result;
            ass.Save(@"D:\vegas\200419_F7RMS_01_AE.ass");
        }
    }
}
