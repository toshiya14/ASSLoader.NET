using System;
using System.Collections.Generic;
using System.Text;

namespace ASSLoader.NET
{
    public class ASSEventTime : IComparable
    {
        public int Hour { get; set; }

        public int Minute { get; set; }

        public int Second { get; set; }

        public int Millisecond { get; set; }

        public ASSEventTime(string assTime)
        {
            var parts = assTime.Split(':', '.');
            var msIndex = parts.Length - 1;
            var secIndex = parts.Length - 2;
            var minIndex = parts.Length - 3;
            var hourIndex = parts.Length - 4;
            this.Hour = hourIndex >= 0 ? Convert.ToInt32(parts[hourIndex]) : 0;
            this.Minute = minIndex >= 0 ? Convert.ToInt32(parts[minIndex]) : 0;
            this.Second = secIndex >= 0 ? Convert.ToInt32(parts[secIndex]) : 0;
            this.Millisecond = msIndex >= 0 ? Convert.ToInt32((parts[msIndex] + "000").Substring(0, 3)) : 0;
        }

        public ASSEventTime(int hour, int minute, int second, int millisecond)
        {
            this.Hour = hour;
            this.Minute = minute;
            this.Second = second;
            this.Millisecond = millisecond;
        }

        public long TotalMilliseconds()
        {
            return this.Hour * 3600000 + this.Minute * 60000 + this.Second * 1000 + this.Millisecond;
        }

        public static explicit operator ASSEventTime(TimeSpan ts)
        {
            return new ASSEventTime(ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
        }

        public static explicit operator TimeSpan(ASSEventTime time)
        {
            return new TimeSpan(0, time.Hour, time.Minute, time.Second, time.Millisecond);
        }

        public static ASSEventTime operator +(ASSEventTime aet, double num)
        {
            var target = new ASSEventTime(aet.ToString());
            var ms = Convert.ToInt32(Math.Floor(num * 1000));
            target.Millisecond = target.Millisecond + ms;
            if (target.Millisecond > 1000)
            {
                target.Second += target.Millisecond / 1000;
                target.Millisecond = target.Millisecond % 1000;
            }
            if (target.Second > 60)
            {
                target.Minute += target.Second / 60;
                target.Second = target.Second % 60;
            }
            if (target.Minute > 60)
            {
                target.Hour += target.Minute / 60;
                target.Minute = target.Minute % 60;
            }

            return target;
        }

        public static ASSEventTime operator -(ASSEventTime aet, double num)
        {
            var ms = Convert.ToInt32(Math.Floor(num * 1000));
            var target = new ASSEventTime(aet.ToString());
            target.Millisecond = aet.Millisecond - ms;
            if (target.Millisecond < 0)
            {
                target.Millisecond += 1000;
                target.Second -= 1;
            }
            if (target.Second < 0)
            {
                target.Second += 60;
                target.Minute -= 1;
            }
            if (target.Minute < 0)
            {
                target.Minute += 60;
                target.Hour -= 1;
            }
            return target;
        }

        public override bool Equals(object obj)
        {
            return CompareTo(obj) == 0;
        }

        public override int GetHashCode()
        {
            return (int)this.TotalMilliseconds();
        }

        public override string ToString()
        {
            return this.Hour.ToString().Substring(0, 1) + ":"
                   + this.Minute.ToString().PadLeft(2, '0') + ":"
                   + this.Second.ToString().PadLeft(2, '0') + "."
                   + this.Millisecond.ToString().PadLeft(3, '0').Substring(0, 2);
        }

        public int CompareTo(object obj)
        {
            var target = obj as ASSEventTime;
            if (target == null)
            {
                target = new ASSEventTime(obj.ToString());
            }
            return (int)(this.TotalMilliseconds() - target.TotalMilliseconds());
        }
    }
}
