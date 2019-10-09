using System;
using System.Collections.Generic;

namespace MyProgOpts
{
    interface ICommandLineParser
    {
        void ParseArgs(IEnumerable<string> args);
        void ParseArgs(params string[] args);

        bool GetOptionValue(string name, out int i);
        bool GetOptionValue(string name, out double d);
        bool GetOptionValue(string name, out string d);

        bool GetOptionValue(string name);

        void AddOptionSpec(string name, char shortName, string longName, Type type);
    }
}
