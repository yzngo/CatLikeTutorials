using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    public GraphFunctionName functionName;
    [SerializeField] private Transform pointPrefab;

    [Range(10,1000)]
    [SerializeField] private int resolusion = 100;

    [Range(1,100)]
    [SerializeField] private int xRange = 10;

    List<Transform> _points = new List<Transform>();
    private int curResolusion = 0;
    private int curXRange = 0;

    static private GraphFunction[] functions = {SineFunction, MultiSineFunction};
    private void Update() {
        SpawnPoints();
        UpdateGraph();
    }

    private void SpawnPoints() {
        if (curResolusion != resolusion || curXRange != xRange){
            curResolusion = resolusion;
            curXRange = xRange;
            foreach(var point in _points) {
                Destroy(point.gameObject);
            }
            _points = new List<Transform>();

            Vector3 scale = Vector3.one * 0.2f;
            Vector3 position = Vector3.zero;
            for(int i = 0; i < resolusion; i++) {

                Transform point = Instantiate(pointPrefab, transform);
                point.localScale = scale;
                _points.Add(point);

                //定义x轴 [0, resolution] -> [-xRange, xRange]
                position.x = ((i+1.0f) * xRange * 2 / resolusion - xRange);
                point.localPosition = position;

                //var pos = position.x/(xRange*2)+0.5f;
                //point.GetComponent<MeshRenderer>().material.color = new Color(pos, pos, pos); 
            }
        }
    }
    
    private void UpdateGraph()
    {
        GraphFunction f = functions[(int)functionName];
        var time = Time.time;
        for(int i = 0; i < resolusion; i++) {
            Transform point = _points[i];
            Vector3 position = point.localPosition;

            position.y = f(position.x, time);
            // position.y = MultiSineFunction(position.x, time);

            point.localPosition = position;
        }
    }

    //Although the SineFunction and MultiSineFunction are part of Graph, 
    //they are effectively self-contained.

    //Because static methods aren't associated with object instances, 
    //the compiled code doesn't have to keep track of 
    //which object you're invoking the method on. 
    //This means that static method invocations are a bit faster, 
    //but it's usually not significant enough to worry about.
    static float SineFunction(float x, float param)
    {
        return Mathf.Sin(Mathf.PI *(x + param));
    }

    static float MultiSineFunction(float x, float param) 
    {
        float y = Mathf.Sin(Mathf.PI * (x + param));
        y += Mathf.Sin(2f * Mathf.PI * (x + param)) / 2f;
        return y;
    }

}
