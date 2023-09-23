using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{

    [SerializeField] float _speed;
    [SerializeField] float DeploymentHeight;
    public GameObject _camera;


    [SerializeField] private float _deathTime;
    [SerializeField] private float cx;
    [SerializeField] private float cy;
    [SerializeField] private float mouseSensivity;

    [SerializeField] private Slider _healthBar;

    [SerializeField] private int _maxHealth;
    public int _health;
    [SerializeField] private int _deathShot;


    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _footClip;
    [SerializeField] private AudioClip[] _deathClip;
    [SerializeField] private AudioClip[] _takeHit;

    [SerializeField] private bool _canJump;


    void Start()
    {
        Time.timeScale = 1;
        _health = _maxHealth;
        _healthBar.maxValue = _maxHealth;
        _healthBar.value = _health;
        _audioSource = GetComponent<AudioSource>();


        _canJump = true;
        Cursor.visible = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            _canJump = true;
        }
    }

    void Update()
    {
        _healthBar.value = _health;

        if (_health <= 0)
        {
            _deathShot += 1;
            if (!_audioSource.isPlaying && _deathShot <= 100)
            {
                _audioSource.PlayOneShot(_deathClip[0]);
            }
            Dead();
        }

        cx += Input.GetAxis("Mouse X") * mouseSensivity * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, cx, 0);
        cy -= Input.GetAxis("Mouse Y") * mouseSensivity * Time.deltaTime;
        cy = Mathf.Clamp(cy, -90, 90);
        _camera.transform.rotation = Quaternion.Euler(cy, cx, 0);


        if (Input.GetKey(KeyCode.Space) && _canJump)
        {
            Jump();
        }

        if (Input.GetKey(KeyCode.W))
        {
            if(!_audioSource.isPlaying && _canJump == true)
            {
                _audioSource.PlayOneShot(_footClip[0]);
            }
            transform.Translate(0, 0, _speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (!_audioSource.isPlaying && _canJump == true)
            {
                _audioSource.PlayOneShot(_footClip[0]);
            }
            transform.Translate(0, 0, _speed * Time.deltaTime * -1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            if (!_audioSource.isPlaying && _canJump == true)
            {
                _audioSource.PlayOneShot(_footClip[0]);
            }
            transform.Translate(_speed * Time.deltaTime * -1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            if(!_audioSource.isPlaying && _canJump == true)
            {
                _audioSource.PlayOneShot(_footClip[0]);
            }
            transform.Translate(_speed * Time.deltaTime, 0, 0);
        }
    }

    public void Dead()
    {
        Destroy(this, _deathTime);
    }

    public void TakeDamage(int _damage)
    {
        if (!_audioSource.isPlaying)
        {
            _audioSource.PlayOneShot(_takeHit[Random.Range(0, _takeHit.Length)]);
        }
        _health -= _damage;
    }

    void Jump()
    {
        if (Input.GetKey(KeyCode.Space) && _canJump == true)
        {
            _canJump = false;
            GetComponent<Rigidbody>().AddForce(0, 400, 0);
        }
    }

}
