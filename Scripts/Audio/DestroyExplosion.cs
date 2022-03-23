using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyExplosion : MonoBehaviour
{
    // Start is called before the first frame update
    float lifetime = 0.7f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (lifetime < 0){
            Destroy(gameObject);
        }
        lifetime -= Time.deltaTime; 
    }
}
