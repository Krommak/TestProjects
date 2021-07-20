using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Header("Количество пуль в пулле")]
    [SerializeField] private int objectsInPoll = 50;
    [Header("Префаб сферы")]
    [SerializeField] private ObjForPool prefab;
    [Header("Количество астероидов в пулле")]


    private ObjectPooling<ObjForPool> pool;

    private void Awake()
    {
        pool = new ObjectPooling<ObjForPool>(prefab, objectsInPoll, this.transform);       
    }

    public ObjectPooling<ObjForPool> GetPool()
    {
        return pool;
    }
}
