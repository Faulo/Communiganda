using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NesScripts.Controls.PathFind;

public class Pathfinding : MonoBehaviour
{
    public int width = 5;
    public int height = 5;
    bool[,] tilesmap;

    void Start()
    {
         tilesmap = new bool[width, height];
        for (int x = 0; x < tilesmap.GetLength(0); x += 1)
        {
            for (int y = 0; y < tilesmap.GetLength(1); y += 1)
            {
                Debug.Log(tilesmap[x, y]);
            }
        }
    }

    void Update()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        tilesmap = new bool[width, height];

        for (int x = 0; x < tilesmap.GetLength(0); x += 1)
        {
            for (int y = 0; y < tilesmap.GetLength(1); y += 1)
            {
                Gizmos.DrawWireCube(new Vector3(x, y, 0), Vector3.one);
            }
        }
    }
}
