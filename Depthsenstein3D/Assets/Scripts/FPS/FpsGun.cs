using UnityEngine;

public class FpsGun : MonoBehaviour
{
    private Animator anim;

    private float lastShot = 0.0f;
    private bool reloading = false;

    private FpsManager.Gun gun;

    public void Init(FpsManager.Gun gun) {
        this.gun = gun;
        gun.CurrentStatus.CurrentAmmo = gun.Config.InitialAmmo;
        fillMagazine();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        //Invoke("DebugShoot", 2.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot() {
        if (ReadyToFire()) {
            anim.Play("Shoot", -1, 0.0f);
            lastShot = Time.time;
            gun.CurrentStatus.AmmoInMagazine--;
            gun.CurrentStatus.CurrentAmmo--;
            lastShot = Time.time;
        }
    }

    public void Reload() {
        if (ReadyToReload()) {
            anim.Play("Reload");
        }
    }

    public bool ReadyToFire() {
        return lastShot < Time.time - 1.0f / gun.Config.FireRate && gun.CurrentStatus.AmmoInMagazine > 0 && gun.CurrentStatus.CurrentAmmo > 0;
    }

    public bool ReadyToReload() {
        return !reloading && gun.CurrentStatus.AmmoInMagazine < gun.Config.MagazineSize && gun.CurrentStatus.CurrentAmmo > gun.CurrentStatus.AmmoInMagazine;
    }

    public void GunReloaded() {
        reloading = false;
    }

    public void MagazineFilled() {
        fillMagazine();
    }

    public void Stow() {
        anim.Play("Stow");
    }

    public void Arm() {
        anim.Play("Arm");
    }

    private void fillMagazine() {
        gun.CurrentStatus.AmmoInMagazine = Mathf.Min(gun.CurrentStatus.CurrentAmmo, gun.Config.MagazineSize);
    }
}
