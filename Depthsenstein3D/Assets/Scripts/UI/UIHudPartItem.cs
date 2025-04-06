using UnityEngine;
using UnityEngine.UI;

public class UIHudPartItem : MonoBehaviour
{
    [SerializeField]
    private Image imgIcon;

    private LockedDoorKey key;
    public LockedDoorKey Key { get { return key; } }


    public void Initialize(LockedDoorKey key)
    {
        this.key = key;
        imgIcon.sprite = key.GetSprite();
    }

    public void Kill() {
        Destroy(gameObject);
    }
}
