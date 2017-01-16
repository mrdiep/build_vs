using System;
using System.Linq;

namespace VSSolutionBuilder
{
    internal class Program
    {
        private static void Main(string[] args) => new SlnBuilder().Build(args.Where(x=>x.EndsWith("sln", StringComparison.Ordinal)).ToArray());
    }
}