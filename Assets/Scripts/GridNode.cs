public class GridNode
{
    public int Row { get; set; }
    public int Column { get; set; }
    public bool IsWalkable { get; set; }
    public GridNode Parent { get; set; }
    public float GCost { get; set; } // Cost from the start node
    public float HCost { get; set; } // Heuristic cost to the end node
    public float FCost => GCost + HCost;

    public GridNode(int row, int column, bool isWalkable)
    {
        Row = row;
        Column = column;
        IsWalkable = isWalkable;
    }
}

