using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OrbType
{
    Red,
    Blue,
    Green,
    Yellow,
    Purple,
    Pink
}

public class Orb : MonoBehaviour
{
    public OrbType type;
    public SpriteRenderer spriteRenderer;
    public List<Color> typeColors = new List<Color>();

    private void Start()
    {
        spriteRenderer.color = typeColors[(int)type];
    }
}
