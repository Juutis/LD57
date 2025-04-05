using System.Collections.Generic;
using UnityEngine;

public class FpsManager : MonoBehaviour
{
    public static FpsManager Main;

    public List<Gun> Guns;
    public Gun SelectedGun;

    void Awake()
    {
        if (Main != null) {
            Destroy(gameObject);
        }
        Main = this;
        foreach(var gun in Guns) {
            gun.GunModel.Init(gun);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [System.Serializable]
    public class Gun {
        public bool Available;
        public GunConfig Config;
        public GunStatus CurrentStatus;
        public FpsGun GunModel;
    }

    [System.Serializable]
    public class GunConfig {
        public float FireRate;
        public int MagazineSize;
        public int MaxAmmo;
        public int InitialAmmo;
    }

    [System.Serializable]
    public class GunStatus {
        public int CurrentAmmo;
        public int AmmoInMagazine;
    }
}
