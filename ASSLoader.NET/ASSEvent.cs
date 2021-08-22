using ASSLoader.NET.Enums;
using log4net;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASSLoader.NET
{
    public class ASSEvent : ICloneable
    {
        public ASSEventType Type { get; set; }

        public int Layer { get; set; }

        public ASSEventTime Start { get; set; }

        public ASSEventTime End { get; set; }

        public string Style { get; set; }

        public string Name { get; set; }

        public int MarginL { get; set; }

        public int MarginR { get; set; }

        public int MarginV { get; set; }

        public string Effect { get; set; }

        public string Text { get; set; }

        private static ILog log = LogManager.GetLogger(typeof(ASSStyle));

        public static IList<string> DefaultFormat = new List<string> {
            "Layer", "Start", "End", "Style", "Name", "MarginL", "MarginR", "MarginV", "Effect", "Text"
        };

        public object Clone()
        {
            var newInst = new ASSEvent();
            newInst.Type = this.Type;
            newInst.Layer = this.Layer;
            newInst.Start = new ASSEventTime(this.Start.ToString());
            newInst.End = new ASSEventTime(this.End.ToString());
            newInst.Style = this.Style;
            newInst.Name = this.Name;
            newInst.MarginL = this.MarginL;
            newInst.MarginR = this.MarginR;
            newInst.MarginV = this.MarginV;
            newInst.Effect = this.Effect;
            newInst.Text = this.Text;
            return newInst;
        }

        public ASSEvent()
        {
            this.Type = ASSEventType.Dialogue;
            this.Layer = 0;
            this.Start = new ASSEventTime(0, 0, 0, 0);
            this.End = new ASSEventTime(0, 0, 0, 0);
            this.Style = "Default";
            this.Name = string.Empty;
            this.MarginL = 0;
            this.MarginR = 0;
            this.MarginV = 0;
            this.Effect = string.Empty;
            this.Text = string.Empty;
        }


        /// <summary>
        /// Convert an event text into ASSEvent object.
        /// </summary>
        /// <param name="prefix">The prefix of the line of the event text. Must be one of the ASSEventType.</param>
        /// <param name="eventFormat">The headings list from the "Format" of the "Events" section.</param>
        /// <param name="values">The values list of the line of the event text.</param>
        /// <returns>The ASSEvent object.</returns>
        public static ASSEvent Parse(string prefix, IList<string> eventFormat, IList<string> values)
        {
            var eventType = (ASSEventType)Enum.Parse(typeof(ASSEventType), prefix);
            var evt = new ASSEvent();
            evt.Type = eventType;
            for (var i = 0; i < eventFormat.Count; i++)
            {
                var field = eventFormat[i];
                var value = values[i];
                switch (field.Trim())
                {
                    case "Layer": evt.Layer = Convert.ToInt32(value); continue;
                    case "Start": evt.Start = new ASSEventTime(value); continue;
                    case "End": evt.End = new ASSEventTime(value); continue;
                    case "Style": evt.Style = value; continue;
                    case "Name": evt.Name = value; continue;
                    case "MarginL": evt.MarginL = Convert.ToInt32(value); continue;
                    case "MarginR": evt.MarginR = Convert.ToInt32(value); continue;
                    case "MarginV": evt.MarginV = Convert.ToInt32(value); continue;
                    case "Effect": evt.Effect = value; continue;
                    case "Text": evt.Text = value; continue;
                    default:
                        log.Warn($"EVENT MAPPING ERROR: Unknown field skipped: [{field}].");
                        continue;
                }
            }
            return evt;
        }

        /// <summary>
        /// Convert an ASSEvent object into a line of event text.
        /// </summary>
        /// <param name="eventFormat">The headings list from the "Format" of the "Events" section.</param>
        /// <param name="spliter">The spliter used for stringify. Defaultly use ",".</param>
        /// <returns>The event text converted from ASSEvent.</returns>
        public string Stringify(IList<string> eventFormat, string spliter = ",")
        {
            var sb = new StringBuilder();
            sb.Append(this.Type.ToString() + ": ");
            for (var i = 0; i < eventFormat.Count; i++)
            {
                var field = eventFormat[i];
                switch (field.Trim())
                {
                    case "Layer": sb.Append(this.Layer); break;
                    case "Start": sb.Append(this.Start); break;
                    case "End": sb.Append(this.End); break;
                    case "Style": sb.Append(this.Style); break;
                    case "Name": sb.Append(this.Name); break;
                    case "MarginL": sb.Append(this.MarginL); break;
                    case "MarginR": sb.Append(this.MarginR); break;
                    case "MarginV": sb.Append(this.MarginV); break;
                    case "Effect": sb.Append(this.Effect); break;
                    case "Text": sb.Append(this.Text); break;
                    default:
                        log.Warn($"EVENT MAPPING ERROR: Unknown field skipped: [{field}].");
                        break;
                }
                if (i != eventFormat.Count - 1)
                {
                    sb.Append(spliter);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Only for debug usage.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Start} - {End} | {Type}:{Name}:{Text}";
        }
    }

}
