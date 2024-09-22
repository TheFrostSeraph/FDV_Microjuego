using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    
    // The object to pool (e.g., a bullet prefab)
    public GameObject objectToPool;
    public int poolSize = 15;  // Number of objects in the pool

    // List to hold the pooled objects
    private List<GameObject> objectPool;

    void Start()
    {
        // creamos y llenamos la pool
        objectPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(objectToPool);
            obj.SetActive(false);  // Deactivate the object
            objectPool.Add(obj);   // Add to the pool
        }
    }

    // Method to get an inactive object from the pool
    public GameObject GetPooledObject()
    {
        // Buscamos y devolvemos un objeto en desuso
        for (int i = 0; i < objectPool.Count; i++)
        {
            if (!objectPool[i].activeInHierarchy)
            {
                return objectPool[i];
            }
        }

        // si estuviese vacia, hacer más
        GameObject newObj = Instantiate(objectToPool);
        newObj.SetActive(false);
        objectPool.Add(newObj);
        return newObj;
    }
}
