using Unity.VisualScripting;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
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
