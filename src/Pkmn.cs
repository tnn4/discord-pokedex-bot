using System.Diagnostics;
using System.Collections.Generic;
using Discord;

public enum PTypes {
    Normal=0,
    Fire=1,
    Water=2,
    Electric=3,
    Grass=4,
    Ice=5,
    Fighting=6,
    Poison=7,
    Ground=8,
    Flying=9,
    Psychic=10,
    Bug=11,
    Rock=12,
    Ghost=13,
    Dragon=14,
    Dark=15,
    Steel=16,
    Fairy=17
}

public class Pokemon {
    private List<string> types = new List<string>();

    public List<string> Types  {
        get { return types; }
        set {
            types = value;
        }
    }

    public List<string> GetTypes() {
        return types;
    }    
    
    // takes a list of types
    public void SetTypes( List<string> typeList) {
        foreach (string type in typeList) {
            types.Add(type);
        }
    }
}

public static class PType {

    public static Dictionary<string, double> pTypes = new Dictionary<string,double>(){
        ["normal"] = 0,
        ["fire"]   = 1,
        ["water"]  = 2,
        ["electric"] = 3,
        ["grass"]  = 4,
        ["ice"]    = 5,
        ["fighting"] = 6,
        ["poison"]  = 7,
        ["ground"] = 8,
        ["flying"] = 9,
        ["psychic"] = 10,
        ["bug"] = 11,
        ["rock"] = 12,
        ["ghost"] = 13,
        ["dragon"] = 14,
        ["dark"] = 15,
        ["steel"] = 16,
        ["fairy"] = 17 

    };

    public static Dictionary<string, string> pTypeColors = new Dictionary<string,string>(){
        ["normal"] = "white",
        ["fire"]   = "red",
        ["water"]  = "blue",
        ["electric"] = "yellow",
        ["grass"]  = "green",
        ["ice"]    = "teal",
        ["fighting"] = "darkred",
        ["poison"]  = "purple",
        ["ground"] = "brown",
        ["flying"] = "lightgrey",
        ["psychic"] = "purple",
        ["bug"] = "darkgreen",
        ["rock"] = "darkorange",
        ["ghost"] = "darkpurple",
        ["dragon"] = "darkteal",
        ["dark"] = "default",
        ["steel"] = "darkgrey",
        ["fairy"] = "pink" 

    };
    // https://discordnet.dev/api/Discord.Color.html#Discord_Color_Default
    public static Dictionary<string, Discord.Color> pTypeDiscordColors = new Dictionary<string,Discord.Color>(){
        ["normal"]     = Color.LightGrey,
        ["fire"]       = Color.Red,
        ["water"]      = Color.Blue,
        ["electric"]   = Color.Gold,
        ["grass"]      = Color.Green,
        ["ice"]        = Color.Teal,
        ["fighting"]   = Color.DarkRed,
        ["poison"]     = Color.Purple,
        ["ground"]     = Color.Orange,
        ["flying"]     = Color.LightGrey,
        ["psychic"]    = Color.Purple,
        ["bug"]        = Color.DarkGreen,
        ["rock"]       = Color.DarkOrange,
        ["ghost"]      = Color.DarkPurple,
        ["dragon"]     = Color.DarkTeal,
        ["dark"]       = Color.Default,
        ["steel"]      = Color.DarkGrey,
        ["fairy"]      = Color.Magenta 

    };
    
