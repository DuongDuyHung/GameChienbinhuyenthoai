using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public UIMap uiMap;
    // Start is called before the first frame update
    void Start()
    {
        if (uiMap != null)
        {
            uiMap.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
