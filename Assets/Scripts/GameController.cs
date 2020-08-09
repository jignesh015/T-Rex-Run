using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public List<GameObject> groundPrefabs;
    public Transform environmentParent, raptor;

    public int groundIndex = 0, spawnedListLimit;
    public float groundPlacementOffset, groundSpawnDistance;

    private Transform lastSpawnedGround;
    [SerializeField]
    private List<GameObject> spawnedGroundList;

    // Start is called before the first frame update
    void Start()
    {
        SpawnGroundRandomly();
        SpawnGroundRandomly();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(raptor.position, lastSpawnedGround.position) < groundSpawnDistance) SpawnGroundRandomly();

        if (spawnedGroundList.Count > spawnedListLimit) DeleteGround();
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
}
