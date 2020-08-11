using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
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

    [Header("Level Variables")]
    public int currentLevel = -1;
    public int score = 0;
    public float scoreIncrementRate;
    public List<LevelVariables> levelVariables;

    [Header("Game UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI startText, gameoverText;

    [Header("Day/Night Cycle")]
    public Material daySkybox;
    public Material nightSkybox;
    public GameObject dayPPV, nightPPV;
    public int nightLevel, dayLevel;

    private int prevHighScore;
    private float obstacleSpawnMinOffset, obstacleSpawnMaxOffset, obstacleDistance;
    private Transform lastSpawnedGround, lastSpawnedObstacle, prevSpawnedObstacle;
    [SerializeField]
    private List<GameObject> spawnedGroundList, spawnedObstacleList;
    private RaptorController raptorController;

    private static GameController _instance;
    public static GameController Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        raptorController = raptor.GetComponent<RaptorController>();
        
        LevelUp();
        ChangeDayNightCycle(0);

        SpawnGroundRandomly();
        SpawnGroundRandomly();

        SpawnObstacle();
        SpawnObstacle();

        startText.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(false);
        gameoverText.gameObject.SetActive(false);
        prevHighScore = PlayerPrefs.HasKey("HighScore") ? PlayerPrefs.GetInt("HighScore") : 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(raptor.position, lastSpawnedGround.position) < groundSpawnDistance) SpawnGroundRandomly();
        if (Vector3.Distance(raptor.position, lastSpawnedObstacle.position) < obstacleDistance) SpawnObstacle();

        if (spawnedGroundList.Count > groundSpawnedListLimit) DeleteGround();
        if (spawnedObstacleList.Count > obstacleSpawnedListLimit) DeleteObstacle();

        //Level up logic
        if (currentLevel + 1 != levelVariables.Count && score > levelVariables[currentLevel + 1].scoreBarrier)
        {
            LevelUp();
        }
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

    public void LevelUp()
    {
        currentLevel++;

        raptorController.jumpTime = levelVariables[currentLevel].jumpTime;
        raptorController.walkSpeed = levelVariables[currentLevel].walkSpeed;
        raptorController.jumpSpeed = levelVariables[currentLevel].jumpSpeed;

        obstacleSpawnMinOffset = levelVariables[currentLevel].obstacleSpawnMinOffset;
        obstacleSpawnMaxOffset = levelVariables[currentLevel].obstacleSpawnMaxOffset;
        obstacleDistance = levelVariables[currentLevel].obstacleDistance;

        if (currentLevel == dayLevel) ChangeDayNightCycle(0);
        if (currentLevel == nightLevel) ChangeDayNightCycle(1);
    }

    public void StartScoreCounter()
    {
        scoreText.gameObject.SetActive(true);
        startText.gameObject.SetActive(false);
        InvokeRepeating("ScoreIcrement", 0.1f, scoreIncrementRate);
    }

    void ScoreIcrement() 
    { 
        score++;
        scoreText.text = "HI " + IntToString(prevHighScore) + "   " + IntToString(score);
    }

    string IntToString(int _number)
    {
        return _number > 99999 ? _number.ToString() : (_number > 9999 ? "0" + _number.ToString()
            : (_number > 999 ? "00" + _number.ToString() : (_number > 99 ? "000" + _number.ToString()
            : (_number > 9 ? "0000" + _number.ToString() : "00000" + _number.ToString()))));
    }

    public void ChangeDayNightCycle(int _index)
    {
        RenderSettings.skybox = _index == 0 ? daySkybox : nightSkybox;
        //StartCoroutine(LerpMat(_index == 0 ? daySkybox : nightSkybox));
        dayPPV.SetActive(_index == 0);
        nightPPV.SetActive(_index == 1);
    }

    public IEnumerator LerpMat(Material targetMat, float overTime = 2f)
    {
        float startTime = Time.time;
        while (Time.time < startTime + overTime)
        {
            RenderSettings.skybox.Lerp(RenderSettings.skybox, targetMat, (Time.time - startTime) / overTime);
            yield return null;
        }
        //RenderSettings.skybox = targetMat;
    }

    public void GameOver()
    {
        raptorController.PlaySFX(1);
        raptorController.raptorStatus = RaptorController.RaptorStatus.Dead;
        raptorController.raptorAnimator.enabled = false;

        CancelInvoke("ScoreIcrement");

        gameoverText.gameObject.SetActive(true);

        if (!PlayerPrefs.HasKey("HighScore"))
            PlayerPrefs.SetInt("HighScore", score);
        else
            PlayerPrefs.SetInt("HighScore", score > prevHighScore ? score : prevHighScore);
    }
    
}

[System.Serializable]
public class LevelVariables
{
    public string levelNo;
    public float jumpTime, walkSpeed, jumpSpeed;
    public float obstacleSpawnMinOffset, obstacleSpawnMaxOffset, obstacleDistance;
    public int scoreBarrier;
}
