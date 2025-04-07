using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class FpsShooter : MonoBehaviour
{
    private FpsManager.Gun desiredGun;
    private State state = State.ARMING;

    private bool canAct = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var selectedGun = FpsManager.Main.Guns.First(it => it.Available);
        SelectGun(selectedGun);
    }


    public void FreezeControls()
    {
        canAct = false;
    }

    public void RestoreControls()
    {
        canAct = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!canAct) {
            return;
        }
        if (Input.GetKey(KeyCode.Mouse0) && state == State.READY) {
            selectedGun().GunModel.Shoot();
        }
        if (Input.GetKeyDown(KeyCode.R) && state == State.READY) {
            selectedGun().GunModel.Reload();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1) && state == State.READY) {
            SelectGun(FpsManager.Main.Guns[0]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && state == State.READY) {
            SelectGun(FpsManager.Main.Guns[1]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && state == State.READY) {
            SelectGun(FpsManager.Main.Guns[2]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && state == State.READY) {
            SelectGun(FpsManager.Main.Guns[3]);
        }
    }

    public void SelectGun(FpsManager.Gun gun) {
        if (!gun.Available) return;
        if (gun == selectedGun()) return;
        desiredGun = gun;

        state = State.STOWING;
        if (selectedGun() == null) {
            Invoke("GunStowed", 0.0f);
        } else {
            selectedGun().GunModel.Stow();
            Invoke("GunStowed", 0.25f);
        }
    }

    void GunStowed() {
        FpsManager.Main.SelectedGun = desiredGun;
        UIManager.main.SetGun(desiredGun);
        UIManager.main.SetAmmo(desiredGun);
        foreach (var g in FpsManager.Main.Guns) {
            g.GunModel.gameObject.SetActive(false);
        }
        desiredGun.GunModel.gameObject.SetActive(true);
        state = State.ARMING;
        Invoke("GunArmed", 0.25f);
        selectedGun().GunModel.Arm();
    }

    void GunArmed() {
        state = State.READY;
    }

    private FpsManager.Gun selectedGun() {
        return FpsManager.Main.SelectedGun;
    }

    enum State {
        ARMING,
        STOWING,
        READY
    }
}
