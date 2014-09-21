using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using NVorbis;
using System.IO;
using NAudio.Wave;
using System.ComponentModel;

namespace RussLibraryAudio
{
    public sealed class AudioServer : IDisposable, INotifyPropertyChanged
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly AudioServer Current = new AudioServer();


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        static WaveStream GetWaveStream(string file)
        {
            WaveChannel32 inputStream = null;
            WaveStream reader = null;
            WaveStream work = null;
            try
            {
                if (file.EndsWith(".ogg", StringComparison.OrdinalIgnoreCase))
                {
                    work = new NVorbis.NAudioSupport.VorbisWaveReader(file);
                    //TODO: Switch to memory stream and test.
                    //work = new NVorbis.NAudioSupport.VorbisWaveReader(
                }
                else if (file.EndsWith(".aiff", StringComparison.OrdinalIgnoreCase))
                {
                    work = new AiffFileReader(file);
                }
                else if (file.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
                {
                    work = new Mp3FileReader(file);
                }
                else if (file.EndsWith(".wma", StringComparison.OrdinalIgnoreCase))
                {
                    work = new NAudio.WindowsMediaFormat.WMAFileReader(file);
                }
                reader = work;
                work = null;

            }
            catch (Exception ex)
            {
                if (_log.IsWarnEnabled)
                {
                    _log.Warn("Unable to get audio stream", ex);
                }
            }
            finally
            {
                if (work != null)
                {
                    work.Dispose();
                }
            }
            try
            {
                if (reader != null)
                {
                    inputStream = new WaveChannel32(reader);
                }
            }
            catch (Exception ex)
            {
                if (_log.IsWarnEnabled)
                {
                    _log.Warn("Error trying to set up inputStream", ex);
                }
            }
            return inputStream;
        }
        void Initialize()
        {
            if (waveOut == null)
            {
                waveOut = new WaveOut();
                waveOut.PlaybackStopped += new EventHandler<StoppedEventArgs>(waveOut_PlaybackStopped);
                
            }
        }
        static readonly ILog _log = LogManager.GetLogger(typeof(AudioServer));
       
        
        void waveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }

            if (!NoPlay)
            {
                Changed("IsPlaying");
                PlayNextInQueue();
            }
            Changed("IsPlaying");
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        private void Play(string file)
        {
            if (!string.IsNullOrEmpty(file) && File.Exists(file))
            {
                bool holdNoplay = NoPlay;
                NoPlay = true;
                Stop();
                NoPlay = holdNoplay;
                using (WaveStream wStream = GetWaveStream(file))
                {
                    if (wStream != null)
                    {
                        Changed("IsPlaying");



                        waveOut.Init(wStream);
                        waveOut.Volume = 1;
                        waveOut.Play();
                        Changed("IsPlaying");
                        while (waveOut.PlaybackState != NAudio.Wave.PlaybackState.Stopped
                            && wStream.TotalTime >= wStream.CurrentTime)
                        {
                            System.Threading.Thread.Sleep(100);
                        }
                    }
                }
                if (waveOut.PlaybackState != PlaybackState.Stopped)
                {
                    waveOut.Stop();
                }
            }

        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public float GetVolume()
        {
            return waveOut.Volume;
        }
        public void SetVolume(float volume)
        {
            waveOut.Volume = volume;
        }
        NAudio.Wave.WaveOut waveOut = null;
        public void PlayAsync(string file)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(PlayAsync, file);
        }
        private void PlayAsync(object state)
        {
            Play(state as string);
        }
        private AudioServer()
        {
            Initialize();  
        }
     
        
        public int FilesQueued
        {
            get
            {
                return audioList.Count;
            }
        }
        bool NoPlay = true;
        public void Stop()
        {
            if (waveOut != null)
            {
                if (waveOut.PlaybackState != PlaybackState.Stopped)
                {
                    waveOut.Stop();
                }
                Changed("IsPlaying");
            }
            NoPlay = true;
        }
        public bool IsPlaying
        {
            get
            {
                return (waveOut.PlaybackState == PlaybackState.Playing);
            }
        }
        
        int index = -1;
        //private void MovePrevious()
        //{
        //    if (--index < 0)
        //    {
        //        index = audioList.Count - 1;
        //    }
        //}
        public void MoveNext()
        {
            PurgeNonExistentFiles();  
            if (++index >= audioList.Count)
            {
                index = 0;
            }
           
        }
        void PurgeNonExistentFiles()
        {
            int i = -1;
            List<string> newList = new List<string>();
            foreach (string item in audioList)
            {
                i++;
                if (System.IO.File.Exists(item))
                {
                    newList.Add(item);
                }
                else
                {
                    if (i <= index)
                    {
                        index--;
                    }
                }
            }
           
            audioList = new List<string>(newList);
        }
        List<string> audioList = new List<string>();
        public void PlayNextInQueue()
        {
         
            NoPlay = false;
            if (audioList.Count > 0)
            {
                
               
                MoveNext();
                if (index >= 0)
                {
                    PlayAsync(audioList[index]);
                }
            }
          
        }
        public void Pause()
        {
            if (this.IsPlaying && waveOut != null)
            {
                waveOut.Pause();
            }
        }
        public void Resume()
        {
            if (waveOut != null)
            {
                waveOut.Resume();
            }
        }

        public void Clear()
        {
            audioList.Clear();
        }
        public void Enqueue(string file)
        {
            if (!string.IsNullOrEmpty(file) && System.IO.File.Exists(file))
            {
                audioList.Add(file);
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "idx")]
        public void RemoveAt(int idx)
        {
            if (idx <= index)
            {
                index--;
            }
            audioList.RemoveAt(idx);
        }
        public void Remove(string file)
        {
            int idx = audioList.IndexOf(file);
            if (idx <= index)
            {
                index--;
            }
            audioList.Remove(file);
        }

        #region IDisposable Members
        private void Dispose(bool isDisposing)
        {
            if (!isDisposed)
            {
                if (isDisposing)
                {

                    if (waveOut != null)
                    {

                        waveOut.PlaybackStopped -= new EventHandler<StoppedEventArgs>(waveOut_PlaybackStopped);
                        Stop();
                        waveOut.Dispose();
                    }
                    isDisposed = true;
                }
            }
        }
        bool isDisposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        void Changed(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        
    }
}
