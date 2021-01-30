using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Liquid : MonoBehaviour
{
    [SerializeField]
    private string color;

    public Liquid(string _color)
    {
        color = _color;
    }

    public string GetColor()
    {
        return color;
    }

    public void SetColor(string _color)
    {
        color = _color;
    }
}
