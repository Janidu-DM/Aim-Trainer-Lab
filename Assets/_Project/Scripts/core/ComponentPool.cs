using System.Collections.Generic;

using UnityEngine;


public class ComponentPool<T> where T : Component
{
    private readonly T _prefab;
    private readonly Transform _poolRoot;
    private readonly Queue<T> _poolQueue = new Queue<T>();

    //Constructor
    public ComponentPool(T prefab, int startingPoolCount, Transform parent = null)
    {
        _prefab = prefab;

        _poolRoot = new GameObject($"{prefab.name}_pool").transform;

        if (parent != null)
        {
            _poolRoot.SetParent(parent);
        }
        else
        {
            _poolRoot.SetParent(null);
        }

            for (int i = 0; i < startingPoolCount; i++)
            {
                CreateNewInstance();
            } 
    }


    private void CreateNewInstance()
    {
        T instance = Object.Instantiate(_prefab , _poolRoot);
        instance.gameObject.SetActive(false);
        _poolQueue.Enqueue(instance);
            
    }

    public T GetInstanceFromPool(Vector3 hitPosition , Quaternion rotation) 
    {
        T instance;

        if (_poolQueue.Count == 0)
        {
            instance = Object.Instantiate(_prefab , hitPosition,rotation);
        }
        else 
        {
            instance = _poolQueue.Dequeue();
        }
        instance.transform.position = hitPosition;
        instance.transform.rotation = rotation;
        instance.gameObject.SetActive(true);
        return instance;
    }

    public void ReturnInstanceToPool(T instance)
    {
        instance.gameObject.SetActive(false);
        _poolQueue.Enqueue(instance);
    }
}
