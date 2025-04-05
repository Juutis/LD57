using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    private List<string> levels = new() { "Level1", "Level2", "Level3" };
    private int currentLevelNum = 0;

    [SerializeField]
    private GameObject currentLevel = null;

    private GameObject nextLevel = null;

    private LevelInfo currentLevelInfo;
    private LevelInfo nextLevelInfo;

    public GameObject CurrentLevel { get { return currentLevel; } }

    AsyncOperation sceneLoad;

    private void Awake()
    {
        main = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentLevelInfo = currentLevel.GetComponent<LevelInfo>();
    }

    // Update is called once per frame
    void Update()
    {

        
    }

    public void LoadNextLevel()
    {
        Elevator currentElevator = currentLevel.GetComponentInChildren<Elevator>();
        if (currentElevator == null)
        {
            Debug.LogError("Elevator not found!");
            return;
        }

        currentElevator.CloseDoor();

        sceneLoad = SceneManager.LoadSceneAsync(levels[currentLevelNum + 1], LoadSceneMode.Additive);
        StartCoroutine(LerpLevels());
    }

    private IEnumerator LerpLevels()
    {
        while (!sceneLoad.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        Debug.Log("Level loaded");

        LevelInfo[] infos = (LevelInfo[])FindObjectsByType<LevelInfo>(FindObjectsSortMode.None);

        foreach (LevelInfo info in infos)
        {
            if (info != currentLevelInfo)
            {
                nextLevelInfo = info;
                nextLevel = info.gameObject;
            }
        }

        Vector3 curPos = currentLevel.transform.position;
        Vector3 targetPos = curPos - new Vector3(0, nextLevelInfo.Depth, 0);

        Vector3 nextPos = currentLevel.transform.position + new Vector3(0, nextLevelInfo.Depth, 0);
        Vector3 nextTargetPos = curPos;

        Vector3 nextLevelOffset = currentLevelInfo.ElevatorPos - nextLevelInfo.SpawnPos;

        nextLevel.transform.position += nextLevelOffset;
        nextLevel.transform.rotation = nextLevel.transform.rotation * Quaternion.Euler(0, nextLevelInfo.LevelYRotation, 0);

        float lerpSpeed = 3f;
        Debug.Log("Lerp started");

        for (float i = 0; i < 1; i += lerpSpeed * (Time.deltaTime / -nextLevelInfo.Depth))
        {
            yield return new WaitForEndOfFrame();
            currentLevel.transform.position = Vector3.Lerp(curPos, targetPos, i);
            nextLevel.transform.position = Vector3.Lerp(nextPos, nextTargetPos, i);
            Debug.Log(i);
        }
        Debug.Log("Lerp finished");

        nextLevel.transform.position = new Vector3(0, 0, 0);

        currentLevelInfo = nextLevelInfo;
        Destroy(currentLevel);
        currentLevelNum++;
        currentLevel = nextLevel;
        nextLevel = null;
    }
}
