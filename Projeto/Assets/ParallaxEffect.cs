using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Transform player; // Referência ao player
    public float parallaxEffect = 0.5f;

    private Vector3 lastPlayerPosition;

    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player").transform; // Ou defina manualmente no Inspector

        lastPlayerPosition = player.position;
    }

    void LateUpdate()
    {
        Vector3 deltaMovement = player.position - lastPlayerPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxEffect, 0, 0);
        lastPlayerPosition = player.position;
    }
}
