using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{

    public static UIManager main;
    void Awake()
    {
        if (main != null && main != this) {
            Destroy(gameObject);
        }
        main = this;
    }

    [SerializeField]
    private UIShowDialog uiShowDialogPrefab;
    [SerializeField]
    private Transform uiShowDialogContainer;


    [SerializeField]
    private UIHudPart HUDHealth;
    [SerializeField]
    private UIHudPart HUDAmmo;
    [SerializeField]
    private UIHudPart HUDScore;
    [SerializeField]
    private UIHudPart HUDInventory;

    void Start()
    {
        SetHealth(100);
        SetAmmo(0);
        SetScore(0);
    }


    public void ShowMessage(string message, UnityAction showCallback, UnityAction hideCallback) {
        UIShowDialog uiShowDialog = Instantiate(uiShowDialogPrefab, uiShowDialogContainer);
        uiShowDialog.Show(message, showCallback, hideCallback);
    }

    public void ShowMessage(string message)
    {
        Time.timeScale = 0f;
        UIShowDialog uiShowDialog = Instantiate(uiShowDialogPrefab, uiShowDialogContainer);
        uiShowDialog.Show(message, delegate {}, delegate {
            Time.timeScale = 1f;
        });
    }

    public void SetHealth(int health) {
        HUDHealth.SetValue($"{health}%");
    }
    public void SetAmmo(int ammo) {
        HUDAmmo.SetValue($"{ammo}");
    }
    public void SetScore(int score) {
        HUDScore.SetValue($"{score}");
    }
    public void AddKeyToInventory(LockedDoorKey key) {
        HUDInventory.AddKey(key);
    }
}
