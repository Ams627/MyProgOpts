using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MyProgOpts
{
    class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var args1 = new[] { "--name=1", "-pef" };
                var options = new CommandLineParser();
                options.AddOptionSpec("name", 'f', "name");
                options.AddOptionSpec("n", 'n', "name");
                options.AddOptionSpec("e", 'e', "name");
                options.AddOptionSpec("p", 'p', "name");
                options.ParseArgs(args1, out var invalidShort, out var invalidLong);
                Console.WriteLine($"n {options.TestBooleanOption('n')}");
                Console.WriteLine($"e {options.TestBooleanOption('e')}");
                Console.WriteLine($"p {options.TestBooleanOption('p')}");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                var fullname = System.Reflection.Assembly.GetEntryAssembly().Location;
                var progname = Path.GetFileNameWithoutExtension(fullname);
                Console.Error.WriteLine(progname + ": Error: " + ex.Message);
            }

        }
    }
}
