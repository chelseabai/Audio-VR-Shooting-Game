using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiate512cubes : MonoBehaviour
{
    public GameObject _sampleCubePrefab;
    GameObject[] _sampleCube = new GameObject[512];
    public float _maxScale;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 160; i++){
            GameObject _instanceSampleCube = (GameObject)Instantiate (_sampleCubePrefab);
            _instanceSampleCube.transform.position = this.transform.position;
            _instanceSampleCube.transform.parent = this.transform;
            _instanceSampleCube.name = "SampleCube" + i;
            this.transform.eulerAngles = new Vector3 (0, -2.25f * i, 0);
            _instanceSampleCube.transform.position = Vector3.forward * 5;
            _sampleCube[i] = _instanceSampleCube;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 160; i++){
            if (_sampleCube != null){
                _sampleCube[i].transform.localScale = new Vector3(0.01f,10f,0.01f);
                // Debug.Log(FrequencyDetector._samples[i]);
                _sampleCube[i].transform.localScale = new Vector3(0.01f,(FrequencyDetector._samples[i]*_maxScale+0.02f),0.01f);
            }
        }
    }
}