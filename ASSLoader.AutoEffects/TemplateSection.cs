using ASSLoader.NET;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASSLoader.AutoEffects
{
    public class TemplateSection
    {
        public TemplateCondition Condition;

        public List<TemplateAction> Actions;

        public IList<ASSEvent> Run(ASSEvent evt)
        {
            var list = new List<ASSEvent>();
            if (Condition.Check(evt))
            {
                foreach(var ac in Actions)
                {
                    list.Add(ac.Apply(evt));
                }
            }
            return list;
        }
    }

    public enum TemplateOperator
    {
        Equals,
        Contains
    }

}
