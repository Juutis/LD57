using System.Collections.Generic;
using UnityEngine;

public class FpsManager : MonoBehaviour
{
    public static FpsManager Main;

    public List<Gun> Guns;
    public Gun SelectedGun = null;
    public Transform bulletOrigin;
    public GameObject HitEffect;
    public GameObject BloodEffect;

    void Awake()
    {
        SelectedGun = null;
        if (Main != null) {
            Destroy(gameObject);
        }
        Main = this;
        foreach(var gun in Guns) {
            gun.GunModel.Init(gun, bulletOrigin);
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

    public void AddAmmo(int gunIndex, int ammo)
    {
        if (Guns.Count <= gunIndex)
        {
            Debug.LogError($"No such gun {gunIndex}! Total count of guns: {Guns.Count}");
            return;
        }

        Gun gun = Guns[gunIndex];
        int currentAmmo = gun.CurrentStatus.CurrentAmmo;
        int maxAmmo = gun.Config.MaxAmmo;

        Guns[gunIndex].CurrentStatus.CurrentAmmo = Mathf.Min(maxAmmo, currentAmmo + ammo);
    }

    public void EnableGun(int gunIndex)
    {
        if (Guns.Count <= gunIndex)
        {
            Debug.LogError($"No such gun {gunIndex}! Total count of guns: {Guns.Count}");
            return;
        }

        Guns[gunIndex].Available = true;
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
        public float AccuracyDegrees;
        public int ProjectileCount;
        public float Damage;
    }

    [System.Serializable]
    public class GunStatus {
        public int CurrentAmmo;
        public int AmmoInMagazine;
    }
}
