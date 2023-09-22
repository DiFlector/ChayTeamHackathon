using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject _bullet;
    public Transform _spawnPoint;

    public int _hp;

    [SerializeField] private float _speedBullet;
    [SerializeField] private float _fireRate;

    private GameObject _player;
    private float _nextFireTime;

    void Start()
    {
        _player = GameObject.FindWithTag("Player");
    }


    void Update()
    {

        if(_hp <= 0 )
        {
            Destroy(this.gameObject);
        }

        transform.LookAt(_player.transform.position);

        if (Time.time > _nextFireTime)
        {
            Fire();
            _nextFireTime = Time.time + 1f / _fireRate;
        }
    }

    private void Fire()
    {
        GameObject bullet = Instantiate(_bullet, _spawnPoint.position, Quaternion.identity);
        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
        bulletRigidbody.velocity = (_player.transform.position - _spawnPoint.position).normalized * _speedBullet;
        bulletRigidbody.useGravity = false;
    }
}
