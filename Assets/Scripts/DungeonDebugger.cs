using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class DungeonDebugger : MonoBehaviour
{
    public NavMeshSurface navMeshSurface;

    void Start()
    {
        navMeshSurface.BuildNavMesh();
    }

    void OnDrawGizmos()
    {
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();
        if (navMeshData.vertices.Length == 0)
            return;

        for (int i = 0; i < navMeshData.indices.Length; i += 3)
        {
            Vector3 v1 = navMeshData.vertices[navMeshData.indices[i]];
            Vector3 v2 = navMeshData.vertices[navMeshData.indices[i + 1]];
            Vector3 v3 = navMeshData.vertices[navMeshData.indices[i + 2]];

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(v1, v2);
            Gizmos.DrawLine(v2, v3);
            Gizmos.DrawLine(v3, v1);
        }
    }
}
