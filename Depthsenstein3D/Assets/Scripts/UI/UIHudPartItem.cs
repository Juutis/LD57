using UnityEngine;
using UnityEngine.UI;

public class UIHudPartItem : MonoBehaviour
{
    [SerializeField]
    private Image imgIcon;

    public void Initialize(LockedDoorKey key)
    {
        imgIcon.sprite = key.GetSprite();
    }
}
