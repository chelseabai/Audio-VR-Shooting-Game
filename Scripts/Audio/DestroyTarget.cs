using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTarget : MonoBehaviour
{
    float speed;
    float xRotation;
    float yRotation;
    float zRotation;
    bool isHit = false;
    bool destroyed = false;
    bool disappeared = false;
    Material _material;
    float[] r = new float[9] {1.717647f, 3.14f, 3.140001f, 0.09244396f, 0.08004143f, 0.08004142f, 0.3871f, 1.287572f, 1.7424f};
    float[] g = new float[9] {0.1254902f, 0.1703471f, 1.86515f, 1.273992f, 1.52f, 0.2167468f, 0.429654f, 0.2173626f, 1.6027f};
    float[] b = new float[9] {0.1254902f, 0f, 0f, 0.07812218f, 0.78f, 1.273993f,1.09434f,1.588995f, 1.78120f};
    int colorIndex = 0;
    public static float numberHit = 0;
    public static float missed = 0;
    public static float distance;
    public GameObject Explosion;

    public AudioSource AppearSFX;
    public AudioSource DisappearSFX;
    public AudioSource DestroySFX;
    AudioLowPassFilter lowPassFilter;
    AudioReverbZone reverbZone;

    [SerializeField] public AudioClip cAppearSFX;
    [SerializeField] public AudioClip cDisappearSFX;
    [SerializeField] public AudioClip cDestroySFX;

    public Renderer rend;

    private void OnTriggerEnter(Collider other){
        Debug.Log("Hit");
        if (other.gameObject.tag == "projectile") {
            isHit = true;
        }
    }


    void Awake(){

        AppearSFX = gameObject.AddComponent<AudioSource>();
        DisappearSFX = gameObject.AddComponent<AudioSource>();
        DestroySFX = gameObject.AddComponent<AudioSource>();
        lowPassFilter = gameObject.AddComponent<AudioLowPassFilter>();
        reverbZone = gameObject.AddComponent<AudioReverbZone>();
        reverbZone.reverbPreset = AudioReverbPreset.Room;

        AppearSFX.clip = cAppearSFX;
        DisappearSFX.clip = cDisappearSFX;
        DestroySFX.clip = cDestroySFX;

        AppearSFX.playOnAwake = false;
        DisappearSFX.playOnAwake = false;
        DestroySFX.playOnAwake = false;

        AppearSFX.rolloffMode = AudioRolloffMode.Linear;
        AppearSFX.volume = 1;
        AppearSFX.spatialBlend = 1;
        AppearSFX.maxDistance = 30;
        AppearSFX.spread = 1;

        DisappearSFX.rolloffMode = AudioRolloffMode.Linear;
        DisappearSFX.volume = 1;
        DisappearSFX.spatialBlend = 1;
        DisappearSFX.maxDistance = 30;
        DisappearSFX.spread = 1;

        DestroySFX.rolloffMode = AudioRolloffMode.Linear;
        DestroySFX.volume = 1;
        DestroySFX.spatialBlend = 1;
        DestroySFX.maxDistance = 30;
        DestroySFX.spread = 1;

        AppearSFX.Play();
        Debug.Log("AppearSFX");
    }

    void Start(){
        xRotation = Random.Range(-1.0f, 1.0f);
        yRotation = Random.Range(-1.0f, 1.0f);
        zRotation = Random.Range(-1.0f, 1.0f);
        speed = Random.Range(0.005f, 0.02f);

        rend = GetComponent<Renderer>();
        rend.enabled = true;
        _material = GetComponentInChildren<MeshRenderer>().materials[0];

    }

    
    

    void Update()
    {
        float scaleMultiplier = FrequencyDetector.amplitudeRatio*2 + 1;
        float minScale = 0.8f;
        // Debug.Log(scaleMultiplier);
        scaleMultiplier = Mathf.Max(scaleMultiplier*0.75f,minScale);
        transform.localScale = new Vector3(0.25f*scaleMultiplier, 0.25f*scaleMultiplier, 0.25f*scaleMultiplier);
        // Debug.Log(scaleMultiplier);

        lowPassFilter.cutoffFrequency =(22000 - 2050 * distance);
        reverbZone.minDistance = (1.2f*distance-2);
        reverbZone.maxDistance = (1.2f*distance-1);

        Debug.Log("test");
        GameObject origin = GameObject.FindWithTag("Player");

        if (isHit)
        {
            if ((!DestroySFX.isPlaying) && (destroyed))
            {
                Destroy(gameObject);
            } else if (!DestroySFX.isPlaying)
            {
                DestroySFX.Play();
                rend.enabled = false;
                Debug.Log("DestroySFX");
                numberHit++;
                destroyed = true;
                Instantiate(Explosion, new Vector3 (transform.position.x,transform.position.y,transform.position.z), Quaternion.identity);
            }
        } else {
            distance = Vector3.Distance(origin.transform.position,transform.position);
            if (distance < (1.5 + speed*(DisappearSFX.clip.length)))
            {
                if ((!DisappearSFX.isPlaying) && (disappeared))
                {
                    Destroy(gameObject);
                    Debug.Log("DisappearDestroy");
                } else if (!DisappearSFX.isPlaying)
                {
                    DisappearSFX.Play();
                    rend.enabled = false;
                    Debug.Log("DisappearSFX");
                    disappeared = true;
                    missed++;
                }
            }            
        }
        colorIndex = FrequencyDetector.maxFreqIndex;
        transform.position = Vector3.MoveTowards(transform.position, origin.transform.position, speed);
        transform.Rotate(xRotation, yRotation, zRotation, Space.Self);
        Color _color = new Color (r[colorIndex],g[colorIndex],b[colorIndex],1.5f);        
        _material.SetColor("_EmissionColor", _color);
        
    }

}