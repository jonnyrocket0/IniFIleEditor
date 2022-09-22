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
        public MainWindow()
        {
            InitializeComponent();
            var characters = new List<string>
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

            var parser = new Parser(@"d:\mqnext updated", @"d:\everquestlazarus\rof2", characters);
            var linkDbEntities = parser.ParseLinkDb();
            var spellFileSpells = parser.ParseSpellFile();
            linkDbEntities[EntityType.Spell].UnionWith(spellFileSpells);
            var iniLines = parser.ParseIniLines();
            var iniLines2 = parser.ParseIniLines2();
        }
    }
}
