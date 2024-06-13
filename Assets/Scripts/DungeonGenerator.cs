using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class DungeonGenerator : MonoBehaviour
{
    private List<NavMeshSurface> navMeshSurfaces;
    public class Cell
    {
        public bool visited = false;
        public bool[] status = new bool[4];
    }

    [System.Serializable]
    public class Rule
    {
        public GameObject room;
        public Vector2Int minPosition;
        public Vector2Int maxPosition;
        public bool obligatory;

        public int ProbabilityOfSpawning(int x, int y)
        {
            if (x >= minPosition.x && x <= maxPosition.x && y >= minPosition.y && y <= maxPosition.y)
            {
                return obligatory ? 2 : 1;
            }
            return 0;
        }
    }

    public Vector2Int size;
    public int startPos = 0;
    public Rule[] rooms;
    public Vector2 offset;
    public GameObject[] enemyPrefabs;
    public float enemySpawnProbability = 0.5f;
    public float dungeonHeight = 10f;

    private NavMeshSurface navMeshSurface;

    List<Cell> board;

    void Start()
    {
        GameObject dungeonParent = new GameObject("Dungeon");

        navMeshSurface = dungeonParent.AddComponent<NavMeshSurface>();
        navMeshSurface.agentTypeID = 0;
        navMeshSurface.collectObjects = CollectObjects.Volume;
        navMeshSurface.useGeometry = NavMeshCollectGeometry.RenderMeshes;

        navMeshSurface.size = new Vector3(size.x * offset.x, dungeonHeight, size.y * offset.y);
        navMeshSurface.center = new Vector3(size.x * offset.x / 2, dungeonHeight / 2, -size.y * offset.y / 2);

        MazeGenerator();
        navMeshSurface.BuildNavMesh();
    }

    void GenerateDungeon()
    {
        GameObject dungeonParent = GameObject.Find("Dungeon");
        navMeshSurfaces = new List<NavMeshSurface>();

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Cell currentCell = board[(i + j * size.x)];
                if (currentCell.visited)
                {
                    int randomRoom = -1;
                    List<int> availableRooms = new List<int>();

                    for (int k = 0; k < rooms.Length; k++)
                    {
                        int p = rooms[k].ProbabilityOfSpawning(i, j);

                        if (p == 2)
                        {
                            randomRoom = k;
                            break;
                        }
                        else if (p == 1)
                        {
                            availableRooms.Add(k);
                        }
                    }

                    if (randomRoom == -1)
                    {
                        if (availableRooms.Count > 0)
                        {
                            randomRoom = availableRooms[Random.Range(0, availableRooms.Count)];
                        }
                        else
                        {
                            randomRoom = 0;
                        }
                    }

                    var newRoom = Instantiate(rooms[randomRoom].room, new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, dungeonParent.transform).GetComponent<RoomBehavior>();
                    newRoom.UpdateRoom(currentCell.status);
                    newRoom.name += " " + i + "-" + j;

                    if (Random.value < enemySpawnProbability && enemyPrefabs.Length > 0)
                    {
                        int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);
                        var enemy = Instantiate(enemyPrefabs[randomEnemyIndex], new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, newRoom.transform);

                        var navMeshAgent = enemy.GetComponent<NavMeshAgent>();
                        if (navMeshAgent == null)
                        {
                            navMeshAgent = enemy.AddComponent<NavMeshAgent>();
                        }
                    }

                    NavMeshSurface navMeshSurface = newRoom.GetComponent<NavMeshSurface>();
                    if (navMeshSurface == null)
                    {
                        navMeshSurface = newRoom.AddComponent<NavMeshSurface>();
                    }
                    navMeshSurfaces.Add(navMeshSurface);
                }
            }
        }

        foreach (var surface in navMeshSurfaces)
        {
            surface.BuildNavMesh();
        }
    }

    void MazeGenerator()
    {
        board = new List<Cell>();

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                board.Add(new Cell());
            }
        }

        int currentCell = startPos;
        Stack<int> path = new Stack<int>();
        int k = 0;

        while (k < 1000)
        {
            k++;
            board[currentCell].visited = true;

            if (currentCell == board.Count - 1)
            {
                break;
            }

            List<int> neighbors = CheckNeighbors(currentCell);

            if (neighbors.Count == 0)
            {
                if (path.Count == 0)
                {
                    break;
                }
                else
                {
                    currentCell = path.Pop();
                }
            }
            else
            {
                path.Push(currentCell);
                int newCell = neighbors[Random.Range(0, neighbors.Count)];

                if (newCell > currentCell)
                {
                    if (newCell - 1 == currentCell)
                    {
                        board[currentCell].status[2] = true;
                        currentCell = newCell;
                        board[currentCell].status[3] = true;
                    }
                    else
                    {
                        board[currentCell].status[1] = true;
                        currentCell = newCell;
                        board[currentCell].status[0] = true;
                    }
                }
                else
                {
                    if (newCell + 1 == currentCell)
                    {
                        board[currentCell].status[3] = true;
                        currentCell = newCell;
                        board[currentCell].status[2] = true;
                    }
                    else
                    {
                        board[currentCell].status[0] = true;
                        currentCell = newCell;
                        board[currentCell].status[1] = true;
                    }
                }
            }
        }
        GenerateDungeon();
    }

    List<int> CheckNeighbors(int cell)
    {
        List<int> neighbors = new List<int>();

        if (cell - size.x >= 0 && !board[(cell - size.x)].visited)
        {
            neighbors.Add((cell - size.x));
        }

        if (cell + size.x < board.Count && !board[(cell + size.x)].visited)
        {
            neighbors.Add((cell + size.x));
        }

        if ((cell + 1) % size.x != 0 && !board[(cell + 1)].visited)
        {
            neighbors.Add((cell + 1));
        }

        if (cell % size.x != 0 && !board[(cell - 1)].visited)
        {
            neighbors.Add((cell - 1));
        }

        return neighbors;
    }
}
