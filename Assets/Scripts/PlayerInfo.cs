using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public enum Race {
    Dragonborn,
    Dwarf,
    Elf,
    Fairy,
    Gnome,
    Harengon,
    HalfElf,
    HalfOrc,
    Halfling,
    Human,
    Tiefling,
};

public enum Class{
    Barbarian,
    Bard,
    Cleric,
    Druid,
    Fighter,
    Monk,
    Paladin,
    Ranger,
    Rogue,
    Sorcerer,
    Warlock,
    Wizard,
};

public static class PlayerInfo
{
    static string playerName = "Kevin";
    static string playerGender = "Male";
    static Race playerRace = Race.Human;
    static Class playerClass = Class.Fighter;

    public static string GetName() { return playerName; }
    public static void SetName(string playerName) {if (playerName != "") PlayerInfo.playerName = playerName; }
    public static string GetGender() { return playerGender; }
    public static void SetGender(string playerGender) { if (playerGender != "") PlayerInfo.playerGender = playerGender; }
    public static Race GetRace() {  return playerRace; }
    public static void SetRace(int idx) { PlayerInfo.playerRace = (Race)idx; }
    public static Class GetClass() {  return playerClass; }
    public static void SetClass(int idx) { PlayerInfo.playerClass = (Class)idx; }

}
