using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExcludeFromGDecal : MonoBehaviour
{
    [Header("Needs a renderer as It changes material render queue in Runtime")]
    [SerializeField]
    int customRenderQueue = 2101;
    void Start()
    {
        GetComponent<Renderer>().sharedMaterial.renderQueue = customRenderQueue;
        Destroy(this);
    }

}
