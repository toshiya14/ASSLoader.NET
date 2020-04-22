using ASSLoader.NET;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASSLoader.AutoEffects
{
    public class TemplateAction
    {
        public TemplateActionType ActionType { get; set; }
        public List<TemplateActionParameter> Parameters { get; set; }

        public ASSEvent Apply(ASSEvent src)
        {
            if(ActionType == TemplateActionType.Copy)
            {
                return src.Clone() as ASSEvent;
            }
            ASSEvent target;
            if(ActionType == TemplateActionType.New)
            {
                target = new ASSEvent();
            }
            target = src.Clone() as ASSEvent;

            foreach(var p in Parameters)
            {
                var left = p.Left;
                var right = p.Escape(target);
                var prop = target.GetType().GetProperty(left, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
                var value = prop.GetValue(target);
                switch (p.Oprt)
                {
                    case TemplateActionParameterOperator.Assign:
                        if (value.GetType().Name.Equals("string", StringComparison.OrdinalIgnoreCase))
                            prop.SetValue(target, right);
                        if (value.GetType().Name.Equals("ASSEventTime", StringComparison.OrdinalIgnoreCase))
                            prop.SetValue(target, new ASSEventTime(right));
                        if (value.GetType().Name.Equals("int", StringComparison.OrdinalIgnoreCase) ||
                            value.GetType().Name.Equals("Int32", StringComparison.OrdinalIgnoreCase))
                            prop.SetValue(target, Convert.ToInt32(right));
                        break;

                    case TemplateActionParameterOperator.Plus:
                        if (value.GetType().Name.Equals("string", StringComparison.OrdinalIgnoreCase))
                            prop.SetValue(target, prop.GetValue(target).ToString() + right);
                        if (value.GetType().Name.Equals("ASSEventTime", StringComparison.OrdinalIgnoreCase))
                            prop.SetValue(target, (prop.GetValue(target) as ASSEventTime + Convert.ToDouble(right)));
                        if (value.GetType().Name.Equals("int", StringComparison.OrdinalIgnoreCase) ||
                            value.GetType().Name.Equals("Int32", StringComparison.OrdinalIgnoreCase))
                            prop.SetValue(target, Convert.ToInt32(prop.GetValue(target)) + Convert.ToInt32(right));
                        break;

                    case TemplateActionParameterOperator.Minus:
                        if (value.GetType().Name.Equals("string", StringComparison.OrdinalIgnoreCase))
                            prop.SetValue(target, prop.GetValue(target).ToString());
                        if (value.GetType().Name.Equals("ASSEventTime", StringComparison.OrdinalIgnoreCase))
                            prop.SetValue(target, (prop.GetValue(target) as ASSEventTime - Convert.ToDouble(right)));
                        if (value.GetType().Name.Equals("int", StringComparison.OrdinalIgnoreCase) ||
                            value.GetType().Name.Equals("Int32", StringComparison.OrdinalIgnoreCase))
                            prop.SetValue(target, Convert.ToInt32(prop.GetValue(target)) - Convert.ToInt32(right));
                        break;
                }
            }

            return target;
        }
    }

    public enum TemplateActionType
    {
        Modify,
        Copy,
        New
    }
}
