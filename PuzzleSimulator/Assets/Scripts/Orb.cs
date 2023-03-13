using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
    public Vector2Int pos;
    public SpriteRenderer spriteRenderer;
    public List<Color> typeColors = new List<Color>();

    private void Start()
    {
        spriteRenderer.color = typeColors[(int)type];
    }

    public void ChangeAlpha(float alpha)
    {
        Color c = spriteRenderer.color;
        c.a = alpha;
        spriteRenderer.color = c;
    }
}
