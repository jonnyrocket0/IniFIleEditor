using IniFIleEditor.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;

namespace IniFIleEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<EntityType, HashSet<string>> _entities = new Dictionary<EntityType, HashSet<string>>();
        private List<string> _characters = new List<string>
        {
            "Kemaal",
            "Merica",
            "Silris",
            "Healies",
            "Troubadour",
            "Zuma",
            "Truck",
            "Zukix",
            "Everest",
            "Chase",
            "Fodder",
            "Rocky",
        };

        private Dictionary<string, HashSet<IniLine>> _characterIniLines = new Dictionary<string, HashSet<IniLine>>();

        public MainWindow()
        {
            InitializeComponent();            
            ParseLinkDb();        
            ParseSpellFile();   
            ParseIniLines();
        }

        private void ParseLinkDb()
        {
            var path = @"d:\mqnext updated\resources\mq2linkdb.txt";
            var fileLines = File.ReadAllLines(path);
            var tempSpellList = new HashSet<string>();
            var tempItemList = new HashSet<string>();
            foreach (var fileLine in fileLines)
            {
                var startOfItemName = 57;
                var endOfItemName = fileLine.LastIndexOf("\u0012") - 1;
                var entityName = fileLine[startOfItemName..endOfItemName];
                if (entityName.Contains("Spell:"))
                {
                    entityName = entityName.Replace("Spell: ", "");
                    tempSpellList.Add(entityName);
                    continue;
                }

                tempItemList.Add(entityName);
            }

            _entities.Add(EntityType.Item, tempItemList);
            _entities.Add(EntityType.Spell, tempSpellList);
        }

        private void ParseSpellFile()
        {
            var path = @"d:\everquestlazarus\rof2\spells_us.txt";
            var lines = File.ReadAllLines(path);
            var spellFileSpells = new HashSet<string?>();
            foreach(var line in lines)
            {
                var firstCaretPos = line.IndexOf('^');
                var secondCaretPos = line.IndexOf('^', firstCaretPos + 1);
                var spellName = line.Substring(firstCaretPos + 1, secondCaretPos - firstCaretPos - 1);
                spellFileSpells.Add(spellName);
            }

            _entities[EntityType.Spell].UnionWith(spellFileSpells!);
        }

        private void ParseIniLines()
        {
            foreach (var character in _characters)
            {
                var lines = ReadIniLines(character);
                CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                TextInfo textInfo = cultureInfo.TextInfo;
                var iniLines = new HashSet<IniLine>();
                foreach (var kvp in lines)
                {
                    var lineType = kvp.Key.Replace("[", "").Replace("]", "");
                    foreach (var line in kvp.Value)
                    {
                        if (string.IsNullOrEmpty(line))
                            continue; 

                        var iniLine = new IniLine
                        {
                            LineType = lineType
                        };

                        var mainSplit = line.Split("=");
                        var lineSubType = mainSplit[0];
                        if (lineSubType.StartsWith(";"))
                            iniLine.IsEnabled = false;

                        iniLine.LineSubType = lineSubType.Replace(";", "");
                        var secondarySplit = mainSplit[1].Split("/");
                        iniLine.Definition = secondarySplit[0];

                        if (secondarySplit.Length > 1)
                        {
                            // if the 2nd item doesn't have a pipe, it's the target of the spell
                            if (!secondarySplit[1].Contains("|"))
                                iniLine.Target = secondarySplit[1];

                            foreach (var split in secondarySplit)
                            {
                                var conditionalSplit = split.Split("|");
                                if (conditionalSplit.Length > 1)
                                {
                                    iniLine.Conditionals.Add(new Conditional(conditionalSplit[0], conditionalSplit[1]));
                                }
                            }
                        }


                        iniLines.Add(iniLine);
                    }
                }

                _characterIniLines.Add(character, iniLines);
            }
        }

        private Dictionary<string, List<string>> ReadIniLines(string characterName)
        {
            var path = $@"d:\mqnext updated\macros\e3 bot inis\{characterName}_peqtgc.ini";
            var fileLines = File.ReadAllLines(path);
            string sectionName = null!;
            var lines = new Dictionary<string, List<string>>();
            foreach (var fileLine in fileLines)
            {
                var sectionLines = new List<string>();
                if (fileLine.StartsWith("[") && fileLine.EndsWith("]"))
                {
                    sectionName = fileLine;
                    lines.Add(sectionName, new List<string>());
                    continue;
                }

                lines[sectionName!].Add(fileLine);
            }

            return lines;
        }
    }
}
