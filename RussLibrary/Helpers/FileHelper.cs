using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.IO;
using System.Collections;
using System.Collections.ObjectModel;
using SharpCompress;
using SharpCompress.Writer;
using SharpCompress.Common;
using SharpCompress.Reader;
namespace RussLibrary.Helpers
{

    public static class FileHelper
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(FileHelper));
        //
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        public static bool IsImageFile(string fileName)
        {
            bool retVal = false;
            FileInfo f = new FileInfo(fileName);
            retVal = (NativeMethods.GetPerceivedType(f.Extension).ToUpperInvariant() == "IMAGE");
            if (!retVal)
            {

                //Have experienced cases where Perceived type is not set.
                retVal = (f.Extension.ToUpperInvariant() == "BMP" || f.Extension.ToUpperInvariant() == "PNG"
                    || f.Extension.ToUpperInvariant() == "JPG" || f.Extension.ToUpperInvariant() == "GIF");
            }
            return retVal;
        }
        public static bool IsAudioFile(string fileName)
        {
            bool retVal = false;
            FileInfo f = new FileInfo(fileName);
            retVal = (NativeMethods.GetPerceivedType(f.Extension).ToUpperInvariant() == "AUDIO");
            if (!retVal)
            {

                //Have experienced cases where Perceived type is not set.
                retVal = (f.Extension.ToUpperInvariant() == "WAV");
            }
            return retVal;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "WAV")]
        public static bool IsWAVFile(string fileName)
        {
            bool retVal = false;
            FileInfo f = new FileInfo(fileName);


            //Have experienced cases where Perceived type is not set.
            retVal = (f.Extension.ToUpperInvariant() == "WAV");

            return retVal;
        }
        ////Method below will only work on .zip files, not .rar files.
        ////public static bool IsCompressedFile(string filename)
        ////{
        ////    FileInfo f = new FileInfo(filename);
        ////    return (NativeMethods.GetPerceivedType(f.Extension).ToUpperInvariant() == "COMPRESSED");
        ////}
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static bool IsValidCompressedFile(string path)
        {


            bool retVal = true;
            try
            {
                using (Stream stream = File.OpenRead(path))
                {
                    IReader reader = ReaderFactory.Open(stream);
                    while (reader.MoveToNextEntry())
                    {
                        
                    }
                }
            }
            catch (Exception ex)
            {
#if CAPTUREBADZIP
                string target = Path.Combine(@"C:\Users\rjudge\Google Drive", new FileInfo(path).Name + ".txt"); 
                FileHelper.Copy(path,target);
#endif
                if (_log.IsWarnEnabled)
                {
                    _log.Warn("Exception browsing compressed file", ex);
                }
                retVal = false;
            }
            return retVal;
        }
        public static void CreateZip(string path, string sourceFileFolder, string comment)
        {
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(sourceFileFolder))
            {
                SharpCompress.Common.CompressionInfo ci = new SharpCompress.Common.CompressionInfo();
                ci.DeflateCompressionLevel = SharpCompress.Compressor.Deflate.CompressionLevel.BestCompression;
                using (FileStream sw = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    SharpCompress.Writer.Zip.ZipWriter zw = new SharpCompress.Writer.Zip.ZipWriter(sw,
                        ci, comment);
                    DirectoryInfo dir = new DirectoryInfo(sourceFileFolder);
                    foreach (FileInfo f in dir.GetFiles("*.*", SearchOption.AllDirectories))
                    {
                        using (FileStream fs = f.OpenRead())
                        {
                            string relativePath = string.Empty;
                            if (f.FullName.Length > sourceFileFolder.Length)
                            {
                                relativePath = f.FullName.Substring(sourceFileFolder.Length + 1);
                            }
                            zw.Write(relativePath, fs, f.LastWriteTime);
                        }
                    }

                }
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static void CreateZip(string path, string[] fileList)
        {
            if (fileList != null && !string.IsNullOrEmpty(path))
            {
                using (FileStream zip = File.OpenWrite(path))
                {
                    CompressionInfo ci = new CompressionInfo();
                    ci.DeflateCompressionLevel = SharpCompress.Compressor.Deflate.CompressionLevel.BestCompression;
                    using (IWriter zipWriter = WriterFactory.Open(zip, ArchiveType.Zip, ci))
                    {
                        foreach (string filePath in fileList)
                        {
                            zipWriter.Write(Path.GetFileName(filePath), filePath);
                        }
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static void Copy(string source, string target)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            FileInfo src = new FileInfo(source);
            FileInfo targ = new FileInfo(target);
            if (targ.FullName != src.FullName)
            {
                FileHelper.CreatePath(targ.DirectoryName);

                if (src.Exists)
                {
                    FileHelper.DeleteFile(target);
                    if (_log.IsInfoEnabled)
                    {
                        _log.InfoFormat("Copying \"{0}\" to \"{1}\"", source, target);
                    }
                    try
                    {
                        src.CopyTo(target);
                    }
                    catch (Exception ex)
                    {
                        if (_log.IsWarnEnabled)
                        {
                            _log.Warn("Unable to copy.", ex);
                        }
                    }
                }
                else
                {
                    SetCopyFilesProblem();
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        static void SetCopyFilesProblem()
        {
            CopyFilesProblemMessage = "The expected folder structure for this Mod does not exist.\r\n\r\nPlease make sure you have installed the correct Mod package to the Mod you selected.\r\n\r\nIf the problem persists, please contact the developer, either of the Mod or of Artemis Mod Loader, and inform them of the problem.";
        }
        public static string CopyFilesProblemMessage { get; set; }
        public static ReadOnlyCollection<DictionaryEntry> CopyFiles(DirectoryInfo source, string targetPath, string searchPattern, FileInfo[] exclusionFiles)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            List<DictionaryEntry> targets = new List<DictionaryEntry>();
            List<string> ExclusionList = new List<string>();
            if (exclusionFiles != null)
            {
                foreach (FileInfo f in exclusionFiles)
                {
                    ExclusionList.Add(f.FullName);
                }
            }
            if (source != null)
            {
                string root = null;

              
                if (source.Exists)
                {
                    foreach (FileInfo f in source.GetFiles(searchPattern, SearchOption.AllDirectories))
                    {
                        if (!ExclusionList.Contains(f.FullName))
                        {
                            if (root == null)
                            {
                                root = source.FullName;
                            }
                            string relative = f.DirectoryName.Substring(root.Length);
                            while (relative.StartsWith("\\", StringComparison.OrdinalIgnoreCase))
                            {
                                relative = relative.Substring(1);
                            }
                            while (relative.EndsWith("\\", StringComparison.OrdinalIgnoreCase))
                            {
                                relative = relative.Substring(0, relative.Length - 1);
                            }
                            string target = Path.Combine(targetPath, relative, f.Name);

                            FileHelper.Copy(f.FullName, target);


                            DictionaryEntry entry = new DictionaryEntry(f.FullName, target);
                            targets.Add(entry);
                        }

                    }
                }
                else
                {
                    SetCopyFilesProblem();
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return new ReadOnlyCollection<DictionaryEntry>(targets);

        }
        /// <summary>
        /// Deletes the file. (removes any read-only attribute to allow for delete).
        /// </summary>
        /// <param name="target">The target.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static void DeleteFile(string target)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (File.Exists(target))
            {
                try
                {
                    FileAttributes attr = File.GetAttributes(target);

                    if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        attr = attr & ~FileAttributes.ReadOnly;
                        File.SetAttributes(target, attr);
                        if (_log.IsInfoEnabled)
                        {
                            _log.InfoFormat("Removing readonly attribute from \"{0}\"", target);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (_log.IsWarnEnabled)
                    {
                        _log.Warn("Error turning off readonly attribute", ex);
                    }
                }
                if (_log.IsInfoEnabled)
                {
                    _log.InfoFormat("Deleting \"{0}\"", target);
                }
                try
                {
                    File.Delete(target);
                }
                catch (Exception ex)
                {
                    if (_log.IsWarnEnabled)
                    {
                        _log.Warn("Unable to delete " + target, ex);
                    }
                    GeneralResult = false;
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        public static bool GeneralResult { get; set; }
        public static void CreatePath(string path)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            DirectoryInfo d = new DirectoryInfo(path);

            if (!d.Parent.Exists)
            {
                CreatePath(d.Parent.FullName);
            }
            if (File.Exists(d.FullName))
            {
                FileHelper.DeleteFile(d.FullName);
            }
            if (!d.Exists)
            {
                d.Create();
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        /// <summary>
        /// Copies all files down the directory tree, starting with the source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="targetPath">The target path.</param>
        public static ReadOnlyCollection<DictionaryEntry> CopyFiles(DirectoryInfo source, string targetPath)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            ReadOnlyCollection<DictionaryEntry> retVal = FileHelper.CopyFiles(source, targetPath, "*.*", null);

            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }

        public static ReadOnlyCollection<DictionaryEntry> CopyFiles(DirectoryInfo source, string targetPath, FileInfo[] exclusionFiles)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            ReadOnlyCollection<DictionaryEntry> retVal = FileHelper.CopyFiles(source, targetPath, "*.*", exclusionFiles);

            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        /// <summary>
        /// Deletes all files.
        /// </summary>
        /// <param name="target">The target.</param>
        public static void DeleteAllFiles(string target)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (Directory.Exists(target))
            {
                foreach (FileInfo f in new DirectoryInfo(target).GetFiles("*.*", SearchOption.AllDirectories))
                {
                    FileHelper.DeleteFile(f.FullName);

                }
                FileHelper.DeleteDirectoryTree(target);
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static void DeleteDirectoryTree(string target)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }

            DirectoryInfo dir = new DirectoryInfo(target);

            foreach (DirectoryInfo d in dir.GetDirectories())
            {
                DeleteDirectoryTree(d.FullName);
            }
            if (_log.IsInfoEnabled)
            {
                _log.InfoFormat("Deleting \"{0}\"", dir.FullName);
            }
            try
            {
                dir.Delete();
            }
            catch (Exception ex)
            {
                if (_log.IsWarnEnabled)
                {
                    _log.Warn("Exception deleting directory " + dir.FullName, ex);
                }

            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
    }
}
