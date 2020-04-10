using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    private static readonly float PI = Mathf.PI;
    private static readonly GraphFunction[] functions = {
        Torus, Sphere, SineFunction, Sine2DFunction, MultiSineFunction, MultiSine2DFunction, Ripple, Cylinder};
//---------------------------------------------------------------------------------------------------
    [SerializeField] private GraphFunctionName functionName;
    [SerializeField] private Transform pointPrefab;

    [Range(10,1000)]
    [SerializeField] private int resolusion = 10;

    [Range(1,100)]
    [SerializeField] private int xRange = 10;
//---------------------------------------------------------------------------------------------------
    //List表示二维数组的方法
    List<List<Transform>> _points = new List<List<Transform>>();        //List.1
    private bool _modeChanged = true;
    private int _curZResolusion = 1;
    private static int _curXResolusion = 0;
    private static int _curXRange = 0;
    private static GraphMode _curMode = GraphMode.Mode1D;
    private static GraphMode _newMode = GraphMode.Mode1D;

//---------------------------------------------------------------------------------------------------
    private void Update() {

        SpawnPoints();
        UpdateGraph();
    }

    private void SpawnPoints() {
        if (_curXResolusion != resolusion || _curXRange != xRange || _curMode != _newMode){
            _curMode = _newMode;
            _curZResolusion = _curMode == GraphMode.Mode1D ? 1 : resolusion;
            _curXResolusion = resolusion;
            _curXRange = xRange;
            foreach(var points in _points) {
                foreach(var point in points)
                Destroy(point.gameObject);
            }
            _points = new List<List<Transform>>();

            Vector3 scale = Vector3.one * 0.2f;
            Vector3 position = Vector3.zero;

            for(int z = 0; z < _curZResolusion; z++) {
                var points = new List<Transform>();             //List.2
                for(int x = 0; x < resolusion; x++) {
                    Transform point = Instantiate(pointPrefab, transform);
                    point.localScale = scale;
                    points.Add(point);                                  //List.3

                    //定义x轴 [0, resolution] -> [-xRange, xRange]
                    position.x = ((x+1.0f) * xRange * 2 / resolusion - xRange);
                    position.z = ((z+1.0f) * xRange * 2 / resolusion - xRange);
                    point.localPosition = position;
                    //var pos = position.x/(xRange*2)+0.5f;
                    //point.GetComponent<MeshRenderer>().material.color = new Color(pos, pos, pos); 
                }
                _points.Add(points);                            //List.4
            }
        }
    }
    
    private void UpdateGraph()
    {
        GraphFunction f = functions[(int)functionName];
        var t = Time.time;
        for(int z = 0; z < _curZResolusion; z++) {
            var v = ((z+1.0f) * xRange * 2 / resolusion - xRange);
            for(int x = 0; x < resolusion; x++){
                var u = ((x+1.0f) * xRange * 2 / resolusion - xRange);
                Transform point = _points[z][x];
                Vector3 position = point.localPosition;
                point.localPosition = f(u, v, t);
            }
        }
    }

    //Although the SineFunction and MultiSineFunction are part of Graph, 
    //they are effectively self-contained.

    //Because static methods aren't associated with object instances, 
    //the compiled code doesn't have to keep track of 
    //which object you're invoking the method on. 
    //This means that static method invocations are a bit faster, 
    //but it's usually not significant enough to worry about.
    static Vector3 SineFunction(float x, float z, float t)
    {
        _newMode = GraphMode.Mode1D;
        Vector3 p;
        p.x = x;
        p.y = Mathf.Sin(PI *(x + t));
        p.z = z;
        return p;
    }

    static Vector3 Sine2DFunction(float x, float z, float t)
    {
        _newMode = GraphMode.Mode2D;
        float y = Mathf.Sin(PI * (x + t));
        y += Mathf.Sin(PI * (z + t));
        y *= 0.5f;
        return new Vector3(x, y, z);
    }

    //能用乘法就不要用除法, 乘法效率比除法高
    static Vector3 MultiSineFunction(float x, float z, float t) 
    {
        _newMode = GraphMode.Mode1D;
        float y = Mathf.Sin(PI * (x + t));
        y += Mathf.Sin(2f * PI * (x + t));
        y *= 0.5f;
        return new Vector3(x, y, z);
    }

    static Vector3 MultiSine2DFunction(float x, float z, float t)
    {
        _newMode = GraphMode.Mode2D;
        float y = 4f * Mathf.Sin(PI * (x + z + t * 0.5f));
        y += Mathf.Sin(PI * (x + t));
        y += Mathf.Sin(2f * PI * (z + 2f * t)) * 0.5f;
        y *= 1f / 5.5f;
        return new Vector3(x, y, z);
    }

    static Vector3 Ripple(float x, float z, float t)
    {
        _newMode = GraphMode.Mode2D;
        float d = Mathf.Sqrt(x * x + z * z);        // square root 平方根
        float y = Mathf.Sin(PI * (4f * d - t));
        y /= 1f + 10f * d;
        return new Vector3(x, y, z);
    }
    
    static Vector3 Cylinder(float u, float v, float t)
    {
        _newMode = GraphMode.Mode2D;
        float r = 0.8f + Mathf.Sin(PI * (6f * u + 2f * v + t)) * 0.2f;
        Vector3 p;
        p.x = r * Mathf.Sin(PI * u);
        p.y = v;
        p.z = r * Mathf.Cos(PI * u);
        return p;
    }

    static Vector3 Sphere(float u, float v, float t)
    {
        _newMode = GraphMode.Mode2D;
        Vector3 p;
        float r = Time.time % 5;
        float s = r * Mathf.Cos(PI * 0.5f * v);
        p.x = s * Mathf.Sin(PI * u);
        p.y = r * Mathf.Sin(PI * 0.5f * v);
        p.z = s * Mathf.Cos(PI * u);
        return p;
    }

    static Vector3 Torus(float u, float v, float t)
    {
        _newMode = GraphMode.Mode2D;
        Vector3 p;
        float r1 = 1f;
        float r2 = 0.5f;
        float s = r2 * Mathf.Cos(PI * v) + r1;
        p.x = s * Mathf.Sin(PI * u);
        p.y = r2 * Mathf.Sin(PI * v);
        p.z = s * Mathf.Cos(PI * u);
        return p;
    }
}
