using UnityEngine;

public class Ammo : MonoBehaviour
{
    [SerializeField]
    private int gunIndex;
    [SerializeField]
    private int ammoAmount;

    public int GunIndex { get { return gunIndex; } }
    public int AmmoAmount { get { return ammoAmount; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
