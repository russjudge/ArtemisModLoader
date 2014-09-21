using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.IO;
using RussLibrary;
using RussLibrary.Xml;
using VesselDataLibrary.Xml;
using System.Windows.Controls;
using System.Windows;
using MissionStudio.Helpers;
using System.Globalization;

namespace MissionStudio
{

    public sealed class Commands
    {
        static Commands()
        {
            Current = new Commands();
        }
        public static Commands Current { get; private set; }
        private Commands()
        {
            CommandDictionary = new Dictionary<string, CommandElement>();
            LoadVesselData();
            LoadMissionDoc();
        }
        #region Public Items
        public Dictionary<string, CommandElement> CommandDictionary
        {
            get;
            private set;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "vessel")]
        public VesselDataLibrary.Xml.VesselDataObject vesselData
        {
            get;
            set;
        }
        #endregion

        #region Private Items
        //static readonly ILog _log = LogManager.GetLogger(typeof(Commands));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }

        //Load from mission-file-docs.txt
        void LoadMissionDoc()
        {

            //First load special critical COMMANDS, then load from file.

            //LoadCreate();
            //12/30/2012
            string txt = Properties.Resources.mission_file_docs;
            bool started = false;
            CommandElement currentActiveCommand = null;
            AttributeElement currentActiveAttribute = null;
            foreach (string ll in txt.Split('\n'))
            {
                string sline = ll.Replace("\t", string.Empty).Replace("\r", string.Empty).Trim();

                if (!started)
                {
                    if (sline.StartsWith("COMMAND", StringComparison.OrdinalIgnoreCase))
                    {
                        started = true;
                    }
                }

                if (started)
                {
                    switch (DetermineLineType(sline))
                    {
                        case LineType.Command:
                        case LineType.Condition:
                            KeyValuePair<string, string> cmd = GetCommandData(sline);
                            currentActiveCommand = new CommandElement(cmd.Key, cmd.Value);
                            if (!CommandDictionary.ContainsKey(cmd.Key))
                            {
                                CommandDictionary.Add(cmd.Key, currentActiveCommand);
                            }
                            else
                            {
                                currentActiveCommand = CommandDictionary[cmd.Key];
                            }
                            break;
                        case LineType.Attribute:
                            if (currentActiveCommand != null)
                            {
                                bool foundAttrib = false;
                                currentActiveAttribute = new AttributeElement(GetAttributeData(sline));
                                currentActiveAttribute.ParentCommand = currentActiveCommand.Text;
                                foreach (AttributeElement att in currentActiveCommand.Attributes)
                                {
                                    if (att.Text == currentActiveAttribute.Text)
                                    {
                                        foundAttrib = true;
                                        currentActiveAttribute = att;
                                        break;
                                    }
                                }
                                if (!foundAttrib)
                                {
                                    currentActiveCommand.Attributes.Add(currentActiveAttribute);
                                }
                            }
                            break;
                        case LineType.ValueList:
                            ProcessValue(sline, currentActiveAttribute);

                            break;

                    }
                }


            }

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Controls.TextBlock.set_Text(System.String)")]
        void ProcessHullID(AttributeElement currentActiveAttribute)
        {
            currentActiveAttribute.AttributeType = AttributeType.LimitedListString;
            currentActiveAttribute.Values = new System.Collections.ObjectModel.ObservableCollection<XmlCompletionData>();
            if (vesselData != null)
            {
                foreach (VesselDataLibrary.Xml.Vessel v in vesselData.Vessels)
                {
                    string race = string.Empty;
                    foreach (VesselDataLibrary.Xml.HullRace r in vesselData.HullRaces)
                    {
                        if (v.Side == r.ID)
                        {
                            race = r.Name;
                            break;
                        }
                    }
                    string wrkD = string.Empty;
                    if (!string.IsNullOrEmpty(v.Description.Text))
                    {
                        wrkD = v.Description.Text.Replace("^", "\r\n");
                    }
                    if (string.IsNullOrEmpty(wrkD))
                    {
                        wrkD = string.Format(CultureInfo.InvariantCulture, "{0} {1}", v.ClassName, v.BroadType);
             
                    }
                    string de = string.Format(CultureInfo.InvariantCulture, "{2} ({0}) {1}", race, wrkD, v.UniqueID.ToString(CultureInfo.InvariantCulture));

                    StackPanel sp = new StackPanel();
                    sp.Orientation = Orientation.Horizontal;
                    if (wrkD.Contains("\r\n"))
                    {
                        wrkD = wrkD.Substring(0, wrkD.IndexOf("\r\n", StringComparison.OrdinalIgnoreCase));
                    }
                    TextBlock t = new TextBlock();
                    t.VerticalAlignment = VerticalAlignment.Center;

                    t.Text = v.UniqueID.ToString(CultureInfo.InvariantCulture);
                    t.Padding = new Thickness(0, 0, 4, 0);
                    t.FontWeight = FontWeights.Bold;
                    sp.Children.Add(t);

                    TextBlock t2 = new TextBlock();
                    t2.VerticalAlignment = VerticalAlignment.Center;
                    t2.Text = string.Format(CultureInfo.InvariantCulture, "({0}) {1}", race, wrkD);
                    sp.Children.Add(t2);

                    XmlCompletionData value = new XmlCompletionData(v.UniqueID.ToString(CultureInfo.InvariantCulture), de, sp);
                    currentActiveAttribute.Values.Add(value);
                }
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "RussLibrary.XmlCompletionData.#ctor(System.String)")]
        void ProcessRaceKeys(AttributeElement currentActiveAttribute)
        {
            currentActiveAttribute.AttributeType = AttributeType.LimitedListString;
            currentActiveAttribute.Values = new System.Collections.ObjectModel.ObservableCollection<XmlCompletionData>();
            //foreach (VesselDataLibrary.Xml.Vessel v in vesselData.Vessels)
            //{
              //  string race = string.Empty;
                List<XmlCompletionData> data = new List<XmlCompletionData>();
                if (vesselData != null)
                {
                    foreach (VesselDataLibrary.Xml.HullRace r in vesselData.HullRaces)
                    {
                        string key = string.Format(CultureInfo.InvariantCulture, "{0} {1}", r.Name, r.Keys);

                        data.Add(new XmlCompletionData(key));

                    }
                }
                if (data.Count > 0)
                {

                    currentActiveAttribute.Values = new System.Collections.ObjectModel.ObservableCollection<XmlCompletionData>();
                    foreach (XmlCompletionData value in data)
                    {
                        currentActiveAttribute.Values.Add(value);
                    }
                }
            //}
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Controls.TextBlock.set_Text(System.String)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "RussLibrary.XmlCompletionData.#ctor(System.String,System.String,System.Object)")]
        void ProcessHullKeys(AttributeElement currentActiveAttribute)
        {
            currentActiveAttribute.AttributeType = AttributeType.LimitedListString;
            currentActiveAttribute.Values = new System.Collections.ObjectModel.ObservableCollection<XmlCompletionData>();
            List<string> vesselDictionary = new List<string>();
            List<XmlCompletionData> items = new List<XmlCompletionData>();
            if (vesselData != null)
            {
                foreach (VesselDataLibrary.Xml.Vessel v in vesselData.Vessels)
                {
                    XmlCompletionData value = null;
                    string key = v.ClassName + " " + v.BroadType;
                    if (!vesselDictionary.Contains(key))
                    {
                        string race = string.Empty;
                        foreach (VesselDataLibrary.Xml.HullRace r in vesselData.HullRaces)
                        {
                            if (v.Side == r.ID)
                            {
                                race = r.Name;
                                break;
                            }
                        }
                        string wrkD = string.Empty;
                        if (!string.IsNullOrEmpty(v.Description.Text))
                        {
                            wrkD = v.Description.Text.Replace("^", "\r\n");
                        }



                        string desc = string.Format(CultureInfo.InvariantCulture, "{0} ({1}-{2})", key, wrkD, race);

                        StackPanel sp = new StackPanel();
                        sp.Orientation = Orientation.Horizontal;

                        TextBlock t = new TextBlock();
                        t.VerticalAlignment = VerticalAlignment.Center;

                        t.Text = key;
                        t.Padding = new Thickness(0, 0, 4, 0);
                        t.FontWeight = FontWeights.Bold;
                        sp.Children.Add(t);

                        TextBlock t2 = new TextBlock();
                        t2.VerticalAlignment = VerticalAlignment.Center;
                        t2.Text = string.Format(CultureInfo.InvariantCulture, "({0}) {1}", race, wrkD);
                        sp.Children.Add(t2);


                        value =
                            new XmlCompletionData(key, desc, sp);
                        vesselDictionary.Add(key);
                        items.Add(value);
                    }
                    if (items.Count > 0)
                    {
                        currentActiveAttribute.Values = new System.Collections.ObjectModel.ObservableCollection<XmlCompletionData>();

                        foreach (XmlCompletionData data in items)
                        {
                            currentActiveAttribute.Values.Add(data);
                        }
                    }
                }
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Controls.TextBlock.set_Text(System.String)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "RussLibrary.XmlCompletionData.#ctor(System.String,System.String,System.Object)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "RussLibrary.XmlCompletionData.#ctor(System.String)")]
        void ProcessValue(string sline, AttributeElement currentActiveAttribute)
        {
            if (currentActiveAttribute != null)
            {
                if (currentActiveAttribute.Text == "hullID")
                {
                    ProcessHullID(currentActiveAttribute);
                }
                else if (currentActiveAttribute.Text == "raceKeys")
                {
                    ProcessRaceKeys(currentActiveAttribute);   
                }
                else if (currentActiveAttribute.Text == "hullKeys")
                {
                    ProcessHullKeys(currentActiveAttribute);
                    
                }
                else
                {
                    IList<XmlCompletionData> data = GetValueData(sline, currentActiveAttribute);
                    if (data != null)
                    {
                        currentActiveAttribute.Values = new System.Collections.ObjectModel.ObservableCollection<XmlCompletionData>();
                        foreach (XmlCompletionData item in data)
                        {
                            currentActiveAttribute.Values.Add(item);
                        }
                    }
                }
            }
        }
        static IList<XmlCompletionData> GetValueData(string line, AttributeElement currentAttribute)
        {
            List<XmlCompletionData> retVal = null;
            //VALID: 0-360
            //VALID: -100000 to 100000
            //VALID: text
            //VALID: 0 or 1
            //VALID: nebulas, asteroids, mines
            string wrk = line.Substring(7).Trim();
            List<XmlCompletionData> wrkData = new List<XmlCompletionData>();
            currentAttribute.AttributeType = AttributeType.LimitedListString;
            if (char.IsDigit(wrk[0]) || (wrk[0] == '-' && char.IsDigit(wrk[1])) || wrk.StartsWith("signed", StringComparison.OrdinalIgnoreCase))
            {
                SetExpressType(currentAttribute);
                
            }
            else if (wrk.StartsWith("anything", StringComparison.OrdinalIgnoreCase))
            {
                currentAttribute.AttributeType = AttributeType.FreeString;  
            }
            else if (wrk.StartsWith("text", StringComparison.OrdinalIgnoreCase) || wrk.Contains(','))
            {
                wrkData = CheckForSpecialValueString(wrk, currentAttribute);
                if (wrkData == null)
                {
                    currentAttribute.AttributeType = AttributeType.FreeString;
                }
                else
                {
                    currentAttribute.AttributeType = AttributeType.LimitedListString;
                }
            }
            if (wrkData != null && wrkData.Count > 0)
            {
                retVal = new List<XmlCompletionData>(wrkData);
            }
            return retVal;
        }
        static void SetExpressType(AttributeElement currentAttribute)
        {

            
            currentAttribute.AttributeType = AttributeType.ExpressionValue;

        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "RussLibrary.XmlCompletionData.#ctor(System.String)")]
        static List<XmlCompletionData> CheckForSpecialValueString(string wrk, AttributeElement currentAttribute)
        {
            List<XmlCompletionData> retVal = null;
            List<XmlCompletionData> wrkData = new List<XmlCompletionData>();
            switch (currentAttribute.ParentCommand)
            {
                //case "add_ai":
                //    if (currentAttribute.Text == "type")
                //    {
                //        wrkData.Add(new XmlCompletionData("TRY_TO_BECOME_LEADER"));
                //        wrkData.Add(new XmlCompletionData("CHASE_PLAYER"));
                //        wrkData.Add(new XmlCompletionData("CHASE_NEUTRAL"));
                //        wrkData.Add(new XmlCompletionData("CHASE_ENEMY"));
                //        wrkData.Add(new XmlCompletionData("CHASE_STATION"));
                //        wrkData.Add(new XmlCompletionData("CHASE_WHALE"));
                //        wrkData.Add(new XmlCompletionData("AVOID_WHALE"));
                //        wrkData.Add(new XmlCompletionData("AVOID_BLACK_HOLE"));
                //        wrkData.Add(new XmlCompletionData("CHASE_ANGER"));
                //        wrkData.Add(new XmlCompletionData("CHASE_FLEET"));
                //        wrkData.Add(new XmlCompletionData("FOLLOW_LEADER"));
                //        wrkData.Add(new XmlCompletionData("FOLLOW_COMMS_ORDERS"));
                //        wrkData.Add(new XmlCompletionData("LEADER_LEADS"));
                //        wrkData.Add(new XmlCompletionData("ELITE_AI"));
                //        wrkData.Add(new XmlCompletionData("DIR_THROTTLE"));
                //        wrkData.Add(new XmlCompletionData("POINT_THROTTLE"));
                //        wrkData.Add(new XmlCompletionData("TARGET_THROTTLE"));
                //        wrkData.Add(new XmlCompletionData("ATTACK"));
                //        wrkData.Add(new XmlCompletionData("DEFEND"));
                //        wrkData.Add(new XmlCompletionData("PROCEED_TO_EXIT"));
                //        wrkData.Add(new XmlCompletionData("FIGHTER_BINGO"));
                //        wrkData.Add(new XmlCompletionData("LAUNCH_FIGHTERS"));
                //    }
                //    else if (currentAttribute.Text == "targetName")
                //    {
                //        wrkData.Add(new XmlCompletionData("ATTACK"));
                //        wrkData.Add(new XmlCompletionData("TARGET_THROTTLE"));
                //    }
                //    break;
                case "create":
                    if (currentAttribute.Text == "type")
                    {
                        //station, player, enemy, neutral, anomaly, blackHole, monster, genericMesh, whale, nebulas, asteroids, mines
                        foreach (string val in "station,player,enemy,neutral,anomaly,blackHole,monster,genericMesh,whale,nebulas,asteroids,mines".Split(','))
                        {
                            wrkData.Add(new XmlCompletionData(val.Trim()));
                        }
                        
                    }
                    break;
                case "set_fleet_property":
                    if (currentAttribute.Text == "property")
                    {
                        foreach (string val in "fleetSpacing,fleetMaxRadius".Split(','))
                        {
                            wrkData.Add(new XmlCompletionData(val.Trim()));
                        }
                    }
                    break;
                //case "set_player_grid_damage":
                //    if (currentAttribute.Text == "systemType")
                //    {
                //        foreach (string val in "systemBeam,systemTorpedo,systemTactical,systemTurning,systemImpulse,systemWarp,systemFrontShield,systemBackShield".Split(','))
                //        {
                //            wrkData.Add(new XmlCompletionData(val.Trim()));
                //        }
                //    }
                //    break;
                default:
                    if (currentAttribute.Text == "property")
                    {
                        wrkData.AddRange(GetPropertyList());
                    }
                    if (wrk.Trim() != "text" && wrk.Trim() != "anything")
                    {
                        foreach (string val in wrk.Replace(" ", string.Empty).Split(','))
                        {
                            wrkData.Add(new XmlCompletionData(val.Trim()));
                        }
                    }
                    break;
            }
            if (wrkData.Count > 0)
            {
                retVal = new List<XmlCompletionData>(wrkData);
            }
            return retVal;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "warpState"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "turnRate"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "triggersMines"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "totalCoolant"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "topSpeed"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "targetPointZ"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "targetPointY"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "targetPointX"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "systemDamageWarp"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "systemDamageTurning"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "systemDamageTorpedo"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "systemDamageTactical"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "systemDamageImpulse"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "systemDamageFrontShield"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "systemDamageBeam"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "systemDamageBackShield"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "surrenderChance"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "shieldsOn"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "shieldStateFront"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "shieldStateBack"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "shieldState"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "shieldMaxStateFront"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "shieldMaxStateBack"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "rollDelta"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "pushRadius"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "positionZ"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "positionY"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "positionX"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "pitchDelta"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "missileStoresNuke"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "missileStoresMine"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "missileStoresHoming"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "missileStoresECM"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "hasSurrendered"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "exitPointZ"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "exitPointY"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "exitPointX"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "eliteAbilityState"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "eliteAbilityBits"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "eliteAIType"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "deltaZ"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "deltaY"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "deltaX"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "currentRealSpeed"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "countNuke"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "countMine"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "countHoming"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "countECM"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "canBuild"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "blocksShotFlag"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "artScale"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "angleDelta"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "RussLibrary.XmlCompletionData.#ctor(System.String)")]
        static List<XmlCompletionData> GetPropertyList()
        {
            List<XmlCompletionData> retVal = new List<XmlCompletionData>();
            //for everything

            retVal.Add(new XmlCompletionData("positionX"));
            retVal.Add(new XmlCompletionData("positionY"));
            retVal.Add(new XmlCompletionData("positionZ"));
            retVal.Add(new XmlCompletionData("deltaX"));
            retVal.Add(new XmlCompletionData("deltaY"));
            retVal.Add(new XmlCompletionData("deltaZ"));
            retVal.Add(new XmlCompletionData("angle"));
            retVal.Add(new XmlCompletionData("pitch"));
            retVal.Add(new XmlCompletionData("roll"));
            // values for GenericMeshs
            retVal.Add(new XmlCompletionData("blocksShotFlag"));
            retVal.Add(new XmlCompletionData("pushRadius"));
            retVal.Add(new XmlCompletionData("pitchDelta"));
            retVal.Add(new XmlCompletionData("rollDelta"));
            retVal.Add(new XmlCompletionData("angleDelta"));
            retVal.Add(new XmlCompletionData("artScale"));

            // values for Stations
            retVal.Add(new XmlCompletionData("shieldState"));
            retVal.Add(new XmlCompletionData("canBuild"));
            retVal.Add(new XmlCompletionData("missileStoresHoming"));
            retVal.Add(new XmlCompletionData("missileStoresNuke"));
            retVal.Add(new XmlCompletionData("missileStoresMine"));
            retVal.Add(new XmlCompletionData("missileStoresECM"));

            // values for ShieldedShips
            retVal.Add(new XmlCompletionData("throttle"));
            retVal.Add(new XmlCompletionData("steering"));
            retVal.Add(new XmlCompletionData("topSpeed"));
            retVal.Add(new XmlCompletionData("turnRate"));
            retVal.Add(new XmlCompletionData("shieldStateFront"));
            retVal.Add(new XmlCompletionData("shieldMaxStateFront"));
            retVal.Add(new XmlCompletionData("shieldStateBack"));
            retVal.Add(new XmlCompletionData("shieldMaxStateBack"));
            retVal.Add(new XmlCompletionData("shieldsOn"));
            retVal.Add(new XmlCompletionData("triggersMines"));
            retVal.Add(new XmlCompletionData("systemDamageBeam"));
            retVal.Add(new XmlCompletionData("systemDamageTorpedo"));
            retVal.Add(new XmlCompletionData("systemDamageTactical"));
            retVal.Add(new XmlCompletionData("systemDamageTurning"));
            retVal.Add(new XmlCompletionData("systemDamageImpulse"));
            retVal.Add(new XmlCompletionData("systemDamageWarp"));
            retVal.Add(new XmlCompletionData("systemDamageFrontShield"));
            retVal.Add(new XmlCompletionData("systemDamageBackShield"));
            retVal.Add(new XmlCompletionData("shieldBandStrength0"));
            retVal.Add(new XmlCompletionData("shieldBandStrength1"));
            retVal.Add(new XmlCompletionData("shieldBandStrength2"));
            retVal.Add(new XmlCompletionData("shieldBandStrength3"));
            retVal.Add(new XmlCompletionData("shieldBandStrength4"));

            // values for Enemys
            retVal.Add(new XmlCompletionData("targetPointX"));
            retVal.Add(new XmlCompletionData("targetPointY"));
            retVal.Add(new XmlCompletionData("targetPointZ"));
            retVal.Add(new XmlCompletionData("hasSurrendered"));
            retVal.Add(new XmlCompletionData("eliteAIType"));
            retVal.Add(new XmlCompletionData("eliteAbilityBits"));
            retVal.Add(new XmlCompletionData("eliteAbilityState"));
            retVal.Add(new XmlCompletionData("surrenderChance"));

            // values for Neutrals
            retVal.Add(new XmlCompletionData("exitPointX"));
            retVal.Add(new XmlCompletionData("exitPointY"));
            retVal.Add(new XmlCompletionData("exitPointZ"));

            // values for Players
            retVal.Add(new XmlCompletionData("countHoming"));
            retVal.Add(new XmlCompletionData("countNuke"));
            retVal.Add(new XmlCompletionData("countMine"));
            retVal.Add(new XmlCompletionData("countECM"));
            retVal.Add(new XmlCompletionData("energy"));
            retVal.Add(new XmlCompletionData("warpState"));
            retVal.Add(new XmlCompletionData("currentRealSpeed"));
            retVal.Add(new XmlCompletionData("totalCoolant"));
            return retVal;
        }
       
