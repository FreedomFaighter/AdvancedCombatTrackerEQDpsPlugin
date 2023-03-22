﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EverQuestDPSPlugin
{
    internal enum EverQuestSwingType : int
    {
        Melee = 1,
        NonMelee = 2,
        InstantHealing = 4,
        HealOverTime = 8,
        Bane = 16,
        Pet = 32,
        Warder = 64,
        Incoming = 128,
        Outgoing = 256,
        SpellDamage = 512,
        PetMeleeIncoming = Pet | Melee | Incoming,
        PetNonMeleeIncoming = Pet | NonMelee,
        PetMeleeOutgoing = Pet | Melee |Outgoing,
        PetNonMeleeOutgoing = Pet | NonMelee | Outgoing
    }

    internal static class EverQuestSwingTypeExtensions
    {
        internal static int GetEverQuestSwingTypeExtensionIntValue(this EverQuestSwingType type)
        {
            return (int)type;
        }
    }

}
