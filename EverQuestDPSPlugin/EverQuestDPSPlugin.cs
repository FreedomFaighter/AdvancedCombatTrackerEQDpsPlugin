﻿using Advanced_Combat_Tracker;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using EverQuestDPS.Enums;

/*
* Project: EverQuest DPS Plugin
* Original: EverQuest 2 English DPS Localization plugin developed by EQAditu
* Description: Missing from the arsenal of the plugin based Advanced Combat Tracker to track EverQuest's current combat messages.  Ignores chat as that is displayed in game.
*/

namespace EverQuestDPS
{
    public class EverQuestDPSPlugin : UserControl, IActPluginV1
    {
        #region Designer generated code (Avoid editing)
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!(components == null))
                {
                    components.Dispose();
                }
                if (!(watcherForDebugFile == null))
                {
                    watcherForDebugFile.Dispose();
                }
                if (!(watcherForRaidRoster == null))
                {
                    watcherForRaidRoster.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.varianceOff = new System.Windows.Forms.RadioButton();
            this.sampVariance = new System.Windows.Forms.RadioButton();
            this.populVariance = new System.Windows.Forms.RadioButton();
            this.selectDirectory = new System.Windows.Forms.Button();
            this.eqDirectory = new System.Windows.Forms.Label();
            this.directoryPathTB = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.varianceOff);
            this.groupBox1.Controls.Add(this.sampVariance);
            this.groupBox1.Controls.Add(this.populVariance);
            this.groupBox1.Location = new System.Drawing.Point(33, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 100);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Variance";
            // 
            // varianceOff
            // 
            this.varianceOff.AutoSize = true;
            this.varianceOff.Location = new System.Drawing.Point(37, 68);
            this.varianceOff.Name = "varianceOff";
            this.varianceOff.Size = new System.Drawing.Size(39, 17);
            this.varianceOff.TabIndex = 2;
            this.varianceOff.TabStop = true;
            this.varianceOff.Text = "Off";
            this.varianceOff.UseVisualStyleBackColor = true;
            // 
            // sampVariance
            // 
            this.sampVariance.AutoSize = true;
            this.sampVariance.Location = new System.Drawing.Point(37, 44);
            this.sampVariance.Name = "sampVariance";
            this.sampVariance.Size = new System.Drawing.Size(60, 17);
            this.sampVariance.TabIndex = 1;
            this.sampVariance.TabStop = true;
            this.sampVariance.Text = "Sample";
            this.sampVariance.UseVisualStyleBackColor = true;
            // 
            // populVariance
            // 
            this.populVariance.AutoSize = true;
            this.populVariance.Location = new System.Drawing.Point(37, 20);
            this.populVariance.Name = "populVariance";
            this.populVariance.Size = new System.Drawing.Size(75, 17);
            this.populVariance.TabIndex = 0;
            this.populVariance.TabStop = true;
            this.populVariance.Text = "Popluation";
            this.populVariance.UseVisualStyleBackColor = true;
            // 
            // selectDirectory
            // 
            this.selectDirectory.Location = new System.Drawing.Point(33, 232);
            this.selectDirectory.Name = "selectDirectory";
            this.selectDirectory.Size = new System.Drawing.Size(189, 23);
            this.selectDirectory.TabIndex = 4;
            this.selectDirectory.Text = "Select EverQuest Directory";
            this.selectDirectory.UseVisualStyleBackColor = true;
            this.selectDirectory.Click += new System.EventHandler(this.selectDirectory_Click);
            // 
            // eqDirectory
            // 
            this.eqDirectory.AutoSize = true;
            this.eqDirectory.Location = new System.Drawing.Point(28, 190);
            this.eqDirectory.Name = "eqDirectory";
            this.eqDirectory.Size = new System.Drawing.Size(102, 13);
            this.eqDirectory.TabIndex = 5;
            this.eqDirectory.Text = "EverQuest Directory";
            // 
            // directoryPathTB
            // 
            this.directoryPathTB.Location = new System.Drawing.Point(33, 206);
            this.directoryPathTB.Name = "directoryPathTB";
            this.directoryPathTB.ReadOnly = true;
            this.directoryPathTB.Size = new System.Drawing.Size(366, 20);
            this.directoryPathTB.TabIndex = 2;
            // 
            // EverQuestDPSPlugin
            // 
            this.Controls.Add(this.eqDirectory);
            this.Controls.Add(this.selectDirectory);
            this.Controls.Add(this.directoryPathTB);
            this.Controls.Add(this.groupBox1);
            this.Name = "EverQuestDPSPlugin";
            this.Size = new System.Drawing.Size(818, 281);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        #endregion

        #region Class Members
        private FileSystemWatcher watcherForRaidRoster;
        TreeNode optionsNode = null;
        Label lblStatus;    // The status label that appears in ACT's Plugin tab
        delegate void matchParse(Match regexMatch);
        List<Tuple<Color, Regex>> regexTupleList;
        Regex selfCheck;
        Regex possesive;
        string settingsFile;
        SettingsSerializer xmlSettings;
        readonly string PluginSettingsFileName = $"Config{Path.DirectorySeparatorChar}ACT_EverQuest_English_Parser.config.xml";
        private GroupBox groupBox1;
        private RadioButton varianceOff;
        private RadioButton sampVariance;
        private RadioButton populVariance;
        private TextBox directoryPathTB;
        private Button selectDirectory;
        private Label eqDirectory;
        private MasterSwing chilled;
        private Match meleeMatch;
        readonly List<string> ignoreStringsForZoneParse = new List<string>()
                        {
                            "an area where levitation effects do not function",
                            "the Drunken Monkey stance adequately"
                        };
        FileSystemWatcher watcherForDebugFile;
        StreamReader sr;
        String dbgFilePath;
        bool readingLine = false;
        readonly string fileNameExpected = $"Logs{Path.DirectorySeparatorChar}dbg.txt";
        Regex zoneEnterRgx;
        #endregion

        /// <summary>
        /// Constructor that calls initialize component
        /// </summary>
        public EverQuestDPSPlugin()
        {
            InitializeComponent();
        }

