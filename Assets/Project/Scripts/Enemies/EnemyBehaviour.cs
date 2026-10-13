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
    [SerializeField] private Animator _enemyAnimator;
    [SerializeField]private SpriteRenderer _spriteRenderer;
    [SerializeField]private SpriteRenderer _exclamationRenderer;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private bool _isDead = false;

    private bool _playerDetected;
    private bool _isAttacking;
    private Rigidbody2D _rb;
    private Collider2D _collider;

    public Action<EnemyBehaviour> OnDeath;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _rb = GetComponent<Rigidbody2D>();
        _exclamationRenderer.enabled = false;
    }

    private void Update()
    {
        LineOfSight();
        UpdatePosition();
    }

    private void LineOfSight()
    {
        if (_isDead) return;

        Ray2D ray = new Ray2D(transform.position, lookDirection);
        hit = Physics2D.Raycast(ray.origin, ray.direction, 10f, _playerLayerMask);
        if(hit)
        {
            if(hit.collider.CompareTag("Player"))
            {
                if (_playerDetected) return;
                StartCoroutine(ShowExclamation());
                _playerDetected = true;
                _isAttacking = true;
                StartCoroutine(Shoot());
            }
            else
            {
                _playerDetected = false;
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
        _enemyAnimator.SetBool("isDead", _isDead);
        _collider.enabled = false;
        _rb.bodyType = RigidbodyType2D.Static;
        
        StartCoroutine(Death());
    }

    IEnumerator Shoot()
    {

        if (_isDead) yield break;

         while (!_isDead && _playerDetected) 
         {
            GameObject bullet = Instantiate(_bulletPrefab, new Vector3(_muzzle.position.x, _muzzle.position.y, 0), Quaternion.identity);
            _enemyAnimator.SetBool("isAttacking", _isAttacking);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(lookDirection*10f, ForceMode2D.Impulse);
            yield return new WaitForSecondsRealtime(1f);
                

         }
        _isAttacking = false;
        _enemyAnimator.SetBool("isAttacking", false);
    }

    IEnumerator Death()
    {
        _spriteRenderer.DOFade(0, 1.5f);
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
        yield return null;
    }
}
