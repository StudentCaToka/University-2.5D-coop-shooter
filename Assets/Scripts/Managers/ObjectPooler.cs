﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    #region Singleton
    public static ObjectPooler Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                obj.transform.parent = this.transform;
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }


    public GameObject SpawnFromPool(string tag, Vector3 position, Vector3 targetPosition, PlayerController instigator)
    {

        if (!poolDictionary.ContainsKey(tag))
        {
            return null;
        }

        GameObject objToSpawn = poolDictionary[tag].Dequeue();

        objToSpawn.SetActive(true);
        objToSpawn.GetComponent<SimpleProjectile>().InitProjectile(position, targetPosition, instigator);

        IPooledObject pooledObject = objToSpawn.GetComponent<IPooledObject>();
        if (pooledObject != null)
        {
            pooledObject.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(objToSpawn);

        return objToSpawn;
    }

    public void PutBackOnStack(string tag, GameObject gameObject)
    {
        gameObject.SetActive(false);
        gameObject.transform.position = this.transform.position;
        poolDictionary[tag].Enqueue(gameObject);
    }
}
