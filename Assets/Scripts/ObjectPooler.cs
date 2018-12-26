using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPoolItem
{
    public GameObject objectToPool;
    public int amountToPool;
    public bool shouldExpand = true;
}


public class ObjectPooler : MonoBehaviour {

    public static ObjectPooler SharedInstance;
    public List<GameObject> pooledObjects;
    public List<ObjectPoolItem> itemsToPool;

    // Use this for initialization
    void Start () 
    {
        pooledObjects = new List<GameObject>();
        foreach (ObjectPoolItem item in itemsToPool)
        {
            for (int i = 0; i < item.amountToPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(item.objectToPool);
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
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

    public GameObject GetPooledObject(string tag) 
    {
        // iterate over pooled objects list
        foreach (GameObject obj in pooledObjects)
        {
            // check if current object is active in scene
            if (!obj.activeInHierarchy && tag.Equals(obj.tag))
                return obj;
        }

        foreach (ObjectPoolItem item in itemsToPool)
        {
            // if there are currently no inactive objects check shouldExpand
            // and matching tag
            if (tag.Equals(item.objectToPool.tag) && item.shouldExpand)
            {
                GameObject obj = (GameObject)Instantiate(item.objectToPool);
                obj.SetActive(false);
                pooledObjects.Add(obj);
                return obj;
            }
        }

        return null;
    }
}
