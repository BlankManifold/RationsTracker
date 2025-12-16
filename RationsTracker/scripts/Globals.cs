using Godot;
using Handlers;

namespace Globals
{
    public struct PackedScenes
    {
        public static PackedScene Portion = (PackedScene)ResourceLoader.Load("res://scenes/Portion.tscn");
    }

    public struct Paths
    {
        public static string SaveSetsPlot = "user://sets";
        public static string SaveConfigs = "user://configs";
    }

    public struct SetsData
    {
        public static Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, Portion>> PortionsDict =
            new Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, Portion>> { };

        //TODO NON SERVE -> SONO LE KEYS DI DICT INTERNO DI PORTIONSDICT! 
        public static Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> PortionsTypesDict =
            new Godot.Collections.Dictionary<string, Godot.Collections.Array<string>> { };

        public static void AddSetKey(string setName)
        {
            PortionsDict[setName] = new Godot.Collections.Dictionary<string, Portion> { };
            PortionsTypesDict[setName] = new Godot.Collections.Array<string> { };
        }
        public static void RemoveSet(string setName)
        {
            PortionsDict.Remove(setName);
            PortionsTypesDict.Remove(setName);
        }
        public static void AddPortion(string setName, string type, Portion portion)
        {
            PortionsDict[setName].Add(type, portion);
            PortionsTypesDict[setName].Add(type);
        }
        public static void RemovePortion(string setName, string type, Portion portion)
        {
            PortionsDict[setName].Remove(type);
            PortionsTypesDict[setName].Remove(type);
        }
        public static void ChangePortionName(string setName, string newType, string oldType)
        {
            PortionsDict[setName][newType] = PortionsDict[setName][oldType];
            PortionsDict[setName].Remove(oldType);
            PortionsTypesDict[setName].Remove(oldType);
            PortionsTypesDict[setName].Add(newType);
            PortionsDict[setName][newType] = PortionsDict[setName][oldType];
            PortionsDict[setName].Remove(oldType);
            PortionsTypesDict[setName].Remove(oldType);
            PortionsTypesDict[setName].Add(newType);
        }
        public static void ChangeSetName(string newSetName, string oldSetName)
        {
            PortionsDict[newSetName] = PortionsDict[oldSetName];
            PortionsDict.Remove(oldSetName);

            PortionsTypesDict[newSetName] = PortionsTypesDict[oldSetName];
            PortionsTypesDict.Remove(oldSetName);
        }
        public static bool ContainsPortionType(string setName, string type)
        {
            return PortionsDict[setName].ContainsKey(type);
        }
        public static Godot.Collections.Dictionary<string, Portion> GetPortionsDict(string setName)
        {
            return (Godot.Collections.Dictionary<string, Portion>)PortionsDict[setName].Values;
        }
    }



}