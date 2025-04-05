using UnityEngine;

public class FpsGun : MonoBehaviour
{
    private Animator anim;

    private float lastShot = 0.0f;
    private bool reloading = false;

    private FpsManager.Gun gun;
    private Transform bulletOrigin;

    public void Init(FpsManager.Gun gun, Transform bulletOrigin) {
        this.gun = gun;
        this.bulletOrigin = bulletOrigin;
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
        if (gun.CurrentStatus.AmmoInMagazine == 0 && lastShot < Time.time - 1.0f / gun.Config.FireRate && ReadyToReload() && !reloading) {
            Reload();
        }
    }

    public void Shoot() {
        if (ReadyToFire()) {
            anim.Play("Shoot", -1, 0.0f);
            lastShot = Time.time;
            gun.CurrentStatus.AmmoInMagazine--;
            gun.CurrentStatus.CurrentAmmo--;
            lastShot = Time.time;
            fireBullet();
        }
    }

    public void Reload() {
        if (ReadyToReload()) {
            anim.Play("Reload");
            reloading = true;
        }
    }

    public bool ReadyToFire() {
        return lastShot < Time.time - 1.0f / gun.Config.FireRate && gun.CurrentStatus.AmmoInMagazine > 0 && gun.CurrentStatus.CurrentAmmo > 0 && !reloading;
    }

    public bool ReadyToReload() {
        return !reloading && gun.CurrentStatus.AmmoInMagazine < gun.Config.MagazineSize && gun.CurrentStatus.CurrentAmmo > gun.CurrentStatus.AmmoInMagazine;
    }

    public void GunReloaded() {
        Debug.Log("RELOADED");
        reloading = false;
    }

    public void MagazineFilled() {
        fillMagazine();
    }

    public void Stow() {
        anim.Play("Stow");
        reloading = false;
    }

    public void Arm() {
        anim.Play("Arm");
        reloading = false;
    }

    private void fillMagazine() {
        gun.CurrentStatus.AmmoInMagazine = Mathf.Min(gun.CurrentStatus.CurrentAmmo, gun.Config.MagazineSize);
    }

    private void fireBullet() {
        for(var i = 0; i < gun.Config.ProjectileCount; i++) {
            var dir = bulletOrigin.forward;
            var inAccuracy = Random.Range(0.0f, 1.0f) * gun.Config.AccuracyDegrees;
            var randomRoll = Random.Range(0.0f, 360.0f);
            dir = Quaternion.AngleAxis(inAccuracy, bulletOrigin.up) * dir;
            dir = Quaternion.AngleAxis(randomRoll, bulletOrigin.forward) * dir;
            if (Physics.Raycast(bulletOrigin.position, dir, out RaycastHit hitInfo, 1000f, ~0)) {
                var effect = Instantiate(FpsManager.Main.HitEffect);
                effect.transform.position = hitInfo.point;
            }
        }
    }
}
