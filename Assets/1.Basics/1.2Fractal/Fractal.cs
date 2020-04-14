using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fractal : MonoBehaviour
{
    public Mesh mesh;
    public Material material;

    //These two functions are called before the first Update method and 
    //there is no performance difference between them. 
    //I would say that Awake is used to initialize all objects (like a constructor), 
    //and Start is used to link the objects or do something before a game starts.
    
    private void Awake() {
        
    }
    void Start()
    {
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = material;    
    }

    void Update()
    {
        
    }
}