        static LineType DetermineLineType(string line)
        {
            LineType retVal = LineType.Continuation;
            string wrk = line.Replace("\t", string.Empty).Trim();
            if (wrk.StartsWith("COMMAND", StringComparison.Ordinal))
            {
                retVal = LineType.Command;
            }
            if (wrk.StartsWith("CONDITION", StringComparison.Ordinal))
            {
                retVal = LineType.Condition;
            }
            if (wrk.StartsWith("ATTRIBUTE", StringComparison.Ordinal))
            {
                retVal = LineType.Attribute;
            }
            if (wrk.StartsWith("VALID", StringComparison.Ordinal))
            {
                retVal = LineType.ValueList;
            }
            return retVal;
        }
        enum LineType
        {
            Command,
            Condition,
            Attribute,
            ValueList,
            Continuation
        }
        static KeyValuePair<string, string> GetCommandData(string line)
        {
            KeyValuePair<string, string> retVal;
            int i = line.IndexOf(" ", StringComparison.OrdinalIgnoreCase);
            string wrk = line.Substring(i + 1).Trim();

            i = wrk.IndexOf(" ", StringComparison.OrdinalIgnoreCase);
            if (i < 0)
            {
                i = wrk.Length;
            }
            string cmd = wrk.Substring(0, i);
            string desc = string.Empty;
            if (i < wrk.Length)
            {
                desc = wrk.Substring(i + 1).Trim();
            }
            retVal = new KeyValuePair<string, string>(cmd, desc);
            return retVal;
        }
        static string GetAttributeData(string line)
        {
            string retVal;
            int i = line.IndexOf(" ", StringComparison.OrdinalIgnoreCase);
            string wrk = line.Substring(i + 1).Trim();

            i = wrk.IndexOf(" ", StringComparison.OrdinalIgnoreCase);
            if (i < 0)
            {
                i = wrk.Length;
            }
            string cmd = wrk.Substring(0, i);
            retVal = cmd;
            return retVal;
        }
        
