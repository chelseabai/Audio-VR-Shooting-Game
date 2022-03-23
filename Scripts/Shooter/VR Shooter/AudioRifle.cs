using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AudioRifle : Weapon
{
    // [SerializeField] private float fireRate;
    private float fireRate = 3;
    [SerializeField] private Projectile bulletPrefab;

    private WaitForSeconds wait;

    public ParticleSystem muzzleFlash;

    private ParticleSystem _aProjectile;
    private TrailRenderer _aProjectileTrail;
    private SphereCollider _aProjectileCollider;
    private Gradient colorGradient;


    // void Update() {
    //     CheckAmplitude();
    // }

    // private void CheckAmplitude(){
    //     if(base.gunInHand){
    //         if (AudioPeer._Amplitude > 0.2){
    //             base.shootingForce = AudioPeer._Amplitude;
    //             StartCoroutine(ShootingCO());
    //             StopAllCoroutines();
    //         }
    //     }
    // }

    void Start(){
        wait = new WaitForSeconds(1 / fireRate);
        _aProjectile = bulletPrefab.GetComponentInChildren<ParticleSystem>();
        _aProjectileTrail = bulletPrefab.GetComponentInChildren<TrailRenderer>();
        _aProjectileCollider = bulletPrefab.GetComponent<SphereCollider>();
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
            // Shoot();
            // Debug.Log(AudioPeer._AmplitudeNoRatio);
            // if (AudioPeer._AmplitudeNoRatio > 0.1){
            if (AudioPeer._mainFreqAmp * 10 > 0.1) { 
                getProjectileProperties();
                Shoot();
                // muzzleFlash.Emit(1);
                yield return wait;
            }else {
                yield return null;
            }
            // yield return wait;
            // yield return 0.1;
        }
    }

    protected override void Shoot()
    {
        base.Shoot();
        Projectile projectileInstance = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        projectileInstance.Init(this);
        projectileInstance.Launch();
    }

    protected override void StopShooting(XRBaseInteractor interactor)
    {
        base.StopShooting(interactor);
        StopAllCoroutines();
    }

    private void getProjectileProperties() {
        base.shootingForce = AudioPeer._mainFreqAmp * 500;
        float projectileSize = 200/(AudioPeer._mainFreq);
        ParticleSystem.MainModule _projectileProperties = _aProjectile.main;
        _projectileProperties.startSize = projectileSize; // I want this to be based on the dominant frequency
        _aProjectileTrail.startWidth = projectileSize;
        _aProjectileTrail.endWidth = 0;
        _aProjectileCollider.radius = projectileSize/2;
        Debug.Log(projectileSize);
    }
}