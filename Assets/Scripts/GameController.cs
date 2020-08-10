using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public List<GameObject> groundPrefabs, obstaclePrefabs;
    public Transform environmentParent, obstaclesParent, raptor;

    [Header("Ground Spawn")]
    public int groundIndex = 0;
    public int groundSpawnedListLimit;
    public float groundPlacementOffset, groundSpawnDistance; 

    [Header("Obstacle Spawn")]
    public int obstacleIndex = 0;
    public int obstacleSpawnedListLimit;
    public float obstacleSpawnMinOffset, obstacleSpawnMaxOffset, obstacleDistance;

    private Transform lastSpawnedGround, lastSpawnedObstacle, prevSpawnedObstacle;
    [SerializeField]
    private List<GameObject> spawnedGroundList, spawnedObstacleList;

    // Start is called before the first frame update
    void Start()
    {
        SpawnGroundRandomly();
        SpawnGroundRandomly();

        SpawnObstacle();
        SpawnObstacle();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(raptor.position, lastSpawnedGround.position) < groundSpawnDistance) SpawnGroundRandomly();
        if (Vector3.Distance(raptor.position, lastSpawnedObstacle.position) < obstacleDistance) SpawnObstacle();

        if (spawnedGroundList.Count > groundSpawnedListLimit) DeleteGround();
        if (spawnedObstacleList.Count > obstacleSpawnedListLimit) DeleteObstacle();
    }

    public void SpawnGroundRandomly()
    {
        int i = Random.Range(0, groundPrefabs.Count);
        GameObject ground = Instantiate(groundPrefabs[i], environmentParent);
        ground.SetActive(true);
        spawnedGroundList.Add(ground);
        lastSpawnedGround = ground.transform;
        lastSpawnedGround.position = new Vector3(groundIndex * groundPlacementOffset, 0, 0);

        groundIndex++;
    }

    void DeleteGround()
    {
        Destroy(spawnedGroundList[0]);
        spawnedGroundList.RemoveAt(0);
    }

    public void SpawnObstacle()
    {
        int i = Random.Range(0, obstaclePrefabs.Count);
        GameObject obstacle = Instantiate(obstaclePrefabs[i], obstaclesParent);
        spawnedObstacleList.Add(obstacle);
        lastSpawnedObstacle = obstacle.transform;
        lastSpawnedObstacle.position = new Vector3((prevSpawnedObstacle == null ? 0 : prevSpawnedObstacle.position.x) 
            + Random.Range(obstacleSpawnMinOffset, obstacleSpawnMaxOffset),
            obstacle.name.Contains("Cactus") ? -0.5f : Random.Range(1.5f, 3.5f), 0);

        obstacleIndex++;
        prevSpawnedObstacle = lastSpawnedObstacle;
    }

    void DeleteObstacle()
    {
        Destroy(spawnedObstacleList[0]);
        spawnedObstacleList.RemoveAt(0);
    }
}
