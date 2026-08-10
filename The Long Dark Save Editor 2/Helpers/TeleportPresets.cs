using System.Collections.Generic;

namespace The_Long_Dark_Save_Editor_2.Helpers
{
    public class TeleportPreset
    {
        public string Name { get; set; }
        public string Region { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }

    public static class TeleportPresets
    {
        public static List<TeleportPreset> Presets = new List<TeleportPreset>
        {
            // Mystery Lake
            new TeleportPreset { Name = "Mystery Lake - Camp Office", Region = "LakeRegion", X = 200, Y = 0, Z = 150 },
            new TeleportPreset { Name = "Mystery Lake - Trapper's Cabin", Region = "LakeRegion", X = -50, Y = 0, Z = 300 },
            new TeleportPreset { Name = "Mystery Lake - Hydro Dam", Region = "LakeRegion", X = 350, Y = 0, Z = -100 },

            // Coastal Highway
            new TeleportPreset { Name = "Coastal Highway - Quonset Gas Station", Region = "CoastalRegion", X = 100, Y = 0, Z = 200 },
            new TeleportPreset { Name = "Coastal Highway - Commuter Cars", Region = "CoastalRegion", X = 250, Y = 0, Z = 100 },

            // Pleasant Valley
            new TeleportPreset { Name = "Pleasant Valley - Farmstead", Region = "RuralRegion", X = 100, Y = 0, Z = 100 },
            new TeleportPreset { Name = "Pleasant Valley - Signal Hill", Region = "RuralRegion", X = -200, Y = 0, Z = -150 },

            // Mountain Town (Milton)
            new TeleportPreset { Name = "Mountain Town - Milton", Region = "MountainTownRegion", X = 100, Y = 0, Z = 200 },
            new TeleportPreset { Name = "Mountain Town - Orca Gas Station", Region = "MountainTownRegion", X = 50, Y = 0, Z = 100 },

            // Timberwolf Mountain
            new TeleportPreset { Name = "Timberwolf Mountain - Mountaineer's Hut", Region = "CrashMountainRegion", X = 0, Y = 0, Z = 0 },

            // Forlorn Muskeg
            new TeleportPreset { Name = "Forlorn Muskeg - Spence's Farm", Region = "MarshRegion", X = 100, Y = 0, Z = 100 },

            // Broken Railroad
            new TeleportPreset { Name = "Broken Railroad - Maintenance Yard", Region = "TracksRegion", X = 100, Y = 0, Z = 100 },

            // Hushed River Valley
            new TeleportPreset { Name = "Hushed River Valley - Pensive Lookout", Region = "RiverValleyRegion", X = 100, Y = 0, Z = 100 },

            // Desolation Point
            new TeleportPreset { Name = "Desolation Point - Riken", Region = "WhalingStationRegion", X = 50, Y = 0, Z = 50 },

            // Ash Canyon
            new TeleportPreset { Name = "Ash Canyon - Gold Mine", Region = "AshCanyonRegion", X = 100, Y = 0, Z = 100 },

            // Bleak Inlet
            new TeleportPreset { Name = "Bleak Inlet - Cannery", Region = "CanneryRegion", X = 100, Y = 0, Z = 100 },
        };
    }
}
