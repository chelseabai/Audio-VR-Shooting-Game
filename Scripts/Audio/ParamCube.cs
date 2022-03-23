using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParamCube : MonoBehaviour
{
	public int _band;
	public float _startScale, _scaleMultiplier;
    public bool _useBuffer;
    Material _material;
    public float _emissionIntensity;
    // Start is called before the first frame update
    void Start()
    {
        _material = GetComponentInChildren<MeshRenderer>().materials[0];
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_useBuffer){
            transform.localScale = new Vector3(transform.localScale.x, (AudioPeer._audioBandBuffer[_band]*_scaleMultiplier)+_startScale, transform.localScale.z);
            Color _mycolor = new Color(2.118547f*AudioPeer._audioBandBuffer[_band]*_emissionIntensity,1.586138f*AudioPeer._audioBandBuffer[_band]*_emissionIntensity,0.04436747f*AudioPeer._audioBandBuffer[_band]*_emissionIntensity);
            _material.SetColor("_EmissionColor", _mycolor);
            Debug.Log(_material.color);
        }
        if (!_useBuffer){
            transform.localScale = new Vector3(transform.localScale.x, (AudioPeer._audioBandBuffer[_band]*_scaleMultiplier)+_startScale, transform.localScale.z);
            Color _color = new Color (AudioPeer._audioBandBuffer[_band]*_emissionIntensity,AudioPeer._audioBandBuffer[_band]*_emissionIntensity,AudioPeer._audioBandBuffer[_band]*_emissionIntensity);        
            _material.SetColor("_EmissionColor", _color);
        }
    	
    }
}
