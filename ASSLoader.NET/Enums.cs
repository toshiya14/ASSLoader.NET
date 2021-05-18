using System;
using System.Collections.Generic;
using System.Text;

namespace ASSLoader.NET.Enums
{
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
        LeftBottom = 1,
        CenterBottom = 2,
        RightBottom = 3,
        LeftMiddle = 4,
        CenterMiddle = 5,
        RightMiddle = 6,
        LeftTop = 7,
        CenterTop = 8,
        RightTop = 9
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

    public enum ASSSection
    {
        Unknown,
        ScriptInfo,
        V4pStyles,
        Events,
        Fonts,
        Graphics
    }
}
