using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FpsManager : MonoBehaviour
{
    public static FpsManager Main;

    public List<Gun> Guns;
    public List<Gun> GunsAtSpawn;

    public Gun SelectedGun = null;
    public int SelectedGunIndexAtSpawn = -1;
    public Transform bulletOrigin;
    public GameObject HitEffect;
    public GameObject BloodEffect;
    [SerializeField]

    private FpsShooter fpsShooter;

    public void FreezeControls()
    {
        fpsShooter.FreezeControls();
    }

    public void RestoreControls()
    {
        fpsShooter.RestoreControls();
    }

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
        if (Main.SelectedGun == Guns[gunIndex]) {
            UIManager.main.SetAmmo(Main.SelectedGun);
        }
    }

    public void EnableGun(int gunIndex, Sprite sprite)
    {
        if (Guns.Count <= gunIndex)
        {
            Debug.LogError($"No such gun {gunIndex}! Total count of guns: {Guns.Count}");
            return;
        }

        var gun = Guns[gunIndex];
        if (gunIndex == 1 && !gun.Available) {
            MusicManager.main.StartMusic(MusicType.Game);
            MusicManager.main.FadeOutOfficeAmbience();
        }
        gun.Available = true;
        gun.CurrentStatus.AmmoInMagazine = Mathf.Min(gun.CurrentStatus.CurrentAmmo, gun.Config.MagazineSize);
        UIManager.main.UpdateGunKeys();
    }

    public void SaveSpawnGuns()
    {
        GunsAtSpawn = Guns.Select(gun => new Gun(gun)).ToList();
        SelectedGunIndexAtSpawn = Guns.FindIndex(x => x == SelectedGun);
    }

    public void ResetGuns()
    {
        for (int i = 0; i < Guns.Count; i++)
        {
            Guns[i].CurrentStatus = new (GunsAtSpawn[i].CurrentStatus);
            Guns[i].Available = GunsAtSpawn[i].Available;
        }

        UIManager.main.UpdateGunKeys();
        fpsShooter.SelectGun(Guns[SelectedGunIndexAtSpawn]);
        UIManager.main.SetAmmo(SelectedGun);
        UIManager.main.SetGun(SelectedGun);
    }

    [System.Serializable]
    public class Gun {
        public bool Available;
        public GunConfig Config;
        public GunStatus CurrentStatus;
        public FpsGun GunModel;

        public Gun(Gun gun)
        {
            this.Available = gun.Available;
            this.Config = gun.Config;
            this.CurrentStatus = new GunStatus(gun.CurrentStatus);
            this.GunModel = gun.GunModel;
        }
    }

    [System.Serializable]
    public class GunConfig {
        public string Name;
        public Sprite Sprite;
        public bool IsMelee;
        public bool UsesAmmo;
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

        public GunStatus(GunStatus gunStatus)
        {
            this.CurrentAmmo = gunStatus.CurrentAmmo;
            this.AmmoInMagazine = gunStatus.AmmoInMagazine;
        }
    }
}
