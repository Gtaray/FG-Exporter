using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGE.Entities.Configuration
{
    public enum GhostWriterConfig
    {
        None,
        GM,
        Player
    }

    public static class GhostWriter
    {
        public static string Player = "player";
        public static string GM = "gm";
        public static string Both = "both";

        public static bool IsValid(string preset)
        {
            return preset == Player || preset == GM || preset == Both;
        }

        public static bool Matches(string preset, GhostWriterConfig config)
        {
            return 
                (config == GhostWriterConfig.None) ||
                (preset == Both) ||
                (config == GhostWriterConfig.Player && preset == Player) || 
                (config == GhostWriterConfig.GM && preset == GM);
        }
    }

}
