using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{
    private NavMeshModifier navMeshModifier;

    void Start()
    {
        navMeshModifier = gameObject.AddComponent<NavMeshModifier>();
        navMeshModifier.area = 0; // Set the area to passable
        navMeshModifier.overrideArea = true; // Override the area type
    }
}
