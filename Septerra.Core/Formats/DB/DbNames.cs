using System;
using System.Collections.Generic;
using Septerra.Core.DB;
// ReSharper disable StringLiteralTypo

namespace Septerra.Core
{
    public static class DbNames
    {
        public static Boolean TryGetRecordName(DbRecordId recordId, out String name)
        {
            return KnownNames.TryGetValue(recordId.Value, out name);
        }

        private static readonly Dictionary<UInt32, String> KnownNames = new Dictionary<UInt32, String>
        { 
            // Animation
            {0x01000FA2, "Burning Animation"},
            {0x01000FA8, "Barrier Animation"},
            {0x01000FAA, "Life Animation"},
            {0x01000FAB, "Lightning Animation"},
            {0x01000FAC, "Buff Animation"},
            {0x01000FAD, "Water Animation"},
            {0x01000FAE, "Stone Animation"},
            {0x01001451, "Skull Animation"},
            {0x01001F40, "Cursor"},
            {0x01001F41, "Inventory Item Icons"},
            {0x01001F42, "Key Items"},
            {0x01001F4C, "Usable Items"},
            {0x01001F43, "Loading"},
            {0x01001F45, "Party Faces"},
            {0x01001F46, "Battle Gauge"},
            {0x01001F47, "Inventory Action Icons"},
            {0x01001F48, "Tarot Cards"},
            {0x01001F4A, "Font"},
            {0x01001F4B, "Inventory Trade Icons"},
            {0x01001F49, "Dialog Icons"},

            {0x01003B60, "Grubb 3D Walk"},
            {0x01003B61, "Grubb 3D Battle"},
            {0x01003C8C, "Led 3D Walk"},
            {0x01003C8D, "Led 3D Battle"},
            
            {0x08001F4D, "Maya 2D"},
            {0x08001F4E, "Grubb 2D"},
            {0x08001F4F, "Runner 2D"},
            {0x08001F50, "Corgan 2D"},
            {0x08001F51, "Led 2D"},
            {0x08001F52, "Selena 2D"},
            {0x08001F53, "Aryam 2D"},
            {0x08001F54, "Badu 2D"},
            {0x08001F55, "Lobo 2D"},
            
            {0x08001F56, "Worlds Map"},
            {0x08001F57, "System Menu"},
            {0x08001F58, "Title Screen"},

            // Text
            {0x05000001, "System"},

            // Characters
            {0x06000014, "Key Items"}
        };
    }
}