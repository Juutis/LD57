using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{

    public static UIManager main;
    void Awake()
    {
        if (main != null && main != this)
        {
            Destroy(gameObject);
        }
        else
        {
            main = this;
        }
    }

    [SerializeField]
    private UIShowDialog uiShowDialogPrefab;
    [SerializeField]
    private Transform uiShowDialogContainer;

    [SerializeField] private FadeInOut fader;


    [SerializeField]
    private UIHudPart HUDHealth;
    [SerializeField]
    private UIHudPart HUDAmmo;
    [SerializeField]
    private UIHudPart HUDGun;
    [SerializeField]
    private UIHudPart HUDScore;
    [SerializeField]
    private UIHudPart HUDInventory;

    private int score = 0;

    void Start()
    {
        HUDHealth.SetValue(100);
        SetAmmoInstant(0);
        AddScore(0);
    }


    public void ShowMessage(string message, UnityAction showCallback, UnityAction hideCallback)
    {
        UIShowDialog uiShowDialog = Instantiate(uiShowDialogPrefab, uiShowDialogContainer);
        uiShowDialog.Show(message, showCallback, hideCallback);
    }

    public void ShowMessage(string message)
    {
        Time.timeScale = 0f;
        UIShowDialog uiShowDialog = Instantiate(uiShowDialogPrefab, uiShowDialogContainer);
        uiShowDialog.Show(message, delegate { }, delegate
        {
            Time.timeScale = 1f;
        });
    }

    public void SetHealth(int health)
    {
        if (health < 0) {
            health = 0;
        }
        HUDHealth.SetValueLerped(health);
    }


    public void SetAmmoInstant(int ammo)
    {
        if (ammo < 0)
        {
            ammo = 0;
        }
        HUDAmmo.SetValue(ammo);
    }


    public void SetAmmoLerped(int ammo)
    {
        if (ammo < 0) {
            ammo = 0;
        }
        HUDAmmo.SetValueLerped(ammo);
    }

    public void SetGun(FpsManager.Gun gun)
    {
        HUDGun.SetGun(gun);
    }

    public void UpdateGunKeys() {
        HUDGun.UpdateGunKeys();
    }

    public void AddScore(int delta)
    {
        score += delta;
        HUDScore.SetValueLerped(score);
    }

    public void AddKeyToInventory(LockedDoorKey key)
    {
        HUDInventory.AddKey(key);
    }

    public void RemoveKeyFromInventory(LockedDoorKey key)
    {
        HUDInventory.RemoveKey(key);
    }

    public void FadeOut()
    {
        fader.FadeOut();
    }

    public void FadeIn()
    {
        fader.FadeIn();
    }
}