        void LoadVesselData()
        {
            //VesselData will be null if the vesselData.Xml file cannot be found.
            vesselData = XmlConverter.ToObject(Path.Combine(Locations.ArtemisInstallPath, "dat", "vesselData.xml"),
                typeof(VesselDataLibrary.Xml.VesselDataObject)) as VesselDataLibrary.Xml.VesselDataObject;
        }
        //    COMMAND: create (the command that creates named objects in the game)
        //ATTRIBUTE: type
        //        VALID: station, player, enemy, neutral, anomaly, blackHole, monster, genericMesh, whale
        //ATTRIBUTE: x
        //        VALID: 0 to 100000
        //ATTRIBUTE: y
        //        VALID: -100000 to 100000
        //ATTRIBUTE: z
        //        VALID: 0 to 100000
        //ATTRIBUTE: use_gm_position
        //        VALID: anything, just use this attribute to cause the x,y,z to be at the game master's selected position

        //ATTRIBUTE: name
        //        VALID: text
        //ATTRIBUTE: hulltype
        //        VALID: 0-?  (corresponds to the unique hull ID in vesselData.xml)
        //ATTRIBUTE: raceKeys
        //        VALID: text (corresponds to hullRace name and keys in vesselData.xml)
        //ATTRIBUTE: hullKeys
        //        VALID: text (corresponds to vessel className and broadType in vesselData.xml)
        //ATTRIBUTE: angle
        //        VALID: 0-360
        //ATTRIBUTE: fleetnumber
        //        VALID: 1-99

