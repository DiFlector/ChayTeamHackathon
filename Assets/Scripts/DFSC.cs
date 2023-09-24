using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
        switch (Random.Range(0, 11))
        {
            case 10:
                skyRotateDir = true;
                break;
            case 9:
                worldRotateSpeed = Random.Range(1.0f, 2.0f);
                break;
            case 8:
                skyRotateDir = false;
                break;
            case 7:
                worldRotateDir = false;
                break;
            case 6:
                worldRotateDir = true;
                break;
            case 5:
                rotateWorld = true;
                break;
            case 4:
                rotateWorld = false;
                break;
            case 3:
                dimension = false;
                break;
            case 2:
                dimension = true;
                break;
            case 1:
                skyRotateSpeed = Random.Range(3.0f, 6.0f);
                break;
            default:
                break;
        }


        if (Random.Range(0, 10) == 5)
            
        if (Random.Range(0, 10) == 6)
            
        if(Random.Range(0, 10) == 7)
            
        if(Random.Range(0, 10) == 5)
        
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
