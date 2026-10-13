using Unity.VisualScripting;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.TryGetComponent<Player>(out Player player);
            player.OnPlayerDeath(true);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