               private void Watcher_CreatedForDebugFile(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType.HasFlag(WatcherChangeTypes.Created))
            {
                SetWatcherToDirectory();
            }
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType.HasFlag(WatcherChangeTypes.Changed) && !readingLine)
                ReadLineFromFileStreamAtPosition();
        }

        private void ReadLineFromFileStreamAtPosition()
        {
            string line;
            readingLine = true;
            while ((line = sr.ReadLine()) != null)
            {
                Match m = zoneEnterRgx.Match(line);
                if (m.Success && (ActGlobals.charName == m.Groups["characterEnteringZone"].Value))
                {
                    ActGlobals.oFormActMain.ChangeZone(m.Groups["ZoneName"].Value);
                }
            }
            readingLine = false;
        }

        private void DirectoryPathTxtBox_TextChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(directoryPathTB.Text))
            {
                SetWatcherToDirectory();
            }
            else
            {
                ChangeLblStatus("EverQuest install directory is missing from plugin settings.");
            }
            ChangeLblStatus($"dbg.txt log file changed to {dbgFilePath}.");
        }
        /// <summary>
        /// Once a file is created read the lines from the roster and include them on the Raid Allies list for the encounter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            if (ActGlobals.oFormActMain.ActiveZone.ActiveEncounter == null)
            {
                ChangeLblStatus("No active encounter and no raid allies will be tracked.");
                return;
            }
            ChangeLblStatus($"Reading {e.FullPath}");
            Regex raidAllyLineRegex = new Regex(Properties.EQDPSPlugin.raidAllyFormat);
            Regex filename = new Regex(Properties.EQDPSPlugin.raidAllyFileName);
            Match m = filename.Match(e.Name);
            if (e.ChangeType.HasFlag(WatcherChangeTypes.Created) && m.Success)
            {
                List<CombatantData> data = new List<CombatantData>();

                using (StreamReader sr = new StreamReader(new FileStream(e.FullPath, FileMode.Open)))
                {
                    String line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        Match raidLineMatch = raidAllyLineRegex.Match(line);
                        CombatantData combatantData = new CombatantData(raidLineMatch.Groups["playerName"].Value, ActGlobals.oFormActMain.ActiveZone.ActiveEncounter);

                        if (combatantData != null && combatantData.Name != ActGlobals.charName)
                            data.Add(combatantData);
                    }
                }

                ActGlobals.oFormActMain.ActiveZone.ActiveEncounter.SetAllies(data);
            }
        }

        private void pluginScreenSpaceAdd(TabPage screenSpace, Control control)
        {
            Action addControl = new Action(() => { screenSpace.Controls.Add(control); });
            if (screenSpace.InvokeRequired)
            {
                screenSpace.Invoke(addControl);
            }
            else
                addControl.Invoke();
        }

        /// <summary>
        /// Called by the ACT program to start the plugin initialization
        /// Calls regex initialization methods and check for update methods
        /// assigns methods to the delegates in ActGlobals class
        /// </summary>
        /// <param name="pluginScreenSpace"></param>
        /// <param name="pluginStatusText"></param>
        public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            Localization.EverQuestDPSPluginLocalization.EditLocalizations();
            settingsFile = Path.Combine(ActGlobals.oFormActMain.AppDataFolder.FullName, PluginSettingsFileName);
            lblStatus = pluginStatusText;   // Hand the status label's reference to our local var
            watcherForDebugFile = new FileSystemWatcher();
            watcherForRaidRoster = new FileSystemWatcher();

            pluginScreenSpaceAdd(pluginScreenSpace, this);

            this.Dock = DockStyle.Fill;

            int dcIndex = -1;   // Find the Data Correction node in the Options tab
            Action optionsTreeViewAdd = () =>
            {
                // Add our own node to the Data Correction node
                optionsNode = ActGlobals.oFormActMain.OptionsTreeView.Nodes[dcIndex].Nodes.Add($"{Properties.EQDPSPlugin.pluginName} Settings");
            };
            for (int i = 0; i < ActGlobals.oFormActMain.OptionsTreeView.Nodes.Count; i++)
            {
                if (ActGlobals.oFormActMain.OptionsTreeView.Nodes[i].Text.Equals("Data Correction"))
                    dcIndex = i;
            }
            if (dcIndex != -1)
            {
                if (ActGlobals.oFormActMain.OptionsTreeView.InvokeRequired)
                {
                    ActGlobals.oFormActMain.OptionsTreeView.Invoke(optionsTreeViewAdd);
                }
                else
                {
                    // Add our own node to the Data Correction node
                    optionsTreeViewAdd.Invoke();
                }
                Action optionsControlSetsAdd = () =>
                {
                    // Register our user control(this) to our newly create node path.  All controls added to the list will be laid out left to right, top to bottom
                    ActGlobals.oFormActMain.OptionsControlSets.Add($"Data Correction\\{Properties.EQDPSPlugin.pluginName}", new List<Control> { this });
                };
                if(ActGlobals.oFormActMain.InvokeRequired)
                {
                    ActGlobals.oFormActMain.Invoke(optionsControlSetsAdd);
                }
                else
                {
                    optionsControlSetsAdd.Invoke();
                }
                Label lblConfig = new Label
                {
                    AutoSize = true,
                    Text = $"Settings under the {Properties.EQDPSPlugin.pluginName} tab."
                };
                pluginScreenSpaceAdd(pluginScreenSpace, lblConfig);
            }
            
            xmlSettings = new SettingsSerializer(this); // Create a new settings serializer and pass it this instance
            LoadSettings();
            PopulateRegexNonCombat();
            PopulateRegexCombat();
            SetupEverQuestEnvironment();
            if (ActGlobals.oFormActMain.InvokeRequired)
            {
                ActGlobals.oFormActMain.Invoke(new Action(() =>
                {
                    ActGlobals.oFormActMain.GetDateTimeFromLog += new FormActMain.DateTimeLogParser(ParseDateTime);
                    ActGlobals.oFormActMain.BeforeLogLineRead += new LogLineEventDelegate(FormActMain_BeforeLogLineRead);
                }));
            }
            else
            {
                ActGlobals.oFormActMain.GetDateTimeFromLog += new FormActMain.DateTimeLogParser(ParseDateTime);
                ActGlobals.oFormActMain.BeforeLogLineRead += new LogLineEventDelegate(FormActMain_BeforeLogLineRead);
                ActGlobals.oFormActMain.OnLogLineRead += new LogLineEventDelegate(FormActMain_OnLogLineRead);
            }
             
            ActGlobals.oFormActMain.CharacterFileNameRegex = new Regex(Properties.EQDPSPlugin.fileNameForLog, RegexOptions.Compiled);
            ActGlobals.oFormActMain.ZoneChangeRegex = new Regex($@"\[(?:.+)\] {Properties.EQDPSPlugin.zoneChange}", RegexOptions.Compiled);
            ChangeLblStatus($"{Properties.EQDPSPlugin.pluginName} {Properties.EQDPSPlugin.pluginStarted}");
            
        }

        /// <summary>
        /// Removes methods from the delegates assigned during initialization
        /// attemps to save the settings and then update the plugin dock with status of the exit
        /// </summary>
        public void DeInitPlugin()
        {

            Action removeOption = () => {
                if (!(optionsNode == null))    // If we added our user control to the Options tab, remove it
                {
                    optionsNode.Remove();
                    ActGlobals.oFormActMain.OptionsControlSets.Remove($"Data Correction\\{Properties.EQDPSPlugin.pluginName}");
                }
                ActGlobals.oFormActMain.GetDateTimeFromLog -= ParseDateTime;
                ActGlobals.oFormActMain.BeforeLogLineRead -= FormActMain_BeforeLogLineRead;
                ActGlobals.oFormActMain.OnLogLineRead -= FormActMain_OnLogLineRead;
            };

            if (ActGlobals.oFormActMain.InvokeRequired)
                ActGlobals.oFormActMain.Invoke(removeOption);
            else
                removeOption.Invoke();
            SaveSettings();
            ChangeLblStatus($"{Properties.EQDPSPlugin.pluginName} {Properties.EQDPSPlugin.pluginExited}");
        }

        /// <summary>
        /// Loads settings file and attempts to assign values to the controls added in the method
        /// </summary>
        void LoadSettings()
        {
            xmlSettings.AddControlSetting(directoryPathTB.Name, directoryPathTB);
            xmlSettings.AddControlSetting(populVariance.Name, populVariance);
            xmlSettings.AddControlSetting(sampVariance.Name, sampVariance);
            xmlSettings.AddControlSetting(varianceOff.Name, varianceOff);
            if (File.Exists(settingsFile))
            {
                using (FileStream fs = new FileStream(settingsFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (XmlTextReader xReader = new XmlTextReader(fs))
                {
                    try
                    {
                        while (xReader.Read())
                        {
                            if (xReader.NodeType == XmlNodeType.Element)
                            {
                                if (xReader.LocalName == "SettingsSerializer")
                                    xmlSettings.ImportFromXml(xReader);
                            }
                        }
                    }
                    catch (ArgumentNullException ex)
                    {
                        ChangeLblStatus($"Argument Null for {ex.ParamName} with message: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        ChangeLblStatus($"With message: {ex.Message}");
                    }
                }
            }
            else
            {
                ChangeLblStatus($"{settingsFile} does not exist and no settings were loaded, first time loading {Properties.EQDPSPlugin.pluginName}?");
                SaveSettings();
            }
        }
        /// <summary>
        /// Saves the settings file usually called when there is a change in the settings, 
        /// a settings file doesn't exist during LoadSettings method call, 
        /// or during the exit of the plugin
        /// </summary>
        void SaveSettings()
        {
            try
            {
                using (FileStream fs = new FileStream(settingsFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (XmlTextWriter xWriter = new XmlTextWriter(fs, Encoding.UTF8))
                    {
                        xWriter.Formatting = Formatting.Indented;
                        xWriter.Indentation = 1;
                        xWriter.IndentChar = '\t';
                        xWriter.WriteStartDocument(true);
                        xWriter.WriteStartElement("Config");    // <Config>
                        xWriter.WriteStartElement("SettingsSerializer");    // <Config><SettingsSerializer>
                        xmlSettings.ExportToXml(xWriter);   // Fill the SettingsSerializer XML
                        xWriter.WriteEndElement();  // </SettingsSerializer>
                        xWriter.WriteEndElement();  // </Config>
                        xWriter.WriteEndDocument(); // Tie up loose ends (shouldn't be any)
                    }
                }
            }
            catch (Exception ex)
            {
                ActGlobals.oFormActMain.WriteExceptionLog(ex, "Failed to save file in entirety");
            }
        }
        /// <summary>
        /// Sets up the EverQuest environment with the standard information for the ACT application
        /// </summary>
        private void SetupEverQuestEnvironment()
        {
            CultureInfo usCulture = new CultureInfo("en-US");   // This is for SQL syntax; do not change
            ActGlobals.blockIsHit = true;
            EncounterData.ColumnDefs.Clear();
            //Do not change the SqlDataName while doing localization
            EncounterData.ColumnDefs.Add("EncId", new EncounterData.ColumnDef("EncId", false, "CHAR(8)", "EncId", (Data) => { return string.Empty; }, (Data) => { return Data.EncId; }));
            EncounterData.ColumnDefs.Add("Title", new EncounterData.ColumnDef("Title", true, "VARCHAR(64)", "Title", (Data) => { return Data.Title; }, (Data) => { return Data.Title; }));
            EncounterData.ColumnDefs.Add("StartTime", new EncounterData.ColumnDef("StartTime", true, "TIMESTAMP", "StartTime", (Data) => { return Data.StartTime == DateTime.MaxValue ? "--:--:--" : String.Format("{0} {1}", Data.StartTime.ToShortDateString(), Data.StartTime.ToLongTimeString()); }, (Data) => { return Data.StartTime == DateTime.MaxValue ? "0000-00-00 00:00:00" : Data.StartTime.ToString("u").TrimEnd(new char[] { 'Z' }); }));
            EncounterData.ColumnDefs.Add("EndTime", new EncounterData.ColumnDef("EndTime", true, "TIMESTAMP", "EndTime", (Data) => { return Data.EndTime == DateTime.MinValue ? "--:--:--" : Data.EndTime.ToString("T"); }, (Data) => { return Data.EndTime == DateTime.MinValue ? "0000-00-00 00:00:00" : Data.EndTime.ToString("u").TrimEnd(new char[] { 'Z' }); }));
            EncounterData.ColumnDefs.Add("Duration", new EncounterData.ColumnDef("Duration", true, "INT", "Duration", (Data) => { return Data.DurationS; }, (Data) => { return Data.Duration.TotalSeconds.ToString("0"); }));
            EncounterData.ColumnDefs.Add("Damage", new EncounterData.ColumnDef("Damage", true, "BIGINT", "Damage", (Data) => { return Data.Damage.ToString(); }, (Data) => { return Data.Damage.ToString(); }));
            EncounterData.ColumnDefs.Add("EncDPS", new EncounterData.ColumnDef("EncDPS", true, "DOUBLE", "EncDPS", (Data) => { return Data.DPS.ToString(); }, (Data) => { return Data.DPS.ToString(usCulture); }));
            EncounterData.ColumnDefs.Add("Zone", new EncounterData.ColumnDef("Zone", false, "VARCHAR(64)", "Zone", (Data) => { return Data.ZoneName; }, (Data) => { return Data.ZoneName; }));
            EncounterData.ColumnDefs.Add("Kills", new EncounterData.ColumnDef("Kills", true, "INT", "Kills", (Data) => { return Data.AlliedKills.ToString(); }, (Data) => { return Data.AlliedKills.ToString(); }));
            EncounterData.ColumnDefs.Add("Deaths", new EncounterData.ColumnDef("Deaths", true, "INT", "Deaths", (Data) => { return Data.AlliedDeaths.ToString(); }, (Data) => { return Data.AlliedDeaths.ToString(); }));

            EncounterData.ExportVariables.Clear();
            EncounterData.ExportVariables.Add("n", new EncounterData.TextExportFormatter("n", "New Line", "Formatting after this element will appear on a new line.", (Data, SelectiveAllies, Extra) => { return "\n"; }));
            EncounterData.ExportVariables.Add("t", new EncounterData.TextExportFormatter("t", "Tab Character", "Formatting after this element will appear in a relative column arrangement.  (The formatting example cannot display this properly)", (Data, SelectiveAllies, Extra) => { return "\t"; }));
            EncounterData.ExportVariables.Add("title", new EncounterData.TextExportFormatter("title", "Encounter Title", "The title of the completed encounter.  This may only be used in Allies formatting.", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "title", Extra); }));
            EncounterData.ExportVariables.Add("duration", new EncounterData.TextExportFormatter("duration", "Duration", "The duration of the combatant or the duration of the encounter, displayed as mm:ss", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "duration", Extra); }));
            EncounterData.ExportVariables.Add("DURATION", new EncounterData.TextExportFormatter("DURATION", "Short Duration", "The duration of the combatant or encounter displayed in whole seconds.", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "DURATION", Extra); }));
            EncounterData.ExportVariables.Add("damage", new EncounterData.TextExportFormatter("damage", "Damage", "The amount of damage from auto-attack, spells, CAs, etc done to other combatants.", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "damage", Extra); }));
            EncounterData.ExportVariables.Add("damage-m", new EncounterData.TextExportFormatter("damage-m", "Damage M", "Damage divided by 1,000,000 (with two decimal places)", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "damage-m", Extra); }));
            EncounterData.ExportVariables.Add("damage-*", new EncounterData.TextExportFormatter("damage-*", "Damage w/suffix", "Damage divided 1/K/M/B/T/Q (with two decimal places)", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "damage-*", Extra); }));
            EncounterData.ExportVariables.Add("DAMAGE-k", new EncounterData.TextExportFormatter("DAMAGE-k", "Short Damage K", "Damage divided by 1,000 (with no decimal places)", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "DAMAGE-k", Extra); }));
            EncounterData.ExportVariables.Add("DAMAGE-m", new EncounterData.TextExportFormatter("DAMAGE-m", "Short Damage M", "Damage divided by 1,000,000 (with no decimal places)", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "DAMAGE-m", Extra); }));
            EncounterData.ExportVariables.Add("DAMAGE-b", new EncounterData.TextExportFormatter("DAMAGE-b", "Short Damage B", "Damage divided by 1,000,000,000 (with no decimal places)", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "DAMAGE-b", Extra); }));
            EncounterData.ExportVariables.Add("DAMAGE-*", new EncounterData.TextExportFormatter("DAMAGE-*", "Short Damage w/suffix", "Damage divided by 1/K/M/B/T/Q (with no decimal places)", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "DAMAGE-*", Extra); }));
            EncounterData.ExportVariables.Add("dps", new EncounterData.TextExportFormatter("dps", "DPS", "The damage total of the combatant divided by their personal duration, formatted as 12.34", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "dps", Extra); }));
            EncounterData.ExportVariables.Add("dps-*", new EncounterData.TextExportFormatter("dps-*", "DPS w/suffix", "The damage total of the combatant divided by their personal duration, formatted as 12.34K", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "dps-*", Extra); }));
            EncounterData.ExportVariables.Add("DPS", new EncounterData.TextExportFormatter("DPS", "Short DPS", "The damage total of the combatatant divided by their personal duration, formatted as 12", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "DPS", Extra); }));
            EncounterData.ExportVariables.Add("encdps", new EncounterData.TextExportFormatter("encdps", "Encounter DPS", "The damage total of the combatant divided by the duration of the encounter, formatted as 12.34 -- This is more commonly used than DPS", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "encdps", Extra); }));
            EncounterData.ExportVariables.Add("encdps-*", new EncounterData.TextExportFormatter("encdps-*", "Encounter DPS w/suffix", "The damage total of the combatant divided by the duration of the encounter, formatted as 12.34 -- This is more commonly used than DPS", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "encdps-*", Extra); }));
            EncounterData.ExportVariables.Add("ENCDPS", new EncounterData.TextExportFormatter("ENCDPS", "Short Encounter DPS", "The damage total of the combatant divided by the duration of the encounter, formatted as 12 -- This is more commonly used than DPS", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "ENCDPS", Extra); }));
            EncounterData.ExportVariables.Add("hits", new EncounterData.TextExportFormatter("hits", "Hits", "The number of attack attempts that produced damage.  IE a spell successfully doing damage.", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "hits", Extra); }));
            EncounterData.ExportVariables.Add("crithits", new EncounterData.TextExportFormatter("crithits", "Critical Hit Count", "The number of damaging attacks that were critical.", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "crithits", Extra); }));
            EncounterData.ExportVariables.Add("crithit%", new EncounterData.TextExportFormatter("crithit%", "Critical Hit Percentage", "The percentage of damaging attacks that were critical.", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "crithit%", Extra); }));
            EncounterData.ExportVariables.Add("misses", new EncounterData.TextExportFormatter("misses", "Misses", "The number of auto-attacks or CAs that produced a miss message.", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "misses", Extra); }));
            EncounterData.ExportVariables.Add("hitfailed", new EncounterData.TextExportFormatter("hitfailed", "Other Avoid", "Any type of failed attack that was not a miss.  This includes resists, reflects, blocks, dodging, etc.", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "hitfailed", Extra); }));
            EncounterData.ExportVariables.Add("swings", new EncounterData.TextExportFormatter("swings", "Swings (Attacks)", "The number of attack attempts.  This includes any auto-attacks or abilities, also including resisted abilities that do no damage.", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "swings", Extra); }));
            EncounterData.ExportVariables.Add("tohit", new EncounterData.TextExportFormatter("tohit", "To Hit %", "The percentage of hits to swings as 12.34", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "tohit", Extra); }));
            EncounterData.ExportVariables.Add("TOHIT", new EncounterData.TextExportFormatter("TOHIT", "Short To Hit %", "The percentage of hits to swings as 12", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "TOHIT", Extra); }));
            EncounterData.ExportVariables.Add("maxhit", new EncounterData.TextExportFormatter("maxhit", "Highest Hit", "The highest single damaging hit formatted as [Combatant-]SkillName-Damage#", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "maxhit", Extra); }));
            EncounterData.ExportVariables.Add("MAXHIT", new EncounterData.TextExportFormatter("MAXHIT", "Short Highest Hit", "The highest single damaging hit formatted as [Combatant-]Damage#", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "MAXHIT", Extra); }));
            EncounterData.ExportVariables.Add("maxhit-*", new EncounterData.TextExportFormatter("maxhit-*", "Highest Hit w/ suffix", "MaxHit divided by 1/K/M/B/T/Q", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "maxhit-*", Extra); }));
            EncounterData.ExportVariables.Add("MAXHIT-*", new EncounterData.TextExportFormatter("MAXHIT-*", "Short Highest Hit w/ suffix", "Short MaxHit divided by 1/K/M/B/T/Q", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "MAXHIT-*", Extra); }));
            EncounterData.ExportVariables.Add("healed", new EncounterData.TextExportFormatter("healed", "Healed", "The numerical total of all heals, wards or similar sourced from this combatant.", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "healed", Extra); }));
            //EncounterData.ExportVariables.Add("enchps", new EncounterData.TextExportFormatter("enchps", "Encounter HPS", "The healing total of the combatant divided by the duration of the encounter, formatted as 12.34", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "enchps", Extra); }));
            //EncounterData.ExportVariables.Add("enchps-*", new EncounterData.TextExportFormatter("enchps-*", "Encounter HPS w/suffix", "Encounter HPS divided by 1/K/M/B/T/Q", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "enchps-*", Extra); }));
            //EncounterData.ExportVariables.Add("ENCHPS", new EncounterData.TextExportFormatter("ENCHPS", "Short Encounter HPS", "The healing total of the combatant divided by the duration of the encounter, formatted as 12", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "ENCHPS", Extra); }));
            EncounterData.ExportVariables.Add("heals", new EncounterData.TextExportFormatter("heals", "Heal Count", "The total number of heals from this combatant.", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "heals", Extra); }));
            EncounterData.ExportVariables.Add("critheals", new EncounterData.TextExportFormatter("critheals", "Critical Heal Count", "The number of heals that were critical.", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "critheals", Extra); }));
            EncounterData.ExportVariables.Add("critheal%", new EncounterData.TextExportFormatter("critheal%", "Critical Heal Percentage", "The percentage of heals that were critical.", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "critheal%", Extra); }));
            EncounterData.ExportVariables.Add("cures", new EncounterData.TextExportFormatter("cures", "Cure or Dispel Count", "The total number of times the combatant cured or dispelled", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "cures", Extra); }));
            EncounterData.ExportVariables.Add("maxheal", new EncounterData.TextExportFormatter("maxheal", "Highest Heal", "The highest single healing amount formatted as [Combatant-]SkillName-Healing#", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "maxheal", Extra); }));
            EncounterData.ExportVariables.Add("MAXHEAL", new EncounterData.TextExportFormatter("MAXHEAL", "Short Highest Heal", "The highest single healing amount formatted as [Combatant-]Healing#", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "MAXHEAL", Extra); }));
            EncounterData.ExportVariables.Add("maxhealward", new EncounterData.TextExportFormatter("maxhealward", "Highest Heal/Ward", "The highest single healing/warding amount formatted as [Combatant-]SkillName-Healing#", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "maxhealward", Extra); }));
            EncounterData.ExportVariables.Add("MAXHEALWARD", new EncounterData.TextExportFormatter("MAXHEALWARD", "Short Highest Heal/Ward", "The highest single healing/warding amount formatted as [Combatant-]Healing#", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "MAXHEALWARD", Extra); }));
            EncounterData.ExportVariables.Add("maxheal-*", new EncounterData.TextExportFormatter("maxheal-*", "Highest Heal w/ suffix", "Highest Heal divided by 1/K/M/B/T/Q", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "maxheal-*", Extra); }));
            EncounterData.ExportVariables.Add("MAXHEAL-*", new EncounterData.TextExportFormatter("MAXHEAL-*", "Short Highest Heal w/ suffix", "Short Highest Heal divided by 1/K/M/B/T/Q", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "MAXHEAL-*", Extra); }));
            EncounterData.ExportVariables.Add("maxhealward-*", new EncounterData.TextExportFormatter("maxhealward-*", "Highest Heal/Ward w/ suffix", "Highest Heal/Ward divided by 1/K/M/B/T/Q", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "maxhealward-*", Extra); }));
            EncounterData.ExportVariables.Add("MAXHEALWARD-*", new EncounterData.TextExportFormatter("MAXHEALWARD-*", "Short Highest Heal/Ward w/ suffix", "Short Highest Heal/Ward divided by 1/K/M/B/T/Q", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "MAXHEALWARD-*", Extra); }));
            EncounterData.ExportVariables.Add("damagetaken", new EncounterData.TextExportFormatter("damagetaken", "Damage Received", "The total amount of damage this combatant received.", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "damagetaken", Extra); }));
            EncounterData.ExportVariables.Add("damagetaken-*", new EncounterData.TextExportFormatter("damagetaken-*", "Damage Received w/suffix", "Damage Received divided by 1/K/M/B/T/Q", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "damagetaken-*", Extra); }));
            EncounterData.ExportVariables.Add("healstaken", new EncounterData.TextExportFormatter("healstaken", "Healing Received", "The total amount of healing this combatant received.", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "healstaken", Extra); }));
            EncounterData.ExportVariables.Add("healstaken-*", new EncounterData.TextExportFormatter("healstaken-*", "Healing Received w/suffix", "Healing Received divided by 1/K/M/B/T/Q", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "healstaken-*", Extra); }));
            EncounterData.ExportVariables.Add("kills", new EncounterData.TextExportFormatter("kills", "Killing Blows", "The total number of times this character landed a killing blow.", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "kills", Extra); }));
            EncounterData.ExportVariables.Add("deaths", new EncounterData.TextExportFormatter("deaths", "Deaths", "The total number of times this character was killed by another.", (Data, SelectiveAllies, Extra) => { return EncounterFormatSwitch(Data, SelectiveAllies, "deaths", Extra); }));

            CombatantData.ColumnDefs.Clear();
            CombatantData.ColumnDefs.Add("EncId", new CombatantData.ColumnDef("EncId", false, "CHAR(8)", "EncId", (Data) => { return string.Empty; }, (Data) => { return Data.Parent.EncId; }, (Left, Right) => { return 0; }));
            CombatantData.ColumnDefs.Add("Ally", new CombatantData.ColumnDef("Ally", false, "CHAR(1)", "Ally", (Data) => { return Data.Parent.GetAllies().Contains(Data).ToString(); }, (Data) => { return Data.Parent.GetAllies().Contains(Data) ? "T" : "F"; }, (Left, Right) => { return Left.Parent.GetAllies().Contains(Left).CompareTo(Right.Parent.GetAllies().Contains(Right)); }));
            CombatantData.ColumnDefs.Add("Name", new CombatantData.ColumnDef("Name", true, "VARCHAR(64)", "Name", (Data) => { return Data.Name; }, (Data) => { return Data.Name; }, (Left, Right) => { return Left.Name.CompareTo(Right.Name); }));
            CombatantData.ColumnDefs.Add("StartTime", new CombatantData.ColumnDef("StartTime", true, "TIMESTAMP", "StartTime", (Data) => { return Data.StartTime == DateTime.MaxValue ? "--:--:--" : Data.StartTime.ToString("T"); }, (Data) => { return Data.StartTime == DateTime.MaxValue ? "0000-00-00 00:00:00" : Data.StartTime.ToString("u").TrimEnd(new char[] { 'Z' }); }, (Left, Right) => { return Left.StartTime.CompareTo(Right.StartTime); }));
            CombatantData.ColumnDefs.Add("EndTime", new CombatantData.ColumnDef("EndTime", false, "TIMESTAMP", "EndTime", (Data) => { return Data.EndTime == DateTime.MinValue ? "--:--:--" : Data.EndTime.ToString("T"); }, (Data) => { return Data.EndTime == DateTime.MinValue ? "0000-00-00 00:00:00" : Data.EndTime.ToString("u").TrimEnd(new char[] { 'Z' }); }, (Left, Right) => { return Left.EndTime.CompareTo(Right.EndTime); }));
            CombatantData.ColumnDefs.Add("Duration", new CombatantData.ColumnDef("Duration", true, "INT", "Duration", (Data) => { return Data.DurationS; }, (Data) => { return Data.Duration.TotalSeconds.ToString("0"); }, (Left, Right) => { return Left.Duration.CompareTo(Right.Duration); }));
            CombatantData.ColumnDefs.Add("Damage", new CombatantData.ColumnDef("Damage", true, "BIGINT", "Damage", (Data) => { return Data.Damage.ToString(); }, (Data) => { return Data.Damage.ToString(); }, (Left, Right) => { return Left.Damage.CompareTo(Right.Damage); }));
            CombatantData.ColumnDefs.Add("Damage%", new CombatantData.ColumnDef("Damage%", true, "VARCHAR(4)", "DamagePerc", (Data) => { return Data.DamagePercent; }, (Data) => { return Data.DamagePercent; }, (Left, Right) => { return Left.Damage.CompareTo(Right.Damage); }));
            CombatantData.ColumnDefs.Add("Kills", new CombatantData.ColumnDef("Kills", false, "INT", "Kills", (Data) => { return Data.Kills.ToString(); }, (Data) => { return Data.Kills.ToString(); }, (Left, Right) => { return Left.Kills.CompareTo(Right.Kills); }));
            CombatantData.ColumnDefs.Add("Healed", new CombatantData.ColumnDef("Healed", false, "BIGINT", "Healed", (Data) => { return Data.Healed.ToString(); }, (Data) => { return Data.Healed.ToString(); }, (Left, Right) => { return Left.Healed.CompareTo(Right.Healed); }));
            CombatantData.ColumnDefs.Add("Healed%", new CombatantData.ColumnDef("Healed%", false, "VARCHAR(4)", "HealedPerc", (Data) => { return Data.HealedPercent; }, (Data) => { return Data.HealedPercent; }, (Left, Right) => { return Left.Healed.CompareTo(Right.Healed); }));
            CombatantData.ColumnDefs.Add("CritHeals", new CombatantData.ColumnDef("CritHeals", false, "INT", "CritHeals", (Data) => { return Data.CritHeals.ToString(); }, (Data) => { return Data.CritHeals.ToString(); }, (Left, Right) => { return Left.CritHeals.CompareTo(Right.CritHeals); }));
            CombatantData.ColumnDefs.Add("Heals", new CombatantData.ColumnDef("Heals", false, "INT", "Heals", (Data) => { return Data.Heals.ToString(); }, (Data) => { return Data.Heals.ToString(); }, (Left, Right) => { return Left.Heals.CompareTo(Right.Heals); }));
            CombatantData.ColumnDefs.Add("Cures", new CombatantData.ColumnDef("Cures", false, "INT", "CureDispels", (Data) => { return Data.CureDispels.ToString(); }, (Data) => { return Data.CureDispels.ToString(); }, (Left, Right) => { return Left.CureDispels.CompareTo(Right.CureDispels); }));
            //CombatantData.ColumnDefs.Add("PowerDrain", new CombatantData.ColumnDef("PowerDrain", true, "BIGINT", "PowerDrain", (Data) => { return Data.PowerDamage.ToString(GetIntCommas()); }, (Data) => { return Data.PowerDamage.ToString(); }, (Left, Right) => { return Left.PowerDamage.CompareTo(Right.PowerDamage); }));
            //CombatantData.ColumnDefs.Add("PowerReplenish", new CombatantData.ColumnDef("PowerReplenish", false, "BIGINT", "PowerReplenish", (Data) => { return Data.PowerReplenish.ToString(GetIntCommas()); }, (Data) => { return Data.PowerReplenish.ToString(); }, (Left, Right) => { return Left.PowerReplenish.CompareTo(Right.PowerReplenish); }));
            CombatantData.ColumnDefs.Add("DPS", new CombatantData.ColumnDef("DPS", false, "DOUBLE", "DPS", (Data) => { return Data.DPS.ToString(); }, (Data) => { return Data.DPS.ToString(usCulture); }, (Left, Right) => { return Left.DPS.CompareTo(Right.DPS); }));
            CombatantData.ColumnDefs.Add("EncDPS", new CombatantData.ColumnDef("EncDPS", true, "DOUBLE", "EncDPS", (Data) => { return Data.EncDPS.ToString(); }, (Data) => { return Data.EncDPS.ToString(usCulture); }, (Left, Right) => { return Left.Damage.CompareTo(Right.Damage); }));
            //CombatantData.ColumnDefs.Add("EncHPS", new CombatantData.ColumnDef("EncHPS", true, "DOUBLE", "EncHPS", (Data) => { return Data.EncHPS.ToString(); }, (Data) => { return Data.EncHPS.ToString(usCulture); }, (Left, Right) => { return Left.Healed.CompareTo(Right.Healed); }));
            CombatantData.ColumnDefs.Add("Hits", new CombatantData.ColumnDef("Hits", false, "INT", "Hits", (Data) => { return Data.Hits.ToString(); }, (Data) => { return Data.Hits.ToString(); }, (Left, Right) => { return Left.Hits.CompareTo(Right.Hits); }));
            CombatantData.ColumnDefs.Add("CritHits", new CombatantData.ColumnDef("CritHits", false, "INT", "CritHits", (Data) => { return Data.CritHits.ToString(); }, (Data) => { return Data.CritHits.ToString(); }, (Left, Right) => { return Left.CritHits.CompareTo(Right.CritHits); }));
            CombatantData.ColumnDefs.Add("Avoids", new CombatantData.ColumnDef("Avoids", false, "INT", "Blocked", (Data) => { return Data.Blocked.ToString(); }, (Data) => { return Data.Blocked.ToString(); }, (Left, Right) => { return Left.Blocked.CompareTo(Right.Blocked); }));
            CombatantData.ColumnDefs.Add("Misses", new CombatantData.ColumnDef("Misses", false, "INT", "Misses", (Data) => { return Data.Misses.ToString(); }, (Data) => { return Data.Misses.ToString(); }, (Left, Right) => { return Left.Misses.CompareTo(Right.Misses); }));
            CombatantData.ColumnDefs.Add("Swings", new CombatantData.ColumnDef("Swings", false, "INT", "Swings", (Data) => { return Data.Swings.ToString(); }, (Data) => { return Data.Swings.ToString(); }, (Left, Right) => { return Left.Swings.CompareTo(Right.Swings); }));
            CombatantData.ColumnDefs.Add("HealingTaken", new CombatantData.ColumnDef("HealingTaken", false, "BIGINT", "HealsTaken", (Data) => { return Data.HealsTaken.ToString(); }, (Data) => { return Data.HealsTaken.ToString(); }, (Left, Right) => { return Left.HealsTaken.CompareTo(Right.HealsTaken); }));
            CombatantData.ColumnDefs.Add("DamageTaken", new CombatantData.ColumnDef("DamageTaken", true, "BIGINT", "DamageTaken", (Data) => { return Data.DamageTaken.ToString(); }, (Data) => { return Data.DamageTaken.ToString(); }, (Left, Right) => { return Left.DamageTaken.CompareTo(Right.DamageTaken); }));
            CombatantData.ColumnDefs.Add("Deaths", new CombatantData.ColumnDef("Deaths", true, "INT", "Deaths", (Data) => { return Data.Deaths.ToString(); }, (Data) => { return Data.Deaths.ToString(); }, (Left, Right) => { return Left.Deaths.CompareTo(Right.Deaths); }));
            CombatantData.ColumnDefs.Add("ToHit%", new CombatantData.ColumnDef("ToHit%", false, "FLOAT", "ToHit", (Data) => { return Data.ToHit.ToString(); }, (Data) => { return Data.ToHit.ToString(usCulture); }, (Left, Right) => { return Left.ToHit.CompareTo(Right.ToHit); }));
            CombatantData.ColumnDefs.Add("CritDam%", new CombatantData.ColumnDef("CritDam%", false, "VARCHAR(8)", "CritDamPerc", (Data) => { return Data.CritDamPerc.ToString("0'%"); }, (Data) => { return Data.CritDamPerc.ToString("0'%"); }, (Left, Right) => { return Left.CritDamPerc.CompareTo(Right.CritDamPerc); }));
            CombatantData.ColumnDefs.Add("CritHeal%", new CombatantData.ColumnDef("CritHeal%", false, "VARCHAR(8)", "CritHealPerc", (Data) => { return Data.CritHealPerc.ToString("0'%"); }, (Data) => { return Data.CritHealPerc.ToString("0'%"); }, (Left, Right) => { return Left.CritHealPerc.CompareTo(Right.CritHealPerc); }));
            CombatantData.ColumnDefs.Add("CritTypes", new CombatantData.ColumnDef("CritTypes", true, "VARCHAR(32)", "CritTypes", CombatantDataGetCritTypes, CombatantDataGetCritTypes, (Left, Right) => { return CombatantDataGetCritTypes(Left).CompareTo(CombatantDataGetCritTypes(Right)); }));

            CombatantData.ColumnDefs["Damage"].GetCellForeColor = (Data) => { return Color.DarkRed; };
            CombatantData.ColumnDefs["Damage%"].GetCellForeColor = (Data) => { return Color.DarkRed; };
            CombatantData.ColumnDefs["Healed"].GetCellForeColor = (Data) => { return Color.DarkBlue; };
            CombatantData.ColumnDefs["Healed%"].GetCellForeColor = (Data) => { return Color.DarkBlue; };
            CombatantData.ColumnDefs["DPS"].GetCellForeColor = (Data) => { return Color.DarkRed; };
            CombatantData.ColumnDefs["DamageTaken"].GetCellForeColor = (Data) => { return Color.DarkOrange; };

            CombatantData.OutgoingDamageTypeDataObjects = new Dictionary<string, CombatantData.DamageTypeDef>
        {
            {"Auto-Attack (Out)", new CombatantData.DamageTypeDef("Auto-Attack (Out)", -1, Color.DarkGoldenrod)},
            {"Skill/Ability (Out)", new CombatantData.DamageTypeDef("Skill/Ability (Out)", -1, Color.DarkOrange)},
            {"Outgoing Damage", new CombatantData.DamageTypeDef("Outgoing Damage", 0, Color.Orange)},
            {"Direct Damage Spell (Out)", new CombatantData.DamageTypeDef("Direct Damage Spell (Out)", -1, Color.LightCyan) },
            {"Damage Over Time Spell (Out)", new CombatantData.DamageTypeDef("Damage Over Time Spell (Out)", -1, Color.ForestGreen) },
            {"Bane (Out)", new CombatantData.DamageTypeDef("Bane (Out)", -1, Color.LightGreen) },
                {"Damage Shield (Out)", new CombatantData.DamageTypeDef("Damage Shield (Out)", -1, Color.Brown) },
            {"Instant Healed (Out)", new CombatantData.DamageTypeDef("Instant Healed (Out)", 1, Color.Blue)},
            {"Heal Over Time (Out)", new CombatantData.DamageTypeDef("Heal Over Time (Out)", 1, Color.DarkBlue)},
            {"All Outgoing (Ref)", new CombatantData.DamageTypeDef("All Outgoing (Ref)", 0, Color.Black)}
        };
            CombatantData.IncomingDamageTypeDataObjects = new Dictionary<string, CombatantData.DamageTypeDef>
        {
            {"Incoming Damage", new CombatantData.DamageTypeDef("Incoming Damage", -1, Color.Red)},
            {"Incoming NonMelee Damage", new CombatantData.DamageTypeDef("Incoming NonMelee Damage", -1 , Color.DarkRed) },
            {"Direct Damage Spell (Inc)", new CombatantData.DamageTypeDef("Direct Damage Spell (Inc)", -1, Color.LightCyan) },
            {"Damage Over Time Spell (Inc)", new CombatantData.DamageTypeDef("Damage Over Time Spell (Inc)", -1, Color.Orchid) },
            {"Damage Shield (Inc)", new CombatantData.DamageTypeDef("Damage Shield (Inc)", -1, Color.Brown) },
            {"Instant Healed (Inc)",new CombatantData.DamageTypeDef("Instant Healed (Inc)", 1, Color.LimeGreen)},
            {"Heal Over Time (Inc)",new CombatantData.DamageTypeDef("Heal Over Time (Inc)", 1, Color.DarkGreen)},
            {"All Incoming (Ref)",new CombatantData.DamageTypeDef("All Incoming (Ref)", 0, Color.Black)}
        };
            CombatantData.SwingTypeToDamageTypeDataLinksOutgoing = new SortedDictionary<int, List<string>>
        {
            {EverQuestSwingType.Melee.GetEverQuestSwingTypeExtensionIntValue(), new List<string> { "Auto-Attack (Out)", "Outgoing Damage" } },
            {EverQuestSwingType.NonMelee.GetEverQuestSwingTypeExtensionIntValue(), new List<string> { "Skill/Ability (Out)", "Outgoing Damage" } },
            {(EverQuestSwingType.Spell | EverQuestSwingType.Instant).GetEverQuestSwingTypeExtensionIntValue(), new List<string> { "Direct Damage Spell (Out)" , "Outgoing Damage"} },
            {(EverQuestSwingType.Spell | EverQuestSwingType.OverTime).GetEverQuestSwingTypeExtensionIntValue(), new List<string>{"Damage Over Time Spell (Out)", "Outgoing Damage"} },
            {(EverQuestSwingType.Healing | EverQuestSwingType.Instant).GetEverQuestSwingTypeExtensionIntValue(), new List<string> { "Instant Healed (Out)" } },
            {(EverQuestSwingType.Healing | EverQuestSwingType.OverTime).GetEverQuestSwingTypeExtensionIntValue(), new List<string> { "Heal Over Time (Out)" } },
                {EverQuestSwingType.DamageShield.GetEverQuestSwingTypeExtensionIntValue(), new List<string> { "Damage Shield (Out)"} },
        };
            CombatantData.SwingTypeToDamageTypeDataLinksIncoming = new SortedDictionary<int, List<string>>
        {
            {EverQuestSwingType.Melee.GetEverQuestSwingTypeExtensionIntValue(), new List<string> { "Incoming Damage" } },
            {EverQuestSwingType.NonMelee.GetEverQuestSwingTypeExtensionIntValue(), new List<string> { "Incoming NonMelee Damage", "Incoming Damage" } },
            {(EverQuestSwingType.Spell | EverQuestSwingType.Instant).GetEverQuestSwingTypeExtensionIntValue(), new List<string> { "Direct Damage Spell (Inc)", "Incoming Damage" } },
            {(EverQuestSwingType.Healing | EverQuestSwingType.Instant).GetEverQuestSwingTypeExtensionIntValue(), new List<string> { "Instant Healed (Inc)" } },
            {(EverQuestSwingType.Spell | EverQuestSwingType.OverTime).GetEverQuestSwingTypeExtensionIntValue(), new List<string> {"Damage Over Time Spell (Inc)", "Incoming Damage" } },
            {(EverQuestSwingType.Healing | EverQuestSwingType.OverTime).GetEverQuestSwingTypeExtensionIntValue(), new List<string> { "Heal Over Time (Inc)" } },
                {EverQuestSwingType.DamageShield.GetEverQuestSwingTypeExtensionIntValue(), new List<string> { "Damage Shield (Inc)"} },
        };

            CombatantData.DamageSwingTypes = new List<int> {
                EverQuestSwingType.Melee.GetEverQuestSwingTypeExtensionIntValue(),
                EverQuestSwingType.NonMelee.GetEverQuestSwingTypeExtensionIntValue(),
                (EverQuestSwingType.Spell | EverQuestSwingType.Instant).GetEverQuestSwingTypeExtensionIntValue(),
                (EverQuestSwingType.Spell | EverQuestSwingType.OverTime).GetEverQuestSwingTypeExtensionIntValue(),
                EverQuestSwingType.DamageShield.GetEverQuestSwingTypeExtensionIntValue(),
                EverQuestSwingType.Bane.GetEverQuestSwingTypeExtensionIntValue(),
            };
            CombatantData.HealingSwingTypes = new List<int> {
                (EverQuestSwingType.Healing | EverQuestSwingType.Instant).GetEverQuestSwingTypeExtensionIntValue(),
                (EverQuestSwingType.Healing | EverQuestSwingType.OverTime).GetEverQuestSwingTypeExtensionIntValue(),
            };

            CombatantData.ExportVariables.Clear();
            CombatantData.ExportVariables.Add("n", new CombatantData.TextExportFormatter("n", "New Line", "Formatting after this element will appear on a new line.", (Data, Extra) => { return "\n"; }));
            CombatantData.ExportVariables.Add("t", new CombatantData.TextExportFormatter("t", "Tab Character", "Formatting after this element will appear in a relative column arrangement.  (The formatting example cannot display this properly)", (Data, Extra) => { return "\t"; }));
            CombatantData.ExportVariables.Add("name", new CombatantData.TextExportFormatter("name", "Name", "The combatant's name.", (Data, Extra) => { return CombatantFormatSwitch(Data, "name", Extra); }));
            CombatantData.ExportVariables.Add("NAME", new CombatantData.TextExportFormatter("NAME", "Short Name", "The combatant's name shortened to a number of characters after a colon, like: \"NAME:5\"", (Data, Extra) => { return CombatantFormatSwitch(Data, "NAME", Extra); }));
            CombatantData.ExportVariables.Add("duration", new CombatantData.TextExportFormatter("duration", "Duration", "The duration of the combatant or the duration of the encounter, displayed as mm:ss", (Data, Extra) => { return CombatantFormatSwitch(Data, "duration", Extra); }));
            CombatantData.ExportVariables.Add("DURATION", new CombatantData.TextExportFormatter("DURATION", "Short Duration", "The duration of the combatant or encounter displayed in whole seconds.", (Data, Extra) => { return CombatantFormatSwitch(Data, "DURATION", Extra); }));
            CombatantData.ExportVariables.Add("damage", new CombatantData.TextExportFormatter("damage", "Damage", "The amount of damage from auto-attack, spells, CAs, etc done to other combatants.", (Data, Extra) => { return CombatantFormatSwitch(Data, "damage", Extra); }));
            CombatantData.ExportVariables.Add("damage-m", new CombatantData.TextExportFormatter("damage-m", "Damage M", "Damage divided by 1,000,000 (with two decimal places)", (Data, Extra) => { return CombatantFormatSwitch(Data, "damage-m", Extra); }));
            CombatantData.ExportVariables.Add("damage-b", new CombatantData.TextExportFormatter("damage-b", "Damage B", "Damage divided by 1,000,000,000 (with two decimal places)", (Data, Extra) => { return CombatantFormatSwitch(Data, "damage-b", Extra); }));
            CombatantData.ExportVariables.Add("damage-*", new CombatantData.TextExportFormatter("damage-*", "Damage w/suffix", "Damage divided by 1/K/M/B/T/Q (with one decimal places)", (Data, Extra) => { return CombatantFormatSwitch(Data, "damage-*", Extra); }));
            CombatantData.ExportVariables.Add("DAMAGE-k", new CombatantData.TextExportFormatter("DAMAGE-k", "Short Damage K", "Damage divided by 1,000 (with no decimal places)", (Data, Extra) => { return CombatantFormatSwitch(Data, "DAMAGE-k", Extra); }));
            CombatantData.ExportVariables.Add("DAMAGE-m", new CombatantData.TextExportFormatter("DAMAGE-m", "Short Damage M", "Damage divided by 1,000,000 (with no decimal places)", (Data, Extra) => { return CombatantFormatSwitch(Data, "DAMAGE-m", Extra); }));
            CombatantData.ExportVariables.Add("DAMAGE-b", new CombatantData.TextExportFormatter("DAMAGE-b", "Short Damage B", "Damage divided by 1,000,000,000 (with no decimal places)", (Data, Extra) => { return CombatantFormatSwitch(Data, "DAMAGE-b", Extra); }));
            CombatantData.ExportVariables.Add("DAMAGE-*", new CombatantData.TextExportFormatter("DAMAGE-*", "Short Damage w/suffix", "Damage divided by 1/K/M/B/T/Q (with no decimal places)", (Data, Extra) => { return CombatantFormatSwitch(Data, "DAMAGE-*", Extra); }));
            CombatantData.ExportVariables.Add("damage%", new CombatantData.TextExportFormatter("damage%", "Damage %", "This value represents the percent share of all damage done by allies in this encounter.", (Data, Extra) => { return CombatantFormatSwitch(Data, "damage%", Extra); }));
            CombatantData.ExportVariables.Add("dps", new CombatantData.TextExportFormatter("dps", "DPS", "The damage total of the combatant divided by their personal duration, formatted as 12.34", (Data, Extra) => { return CombatantFormatSwitch(Data, "dps", Extra); }));
            CombatantData.ExportVariables.Add("dps-*", new CombatantData.TextExportFormatter("dps-*", "DPS w/suffix", "The damage total of the combatant divided by their personal duration, formatted as 12.34K", (Data, Extra) => { return CombatantFormatSwitch(Data, "dps-*", Extra); }));
            CombatantData.ExportVariables.Add("DPS", new CombatantData.TextExportFormatter("DPS", "Short DPS", "The damage total of the combatatant divided by their personal duration, formatted as 12K", (Data, Extra) => { return CombatantFormatSwitch(Data, "DPS", Extra); }));
            CombatantData.ExportVariables.Add("DPS-k", new CombatantData.TextExportFormatter("DPS-k", "Short DPS K", "Short DPS divided by 1,000 (with no decimal places)", (Data, Extra) => { return CombatantFormatSwitch(Data, "DPS-k", Extra); }));
            CombatantData.ExportVariables.Add("DPS-m", new CombatantData.TextExportFormatter("DPS-m", "Short DPS M", "Short DPS divided by 1,000,000 (with no decimal places)", (Data, Extra) => { return CombatantFormatSwitch(Data, "DPS-m", Extra); }));
            CombatantData.ExportVariables.Add("DPS-*", new CombatantData.TextExportFormatter("DPS-*", "Short DPS w/suffix", "Short DPS divided by 1/K/M/B/T/Q (with no decimal places)", (Data, Extra) => { return CombatantFormatSwitch(Data, "DPS-*", Extra); }));
            CombatantData.ExportVariables.Add("encdps", new CombatantData.TextExportFormatter("encdps", "Encounter DPS", "The damage total of the combatant divided by the duration of the encounter, formatted as 12.34 -- This is more commonly used than DPS", (Data, Extra) => { return CombatantFormatSwitch(Data, "encdps", Extra); }));
            CombatantData.ExportVariables.Add("encdps-*", new CombatantData.TextExportFormatter("encdps-*", "Encounter DPS w/suffix", "The damage total of the combatant divided by the duration of the encounter, formatted as 12.34 -- This is more commonly used than DPS", (Data, Extra) => { return CombatantFormatSwitch(Data, "encdps-*", Extra); }));
            CombatantData.ExportVariables.Add("ENCDPS", new CombatantData.TextExportFormatter("ENCDPS", "Short Encounter DPS", "The damage total of the combatant divided by the duration of the encounter, formatted as 12 -- This is more commonly used than DPS", (Data, Extra) => { return CombatantFormatSwitch(Data, "ENCDPS", Extra); }));
            CombatantData.ExportVariables.Add("ENCDPS-k", new CombatantData.TextExportFormatter("ENCDPS-k", "Short Encounter DPS K", "Short Encounter DPS divided by 1,000 (with no decimal places)", (Data, Extra) => { return CombatantFormatSwitch(Data, "ENCDPS-k", Extra); }));
            CombatantData.ExportVariables.Add("ENCDPS-m", new CombatantData.TextExportFormatter("ENCDPS-m", "Short Encounter DPS M", "Short Encounter DPS divided by 1,000,000 (with no decimal places)", (Data, Extra) => { return CombatantFormatSwitch(Data, "ENCDPS-m", Extra); }));
            CombatantData.ExportVariables.Add("ENCDPS-*", new CombatantData.TextExportFormatter("ENCDPS-*", "Short Encounter DPS w/suffix", "Short Encounter DPS divided by 1/K/M/B/T/Q (with no decimal places)", (Data, Extra) => { return CombatantFormatSwitch(Data, "ENCDPS-*", Extra); }));
            CombatantData.ExportVariables.Add("hits", new CombatantData.TextExportFormatter("hits", "Hits", "The number of attack attempts that produced damage.  IE a spell successfully doing damage.", (Data, Extra) => { return CombatantFormatSwitch(Data, "hits", Extra); }));
            CombatantData.ExportVariables.Add("crithits", new CombatantData.TextExportFormatter("crithits", "Critical Hit Count", "The number of damaging attacks that were critical.", (Data, Extra) => { return CombatantFormatSwitch(Data, "crithits", Extra); }));
            CombatantData.ExportVariables.Add("crithit%", new CombatantData.TextExportFormatter("crithit%", "Critical Hit Percentage", "The percentage of damaging attacks that were critical.", (Data, Extra) => { return CombatantFormatSwitch(Data, "crithit%", Extra); }));
            CombatantData.ExportVariables.Add("crittypes", new CombatantData.TextExportFormatter("crittypes", "Critical Types", "Distribution of Critical Types  (Normal|Legendary|Fabled|Mythical)", (Data, Extra) => { return CombatantFormatSwitch(Data, "crittypes", Extra); }));
            CombatantData.ExportVariables.Add("misses", new CombatantData.TextExportFormatter("misses", "Misses", "The number of auto-attacks or CAs that produced a miss message.", (Data, Extra) => { return CombatantFormatSwitch(Data, "misses", Extra); }));
            CombatantData.ExportVariables.Add("hitfailed", new CombatantData.TextExportFormatter("hitfailed", "Other Avoid", "Any type of failed attack that was not a miss.  This includes resists, reflects, blocks, dodging, etc.", (Data, Extra) => { return CombatantFormatSwitch(Data, "hitfailed", Extra); }));
            CombatantData.ExportVariables.Add("swings", new CombatantData.TextExportFormatter("swings", "Swings (Attacks)", "The number of attack attempts.  This includes any auto-attacks or abilities, also including resisted abilities that do no damage.", (Data, Extra) => { return CombatantFormatSwitch(Data, "swings", Extra); }));
            CombatantData.ExportVariables.Add("tohit", new CombatantData.TextExportFormatter("tohit", "To Hit %", "The percentage of hits to swings as 12.34", (Data, Extra) => { return CombatantFormatSwitch(Data, "tohit", Extra); }));
            CombatantData.ExportVariables.Add("TOHIT", new CombatantData.TextExportFormatter("TOHIT", "Short To Hit %", "The percentage of hits to swings as 12", (Data, Extra) => { return CombatantFormatSwitch(Data, "TOHIT", Extra); }));
            CombatantData.ExportVariables.Add("maxhit", new CombatantData.TextExportFormatter("maxhit", "Highest Hit", "The highest single damaging hit formatted as [Combatant-]SkillName-Damage#", (Data, Extra) => { return CombatantFormatSwitch(Data, "maxhit", Extra); }));
            CombatantData.ExportVariables.Add("MAXHIT", new CombatantData.TextExportFormatter("MAXHIT", "Short Highest Hit", "The highest single damaging hit formatted as [Combatant-]Damage#", (Data, Extra) => { return CombatantFormatSwitch(Data, "MAXHIT", Extra); }));
            CombatantData.ExportVariables.Add("maxhit-*", new CombatantData.TextExportFormatter("maxhit-*", "Highest Hit w/ suffix", "MaxHit divided by 1/K/M/B/T/Q", (Data, Extra) => { return CombatantFormatSwitch(Data, "maxhit-*", Extra); }));
            CombatantData.ExportVariables.Add("MAXHIT-*", new CombatantData.TextExportFormatter("MAXHIT-*", "Short Highest Hit w/ suffix", "Short MaxHit divided by 1/K/M/B/T/Q", (Data, Extra) => { return CombatantFormatSwitch(Data, "MAXHIT-*", Extra); }));
            CombatantData.ExportVariables.Add("healed", new CombatantData.TextExportFormatter("healed", "Healed", "The numerical total of all heals, wards or similar sourced from this combatant.", (Data, Extra) => { return CombatantFormatSwitch(Data, "healed", Extra); }));
            CombatantData.ExportVariables.Add("healed%", new CombatantData.TextExportFormatter("healed%", "Healed %", "This value represents the percent share of all healing done by allies in this encounter.", (Data, Extra) => { return CombatantFormatSwitch(Data, "healed%", Extra); }));
            CombatantData.ExportVariables.Add("enchps", new CombatantData.TextExportFormatter("enchps", "Encounter HPS", "The healing total of the combatant divided by the duration of the encounter, formatted as 12.34", (Data, Extra) => { return CombatantFormatSwitch(Data, "enchps", Extra); }));
            CombatantData.ExportVariables.Add("enchps-*", new CombatantData.TextExportFormatter("enchps-*", "Encounter HPS w/suffix", "Encounter HPS divided by 1/K/M/B/T/Q", (Data, Extra) => { return CombatantFormatSwitch(Data, "enchps-*", Extra); }));
            CombatantData.ExportVariables.Add("ENCHPS", new CombatantData.TextExportFormatter("ENCHPS", "Short Encounter HPS", "The healing total of the combatant divided by the duration of the encounter, formatted as 12", (Data, Extra) => { return CombatantFormatSwitch(Data, "ENCHPS", Extra); }));
            CombatantData.ExportVariables.Add("ENCHPS-k", new CombatantData.TextExportFormatter("ENCHPS-k", "Short Encounter HPS K", "Short Encounter HPS divided by 1,000 (with no decimal places)", (Data, Extra) => { return CombatantFormatSwitch(Data, "ENCHPS-k", Extra); }));
            CombatantData.ExportVariables.Add("ENCHPS-m", new CombatantData.TextExportFormatter("ENCHPS-m", "Short Encounter HPS M", "Short Encounter HPS divided by 1,000,000 (with no decimal places)", (Data, Extra) => { return CombatantFormatSwitch(Data, "ENCHPS-m", Extra); }));
            CombatantData.ExportVariables.Add("ENCHPS-*", new CombatantData.TextExportFormatter("ENCHPS-*", "Short Encounter HPS w/suffix", "Short Encounter HPS divided by 1/K/M/B/T/Q (with no decimal places)", (Data, Extra) => { return CombatantFormatSwitch(Data, "ENCHPS-*", Extra); }));
            CombatantData.ExportVariables.Add("critheals", new CombatantData.TextExportFormatter("critheals", "Critical Heal Count", "The number of heals that were critical.", (Data, Extra) => { return CombatantFormatSwitch(Data, "critheals", Extra); }));
            CombatantData.ExportVariables.Add("critheal%", new CombatantData.TextExportFormatter("critheal%", "Critical Heal Percentage", "The percentage of heals that were critical.", (Data, Extra) => { return CombatantFormatSwitch(Data, "critheal%", Extra); }));
            CombatantData.ExportVariables.Add("heals", new CombatantData.TextExportFormatter("heals", "Heal Count", "The total number of heals from this combatant.", (Data, Extra) => { return CombatantFormatSwitch(Data, "heals", Extra); }));
            CombatantData.ExportVariables.Add("cures", new CombatantData.TextExportFormatter("cures", "Cure or Dispel Count", "The total number of times the combatant cured or dispelled", (Data, Extra) => { return CombatantFormatSwitch(Data, "cures", Extra); }));
            CombatantData.ExportVariables.Add("maxheal", new CombatantData.TextExportFormatter("maxheal", "Highest Heal", "The highest single healing amount formatted as [Combatant-]SkillName-Healing#", (Data, Extra) => { return CombatantFormatSwitch(Data, "maxheal", Extra); }));
            CombatantData.ExportVariables.Add("MAXHEAL", new CombatantData.TextExportFormatter("MAXHEAL", "Short Highest Heal", "The highest single healing amount formatted as [Combatant-]Healing#", (Data, Extra) => { return CombatantFormatSwitch(Data, "MAXHEAL", Extra); }));
            CombatantData.ExportVariables.Add("maxhealward", new CombatantData.TextExportFormatter("maxhealward", "Highest Heal/Ward", "The highest single healing/warding amount formatted as [Combatant-]SkillName-Healing#", (Data, Extra) => { return CombatantFormatSwitch(Data, "maxhealward", Extra); }));
            CombatantData.ExportVariables.Add("MAXHEALWARD", new CombatantData.TextExportFormatter("MAXHEALWARD", "Short Highest Heal/Ward", "The highest single healing/warding amount formatted as [Combatant-]Healing#", (Data, Extra) => { return CombatantFormatSwitch(Data, "MAXHEALWARD", Extra); }));
            CombatantData.ExportVariables.Add("maxheal-*", new CombatantData.TextExportFormatter("maxheal-*", "Highest Heal w/ suffix", "Highest Heal divided by 1/K/M/B/T/Q", (Data, Extra) => { return CombatantFormatSwitch(Data, "maxheal-*", Extra); }));
            CombatantData.ExportVariables.Add("MAXHEAL-*", new CombatantData.TextExportFormatter("MAXHEAL-*", "Short Highest Heal w/ suffix", "Short Highest Heal divided by 1/K/M/B/T/Q", (Data, Extra) => { return CombatantFormatSwitch(Data, "MAXHEAL-*", Extra); }));
            CombatantData.ExportVariables.Add("maxhealward-*", new CombatantData.TextExportFormatter("maxhealward-*", "Highest Heal/Ward w/ suffix", "Highest Heal/Ward divided by 1/K/M/B/T/Q", (Data, Extra) => { return CombatantFormatSwitch(Data, "maxhealward-*", Extra); }));
            CombatantData.ExportVariables.Add("MAXHEALWARD-*", new CombatantData.TextExportFormatter("MAXHEALWARD-*", "Short Highest Heal/Ward w/ suffix", "Short Highest Heal/Ward divided by 1/K/M/B/T/Q", (Data, Extra) => { return CombatantFormatSwitch(Data, "MAXHEALWARD-*", Extra); }));
            CombatantData.ExportVariables.Add("damagetaken", new CombatantData.TextExportFormatter("damagetaken", "Damage Received", "The total amount of damage this combatant received.", (Data, Extra) => { return CombatantFormatSwitch(Data, "damagetaken", Extra); }));
            CombatantData.ExportVariables.Add("damagetaken-*", new CombatantData.TextExportFormatter("damagetaken-*", "Damage Received w/suffix", "Damage Received divided by 1/K/M/B/T/Q", (Data, Extra) => { return CombatantFormatSwitch(Data, "damagetaken-*", Extra); }));
            CombatantData.ExportVariables.Add("healstaken", new CombatantData.TextExportFormatter("healstaken", "Healing Received", "The total amount of healing this combatant received.", (Data, Extra) => { return CombatantFormatSwitch(Data, "healstaken", Extra); }));
            CombatantData.ExportVariables.Add("healstaken-*", new CombatantData.TextExportFormatter("healstaken-*", "Healing Received w/suffix", "Healing Received divided by 1/K/M/B/T/Q", (Data, Extra) => { return CombatantFormatSwitch(Data, "healstaken-*", Extra); }));
            CombatantData.ExportVariables.Add("powerdrain", new CombatantData.TextExportFormatter("powerdrain", "Power Drain", "The amount of power this combatant drained from others.", (Data, Extra) => { return CombatantFormatSwitch(Data, "powerdrain", Extra); }));
            CombatantData.ExportVariables.Add("powerdrain-*", new CombatantData.TextExportFormatter("powerdrain-*", "Power Drain w/suffix", "Power Drain divided by 1/K/M/B/T/Q", (Data, Extra) => { return CombatantFormatSwitch(Data, "powerdrain-*", Extra); }));
            CombatantData.ExportVariables.Add("powerheal", new CombatantData.TextExportFormatter("powerheal", "Power Replenish", "The amount of power this combatant replenished to others.", (Data, Extra) => { return CombatantFormatSwitch(Data, "powerheal", Extra); }));
            CombatantData.ExportVariables.Add("powerheal-*", new CombatantData.TextExportFormatter("powerheal-*", "Power Replenish w/suffix", "Power Replenish divided by 1/K/M/B/T/Q", (Data, Extra) => { return CombatantFormatSwitch(Data, "powerheal-*", Extra); }));
            CombatantData.ExportVariables.Add("kills", new CombatantData.TextExportFormatter("kills", "Killing Blows", "The total number of times this character landed a killing blow.", (Data, Extra) => { return CombatantFormatSwitch(Data, "kills", Extra); }));
            CombatantData.ExportVariables.Add("deaths", new CombatantData.TextExportFormatter("deaths", "Deaths", "The total number of times this character was killed by another.", (Data, Extra) => { return CombatantFormatSwitch(Data, "deaths", Extra); }));
            CombatantData.ExportVariables.Add("threatstr", new CombatantData.TextExportFormatter("threatstr", "Threat Increase/Decrease", "The amount of direct threat output that was increased/decreased.", (Data, Extra) => { return CombatantFormatSwitch(Data, "threatstr", Extra); }));
            CombatantData.ExportVariables.Add("threatdelta", new CombatantData.TextExportFormatter("threatdelta", "Threat Delta", "The amount of direct threat output relative to zero.", (Data, Extra) => { return CombatantFormatSwitch(Data, "threatdelta", Extra); }));
            CombatantData.ExportVariables.Add("NAME3", new CombatantData.TextExportFormatter("NAME3", "Name (3 chars)", "The combatant's name, up to 3 characters will be displayed.", (Data, Extra) => { return CombatantFormatSwitch(Data, "NAME3", Extra); }));
            CombatantData.ExportVariables.Add("NAME4", new CombatantData.TextExportFormatter("NAME4", "Name (4 chars)", "The combatant's name, up to 4 characters will be displayed.", (Data, Extra) => { return CombatantFormatSwitch(Data, "NAME4", Extra); }));
            CombatantData.ExportVariables.Add("NAME5", new CombatantData.TextExportFormatter("NAME5", "Name (5 chars)", "The combatant's name, up to 5 characters will be displayed.", (Data, Extra) => { return CombatantFormatSwitch(Data, "NAME5", Extra); }));
            CombatantData.ExportVariables.Add("NAME6", new CombatantData.TextExportFormatter("NAME6", "Name (6 chars)", "The combatant's name, up to 6 characters will be displayed.", (Data, Extra) => { return CombatantFormatSwitch(Data, "NAME6", Extra); }));
            CombatantData.ExportVariables.Add("NAME7", new CombatantData.TextExportFormatter("NAME7", "Name (7 chars)", "The combatant's name, up to 7 characters will be displayed.", (Data, Extra) => { return CombatantFormatSwitch(Data, "NAME7", Extra); }));
            CombatantData.ExportVariables.Add("NAME8", new CombatantData.TextExportFormatter("NAME8", "Name (8 chars)", "The combatant's name, up to 8 characters will be displayed.", (Data, Extra) => { return CombatantFormatSwitch(Data, "NAME8", Extra); }));
            CombatantData.ExportVariables.Add("NAME9", new CombatantData.TextExportFormatter("NAME9", "Name (9 chars)", "The combatant's name, up to 9 characters will be displayed.", (Data, Extra) => { return CombatantFormatSwitch(Data, "NAME9", Extra); }));
            CombatantData.ExportVariables.Add("NAME10", new CombatantData.TextExportFormatter("NAME10", "Name (10 chars)", "The combatant's name, up to 10 characters will be displayed.", (Data, Extra) => { return CombatantFormatSwitch(Data, "NAME10", Extra); }));
            CombatantData.ExportVariables.Add("NAME11", new CombatantData.TextExportFormatter("NAME11", "Name (11 chars)", "The combatant's name, up to 11 characters will be displayed.", (Data, Extra) => { return CombatantFormatSwitch(Data, "NAME11", Extra); }));
            CombatantData.ExportVariables.Add("NAME12", new CombatantData.TextExportFormatter("NAME12", "Name (12 chars)", "The combatant's name, up to 12 characters will be displayed.", (Data, Extra) => { return CombatantFormatSwitch(Data, "NAME12", Extra); }));
            CombatantData.ExportVariables.Add("NAME13", new CombatantData.TextExportFormatter("NAME13", "Name (13 chars)", "The combatant's name, up to 13 characters will be displayed.", (Data, Extra) => { return CombatantFormatSwitch(Data, "NAME13", Extra); }));
            CombatantData.ExportVariables.Add("NAME14", new CombatantData.TextExportFormatter("NAME14", "Name (14 chars)", "The combatant's name, up to 14 characters will be displayed.", (Data, Extra) => { return CombatantFormatSwitch(Data, "NAME14", Extra); }));
            CombatantData.ExportVariables.Add("NAME15", new CombatantData.TextExportFormatter("NAME15", "Name (15 chars)", "The combatant's name, up to 15 characters will be displayed.", (Data, Extra) => { return CombatantFormatSwitch(Data, "NAME15", Extra); }));

            DamageTypeData.ColumnDefs.Clear();
            DamageTypeData.ColumnDefs.Add("EncId", new DamageTypeData.ColumnDef("EncId", false, "CHAR(8)", "EncId", (Data) => { return string.Empty; }, (Data) => { return Data.Parent.Parent.EncId; }));
            DamageTypeData.ColumnDefs.Add("Combatant", new DamageTypeData.ColumnDef("Combatant", false, "VARCHAR(64)", "Combatant", (Data) => { return Data.Parent.Name; }, (Data) => { return Data.Parent.Name; }));
            DamageTypeData.ColumnDefs.Add("Type", new DamageTypeData.ColumnDef("Type", true, "VARCHAR(64)", "Type", (Data) => { return Data.Type; }, (Data) => { return Data.Type; }));
            DamageTypeData.ColumnDefs.Add("StartTime", new DamageTypeData.ColumnDef("StartTime", false, "TIMESTAMP", "StartTime", (Data) => { return Data.StartTime == DateTime.MaxValue ? "--:--:--" : Data.StartTime.ToString("T"); }, (Data) => { return Data.StartTime == DateTime.MaxValue ? "0000-00-00 00:00:00" : Data.StartTime.ToString("u").TrimEnd(new char[] { 'Z' }); }));
            DamageTypeData.ColumnDefs.Add("EndTime", new DamageTypeData.ColumnDef("EndTime", false, "TIMESTAMP", "EndTime", (Data) => { return Data.EndTime == DateTime.MinValue ? "--:--:--" : Data.EndTime.ToString("T"); }, (Data) => { return Data.EndTime == DateTime.MinValue ? "0000-00-00 00:00:00" : Data.EndTime.ToString("u").TrimEnd(new char[] { 'Z' }); }));
            DamageTypeData.ColumnDefs.Add("Duration", new DamageTypeData.ColumnDef("Duration", false, "INT", "Duration", (Data) => { return Data.DurationS; }, (Data) => { return Data.Duration.TotalSeconds.ToString("0"); }));
            DamageTypeData.ColumnDefs.Add("Damage", new DamageTypeData.ColumnDef("Damage", true, "BIGINT", "Damage", (Data) => { return Data.Damage.ToString(); }, (Data) => { return Data.Damage.ToString(); }));
            DamageTypeData.ColumnDefs.Add("EncDPS", new DamageTypeData.ColumnDef("EncDPS", true, "DOUBLE", "EncDPS", (Data) => { return Data.EncDPS.ToString(); }, (Data) => { return Data.EncDPS.ToString(usCulture); }));
            DamageTypeData.ColumnDefs.Add("CharDPS", new DamageTypeData.ColumnDef("CharDPS", false, "DOUBLE", "CharDPS", (Data) => { return Data.CharDPS.ToString(); }, (Data) => { return Data.CharDPS.ToString(usCulture); }));
            DamageTypeData.ColumnDefs.Add("DPS", new DamageTypeData.ColumnDef("DPS", false, "DOUBLE", "DPS", (Data) => { return Data.DPS.ToString(); }, (Data) => { return Data.DPS.ToString(usCulture); }));
            DamageTypeData.ColumnDefs.Add("Average", new DamageTypeData.ColumnDef("Average", true, "DOUBLE", "Average", (Data) => { return Data.Average.ToString(); }, (Data) => { return Data.Average.ToString(usCulture); }));
            DamageTypeData.ColumnDefs.Add("Median", new DamageTypeData.ColumnDef("Median", false, "BIGINT", "Median", (Data) => { return Data.Median.ToString(); }, (Data) => { return Data.Median.ToString(); }));
            DamageTypeData.ColumnDefs.Add("MinHit", new DamageTypeData.ColumnDef("MinHit", true, "BIGINT", "MinHit", (Data) => { return Data.Swings == 0 ? String.Empty : Data.MinHit.ToString(); }, (Data) => { return Data.Swings == 0 ? String.Empty : Data.MinHit.ToString(); }));
            DamageTypeData.ColumnDefs.Add("MaxHit", new DamageTypeData.ColumnDef("MaxHit", true, "BIGINT", "MaxHit", (Data) => { return Data.Swings == 0 ? String.Empty : Data.MaxHit.ToString(); }, (Data) => { return Data.Swings == 0 ? String.Empty : Data.MaxHit.ToString(); }));
            DamageTypeData.ColumnDefs.Add("Hits", new DamageTypeData.ColumnDef("Hits", true, "INT", "Hits", (Data) => { return Data.Hits.ToString(); }, (Data) => { return Data.Hits.ToString(); }));
            DamageTypeData.ColumnDefs.Add("CritHits", new DamageTypeData.ColumnDef("CritHits", false, "INT", "CritHits", (Data) => { return Data.CritHits.ToString(); }, (Data) => { return Data.CritHits.ToString(); }));
            DamageTypeData.ColumnDefs.Add("Avoids", new DamageTypeData.ColumnDef("Avoids", false, "INT", "Blocked", (Data) => { return Data.Blocked.ToString(); }, (Data) => { return Data.Blocked.ToString(); }));
            DamageTypeData.ColumnDefs.Add("Misses", new DamageTypeData.ColumnDef("Misses", false, "INT", "Misses", (Data) => { return Data.Misses.ToString(); }, (Data) => { return Data.Misses.ToString(); }));
            DamageTypeData.ColumnDefs.Add("Swings", new DamageTypeData.ColumnDef("Swings", true, "INT", "Swings", (Data) => { return Data.Swings.ToString(); }, (Data) => { return Data.Swings.ToString(); }));
            DamageTypeData.ColumnDefs.Add("ToHit", new DamageTypeData.ColumnDef("ToHit", false, "FLOAT", "ToHit", (Data) => { return Data.ToHit.ToString(); }, (Data) => { return Data.ToHit.ToString(); }));
            DamageTypeData.ColumnDefs.Add("AvgDelay", new DamageTypeData.ColumnDef("AvgDelay", false, "FLOAT", "AverageDelay", (Data) => { return Data.AverageDelay.ToString(); }, (Data) => { return Data.AverageDelay.ToString(usCulture); }));
            DamageTypeData.ColumnDefs.Add("Crit%", new DamageTypeData.ColumnDef("Crit%", false, "VARCHAR(8)", "CritPerc", (Data) => { return Data.CritPerc.ToString("0'%"); }, (Data) => { return Data.CritPerc.ToString("0'%"); }));
            DamageTypeData.ColumnDefs.Add("CritTypes", new DamageTypeData.ColumnDef("CritTypes", true, "VARCHAR(32)", "CritTypes", DamageTypeDataGetCritTypes, DamageTypeDataGetCritTypes));


            AttackType.ColumnDefs.Clear();
            AttackType.ColumnDefs.Add("EncId", new AttackType.ColumnDef("EncId", false, "CHAR(8)", "EncId", (Data) => { return string.Empty; }, (Data) => { return Data.Parent.Parent.Parent.EncId; }, (Left, Right) => { return 0; }));
            AttackType.ColumnDefs.Add("Attacker", new AttackType.ColumnDef("Attacker", false, "VARCHAR(64)", "Attacker", (Data) => { return Data.Parent.Outgoing ? Data.Parent.Parent.Name : string.Empty; }, (Data) => { return Data.Parent.Outgoing ? Data.Parent.Parent.Name : string.Empty; }, (Left, Right) => { return 0; }));
            AttackType.ColumnDefs.Add("Victim", new AttackType.ColumnDef("Victim", false, "VARCHAR(64)", "Victim", (Data) => { return Data.Parent.Outgoing ? string.Empty : Data.Parent.Parent.Name; }, (Data) => { return Data.Parent.Outgoing ? string.Empty : Data.Parent.Parent.Name; }, (Left, Right) => { return 0; }));
            AttackType.ColumnDefs.Add("SwingType", new AttackType.ColumnDef("SwingType", false, "TINYINT", "SwingType", GetAttackTypeSwingType, GetAttackTypeSwingType, (Left, Right) => { return GetAttackTypeSwingType(Left).CompareTo(GetAttackTypeSwingType(Right)); }));
            AttackType.ColumnDefs.Add("Type", new AttackType.ColumnDef("Type", true, "VARCHAR(64)", "Type", (Data) => { return Data.Type; }, (Data) => { return Data.Type; }, (Left, Right) => { return Left.Type.CompareTo(Right.Type); }));
            AttackType.ColumnDefs.Add("StartTime", new AttackType.ColumnDef("StartTime", false, "TIMESTAMP", "StartTime", (Data) => { return Data.StartTime == DateTime.MaxValue ? "--:--:--" : Data.StartTime.ToString("T"); }, (Data) => { return Data.StartTime == DateTime.MaxValue ? "0000-00-00 00:00:00" : Data.StartTime.ToString("u").TrimEnd(new char[] { 'Z' }); }, (Left, Right) => { return Left.StartTime.CompareTo(Right.StartTime); }));
            AttackType.ColumnDefs.Add("EndTime", new AttackType.ColumnDef("EndTime", false, "TIMESTAMP", "EndTime", (Data) => { return Data.EndTime == DateTime.MinValue ? "--:--:--" : Data.EndTime.ToString("T"); }, (Data) => { return Data.EndTime == DateTime.MinValue ? "0000-00-00 00:00:00" : Data.EndTime.ToString("u").TrimEnd(new char[] { 'Z' }); }, (Left, Right) => { return Left.EndTime.CompareTo(Right.EndTime); }));
            AttackType.ColumnDefs.Add("Duration", new AttackType.ColumnDef("Duration", false, "INT", "Duration", (Data) => { return Data.DurationS; }, (Data) => { return Data.Duration.TotalSeconds.ToString("0"); }, (Left, Right) => { return Left.Duration.CompareTo(Right.Duration); }));
            AttackType.ColumnDefs.Add("Damage", new AttackType.ColumnDef("Damage", true, "BIGINT", "Damage", (Data) => { return Data.Damage.ToString(); }, (Data) => { return Data.Damage.ToString(); }, (Left, Right) => { return Left.Damage.CompareTo(Right.Damage); }));
            AttackType.ColumnDefs.Add("EncDPS", new AttackType.ColumnDef("EncDPS", true, "DOUBLE", "EncDPS", (Data) => { return Data.EncDPS.ToString(); }, (Data) => { return Data.EncDPS.ToString(usCulture); }, (Left, Right) => { return Left.EncDPS.CompareTo(Right.EncDPS); }));
            AttackType.ColumnDefs.Add("CharDPS", new AttackType.ColumnDef("CharDPS", false, "DOUBLE", "CharDPS", (Data) => { return Data.CharDPS.ToString(); }, (Data) => { return Data.CharDPS.ToString(usCulture); }, (Left, Right) => { return Left.CharDPS.CompareTo(Right.CharDPS); }));
            AttackType.ColumnDefs.Add("DPS", new AttackType.ColumnDef("DPS", false, "DOUBLE", "DPS", (Data) => { return Data.DPS.ToString(); }, (Data) => { return Data.DPS.ToString(usCulture); }, (Left, Right) => { return Left.DPS.CompareTo(Right.DPS); }));
            AttackType.ColumnDefs.Add("Average", new AttackType.ColumnDef("Average", true, "DOUBLE", "Average", (Data) => { return Data.Average.ToString(); }, (Data) => { return Data.Average.ToString(); }, (Left, Right) => { return Left.Average.CompareTo(Right.Average); }));
            AttackType.ColumnDefs.Add("Median", new AttackType.ColumnDef("Median", true, "BIGINT", "Median", (Data) => { return Data.Median.ToString(); }, (Data) => { return Data.Median.ToString(); }, (Left, Right) => { return Left.Median.CompareTo(Right.Median); }));
            AttackType.ColumnDefs.Add("StdDev", new AttackType.ColumnDef("StdDev", true, "DOUBLE", "StdDev", (Data) => { return Math.Sqrt(AttackTypeGetVariance(Data)).ToString(); }, (Data) => { return Math.Sqrt(AttackTypeGetVariance(Data)).ToString(); }, (Left, Right) => { return Math.Sqrt(AttackTypeGetVariance(Left)).CompareTo(Math.Sqrt(AttackTypeGetVariance(Right))); }));
            AttackType.ColumnDefs.Add("CritTypes", new AttackType.ColumnDef("CritTypes", true, "VARCHAR(32)", "CritTypes", AttackTypeGetCritTypes, AttackTypeGetCritTypes, (Left, Right) => { return AttackTypeGetCritTypes(Left).CompareTo(AttackTypeGetCritTypes(Right)); }));
            AttackType.ColumnDefs.Add("Max", new AttackType.ColumnDef("Max", true, "BIGINT", "Max", (Data) => { return Data.MaxHit.ToString(); }, (Data) => { return Data.MaxHit.ToString(); }, (Left, Right) => { return Left.MaxHit.CompareTo(Right.MaxHit); }));
            AttackType.ColumnDefs.Add("Min", new AttackType.ColumnDef("Min", true, "BIGINT", "Min", (Data) => { return Data.MinHit.ToString(); }, (Data) => { return Data.MinHit.ToString(); }, (Left, Right) => { return Left.MinHit.CompareTo(Right.MinHit); }));


            MasterSwing.ColumnDefs.Clear();
            MasterSwing.ColumnDefs.Add("EncId", new MasterSwing.ColumnDef("EncId", false, "CHAR(8)", "EncId", (Data) => { return string.Empty; }, (Data) => { return Data.ParentEncounter.EncId; }, (Left, Right) => { return 0; }));
            MasterSwing.ColumnDefs.Add("Time", new MasterSwing.ColumnDef("Time", true, "TIMESTAMP", "STime", (Data) => { return Data.Time.ToString("T"); }, (Data) => { return Data.Time.ToString("u").TrimEnd(new char[] { 'Z' }); }, (Left, Right) => { return Left.Time.CompareTo(Right.Time); }));
            MasterSwing.ColumnDefs.Add("RelativeTime", new MasterSwing.ColumnDef("RelativeTime", true, "FLOAT", "RelativeTime", (Data) => { return !(Data.ParentEncounter == null) ? (Data.Time - Data.ParentEncounter.StartTime).ToString("g") : String.Empty; }, (Data) => { return !(Data.ParentEncounter == null) ? (Data.Time - Data.ParentEncounter.StartTime).TotalSeconds.ToString(usCulture) : String.Empty; }, (Left, Right) => { return Left.Time.CompareTo(Right.Time); }));
            MasterSwing.ColumnDefs.Add("Attacker", new MasterSwing.ColumnDef("Attacker", true, "VARCHAR(64)", "Attacker", (Data) => { return Data.Attacker; }, (Data) => { return Data.Attacker; }, (Left, Right) => { return Left.Attacker.CompareTo(Right.Attacker); }));
            MasterSwing.ColumnDefs.Add("SwingType", new MasterSwing.ColumnDef("SwingType", false, "TINYINT", "SwingType", (Data) => { return Data.SwingType.ToString(); }, (Data) => { return Data.SwingType.ToString(); }, (Left, Right) => { return Left.SwingType.CompareTo(Right.SwingType); }));
            MasterSwing.ColumnDefs.Add("AttackType", new MasterSwing.ColumnDef("AttackType", true, "VARCHAR(64)", "AttackType", (Data) => { return Data.AttackType; }, (Data) => { return Data.AttackType; }, (Left, Right) => { return Left.AttackType.CompareTo(Right.AttackType); }));
            MasterSwing.ColumnDefs.Add("DamageType", new MasterSwing.ColumnDef("DamageType", true, "VARCHAR(64)", "DamageType", (Data) => { return Data.DamageType; }, (Data) => { return Data.DamageType; }, (Left, Right) => { return Left.DamageType.CompareTo(Right.DamageType); }));
            MasterSwing.ColumnDefs.Add("Victim", new MasterSwing.ColumnDef("Victim", true, "VARCHAR(64)", "Victim", (Data) => { return Data.Victim; }, (Data) => { return Data.Victim; }, (Left, Right) => { return Left.Victim.CompareTo(Right.Victim); }));

            MasterSwing.ColumnDefs.Add("DamageNum", new MasterSwing.ColumnDef("DamageNum", false, "BIGINT", "DamageNum", (Data) =>
            {
                if (Data.Damage.Number < 0)
                    return String.Empty;
                else
                    return ((long)Data.Damage).ToString();
            }
            ,
            (Data) =>
            {
                if (Data.Damage.Number < 0)
                    return String.Empty;
                else
                    return ((long)Data.Damage).ToString();
            },
            (Left, Right) =>
            {
                return Left.Damage.CompareTo(Right.Damage);
            }
            ));
            MasterSwing.ColumnDefs.Add("Damage", new MasterSwing.ColumnDef("Damage", true, "VARCHAR(128)", "DamageString", (Data) => { return Data.Damage.ToString(); }, (Data) => { return Data.Damage.ToString(); }, (Left, Right) => { return Left.Damage.CompareTo(Right.Damage); }));
            MasterSwing.ColumnDefs.Add("Critical", new MasterSwing.ColumnDef("Critical", false, "BOOLEAN", "Critical", (Data) => { return Data.Critical.ToString(); }, (Data) => { return Data.Critical.ToString(usCulture)[0].ToString(); }, (Left, Right) => { return Left.Critical.CompareTo(Right.Critical); }));
            MasterSwing.ColumnDefs.Add("Special", new MasterSwing.ColumnDef("Special", true, "VARCHAR(90)", "Special", (Data) => { return Data.Special; }, (Data) => { return Data.Special; }, (Left, Right) => { return Left.Special.CompareTo(Left.Special); }));
            MasterSwing.ColumnDefs.Add("Overheal", new MasterSwing.ColumnDef("Overheal", true, "BIGINT", "Overheal", (Data) => { return Data.Tags.ContainsKey("overheal") ? ((long)Data.Tags["overheal"]).ToString() : string.Empty; }, (Data) => { return Data.Tags.ContainsKey("overheal") ? ((long)Data.Tags["overheal"]).ToString() : string.Empty; }, (Left, Right) =>
            {
                return (Left.Tags.ContainsKey("overheal") && Right.Tags.ContainsKey("overheal")) ? ((long)Left.Tags["overheal"]).CompareTo((long)Right.Tags["overheal"]) : 0;
            }));
            MasterSwing.ColumnDefs.Add("Outgoing", new MasterSwing.ColumnDef("Outgoing", true, "VARCHAR2(16)", "Outgoing",
                (Data) => { return Data.Tags.ContainsKey("Outgoing") ? Data.Tags["Outgoing"].ToString() : String.Empty; },
                (Data) => { return Data.Tags.ContainsKey("Outgoing") ? Data.Tags["Outgoing"].ToString() : String.Empty; }, (Left, Right) =>
                {
                    return (Left.Tags.ContainsKey("Outgoing") && Right.Tags.ContainsKey("Outgoing")) ? Left.Tags["Outgoing"].ToString().CompareTo(Right.Tags["Outgoing"].ToString()) : 0;
                })
            );
            MasterSwing.ColumnDefs.Add("Incoming", new MasterSwing.ColumnDef("Incoming", true, "VARCHAR2(16)", "Incoming",
                (Data) => { return Data.Tags.ContainsKey("Incoming") ? Data.Tags["Incoming"].ToString() : String.Empty; },
                (Data) => { return Data.Tags.ContainsKey("Incoming") ? Data.Tags["Incoming"].ToString() : String.Empty; }, (Left, Right) =>
                {
                    return (Left.Tags.ContainsKey("Incoming") && Right.Tags.ContainsKey("Incoming")) ? Left.Tags["Incoming"].ToString().CompareTo(Right.Tags["Incoming"].ToString()) : 0;
                })
            );
            foreach (KeyValuePair<string, MasterSwing.ColumnDef> pair in MasterSwing.ColumnDefs)
                pair.Value.GetCellForeColor = (Data) => { return GetSwingTypeColor((EverQuestSwingType)Data.SwingType); };

            ActGlobals.oFormActMain.ValidateLists();
            ActGlobals.oFormActMain.ValidateTableSetup();
        }

        //modified to display an empty string in the event no special type of attack is detected by the regex processing
        private string CombatantDataGetCritTypes(CombatantData Data)
        {
            if (Data.AllOut.TryGetValue(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText, out AttackType at))
            {
                return AttackTypeGetCritTypes(at);
            }
            else
                return String.Empty;
        }
        /// <summary>
        /// Gets the data type's display type from the localization and attempt to parse the value
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        private string DamageTypeDataGetCritTypes(DamageTypeData Data)
        {
            if (Data.Items.TryGetValue(ActGlobals.ActLocalization.LocalizationStrings["attackTypeTerm-all"].DisplayedText, out AttackType at))
            {
                return AttackTypeGetCritTypes(at);
            }
            else
                return String.Empty;
        }
        /// <summary>
        /// attempts to get attack type and the swing type
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        private string GetAttackTypeSwingType(AttackType Data)
        {
            int? swingType = null;
            List<String> swingTypes = Data.Items.ToList().Select(o => o.AttackType).Distinct().ToList();
            List<MasterSwing> cachedItems = new List<MasterSwing>();
            for (int i = 0; i < Data.Items.Count; i++)
            {
                MasterSwing s = Data.Items[i];
                if (swingTypes.Contains(Data.Items[i].SwingType.ToString()) == false)
                    swingTypes.Add(Data.Items[i].SwingType.ToString());
            }
            if (swingTypes.Count == 1)
                swingType = Data.Items[0].SwingType;
            if (!(swingType == null))
                return String.Empty;
            else
                return !(swingType == null) ? String.Empty : swingType.ToString();
        }
        /// <summary>
        /// parses the combatant format switch
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="VarName"></param>
        /// <param name="Extra"></param>
        /// <returns></returns>
        private string CombatantFormatSwitch(CombatantData Data, string VarName, string Extra)
        {
            int len;
            switch (VarName)
            {
                case "name":
                    return Data.Name;
                case "NAME":
                    len = Int32.Parse(Extra);
                    return Data.Name.Length - len > 0 ? Data.Name.Remove(len, Data.Name.Length - len).Trim() : Data.Name;
                case "NAME3":
                    len = 3;
                    return Data.Name.Length - len > 0 ? Data.Name.Remove(len, Data.Name.Length - len).Trim() : Data.Name;
                case "NAME4":
                    len = 4;
                    return Data.Name.Length - len > 0 ? Data.Name.Remove(len, Data.Name.Length - len).Trim() : Data.Name;
                case "NAME5":
                    len = 5;
                    return Data.Name.Length - len > 0 ? Data.Name.Remove(len, Data.Name.Length - len).Trim() : Data.Name;
                case "NAME6":
                    len = 6;
                    return Data.Name.Length - len > 0 ? Data.Name.Remove(len, Data.Name.Length - len).Trim() : Data.Name;
                case "NAME7":
                    len = 7;
                    return Data.Name.Length - len > 0 ? Data.Name.Remove(len, Data.Name.Length - len).Trim() : Data.Name;
                case "NAME8":
                    len = 8;
                    return Data.Name.Length - len > 0 ? Data.Name.Remove(len, Data.Name.Length - len).Trim() : Data.Name;
                case "NAME9":
                    len = 9;
                    return Data.Name.Length - len > 0 ? Data.Name.Remove(len, Data.Name.Length - len).Trim() : Data.Name;
                case "NAME10":
                    len = 10;
                    return Data.Name.Length - len > 0 ? Data.Name.Remove(len, Data.Name.Length - len).Trim() : Data.Name;
                case "NAME11":
                    len = 11;
                    return Data.Name.Length - len > 0 ? Data.Name.Remove(len, Data.Name.Length - len).Trim() : Data.Name;
                case "NAME12":
                    len = 12;
                    return Data.Name.Length - len > 0 ? Data.Name.Remove(len, Data.Name.Length - len).Trim() : Data.Name;
                case "NAME13":
                    len = 13;
                    return Data.Name.Length - len > 0 ? Data.Name.Remove(len, Data.Name.Length - len).Trim() : Data.Name;
                case "NAME14":
                    len = 14;
                    return Data.Name.Length - len > 0 ? Data.Name.Remove(len, Data.Name.Length - len).Trim() : Data.Name;
                case "NAME15":
                    len = 15;
                    return Data.Name.Length - len > 0 ? Data.Name.Remove(len, Data.Name.Length - len).Trim() : Data.Name;
                case "DURATION":
                    return Data.Duration.TotalSeconds.ToString("0");
                case "duration":
                    return Data.DurationS;
                case "maxhit":
                    return Data.GetMaxHit(true, false);
                case "MAXHIT":
                    return Data.GetMaxHit(false, false);
                case "maxhit-*":
                    return Data.GetMaxHit(true, true);
                case "MAXHIT-*":
                    return Data.GetMaxHit(false, true);
                case "maxheal":
                    return Data.GetMaxHeal(true, false, false);
                case "MAXHEAL":
                    return Data.GetMaxHeal(false, false, false);
                case "maxheal-*":
                    return Data.GetMaxHeal(true, false, true);
                case "MAXHEAL-*":
                    return Data.GetMaxHeal(false, false, true);
                case "maxhealward":
                    return Data.GetMaxHeal(true, true, false);
                case "MAXHEALWARD":
                    return Data.GetMaxHeal(false, true, false);
                case "maxhealward-*":
                    return Data.GetMaxHeal(true, true, true);
                case "MAXHEALWARD-*":
                    return Data.GetMaxHeal(false, true, true);
                case "damage":
                    return Data.Damage.ToString();
                case "damage-k":
                    return (Data.Damage / 1000.0).ToString("0.00");
                case "damage-m":
                    return (Data.Damage / 1000000.0).ToString("0.00");
                case "damage-b":
                    return (Data.Damage / 1000000000.0).ToString("0.00");
                case "damage-*":
                    return ActGlobals.oFormActMain.CreateDamageString(Data.Damage, true, true);
                case "DAMAGE-k":
                    return (Data.Damage / 1000.0).ToString("0");
                case "DAMAGE-m":
                    return (Data.Damage / 1000000.0).ToString("0");
                case "DAMAGE-b":
                    return (Data.Damage / 1000000000.0).ToString("0");
                case "DAMAGE-*":
                    return ActGlobals.oFormActMain.CreateDamageString(Data.Damage, true, false);
                case "healed":
                    return Data.Healed.ToString();
                case "healed-*":
                    return ActGlobals.oFormActMain.CreateDamageString(Data.Healed, true, true);
                case "swings":
                    return Data.Swings.ToString();
                case "hits":
                    return Data.Hits.ToString();
                case "crithits":
                    return Data.CritHits.ToString();
                case "critheals":
                    return Data.CritHeals.ToString();
                case "crittypes":
                    return CombatantDataGetCritTypes(Data);
                case "crithit%":
                    return Data.CritDamPerc.ToString("0'%");
                case "critheal%":
                    return Data.CritHealPerc.ToString("0'%");
                case "heals":
                    return Data.Heals.ToString();
                case "cures":
                    return Data.CureDispels.ToString();
                case "misses":
                    return Data.Misses.ToString();
                case "hitfailed":
                    return Data.Blocked.ToString();
                case "TOHIT":
                    return Data.ToHit.ToString("0");
                case "DPS":
                    return Data.DPS.ToString("0");
                case "DPS-k":
                    return (Data.DPS / 1000.0).ToString("0");
                case "DPS-m":
                    return (Data.DPS / 1000000.0).ToString("0");
                case "DPS-*":
                    return ActGlobals.oFormActMain.CreateDamageString((long)Data.DPS, true, false);
                case "ENCDPS":
                    return Data.EncDPS.ToString("0");
                case "ENCDPS-k":
                    return (Data.EncDPS / 1000.0).ToString("0");
                case "ENCDPS-m":
                    return (Data.EncDPS / 1000000.0).ToString("0");
                case "ENCDPS-*":
                    return ActGlobals.oFormActMain.CreateDamageString((long)Data.EncDPS, true, false);
                case "ENCHPS":
                    return Data.EncHPS.ToString("0");
                case "ENCHPS-k":
                    return (Data.EncHPS / 1000.0).ToString("0");
                case "ENCHPS-m":
                    return (Data.EncHPS / 1000000.0).ToString("0");
                case "ENCHPS-*":
                    return ActGlobals.oFormActMain.CreateDamageString((long)Data.EncHPS, true, false);
                case "tohit":
                    return Data.ToHit.ToString("F");
                case "dps":
                    return Data.DPS.ToString("F");
                case "dps-k":
                    return (Data.DPS / 1000.0).ToString("F");
                case "dps-*":
                    return ActGlobals.oFormActMain.CreateDamageString((long)Data.DPS, true, true);
                case "encdps":
                    return Data.EncDPS.ToString("F");
                case "encdps-k":
                    return (Data.EncDPS / 1000.0).ToString("F");
                case "encdps-m":
                    return (Data.EncDPS / 1000000.0).ToString("F");
                case "encdps-*":
                    return ActGlobals.oFormActMain.CreateDamageString((long)Data.EncDPS, true, true);
                case "enchps":
                    return Data.EncHPS.ToString("F");
                case "enchps-k":
                    return (Data.EncHPS / 1000.0).ToString("F");
                case "enchps-m":
                    return (Data.EncHPS / 1000000.0).ToString("F");
                case "enchps-*":
                    return ActGlobals.oFormActMain.CreateDamageString((long)Data.EncHPS, true, true);
                case "healstaken":
                    return Data.HealsTaken.ToString();
                case "healstaken-*":
                    return ActGlobals.oFormActMain.CreateDamageString((long)Data.HealsTaken, true, true);
                case "damagetaken":
                    return Data.DamageTaken.ToString();
                case "damagetaken-*":
                    return ActGlobals.oFormActMain.CreateDamageString((long)Data.DamageTaken, true, true);
                case "powerdrain":
                    return Data.PowerDamage.ToString();
                case "powerdrain-*":
                    return ActGlobals.oFormActMain.CreateDamageString((long)Data.PowerDamage, true, true);
                case "powerheal":
                    return Data.PowerReplenish.ToString();
                case "powerheal-*":
                    return ActGlobals.oFormActMain.CreateDamageString((long)Data.PowerReplenish, true, true);
                case "kills":
                    return Data.Kills.ToString();
                case "deaths":
                    return Data.Deaths.ToString();
                case "damage%":
                    return Data.DamagePercent;
                case "healed%":
                    return Data.HealedPercent;
                case "threatstr":
                    return Data.GetThreatStr("Threat (Out)");
                case "threatdelta":
                    return Data.GetThreatDelta("Threat (Out)").ToString();
                case "n":
                    return "\n";
                case "t":
                    return "\t";

                default:
                    return VarName;
            }
        }
        /// <summary>
        /// attempts to get the encounter format switch
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="SelectiveAllies"></param>
        /// <param name="VarName"></param>
        /// <param name="Extra"></param>
        /// <returns></returns>
        private string EncounterFormatSwitch(EncounterData Data, List<CombatantData> SelectiveAllies, string VarName, string Extra)
        {
            long damage = 0;
            long healed = 0;
            int swings = 0;
            int hits = 0;
            int crits = 0;
            int heals = 0;
            int critheals = 0;
            int cures = 0;
            int misses = 0;
            int hitfail = 0;
            float tohit = 0;
            double dps;
            double hps;
            long healstaken = 0;
            long damagetaken = 0;
            int kills = 0;
            int deaths = 0;

            switch (VarName)
            {
                case "maxheal":
                    return Data.GetMaxHeal(true, false, false);
                case "MAXHEAL":
                    return Data.GetMaxHeal(false, false, false);
                case "maxheal-*":
                    return Data.GetMaxHeal(true, false, true);
                case "MAXHEAL-*":
                    return Data.GetMaxHeal(false, false, true);
                case "maxhealward":
                    return Data.GetMaxHeal(true, true, false);
                case "MAXHEALWARD":
                    return Data.GetMaxHeal(false, true, false);
                case "maxhealward-*":
                    return Data.GetMaxHeal(true, true, true);
                case "MAXHEALWARD-*":
                    return Data.GetMaxHeal(false, true, true);
                case "maxhit":
                    return Data.GetMaxHit(true, false);
                case "MAXHIT":
                    return Data.GetMaxHit(false, false);
                case "maxhit-*":
                    return Data.GetMaxHit(true, true);
                case "MAXHIT-*":
                    return Data.GetMaxHit(false, true);
                case "duration":
                    if (ActGlobals.wallClockDuration)
                    {
                        if (Data.Active)
                        {
                            if (ActGlobals.oFormActMain.LastEstimatedTime > Data.StartTime)
                            {
                                TimeSpan wallDuration = ActGlobals.oFormActMain.LastEstimatedTime - Data.StartTime;
                                if (wallDuration.Hours == 0)
                                    return String.Format("{0:00}:{1:00}", wallDuration.Minutes, wallDuration.Seconds);
                                else
                                    return String.Format("{0:00}:{1:00}:{2:00}", wallDuration.Hours, wallDuration.Minutes, wallDuration.Seconds);
                            }
                            else
                            {
                                return "00:00";
                            }
                        }
                        else
                        {
                            return Data.DurationS;
                        }
                    }
                    else
                        return Data.DurationS;
                case "DURATION":
                    if (ActGlobals.wallClockDuration)
                    {
                        if (Data.Active)
                        {
                            if (ActGlobals.oFormActMain.LastEstimatedTime > Data.StartTime)
                            {
                                TimeSpan wallDuration = ActGlobals.oFormActMain.LastEstimatedTime - Data.StartTime;
                                return ((int)wallDuration.TotalSeconds).ToString("0");
                            }
                            else
                            {
                                return "0";
                            }
                        }
                        else
                        {
                            return ((int)Data.Duration.TotalSeconds).ToString("0");
                        }
                    }
                    else
                        return Data.Duration.TotalSeconds.ToString("0");
                case "damage":
                    foreach (CombatantData cd in SelectiveAllies)
                        damage += cd.Damage;
                    return damage.ToString();
                case "damage-m":
                    foreach (CombatantData cd in SelectiveAllies)
                        damage += cd.Damage;
                    return (damage / 1000000.0).ToString("0.00");
                case "damage-b":
                    foreach (CombatantData cd in SelectiveAllies)
                        damage += cd.Damage;
                    return (damage / 1000000000.0).ToString("0.00");
                case "damage-*":
                    foreach (CombatantData cd in SelectiveAllies)
                        damage += cd.Damage;
                    return ActGlobals.oFormActMain.CreateDamageString(damage, true, true);
                case "DAMAGE-k":
                    foreach (CombatantData cd in SelectiveAllies)
                        damage += cd.Damage;
                    return (damage / 1000.0).ToString("0");
                case "DAMAGE-m":
                    foreach (CombatantData cd in SelectiveAllies)
                        damage += cd.Damage;
                    return (damage / 1000000.0).ToString("0");
                case "DAMAGE-b":
                    foreach (CombatantData cd in SelectiveAllies)
                        damage += cd.Damage;
                    return (damage / 1000000000.0).ToString("0");
                case "DAMAGE-*":
                    foreach (CombatantData cd in SelectiveAllies)
                        damage += cd.Damage;
                    return ActGlobals.oFormActMain.CreateDamageString(damage, true, false);
                case "healed":
                    foreach (CombatantData cd in SelectiveAllies)
                        healed += cd.Healed;
                    return healed.ToString();
                case "healed-*":
                    foreach (CombatantData cd in SelectiveAllies)
                        healed += cd.Healed;
                    return ActGlobals.oFormActMain.CreateDamageString(healed, true, true);
                case "swings":
                    foreach (CombatantData cd in SelectiveAllies)
                        swings += cd.Swings;
                    return swings.ToString();
                case "hits":
                    foreach (CombatantData cd in SelectiveAllies)
                        hits += cd.Hits;
                    return hits.ToString();
                case "crithits":
                    foreach (CombatantData cd in SelectiveAllies)
                        crits += cd.CritHits;
                    return crits.ToString();
                case "crithit%":
                    foreach (CombatantData cd in SelectiveAllies)
                        crits += cd.CritHits;
                    foreach (CombatantData cd in SelectiveAllies)
                        hits += cd.Hits;
                    float crithitperc = (float)crits / (float)hits;
                    return crithitperc.ToString("0'%");
                case "heals":
                    foreach (CombatantData cd in SelectiveAllies)
                        heals += cd.Heals;
                    return heals.ToString();
                case "critheals":
                    foreach (CombatantData cd in SelectiveAllies)
                        critheals += cd.CritHits;
                    return critheals.ToString();
                case "critheal%":
                    foreach (CombatantData cd in SelectiveAllies)
                        critheals += cd.CritHeals;
                    foreach (CombatantData cd in SelectiveAllies)
                        heals += cd.Heals;
                    float crithealperc = (float)critheals / (float)heals;
                    return crithealperc.ToString("0'%");
                case "cures":
                    foreach (CombatantData cd in SelectiveAllies)
                        cures += cd.CureDispels;
                    return cures.ToString();
                case "misses":
                    foreach (CombatantData cd in SelectiveAllies)
                        misses += cd.Misses;
                    return misses.ToString();
                case "hitfailed":
                    foreach (CombatantData cd in SelectiveAllies)
                        hitfail += cd.Blocked;
                    return hitfail.ToString();
                case "TOHIT":
                    foreach (CombatantData cd in SelectiveAllies)
                        tohit += cd.ToHit;
                    tohit /= SelectiveAllies.Count;
                    return tohit.ToString("0");
                case "DPS":
                case "ENCDPS":
                    foreach (CombatantData cd in SelectiveAllies)
                        damage += cd.Damage;
                    dps = damage / Data.Duration.TotalSeconds;
                    return dps.ToString("0");
                case "DPS-*":
                case "ENCDPS-*":
                    foreach (CombatantData cd in SelectiveAllies)
                        damage += cd.Damage;
                    dps = damage / Data.Duration.TotalSeconds;
                    return ActGlobals.oFormActMain.CreateDamageString((long)dps, true, false);
                case "DPS-k":
                case "ENCDPS-k":
                    foreach (CombatantData cd in SelectiveAllies)
                        damage += cd.Damage;
                    dps = damage / Data.Duration.TotalSeconds;
                    return (dps / 1000.0).ToString("0");
                case "ENCDPS-m":
                    foreach (CombatantData cd in SelectiveAllies)
                        damage += cd.Damage;
                    dps = damage / Data.Duration.TotalSeconds;
                    return (dps / 1000000.0).ToString("0");
                case "ENCHPS":
                    foreach (CombatantData cd in SelectiveAllies)
                        healed += cd.Healed;
                    hps = healed / Data.Duration.TotalSeconds;
                    return hps.ToString("0");
                case "ENCHPS-k":
                    foreach (CombatantData cd in SelectiveAllies)
                        healed += cd.Healed;
                    hps = healed / Data.Duration.TotalSeconds;
                    return (hps / 1000.0).ToString("0");
                case "ENCHPS-m":
                    foreach (CombatantData cd in SelectiveAllies)
                        healed += cd.Healed;
                    hps = healed / Data.Duration.TotalSeconds;
                    return (hps / 1000000.0).ToString("0");
                case "ENCHPS-*":
                    foreach (CombatantData cd in SelectiveAllies)
                        healed += cd.Healed;
                    hps = healed / Data.Duration.TotalSeconds;
                    return ActGlobals.oFormActMain.CreateDamageString((long)hps, true, false);
                case "tohit":
                    foreach (CombatantData cd in SelectiveAllies)
                        tohit += cd.ToHit;
                    tohit /= SelectiveAllies.Count;
                    return tohit.ToString("F");
                case "dps":
                case "encdps":
                    foreach (CombatantData cd in SelectiveAllies)
                        damage += cd.Damage;
                    dps = damage / Data.Duration.TotalSeconds;
                    return dps.ToString("F");
                case "dps-k":
                case "encdps-k":
                    foreach (CombatantData cd in SelectiveAllies)
                        damage += cd.Damage;
                    dps = damage / Data.Duration.TotalSeconds;
                    return (dps / 1000.0).ToString("F");
                case "encdps-m":
                    foreach (CombatantData cd in SelectiveAllies)
                        damage += cd.Damage;
                    dps = damage / Data.Duration.TotalSeconds;
                    return (dps / 1000000.0).ToString("F");
                case "encdps-*":
                    foreach (CombatantData cd in SelectiveAllies)
                        damage += cd.Damage;
                    dps = damage / Data.Duration.TotalSeconds;
                    return ActGlobals.oFormActMain.CreateDamageString((long)dps, true, true);
                case "enchps":
                    foreach (CombatantData cd in SelectiveAllies)
                        healed += cd.Healed;
                    hps = healed / Data.Duration.TotalSeconds;
                    return hps.ToString("F");
                case "enchps-k":
                    foreach (CombatantData cd in SelectiveAllies)
                        healed += cd.Healed;
                    hps = healed / Data.Duration.TotalSeconds;
                    return (hps / 1000.0).ToString("F");
                case "enchps-m":
                    foreach (CombatantData cd in SelectiveAllies)
                        healed += cd.Healed;
                    hps = healed / Data.Duration.TotalSeconds;
                    return (hps / 1000000.0).ToString("F");
                case "enchps-*":
                    foreach (CombatantData cd in SelectiveAllies)
                        healed += cd.Healed;
                    hps = healed / Data.Duration.TotalSeconds;
                    return ActGlobals.oFormActMain.CreateDamageString((long)hps, true, true);
                case "healstaken":
                    foreach (CombatantData cd in SelectiveAllies)
                        healstaken += cd.HealsTaken;
                    return healstaken.ToString();
                case "healstaken-*":
                    foreach (CombatantData cd in SelectiveAllies)
                        healstaken += cd.HealsTaken;
                    return ActGlobals.oFormActMain.CreateDamageString(healstaken, true, true);
                case "damagetaken":
                    foreach (CombatantData cd in SelectiveAllies)
                        damagetaken += cd.DamageTaken;
                    return damagetaken.ToString();
                case "damagetaken-*":
                    foreach (CombatantData cd in SelectiveAllies)
                        damagetaken += cd.DamageTaken;
                    return ActGlobals.oFormActMain.CreateDamageString(damagetaken, true, true);
                case "kills":
                    foreach (CombatantData cd in SelectiveAllies)
                        kills += cd.Kills;
                    return kills.ToString();
                case "deaths":
                    foreach (CombatantData cd in SelectiveAllies)
                        deaths += cd.Deaths;
                    return deaths.ToString();
                case "title":
                    return Data.Title;

                default:
                    return VarName;
            }
        }
        /// <summary>
        /// Parsess the date and time based on the EverQuest character log time stamp format
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        internal DateTime ParseDateTime(String timeStamp)
        {
            DateTime.TryParseExact(timeStamp, Properties.EQDPSPlugin.eqDateTimeStampFormat, DateTimeFormatInfo.CurrentInfo, DateTimeStyles.AssumeLocal, out DateTime currentEQTimeStamp);
            return currentEQTimeStamp;
        }
        /// <summary>
        /// Builds a regex string with the timestamp and regex provided
        /// </summary>
        /// <param name="regex"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal String RegexString(String regex)
        {
            if (regex == null)
                throw new ArgumentNullException("Missing value for regex");
            else
                return $@"\[(?<{Properties.EQDPSPlugin.dateTimeOfLogLine}>.+)\] {regex}";
        }
        /// <summary>
        /// Populates noncombat style regexes
        /// </summary>
        private void PopulateRegexNonCombat()
        {
            zoneEnterRgx = new Regex(Properties.EQDPSPlugin.zoneEnter);
            possesive = new Regex(Properties.EQDPSPlugin.petAndPlayerName, RegexOptions.Compiled);
            selfCheck = new Regex(Properties.EQDPSPlugin.selfMatch, RegexOptions.Compiled);
            regexTupleList = new List<Tuple<Color, Regex>>();
        }

        /// <summary>
        /// Populates the regex list with combat strings associated with combat actions in the character log file
        /// </summary>
        private void PopulateRegexCombat()
        {
            String MeleeAttack = @"(?<attacker>.+) (?<attackType>" + $@"{Properties.EQDPSPlugin.attackTypes}" + @")(|s|es|bed) (?<victim>.+)(\sfor\s)(?<damageAmount>[\d]+) ((?:point)(?:s|)) of damage.(?:\s\((?<damageSpecial>.+)\)){0,1}";
            String Evasion = @"(?<attacker>.*) tries to (?<attackType>\S+) (?:(?<victim>(.+)), but \1) (?:(?<evasionType>" + $@"{Properties.EQDPSPlugin.evasionTypes}" + @"))(?:\swith (your|his|hers|its) (shield|staff)){0,1}!(?:[\s][\(](?<evasionSpecial>.+)[\)]){0,1}";
            ActGlobals.oFormEncounterLogs.LogTypeToColorMapping.Clear();
            regexTupleList.Add(new Tuple<Color, Regex>(Color.Red, new Regex(RegexString(MeleeAttack), RegexOptions.Compiled)));
            ActGlobals.oFormEncounterLogs.LogTypeToColorMapping.Add(regexTupleList.Count, regexTupleList[regexTupleList.Count - 1].Item1);
            regexTupleList.Add(new Tuple<Color, Regex>(Color.ForestGreen, new Regex(RegexString(Properties.EQDPSPlugin.DamageShield), RegexOptions.Compiled)));
            ActGlobals.oFormEncounterLogs.LogTypeToColorMapping.Add(regexTupleList.Count, regexTupleList[regexTupleList.Count - 1].Item1);
            regexTupleList.Add(new Tuple<Color, Regex>(Color.Plum, new Regex(RegexString(Properties.EQDPSPlugin.MissedMeleeAttack), RegexOptions.Compiled)));
            ActGlobals.oFormEncounterLogs.LogTypeToColorMapping.Add(regexTupleList.Count, regexTupleList[regexTupleList.Count - 1].Item1);
            regexTupleList.Add(new Tuple<Color, Regex>(Color.Black, new Regex(RegexString(Properties.EQDPSPlugin.SlainMessage), RegexOptions.Compiled)));
            ActGlobals.oFormEncounterLogs.LogTypeToColorMapping.Add(regexTupleList.Count, regexTupleList[regexTupleList.Count - 1].Item1);
            regexTupleList.Add(new Tuple<Color, Regex>(Color.Red, new Regex(RegexString(Properties.EQDPSPlugin.SpellDamage), RegexOptions.Compiled)));
            ActGlobals.oFormEncounterLogs.LogTypeToColorMapping.Add(regexTupleList.Count, regexTupleList[regexTupleList.Count - 1].Item1);
            regexTupleList.Add(new Tuple<Color, Regex>(Color.DarkBlue, new Regex(RegexString(Properties.EQDPSPlugin.Heal), RegexOptions.Compiled)));
            ActGlobals.oFormEncounterLogs.LogTypeToColorMapping.Add(regexTupleList.Count, regexTupleList[regexTupleList.Count - 1].Item1);
            regexTupleList.Add(new Tuple<Color, Regex>(Color.Silver, new Regex(RegexString(Properties.EQDPSPlugin.Unknown), RegexOptions.Compiled)));
            ActGlobals.oFormEncounterLogs.LogTypeToColorMapping.Add(regexTupleList.Count, regexTupleList[regexTupleList.Count - 1].Item1);
            regexTupleList.Add(new Tuple<Color, Regex>(Color.DeepSkyBlue, new Regex(RegexString(Evasion), RegexOptions.Compiled)));
            ActGlobals.oFormEncounterLogs.LogTypeToColorMapping.Add(regexTupleList.Count, regexTupleList[regexTupleList.Count - 1].Item1);
            regexTupleList.Add(new Tuple<Color, Regex>(Color.LightBlue, new Regex(RegexString(Properties.EQDPSPlugin.Banestrike), RegexOptions.Compiled)));
            ActGlobals.oFormEncounterLogs.LogTypeToColorMapping.Add(regexTupleList.Count, regexTupleList[regexTupleList.Count - 1].Item1);
            regexTupleList.Add(new Tuple<Color, Regex>(Color.AliceBlue, new Regex(RegexString(Properties.EQDPSPlugin.SpellDamageOverTime), RegexOptions.Compiled)));
            ActGlobals.oFormEncounterLogs.LogTypeToColorMapping.Add(regexTupleList.Count, regexTupleList[regexTupleList.Count - 1].Item1);
            regexTupleList.Add(new Tuple<Color, Regex>(Color.PaleVioletRed, new Regex(RegexString(Properties.EQDPSPlugin.FocusDamageEffect), RegexOptions.Compiled)));
            ActGlobals.oFormEncounterLogs.LogTypeToColorMapping.Add(regexTupleList.Count, regexTupleList[regexTupleList.Count - 1].Item1);
            regexTupleList.Add(new Tuple<Color, Regex>(Color.DarkOliveGreen, new Regex(RegexString(Properties.EQDPSPlugin.DamageShieldUnknownOrigin), RegexOptions.Compiled)));
            ActGlobals.oFormEncounterLogs.LogTypeToColorMapping.Add(regexTupleList.Count, regexTupleList[regexTupleList.Count - 1].Item1);
            regexTupleList.Add(new Tuple<Color, Regex>(Color.SaddleBrown, new Regex(RegexString(Properties.EQDPSPlugin.zoneChange))));
            ActGlobals.oFormEncounterLogs.LogTypeToColorMapping.Add(regexTupleList.Count, regexTupleList[regexTupleList.Count - 1].Item1);
            regexTupleList.Add(new Tuple<Color, Regex>(Color.Tan, new Regex(RegexString(Properties.EQDPSPlugin.spellResist), RegexOptions.Compiled)));
            ActGlobals.oFormEncounterLogs.LogTypeToColorMapping.Add(regexTupleList.Count, regexTupleList[regexTupleList.Count - 1].Item1);
            regexTupleList.Add(new Tuple<Color, Regex>(Color.ForestGreen, new Regex(RegexString(Properties.EQDPSPlugin.chilledDamageShield), RegexOptions.Compiled)));
            ActGlobals.oFormEncounterLogs.LogTypeToColorMapping.Add(regexTupleList.Count, regexTupleList[regexTupleList.Count - 1].Item1);
            regexTupleList.Add(new Tuple<Color, Regex>(Color.Black, new Regex(RegexString(Properties.EQDPSPlugin.youDied), RegexOptions.Compiled)));
            ActGlobals.oFormEncounterLogs.LogTypeToColorMapping.Add(regexTupleList.Count, regexTupleList[regexTupleList.Count - 1].Item1);
        }

        private void FormActMain_BeforeLogLineRead(bool isImport, LogLineEventArgs logInfo)
        {
            if (chilled != default)
            {
                meleeMatch = regexTupleList[0].Item2.Match(logInfo.logLine);
                Tuple<String, String> petTypeAndName = GetTypeAndNameForPet(CharacterNamePersonaReplace(meleeMatch.Groups["attacker"].Value));
                Dictionary<String, object> newPetInfo = chilled.Tags;
                newPetInfo.Add("Outgoing", petTypeAndName.Item1);
                AddMasterSwing(chilled.SwingType.GetFromIntEverQuestSwingType(),
                    chilled.Special,
                    chilled.Damage,
                    chilled.Time,
                    chilled.AttackType,
                    CharacterNamePersonaReplace(petTypeAndName.Item2),
                    chilled.DamageType,
                    chilled.Victim,
                    newPetInfo
                    );
                chilled = default;
            }
        }

        /// <summary>
        /// Attempts to read before the log line is parsed
        /// </summary>
        /// <param name="isImport"></param>
        /// <param name="logInfo"></param>
        private void FormActMain_OnLogLineRead(bool isImport, LogLineEventArgs logInfo)
        {
            for (int i = 0; i < regexTupleList.Count; i++)
            {
                Match regexMatch = meleeMatch != default ? meleeMatch : regexTupleList[i].Item2.Match(logInfo.logLine);
                if (regexMatch.Success)
                {
                    logInfo.detectedType = i + 1;
                    ParseEverQuestLogLine(regexMatch, i + 1);
                    if(meleeMatch != default)
                        meleeMatch = default;
                    return;
                }
            }
        }
        /// <summary>
        /// Examines the parameter for summoned entities and provides such type entity back to the parsing method with entity name and character name
        /// </summary>
        /// <param name="nameToSetTypeTo"></param>
        /// <returns></returns>
        internal Tuple<String, String> GetTypeAndNameForPet(String nameToSetTypeTo)
        {
            Match possessiveMatch = possesive.Match(nameToSetTypeTo);
            if (possessiveMatch.Success)
            {
                return new Tuple<String, string>(possessiveMatch.Groups["petName"].Value, possessiveMatch.Groups["playerName"].Value);
            }
            else return new Tuple<String, string>(String.Empty, nameToSetTypeTo);
        }
        /// <summary>
        /// Examines the string if the parsed line was a self type action
        /// </summary>
        /// <param name="nameOfCharacter"></param>
        /// <returns>true if self type action, false otherwise</returns>
        internal bool CheckIfSelf(String nameOfCharacter)
        {
            Regex regexSelf = new Regex(@"(you|your|it|her|him|them)(s|sel(f|ves))", RegexOptions.Compiled);
            Match m = regexSelf.Match(nameOfCharacter);
            return m.Success;
        }

        private static Tuple<bool, string> GetCriticalAndSpecialString(String special)
        {
            return new Tuple<bool, String>(special.Contains("Critical"), special.Replace("Critical", "").Replace("  ", " "));
        }

        /// <summary>
        /// Construct Master Swing object
        /// </summary>
        internal static void AddMasterSwing(
            EverQuestSwingType eqst
            , String attackSpecial
            , Dnum damage
            , DateTime dateTimeOfAttack
            , String attackName
            , String attacker
            , String typeOfResource
            , String victim
            , Dictionary<string, Object> tags
            )
        {
            Tuple<bool, string> specCrit = GetCriticalAndSpecialString(attackSpecial);
            ActGlobals.oFormActMain.AddCombatAction(new MasterSwing(
                eqst.GetEverQuestSwingTypeExtensionIntValue()
                , specCrit.Item1
                , specCrit.Item2
                , damage
                , dateTimeOfAttack
                , ActGlobals.oFormActMain.GlobalTimeSorter
                , attackName
                , attacker
                , typeOfResource
                , victim)
            { Tags = tags });
        }

        /// <summary>
        /// Parses if the line is a matched action read in the log file and provides a combat action entry with the swingtype method
        /// </summary>
        /// <param name="regexMatch"></param>
        /// <param name="logMatched"></param>
        private void ParseEverQuestLogLine(Match regexMatch, int logMatched)
        {
            DateTime dateTimeOfParse = ParseDateTime(regexMatch.Groups[Properties.EQDPSPlugin.dateTimeOfLogLine].Value);
            Tuple<String, String> petTypeAndName = GetTypeAndNameForPet(CharacterNamePersonaReplace(regexMatch.Groups["attacker"].Value));
            Tuple<String, String> victimPetTypeAndName = GetTypeAndNameForPet(regexMatch.Groups["victim"].Value);
            Dictionary<string, Object> tags = new Dictionary<string, Object>();

            try
            {
                if(logMatched != 15)
                    tags.Add("Outgoing", petTypeAndName.Item1);
                tags.Add("Incoming", victimPetTypeAndName.Item1);
            }
            catch(Exception ex)
            {
                ActGlobals.oFormActMain.WriteExceptionLog(ex, regexMatch.Value);
            }
            if (logMatched != 15 && logMatched != 13 && logMatched != 6 && ActGlobals.oFormActMain.SetEncounter(ActGlobals.oFormActMain.LastKnownTime, CharacterNamePersonaReplace(petTypeAndName.Item2), CharacterNamePersonaReplace(victimPetTypeAndName.Item2)))
            {
                switch (logMatched)
                {
                    //Melee attack
                    case 1:
                        Dnum damage = new Dnum(Int64.Parse(regexMatch.Groups["damageAmount"].Value), "melee");
                        String attackName = regexMatch.Groups["attackType"].Value == "frenzies on" ? "frenzy" : regexMatch.Groups["attackType"].Value;
                        AddMasterSwing(
                                EverQuestSwingType.Melee
                                , regexMatch.Groups["damageSpecial"].Success ? regexMatch.Groups["damageSpecial"].Value : String.Empty
                                , damage
                                , dateTimeOfParse
                                , attackName
                                , CharacterNamePersonaReplace(petTypeAndName.Item2)
                                , "Hitpoints"
                                , CharacterNamePersonaReplace(victimPetTypeAndName.Item2)
                                , tags
                            );
                        break;
                    //Non-melee damage shield
                    case 2:
                        Dnum nonMeleeDamage = new Dnum(Int64.Parse(regexMatch.Groups["damagePoints"].Value), "damage shield");
                        AddMasterSwing(EverQuestSwingType.DamageShield,
                            regexMatch.Groups["damageSpecial"].Success ? regexMatch.Groups["damageSpecial"].Value : String.Empty,
                            nonMeleeDamage,
                            dateTimeOfParse,
                            regexMatch.Groups["damageShieldType"].Value,
                            CharacterNamePersonaReplace(petTypeAndName.Item2),
                            "Hitpoints",
                            CharacterNamePersonaReplace(victimPetTypeAndName.Item2),
                            tags);
                        break;
                    //Missed melee
                    case 3:
                        Dnum miss = new Dnum(Dnum.Miss, "melee");
                        AddMasterSwing(
                                EverQuestSwingType.Melee
                                , regexMatch.Groups["damageSpecial"].Success ? regexMatch.Groups["damageSpecial"].Value : String.Empty
                                , miss
                                , dateTimeOfParse
                                , regexMatch.Groups["attackType"].Value
                                , CharacterNamePersonaReplace(petTypeAndName.Item2)
                                , "Hitpoints"
                                , CharacterNamePersonaReplace(victimPetTypeAndName.Item2)
                                , tags
                            );
                        break;
                    //Death message
                    case 4:
                        AddMasterSwing(
                                EverQuestSwingType.NonMelee
                                , String.Empty
                                , Dnum.Death
                                , dateTimeOfParse
                                , "Killing"
                                , CharacterNamePersonaReplace(petTypeAndName.Item2)
                                , "Death"
                                , CharacterNamePersonaReplace(victimPetTypeAndName.Item2)
                                , tags
                            );
                        break;
                    //Spell Cast
                    case 5:
                        Dnum spellCastDamage = new Dnum(Int64.Parse(regexMatch.Groups["damagePoints"].Value), regexMatch.Groups["typeOfDamage"].Value);
                        AddMasterSwing(
                                EverQuestSwingType.Spell | EverQuestSwingType.Instant
                                , regexMatch.Groups["damageSpecial"].Success ? regexMatch.Groups["damageSpecial"].Value : String.Empty
                                , spellCastDamage
                                , dateTimeOfParse
                                , regexMatch.Groups["attackType"].Value
                                , CharacterNamePersonaReplace(petTypeAndName.Item2)
                                , "Hitpoints"
                                , CharacterNamePersonaReplace(victimPetTypeAndName.Item2)
                                , tags
                            );
                        break;
                    //Unknown
                    case 7:
                        MasterSwing msUnknown = new MasterSwing(
                            EverQuestSwingType.NonMelee.GetEverQuestSwingTypeExtensionIntValue()
                            , false
                            , new Dnum(Dnum.Unknown)
                            {
                                DamageString2 = regexMatch.Value
                            },
                            dateTimeOfParse,
                            ActGlobals.oFormActMain.GlobalTimeSorter,
                            "Unknown",
                            "Unknown",
                            "Unknown",
                            "Unknown")
                        { Tags = tags };
                        ActGlobals.oFormActMain.AddCombatAction(msUnknown);
                        break;
                    //Evasion
                    case 8:
                        AddMasterSwing(
                               EverQuestSwingType.Melee
                               , regexMatch.Groups["evasionSpecial"].Success ? regexMatch.Groups["evasionSpecial"].Value : String.Empty
                               , new Dnum(Dnum.Miss, regexMatch.Groups["evasionType"].Value)
                               , dateTimeOfParse
                               , regexMatch.Groups["attackType"].Value
                               , CharacterNamePersonaReplace(petTypeAndName.Item2)
                               , "Hitpoints"
                               , CharacterNamePersonaReplace(victimPetTypeAndName.Item2)
                               , tags
                           );
                        break;
                    //Bane damage
                    case 9:
                        AddMasterSwing(
                               EverQuestSwingType.Bane
                               , regexMatch.Groups["baneSpecial"].Success ? regexMatch.Groups["baneSpecial"].Value : String.Empty
                               , new Dnum(Int64.Parse(regexMatch.Groups["baneDamage"].Value), "bane")
                               , dateTimeOfParse
                               , regexMatch.Groups["typeOfDamage"].Value
                               , CharacterNamePersonaReplace(petTypeAndName.Item2)
                               , "Hitpoints"
                               , CharacterNamePersonaReplace(victimPetTypeAndName.Item2)
                               , tags
                           );
                        break;
                    //Damage over time
                    case 10:
                        Dnum spellDamageOverTimeDamage = new Dnum(Int64.Parse(regexMatch.Groups["damagePoints"].Value), "spell dot");
                        AddMasterSwing(
                               EverQuestSwingType.Spell | EverQuestSwingType.OverTime
                               , regexMatch.Groups["spellSpecial"].Success ? regexMatch.Groups["spellSpecial"].Value : String.Empty
                               , spellDamageOverTimeDamage
                               , dateTimeOfParse
                               , regexMatch.Groups["damageEffect"].Value
                               , CharacterNamePersonaReplace(petTypeAndName.Item2)
                               , "Hitpoints"
                               , CharacterNamePersonaReplace(victimPetTypeAndName.Item2)
                               , tags
                           );
                        break;
                    //Focus Direct Damage Spell
                    case 11:
                        AddMasterSwing(
                               EverQuestSwingType.Spell
                               , regexMatch.Groups["focusSpecial"].Success ? regexMatch.Groups["focusSpecial"].Value : String.Empty
                               , new Dnum(Int64.Parse(regexMatch.Groups["damagePoints"].Value), "spell focus")
                               , dateTimeOfParse
                               , regexMatch.Groups["damageEffect"].Value
                               , CharacterNamePersonaReplace(petTypeAndName.Item2)
                               , "Hitpoints"
                               , CharacterNamePersonaReplace(victimPetTypeAndName.Item2)
                               , tags
                           );
                        break;
                    //Unknown damage shield
                    case 12:
                        AddMasterSwing(
                               EverQuestSwingType.DamageShield
                               , regexMatch.Groups["damageSpecial"].Success ? regexMatch.Groups["damageSpecial"].Value : String.Empty
                               , new Dnum(Int64.Parse(regexMatch.Groups["damagePoints"].Value), "damage shield")
                               , dateTimeOfParse
                               , regexMatch.Groups["damageShieldResponse"].Value
                               , String.Empty
                               , "Hitpoints"
                               , CharacterNamePersonaReplace(victimPetTypeAndName.Item2)
                               , tags
                           );
                        break;
                    //Spell resist
                    case 14:
                        AddMasterSwing(
                               EverQuestSwingType.Spell
                               , regexMatch.Groups["damageSpecial"].Success ? regexMatch.Groups["damageSpecial"].Value : String.Empty
                               , new Dnum(Dnum.NoDamage, "spell")
                               , dateTimeOfParse
                               , regexMatch.Groups["spellName"].Value
                               , CharacterNamePersonaReplace(petTypeAndName.Item2)
                               , "Hitpoints"
                               , CharacterNamePersonaReplace(victimPetTypeAndName.Item2)
                               , tags
                           );
                        break;
                    case 16:
                        AddMasterSwing(
                                EverQuestSwingType.NonMelee
                                , String.Empty
                                , Dnum.Death
                                , dateTimeOfParse
                                , "Killing"
                                , CharacterNamePersonaReplace(petTypeAndName.Item2)
                                , "Death"
                                , CharacterNamePersonaReplace(victimPetTypeAndName.Item2)
                                , tags
                            );
                        break;
                    default:
                        ArgumentOutOfRangeException argumentOutOfRangeException = new ArgumentOutOfRangeException(logMatched.GetType().Name, "Match found but no case to assign to");
                        ActGlobals.oFormActMain.WriteExceptionLog(argumentOutOfRangeException, "Method invoked with no matching case");
                        break;
                }
            }
            else if ((logMatched == 13 || logMatched == 6) && logMatched != 15)
            {
                switch (logMatched)
                {
                    case 6:
                        if (ActGlobals.oFormActMain.InCombat)
                        {
                            if (regexMatch.Groups["overHealPoints"].Success)
                                tags["overheal"] = Int64.Parse(regexMatch.Groups["overHealPoints"].Value);
                            AddMasterSwing(
                                        EverQuestSwingType.Healing | ( regexMatch.Groups["overTime"].Success ? EverQuestSwingType.OverTime : EverQuestSwingType.Instant),
                                        regexMatch.Groups["healingSpecial"].Success ? regexMatch.Groups["healingSpecial"].Value : String.Empty,
                                        new Dnum(Int64.Parse(regexMatch.Groups["pointsOfHealing"].Value), "healing"),
                                        dateTimeOfParse,
                                        regexMatch.Groups["healingSpellName"].Success ? regexMatch.Groups["healingSpellName"].Value : new String("unnamed heal".ToCharArray()),
                                        CharacterNamePersonaReplace(petTypeAndName.Item2),
                                        "Hitpoints",
                                        CheckIfSelf(victimPetTypeAndName.Item2) ? CharacterNamePersonaReplace(petTypeAndName.Item2) : CharacterNamePersonaReplace(victimPetTypeAndName.Item2),
                                        tags);
                        }
                        break;
                    case 13:
                        String zoneName = regexMatch.Groups["zoneName"].Value;
                        if (ignoreStringsForZoneParse.Contains(zoneName))
                            break;
                        ActGlobals.oFormActMain.ChangeZone(zoneName);
                        break;
                    default:
                        break;
                }
            }
            else if(logMatched == 15 && ActGlobals.oFormActMain.SetEncounter(ActGlobals.oFormActMain.LastKnownTime, CharacterNamePersonaReplace(petTypeAndName.Item2), CharacterNamePersonaReplace(victimPetTypeAndName.Item2)))
            {
                chilled = new MasterSwing(EverQuestSwingType.DamageShield.GetEverQuestSwingTypeExtensionIntValue(),
                    regexMatch.Groups["damageShieldSpecial"].Success ? regexMatch.Groups["damageShieldSpecial"].Value.Contains("Critical") : false,
                    regexMatch.Groups["damageShieldSpecial"].Success ? regexMatch.Groups["damageShieldSpecial"].Value : String.Empty,
                    new Dnum(Int64.Parse(regexMatch.Groups["damageAmount"].Value), "damage shield"),
                    dateTimeOfParse,
                    ActGlobals.oFormActMain.GlobalTimeSorter,
                    "chilled",
                    String.Empty,
                    "Hitpoints",
                    regexMatch.Groups["victim"].Value)
                { Tags = tags };
            }
            else
            {
                ActGlobals.oFormActMain.WriteExceptionLog(new Exception($"{logMatched} matched but case not set for parsing"), "Case not set for parsing but match was found");
            }
        }
        /// <summary>
        /// returns the charater's name from the log file if there is a match to the persona's listed in the regex used
        /// </summary>
        /// <param name="PersonaString"></param>
        /// <returns></returns>
        private string CharacterNamePersonaReplace(string PersonaString)
        {
            return selfCheck.Match(PersonaString).Success ? ActGlobals.charName : PersonaString;
        }

        #region File System Watcher
        private void SetWatcherToDirectory()
        {
            if (Directory.Exists(directoryPathTB.Text))
            {
                dbgFilePath = Path.Combine(directoryPathTB.Text, fileNameExpected);

                if (File.Exists(dbgFilePath))
                {
                    watcherForDebugFile = new FileSystemWatcher(Path.GetDirectoryName(dbgFilePath), "dbg.txt");
                    watcherForDebugFile.EnableRaisingEvents = true;
                    watcherForDebugFile.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Attributes | NotifyFilters.CreationTime;
                    watcherForDebugFile.Changed += Watcher_Changed;
                    watcherForDebugFile.Created += Watcher_CreatedForDebugFile;
                    sr = new StreamReader(new FileStream(dbgFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                }
                else
                {
                    ActGlobals.oFormActMain.WriteInfoLog($"{dbgFilePath.ToString()} does not exist for reading.");
                    return;
                }

                watcherForRaidRoster = new FileSystemWatcher(Path.GetFullPath(directoryPathTB.Text), "RaidRoster_*-*-*.txt")
                {
                    EnableRaisingEvents = true,
                    IncludeSubdirectories = false,
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Attributes | NotifyFilters.CreationTime
                };
                watcherForRaidRoster.Created += Watcher_Created;
            }
            else
            {
                watcherForDebugFile.Dispose();
                watcherForRaidRoster.Dispose();
            }
        }
        #endregion

        #region Statistical processing

        //Variance calculation for attack damage
        /// <summary>
        /// gets the variance of the attack type for display in the ACT application
        /// </summary>
        /// <param name="Data">data from attacktype collection to be parsed for variance type selected</param>
        /// <returns></returns>
        private double AttackTypeGetVariance(AttackType Data)
        {
            List<MasterSwing> ms = Data.Items.ToList().Where((item) => item.Damage.Number >= 0).ToList();
            double average;
                if (sampVariance.Checked && ms.Count > 1)
                {
                    average = ms.Select((item) => item.Damage.Number).Average();
                    return (ms.Sum((item) =>
                    {
                        return Math.Pow(average - item.Damage.Number, 2.0);
                    }) / (ms.Count - 1));
                }
                else if (populVariance.Checked && Data.Items.Count > 0)
                {
                    average = ms.Select((item) => item.Damage.Number).Average();
                    return (ms.Sum((item) =>
                    {
                        return Math.Pow(average - item.Damage.Number, 2.0);
                    }) / ms.Count);
                }
                else
                    return default;
        }
        #endregion
        /// <summary>
        /// gets the color associated with the type of action in the log file
        /// </summary>
        /// <param name="eqst"></param>
        /// <returns></returns>
        private Color GetSwingTypeColor(EverQuestSwingType eqst)
        {
            switch (eqst)
            {
                case EverQuestSwingType.Melee:
                    return Color.DarkViolet;
                case EverQuestSwingType.NonMelee:
                    return Color.DarkRed;
                case EverQuestSwingType.Healing | EverQuestSwingType.Instant:
                    return Color.DodgerBlue;
                case EverQuestSwingType.Healing | EverQuestSwingType.OverTime:
                    return Color.GreenYellow;
                case EverQuestSwingType.Bane:
                    return Color.Honeydew;
                case EverQuestSwingType.DamageShield:
                    return Color.Green;
                default:
                    return Color.Black;
            }
        }

        #region User Interface Update code
        private void ChangedbgLogFileTextValue(String status)
        {
            switch (directoryPathTB.InvokeRequired)
            {
                case true:
                    this.directoryPathTB.Invoke(new Action(() =>
                    {
                        this.directoryPathTB.Text = status;
                    }));
                    break;
                case false:
                    this.directoryPathTB.Text = status;
                    break;
                default:
                    break;
            }
        }

        private void ChangeEditabilityOfLogFileTextValue(bool enabled)
        {
            if (directoryPathTB.InvokeRequired)
            {
                this.directoryPathTB.Invoke(new Action(() =>
                {
                    this.directoryPathTB.Enabled = enabled;
                }));
            }
            else
                this.directoryPathTB.Enabled = enabled;
        }
        /// <summary>
        /// updates the status label with thread safety based on whether the plugin needs to invoke the codes in separate thread to update the user interface control
        /// </summary>
        /// <param name="status"></param>
        private void ChangeLblStatus(String status)
        {

            switch (lblStatus.InvokeRequired)
            {
                case true:
                    this.lblStatus.Invoke(new Action(() =>
                    {
                        this.lblStatus.Text = status;
                    }));
                    break;
                case false:
                    this.lblStatus.Text = status;
                    break;
                default:
                    break;
            }
        }

        #endregion
        /// <summary>
        /// Gets the attack type crit types percentages for special attacks in the log lines
        /// </summary>
        /// <param name="Data"></param>
        /// <returns>string attack crit type percentage information</returns>
        internal string AttackTypeGetCritTypes(AttackType Data)
        {
            List<MasterSwing> ms = Data.Items.ToList().Where((item) => item.Damage >= 0).ToList();
            int CripplingBlowCount = default;
            int LockedCount = default;
            int CriticalCount = default;
            int StrikethroughCount = default;
            int RiposteCount = default;
            int NonDefinedCount = default;
            int FlurryCount = default;
            int LuckyCount = default;
            int DoubleBowShotCount = default;
            int TwincastCount = default;
            int WildRampageCount = default;
            int FinishingBlowCount = default;
            int count = ms.Count;

            FinishingBlowCount = ms.Where((finishingBlow) =>
            {
                return finishingBlow.Special.Contains(Properties.EQDPSPlugin.FinishingBlow);
            }).Count();
            CriticalCount = ms.Where((critital) =>
            {
                return critital.Critical;
            }).Count();
            FlurryCount = ms.Where((flurry) =>
            {
                return flurry.Special.Contains(Properties.EQDPSPlugin.Flurry);
            }).Count();
            LuckyCount = ms.Where((lucky) =>
            {
                return lucky.Special.Contains(Properties.EQDPSPlugin.Lucky);
            }).Count();
            CripplingBlowCount = ms.Where((cripplingBlow) =>
            {
                return cripplingBlow.Special.Contains(Properties.EQDPSPlugin.CripplingBlow);
            }).Count();
            LockedCount = ms.Where((locked) =>
            {
                return locked.Special.Contains(Properties.EQDPSPlugin.Locked);
            }).Count();
            StrikethroughCount = ms.Where((srikethrough) =>
            {
                return srikethrough.Special.Contains(Properties.EQDPSPlugin.Strikethrough);
            }).Count();
            RiposteCount = ms.Where((riposte) =>
            {
                return riposte.Special.Contains(Properties.EQDPSPlugin.Riposte);
            }).Count();
            DoubleBowShotCount = ms.Where((doubleBowShot) =>
            {
                return doubleBowShot.Special.Contains(Properties.EQDPSPlugin.DoubleBowShot);
            }).Count();
            TwincastCount = ms.Where((twincast) =>
            {
                return twincast.Special.Contains(Properties.EQDPSPlugin.Twincast);
            }).Count();
            WildRampageCount = ms.Where((twincast) =>
            {
                return twincast.Special.Contains(Properties.EQDPSPlugin.WildRampage);
            }).Count();
            NonDefinedCount = ms.Where((nondefined) =>
            {
                return !nondefined.Special.Contains(Properties.EQDPSPlugin.Twincast) &&
                    !nondefined.Special.Contains(Properties.EQDPSPlugin.DoubleBowShot) &&
                    !nondefined.Special.Contains(Properties.EQDPSPlugin.Riposte) &&
                    !nondefined.Special.Contains(Properties.EQDPSPlugin.CripplingBlow) &&
                    !nondefined.Special.Contains(Properties.EQDPSPlugin.Lucky) &&
                    !nondefined.Special.Contains(Properties.EQDPSPlugin.Flurry) &&
                    !nondefined.Special.Contains(Properties.EQDPSPlugin.Critical) &&
                    !nondefined.Special.Contains(Properties.EQDPSPlugin.WildRampage) &&
                    !nondefined.Special.Contains(Properties.EQDPSPlugin.CripplingBlow) &&
                    !nondefined.Special.Contains(Properties.EQDPSPlugin.Strikethrough) &&
                    !nondefined.Special.Contains(Properties.EQDPSPlugin.FinishingBlow)
                    && nondefined.Special.Length > ActGlobals.ActLocalization.LocalizationStrings["specialAttackTerm-none"].DisplayedText.Length;

            }).Count();

            if (count > 0)
            {
                float CripplingBlowPerc = ((float)CripplingBlowCount / (float)count) * 100f;
                float LockedPerc = ((float)LockedCount / (float)count) * 100f;
                float CriticalPerc = ((float)CriticalCount / (float)count) * 100f;
                float NonDefinedPerc = ((float)NonDefinedCount / (float)count) * 100f;
                float StrikethroughPerc = ((float)StrikethroughCount / (float)count) * 100f;
                float RipostePerc = ((float)RiposteCount / (float)count) * 100f;
                float FlurryPerc = ((float)FlurryCount / (float)count) * 100f;
                float LuckyPerc = ((float)LuckyCount / (float)count) * 100f;
                float DoubleBowShotPerc = ((float)DoubleBowShotCount / (float)count) * 100f;
                float TwincastPerc = ((float)TwincastCount / (float)count) * 100f;
                float WildRampagePerc = ((float)WildRampageCount / (float)count) * 100f;
                float FinishingBlowPerc = ((float)FinishingBlowCount / (float)count) * 100f;
                return $"{CripplingBlowPerc:000.0}%CB-{LockedPerc:000.0}%Locked-{CriticalPerc:000.0}%C-{StrikethroughPerc:000.0}%S-{RipostePerc:000.0}%R-{FlurryPerc:000.0}%F-{LuckyPerc:000.0}%Lucky-{DoubleBowShotPerc:000.0}%DB-{TwincastPerc:000.0}%TC-{WildRampagePerc:000.0}%WR-{FinishingBlowPerc:000.0}%FB-{NonDefinedPerc:000.0}%ND";
            }
            return $"---";
        }

        private void selectDirectory_Click(object sender, EventArgs e)
        {
            using (var EverQuestInstallPath = new FolderBrowserDialog())
            {
                DialogResult dr = EverQuestInstallPath.ShowDialog();

                if (dr == DialogResult.OK && Directory.Exists(EverQuestInstallPath.SelectedPath))
                {
                    ChangedbgLogFileTextValue(EverQuestInstallPath.SelectedPath);
                    SetWatcherToDirectory();
                    SaveSettings();
                }
                else
                {
                    MessageBox.Show($"{EverQuestInstallPath.SelectedPath} is not a valid path.", "File Name not as expected", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
        }
    }

}