        //-- for genericMeshs
        //ATTRIBUTE: meshFileName
        //        VALID: text
        //ATTRIBUTE: textureFileName
        //        VALID: text
        //ATTRIBUTE: hullRace
        //        VALID: text
        //ATTRIBUTE: hullType
        //        VALID: text

        //ATTRIBUTE: fakeShieldsFront
        //        VALID: 1-1000
        //ATTRIBUTE: fakeShieldsRear
        //        VALID: 1-1000
        //            NOTE: the fake shields default to -1, which means no fake shields
        //                  if only the fakeShieldsFront is positive, the generic looks like a station

        //ATTRIBUTE: hasFakeShldFreq
        //        VALID: 0 or 1

        //ATTRIBUTE: ColorRed
        //        VALID: 0.0-1.0
        //ATTRIBUTE: ColorGreen
        //        VALID: 0.0-1.0
        //ATTRIBUTE: ColorBlue
        //        VALID: 0.0-1.0

        //-- for whales
        //ATTRIBUTE: podnumber
        //        VALID: 0-9

        //If you use a fleet number that's illegal, crashes and wierd graphical glitches will occur.

        //ATTRIBUTE: type
        //        VALID: station, player, enemy, neutral, anomaly, blackHole, monster, genericMesh, whale
        // string[] TypesList = { "station", "player", "enemy", "neutral", "anomaly", "blackHole", "monster", "genericMesh", "whale" };
        //ATTRIBUTE: x
        //        VALID: 0 to 100000
        //ATTRIBUTE: y
        //        VALID: -100000 to 100000
        //ATTRIBUTE: z
        //        VALID: 0 to 100000



