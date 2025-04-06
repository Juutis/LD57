using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    private List<string> levels = new() { "Level1", "Level2", "Level3" };
    private int currentLevelNum = 0;
    public int CurrentLevelNum {get {return currentLevelNum;}}

    [SerializeField]
    private GameObject currentLevel = null;

    private GameObject nextLevel = null;

    public GameObject CurrentLevel { get { return currentLevel; } }

    AsyncOperation sceneLoad;

    private UnityAction sceneLoadCallback;
    private UnityAction elevatorFinishedCallback;

    private float elevatorDuration = 5f;
    private float elevatorTimer = 0;
    private Transform currentLevelTransform;
    private Transform nextLevelTransform;
    private Vector3 nextLevelTarget;
    private Vector3 nextLevelOrigin;
    private Vector3 currentLevelTarget;
    private Vector3 currentLevelOrigin;
    private bool elevatorIsMoving = false;


    

    private void Awake()
    {
        if (main != null) {
            Destroy(gameObject);
        }
        main = this;
    }

    public void SetCurrentLevel(int level) {
        currentLevelNum = level;
    }


    // Update is called once per frame
    void Update()
    {

        if (sceneLoad != null) {
            if (sceneLoad.isDone && sceneLoadCallback != null) {
                sceneLoadCallback.Invoke();
                sceneLoadCallback = null;
                sceneLoad = null;
            }
        }
        if (elevatorIsMoving) {
            elevatorTimer += Time.deltaTime;
            currentLevelTransform.position = Vector3.Lerp(currentLevelOrigin, currentLevelTarget, elevatorTimer / elevatorDuration);
            nextLevelTransform.position = Vector3.Lerp(nextLevelOrigin, nextLevelTarget, elevatorTimer / elevatorDuration);
            if (elevatorTimer >= elevatorDuration) {
                currentLevelTransform.position = currentLevelTarget;
                nextLevelTransform.position = nextLevelTarget;
                elevatorIsMoving = false;
                elevatorFinishedCallback.Invoke();
                elevatorFinishedCallback = null;
            }
        }
    }

    public void LoadNextLevel(Vector2Int elevatorSwitchPosition)
    {
        Debug.Log("Loading next level..");
        ElevatorDoors currentElevator = currentLevel.GetComponentInChildren<ElevatorDoors>();
        if (currentElevator == null)
        {
            Debug.LogError("ElevatorDoors not found!");
            return;
        }

        currentElevator.CloseDoors(delegate {
            Debug.Log("DoorsClosed");
        });

        GameObject elevatorContainer = GameObject.FindGameObjectWithTag("Finish");
        elevatorContainer.tag = "Untagged";
        Vector2Int elevatorPoint = elevatorSwitchPosition - currentElevator.GetComponent<MapPrefab>().Position;

        sceneLoad = SceneManager.LoadSceneAsync(levels[currentLevelNum + 1], LoadSceneMode.Additive);
        sceneLoadCallback = delegate {
            LevelManager.main.SetCurrentLevel(currentLevelNum + 1);
            ElevatorToLoadedLevel(elevatorContainer, elevatorPoint);
        };
        //StartCoroutine(LerpLevels());
    }

    public void ElevatorToLoadedLevel(GameObject elevatorContainer, Vector2Int spawnPointTarget) {
        ElevatorDoors currentElevator = currentLevel.GetComponentInChildren<ElevatorDoors>();

        int levelHeight = 3;
        int elevatorTravelDistance = 3;

        nextLevel = MapGenerator.main.gameObject;

        MapManager mapManager = nextLevel.GetComponent<MapManager>();
        MapPrefab spawn = mapManager.GetSpawnPoint();
        if (spawn == null) {
            Debug.Log("nO SPAWN");
        }
        Vector2Int diff = spawnPointTarget - spawn.Position;
        currentLevelTransform = currentLevel.transform.GetComponent<MapGenerator>().Container;
        nextLevelTransform = nextLevel.transform;

        nextLevelTransform.position = nextLevelTransform.position + new Vector3(diff.x, levelHeight, diff.y);

        MoveElevator(-elevatorTravelDistance, delegate {
            elevatorContainer.transform.parent = nextLevel.transform;
            Destroy(currentLevel.gameObject);
            currentElevator.OpenDoors(delegate
            {
                Debug.Log("Doors opened");
                currentLevel = nextLevel;
            });
        });
    }

    private void MoveElevator(float distance, UnityAction finishedCallback) {
        if (elevatorIsMoving) {return;}
        nextLevelOrigin = nextLevelTransform.position;
        currentLevelOrigin = currentLevelTransform.position;
        currentLevelTarget = currentLevelOrigin + new Vector3(0, distance, 0);
        nextLevelTarget = nextLevelOrigin + new Vector3(0, distance, 0);
        elevatorIsMoving = true;
        elevatorFinishedCallback = finishedCallback;
    }

}
