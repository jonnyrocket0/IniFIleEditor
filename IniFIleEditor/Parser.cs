using IniFIleEditor.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace IniFIleEditor
{
    internal class Parser
    {
        private string _parentMacroquestDirectory;
        private string _parentEverquestDirectory;
        private List<string> _characters = new List<string>();
        private Dictionary<string, string> _charactersWithFiles = new Dictionary<string, string>();

        internal Parser(string parentMacroquestDirectory, string parentEverquestDirectory, List<string> characters)
        {
            _parentMacroquestDirectory = parentMacroquestDirectory;
            _parentEverquestDirectory = parentEverquestDirectory;
            _characters = characters;

            var botIniPath = $@"{_parentMacroquestDirectory}\macros\e3 bot inis\";
            var files = Directory.GetFiles(botIniPath);
            _charactersWithFiles = files.ToDictionary(s =>
            {
                var lastBackslashPos = s.LastIndexOf('\\');
                var lastUnderscorePos = s.LastIndexOf('_');
                var character = s.Substring(lastBackslashPos + 1, lastUnderscorePos - lastBackslashPos - 1);
                return character;
            });
        }

        internal Dictionary<EntityType, HashSet<string>> ParseLinkDb()
        {
            var path = $@"{_parentMacroquestDirectory}\resources\mq2linkdb.txt";
            var fileLines = File.ReadAllLines(path);
            var tempSpellList = new HashSet<string>();
            var tempItemList = new HashSet<string>();
            var entities = new Dictionary<EntityType, HashSet<string>>();

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

            entities.Add(EntityType.Item, tempItemList);
            entities.Add(EntityType.Spell, tempSpellList);

            return entities;
        }

        internal HashSet<string> ParseSpellFile()
        {
            var path = $@"{_parentEverquestDirectory}\spells_us.txt";
            var lines = File.ReadAllLines(path);
            var spellFileSpells = new HashSet<string?>();
            foreach (var line in lines)
            {
                var firstCaretPos = line.IndexOf('^');
                var secondCaretPos = line.IndexOf('^', firstCaretPos + 1);
                var spellName = line.Substring(firstCaretPos + 1, secondCaretPos - firstCaretPos - 1);
                spellFileSpells.Add(spellName);
            }

            return spellFileSpells!;
        }

        internal Dictionary<string, HashSet<IniLine>> ParseIniLines()
        {
            var characterIniLines = new Dictionary<string, HashSet<IniLine>>();
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

                characterIniLines.Add(character, iniLines);
            }

            return characterIniLines;
        }

        internal Dictionary<string, HashSet<IniLine>> ParseIniLines2()
        {
            var iniPath = $@"{_parentMacroquestDirectory}\macros\e3 bot inis\";
            var iniFiles = Directory.GetFiles(iniPath);

            var characterIniLines = new Dictionary<string, HashSet<IniLine>>();
            foreach (var file in iniFiles)
            {
                var lastBackslashPos = file.LastIndexOf('\\');
                var lastUnderscorePos = file.LastIndexOf('_');
                var character = file.Substring(lastBackslashPos + 1, lastUnderscorePos - lastBackslashPos - 1);
                var lines = ReadIniLines2(file);
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

                characterIniLines.Add(character, iniLines);
            }

            return characterIniLines;
        }

        internal Dictionary<string, List<string>> ReadIniLines(string characterName)
        {
            if (!_charactersWithFiles.TryGetValue(characterName, out var path))
                return new Dictionary<string, List<string>>();

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

        internal Dictionary<string, List<string>> ReadIniLines2(string filePath)
        {
            var fileLines = File.ReadAllLines(filePath);
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
