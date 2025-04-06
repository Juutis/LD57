using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHudPart : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI txtTitle;
    [SerializeField]
    private TextMeshProUGUI txtValue;
    [SerializeField]
    private TextMeshProUGUI txtGunKeys;
    [SerializeField]
    private Image imgGun;
    [SerializeField]
    private Transform inventoryContainer;
    [SerializeField]
    private UIHudPartItem uiHudPartItemPrefab;

    private List<UIHudPartItem> inventory = new();

    private int oldValue;
    private int targetValue;
    private int currentValue;

    private float lerpTimer = 0f;
    private float lerpDuration = 0.2f;

    private Vector2 originalScale;
    private Vector2 originalTargetScale = new Vector2(1.3f, 1.3f);
    private Vector2 targetScale;
    private Vector2 backTargetScale = new Vector2(1f, 1f);

    private bool isLerping = false;
    [SerializeField]
    private string postfix = "";
    [SerializeField]
    private string prefix = "";


    public void SetValue(int value) {
        currentValue = value;
        txtValue.text = $"{prefix}{value}{postfix}";
    }

    public void SetValue(string value) {
        txtValue.text = $"{prefix}{value}{postfix}";
    }

    public void UpdateGunKeys() {
        txtGunKeys.enabled = true;
        string gunKeys = "";
        List<FpsManager.Gun> guns = FpsManager.Main.Guns;
        FpsManager.Gun selected = FpsManager.Main.SelectedGun;
        int index = 0;
        foreach(FpsManager.Gun gun in guns) {
            gunKeys += GetGunKey(index, selected) + "\n";
            index += 1;
        }
        txtGunKeys.text = gunKeys;
    }

    private string GetGunKey(int index, FpsManager.Gun selected) {
        FpsManager.Gun gun = FpsManager.Main.Guns[index];
        if (gun != null && gun.Available) {
            if (selected == gun) {
                return $"<color=white>{index + 1}</color>";
            }
            return $"{index+1}";
        }
        return "";
    }

    public void SetValueLerped(int newValue)
    {
        originalScale = backTargetScale;
        targetScale = originalTargetScale;
        isLerping = true;
        oldValue = currentValue;
        targetValue = newValue;
    }

    public void SetGun(FpsManager.Gun gun) {
        imgGun.enabled = true;
        imgGun.sprite = gun.Config.Sprite;
        txtTitle.text = gun.Config.Name.ToUpper();
        UpdateGunKeys();
    }

    public void RemoveKey(LockedDoorKey key)
    {
        UIHudPartItem partItem = inventory.Find(item => item.Key == key);
        inventory.Remove(partItem);
        partItem.Kill();
    }

    public void AddKey(LockedDoorKey key) {
        UIHudPartItem uiHudPartItem = Instantiate(uiHudPartItemPrefab, inventoryContainer);
        uiHudPartItem.Initialize(key);
        inventory.Add(uiHudPartItem);
    }

    void Update()
    {
        if (isLerping) {
            lerpTimer += Time.deltaTime;
            SetValue((int)Mathf.Lerp(oldValue, targetValue, lerpTimer / lerpDuration));
            txtValue.transform.localScale = Vector2.Lerp(originalScale, targetScale, lerpTimer / (lerpDuration / 2.0f));
            if (lerpTimer >= lerpDuration / 2.0f) {
                originalScale = txtValue.transform.localScale;
                targetScale = backTargetScale;
            }
            if (lerpTimer >= lerpDuration) {
                originalScale = backTargetScale;
                SetValue(targetValue);
                isLerping = false;
                lerpTimer = 0f;
            }
        }
    }
}
