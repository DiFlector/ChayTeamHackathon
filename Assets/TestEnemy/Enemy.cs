using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public GameObject _bullet;
    public Transform _spawnPoint;
    public Transform _spawnPoint2;

    public bool _isDroid;

    [SerializeField] private Slider _slider;
    [SerializeField] private int _maxHp;
    public int _hp;

    [SerializeField] private int _deathShot;

    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip[] _gun;
    [SerializeField] private AudioClip[] _deathSound;
    [SerializeField] private AudioClip[] _takeDamage;
    [SerializeField] private GameObject _idleSound;

    [SerializeField] private float _speedBullet;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _deathTime;
    [SerializeField] private float _range;

    [SerializeField] private BoxCollider box;

    private GameObject _player;
    private float _nextFireTime;

    void Start()
    {
        _deathShot = 0;
        _hp = _maxHp;
        _slider.maxValue = _maxHp;
        _slider.value = _hp;
        _player = GameObject.FindWithTag("Player");
        _source = GetComponent<AudioSource>();
    }


    void Update()
    {
        _slider.value = _hp;

        if (_hp <= 0)
        {
            _idleSound.SetActive(false);
            _fireRate = 0;
            _deathShot += 1;
            Destroy(this.box);
            if (!_source.isPlaying && _deathShot <= 100)
            {
                _source.PlayOneShot(_deathSound[Random.Range(0, _deathSound.Length)]);
            }
            GetComponent<Animator>().SetTrigger("Death");
            Death();
        }

        if (Time.timeScale < 1.0f)
        {
            _idleSound.SetActive(false);
        }
        else
        {
            _idleSound.SetActive(true);
        }

        transform.LookAt(_player.transform.position);

        Vector3 direction = Vector3.forward;
        Ray theRay = new Ray(transform.position, transform.TransformDirection(direction * _range));
        Debug.DrawRay(transform.position, transform.TransformDirection(direction * _range));

        if (Physics.Raycast(theRay, out RaycastHit hit, _range))
        {
            if (hit.collider.tag == "Player" && Time.time > _nextFireTime)
            {
                Fire();
                _source.PlayOneShot(_gun[Random.Range(0, _gun.Length)]);
                _nextFireTime = Time.time + 1f / _fireRate;
            }
        }
    }

    private void Fire()
    {
        if (!_isDroid)
        {
            GetComponent<Animator>().SetTrigger("Fire");
            GameObject bullet = Instantiate(_bullet, _spawnPoint.position, Quaternion.identity);
            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
            bulletRigidbody.velocity = (_player.transform.position - _spawnPoint.position).normalized * _speedBullet;
            bulletRigidbody.useGravity = false;
        }
        else
        {
            GetComponent<Animator>().SetTrigger("Fire");
            GameObject bullet = Instantiate(_bullet, _spawnPoint.position, Quaternion.identity);
            GameObject bullet2 = Instantiate(_bullet, _spawnPoint2.position, Quaternion.identity);
            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
            bulletRigidbody.velocity = (_player.transform.position - _spawnPoint.position).normalized * _speedBullet;
            bulletRigidbody.useGravity = false;
            Rigidbody bullet2Rigidbody = bullet2.GetComponent<Rigidbody>();
            bullet2Rigidbody.velocity = (_player.transform.position - _spawnPoint2.position).normalized * _speedBullet;
            bullet2Rigidbody.useGravity = false;
        }
    }

    public void TakeDamage(int _damage)
    {
        if (!_source.isPlaying)
        {
            _source.PlayOneShot(_takeDamage[Random.Range(0, _takeDamage.Length)]);
        }
        _hp -= _damage;
        _slider.value = _hp;
    }

    private void Death()
    {
        Destroy(this.gameObject, _deathTime);
    }
}
