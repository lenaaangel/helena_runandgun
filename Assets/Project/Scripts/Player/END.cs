using UnityEngine;

public class END : MonoBehaviour
{
    [SerializeField] private GameObject _winScreen;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _winScreen.SetActive(true);
        }
    }

}
