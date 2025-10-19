using UnityEngine;

public class Basket : MonoBehaviour
{
    private GameManager manager;

    void Start()
    {
        manager = FindObjectOfType<GameManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            manager.GameOver();
        }
    }
}
