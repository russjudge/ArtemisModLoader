using DMXCommander.Xml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace DMXCommander
{
    public static class GeneralHelper
    {
        static string[] ArtemisCues = new string[] {
            "COLLISION_HITS_PLAYER",
            "COMPLETELY_DOCKED",
            "DAMCON_CASUALTY",
            "DRONE_HITS_PLAYER",
            "ENERGY_LOW",
            "ENERGY_20",
            "ENERGY_40",
            "ENERGY_60",
            "ENERGY_80",
            "ENERGY_100",
            "ENERGY_200",
            "ENTERING_NEBULA",
            "EXITING_NEBULA",
            "FRONT_SHIELD_LOW",
            "GAME_OVER",
            "HELM_IN_REVERSE",
            "JUMP_EXECUTED",
            "JUMP_FIZZLED",
            "JUMP_INITIATED",
            "JUST_KILLED_DAMCON_MEMBER",
            "LIGHTNING_HITS_PLAYER",
            "MINE_HITS_PLAYER",
            "NORMAL_CONDITION_1",
            "NPC_BEAM_HITS_PLAYER",
            "PLAYER_BEAM_HITS_PLAYER",
            "PLAYER_DESTROYED",
            "PLAYER_SHIELDS_LOWERED",
            "PLAYER_SHIELDS_ON",
            "PLAYER_SHIELDS_RAISED",
            "PLAYER_TAKES_FRONT_SHIELD_DAMAGE",
            "PLAYER_TAKES_INTERNAL_DAMAGE",
            "PLAYER_TAKES_REAR_SHIELD_DAMAGE",
            "PLAYER_TAKES_SHIELD_DAMAGE",
            "REAR_SHIELD_LOW",
            "RED_ALERT",
            "SELF_DESTRUCTED",
            "SHIP_DAMAGE_20", 
            "SHIP_DAMAGE_40",
            "SHIP_DAMAGE_60",
            "SOMETHING_HITS_PLAYER",
            "START_DOCKING",
            "TORPEDO_HITS_PLAYER",
            "TRACTORED_FOR_DOCKED",
            "WITHIN_NEBULA"};



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static ObservableCollection<string> GetCues()
        {
            return new ObservableCollection<string>(ArtemisCues);
        }

        static ObservableCollection<byte> ZeroThru255 = null;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "thru")]
        public static ObservableCollection<byte> Get0thru255()
        {
            if (ZeroThru255 == null)
            {
                ZeroThru255 = new ObservableCollection<byte>();
                for (short i = 0; i <= 255; i++)
                {
                    ZeroThru255.Add(Convert.ToByte(i));
                }

            }
            return ZeroThru255;
        }
        static void RefreshChannelList()
        {

            ChannelList = new List<KeyValuePair<string, string>>();


            LabelsToChannel = new Dictionary<string, int>();
            ChannelsToLabel = new Dictionary<int, string>();
            foreach (ChannelDefinition def in DMXConfigurationFile.Current.Definitions)
            {
                if (def != null)
                {
                    KeyValuePair<string, string> defItem = new KeyValuePair<string, string>(def.Group, def.Label);
                    if (!ChannelList.Contains(defItem))
                    {
                        ChannelList.Add(defItem);
                    }

                    if (!ChannelsToLabel.ContainsKey(def.Channel))
                    {
                        ChannelsToLabel.Add(def.Channel, def.Label);
                    }
                    if (!string.IsNullOrEmpty(def.Label) && !LabelsToChannel.ContainsKey(def.Label))
                    {
                        LabelsToChannel.Add(def.Label, def.Channel);
                    }
                }

            }

            if (ChannelListChanged != null)
            {
                ChannelListChanged(null, EventArgs.Empty);
            }
        }

        public static ListCollectionView GetChannelList()
        {
            
            
            if (ChannelList == null)
            {
                RefreshChannelList();
            }

            ListCollectionView retVal = new ListCollectionView(ChannelList);
            retVal.GroupDescriptions.Add(new PropertyGroupDescription("Key"));


            return retVal;
        }
        public static void ResetChannelList()
        {
            ChannelList = null;
            RefreshChannelList();
        }
        public static event EventHandler ChannelListChanged;

        public static string GetChannelLabel(int channel)
        {
            if (ChannelsToLabel != null)
            {
                if (ChannelsToLabel.ContainsKey(channel))
                {
                    return ChannelsToLabel[channel];
                }
                else
                {
                    ChannelDefinition def = new ChannelDefinition();
                    def.Channel = channel;
                    def.Label=channel.ToString();
                    
                    DMXConfigurationFile.Current.Definitions.Add(def);

                    RefreshChannelList();
                    return def.Label;
                }
            }
            else
            {
                return channel.ToString();
            }
        }
        public static int GetLabelToInt(string label)
        {
            if (LabelsToChannel == null)
            {
                return 0;
            }
            else
            {
                if (LabelsToChannel.ContainsKey(label))
                {
                    return LabelsToChannel[label];
                }
                else
                {
                    int lbl = 0;
                    if (!int.TryParse(label, out lbl))
                    {
                        return 0;
                    }
                    else
                    {
                        return lbl;
                    }
                }
            }
        }
        static List<KeyValuePair<string,string>> ChannelList = null;
        static Dictionary<string, int> LabelsToChannel = null;
        static Dictionary<int, string> ChannelsToLabel = null;
    }
}