    static int[,] matrix = new int[2,2] {
        {1,1},
        {1,1}
    };
    // see: https://pokemondb.net/type
    private static double[,] damageMatrix = new double[18,18] {
        
        // NOR  FIR  WAT    ELE  GRA  ICE    FIG  POI  GRO    FLY  PSY  BUG    ROC  GHO  DRA      DAR  STE  FAI
// NOR
        {  1.0, 1.0, 1.0,   1.0, 1.0, 1.0,   1.0, 1.0, 1.0,   1.0, 1.0, 1.0,   1/2, 0.0, 1.0,   1.0, 1/2, 1.0},
// FIR
        {  1.0, 1/2, 1/2,   1.0, 2.0, 2.0,   1.0, 1.0, 1.0,   1.0, 1.0, 2.0,   1/2, 1.0, 1/2,   1.0, 2.0, 1.0},
// WAT
        {  1.0, 2.0, 1/2,   1.0, 1/2, 1.0,   1.0, 1.0, 2.0,   1.0, 1.0, 1.0,   2.0, 1.0, 1/2,   1.0, 1.0, 1.0},


// ELE
        {  1.0, 1.0, 2.0,   1/2, 1/2, 1.0,   1.0, 1.0, 0.0,   2.0, 1.0, 1.0,   1.0, 1.0, 1/2,   1.0, 1.0, 1.0},
// GRA
        {  1.0, 1/2, 2.0,   1.0, 1/2, 1.0,   1.0, 1/2, 2.0,   1/2, 1.0, 1/2,   2.0, 1.0, 1/2,   1.0, 1/2, 1.0},
// ICE
        {  1.0, 1/2, 1/2,   1.0, 2.0, 1/2,   1.0, 1.0, 2.0,   2.0, 1.0, 1.0,   1.0, 1.0, 2.0,   1.0, 1/2, 1.0},


// FIG
        {  2.0, 1.0, 1.0,   1.0, 1.0, 2.0,   1.0, 1/2, 1.0,   1/2, 1/2, 1/2,   2.0, 0.0, 1.0,   2.0, 2.0, 1/2},
// POI
        {  1.0, 1.0, 1.0,   1.0, 2.0, 1.0,   1.0, 1/2, 1/2,   1.0, 1.0, 1.0,   1/2, 1/2, 1.0,   1.0, 0.0, 2.0},
// GRO
        {  1.0, 2.0, 1.0,   2.0, 1/2, 1.0,   1.0, 2.0, 1.0,   0.0, 1.0, 1/2,   2.0, 1.0, 1.0,   1.0, 2.0, 1.0},


// FLY 
        {  1.0, 1.0, 1.0,   1/2, 2.0, 1.0,   2.0, 1.0, 1.0,   1.0, 1.0, 2.0,   1/2, 1.0, 1.0,   1.0, 1/2, 1.0},
// PSY
        {  1.0, 1.0, 1.0,   1.0, 1.0, 1.0,   2.0, 2.0, 1.0,   1.0, 1/2, 1.0,   1.0, 1.0, 1.0,   0.0, 1/2, 1.0},
// BUG
        {  1.0, 1/2, 1.0,   1.0, 2.0, 1.0,   1/2, 1/2, 1.0,   1/2, 2.0, 1.0,   1.0, 1/2, 1.0,   2.0, 1/2, 1/2},

  // ROC
        {  1.0, 2.0, 1.0,   1.0, 1.0, 2.0,   1/2, 1.0, 1/2,   2.0, 1.0, 2.0,   1.0, 1.0, 1.0,   1.0, 1/2, 1.0},
// GHO
        {  0.0, 1.0, 1.0,   1.0, 1.0, 1.0,   1.0, 1.0, 1.0,   1.0, 2.0, 1.0,   1.0, 2.0, 1.0,   1/2, 1.0, 1.0},
// DRA
        {  1.0, 1.0, 1.0,   1.0, 1.0, 1.0,   1.0, 1.0, 1.0,   1.0, 1.0, 1.0,   1.0, 1.0, 2.0,   1.0, 1/2, 0.0},

// DAR
        {  1.0, 1.0, 1.0,   1.0, 1.0, 1.0,   1/2, 1.0, 1.0,   1.0, 2.0, 1.0,   1.0, 2.0, 1.0,   1/2, 1.0, 1/2},
// STE 
        {  1.0, 1/2, 1/2,   1/2, 1.0, 2.0,   1.0, 1.0, 1.0,   1.0, 1.0, 1.0,   2.0, 1.0, 1.0,   1.0, 1/2, 2.0},
// FAI
        {  1.0, 1/2, 1.0,   1.0, 1.0, 1.0,   2.0, 1/2, 1.0,   1.0, 1.0, 1.0,   1.0, 1.0, 2.0,   2.0, 1/2, 1.0}
    };

    public static void printTestType() {
        Console.WriteLine("DEBUG Type chart");
        Debug.Assert(damageMatrix[(int)PTypes.Electric, (int)PTypes.Ground] == 0);
        Debug.Assert(damageMatrix[(int)PTypes.Ground, (int)PTypes.Electric] == 2);
        Debug.Assert(damageMatrix[(int)PTypes.Dragon, (int)PTypes.Steel] == 1/2);
        Console.WriteLine($"Electric -> Ground = {damageMatrix[(int)PTypes.Electric, (int)PTypes.Ground]}x");
        Console.WriteLine($"Water -> Fire = {damageMatrix[(int)PTypes.Water, (int)PTypes.Fire]}x");
    }

    public static double calculateDamageX(string attackerType, Pokemon pokemon) {
        List<string> defenderTypes = pokemon.GetTypes();
        double damageX= 1;
        foreach (string defenderType in defenderTypes){
            damageX = damageX * damageMatrix[(int)pTypes[attackerType], (int)pTypes[defenderType]];
        }
        return damageX;
    }
}