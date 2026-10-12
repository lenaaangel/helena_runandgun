using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        Shoot();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.TryGetComponent<Player>(out Player player);
            player.OnPlayerDeath(true);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.TryGetComponent<EnemyBehaviour>(out EnemyBehaviour enemy);
            enemy.OnEnemyDeath(true);
        }
    }

    void Shoot()
    {
        _rb.AddForce(Vector2.left, ForceMode2D.Impulse);
    }
}
