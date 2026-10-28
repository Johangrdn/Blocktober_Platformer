using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementColor
{
    Color1,
    Color2
}

public class WorldParameters : MonoBehaviour
{
    private static WorldParameters instance;
    public static WorldParameters Instance { get { return instance; } }

    [SerializeField] private Color color1;
    [SerializeField] private Color color2;

    [SerializeField] private LayerMask groundForCharacterColor1;
    [SerializeField] private LayerMask groundForCharacterColor2;
    private void Awake()
    {
        instance = this;
    }
    private void OnValidate()
    {
        instance = this;

        foreach(ColoredElement el in FindObjectsOfType<ColoredElement>())
        {
            el.OnValidate();
        }

        foreach(ColoredCharacter c in FindObjectsOfType<ColoredCharacter>())
        {
            c.OnValidate();
        }
    }

    public Color GetColor(ElementColor color)
    {
        switch(color)
        {
            case ElementColor.Color1:
                return color1;

            case ElementColor.Color2:
                return color2;

            default:
                return color1;
        }
    }

    public LayerMask GetColorGroundMask(ElementColor color)
    {
        switch(color)
        {
            case ElementColor.Color1:
                return LayerMask.NameToLayer("ColoredGround1");

            case ElementColor.Color2:
                return LayerMask.NameToLayer("ColoredGround2");

            default:
                return LayerMask.NameToLayer("ColoredGround1");
        }
    }

    public LayerMask GetColorCharacterMask(ElementColor color)
    {
        switch (color)
        {
            case ElementColor.Color1:
                return LayerMask.NameToLayer("ColoredCharacter1");

            case ElementColor.Color2:
                return LayerMask.NameToLayer("ColoredCharacter2");

            default:
                return LayerMask.NameToLayer("ColoredCharacter1");
        }
    }

    public LayerMask GetColorCharacterGroundMask(ElementColor color)
    {
        switch (color)
        {
            case ElementColor.Color1:
                return LayerMask.NameToLayer("EnvironmentChecks1");

            case ElementColor.Color2:
                return LayerMask.NameToLayer("EnvironmentChecks2");

            default:
                return LayerMask.NameToLayer("EnvironmentChecks1");
        }
    }
}
