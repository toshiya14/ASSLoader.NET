using ASSLoader.NET.Enums;
using ASSLoader.NET.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ASSLoader.NET
{
    public struct ASSScriptInfo
    {
        public ScriptInfoType Type;
        public string Key;
        public string Value;

        // implicit converted from tuple to keep compatibility with old version.
        public static implicit operator ASSScriptInfo(Tuple<string, string> tuple)
        {
            var si = new ASSScriptInfo();
            switch (tuple.Item1.ToLower())
            {
                case "comment":
                    si.Type = ScriptInfoType.Comments;
                    si.Value = tuple.Item2;
                    break;

                case "key-value":
                    si.Type = ScriptInfoType.KeyValue;
                    break;

                default:
                    si.Type = ScriptInfoType.Unspecified;
                    break;
            }
            si.Value = tuple.Item2;
            return si;
        }
    }
    public class ASSSubtitle
    {
        public Dictionary<string, ASSScriptInfo> ScriptInfo { get; set; }

        public IList<string> V4pStyleFormat { get; set; }

        public Dictionary<string, ASSStyle> V4pStyles { get; set; }

        public IList<string> EventFormat { get; set; }

        public IList<ASSEvent> Events { get; set; }

        /// <summary>
        /// *** NOT IMPLEMENT FOR NOW.
        /// </summary>
        public Dictionary<string, ASSEmbeddedFont> Fonts { get; set; }

        /// <summary>
        /// *** NOT IMPLEMENT FOR NOW.
        /// </summary>
        public Dictionary<string, ASSEmbeddedGraphics> Graphics { get; set; }

        public Dictionary<string, string> UnknownSections { get; set; }

        /// <summary>
        /// Load a ".ass" file, and generate the ASSSubtitle object.
        /// </summary>
        /// <param name="path">The path of the ".ass" file.</param>
        /// <param name="enc">The encoding used to process the file. Default encoding is 'utf-8'(no BOM).</param>
        public void Load(string path, Encoding enc = null)
        {
            string[] lines;
            if (enc == null)
            {
                lines = File.ReadAllLines(path);
            }
            else
            {
                lines = File.ReadAllLines(path, enc);
            }

            var workingSection = ASSSection.ScriptInfo;
            var scriptInfoCommentIndex = 0;
            var unknowSectionName = string.Empty;

            V4pStyleFormat = new List<string>();
            EventFormat = new List<string>();
            ScriptInfo = new Dictionary<string, ASSScriptInfo>();
            V4pStyles = new Dictionary<string, ASSStyle>();
            Events = new List<ASSEvent>();
            Fonts = new Dictionary<string, ASSEmbeddedFont>();
            Graphics = new Dictionary<string, ASSEmbeddedGraphics>();
            UnknownSections = new Dictionary<string, string>();

            // Regex defination
            var regexScriptInfoKeyValue = new Regex(@"^(?<key>[0-9a-zA-z ]+)\s*\:\s*(?<value>.+)$");
            var availablePrefix = Enum.GetNames(typeof(ASSEventType));
            var regexAvailablePrefix = new Regex(@"^(?<prefix>" + string.Join("|", availablePrefix) + @"):\s*(?<values>.+)$");

            for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                var line = lines[lineIndex].Trim();

                // Format checking
                if (lineIndex == 0)
                {
                    if (!line.StartsWith("[Script Info]"))
                    {
                        throw new ASSFileFormatException(path, lineIndex, "This is not a correct ASS(V4+) standard style file. The first line is not [Script Info].");
                    }
                }

                if (string.IsNullOrWhiteSpace(line))
                {
                    // Skip space line.
                    continue;
                }

                // Section detection.
                if (line.StartsWith("["))
                {
                    if (line.StartsWith("[Script Info]"))
                    {
                        workingSection = ASSSection.ScriptInfo;
                        continue;
                    }
                    else if (line.StartsWith("[V4+ Styles]"))
                    {
                        workingSection = ASSSection.V4pStyles;
                        continue;
                    }
                    else if (line.StartsWith("[Events]"))
                    {
                        workingSection = ASSSection.Events;
                        continue;
                    }
                    else if (line.StartsWith("[Fonts]"))
                    {
                        workingSection = ASSSection.Fonts;
                        continue;
                    }
                    else if (line.StartsWith("[Graphics]"))
                    {
                        workingSection = ASSSection.Graphics;
                        continue;
                    }
                    else
                    {
                        workingSection = ASSSection.Unknown;
                        unknowSectionName = line;
                        continue;
                    }
                }

                switch (workingSection)
                {
                    // Unknown section processor.
                    // Each unknown sections would keep the same while outputing.
                    case ASSSection.Unknown:
                        if (UnknownSections.ContainsKey(unknowSectionName))
                        {
                            UnknownSections[unknowSectionName] += "\n" + line;
                        }
                        else
                        {
                            UnknownSections[unknowSectionName] = line;
                        }
                        continue;

                    // [Script Info] Processor.
                    // Lines starts with `!:` and `;` would be skipped because they are identified as comments.
                    // `regexScriptInfoKeyValue` would be used for the other lines to parse the key-value pair script informations.
                    case ASSSection.ScriptInfo:
                        if (line.StartsWith("!:"))
                        {
                            ScriptInfo["Comment" + scriptInfoCommentIndex] = new ASSScriptInfo { Type = ScriptInfoType.Comments, Value = line.Substring(2) };
                            scriptInfoCommentIndex++;
                        }
                        else if (line.StartsWith(";"))
                        {
                            ScriptInfo["Comment" + scriptInfoCommentIndex] = new ASSScriptInfo { Type = ScriptInfoType.Comments, Value = line.Substring(1) };
                            scriptInfoCommentIndex++;
                        }
                        else
                        {
                            var match = regexScriptInfoKeyValue.Match(line);
                            if (match.Success)
                            {
                                var key = match.Groups["key"].Value;
                                var value = match.Groups["value"].Value;
                                ScriptInfo[key] = new ASSScriptInfo { Type = ScriptInfoType.KeyValue, Key = key, Value = value };
                            }
                            else
                            {
                                Trace.TraceWarning("LINE " + (lineIndex + 1) + ": Unknown syntax while load key-value in Script Info Section.");
                                Trace.TraceWarning("LINE " + (lineIndex + 1) + ": " + line);
                            }
                        }
                        continue;

                    // [V4+Styles] Processor
                    //
                    // 1. Check the first line in this section.
                    //    It should be started with "Format:" and followed with the headings of each column splitted by ",".
                    //    If "Format" line was missing, the whole [V4+Styles] section would be skipped.
                    //
                    // 2. The remaining lines should be started with "Style:" and followed by the values of each column splitted by ",".
                    //    * The count of the values in each line of these "Style" must as same as the count of headings in "Format".
                    //    * `MappingToStyle` function could help to parse the list of values(not the whole string of the line) into ASSStyle object.
                    case ASSSection.V4pStyles:
                        if (V4pStyleFormat.Count == 0)
                        {
                            if (line.StartsWith("Format:"))
                            {
                                V4pStyleFormat = line.Substring(7).Split(',').Select(x => x.Trim()).ToList();
                            }
                            else
                            {
                                Trace.TraceWarning("LINE " + (lineIndex + 1) + ": The format has not been defined, this line would be skipped.");
                                Trace.TraceWarning("LINE " + (lineIndex + 1) + ": " + line);
                            }
                        }
                        else
                        {
                            if (line.StartsWith("Style:"))
                            {
                                var values = line.Substring(6).Split(',').Select(x => x.Trim()).ToList();
                                if (values.Count == V4pStyleFormat.Count)
                                {
                                    try
                                    {
                                        // mapping
                                        var style = ASSStyle.Parse(V4pStyleFormat, values);
                                        V4pStyles[style.Name] = style;
                                    }
                                    catch (Exception exc)
                                    {
                                        Trace.TraceWarning("LINE " + (lineIndex + 1) + ": Error(" + exc.Message + ") while map this line to object.");
                                        Trace.TraceWarning("LINE " + (lineIndex + 1) + ": " + line);
                                    }
                                }
                                else
                                {
                                    Trace.TraceWarning("LINE " + (lineIndex + 1) + ": the count of the fields(" + values.Count + ") in style do not match with format defined(" + V4pStyleFormat.Count + "), this line would be skipped.");
                                    Trace.TraceWarning("LINE " + (lineIndex + 1) + ": " + line);
                                }
                            }
                            else
                            {
                                Trace.TraceWarning("LINE " + (lineIndex + 1) + ": Unknown syntax not started with 'Style:', this line would be skipped.");
                                Trace.TraceWarning("LINE " + (lineIndex + 1) + ": " + line);
                            }
                        }
                        continue;

                    // [Events] Processor
                    //
                    // 1. Check the first line in this section.
                    //    It should be started with "Format:" and followed with the headings of each column splitted by ",".
                    //    If "Format" line was missing, the whole [Events] section would be skipped.
                    //
                    // 2. The remaining lines should be started with one of `ASSEventType` and ":".
                    //    Then followed by the values of each column splitted by ",".
                    //    * The count of the values in each line of these "Event" must as same as the count of headings in "Format".
                    //    * `MappingToEvent` function could help to parse the list of values(not the whole string of the line) into ASSEvent object.
                    case ASSSection.Events:
                        if (EventFormat.Count == 0)
                        {
                            if (line.StartsWith("Format:"))
                            {
                                EventFormat = line.Substring(7).Split(',').Select(x => x.Trim()).ToList();
                            }
                            else
                            {
                                Trace.TraceWarning("LINE " + (lineIndex + 1) + ": The format has not been defined, this line would be skipped.");
                                Trace.TraceWarning("LINE " + (lineIndex + 1) + ": " + line);
                            }
                        }
                        else
                        {
                            var match = regexAvailablePrefix.Match(line);
                            if (match.Success)
                            {
                                var values = match.Groups["values"].Value.Split(new[] { ',' }, EventFormat.Count).Select(x => x.Trim()).ToList();
                                if (values.Count == EventFormat.Count)
                                {
                                    try
                                    {
                                        // mapping
                                        var evt = ASSEvent.Parse(match.Groups["prefix"].Value, EventFormat, values);
                                        Events.Add(evt);
                                    }
                                    catch (Exception exc)
                                    {
                                        Trace.TraceWarning("LINE " + (lineIndex + 1) + ": Error(" + exc.Message + ") while map this line to object.");
                                        Trace.TraceWarning("LINE " + (lineIndex + 1) + ": " + line);
                                    }
                                }
                                else
                                {
                                    Trace.TraceWarning("LINE " + (lineIndex + 1) + ": the count of the fields(" + values.Count + ") in style do not match with format defined(" + EventFormat.Count + "), this line would be skipped.");
                                    Trace.TraceWarning("LINE " + (lineIndex + 1) + ": " + line);
                                }
                            }
                            else
                            {
                                Trace.TraceWarning("LINE " + (lineIndex + 1) + ": Unknown syntax, this line would be skipped.");
                                Trace.TraceWarning("LINE " + (lineIndex + 1) + ": " + line);
                            }
                        }
                        continue;
                }
            }
        }

        /// <summary>
        /// Convert ASSSubtitle object into ".ass" text formatted string.
        /// </summary>
        /// <returns>ASS formatted string.</returns>
        public string Stringify()
        {
            var sb = new StringBuilder();

            // Script Info
            sb.AppendLine("[Script Info]");
            foreach (var kv in ScriptInfo)
            {
                var si = kv.Value;
                if (si.Type == ScriptInfoType.Comments)
                {
                    sb.AppendLine($";{si.Value}");
                }
                if (si.Type == ScriptInfoType.KeyValue)
                {
                    sb.AppendLine($"{si.Key}: {si.Value}");
                }
            }
            sb.AppendLine();

            // Unknown Sections
            foreach (var us in UnknownSections)
            {
                sb.AppendLine(us.Key);
                sb.AppendLine(us.Value);
                sb.AppendLine();
            }

            // V4+ Styles
            sb.AppendLine("[V4+ Styles]");
            sb.AppendLine($"Format: {string.Join(", ", V4pStyleFormat)}");
            foreach (var s in V4pStyles)
            {
                var style = s.Value;
                sb.AppendLine(style.Stringify(V4pStyleFormat));
            }
            sb.AppendLine();

            // Events
            sb.AppendLine("[Events]");
            sb.AppendLine($"Format: {string.Join(", ", EventFormat)}");
            foreach (var ev in Events)
            {
                sb.AppendLine(ev.Stringify(EventFormat));
            }
            sb.AppendLine();

            return sb.ToString();
        }

        /// <summary>
        /// Convert ASSSubtitle object into ".ass" text formatted string, and then save into a file.
        /// </summary>
        /// <param name="path">The path of the ".ass" file.</param>
        /// <param name="enc">The encoding used to process the file. Default encoding is 'utf-8'(no BOM).</param>
        public void Save(string path, Encoding enc = null)
        {
            if (enc == null)
            {
                File.WriteAllText(path, Stringify(), Encoding.UTF8);
            }
            else
            {
                File.WriteAllText(path, Stringify(), enc);
            }
        }

        /// <summary>
        /// Create an ASS subtitle object, with a `Default` style and no event lines.
        /// The default resolution set to the subtitle would be 1920 x 1080.
        /// </summary>
        /// <returns></returns>
        public static ASSSubtitle CreateNew()
        {
            var instance = new ASSSubtitle();
            instance.V4pStyleFormat = ASSStyle.DefaultFormat;
            instance.EventFormat = ASSEvent.DefaultFormat;
            instance.ScriptInfo = new Dictionary<string, ASSScriptInfo>();
            instance.ScriptInfo.Add("PlayResX", new ASSScriptInfo { Type = ScriptInfoType.KeyValue, Key = "PlayResX", Value = "1920" });
            instance.ScriptInfo.Add("PlayResY", new ASSScriptInfo { Type = ScriptInfoType.KeyValue, Key = "PlayResY", Value = "1080" });
            instance.ScriptInfo.Add("YCbCr Matrix", new ASSScriptInfo { Type = ScriptInfoType.KeyValue, Key = "YCbCr Matrix", Value = "TV.709" });
            instance.V4pStyles = new Dictionary<string, ASSStyle>();
            instance.V4pStyles.Add("Default", new ASSLoader.NET.ASSStyle
            {
                Name = "Default",
                Fontname = "Arial",
                Fontsize = 45d,
                PrimaryColour = "&H00FFFFFF",
                SecondaryColour = "&H0000FFFF",
                OutlineColour = "&H000000FF",
                BackColour = "&H000000FF",
                Bold = false,
                Italic = false,
                Underline = false,
                StrikeOut = false,
                ScaleX = 100,
                ScaleY = 100,
                Spacing = 0,
                Angle = 0,
                BorderStyle = V4pStyleBorderStyle.ColorBackground,
                Outline = 2,
                Shadow = 2,
                Alignment = V4pStyleAlignment.LeftBottom,
                MarginL = 0,
                MarginR = 0,
                MarginV = 0,
                Encoding = 1
            });
            instance.Events = new List<ASSEvent>();
            instance.UnknownSections = new Dictionary<string, string>();
            return instance;
        }
    } // class ASSSubtitle

}
