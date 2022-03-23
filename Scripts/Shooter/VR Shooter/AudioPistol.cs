using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System;
using System.Linq;

public class AudioPistol : Weapon
{
    [SerializeField] private float fireRate;
    // private float fireRate = 10;
    [SerializeField] private Projectile bulletPrefab;

    private WaitForSeconds wait;

    public XRBaseControllerInteractor controller;

    public ParticleSystem muzzleFlash;

    private ParticleSystem _aProjectile;
    private TrailRenderer _aProjectileTrail;
    private SphereCollider _aProjectileCollider;
    private Gradient colorGradient;

    public float projectileSize;
    public float _shootingForce;


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
        while (gunInHand)
        {
            // Shoot();
            // Debug.Log(AudioPeer._AmplitudeNoRatio);
            // if (AudioPeer._AmplitudeNoRatio > 0.1){
            float noisegate = 0.007f;
            if (AudioPeer._mainFreqAmp > noisegate) { // loudness gate to fire weapon
                float prevAmp = noisegate;
                // int i = 0;
                // while (i < 10) {
                //     i++;
                // }
                float currAmp = AudioPeer._mainFreqAmp;
                while (currAmp > prevAmp) {
                    prevAmp = currAmp;
                    currAmp = AudioPeer._mainFreqAmp;
                    getProjectileProperties();
                    // yield return null;
                }
                Shoot();
                // muzzleFlash.Emit(1);
                yield return wait;getProjectileProperties
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
        controller = (XRBaseControllerInteractor)GetComponent<XRGrabInteractable>().selectingInteractor;
        float hapticForce = (projectileSize + _shootingForce / 10) / 2;
        if (hapticForce > 1) {hapticForce = 1;}
        controller.SendHapticImpulse(hapticForce, 0.05f);
    }

    protected override void StopShooting(XRBaseInteractor interactor)
    {
        base.StopShooting(interactor);
        StopAllCoroutines();
    }

    private void getProjectileProperties() {
        _shootingForce = Mathf.Pow(AudioPeer._mainFreqAmp * 1000, 3f) / 10000 + 1; // multiply amplitude by 1000 so it's guaranteed to be above 1, cube for making louder things louder, divide by 10000 to bring it back to a usable number.
        base.shootingForce = _shootingForce;
        // Debug.Log(Mathf.Pow(AudioPeer._mainFreqAmp * 1000, 3f) / 10000);
        projectileSize = 100000000 / Mathf.Pow(AudioPeer._mainFreq, 3f);
        ParticleSystem.MainModule _projectileProperties = _aProjectile.main;
        _projectileProperties.startSize = projectileSize; // set projectile size
        _aProjectileTrail.startWidth = projectileSize;  // set trail width to match projectile size
        _aProjectileTrail.endWidth = 0; // trail taper off to nothing
        _aProjectileCollider.radius = projectileSize * 0.8f / 2; // hitbox radius
        // Debug.Log(projectileSize);
        var colorOverLifetime = _aProjectile.colorOverLifetime; // set projectile color animation
        if ((AudioPeer._1FreqAmp > 0.5 * AudioPeer._mainFreqAmp) && (AudioPeer._2FreqAmp > 0.5 * AudioPeer._mainFreqAmp)) {
            colorOverLifetime.enabled = true;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.magenta, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
            );
            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);
        } else {
            colorOverLifetime.enabled = false;
        }
    }
}