using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheck : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") && !GetComponent<PlayerController>().isGround && !GetComponent<PlayerController>().isDead)
        {
            rb.linearVelocity = Vector2.zero;

            if (collision.contacts[0].point.x > transform.position.x) // ���� ������
            {
                transform.position = new Vector3(collision.contacts[0].point.x - GetComponent<CapsuleCollider2D>().size.x,
                    transform.position.y, transform.position.z);

                GetComponent<PlayerController>().GrabWall(0);
            }
            else // ���� ����
            {
                transform.position = new Vector3(collision.contacts[0].point.x + GetComponent<CapsuleCollider2D>().size.x,
                    transform.position.y, transform.position.z);

                GetComponent<PlayerController>().GrabWall(1);
            }
        }
    }
}
