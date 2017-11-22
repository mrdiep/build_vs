using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace VSSolutionBuilder
{
    public class SlnBuilder
    {
        public void Build(string[] files)
        {
            if (!files.Any())
                return;

            var hostDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            if (hostDirectory == null)
                return;

            var tempPath = Path.GetTempPath();
            var templateBatText = File.ReadAllText(Path.Combine(hostDirectory, @"template\Template.bat"));
            var nugetRestoreBatText = File.ReadAllText(Path.Combine(hostDirectory, @"template\RestoreNuget.bat"));
            nugetRestoreBatText = nugetRestoreBatText.Replace("__{1}", new FileInfo(Path.Combine(hostDirectory, @"template\nuget.exe")).FullName);

            var builder = new StringBuilder();
            builder.AppendLine("ECHO OFF");
            builder.AppendLine("cd " + $@"""{tempPath}""");

            foreach (var file in files)
            {
                builder.AppendLine();
                var fileName = Path.GetFileNameWithoutExtension(file);

                var innerNugetRestoreBatText = nugetRestoreBatText.Replace("__{0}", fileName);
                innerNugetRestoreBatText = innerNugetRestoreBatText.Replace("__{2}", file);
                builder.AppendLine(innerNugetRestoreBatText);
                builder.AppendLine();

                var innerTemplateBatText = templateBatText.Replace("__{0}", fileName);
                innerTemplateBatText = innerTemplateBatText.Replace("__{1}", file);
                innerTemplateBatText = innerTemplateBatText.Replace("__{2}", Path.Combine(tempPath, @"Logs\Build_" + fileName + "_LOG.log"));
                builder.AppendLine(innerTemplateBatText);
                builder.AppendLine();
            }

            builder.AppendLine("pause");
            var fileBatFillName = "build_sln_" + Guid.NewGuid().ToString("N").ToLower() + ".bat";
            var fullFilebat = Path.Combine(tempPath, fileBatFillName);
            File.WriteAllText(fullFilebat, builder.ToString());
            if (!File.Exists(Path.Combine(tempPath, "ColorText.bat")))
            {
                File.Copy(Path.Combine(hostDirectory, @"template\ColorText.bat"), Path.Combine(tempPath, "ColorText.bat"));
            }

            if (!Directory.Exists(Path.Combine(tempPath, "Logs")))
            {
                Directory.CreateDirectory(Path.Combine(tempPath, "Logs"));
            }

            Process.Start(fullFilebat);
        }
    }
}