using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    private List<string> levels = new() { "Level1", "DirectorsOffice", "Level2", "Level3" };
    private int currentLevelNum = 0;
    public int CurrentLevelNum { get { return currentLevelNum; } }

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

    private Vector3 playerSpawnPos = Vector3.zero;
    private Quaternion playerSpawnRot = Quaternion.identity;

    private void Awake()
    {
        if (main != null)
        {
            Destroy(gameObject);
        }
        else
        {
            main = this;
        }
    }

    public void SetCurrentLevel(int level)
    {
        currentLevelNum = level;
    }


    // Update is called once per frame
    void Update()
    {
        if (sceneLoad != null)
        {
            if (sceneLoad.isDone && sceneLoadCallback != null)
            {
                sceneLoadCallback.Invoke();
                sceneLoadCallback = null;
                sceneLoad = null;
            }
        }
        if (elevatorIsMoving)
        {
            elevatorTimer += Time.deltaTime;
            currentLevelTransform.position = Vector3.Lerp(currentLevelOrigin, currentLevelTarget, elevatorTimer / elevatorDuration);
            nextLevelTransform.position = Vector3.Lerp(nextLevelOrigin, nextLevelTarget, elevatorTimer / elevatorDuration);
            if (elevatorTimer >= elevatorDuration)
            {
                elevatorTimer = 0f;
                currentLevelTransform.position = currentLevelTarget;
                nextLevelTransform.position = nextLevelTarget;
                elevatorIsMoving = false;
                elevatorFinishedCallback.Invoke();
                elevatorFinishedCallback = null;
            }
        }
    }

    public void LoadNextLevel(MapPrefab elevatorSwitch)
    {
        Debug.Log("Loading next level..");
        ElevatorDoors currentElevator = currentLevel.GetComponentInChildren<ElevatorDoors>();
        if (currentElevator == null)
        {
            Debug.LogError("ElevatorDoors not found!");
            return;
        }

        currentElevator.CloseDoors(delegate
        {
            Debug.Log("DoorsClosed");
        });

        GameObject elevatorContainer = GameObject.FindGameObjectWithTag("Finish");
        elevatorContainer.tag = "Elevator";

        sceneLoad = SceneManager.LoadSceneAsync(levels[currentLevelNum + 1], LoadSceneMode.Additive);
        sceneLoadCallback = delegate
        {
            LevelManager.main.SetCurrentLevel(currentLevelNum + 1);
            ElevatorToLoadedLevel(elevatorContainer, elevatorSwitch);
        };
    }

    public void RestartLevel()
    {
        Vector3 levelPos = currentLevel.transform.position;

        ElevatorDoors currentElevator = currentLevel.GetComponentInChildren<ElevatorDoors>();
        currentElevator.CloseDoors(delegate { });
        MapGenerator.main.Player.ResetPlayer(playerSpawnPos, playerSpawnRot);
        Destroy(currentLevel.gameObject);
        GameObject elevatorContainer = GameObject.FindGameObjectWithTag("Elevator");

        if (elevatorContainer != null)
        {
            elevatorContainer.transform.parent = null;
        }

        sceneLoad = SceneManager.LoadSceneAsync(levels[currentLevelNum], LoadSceneMode.Additive);

        sceneLoadCallback = delegate
        {
            nextLevel = MapGenerator.main.gameObject;
            nextLevel.transform.position = levelPos;
            MapGenerator.main.InitAINavigation();

            ElevatorDoors currentElevator = nextLevel.GetComponentInChildren<ElevatorDoors>();
            UIManager.main.FadeIn();
            currentElevator.OpenDoors(delegate
            {
                currentLevel = nextLevel;
                if (elevatorContainer != null)
                {
                    elevatorContainer.transform.parent = MapGenerator.main.transform;
                }
                MapGenerator.main.StartEnemies();
            });
        };
    }

    public void ElevatorToLoadedLevel(GameObject elevatorContainer, MapPrefab elevatorSwitchPrefab)
    {
        ElevatorDoors currentElevator = currentLevel.GetComponentInChildren<ElevatorDoors>();

        int levelHeight = -3;
        int elevatorTravelDistance = -3;

        nextLevel = MapGenerator.main.gameObject;

        MapManager mapManager = nextLevel.GetComponent<MapManager>();
        MapPrefab spawn = mapManager.GetSpawnPoint();
        if (spawn == null)
        {
            Debug.Log("nO SPAWN");
        }
        Vector3 diff = elevatorSwitchPrefab.transform.position - spawn.transform.position;
        Debug.Log($"{elevatorSwitchPrefab.transform.position} - {spawn.transform.position} = {diff}");
        currentLevelTransform = currentLevel.transform.GetComponent<MapGenerator>().Container;
        nextLevelTransform = nextLevel.transform;

        nextLevelTransform.position = nextLevelTransform.position + new Vector3(diff.x, levelHeight, diff.z);

        MoveElevator(-elevatorTravelDistance, delegate
        {
            elevatorContainer.transform.parent = nextLevel.transform;
            Destroy(currentLevel.gameObject);
            MapGenerator.main.InitAINavigation();
            currentElevator.OpenDoors(delegate
            {
                playerSpawnPos = MapGenerator.main.Player.transform.position;
                playerSpawnRot = MapGenerator.main.Player.transform.rotation;
                Debug.Log("Doors opened");
                currentLevel = nextLevel;
                MapGenerator.main.StartEnemies();
            });
        });
    }

    private void MoveElevator(float distance, UnityAction finishedCallback)
    {
        if (elevatorIsMoving) { return; }
        nextLevelOrigin = nextLevelTransform.position;
        currentLevelOrigin = currentLevelTransform.position;
        currentLevelTarget = currentLevelOrigin + new Vector3(0, distance, 0);
        nextLevelTarget = nextLevelOrigin + new Vector3(0, distance, 0);
        elevatorIsMoving = true;
        elevatorFinishedCallback = finishedCallback;
    }

}
