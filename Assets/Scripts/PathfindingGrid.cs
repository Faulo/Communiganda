using NesScripts.Controls.PathFind;
using UnityEngine;

namespace Communiganda {
    public class PathfindingGrid : MonoBehaviour {
        public int width = 5;
        public int height = 5;
        bool[,] tilesmap;

        public NesScripts.Controls.PathFind.Grid grid { get; private set; }
        public static PathfindingGrid instance;

        [SerializeField] Transform backgroundTransform;

        public LayerMask obstacleMask;

        void Awake() {
            instance = this;
            SetEdgeColliders();
            // backgroundTransform.position = new Vector3((width / 2f) - .5f, (height / 2) - .5f, 0);
            //  backgroundTransform.localScale = new Vector3(width, height, 0);
        }

        void Start() {
            tilesmap = new bool[width, height];
            for (int x = 0; x < tilesmap.GetLength(0); x += 1) {
                for (int y = 0; y < tilesmap.GetLength(1); y += 1) {
                    if (Physics2D.BoxCast(new Vector2(x, y), new Vector2(.95f, .95f), 0f, Vector2.up, 1f, obstacleMask)) {
                        tilesmap[x, y] = false;
                    } else {
                        tilesmap[x, y] = true;
                    }
                }
            }
            grid = new NesScripts.Controls.PathFind.Grid(tilesmap);
            // Camera.main.transform.position = Vector3.zero + new Vector3((width - 1) / 2, (height - 1) / 2f, -100f);
        }

        void Update() {

        }

        void OnDrawGizmos() {
            if (tilesmap == null) {
                for (int i = 0; i < width; i++) {
                    for (int j = 0; j < height; j++) {
                        Gizmos.DrawWireCube(new Vector2(i, j), new Vector3(0.9f, .9f));
                    }
                }
                return;
            }

            for (int x = 0; x < tilesmap.GetLength(0); x += 1) {
                for (int y = 0; y < tilesmap.GetLength(1); y += 1) {
                    Gizmos.color = tilesmap[x, y] == true ? Color.green : Color.red;
                    Gizmos.DrawWireCube(new Vector2(x, y), new Vector3(0.9f, .9f));
                }
            }
        }

        public Point ConvertPositionToPoint(Vector2 pos) {
            var point = new Point(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
            point.x = Mathf.Clamp(point.x, 0, width - 1);
            point.y = Mathf.Clamp(point.y, 0, height - 1);
            return point;
        }

        public Point GenerateRandomTargetPointInsideGrid() {
            return new Point(Random.Range(0, width), Random.Range(0, height));
        }

        public Vector2 GenerateRandomTargetVector2InsideGrid() {
            var point = new Point(Random.Range(0, width), Random.Range(0, height));
            return new Vector2(point.x, point.y);
        }

        public Vector2[] GetWaypoints(Point from, Point to) {
            var path = Pathfinding.FindPath(grid, from, to);
            var points = new Vector2[path.Count];
            for (int i = 0; i < points.Length; i++) {
                points[i] = new Vector2(path[i].x, path[i].y);
            }
            return points;
        }

        void SetEdgeColliders() {
            float offset = .5f;
            var edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
            edgeCollider.points = new Vector2[] { new Vector2(-offset, -offset), new Vector2(-offset, height - offset),
                                              new Vector2(width -offset, height-offset), new Vector2(width-offset, -offset),new Vector2(-offset, -offset) };

        }
    }
}