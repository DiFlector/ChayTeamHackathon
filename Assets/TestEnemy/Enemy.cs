using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Enemy : MonoBehaviour
{
    public GameObject _bullet;
    public Transform _spawnPoint;

    public int _hp;

    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip[] _gun;
    [SerializeField] private AudioClip[] _deathSound;

    [SerializeField] private float _speedBullet;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _deathTime;

    [SerializeField] private BoxCollider box;

    private GameObject _player;
    private float _nextFireTime;

    void Start()
    {
        _player = GameObject.FindWithTag("Player");
        _source = GetComponent<AudioSource>();
    }


    void Update()
    {

        if(_hp <= 0 )
        {
            _fireRate = 0;
            Destroy(this.box);
            GetComponent<Animator>().SetTrigger("Death");
            Death();
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
        GetComponent<Animator>().SetTrigger("Fire");
        _source.PlayOneShot(_gun[Random.Range(0, _gun.Length)]);
        GameObject bullet = Instantiate(_bullet, _spawnPoint.position, Quaternion.identity);
        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
        bulletRigidbody.velocity = (_player.transform.position - _spawnPoint.position).normalized * _speedBullet;
        bulletRigidbody.useGravity = false;
    }

    private void Death()
    {
        _source.PlayOneShot(_deathSound[Random.Range(0, _deathSound.Length)]);
        Destroy(this.gameObject, _deathTime);
    }
}
