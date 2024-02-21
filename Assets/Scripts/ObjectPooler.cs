using System.Collections.Generic;
using UnityEngine;

/**
 * Responsible for pooling objects for optimisation
 * over instantiation and deletion
 */
public class ObjectPooler : MonoBehaviour
{
    /**
     * Representing a pool we can draw from
     */
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public static ObjectPooler Instance;
    public List<Pool> Pools;
    private Dictionary<string, Queue<GameObject>> _poolDictionary;

    private void Awake() {
        Instance = this;
    }

    void Start() {
        _poolDictionary = new Dictionary<string, Queue<GameObject>>();


        foreach (Pool pool in Pools) {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++) {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            _poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation) {
        if (!_poolDictionary.ContainsKey(tag)) {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist");
            return null;
        }
        GameObject objectToSpawn = _poolDictionary[tag].Dequeue();

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        IPooledObject pooledObject = objectToSpawn.GetComponent<IPooledObject>();
        if (pooledObject != null) { pooledObject.OnObjectSpawn(); }

        _poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}
