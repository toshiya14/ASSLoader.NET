using ASSLoader.NET;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASSLoader.AutoEffects
{
    public class TemplateCondition
    {
        public string Left { get; set; }
        public TemplateOperator Oprt { get; set; }
        public string Right { get; set; }

        public TemplateCondition(string left, TemplateOperator oprt, string right)
        {
            this.Left = left;
            this.Oprt = oprt;
            this.Right = right.Replace("\\_", " ");
        }

        public bool Check(ASSEvent evtLine)
        {
            var prop = evtLine.GetType().GetProperty(Left, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
            if(prop != null)
            {
                var value = prop.GetValue(evtLine);
                switch (Oprt)
                {
                    case TemplateOperator.Equals:
                        if (value.GetType().Name.Equals("string", StringComparison.OrdinalIgnoreCase))
                            return value.ToString().Equals(Right, StringComparison.OrdinalIgnoreCase);
                        if (value.GetType().Name.Equals("ASSEventTime", StringComparison.OrdinalIgnoreCase))
                            return value.Equals(Right);
                        if (value.GetType().Name.Equals("int", StringComparison.OrdinalIgnoreCase) ||
                            value.GetType().Name.Equals("Int32", StringComparison.OrdinalIgnoreCase))
                            return value.Equals(Convert.ToInt32(Right));
                        return false;

                    case TemplateOperator.Contains:
                        if (value.GetType().Name.Equals("string", StringComparison.OrdinalIgnoreCase))
                            return value.ToString().Contains(Right);
                        if (value.GetType().Name.Equals("ASSEventTime", StringComparison.OrdinalIgnoreCase))
                            return value.Equals(Right);
                        if (value.GetType().Name.Equals("int", StringComparison.OrdinalIgnoreCase) ||
                            value.GetType().Name.Equals("Int32", StringComparison.OrdinalIgnoreCase))
                            return value.Equals(Convert.ToInt32(Right));
                        return false;
                }
            }
            return false;
        }
    }
}
