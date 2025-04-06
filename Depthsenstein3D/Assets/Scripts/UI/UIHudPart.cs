using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIHudPart : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI txtTitle;
    [SerializeField]
    private TextMeshProUGUI txtValue;
    [SerializeField]
    private Transform inventoryContainer;
    [SerializeField]
    private UIHudPartItem uiHudPartItemPrefab;

    private List<UIHudPartItem> inventory = new();

    public void Initialize(string title, string value)
    {
        txtTitle.text = title;
        txtValue.text = value;
    }

    public void SetValue(string value) {
        txtValue.text = value;
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
}
