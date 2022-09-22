using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IniFIleEditor.Models
{
    public class IniLine
    {
        public string? LineType { get; set; }
        public string? LineSubType { get; set; }
        public bool IsEnabled { get; set; } = true;
        public string? Definition { get; set; }
        public string? Target { get; set; }
        public List<Conditional> Conditionals { get; set; } = new List<Conditional>();
    }
}
