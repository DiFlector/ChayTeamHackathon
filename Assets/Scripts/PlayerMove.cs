using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    public int _hp;

    [SerializeField] float _speed;
    [SerializeField] float DeploymentHeight;
    public GameObject _camera;

    [SerializeField] private float cx;
    [SerializeField] private float cy;
    [SerializeField] private float mouseSensivity;

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _footClip;
    [SerializeField] private AudioClip[] _deathClip;

    [SerializeField] private bool _canJump;

    void Start()
    {
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

        if (_hp <= 0)
        {
            if (!_audioSource.isPlaying)
            {
                _audioSource.PlayOneShot(_deathClip[0]);
            }
            
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

    void Jump()
    {
        if (Input.GetKey(KeyCode.Space) && _canJump == true)
        {
            _canJump = false;
            GetComponent<Rigidbody>().AddForce(0, 400, 0);
        }
    }

}
