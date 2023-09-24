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
        StartCoroutine(Rand());
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

    IEnumerator Rand()
    {
        print(1);
        yield return new WaitForSeconds(1);
        print(2);
        switch (Random.Range(0, 9))
        {
            case 8:
                skyRotateDir = true;
                break;
            case 7:
                worldRotateSpeed = Random.Range(1.0f, 25.0f);
                break;
            case 6:
                skyRotateDir = false;
                break;
            case 5:
                worldRotateDir = false;
                break;
            case 4:
                worldRotateDir = true;
                break;
            case 3:
                dimension = false;
                break;
            case 2:
                dimension = true;
                break;
            default:
                break;
        }
        print(3);
        StartCoroutine(Rand());
    }
}
