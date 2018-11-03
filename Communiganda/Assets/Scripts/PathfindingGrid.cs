using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NesScripts.Controls.PathFind;

public class PathfindingGrid : MonoBehaviour
{
    public int width = 5;
    public int height = 5;
    bool[,] tilesmap;

    public Vector2 targetPoint;

    public NesScripts.Controls.PathFind.Grid Grid { get; private set; }
    public static PathfindingGrid Instance;


    private void Awake()
    {
        Instance = this;    
    }

    void Start()
    {
        tilesmap = new bool[width, height];
        for (int x = 0; x < tilesmap.GetLength(0); x += 1)
        {
            for (int y = 0; y < tilesmap.GetLength(1); y += 1)
            {
                tilesmap[x, y] = true;
            }
        }
        tilesmap[1, 0] = false;
        tilesmap[1, 1] = false;
        tilesmap[1, 2] = false;
        tilesmap[1, 3] = false;

        Grid = new NesScripts.Controls.PathFind.Grid(tilesmap);
        Camera.main.transform.position = Vector3.zero + new Vector3((width - 1) / 2, (height - 1) / 2f, -10f);
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Point _from = ConvertPositionToPoint(new Vector2(transform.position.x, transform.position.y));
        //    Point _to = ConvertPositionToPoint(targetPoint);//GenerateRandomTargetPointInsideGrid();

        //    List<Point> path = NesScripts.Controls.PathFind.Pathfinding.FindPath(Grid, _from, _to, NesScripts.Controls.PathFind.Pathfinding.DistanceType.Manhattan);
        //    Vector2[] points = new Vector2[path.Count];
        //    for (int i = 0; i < points.Length; i++)
        //    {
        //        points[i] = new Vector2(path[i].x, path[i].y);
        //    }
        //    Utility.instance.MoveToWaypoints(transform, .1f, null, points);
        //}


    }

    private void OnDrawGizmos()
    {
        if (tilesmap == null) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(targetPoint, 0.25f);
        for (int x = 0; x < tilesmap.GetLength(0); x += 1)
        {
            for (int y = 0; y < tilesmap.GetLength(1); y += 1)
            {
                Gizmos.color = tilesmap[x, y] == true ? Color.green : Color.red;
                Gizmos.DrawWireCube(new Vector2(x, y), new Vector3(0.9f, .9f));
            }
        }
    }

    public Point ConvertPositionToPoint(Vector2 pos)
    {
        return new Point(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
    }

    public Point GenerateRandomTargetPointInsideGrid()
    {
        return new Point(Random.Range(0, width), Random.Range(0, height));
    }

    public Vector2 GenerateRandomTargetVector2InsideGrid()
    {
        Point point = new Point(Random.Range(0, width), Random.Range(0, height));
        return new Vector2(point.x, point.y);
    }

    public Vector2[] GetWaypoints(Point from, Point to)
    {
        List<Point> path = NesScripts.Controls.PathFind.Pathfinding.FindPath(Grid, from, to);
        Vector2[] points = new Vector2[path.Count];
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = new Vector2(path[i].x, path[i].y);
        }
        return points;
    }
}
