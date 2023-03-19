using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public OrbType type
    {
        get { return _type; }
        set
        {
            _type = value;
            UpdateColour();
        }
    }

    [SerializeField]
    private OrbType _type;

    public Vector2Int pos;
    public bool isChecked = false;

    public SpriteRenderer spriteRenderer;
    public List<Color> typeColors = new List<Color>();

    public static readonly Vector2Int rightDir = new Vector2Int(1, 0);
    public static readonly Vector2Int leftDir = new Vector2Int(-1, 0);
    public static readonly Vector2Int upDir = new Vector2Int(0, 1);
    public static readonly Vector2Int downDir = new Vector2Int(0, -1);
    
    private void Start()
    {
        UpdateColour();
    }

    public void ChangeAlpha(float alpha)
    {
        Color c = spriteRenderer.color;
        c.a = alpha;
        spriteRenderer.color = c;
    }

    public void UpdateColour()
    {
        spriteRenderer.color = typeColors[(int)type];
    }

    // Find orbs that makes a Combo with this orb
    public List<Orb> FindComboOrbs()
    {
        List<Orb> comboOrbs = new List<Orb>();

        // If this orb is already checked, exit method
        if (isChecked)
        {
            return comboOrbs;
        }
        else
        {
            isChecked = true;
        }

        List<Orb> xAxisSameTypedOrbs = new List<Orb>();
        // Checks for orbs of the same type on the x axis
        xAxisSameTypedOrbs.AddRange(FindSameTypedOrbsInDirection(rightDir));
        xAxisSameTypedOrbs.AddRange(FindSameTypedOrbsInDirection(leftDir));

        // If Combo detected on the x axis, add to comboOrbs
        if (xAxisSameTypedOrbs.Count >= 2)
        {
            comboOrbs.AddRange(xAxisSameTypedOrbs);
        }

        List<Orb> yAxisSameTypedOrbs = new List<Orb>();
        // Checks for orbs of the same type on the y axis
        yAxisSameTypedOrbs.AddRange(FindSameTypedOrbsInDirection(upDir));
        yAxisSameTypedOrbs.AddRange(FindSameTypedOrbsInDirection(downDir));

        // If Combo detected on the y axis, add to comboOrbs
        if (yAxisSameTypedOrbs.Count >= 2)
        {
            comboOrbs.AddRange(yAxisSameTypedOrbs);
        }

        // If Combo detected at this orb
        if (comboOrbs.Count >= 2)
        {
            // Add this orb to comboOrbs
            comboOrbs.Add(this);

            // Join the orbs found on both axes
            xAxisSameTypedOrbs.AddRange(yAxisSameTypedOrbs);

            // For each orb of the same type found on both axes, check for Combo
            foreach (Orb orb in xAxisSameTypedOrbs)
            {
                // Add the orbs to comboOrbs
                comboOrbs.AddRange(orb.FindComboOrbs());
            }
        }

        // Remove duplicated orbs
        comboOrbs = comboOrbs.Distinct().ToList();

        return comboOrbs;
    }

    // Find connecting orbs of the same type in a direction
    public List<Orb> FindSameTypedOrbsInDirection(Vector2Int dir)
    {
        List<Orb> sameTypedOrbs = new List<Orb>();

        // Set position pointer to the next orb
        int xPointer = pos.x + dir.x;
        int yPointer = pos.y + dir.y;

        // Continues check while pointers are within bounds
        while (xPointer < GameManager.instance.gridSize.x && xPointer >= 0 && yPointer < GameManager.instance.gridSize.y && yPointer >= 0)
        {
            Orb nextOrb = GameManager.instance.grid[xPointer][yPointer].orb;
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
            xPointer += dir.x;
            yPointer += dir.y;
        }

        return sameTypedOrbs;
    }
}
