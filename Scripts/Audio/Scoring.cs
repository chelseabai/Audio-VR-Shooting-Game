using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoring : MonoBehaviour
{
    // Start is called before the first frame update
    public Text scoreText;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = DestroyTarget.numberHit.ToString();
        
    }
}
