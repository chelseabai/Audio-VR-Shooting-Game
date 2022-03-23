using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RaycastWeapon : Weapon
{
    [SerializeField] private float fireRate;
    private Projectile projectile;

    private WaitForSeconds wait;

    public ParticleSystem muzzleFlash;

    Ray ray;
    RaycastHit hitInfo;

    protected override void Awake()
    {
        base.Awake();
        projectile = GetComponentInChildren<Projectile>();
    }

    private void Start()
    {
        wait = new WaitForSeconds(1 / fireRate);
        projectile.Init(this);
    }

    protected override void StartShooting(XRBaseInteractor interactor)
    {
        base.StartShooting(interactor);
        StartCoroutine(ShootingCO());
    }

    private IEnumerator ShootingCO()
    {
        while (true)
        {
            Shoot();
            muzzleFlash.Emit(1);

            // ray.origin = raycastOrigin.position;
            // ray.direction = raycastOrigin.forward;
            if(Physics.Raycast(ray, out hitInfo)) {
                Debug.DrawLine(ray.origin, hitInfo.point, Color.red, 1.0f);
            }
            yield return wait;
        }
    }

    protected override void Shoot()
    {
        base.Shoot();
        projectile.Launch();
    }

    protected override void StopShooting(XRBaseInteractor interactor)
    {
        base.StopShooting(interactor);
        StopAllCoroutines();
    }
}
