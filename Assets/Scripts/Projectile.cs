using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float bulletSpeed;
    PlayerMovement player;
    float xSpeed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerMovement>();
        xSpeed = player.transform.localScale.x * bulletSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(xSpeed, 0f);
    }

    void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if(otherCollider.tag == "Enemy")
        {
            FindObjectOfType<GameSession>().AddToScore(100);
            Destroy(otherCollider.gameObject);
            Destroy(this.gameObject);
        }
        if(otherCollider.tag == "Platform")
        {
            Destroy(this.gameObject);
        }
    }

}
