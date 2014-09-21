using System.Threading;
using DMXCommander.Xml;
using log4net;
using RussLibrary.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml;
using DMXCommander.Controls;

namespace DMXCommander.Engine
{
    public class ScriptEngine
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(ScriptEngine));
        public event EventHandler LogEvent;
        public event EventHandler RunComplete;
        private void LogData(string message)
        {

            if (LogEvent != null)
            {
                if (Application.Current.Dispatcher != System.Windows.Threading.Dispatcher.CurrentDispatcher)
                {

                    Application.Current.Dispatcher.Invoke(new Action<string>(LogData), message);
                }
                else
                {
                    LogEvent(message, EventArgs.Empty);
                }
            }
        }

        string _filename = null;
        public string Filename
        {
            get
            {
                return _filename;
            }
            set
            {
                if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
                _filename = value;
                if (!string.IsNullOrEmpty(value) && File.Exists(value))
                {
                    using (StreamReader sr = new StreamReader(value))
                    {

                        ScriptEngine.Current.BaseScript = sr.ReadToEnd();

                    }

                }
                if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            }
        }
        void SetRunWorkPath()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            RunWorkPath = new FileInfo(Filename).DirectoryName;
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        public static ScriptEngine Current { get; private set; }


        private int GetCommentPosition(string line)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            int pos = -1;
            int i = -1;
            bool inQuote = false;
            while (++i < line.Length)
            {
                if (line[i] == '"')
                {
                    inQuote = !inQuote;
                }
                if (!inQuote)
                {
                    if (line[i] == ';')
                    {
                        pos = i;
                        break;
                    }
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return pos;
        }
        private string LineWithoutComment(string line)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            string retVal = line.Replace("\t", "  ");
            int pos = GetCommentPosition(retVal);
            if (pos > -1)
            {
                retVal = retVal.Substring(0, pos).Trim();
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        private KeyValuePair<string, string> ExtractCommandFromLine(string line)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            string wrk = LineWithoutComment(line);
            KeyValuePair<string, string> retVal;
            int i = wrk.IndexOf(' ');
            if (i > 0)
            {

                string parm = wrk.Substring(i + 1).Trim();
                if (parm.Length > 1 && parm[0] == '"' && parm[parm.Length - 1] == '"')
                {
                    parm = parm.Substring(1, parm.Length - 2);
                }
                retVal = new KeyValuePair<string, string>(wrk.Substring(0, i), parm);
            }
            else
            {
                retVal = new KeyValuePair<string, string>(wrk.Trim(), string.Empty);
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }

        DMXCommandFile baseDataFile = null;
        public DMXCommandFile BaseDataFile
        {
            get
            {
                return baseDataFile;
            }
            set
            {
                if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
                baseDataFile = value;

                if (baseDataFile != null)
                {
                    workingCommandFile = new DMXCommandFile();
                    foreach (EventObject ev in baseDataFile.Events)
                    {
                        workingCommandFile.Events.Add(ev);
                    }
                }
                if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }

            }
        }

        DMXCommandFile workingCommandFile = null;

        private ScriptEngine()
        {
        }
        static ScriptEngine()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            Current = new ScriptEngine();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        public string BaseScript { get; set; }
        private StringBuilder workingScript = null;

        bool ValidationPerformed = false;
        Dictionary<string, List<EventObject>> EventItems = null;
        void ActivateEvent(string cue)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            foreach (EventObject ev in EventItems[cue.ToUpperInvariant()])
            {
                Controller.Current.ActivateEvent(ev);
                CueEvents.Add(ev);
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        void DeactivateEvent(string cue)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            foreach (EventObject ev in EventItems[cue.ToUpperInvariant()])
            {
                Controller.Current.DeactivateEvent(ev);
                if (CueEvents.Contains(ev))
                {
                    CueEvents.Remove(ev);
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        public void AbortRun()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            abort = true;
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        bool abort = false;
        ImageWindow ImgWindow = null;
        List<EventObject> CueEvents = null;
        void ProcessRun(string[] lines)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            int currentLine = 0;
            try
            {

                //";CurrentLine: 0"
                if (!int.TryParse(lines[0].Substring(14), out currentLine))
                {
                    throw new InvalidOperationException("First line of process run does not identify line number of run.");
                }
                bool firstLineSkipped = false;
                string f = null;
                int RepeatDepth = 0;
                int repeatCount = 0;
                List<string> LoopList = null;
                foreach (string line in lines)
                {
                    if (!firstLineSkipped)
                    {
                        firstLineSkipped = true;
                        continue;
                    }
                    currentLine++;
                    if (abort)
                    {
                        return;
                    }
                    KeyValuePair<string, string> wrk = ExtractCommandFromLine(line);
                    if (LoopList != null)
                    {
                        if (wrk.Key.ToUpperInvariant() == "REPEAT_END")
                        {
                            if (_log.IsInfoEnabled)
                            {
                                _log.Info("REPEAT_END found.");
                            }
                            LogData(line);
                            RepeatDepth--;
                            if (RepeatDepth < 0)
                            {
                                if (repeatCount == 0)
                                {
                                    do
                                    {

                                        ProcessRun(LoopList.ToArray());
                                    } while (!abort);
                                }
                                else
                                {
                                    for (int j = 0; j < repeatCount; j++)
                                    {
                                        if (!abort)
                                        {
                                            ProcessRun(LoopList.ToArray());
                                        }
                                    }
                                }

                                LoopList = null;
                            }
                            else
                            {
                                LoopList.Add(line);
                            }

                        }
                        else if (wrk.Key.ToUpperInvariant() == "REPEAT_BEGIN")
                        {
                            if (_log.IsInfoEnabled)
                            {
                                _log.Info("REPEAT_BEGIN found.");
                            }
                            LoopList.Add(line);
                            RepeatDepth++;
                        }
                        else
                        {
                            LoopList.Add(line);
                        }
                    }
                    else
                    {
                        if (_log.IsInfoEnabled)
                        {
                            _log.InfoFormat("Processing: {0}.", wrk.Key.ToUpperInvariant());
                        }
                        switch (wrk.Key.ToUpperInvariant())
                        {
                            //Commands that don't need any other work:
                            case "":
                            case "APPEND_DMXCOMMANDFILE":
                            case "DMXCOMMAND_BEGIN":
                            case "DMXCOMMAND_END":
                            case "INCLUDE":
                            case ";":
                                
                                break;

                            case "CUE_START":
                                if (_log.IsInfoEnabled)
                                {
                                    _log.InfoFormat("CUE_START {0}", wrk.Value);
                                }
                                LogData(line);
                                RussLibrary.Helpers.GeneralHelper.DoInvoke(new Action<string>(ActivateEvent), wrk.Value);
                                
                                break;
                            case "CUE_END":
                                if (_log.IsInfoEnabled)
                                {
                                    _log.InfoFormat("CUE_END {0}", wrk.Value);
                                }
                                LogData(line);
                                RussLibrary.Helpers.GeneralHelper.DoInvoke(new Action<string>(DeactivateEvent), wrk.Value);
                                
                                break;


                            case "WAIT":
                                int i = 0;
                                if (_log.IsInfoEnabled)
                                {
                                    _log.InfoFormat("WAIT {0}", wrk.Value);
                                }
                                if (int.TryParse(wrk.Value, out i))
                                {
                                    LogData(line);
                                    RussLibrary.Helpers.GeneralHelper.DoInvoke(new Action<int>(System.Threading.Thread.Sleep), i);
                                    
                                }


                                break;


                            case "SET_WORKPATH":
                                if (_log.IsInfoEnabled)
                                {
                                    _log.InfoFormat("SET_WORKPATH {0}", wrk.Value);
                                }
                                if (!string.IsNullOrEmpty(wrk.Value))
                                {
                                    if (System.IO.Directory.Exists(wrk.Value))
                                    {
                                        LogData(line);
                                        RunWorkPath = wrk.Value;
                                    }

                                }

                                break;


                            case "CALL":
                                if (_log.IsInfoEnabled)
                                {
                                    _log.InfoFormat("CALL {0}", wrk.Value);
                                }
                                List<string> newList = new List<string>(Subroutines[wrk.Value.ToUpperInvariant()]);
                                newList.Insert(0, ";CurrentLine: " + (currentLine - 1).ToString());
                                LogData(line);
                                ProcessRun(newList.ToArray());
                                break;

                            case "END":
                                  if (_log.IsInfoEnabled)
                                {
                                    _log.InfoFormat("{1} {0}", wrk.Value, wrk.Key);
                                }
                                LogData(line);
                                return;
                            case "RETURN":   //marks the end of a labeled routine--only at runtime can issues be found.
                                if (_log.IsInfoEnabled)
                                {
                                    _log.InfoFormat("{1} {0}", wrk.Value, wrk.Key);
                                }
                                LogData(line);
                                return;


                            case "REPEAT_BEGIN":
                                if (_log.IsInfoEnabled)
                                {
                                    _log.InfoFormat("REPEAT_BEGIN {0}", wrk.Value);
                                }
                                LoopList = new List<string>();
                                LoopList.Add(";CurrentLine: " + currentLine.ToString());
                                //Build list to REPEAT_END and recursive call
                                int.TryParse(wrk.Value, out repeatCount);
                                LogData(line);
                                break;

                            case "REPEAT_END":
                                LogData(line);

                                break;

                            //Commands where the parameter must be an existing file:
                            case "AUDIO_QUEUE":
                                f = GetFilename(wrk.Value);
                                if (!string.IsNullOrEmpty(f))
                                {
                                    LogData(line);
                                    RussLibraryAudio.AudioServer.Current.Enqueue(f);
                                    if (!RussLibraryAudio.AudioServer.Current.IsPlaying)
                                    {
                                        RussLibrary.Helpers.GeneralHelper.DoInvoke(new Action(RussLibraryAudio.AudioServer.Current.PlayNextInQueue));
                                        
                                    }
                                }

                                break;
                            case "AUDIO_START":
                                f = GetFilename(wrk.Value);
                                if (!string.IsNullOrEmpty(f))
                                {
                                    LogData(line);
                                    if (RussLibraryAudio.AudioServer.Current.IsPlaying)
                                    {
                                        RussLibrary.Helpers.GeneralHelper.DoInvoke(new Action(RussLibraryAudio.AudioServer.Current.Stop));
                                        
                                    }
                                    RussLibrary.Helpers.GeneralHelper.DoInvoke(new Action<string>(RussLibraryAudio.AudioServer.Current.PlayAsync), f);
                                    
                                }

                                break;
                            case "AUDIO_NEXTFILE":
                                f = GetFilename(wrk.Value);
                                if (!string.IsNullOrEmpty(f))
                                {
                                    LogData(line);
                                    if (RussLibraryAudio.AudioServer.Current.IsPlaying)
                                    {
                                        RussLibraryAudio.AudioServer.Current.Stop();
                                    }
                                    
                                    RussLibrary.Helpers.GeneralHelper.DoInvoke(new Action(RussLibraryAudio.AudioServer.Current.PlayNextInQueue));
                                    
                                }
                                break;

                            case "AUDIO_STOP":  //Stops file and flushes from memory.
                                if (RussLibraryAudio.AudioServer.Current.IsPlaying)
                                {
                                    LogData(line);
                                    RussLibrary.Helpers.GeneralHelper.DoInvoke(new Action(RussLibraryAudio.AudioServer.Current.Stop));
                                    
                                }
                                break;
                            //Must have a file  started for playing: 
                            case "AUDIO_PAUSE":  //Pauses file, can be resumed by PLAY_RESUME.
                                if (RussLibraryAudio.AudioServer.Current.IsPlaying)
                                {
                                    LogData(line);
                                    RussLibrary.Helpers.GeneralHelper.DoInvoke(new Action(RussLibraryAudio.AudioServer.Current.Pause));
                                    
                                }
                                break;

                            //Must have file  started for playing, but paused.
                            case "AUDIO_RESUME":
                                LogData(line);
                                RussLibrary.Helpers.GeneralHelper.DoInvoke(new Action(RussLibraryAudio.AudioServer.Current.Resume));
                                break;

                            //case "EXECUTE_FILE":
                            //    LogData(line);
                            //    //RussLibrary.Helpers.GeneralHelper.DoInvoke(new Func<string, System.Diagnostics.Process>(System.Diagnostics.Process.Start), wrk.Value);
                            //    System.Diagnostics.Process.Start(wrk.Value);
                                
                            //    break;
                            case "IMAGE_DISPLAY":
                                LogData(line);
                                DisplayImage(wrk.Value);

                                break;




                            default:

                                LogData(line);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("[Line: " + currentLine.ToString() + "]::Error: " + ex.Message, ex);
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        void SetImgWindow()
        {
            ImgWindow = new ImageWindow();
            ImgWindow.Abort += ImgWindow_Abort;
            ImgWindow.PauseResume += ImgWindow_PauseResume;
            ImgWindow.Show();
            //ImgWindow.WindowState = System.Windows.WindowState.Maximized;
            //ImgWindow.WindowStyle = System.Windows.WindowStyle.None;

        }

        void ImgWindow_PauseResume(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        void ImgWindow_Abort(object sender, EventArgs e)
        {
            abort = true;
            //RunCleanup(null);
        }
        void DisplayImage(string path)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            string f = GetFilename(path);
            if (!string.IsNullOrEmpty(f))
            {
                try
                {
                    //BitmapImage bmp = new BitmapImage();
                    //bmp.BeginInit();
                    //bmp.UriSource = new Uri(f);
                    //bmp.EndInit();
                    if (ImgWindow == null)
                    {
                        Application.Current.Dispatcher.Invoke(new Action(SetImgWindow));
                        

                        //ImageWindow.Topmost = true;
                        
                    }
                    ImgWindow.SetImage(f);
                    //ImgWindow.Show();
                }
                catch { }

            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        string GetFilename(string path)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            string f = path;
            if (!path.Contains(':') && !path.StartsWith("\\"))
            {

                f = System.IO.Path.Combine(RunWorkPath, f);
            }

            if (!System.IO.File.Exists(f))
            {

                BuildErrorMessage(0, "File does not exist: " + f, false, 0);
            }
            else
            {
                //f = null;
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return f;

        }
        string RunWorkPath = null;
        public bool Run(string scriptFile)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retVal = false;
            Filename = scriptFile;
            retVal = Run();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        
        public bool Run()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            IsRunning = true;
            if (BaseDataFile == null)
            {
                BaseDataFile = new DMXCommandFile();
            }
            SetRunWorkPath();
            abort = false;
            CueEvents = new List<EventObject>();
            bool retVal = ValidateScript();


            if (retVal)
            {

                EventItems = new Dictionary<string, List<EventObject>>();
                foreach (EventObject ev in workingCommandFile.Events)
                {
                    string eventName = ev.EventType.ToUpperInvariant();
                    if (!EventItems.ContainsKey(eventName))
                    {
                        EventItems.Add(eventName, new List<EventObject>());
                    }
                    EventItems[eventName].Add(ev);
                }
                

                try
                {

                    LogData("Running...");
                    ResultListing.Insert(0, ";CurrentLine: 0\r\n");
                    
                    System.Threading.ThreadPool.QueueUserWorkItem(
                        new WaitCallback(ThreadRun),
                        ResultListing.ToString().Replace("\n", string.Empty).Split('\r'));

                    

                }
                catch (Exception ex)
                {

                    ErrorListingBuilder.AppendLine("Exception during runtime: " + ex.Message);
                    currentValidationStatus = false;
                }




            }
            System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(RunCleanup));
                
           
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return currentValidationStatus;
        }
        void ResetImgWindow()
        {
            ImgWindow = null;
        }
        private void RunCleanup(object state)
        {
            System.Threading.Thread.Sleep(1000);
            lock (LockObject)
            {
                if (ImgWindow != null)
                {
                    Application.Current.Dispatcher.Invoke(new Action(ImgWindow.Close));
                    Application.Current.Dispatcher.Invoke(new Action(ResetImgWindow));
                }
                foreach (EventObject ev in CueEvents)
                {
                    Controller.Current.DeactivateEvent(ev);
                }

                RussLibrary.Helpers.GeneralHelper.DoInvoke(new Action(RussLibraryAudio.AudioServer.Current.Stop));

                CueEvents = null;
                if (ErrorListingBuilder != null && ErrorListingBuilder.Length > 0)
                {
                    LogData(ErrorListingBuilder.ToString());
                }
                IsRunning = false;
                RaiseRunComplete();
            }
        }

        private void RaiseRunComplete()
        {
            


            if (Application.Current.Dispatcher != System.Windows.Threading.Dispatcher.CurrentDispatcher)
            {

                Application.Current.Dispatcher.Invoke(new Action(RaiseRunComplete));
            }
            else
            {
                LogData("Run Complete.");
                if (RunComplete != null)
                {
                    RunComplete(this, EventArgs.Empty);

                }
            }
        }

        private object LockObject = new object();
        public bool IsRunning { get; private set; }

        void ThreadRun(object state)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            lock (LockObject)
            {
                string[] lines = state as string[];
                if (lines != null)
                {
                    ProcessRun(lines);
                }
                
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        void TestForChange()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }

            if (ValidationPerformed)
            {
                ValidationPerformed = false;
                //TODO: Find a way to check for changes.
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }

        }
        /// <summary>
        /// Validates the script.
        /// </summary>
        /// <returns></returns>
        public bool ValidateScript()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            TestForChange();
            if (!ValidationPerformed)
            {
                LogData("Validating Script");
                ErrorListingBuilder = new StringBuilder();
                currentValidationStatus = true;
                workingScript = new StringBuilder();
                FirstPassFileLineStack = new Stack<string>();
                string[] baseScript = BaseScript.Replace("\n", string.Empty).Split('\r');

                //Pass 1: import all includes.  (no includes in resulting "workingScript"),
                //      import all DMX data files. (all APPEND and DMX_COMMANDS removed).
                //      all comments also removed.

                CurrentFile = Filename;

                FirstPassFileLineStack.Push(CurrentFile);
                workingScript.AppendLine(";" + CurrentFile);
                Labels = new Dictionary<string, int>();
                workFileLine = 0;
                ProcessFirstPass(baseScript);

                //Pass 2: validate syntax
                if (currentValidationStatus)
                {
                    ResultListing = new StringBuilder();
                    ProcessSecondPass();
                }
                else
                {
                    ResultListing = new StringBuilder(workingScript.ToString());
                    workingScript = null;
                }
            }
            ValidationPerformed = true;
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return currentValidationStatus;
        }
        StringBuilder ErrorListingBuilder = null;
        public string ErrorListing
        {
            get
            {
                return ErrorListingBuilder.ToString();
            }
        }


        StringBuilder ResultListing = null;
        public string ResultScriptListing
        {
            get
            {
                if (ResultListing != null)
                {

                    return ResultListing.ToString();
                }
                else
                {
                    return null;
                }
            }
        }


        Dictionary<string, List<string>> Subroutines = null;

        /// <summary>
        /// Processes the second pass.
        /// </summary>
        void ProcessSecondPass()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            Subroutines = new Dictionary<string, List<string>>();
            List<string> CurrentRoutine = new List<string>();
            Subroutines.Add(string.Empty, CurrentRoutine);
            List<string> CueList = new List<string>();
            bool AudioStarted = false;
            bool AudioPaused = false;
            bool RepeatBeginStarted = false;
            int AudioQueueCount = 0;
            int callStackCount = 0;

            foreach (EventObject ev in workingCommandFile.Events)
            {
                if (!CueList.Contains(ev.EventType))
                {
                    CueList.Add(ev.EventType);
                }
            }


            string workPath = RunWorkPath;

            List<string> startedCues = new List<string>();
            Stack<int> lineNumberStack = new Stack<int>();


            int wrkLineNumber = -1;
            string[] workLines = workingScript.ToString().Replace("\n", string.Empty).Split('\r');
            foreach (string line in workLines)
            {
                if (line.StartsWith(";\x0001"))
                {

                    if (line.StartsWith(";\x0001("))
                    {
                        //workingScript.AppendLine(";\x0001(" + thisLine.ToString() + "):" + wrk.Value);
                        lineNumberStack.Push(wrkLineNumber);

                        wrkLineNumber = -1;

                    }
                    else if (line.StartsWith(";\x0001-"))
                    {
                        //workingScript.AppendLine(";\x0001-pop");
                        wrkLineNumber = lineNumberStack.Pop();

                    }

                }
                else
                {
                    ResultListing.AppendLine(line.Replace("\x0002", string.Empty));
                    CurrentRoutine.Add(line);
                    if (!line.Contains("\x0002"))
                    {
                        wrkLineNumber++;
                    }
                    KeyValuePair<string, string> wrk = ExtractCommandFromLine(line);
                    switch (wrk.Key.ToUpperInvariant())
                    {
                        case "END":
                            callStackCount--;
                            break;
                        case "RETURN":   //marks the end of a labeled routine--only at runtime can issues be found.

                            if (callStackCount < 0)
                            {
                                ResultListing.AppendLine(BuildErrorMessage(wrkLineNumber, "RETURN without routine label--unreachable code", false, lineNumberStack.Count));
                            }
                            else
                            {
                                callStackCount--;
                                CurrentRoutine = Subroutines[string.Empty];
                            }
                            break;
                        //Commands that don't need any other work:
                        case "APPEND_DMXCOMMANDFILE":
                        case "DMXCOMMAND_BEGIN":
                        case "DMXCOMMAND_END":
                        case "INCLUDE":
                            break;
                        //case "EXECUTE_FILE":
                        //    //System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(new Func<string, System.Diagnostics.Process>(System.Diagnostics.Process.Start),
                        //    //        wrk.Value);
                        //    System.Diagnostics.Process.Start(wrk.Value);
                        //    break;
                        case "CUE_START":
                            //Cue must be in Cue list, list of events in DMX Command data that is loaded.
                            if (!CueList.Contains(wrk.Value.ToUpperInvariant()))
                            {
                                ResultListing.AppendLine(BuildErrorMessage(wrkLineNumber, "Cue Not Found in DMX Command data: " + wrk.Value.ToUpperInvariant(), false, lineNumberStack.Count));

                            }
                            else
                            {
                                if (startedCues.Contains(wrk.Value.ToUpperInvariant()))
                                {
                                    ResultListing.AppendLine(
                                        BuildWarningMessage(wrkLineNumber, "Cue Start already set: " + wrk.Value.ToUpperInvariant(), lineNumberStack.Count));
                                }
                                else
                                {
                                    startedCues.Add(wrk.Value.ToUpperInvariant());
                                }
                            }
                            break;
                        case "CUE_END":
                            //names a cue that must have been started.
                            if (!startedCues.Contains(wrk.Value.ToUpperInvariant()))
                            {
                                ResultListing.AppendLine(BuildErrorMessage(wrkLineNumber, "Cue not started: " + wrk.Value.ToUpperInvariant(), false, lineNumberStack.Count));
                            }
                            break;

                        //Commands where the parameter must be an existing file:
                        case "AUDIO_QUEUE":
                        case "AUDIO_START":
                        case "IMAGE_DISPLAY":
                            if (!string.IsNullOrEmpty(wrk.Value))
                            {
                                string f = wrk.Value;
                                if (!wrk.Value.Contains(':') && !wrk.Value.StartsWith("\\"))
                                {

                                    f = System.IO.Path.Combine(workPath, f);
                                }

                                if (!System.IO.File.Exists(f))
                                {
                                    ResultListing.AppendLine(BuildErrorMessage(wrkLineNumber, "File does not exist: " + f, false, lineNumberStack.Count));
                                }
                                else
                                {
                                    if (wrk.Key.ToUpperInvariant().StartsWith("AUDIO_"))
                                    {
                                        AudioStarted = true;
                                        if (wrk.Key.ToUpperInvariant() == "AUDIO_QUEUE")
                                        {
                                            AudioQueueCount++;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ResultListing.AppendLine(BuildErrorMessage(wrkLineNumber, "File required", false, lineNumberStack.Count));
                            }

                            break;

                        //Must have a file started for playing:
                        case "AUDIO_STOP":  //Stops file and flushes from memory.
                            if (!AudioStarted)
                            {
                                ResultListing.AppendLine(BuildErrorMessage(wrkLineNumber, "No audio has been started", false, lineNumberStack.Count));
                            }
                            else
                            {
                                AudioStarted = false;
                                AudioPaused = false;
                            }
                            break;
                        //Must have a file  started for playing: 
                        case "AUDIO_PAUSE":  //Pauses file, can be resumed by PLAY_RESUME.
                            if (!AudioStarted)
                            {
                                ResultListing.AppendLine(BuildErrorMessage(wrkLineNumber, "No audio has been started", false, lineNumberStack.Count));
                            }
                            AudioPaused = true;
                            break;

                        //Must have file  started for playing, but paused.
                        case "AUDIO_RESUME":
                            if (!AudioStarted && !AudioPaused)
                            {
                                ResultListing.AppendLine(BuildErrorMessage(wrkLineNumber, "No audio has been started or paused", false, lineNumberStack.Count));
                            }
                            else
                            {
                                AudioPaused = false;
                            }
                            break;
                        //Must have at least one file in queue with PLAY_QUEUE
                        case "AUDIO_NEXTFILE":
                            if (AudioQueueCount == 0)
                            {
                                ResultListing.AppendLine(BuildWarningMessage(wrkLineNumber, "No audio file queued", lineNumberStack.Count));
                            }
                            break;

                        //Must have a parm with a number.  is in milliseconds.
                        case "WAIT":
                            int i = 0;
                            if (string.IsNullOrEmpty(wrk.Value))
                            {
                                ResultListing.AppendLine(BuildErrorMessage(wrkLineNumber, "Wait period required", false, lineNumberStack.Count));
                            }
                            else
                            {
                                if (!int.TryParse(wrk.Value, out i))
                                {
                                    ResultListing.AppendLine(BuildErrorMessage(wrkLineNumber, "Invalid WAIT parameter: " + wrk.Value, false, lineNumberStack.Count));
                                }
                            }

                            break;

                        //Can have a number for the number of times to repeat.   no number or zero means repeat forever.
                        // MUST have a REPEAT_END.
                        case "REPEAT_BEGIN":
                            int i1 = 0;
                            if (!string.IsNullOrEmpty(wrk.Value))
                            {
                                if (!int.TryParse(wrk.Value, out i1))
                                {
                                    ResultListing.AppendLine(BuildErrorMessage(wrkLineNumber, "Invalid REPEAT_BEGIN parameter: " + wrk.Value, false, lineNumberStack.Count));
                                }
                                else
                                {
                                    RepeatBeginStarted = true;
                                }
                            }

                            break;
                        //Must have a REPEAT_BEGIN"
                        case "REPEAT_END":
                            if (!RepeatBeginStarted)
                            {
                                ResultListing.AppendLine(BuildErrorMessage(wrkLineNumber, "REPEAT_END without REPEAT_BEGIN", false, lineNumberStack.Count));
                            }
                            RepeatBeginStarted = false;

                            break;
                        //Must have a Label
                        case "CALL":
                            if (!Labels.Keys.Contains(wrk.Value.ToUpperInvariant()))
                            {

                                ResultListing.AppendLine(BuildErrorMessage(wrkLineNumber, "Label Not Found: " + wrk.Value.ToUpperInvariant(), false, lineNumberStack.Count));

                            }

                            break;
                        case "SET_WORKPATH":
                            if (!string.IsNullOrEmpty(wrk.Value))
                            {
                                if (System.IO.Directory.Exists(wrk.Value))
                                {
                                    workPath = wrk.Value;
                                }
                                else
                                {
                                    ResultListing.AppendLine(BuildErrorMessage(wrkLineNumber, "Path does not exist: " + wrk.Value, false, lineNumberStack.Count));
                                }
                            }
                            else
                            {
                                ResultListing.AppendLine(BuildErrorMessage(wrkLineNumber, "Path required", false, lineNumberStack.Count));
                            }
                            break;
                        case "":
                            break;
                        default:
                            //is it a label?
                            if (!wrk.Key.StartsWith(":") && !wrk.Key.StartsWith(";"))
                            {


                                ResultListing.AppendLine(BuildErrorMessage(wrkLineNumber, "Invalid Command: " + wrk.Key.ToUpperInvariant(), false, lineNumberStack.Count));

                            }
                            else
                            {
                                if (wrk.Key.StartsWith(":"))
                                {
                                    //@@@

                                    if (callStackCount > 0)
                                    {
                                        ResultListing.AppendLine(BuildErrorMessage(wrkLineNumber, "Invalid second label without RETURN: " + wrk.Key.ToUpperInvariant(), false, lineNumberStack.Count));
                                    }
                                    else
                                    {
                                        CurrentRoutine = new List<string>();
                                        Subroutines.Add(wrk.Key.ToUpperInvariant(), CurrentRoutine);
                                    }
                                    callStackCount++;
                                }
                            }
                            break;
                    }
                }
                //if (callStackCount < 0)
                //{
                //    break;
                //}
            }
            if (RepeatBeginStarted)
            {
                ResultListing.AppendLine(BuildErrorMessage(0, "REPEAT_BEGIN without REPEAT_END", false, lineNumberStack.Count));
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        string BuildErrorMessage(int lineNumber, string message, bool specialCode, int indentLevel)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            string retVal = null;
            currentValidationStatus = false;
            string stackIndent = string.Empty;
            if (indentLevel > 0)
            {
                stackIndent = "".PadLeft(indentLevel, '\t');
            }
            string code = specialCode ? "\x0002" : string.Empty;
            retVal = string.Format("{3};{2}[Line: {0}]::Error: {1}", lineNumber.ToString(), message, code, stackIndent);
            ErrorListingBuilder.AppendLine(retVal);
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;

        }
        string BuildWarningMessage(int lineNumber, string message, int indentLevel)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            string retVal = null;
            string stackIndent = string.Empty;
            if (indentLevel > 0)
            {
                stackIndent = "".PadLeft(indentLevel, '\t');
            }

            retVal = string.Format("{2};[Line: {0}]::Error: {1}", lineNumber.ToString(), message, stackIndent);
            ErrorListingBuilder.AppendLine(retVal);
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;

        }

        string CurrentFile = null;
        bool currentValidationStatus = true;
        Dictionary<string, int> Labels = null;
        Stack<string> FirstPassFileLineStack = null;
        int workFileLine = 0;

        void ProcessFirstPass(string[] lines)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
         
            int thisLine = 0;
            bool IsLoadingDMXData = false;
            StringBuilder DMXData = null;
            foreach (string line in lines)
            {


                
                if (IsLoadingDMXData)
                {
                    if (line.Trim().ToUpperInvariant() == "DMXCOMMAND_END")
                    {
                        IsLoadingDMXData = false;
                        try
                        {
                            workFileLine++;
                            thisLine++;
                            workingScript.AppendLine(line);
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(DMXData.ToString());
                            DMXCommandFile dmx = XmlConverter.ToObject(doc, typeof(DMXCommandFile)) as DMXCommandFile;
                            if (dmx != null)
                            {
                                foreach (EventObject ev in dmx.Events)
                                {
                                    workingCommandFile.Events.Add(ev);
                                }
                            }
                            DMXData = null;
                        }
                        catch (Exception ex)
                        {
                            workFileLine++;
                            thisLine++;
                            workingScript.AppendLine("DMXCOMMAND_BEGIN");
                            workFileLine++;


                            workingScript.AppendLine(BuildErrorMessage(thisLine, "Unable to import DMX data: " + ex.Message, true, FirstPassFileLineStack.Count));
                            workFileLine++;
                            
                            workingScript.AppendLine(DMXData.ToString());
                            workFileLine++;
                            
                            workingScript.AppendLine("DMXCOMMAND_END");
                            
                            
                        }
                    }
                    else
                    {
                        DMXData.AppendLine(line);
                    }
                }
                else
                {
                    workFileLine++;
                    thisLine++;
                    workingScript.AppendLine(line);
                    KeyValuePair<string, string> wrk = ExtractCommandFromLine(line);
                    
                    
                    if (wrk.Key.ToUpperInvariant() == "INCLUDE")
                    {
                        //LogData(line);
                        LogData("Importing file: " + wrk.Value);
                        if (System.IO.File.Exists(wrk.Value))
                        {
                            workingScript.AppendLine(";\x0001(" + thisLine.ToString() + "):" + wrk.Value);

                            using (System.IO.StreamReader sr = new System.IO.StreamReader(wrk.Value))
                            {
                                string[] includefile = sr.ReadToEnd().Replace("\n", string.Empty).Split('\r');
                                CurrentFile = wrk.Value;

                                FirstPassFileLineStack.Push(CurrentFile);
                                
                                ProcessFirstPass(includefile);
                                workFileLine++;
                                workingScript.AppendLine(";\x0002INCLUDE END: " + CurrentFile);
                                workingScript.AppendLine(";\x0001-pop");
                                CurrentFile = FirstPassFileLineStack.Pop();
                            }
                        }
                        else
                        {
                            workFileLine++;
                            
                            workingScript.AppendLine(BuildErrorMessage(thisLine, "File Not Found: " + wrk.Value, true, FirstPassFileLineStack.Count));
                            
                        }
                    }
                    else if (wrk.Key.ToUpperInvariant() == "APPEND_DMXCOMMANDFILE")
                    {
                        
                        if (System.IO.File.Exists(wrk.Value))
                        {
                            try
                            {
                                LogData("Importing DMX file: " + wrk.Value);
                                DMXCommandFile dmx =RussLibrary.Xml.XmlConverter.ToObject(wrk.Value, typeof(DMXCommandFile)) as DMXCommandFile ;
                                if (dmx != null)
                                {
                                    foreach (EventObject ev in dmx.Events)
                                    {
                                        workingCommandFile.Events.Add(ev);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                workFileLine++;
                                workingScript.AppendLine(wrk.Key + " " + wrk.Value);
                                workFileLine++;

                                workingScript.AppendLine(
                                    BuildErrorMessage(thisLine, "Unable to load DMX Command File: " + ex.Message, true, FirstPassFileLineStack.Count)); 
                                
                            }

                        }
                        else
                        {
                            workFileLine++;
                            workingScript.AppendLine(wrk.Key + " " + wrk.Value);
                            workFileLine++;

                            workingScript.AppendLine(
                                    BuildErrorMessage(thisLine, "File Not Found: " + wrk.Value, true, FirstPassFileLineStack.Count)); 
                            
                        }
                    }
                    else if (wrk.Key.ToUpperInvariant() == "DMXCOMMAND_BEGIN")  //Does require a DMXCOMMAND_END--need to validate for that.
                    {
                        LogData("Imorting DMX Data");
                        //Load XML data into DMXCommand Data.
                        IsLoadingDMXData = true;
                        DMXData = new StringBuilder();
                        //need to read ahead to "DMXCOMMAND_END".

                    }
                    else if (wrk.Key.StartsWith(":"))
                    {
                        Labels.Add(wrk.Key.ToUpperInvariant(), workFileLine);
                    }
                   
                    
                }
            }
            if (IsLoadingDMXData)
            {
                workFileLine++;

                workingScript.AppendLine(
                    BuildErrorMessage(thisLine, "DMXCOMMAND_END command not found", true, FirstPassFileLineStack.Count)); 
                
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

    }
}
