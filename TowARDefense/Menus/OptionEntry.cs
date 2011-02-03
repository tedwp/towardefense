using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TowARDefense.Menus
{
    class OptionEntry : Entry
    {
        public String text;

        public Dictionary<String,Object> options;
        private Dictionary<String,Object>.KeyCollection keys;
        private int i;

        public OptionEntry(String text_f, bool selected)
            : base(selected)
        {
            text = text_f;
            handler += h;
            options = new Dictionary<string, object>();
            i = 0;
        }

        public void addEntry(String text, Object val)
        {
            options.Add(text, val);
            keys = options.Keys;
        }

        public void h(object sender, EventArgs e)
        {
            i++;
            if (i > keys.Count - 1)
                i = 0;
        }

        public string getValue()
        {
            return keys.ElementAt(i);
        }

        public object getInternalValue()
        {
            return options[keys.ElementAt(i)];
        }
    }
}