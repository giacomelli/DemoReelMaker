using System;
using System.Diagnostics;

namespace DemoReelMaker.Proxies
{
    // http://jake.ginnivan.net/redirecting-process-output/
    public static class ProcessExtensions
    {
        public static void ReadOutputToEnd(this Process process, out string standardOutput, out string standardError)
        {
            if (process.StartInfo.UseShellExecute)
                throw new ArgumentException("Set StartInfo.UseShellExecute to false");

            var outputData = new OutputData();
            var errorData = new OutputData();
            
            if (process.StartInfo.RedirectStandardOutput)
            {
                outputData.Stream = process.StandardOutput;
                outputData.Thread = new System.Threading.Thread(ReadToEnd);
                outputData.Thread.Start(outputData);
            }
            if (process.StartInfo.RedirectStandardError)
            {
                errorData.Stream = process.StandardError;
                errorData.Thread = new System.Threading.Thread(ReadToEnd);
                errorData.Thread.Start(errorData);
            }

            if (process.StartInfo.RedirectStandardOutput)
            {
                outputData.Thread.Join();
                standardOutput = outputData.Output;
            }
            else
                standardOutput = string.Empty;

            if (process.StartInfo.RedirectStandardError)
            {
                errorData.Thread.Join();
                standardError = errorData.Output;
            }
            else
                standardError = string.Empty;

            if (outputData.Exception != null)
                throw outputData.Exception;
            if (errorData.Exception != null)
                throw errorData.Exception;
        }

        private class OutputData
        {
            public System.Threading.Thread Thread;
            public System.IO.StreamReader Stream;
            public string Output;
            public Exception Exception;
        }

        private static void ReadToEnd(object data)
        {
            var ioData = (OutputData)data;
            try
            {
                ioData.Output = ioData.Stream.ReadToEnd();
            }
            catch (Exception e)
            {
                ioData.Exception = e;
            }
        }
    }
}
