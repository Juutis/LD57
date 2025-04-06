using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField]
    private int gunIndex;

    public int GunIndex { get { return gunIndex; } }

    private Sprite sprite;

    public Sprite Sprite { get { return sprite; } }

    public void Initialize(Sprite sprite)
    {
        this.sprite = sprite;
    }
}
