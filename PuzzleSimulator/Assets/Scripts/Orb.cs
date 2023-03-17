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
    public bool isChecked = false;

    public SpriteRenderer spriteRenderer;
    public List<Color> typeColors = new List<Color>();

    public static readonly Vector2Int rightDir = new Vector2Int(1, 0);
    public static readonly Vector2Int leftDir = new Vector2Int(-1, 0);
    public static readonly Vector2Int downDir = new Vector2Int(0, 1);
    public static readonly Vector2Int upDir = new Vector2Int(0, -1);

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

    // Find orbs that makes a Combo with this orb
    public List<Orb> FindComboOrbs()
    {
        List<Orb> comboOrbs = new List<Orb>();

        // If this orb is already checked, return
        if (isChecked)
        {
            return comboOrbs;
        }
        else
        {
            isChecked = true;
        }

        // Checks for orbs of the same type on the x axis
        List<Orb> xAxisSameTypedOrbs = new List<Orb>();
        xAxisSameTypedOrbs.AddRange(FindSameTypedOrbsInDirection(rightDir));
        xAxisSameTypedOrbs.AddRange(FindSameTypedOrbsInDirection(leftDir));

        // If Combo detected on the x axis, add to return list
        if (xAxisSameTypedOrbs.Count >= 2)
        {
            comboOrbs.AddRange(xAxisSameTypedOrbs);
        }

        // Checks for orbs of the same type on the y axis
        List<Orb> yAxisSameTypedOrbs = new List<Orb>();
        yAxisSameTypedOrbs.AddRange(FindSameTypedOrbsInDirection(upDir));
        yAxisSameTypedOrbs.AddRange(FindSameTypedOrbsInDirection(downDir));

        // If Combo detected on the y axis, add to return list
        if (yAxisSameTypedOrbs.Count >= 2)
        {
            comboOrbs.AddRange(yAxisSameTypedOrbs);
        }

        // If Combo detected at this orb
        if (comboOrbs.Count >= 2)
        {
            // Add this orb to Combo
            if (!comboOrbs.Contains(this))
            {
                comboOrbs.Add(this);
            }

            List<Orb> sameTypedOrbs = new List<Orb>();

            sameTypedOrbs.AddRange(xAxisSameTypedOrbs);
            sameTypedOrbs.AddRange(yAxisSameTypedOrbs);

            // For each sameTypedOrbs, check for Combo at that orb
            foreach (Orb orb in sameTypedOrbs)
            {
                foreach (Orb o in orb.FindComboOrbs())
                {
                    // Does not add already existing Combo orbs
                    if (!comboOrbs.Contains(o))
                    {
                        comboOrbs.Add(o);
                    }
                }
            }
        }

        return comboOrbs;
    }

    // Find connecting orbs of the same type in a direction
    public List<Orb> FindSameTypedOrbsInDirection(Vector2Int dir)
    {
        List<Orb> sameTypedOrbs = new List<Orb>();

        // Set position pointer to the next orb
        int x = pos.x + dir.x;
        int y = pos.y + dir.y;

        // Continues check while pointers are within bounds
        while (x < GameManager.instance.gridSize.x && x >= 0 && y < GameManager.instance.gridSize.y && y >= 0)
        {
            Orb nextOrb = GameManager.instance.grid[x][y].orb;
            // Adds the checked orb to return list if it has the same type
            if (nextOrb.type == type)
            {
                sameTypedOrbs.Add(nextOrb);
            }
            else
            {
                // Stops the check if a different typed orb is found
                break;
            }

            // Move pointers to next orb
            x += dir.x;
            y += dir.y;
        }

        return sameTypedOrbs;
    }
}