        //    CommandElement LoadCreate()
        //    {
        //        CommandElement retVal = null;
        //        List<XmlCompletionData> values = new List<XmlCompletionData>();
        //        List<AttributeElement> attributes = new List<AttributeElement>();
        //        XmlCompletionData v = null;

        //        foreach (string type in TypesList)
        //        {
        //            v = new XmlCompletionData(type, type);
        //            values.Add(v);
        //        }


        //        AttributeElement attrib = new AttributeElement("type", AttributeType.LimitedListString, "Named Object to Create", values);
        //        attributes.Add(attrib);
        //        attrib = new AttributeElement("x", AttributeType.ExpressionValue, "X position", null);
        //        attrib.Maximum = 100000;
        //        attrib.Minimum = 0;

        //        attributes.Add(attrib);
        //        attrib = new AttributeElement("y", AttributeType.ExpressionValue, "Y position", null);
        //        attrib.Minimum = -100000;
        //        attrib.Maximum = 100000;

        //        attributes.Add(attrib);
        //        attrib = new AttributeElement("z", AttributeType.ExpressionValue, "z position", null);
        //        attrib.Maximum = 100000;
        //        attrib.Minimum = 0;

        //        attributes.Add(attrib);

        //         //ATTRIBUTE: use_gm_position
        ////        VALID: anything, just use this attribute to cause the x,y,z to be at the game master's selected position

        //        attrib = new AttributeElement("use_gm_position", AttributeType.FreeString, "Set to game master selected position", null);
        //        attributes.Add(attrib);



        //        //ATTRIBUTE: name
        //        //        VALID: text
        //        attrib = new AttributeElement("name", AttributeType.FreeString, "Name of object to create", null);
        //        attributes.Add(attrib);

        //        //ATTRIBUTE: hulltype
        //        //        VALID: 0-?  (corresponds to the unique hull ID in vesselData.xml)
        //        attrib = new AttributeElement("hulltype", AttributeType.ExpressionValue, "Enter Hull Type", null);
        //        attributes.Add(attrib);

        //        //ATTRIBUTE: raceKeys
        //        //        VALID: text (corresponds to hullRace name and keys in vesselData.xml)
        //        //ATTRIBUTE: hullKeys
        //        //        VALID: text (corresponds to vessel className and broadType in vesselData.xml)
        //        //ATTRIBUTE: angle
        //        //        VALID: 0-360
        //        //ATTRIBUTE: fleetnumber
        //        //        VALID: 1-99
        //        return retVal;
        //    }
        #endregion
    }
}
