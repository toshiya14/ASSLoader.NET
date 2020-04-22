using ASSLoader.NET;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ASSLoader.AutoEffects
{
    public class TemplateActionParameter
    {
        public string Left { get; set; }

        public TemplateActionParameterOperator Oprt { get; set; }

        public string Right { get; set; }

        public TemplateActionParameter(string left, TemplateActionParameterOperator oprt, string right)
        {
            this.Left = left;
            this.Oprt = oprt;
            this.Right = right;
            this.Right = right.Replace("\\_", " ");
        }

        public static List<TemplateActionParameter> Parse(string syntax)
        {
            var list = new List<TemplateActionParameter>();
            var parts = syntax.Split(' ');
            var regParam = new Regex(@"^(Layer|Start|End|Style|Name|MarginL|MarginR|MarginV|Effect|Text)([=+-])(.*)$");
            foreach(var p in parts)
            {
                var m = regParam.Match(p);
                if (m.Success)
                {
                    var left = m.Groups[1].Value;
                    var oprt = m.Groups[2].Value[0] == '=' ? TemplateActionParameterOperator.Assign :
                               m.Groups[2].Value[0] == '+' ? TemplateActionParameterOperator.Plus :
                               m.Groups[2].Value[0] == '-' ? TemplateActionParameterOperator.Minus :
                               TemplateActionParameterOperator.Assign;
                    var right = m.Groups[3].Value;
                    list.Add(new TemplateActionParameter(left, oprt, right));
                }
            }
            return list;
        }

        public string Escape(ASSEvent evtLine)
        {
            var text = Right
                        .Replace("@Layer", evtLine.Layer.ToString())
                        .Replace("@Start", evtLine.Start.ToString())
                        .Replace("@End", evtLine.End.ToString())
                        .Replace("@Style", evtLine.Style)
                        .Replace("@Name", evtLine.Name)
                        .Replace("@MarginL", evtLine.MarginL.ToString())
                        .Replace("@MarginR", evtLine.MarginR.ToString())
                        .Replace("@MarginV", evtLine.MarginV.ToString())
                        .Replace("@Effect", evtLine.Effect.ToString())
                        .Replace("@Text", evtLine.Text);
            return text;
        }
    }



    public enum TemplateActionParameterOperator
    {
        Assign,
        Plus,
        Minus
    }
}
