using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DFSC : MonoBehaviour
{
    public Material skybox;
    public GameObject SpaceWorld;


    public bool dimension = false;
    public float i = 1f;
    public float j = 1f;

    public bool rotateWorld = false;

    public bool skyRotateDir = false;
    public bool worldRotateDir = false;

    public float skyRotateSpeed = 3f;
    public float worldRotateSpeed = 1f;

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
    private void FixedUpdate()
    {
        
        if(skyRotateDir)
            i = i + 1f;
        else
            i = i - 1f;

        if (rotateWorld)
            j = 1;
        else 
            j = 0;

        if (worldRotateDir)
            SpaceWorld.transform.rotation *= Quaternion.Euler(0, j * Time.fixedDeltaTime * worldRotateSpeed, 0);
        else
            SpaceWorld.transform.rotation *= Quaternion.Euler(0, -j * Time.fixedDeltaTime * worldRotateSpeed, 0);

        RenderSettings.skybox.SetFloat("_Rotation", i * Time.fixedDeltaTime * skyRotateSpeed);
    }
}
