using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public GameObject player;
    public GameObject prefabSphereMelee;
    // Start is called before the first frame update
    void Start()
    {
        GameObject sphereMelee = Instantiate(prefabSphereMelee, player.transform.position, Quaternion.identity);
        sphereMelee.transform.SetParent(player.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
