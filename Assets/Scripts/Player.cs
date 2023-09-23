using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    [SerializeField] private float _deathTime;

    [SerializeField] private Slider _healthBar;

    [SerializeField] private int _maxHealth;
    public int _health;
    [SerializeField] private int _deathShot;


    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _footClip;
    [SerializeField] private AudioClip[] _deathClip;
    [SerializeField] private AudioClip[] _takeHit;


    void Start()
    {
        Time.timeScale = 1;
        _health = _maxHealth;
        _healthBar.maxValue = _maxHealth;
        _healthBar.value = _health;
        _audioSource = GetComponent<AudioSource>();

        Cursor.visible = false;
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


         if(!_audioSource.isPlaying)
         {
            _audioSource.PlayOneShot(_footClip[0]);
         }

        if (!_audioSource.isPlaying)
        {
            _audioSource.PlayOneShot(_footClip[0]);
        }
        if (!_audioSource.isPlaying)
        {
            _audioSource.PlayOneShot(_footClip[0]);
        }

        if (!_audioSource.isPlaying)
        {
            _audioSource.PlayOneShot(_footClip[0]);
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

}
