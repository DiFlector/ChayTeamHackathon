using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class BulletEnemy : MonoBehaviour
{
    [SerializeField] private float bulletLife;
    private GameObject _player;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            Destroy(this.gameObject);
        }
    }

    public void Start()
    {
        _player = GameObject.FindWithTag("Player");
    }

    public void Update()
    {
        transform.LookAt(_player.transform.position);
    }

    private void Awake()
    {
        Destroy(gameObject, bulletLife);
    }
}
