using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UILevelStats : MonoBehaviour
{
    [SerializeField]
    private Transform statContainer;
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private UILevelSingleStat uiLevelSingleStatPrefab;

    private List<UILevelSingleStat> singleLevelStats = new();


    UnityAction hideCallback;

    private bool isShown = false;
    private bool isHiding = false;

    public void Initialize(LevelStats levelStats, UnityAction hideCallback) 
    {
        foreach(var stat in singleLevelStats) {
            Destroy(stat.gameObject);
        }
        this.hideCallback = hideCallback;
        animator.Play("levelStatsShow");
        Debug.Log("levelStats");
        foreach (var stat in levelStats.Stats)
        {
//            Debug.Log($"{stat.Name} {stat.Value} / {stat.Max}");
            UILevelSingleStat singleStat = Instantiate(uiLevelSingleStatPrefab, statContainer);
            singleStat.Initialize(stat);
            singleLevelStats.Add(singleStat);
        }
    }

    public void ShowFinished() {
        isShown = true;
    }

    public void Hide() {
        if (!isShown) {
            return;
        }
        isHiding = true;
        animator.Play("levelStatsHide");
    }

    public void HideFinished() {
        isHiding = false;
        isShown = false;
        hideCallback();
    }

    void Update()
    {
       if (!isHiding && isShown && Input.GetKeyDown(KeyCode.E)) {
           Hide();
       }
    }


}
