
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    private List<string> levels = new() { "Level1", "DirectorsOffice", "Level2", "Garage", "RoadToUnderground", "Underworld" };
    private int currentLevelNum = 0;
    public int CurrentLevelNum { get { return currentLevelNum; } }

    [SerializeField]
    private GameObject currentLevel = null;

    private GameObject nextLevel = null;

    public GameObject CurrentLevel { get { return currentLevel; } }

    AsyncOperation sceneLoad;

    private UnityAction sceneLoadCallback;
    private UnityAction elevatorFinishedCallback;

    private float elevatorDuration = 1f;
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
            ScreenShake.Instance.Shake(0.1f);
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
        MapGenerator.main.Player.FreezeControls();
        FpsManager.Main.FreezeControls();
        if (currentLevelNum == 0) {
            MusicManager.main.StartMusic(MusicType.Elevator);
        } else {
            MusicManager.main.SwitchMusic(MusicType.Elevator);
        }
        ElevatorDoors currentElevator = currentLevel.GetComponentInChildren<ElevatorDoors>();
        MapGenerator.main.ResetKeys();
        UIManager.main.ClearInventory();
        MapGenerator.main.Player.ElevatorRotate(currentElevator.transform.position, delegate
        {
            LevelStats stats = MapGenerator.main.Player.Stats.CalculateCurrentLevelStats();


            if (currentElevator == null)
            {
                Debug.LogError("ElevatorDoors not found!");
                return;
            }

            currentElevator.CloseDoors(delegate
            {
            });

            GameObject elevatorContainer = GameObject.FindGameObjectWithTag("Finish");
            elevatorContainer.tag = "Elevator";

            sceneLoad = SceneManager.LoadSceneAsync(levels[currentLevelNum + 1], LoadSceneMode.Additive);
            // check level num here if you want ambience
            if (currentLevelNum == 0) {
                MusicManager.main.FadeOutAmbience();
            }
            sceneLoadCallback = delegate
            {
                LevelManager.main.SetCurrentLevel(currentLevelNum + 1);
                ElevatorToLoadedLevel(elevatorContainer, elevatorSwitch, stats);
            };
        });
    }

    public void RestartLevel()
    {
        Vector3 levelPos = currentLevel.transform.position;

        ElevatorDoors currentElevator = currentLevel.GetComponentInChildren<ElevatorDoors>();
        currentElevator.CloseDoors(delegate { });
        MapGenerator.main.Player.ResetPlayer(playerSpawnPos, playerSpawnRot);
        FpsManager.Main.ResetGuns();
        Destroy(currentLevel.gameObject);
        GameObject elevatorContainer = GameObject.FindGameObjectWithTag("Elevator");
        MapGenerator.main.ResetKeys();
        UIManager.main.ClearInventory();

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

    public void ElevatorToLoadedLevel(GameObject elevatorContainer, MapPrefab elevatorSwitchPrefab, LevelStats stats)
    {
        ElevatorDoors currentElevator = currentLevel.GetComponentInChildren<ElevatorDoors>();

        int levelHeight = -3;
        int elevatorTravelDistance = -3;

        nextLevel = MapGenerator.main.gameObject;

        MapManager mapManager = nextLevel.GetComponent<MapManager>();
        MapPrefab spawn = mapManager.GetSpawnPoint();
        if (spawn == null)
        {
            //Debug.Log("nO SPAWN");
        }
        Vector3 diff = elevatorSwitchPrefab.transform.position - spawn.transform.position;
        currentLevelTransform = currentLevel.transform.GetComponent<MapGenerator>().Container;
        nextLevelTransform = nextLevel.transform;

        nextLevelTransform.position = nextLevelTransform.position + new Vector3(diff.x, levelHeight, diff.z);

        MoveElevator(-elevatorTravelDistance, delegate
        {
            elevatorContainer.transform.parent = nextLevel.transform;
            currentLevel.gameObject.SetActive(false);
            Destroy(currentLevel.gameObject);
            MapGenerator.main.InitAINavigation();

            if (currentLevelNum == 1) {
                ElevatorFinalStep(currentElevator);
            } else {
                UIManager.main.ShowLevelStats(stats, delegate
                {
                    ElevatorFinalStep(currentElevator);
                });
            }
        });

    }

    private void ElevatorFinalStep (ElevatorDoors currentElevator) {
        currentElevator.OpenDoors(delegate
            {
                playerSpawnPos = MapGenerator.main.Player.transform.position;
                playerSpawnRot = MapGenerator.main.Player.transform.rotation;
                FpsManager.Main.SaveSpawnGuns();

                MapGenerator.main.Player.RestoreControls();
                FpsManager.Main.RestoreControls();
                if (currentLevelNum > 1)
                {
                    MusicManager.main.SwitchMusic(MusicType.Game);
                }
                else
                {
                    MusicManager.main.FadeOutMusic();
                }
                SoundManager.main.PlaySound(GameSoundType.ElevatorDing);

                //Debug.Log("Doors opened");

                currentLevel = nextLevel;
                MapGenerator.main.StartEnemies();
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
