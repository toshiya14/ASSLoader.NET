using ASSLoader.NET.Enums;
using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ASSLoader.NET
{

    public class ASSStyle
    {
        public string Name { get; set; }

        public string Fontname { get; set; }

        public double Fontsize { get; set; }

        public string PrimaryColour { get; set; }

        public string SecondaryColour { get; set; }

        public string OutlineColour { get; set; }

        public string BackColour { get; set; }

        public bool Bold { get; set; }

        public bool Italic { get; set; }

        public bool Underline { get; set; }

        public bool StrikeOut { get; set; }

        public double ScaleX { get; set; }

        public double ScaleY { get; set; }

        public double Spacing { get; set; }

        public double Angle { get; set; }

        public V4pStyleBorderStyle BorderStyle { get; set; }

        public double Outline { get; set; }

        public double Shadow { get; set; }

        public V4pStyleAlignment Alignment { get; set; }

        public int MarginL { get; set; }

        public int MarginR { get; set; }

        public int MarginV { get; set; }

        public int AlphaLevel { get; set; }

        public int Encoding { get; set; }

        private static ILog log = LogManager.GetLogger(typeof(ASSStyle));


        /// <summary>
        /// Convert an ASSStle object into a line of style text.
        /// </summary>
        /// <param name="v4pStyleFormat">The headings list from the "Format" of the "Styles" section.</param>
        /// <param name="style">The ASSStyle object.</param>
        /// <param name="spliter">The spliter used for stringify. Defaultly use ",".</param>
        /// <returns>The style text converted from ASSEvent.</returns>
        public string Stringify(IList<string> v4pStyleFormat, string spliter = ",")
        {
            var sb = new StringBuilder();
            var fp = CultureInfo.InvariantCulture;
            sb.Append("Style: ");
            for (var i = 0; i < v4pStyleFormat.Count; i++)
            {
                var field = v4pStyleFormat[i];
                switch (field.Trim())
                {
                    case "Name": sb.Append(this.Name); break;
                    case "Fontname": sb.Append(this.Fontname); break;
                    case "Fontsize": sb.Append(this.Fontsize.ToString(fp)); break;
                    case "PrimaryColour": sb.Append(this.PrimaryColour); break;
                    case "SecondaryColour": sb.Append(this.SecondaryColour); break;
                    case "OutlineColour": sb.Append(this.OutlineColour); break;
                    case "BackColour": sb.Append(this.BackColour); break;
                    case "Bold": sb.Append(this.Bold ? "-1" : "0"); break;
                    case "Italic": sb.Append(this.Italic ? "-1" : "0"); break;
                    case "Underline": sb.Append(this.Underline ? "-1" : "0"); break;
                    case "StrikeOut": sb.Append(this.StrikeOut ? "-1" : "0"); break;
                    case "ScaleX": sb.Append(this.ScaleX.ToString(fp)); break;
                    case "ScaleY": sb.Append(this.ScaleY.ToString(fp)); break;
                    case "Spacing": sb.Append(this.Spacing.ToString(fp)); break;
                    case "Angle": sb.Append(this.Angle.ToString(fp)); break;
                    case "BorderStyle": sb.Append((int)this.BorderStyle); break;
                    case "Outline": sb.Append(this.Outline.ToString(fp)); break;
                    case "Shadow": sb.Append(this.Shadow.ToString(fp)); break;
                    case "Alignment": sb.Append((int)this.Alignment); break;
                    case "MarginL": sb.Append(this.MarginL); break;
                    case "MarginR": sb.Append(this.MarginR); break;
                    case "MarginV": sb.Append(this.MarginV); break;
                    case "Encoding": sb.Append(this.Encoding); break;
                    default:
                        log.Warn($"STYLE MAPPING ERROR: Unknown field skipped: [{field}].");
                        break;
                }
                if (i != v4pStyleFormat.Count - 1)
                {
                    sb.Append(spliter);
                }
            }
            return sb.ToString();
        }

        private static bool ParseASSBoolean(int value)
        {
            if (value == -1)
            {
                return true;
            }
            else if(value==0){
                return false;
            }
            else
            {
                throw new ArgumentException($"The boolean value in .ass file must be '-1(true)' or '0(false)', but {value} got.");
            }
        }

        /// <summary>
        /// Convert a style text into ASSStyle object.
        /// </summary>
        /// <param name="v4pStyleFormat">The headings list from the "Format" of the "Styles" section.</param>
        /// <param name="values"></param>
        /// <exception cref="ArgumentException">If the boolean value is not '-1' or '0', it would throw <code>ArgumentException</code>.</exception>
        /// <returns></returns>
        public static ASSStyle Parse(IList<string> v4pStyleFormat, IList<string> values)
        {
            var style = new ASSStyle();
            for (var i = 0; i < v4pStyleFormat.Count; i++)
            {
                var field = v4pStyleFormat[i];
                var value = values[i];
                switch (field.Trim())
                {
                    case "Name": style.Name = value; continue;
                    case "Fontname": style.Fontname = value; continue;
                    case "Fontsize": style.Fontsize = Convert.ToDouble(value); continue;
                    case "PrimaryColour": style.PrimaryColour = value; continue;
                    case "SecondaryColour": style.SecondaryColour = value; continue;
                    case "OutlineColour": style.OutlineColour = value; continue;
                    case "BackColour": style.BackColour = value; continue;
                    case "Bold": style.Bold = ParseASSBoolean(Convert.ToInt16(value)); continue;
                    case "Italic": style.Italic = ParseASSBoolean(Convert.ToInt16(value)); continue;
                    case "Underline": style.Underline = ParseASSBoolean(Convert.ToInt16(value)); continue;
                    case "StrikeOut": style.StrikeOut = ParseASSBoolean(Convert.ToInt16(value)); continue;
                    case "ScaleX": style.ScaleX = Convert.ToDouble(value); continue;
                    case "ScaleY": style.ScaleY = Convert.ToDouble(value); continue;
                    case "Spacing": style.Spacing = Convert.ToDouble(value); continue;
                    case "Angle": style.Angle = Convert.ToDouble(value); continue;
                    case "BorderStyle": style.BorderStyle = (V4pStyleBorderStyle)Convert.ToInt16(value); continue;
                    case "Outline": style.Outline = Convert.ToDouble(value); continue;
                    case "Shadow": style.Shadow = Convert.ToDouble(value); continue;
                    case "Alignment": style.Alignment = (V4pStyleAlignment)Convert.ToInt16(value); continue;
                    case "MarginL": style.MarginL = Convert.ToInt32(value); continue;
                    case "MarginR": style.MarginR = Convert.ToInt32(value); continue;
                    case "MarginV": style.MarginV = Convert.ToInt32(value); continue;
                    case "Encoding": style.Encoding = Convert.ToInt32(value); continue;
                    default:
                        log.Warn($"STYLE MAPPING ERROR: Unknown field skipped: [{field}].");
                        continue;
                }
            }
            return style;
        }

        /// <summary>
        /// For debug output usage.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name}: {Fontname},{Fontsize}{(Bold ? "B" : "")}{(Italic ? "I" : "")}{(Underline ? "U" : "")}{(StrikeOut ? "S" : "")}";
        }
    }
}
