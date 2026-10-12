using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.TryGetComponent<Player>(out Player player);
            player.OnPlayerDeath(true);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.TryGetComponent<EnemyBehaviour>(out EnemyBehaviour enemy);
            enemy.OnEnemyDeath(true);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
