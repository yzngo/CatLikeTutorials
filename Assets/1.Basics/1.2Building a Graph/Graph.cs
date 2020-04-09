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
        for(int i = 0; i < resolusion; i++) {
            Transform point = _points[i];
            Vector3 position = point.localPosition;
            position.y = CalcX(position.x);
            point.localPosition = position;
        }
    }

    private float CalcX(float x)
    {
        return Mathf.Sin(Mathf.PI *(x + Time.time));
    }
}
