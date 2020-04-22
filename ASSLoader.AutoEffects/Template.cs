using ASSLoader.NET;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ASSLoader.AutoEffects
{
    public class Template
    {
        public IList<TemplateSection> Sections { get; set; }

        public IList<ASSEvent> Run(IList<ASSEvent> evts)
        {
            var list = new List<ASSEvent>();
            foreach(var e in evts)
            {
                foreach(var s in Sections)
                {
                    list.AddRange(s.Run(e));
                }
            }
            return list;
        }

        public void Load(string template)
        {
            Sections = new List<TemplateSection>();
            TemplateSection lastSection = null;
            foreach (var line in template.Split('\r', '\n'))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var l = line.Trim();
                if (l.StartsWith("IF "))
                {
                    // Conditions
                    var syntax = l.Substring(3);
                    var regCond = new Regex(@"^([a-zA-Z]+)([=^])(.*)$");
                    var match = regCond.Match(syntax);
                    if (match.Success)
                    {
                        if(lastSection != null)
                        {
                            Sections.Add(lastSection);
                        }
                        var chrOprt = match.Groups[2].Value[0];
                        var oprt = chrOprt == '=' ? TemplateOperator.Equals :
                                   chrOprt == '^' ? TemplateOperator.Contains :
                                   TemplateOperator.Equals;
                        lastSection = new TemplateSection();
                        lastSection.Condition = new TemplateCondition(match.Groups[1].Value, oprt, match.Groups[3].Value);
                        lastSection.Actions = new List<TemplateAction>();
                    }
                }
                if(l.StartsWith("Mod ") || l.Equals("Cop") || l.StartsWith("New "))
                {
                    var cmd = l.Substring(0, 3);
                    string syntax;
                    TemplateAction ac = null;
                    switch (cmd)
                    {
                        case "Mod":
                            syntax = l.Substring(3);
                            ac = new TemplateAction() { ActionType = TemplateActionType.Modify, Parameters = TemplateActionParameter.Parse(syntax) };

                            break;

                        case "Cop":
                            lastSection.Actions.Add(new TemplateAction() { ActionType = TemplateActionType.Copy });
                            break;

                        case "New":
                            syntax = l.Substring(3);
                            ac = new TemplateAction() { ActionType = TemplateActionType.New, Parameters = TemplateActionParameter.Parse(syntax) };
                            break;
                    }
                    if (ac != null)
                    {
                        lastSection.Actions.Add(ac);
                    }
                }
            }
            if(lastSection != null)
            {
                Sections.Add(lastSection);
            }
        }
    }
}
