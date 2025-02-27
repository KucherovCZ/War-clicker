using System.Collections.Generic;
using UnityEngine;

public static class Translator
{
    private static Dictionary<string, string> LocalTranslationItems = new Dictionary<string, string>();
    private static Dictionary<string, string> EnglishTranslationItems = new Dictionary<string, string>();

    public static void Init(Dictionary<string, string> englishTranslations, Dictionary<string, string> localTranslations)
    {
        LocalTranslationItems = localTranslations;
        EnglishTranslationItems = englishTranslations;
    }

    public static string Translate(string code)
    {
        code = code.ToLower();
        if (LocalTranslationItems.TryGetValue(code, out string result))
        {
            if (string.IsNullOrWhiteSpace(result))
                EnglishTranslationItems.TryGetValue(code, out result);

            return result;
        }
        else
        {
            Logger.Log(LogLevel.WARNING, "Missing translation for item: " + code, "");
            return code;
        }
    }

    public static string TryTranslate(string code)
    {
        code = code.ToLower();
        if (LocalTranslationItems.TryGetValue(code, out string result))
        {
            if (string.IsNullOrWhiteSpace(result))
                EnglishTranslationItems.TryGetValue(code, out result);

            return result;
        }
        else
        {
            return result;
        }
    }
}

