namespace ASSLoader.NET.ConsoleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var ass = new ASSSubtitle();
            ass.Load(@"D:\demo.ass");
            ass.Events.Add(new ASSEvent
            {
                Type = ASSEventType.Dialogue,
                Layer = 0,
                Name = "Shaya",
                Start = "0:00:20.00",
                End = "0:00:30.00",
                Style = "Uta",
                Text = "Sayonara"
            });
            ass.Save(@"D:\demo_.ass");
        }
    }
}
