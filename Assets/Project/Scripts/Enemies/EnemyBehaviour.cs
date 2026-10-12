using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private RaycastHit2D hit;
    [Header("Enemy Variables")]
    [SerializeField] private Transform _muzzle;
    [SerializeField] private Vector2 _standingRightOffset;
    [SerializeField] private Vector2 _standingLeftOffset;
    [SerializeField]private Vector2 lookDirection = new Vector2(1f, 0f);
    [SerializeField] private LayerMask _playerLayerMask;
    [SerializeField]private SpriteRenderer _spriteRenderer;
    [SerializeField]private SpriteRenderer _exclamationRenderer;
    [SerializeField] private GameObject _bulletPrefab;
    private bool playerDetected;
    private bool _isDead;

    public Action<EnemyBehaviour> OnDeath;

    private void Awake()
    {
        _exclamationRenderer.enabled = false;
    }

    private void Update()
    {
        LineOfSight();
        UpdatePosition();
    }

    private void LineOfSight()
    {
        Ray2D ray = new Ray2D(transform.position, lookDirection);
        hit = Physics2D.Raycast(ray.origin, ray.direction, 10f, _playerLayerMask);
        if(hit)
        {
            if(hit.collider.CompareTag("Player"))
            {
                StartCoroutine(ShowExclamation());
                playerDetected = true;
            }
            else
            {
                playerDetected = false;
            }
        }
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red);
    }

    private void UpdatePosition()
    {
        if(lookDirection.x > 0)
        {
            _spriteRenderer.flipX = false;
            _muzzle.localPosition = _standingRightOffset;
        }
        else if (lookDirection.x < 0)
        {
            _muzzle.localPosition = _standingLeftOffset;
            _spriteRenderer.flipX = true;
        }
    }

    IEnumerator ShowExclamation()
    {
        _exclamationRenderer.enabled = true;
        _exclamationRenderer.DOFade(0f, 2f);
        yield return new WaitForSecondsRealtime(2f);
        _exclamationRenderer.enabled = false;
    }

    public void OnEnemyDeath(bool isDead)
    {
        _isDead = isDead;
    }

    IEnumerator Shoot()
    {
        if(playerDetected && !_isDead)
        {
            while (!_isDead) 
            {
                Instantiate(_bulletPrefab, new Vector3(_muzzle.position.x, _muzzle.position.y, 0), Quaternion.identity);
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
