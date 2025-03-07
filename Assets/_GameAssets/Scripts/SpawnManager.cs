using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] Transform NPCSpawnPoint;
    [SerializeField] NPCBehaviour NPCPrefab;

    public static SpawnManager instance { get; private set; }
    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    private void Start()
    {
        SpawnNewNpc(4);
    }
    public Vector3 GetSpawnPoint()
    {
        return NPCSpawnPoint.position;
    }
    public void SpawnNewNpc(int spawnCount)
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Instantiate(NPCPrefab, NPCSpawnPoint.position, NPCSpawnPoint.rotation);
        }        
    }
}
