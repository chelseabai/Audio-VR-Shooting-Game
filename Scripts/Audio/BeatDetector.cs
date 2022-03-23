using UnityEngine;
using System.Collections;

[RequireComponent (typeof (AudioSource))]
public class BeatDetector : MonoBehaviour {
    
    public GameObject beatTarget;
    public static float spawnTime = 3;
    public int targetsInRoom;
    bool canSpawn = false;
    public GameObject origin;

    // Audio
    public AudioSource song;
    private bool Beated;
    private float[] historyBuffer = new float[43];
    private int SamplesSize = 1024;
    public static float InstantSpec;
    public static float AverageSpec;
    public static float Variance;
    public static float Constant;
    public static float timeRemaining = -1000;
    
    // Use this for initialization
    void Start () {
        song = GetComponent<AudioSource>();
        Beated = false;
    }
        
    // Update is called once per frame
    void Update () {
            detectBeat();     
    }

    //spawn beat target when a beat is detected
    void SpawnTargets(bool canSpawn){
        if (canSpawn){
            //generate targets around the player within a given range
            float upperRange = 10.0f;
            float x = Random.Range(-upperRange, upperRange) + origin.transform.position.x;
            float y = Random.Range(1.5f, 5.0f) + origin.transform.position.y;
            float z = Random.Range(-upperRange, upperRange) + origin.transform.position.z;
            float distance = (x-origin.transform.position.x)*(x-origin.transform.position.x) + (z-origin.transform.position.z)*(z-origin.transform.position.z);
            distance = Mathf.Pow(distance,0.5f);
            if (distance < 4.0f){
                x = 4.0f/distance * x;
                z = 4.0f/distance * z;
            }
            beatTarget.transform.localScale = new Vector3(0.25f,0.25f,0.25f);
            Instantiate(beatTarget, new Vector3 (x,y,z), Quaternion.identity);
            targetsInRoom ++;
        }
        canSpawn = false;
    }

    // void TargetDestroy(){
    //     Destroy(gameObject);
    //     targetsInRoom --;
    // }

    void detectBeat(){
        //compute instant sound energy
        InstantSpec = sumStereo(song.GetSpectrumData(SamplesSize, 0, FFTWindow.Hamming));  //Rafa
        
        //compute local average sound evergy
        AverageSpec = (SamplesSize / historyBuffer.Length) * sumLocalEnergy(historyBuffer);  //Rafa
        
        Variance = VarianceAdder(historyBuffer) / historyBuffer.Length;  

        Constant = (float)((-0.0025714 * Variance) + 1.5142857);
        
        float[] shiftingHistoryBuffer = new float[historyBuffer.Length]; // make a new array and copy all the values to it
        
        for (int i = 0; i < (historyBuffer.Length - 1); i++) { // shift the array one slot to the right
            shiftingHistoryBuffer[i+1] = historyBuffer[i]; //fill the empty slot with the new instant sound energy
        }
        
        shiftingHistoryBuffer [0] = InstantSpec;
        
        for (int i = 0; i < historyBuffer.Length; i++) {
            historyBuffer[i] = shiftingHistoryBuffer[i]; //then we return the values to the original array
        }
        
        if (InstantSpec > (Constant * AverageSpec)) { // now we check if we have a beat
            if (timeRemaining < 0){
                if(!Beated) {
                    Debug.Log("Beat");
                    Beated = true;  
                    canSpawn = true;  
                    SpawnTargets(canSpawn); 
                }
                timeRemaining = 0.5f;   
            } 
        } 
        else {
            if(Beated) {
                Beated = false;
            }
            Debug.Log("No Beat");
            timeRemaining -= Time.deltaTime;
        } 
        canSpawn = false;
    }

    float sumStereo(float[] Channel) {
        float e = 0;
        for (int i = 0; i < 50; i++) {
            float ToSquare = Channel[i];
            e += (ToSquare * ToSquare);
        }
        return e;
    }

    float sumLocalEnergy(float[] Buffer) {
        float E = 0;
        for (int i = 0; i < Buffer.Length; i++) {
            float ToSquare = Buffer[i];
            E += (Buffer[i] * Buffer[i]);
        }
        return E;
    }

    float VarianceAdder (float[] Buffer) {
        float VarSum = 0;
        for (int i = 0; i < Buffer.Length; i++) { 
            float ToSquare = Buffer[i] - AverageSpec;
            VarSum += (ToSquare * ToSquare);
        }
        return VarSum;
    }
            
}
