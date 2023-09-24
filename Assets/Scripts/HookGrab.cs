using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookGrab : MonoBehaviour
{
    [SerializeField] private PlayerMove player;

    private void OnCollisionEnter(Collision collision)
    {
        GetComponent<Rigidbody>().isKinematic = true;
        player.hookGrabbed = true;
    }
}
