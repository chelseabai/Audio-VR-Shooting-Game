using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Missed : MonoBehaviour
{
    // Start is called before the first frame update
    public Text missedText;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        missedText.text = DestroyTarget.missed.ToString();
        
    }
}
