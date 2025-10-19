using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    public int fruitLevel;
    private bool merged = false;

    public AudioManager audioManager;
    public GameObject starBurstPrefab;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (merged) return;

        Fruit other = collision.gameObject.GetComponent<Fruit>();
        if (other != null && other.fruitLevel == fruitLevel && !other.merged)
        {
            merged = true;
            other.merged = true;

            Vector2 contactPoint = collision.GetContact(0).point;
            audioManager.PlaySFX();

            if (starBurstPrefab != null)
            {
                Instantiate(starBurstPrefab, contactPoint, Quaternion.identity);
            }

            GameManager manager = FindObjectOfType<GameManager>();
            if (manager != null && fruitLevel < manager.fruitPrefabs.Length - 1)
            {
                GameObject newFruit = Instantiate(manager.fruitPrefabs[fruitLevel + 1], contactPoint, Quaternion.identity);
                newFruit.GetComponent<Fruit>().audioManager = this.audioManager;
            }

            Rigidbody2D rb1 = GetComponent<Rigidbody2D>();
            Rigidbody2D rb2 = other.GetComponent<Rigidbody2D>();
            float totalMass = rb1.mass + rb2.mass;
            int gainedScore = Mathf.RoundToInt(totalMass * 100f);

            if (manager != null)
                manager.AddScore(gainedScore);

            Destroy(gameObject);
            Destroy(other.gameObject);
        }
    }
}
