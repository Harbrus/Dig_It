using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentLists : MonoBehaviour
{
    public List<GameObject> TriggerableCamerasInTheScene;
    public List<GameObject> CameraTriggers;

    protected static ComponentLists _instance;
    protected bool _enabled;
    public static ComponentLists Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ComponentLists>();
                if (_instance == null)
                {
                    GameObject componentList = new GameObject();
                    _instance = componentList.AddComponent<ComponentLists>();
                }
            }
            return _instance;
        }
    }


    private void Awake()
    {
        CameraTriggers.Clear();
        TriggerableCamerasInTheScene.Clear();

        if (!Application.isPlaying)
        {
            return;
        }

        if (_instance == null)
        {
            //If I am the first instance, make me the Singleton
            _instance = this as ComponentLists;
            DontDestroyOnLoad(transform.gameObject);
            _enabled = true;
        }
        else
        {
            //If a Singleton already exists and you find
            //another reference in scene, destroy it!
            if (this != _instance)
            {
                Destroy(this.gameObject);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
