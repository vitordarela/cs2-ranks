using System;
using System.Collections.Generic;

public enum Rank
{
    One = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Eleven = 11,
    Twelve = 12,
    Thirteen = 13,
    Fourteen = 14,
    Fifteen = 15
}

public static class RankHelper
{
    public static string GetRankSymbol(Rank rank)
    {
        // Mapeia os valores do enum para os símbolos Unicode
        switch (rank)
        {
            case Rank.One: return "①";
            case Rank.Two: return "②";
            case Rank.Three: return "③";
            case Rank.Four: return "④";
            case Rank.Five: return "⑤";
            case Rank.Six: return "⑥";
            case Rank.Seven: return "⑦";
            case Rank.Eight: return "⑧";
            case Rank.Nine: return "⑨";
            case Rank.Ten: return "⑩";
            case Rank.Eleven: return "⑪";
            case Rank.Twelve: return "⑫";
            case Rank.Thirteen: return "⑬";
            case Rank.Fourteen: return "⑭";
            case Rank.Fifteen: return "⑮";
            default: return "";
        }
    }

    public static Rank? GetRankFromKey(string key)
    {
        if (Enum.TryParse(typeof(Rank), key, out var rank))
        {
            return (Rank)rank;
        }
        return null;
    }

    public static IEnumerable<string> GetAllRankSymbols()
    {
        var symbols = new List<string>();
        foreach (Rank rank in Enum.GetValues(typeof(Rank)))
        {
            symbols.Add(GetRankSymbol(rank));
        }
        return symbols;
    }
}
