using System;
using System.Collections.Generic;
using System.Text;

namespace ASSLoader.NET
{
    public class ASSSubtitle
    {
        public Dictionary<string, string> ScriptInfo { get; set; }
        public Dictionary<string, ASSStyle> V4pStyles { get; set; }
        public IList<ASSEvent> Events { get; set; }
        public Dictionary<string, ASSEmbeddedFont> Fonts { get; set; }
        public Dictionary<string, ASSEmbeddedGraphics> Graphics { get; set; }


    }

    public class ASSStyle
    {
        /// <summary>
        /// The name of the Style. Case sensitive. Cannot include commas.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The fontname as used by Windows. Case-sensitive.
        /// </summary>
        public string Fontname { get; set; }

        /// <summary>
        /// The fontsize.
        /// </summary>
        public double Fontsize { get; set; }

        public string PrimaryColour { get; set; }

        public string SecondaryColour { get; set; }

        public string OutlineColor { get; set; }

        public string BackColour { get; set; }

        public bool Bold { get; set; }

        public bool Italic { get; set; }

        public bool Underline { get; set; }

        public bool StrikeOut { get; set; }

        public double ScaleX { get; set; }

        public double ScaleY { get; set; }

        public int Spacing { get; set; }

        public double Angle { get; set; }

        public V4pStyleBorderStyle BorderStyle { get; set; }

        public int Outline { get; set; }

        public int Shadow { get; set; }

        public V4pStyleAlignment Alignment { get; set; }

        public int MarginL { get; set; }

        public int MarginR { get; set; }

        public int MarginV { get; set; }

        public int AlphaLevel { get; set; }

        public int Encoding { get; set; }
    }

    public class ASSEvent
    {
        public ASSEventType Type { get; set; }

        public string Marked { get; set; }

        public string Start { get; set; }

        public string End { get; set; }

        public string Style { get; set; }

        public string Name { get; set; }

        public string MarginL { get; set; }

        public string MarginR { get; set; }

        public string MarginV { get; set; }

        public string Effect { get; set; }

        public string Text { get; set; }
    }

    public class ASSEmbeddedFont
    {

    }

    public class ASSEmbeddedGraphics
    {

    }

    public enum V4pStyleBorderStyle
    {
        BorderAndShadow = 1,
        ColorBackground = 3
    }

    public enum V4pStyleAlignment
    {
        SubLF = 1,
        SubCT = 2,
        SubRT = 3,
        MidLF = 4,
        MidCT = 5,
        MidRT = 6,
        TopLF = 7,
        TopCT = 8,
        TopRT = 9
    }

    public enum ASSEventType
    {
        Dialogue,
        Comment,
        Picture,
        Sound,
        Movie,
        Command
    }
}
