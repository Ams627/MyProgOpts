using System;
using System.Collections.Generic;

namespace MyProgOpts
{
    class Option
    {
        public string LongName { get; set; }
        public char ShortName { get; set; }
        public Type Type { get; set; }

        public int NumOptionParams { get; set; }

        public bool Found { get; set; } = false;

        public List<string> OptionParams { get; set; }
        public Option(string longname, char shortname, Type type, int optionParams)
        {
            LongName = longname;
            ShortName = shortname;
            Type = type;
            NumOptionParams = optionParams;
            OptionParams = new List<string>();
        }
    }
}
