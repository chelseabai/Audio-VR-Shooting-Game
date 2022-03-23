using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyBall : MonoBehaviour
{
    // Start is called before the first frame update
	public float _startScale, _scaleMultiplier;
    public bool _useBuffer;
    Material _material;
    public float _red, _green, _blue;
    // Start is called before the first frame update
    void Start()
    {
        _material = GetComponent<MeshRenderer>().materials[0];
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!_useBuffer){
            transform.localScale = new Vector3((AudioPeer._Amplitude * _scaleMultiplier) + _startScale,(AudioPeer._Amplitude * _scaleMultiplier) + _startScale,(AudioPeer._Amplitude * _scaleMultiplier) + _startScale);
            Color _mycolor = new Color(_red*AudioPeer._Amplitude, _green*AudioPeer._Amplitude, _blue*AudioPeer._Amplitude);
            _material.SetColor("_EmissionColor", _mycolor);
        }
        if (_useBuffer){
             transform.localScale = new Vector3((AudioPeer._AmplitudeBuffer * _scaleMultiplier) + _startScale,(AudioPeer._AmplitudeBuffer * _scaleMultiplier) + _startScale,(AudioPeer._AmplitudeBuffer * _scaleMultiplier) + _startScale);
            Color _mycolor = new Color(_red*AudioPeer._AmplitudeBuffer, _green*AudioPeer._AmplitudeBuffer, _blue*AudioPeer._AmplitudeBuffer);
            _material.SetColor("_EmissionColor", _mycolor);
        }
    	
    }
}
