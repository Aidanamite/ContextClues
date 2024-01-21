using SRML;
using UnityEngine;

static class Example
{
    public static bool Loaded = SRModLoader.IsModPresent("contextclues");
    public static GameObject CreateClue(System.Func<string> text)
    {
        if (Loaded)
            return Create(text);
        return null;
    }
    public static GameObject CreateClue(string text)
    {
        if (Loaded)
            return Create(text);
        return null;
    }
    static GameObject Create(System.Func<string> text) => ContextClues.ContextClue.Create(text);
    static GameObject Create(string text) => ContextClues.ContextClue.Create(text);
}
