using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fractal : MonoBehaviour {
    public int maxDepth = 4;
    public float childScale = 4;

    public Mesh mesh;
    public Material material;

    private int depth;

    //These two functions are called before the first Update method and 
    //there is no performance difference between them. 
    //I would say that Awake is used to initialize all objects (like a constructor), 
    //and Start is used to link the objects or do something before a game starts.

    private void Awake() {

    }
    void Start() {
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = material;

        if (depth < maxDepth) {
            StartCoroutine(CreateChildren());
        }
    }

    private IEnumerator CreateChildren() {
        yield return new WaitForSeconds(0.5f);
        //调用次序
        // 1. 创建子对象
        // 2. 附加,创建Fractal组件, 并调用Awake() OnEnable() 方法
        // 4. 调用Initialize()
        // 5. 下一帧调用 Start()
        new GameObject("Fractal Child")
            .AddComponent<Fractal>()
            .Initialize(this, Vector3.up, Quaternion.identity);

        yield return new WaitForSeconds(0.5f);
        new GameObject("Fractal Child")
            .AddComponent<Fractal>()
            .Initialize(this, Vector3.right, Quaternion.Euler(0f, 0f, -90f));

        yield return new WaitForSeconds(0.5f);
        new GameObject("Fractal Child")
            .AddComponent<Fractal>()
            .Initialize(this, Vector3.left, Quaternion.Euler(0f, 0f, 90f));
    }
    private void Initialize(Fractal parent, 
                            Vector3 direction,
                            Quaternion orientation) {
        mesh = parent.mesh;
        material = parent.material;
        maxDepth = parent.maxDepth;
        depth = parent.depth + 1;
        childScale = parent.childScale;
        transform.parent = parent.transform;
        transform.localScale = Vector3.one * childScale;
        transform.localPosition = direction * (0.5f + 0.5f * childScale);
        transform.localRotation = orientation;
    }

    void Update() {

    }
}