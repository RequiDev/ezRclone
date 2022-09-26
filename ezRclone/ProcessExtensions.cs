using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace ezRclone
{
    internal static class ProcessExtensions
    {
        public static string GetCommandLine(this Process process)
        {
            using var searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id);
            using var objects = searcher.Get();
            return objects.Cast<ManagementBaseObject>().SingleOrDefault()?["CommandLine"]?.ToString() ?? string.Empty;
        }
    }
}
