using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour {

    public static ObjectPooler SharedInstance;
    public List<GameObject> pooledObjects;
    public GameObject objectToPool;
    public int amountToPool;

    // Use this for initialization
    void Start () 
    {
        pooledObjects = new List<GameObject>();

        for (int i = 0; i < amountToPool; i++) 
        {
            GameObject obj = (GameObject)Instantiate(objectToPool);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }
	
	// Update is called once per frame
	void Update () 
    {
		
	}

    void Awake() 
    {
        SharedInstance = this;
    }

    public GameObject GetPooledObject() 
    {
        // iterate over pooled objects list
        foreach (GameObject obj in pooledObjects)
        {
            // check if current object is active in scene
            if (!obj.activeInHierarchy)
                return obj;
        }
        // if there are currently no inactive objects, return nothing
        return null;
    }

}
