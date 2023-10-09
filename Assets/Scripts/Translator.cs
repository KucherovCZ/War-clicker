using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public static class Translator
{
    private static Dictionary<string, string> TranslationItems = new Dictionary<string, string>();
    private static Dictionary<string, string> EnglishTranslationItems = new Dictionary<string, string>();

    public static void Init(Dictionary<string, string> englishTranslations, Dictionary<string, string> localTranslations)
    {
        TranslationItems = localTranslations;
        EnglishTranslationItems = englishTranslations;
    }

    

    public static string Translate(string code)
    {
        code = code.ToLower();
        if (TranslationItems.TryGetValue(code, out string result))
        {
            if (string.IsNullOrWhiteSpace(result))
                EnglishTranslationItems.TryGetValue(code, out result);

            return result;

        }
        else
        {
            Debug.LogWarning("Missing translation for item: " + code);
            return code;
        }
    }
}

