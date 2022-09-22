using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IniFIleEditor.Models
{
    public enum ConditionalType
    {
        CheckFor,
        Ifs,
        Gem,
        MinSick,
        Zone,
        BeforeEvent,
        AfterEvent,
        BeforeSpell,
        AfterSpell,
        HealPct,
        NoInterrupt,
        NoEarlyRecast,
        MinHP,
        MinMana,
        MaxMana,
        Delay,
        TriggerSpell,
        MaxTries,
        PctAggro,
        MinEnd,
        Reagent
    }
}
