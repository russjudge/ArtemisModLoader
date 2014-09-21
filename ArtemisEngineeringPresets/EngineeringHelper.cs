using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.IO;
using ArtemisModLoader;
using System.Collections.ObjectModel;

namespace ArtemisEngineeringPresets
{

    public static class EngineeringHelper
    {
        //static readonly ILog _log = LogManager.GetLogger(typeof(EngineeringHelper));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        public static bool IsSupportedVersion
        {
            get
            {
                return (ModManagement.GetActiveArtemisVersion() != "Version unknown.");
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static Collection<Preset> LoadPresetFile(string file)
        {
            Collection<Preset> Presets = new Collection<Preset>();
            if (File.Exists(file))
            {
                using (FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            if (br.ReadByte() != Convert.ToByte(254))
                            {
                                throw new InvalidPresetFileException("Header \"0xfefe\" missing!");
                            }
                        }



                        for (int i = 0; i < 10; i++)
                        {
                            List<int> energyLevels = new List<int>();
                            List<int> coolantLevels = new List<int>();
                            for (int j = 0; j < 8; j++)
                            {
                                energyLevels.Add((int)Math.Round(br.ReadSingle() * 300));
                            }
                            for (int j = 0; j < 8; j++)
                            {
                                coolantLevels.Add(Convert.ToInt32(br.ReadByte()));
                            }
                            Preset p = new Preset(energyLevels, coolantLevels);

                            Presets.Add(p);
                        }
                    }
                }
            }
            else
            {

                for (int i = 0; i < 10; i++)
                {
                    
                    Preset p = new Preset();

                    Presets.Add(p);
                }
            }
            return Presets;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static void WritePresetFile(string file, IList<Preset> presets)
        {
            if (presets != null && presets.Count == 10)
            {
                using (FileStream fs = File.Open(file, FileMode.Create, FileAccess.Write))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        bw.Write(new byte[] { 254, 254 });
                        foreach (Preset p in presets)
                        {

                            for (int i = 0; i < 8; i++)
                            {
                                bw.Write((float)p.SystemLevels[i].EnergyLevel / 300);
                            }
                            for (int i = 0; i < 8; i++)
                            {
                                byte b = (byte)p.SystemLevels[i].CoolantLevel;
                                bw.Write(b);
                            }
                        }
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("Must have 10 preset entries.");
            }
        }
       

    }
}
