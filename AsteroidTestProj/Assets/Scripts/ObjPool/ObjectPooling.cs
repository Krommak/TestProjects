using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling<T> where T : MonoBehaviour
{
    public T pref { get; }
    public Transform container { get; }

    public List<T> pool;

    public ObjectPooling(T prefab, int count)
    {
        this.pref = prefab;
        this.container = null;
        
        this.CreatePool(count);
    }

    public ObjectPooling(T prefab, int count, Transform container)
    {
        this.pref = prefab;
        this.container = container;
        
        this.CreatePool(count);
    }

    private void CreatePool(int count)
    {
        pool = new List<T>();

        for (int i = 0; i < count; i++)
        {
            this.CreateObject(false);
        }
    }

    private T CreateObject(bool isActive = false)
    {
        var createdObject = Object.Instantiate(this.pref, this.container);
        createdObject.gameObject.SetActive(isActive);
        this.pool.Add(createdObject);
        return createdObject;
    }

    public bool HasFreeElement(out T element)
    {
        foreach (var obj in pool)
        {
            if (!obj.gameObject.activeInHierarchy)
            {
                element = obj;
                obj.gameObject.SetActive(true);
                return true;
            }
        }

        element = null;
        return false;
    }

    public T GetFreeElement()
    {
        if (this.HasFreeElement(out var element))
        {
            return element;
        }
        else
        {
            return this.CreateObject(true);
        }
    }

    public int GetActiveElementsNum()
    {
        int count = 0;
        foreach(var obj in pool)
        {
            if(obj.gameObject.activeInHierarchy)
            {
                count++;
            }
        }
        return count;
    }

    public List<GameObject> GetActiveObjects()
    {
        List<GameObject> res = new List<GameObject>();

        foreach(var obj in pool)
        {
            if(obj.gameObject.activeInHierarchy)
            {
                res.Add(obj.gameObject);
            }
        }

        return res;
    }
}
