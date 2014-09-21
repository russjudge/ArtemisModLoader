using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.IO;
using System.Diagnostics;
using System.Security.Permissions;
using System.Security;

namespace RussLibrary.Helpers
{

    public static class ProcessHelper
    {
        //static readonly ILog _log = LogManager.GetLogger(typeof(ProcessHelper));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }

        /// <summary>
        /// Starts a process, executing filename.  Standard output and error are redirected to the passed delegates.
        /// </summary>
        /// <param name="fileName">The filename.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="input">The input.</param>
        /// <param name="outputDelegate">The output delegate.</param>
        /// <param name="errorDelegate">The error delegate.</param>

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public static Process Start(string fileName, string arguments,
            string input, Action<string> outputDelegate, Action<string> errorDelegate)
        {
            ProcessStartInfo start =
                new ProcessStartInfo(fileName, arguments);


            if (!string.IsNullOrEmpty(input))
            {
                start.RedirectStandardInput = true;
            }
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;

            start.WindowStyle = ProcessWindowStyle.Hidden;
            start.CreateNoWindow = true;
            Process p = null;
            Process retVal = null;
            try
            {
                p = new Process();
                start.UseShellExecute = false;
                p.StartInfo = start;

                p.Start();

                ProcessStruct proc = new ProcessStruct();
                proc.Reader = p.StandardOutput;
                proc.ProcessDelegate = outputDelegate;

                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ProcessOutputReader), proc);


                ProcessStruct proc2 = new ProcessStruct();
                proc2.Reader = p.StandardError;
                proc2.ProcessDelegate = errorDelegate;

                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ProcessOutputReader), proc2);


                if (!string.IsNullOrEmpty(input))
                {
                    p.StandardInput.Write(input);
                    p.StandardInput.Close();
                }
                retVal = p;
                p = null;
            }
            finally
            {
                if (p != null)
                {
                    p.Dispose();
                }
            }
            return retVal;
        }
        public static Process Start(string fileName, string arguments)
        {
            return Start(fileName, arguments, null, null, null);
        }
        public static Process Start(string fileName, string arguments,
            Action<string> outputDelegate, Action<string> errorDelegate)
        {
            return Start(fileName, arguments, null, outputDelegate, errorDelegate);
        }
        public static Process Start(string fileName, string arguments, Action<string> outputDelegate)
        {
            return Start(fileName, arguments, null, outputDelegate, outputDelegate);
        }

        struct ProcessStruct
        {
            public StreamReader Reader { get; set; }
            public Action<string> ProcessDelegate { get; set; }
        }
       
        static void ProcessOutputReader(object state)
        {
            ProcessStruct proc = (ProcessStruct)state;
            string sLine = string.Empty;
            do
            {
                sLine = proc.Reader.ReadLine();
                if (sLine != null && proc.ProcessDelegate != null)
                {
                    proc.ProcessDelegate.Invoke(sLine);
                    
                }
            } while (sLine != null);
        }
        
    }
}
