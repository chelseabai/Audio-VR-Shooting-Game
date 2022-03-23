using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class Weapon : MonoBehaviour
{
    // [SerializeField] protected float shootingForce;
    protected float shootingForce;
    [SerializeField] protected Transform bulletSpawn;
    // [SerializeField] private float recoilForce;
    private float recoilForce;
    // [SerializeField] private float damage;
    private float damage;

    private Rigidbody rigidBody;
    private XRGrabInteractable interactableWeapon;

    public bool gunInHand = false;

    protected virtual void Awake()
    {
        interactableWeapon = GetComponent<XRGrabInteractable>();
        rigidBody = GetComponent<Rigidbody>();
        SetupInteractableWeaponEvents();
    }

    private void SetupInteractableWeaponEvents()
    {
        interactableWeapon.onSelectEntered.AddListener(PickUpWeapon);
        interactableWeapon.onSelectExited.AddListener(DropWeapon);
        interactableWeapon.onActivate.AddListener(StartShooting);
        interactableWeapon.onDeactivate.AddListener(StopShooting);
    }

    // void Update() {

    //     if (gunInHand){
    //         if(AudioPeer._Amplitude > 0.2) {
    //             StartShooting(interactableWeapon);
    //         } else {
    //             StopShooting(interactableWeapon);
    //         }
    //     }
    // }

    private void PickUpWeapon(XRBaseInteractor interactor)
    {
        interactor.GetComponent<MeshHider>().Hide();
        gunInHand = true;
    }
 
    private void DropWeapon(XRBaseInteractor interactor)
    {
        interactor.GetComponent<MeshHider>().Show();
        gunInHand = false;
    }

    protected virtual void StartShooting(XRBaseInteractor interactor)
    {
        Debug.Log("Shooting Started.");
    }

    protected virtual void StopShooting(XRBaseInteractor interactor)
    {

    }

    protected virtual void Shoot()
    {
        ApplyRecoil();
    }

    private void ApplyRecoil()
    {
        rigidBody.AddRelativeForce(Vector3.back * recoilForce, ForceMode.Impulse);
    }

    public float GetShootingForce()
    {
        return shootingForce;
    }

    public float GetDamage()
    {
        return damage;
    }
}