using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    [SerializeField] private Transform pointPrefab;

    [Range(10,1000)]
    [SerializeField] private int resolusion = 100;

    [Range(1,100)]
    [SerializeField] private int xRange = 10;

    List<Transform> _points = new List<Transform>();
    private int curResolusion = 0;
    private int curXRange = 0;
    private void Update() {
        DisplayGraph();
    }
    private void DisplayGraph() {
        if (curResolusion != resolusion || curXRange != xRange){
            curResolusion = resolusion;
            curXRange = xRange;
            foreach(var point in _points) {
                Destroy(point.gameObject);
            }
            _points = new List<Transform>();
        } else {
            return;
        }
        Vector3 scale = Vector3.one * 0.2f;
        Vector3 position = Vector3.zero;
        for(int i = 0; i < resolusion; i++) {
            Transform point = Instantiate(pointPrefab, transform);
            _points.Add(point);
            position.x = ((i+1.0f) * xRange * 2 / resolusion - xRange);//  定义x轴    
            position.y = Mathf.Sin(position.x);
            // position.y = position.x * position.x;
            // position.z = position.x;
            //沿坐标轴往左平移一半
            point.localPosition = position;
            point.localScale = scale;
            var pos = position.x/(xRange*2)+0.5f;
            point.GetComponent<MeshRenderer>().material.color = new Color(pos, pos, pos); 
        }
    }

    void Start()
    {
        
    }

}
