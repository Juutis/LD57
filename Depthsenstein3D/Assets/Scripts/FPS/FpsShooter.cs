using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FpsShooter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var selectedGun = FpsManager.Main.Guns.First(it => it.Available);
        selectGun(selectedGun);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0)) {
            selectedGun().GunModel.Shoot();
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            selectedGun().GunModel.Reload();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            selectGun(FpsManager.Main.Guns[0]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            selectGun(FpsManager.Main.Guns[1]);
        }
    }

    private void selectGun(FpsManager.Gun gun) {
        if (!gun.Available) return;
        FpsManager.Main.SelectedGun = gun;
        foreach(var g in FpsManager.Main.Guns) {
            g.GunModel.gameObject.SetActive(false);
        }
        gun.GunModel.gameObject.SetActive(true);
    }

    private FpsManager.Gun selectedGun() {
        return FpsManager.Main.SelectedGun;
    }
}
