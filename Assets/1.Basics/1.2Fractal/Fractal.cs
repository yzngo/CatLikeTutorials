using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fractal : MonoBehaviour {
    public int maxDepth = 4;
    public float childScale = 4;

    [Range(0,1)]
    public float spawnProbability;
    public Mesh[] meshes;
    public Material material;
    public float maxRotationSpeed;
    public float maxTwist;
//----------------------------------------------------------
    private int depth;
    private float rotationSpeed;
    private Material[,] materials;

    private static Vector3[] childDirections = {
		Vector3.up,
		Vector3.right,
		Vector3.left,
        Vector3.forward,
        Vector3.back
	};

	private static Quaternion[] childOrientations = {
		Quaternion.identity,
		Quaternion.Euler(0f, 0f, -90f),
		Quaternion.Euler(0f, 0f, 90f),
		Quaternion.Euler(90f, 0f, 0f),
		Quaternion.Euler(-90f, 0f, 0f)
	};

    //These two functions are called before the first Update method and 
    //there is no performance difference between them. 
    //I would say that Awake is used to initialize all objects (like a constructor), 
    //and Start is used to link the objects or do something before a game starts.

    private void Awake() {

    }
    void Start() {
        if (materials == null) {
            InitializeMaterials();
        }
        gameObject.AddComponent<MeshFilter>().mesh = meshes[Random.Range(0, meshes.Length)];
        gameObject.AddComponent<MeshRenderer>().material = materials[depth, Random.Range(0,2)];
        if (depth < maxDepth) {
            StartCoroutine(CreateChildren());
        }
        rotationSpeed = Random.Range(-maxRotationSpeed, maxRotationSpeed);
        transform.Rotate(Random.Range(-maxTwist, maxTwist), 0f, 0f);
    }

	private void InitializeMaterials () {
		materials = new Material[maxDepth + 1, 2];
		for (int i = 0; i <= maxDepth; i++) {
            float t = i / (maxDepth - 1f);
            t *= t;
			materials[i, 0] = new Material(material);
			materials[i, 0].color = Color.Lerp(Color.white, Color.yellow, t);

			materials[i, 1] = new Material(material);
			materials[i, 1].color = Color.Lerp(Color.white, Color.cyan, t);
		}
        materials[maxDepth, 0].color = Color.cyan;
        materials[maxDepth, 1].color = Color.red;
        //Lerp == linear interpolation
	}
    private IEnumerator CreateChildren() {
        //调用次序
        // 1. 创建子对象
        // 2. 附加,创建Fractal组件, 并调用Awake() OnEnable() 方法
        // 4. 调用Initialize()
        // 5. 下一帧调用 Start()
        for (int i = 0; i < childDirections.Length; i++) {
            if (Random.value < spawnProbability) {
                yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
                new GameObject("Fractal Child")
                    .AddComponent<Fractal>()
                    .Initialize(this, i);
            }
        }
    }
    private void Initialize(Fractal parent, int childIndex) {
        meshes = parent.meshes;
        materials = parent.materials;
        maxDepth = parent.maxDepth;
        depth = parent.depth + 1;
        childScale = parent.childScale;
        transform.parent = parent.transform;
        transform.localScale = Vector3.one * childScale;
        transform.localPosition = childDirections[childIndex] * (0.5f + 0.5f * childScale);
        transform.localRotation = childOrientations[childIndex];
        spawnProbability = parent.spawnProbability;
        maxRotationSpeed = parent.maxRotationSpeed;
        maxTwist = parent.maxTwist;
    }

    void Update() {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}