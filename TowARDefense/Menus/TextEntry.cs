using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TowARDefense.Menus
{
    class TextEntry : Entry
    {
        public String text;

        public TextEntry(String text_f, bool selected)
            : base(selected)
        {
            text = text_f;
        }
    }
}
