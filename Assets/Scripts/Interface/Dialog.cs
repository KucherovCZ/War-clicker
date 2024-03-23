using System;
using UnityEngine;

public class Dialog : MonoBehaviour
{
    public static Dialog Instance;

    private GameObject UIObject;

    // Use this for initialization
    public void Init()
    {
        UIObject = gameObject;
    }

    public void ShowDialog()
    {
        throw new NotImplementedException();
    }

}