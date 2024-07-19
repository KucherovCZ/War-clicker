using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugConsole : MonoBehaviour
{
    // normally scripts shouldnt reference back to main, but debug console is an exception
    public Main main;

    public TMP_InputField MoneyInput;
    public TMP_InputField WarfundsInput;

    public void ResetData()
    { 
        main.ResetData();
    }

    public void AddMoney()
    {
        int value = 0;
        if (Int32.TryParse(MoneyInput.text.Trim(), out value))
        {
            PlayerController.Instance.AddMoney(value);
        }
        else
        { 
            // wtf? sem se to nemuze dostat, inputText je limited na integer
            Debug.LogError("Wrong input for DebugConsole money. Value: " + MoneyInput.text);
        }  
    }

    public void AddWarfunds()
    {
        int value;
        if (Int32.TryParse(WarfundsInput.text, out value))
        {
            PlayerController.Instance.AddWarFunds(value);
        }
        else
        {
            // wtf? sem se to nemuze dostat, inputText je limited na integer
            Debug.LogError("Wrong input for DebugConsole warfunds. Value: " + WarfundsInput.text);
        }
    }

}
