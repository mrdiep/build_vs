using System;
using System.IO;
using System.Linq;

namespace VSSolutionBuilder
{
    internal class Program
    {
        private static void Main(string[] args) => new SlnBuilder().Build(
            args
            .Where(x=>x.EndsWith("sln", StringComparison.Ordinal))
            .Select(x=>x.StartsWith(@".\")? Path.Combine(Environment.CurrentDirectory, x) : x)
            .ToArray());
    }
}