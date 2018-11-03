using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NesScripts.Controls.PathFind;

public class NPC_Pathfinding : MonoBehaviour
{
    private PathfindingGrid pathfindingGrid { get { return PathfindingGrid.Instance; } }

    public Coroutine MoveRoutine { get; private set; }

    private Vector2[] wayPoints;

    private void Awake()
    {

    }

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Point _from = pathfindingGrid.ConvertPositionToPoint(new Vector2(transform.position.x, transform.position.y));
            Point _to = pathfindingGrid.GenerateRandomTargetPointInsideGrid();
            wayPoints = pathfindingGrid.GetWaypoints(_from, _to);
            if (MoveRoutine != null) StopCoroutine(MoveRoutine);
            MoveRoutine = StartCoroutine(Utility.instance.MoveToWaypointsRoutine(transform, .1f, null, wayPoints));
        }

    }

    public void StopMoving()
    {
        if (MoveRoutine != null) StopCoroutine(MoveRoutine);

    }
}
