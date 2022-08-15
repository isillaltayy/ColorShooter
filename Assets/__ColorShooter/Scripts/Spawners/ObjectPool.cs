using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spawners
{
    [Serializable]
    public class ObjectToPool
    {
        public string name;
        public GameObject gameObject;
        public int amount;
        public Transform parent;
    }

    [Serializable]
    public class PooledObject
    {
        public string name;
        public GameObject gameObject;
        public Transform transform;
        public Rigidbody rigidbody;
    }

    public class ObjectPool : MonoBehaviour
    {
        public static ObjectPool instance;
        public List<ObjectToPool> objectToPool;
        public Queue<PooledObject> pooledObjectsQ;
        public Dictionary<string, Queue<PooledObject>> poolDictionary;

        public bool isPoolSet;
        public int amountToExpand;

        private void Awake()
        {
            instance = this;
            DontDestroyOnLoad(this);
        }

        public void StartPool()
        {
            poolDictionary = new Dictionary<string, Queue<PooledObject>>();
            foreach (var item in objectToPool)
            {
                pooledObjectsQ = new Queue<PooledObject>();
                for (var i = 0; i < item.amount; i++)
                {
                    var obj = Instantiate(item.gameObject, item.parent);

                    obj.SetActive(false);

                    Rigidbody rb = null;
                    if (TryGetComponent(out Rigidbody r))
                        rb = r;

                    pooledObjectsQ.Enqueue(new PooledObject()
                    {
                        name = item.name,
                        gameObject = obj,
                        transform = obj.transform,
                        rigidbody = rb,
                    });
                }

                poolDictionary.Add(item.name, pooledObjectsQ);
            }

            isPoolSet = true;
        }

        public PooledObject GetPooledObject(string objectName)
        {
            if (!poolDictionary.ContainsKey(objectName))
            {
                return null;
            }

            var obj = poolDictionary[objectName].Dequeue();
            if (obj.gameObject.activeSelf)
                return GetPooledObject(objectName);

            obj.gameObject.transform.rotation = Quaternion.identity;

            if (obj.rigidbody != null)
            {
                obj.rigidbody.velocity = Vector3.zero;
                obj.rigidbody.angularVelocity = Vector3.zero;
                obj.transform.rotation = Quaternion.identity;
            }

            return obj;
        }
        public void TakeBack(PooledObject obj)
        {
            obj.gameObject.SetActive(false);
            var objectName = obj.name;
            poolDictionary[objectName].Enqueue(obj);
        }
    }
}