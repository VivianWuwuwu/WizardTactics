using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLocator : MonoBehaviour
{
    public StatusRegistry Statuses;
    public static ResourceLocator Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
