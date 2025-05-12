using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Estado")]
    public State currentState;
    private bool canMove = true;

    // Componentes
    private Animator animator;
    private SpriteRenderer sr;
    private Rigidbody2D rb;

    [Header("Perseguição")]
    [SerializeField] private float moveSpeed = 7.0f;
    [SerializeField] private float detectionRadius = 15.0f;
    private GameObject player;
    
    [Header("Ataque")]
    [SerializeField] private float attackRange = 2.0f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private Vector2 attackHitboxSize = new Vector2(2.5f, 2.0f);
    private bool isAttacking = false;
    private float attackTimer = 0f;

    [Header("Efeitos")]
    [SerializeField] private GameObject attackEffect;
    [SerializeField] private GameObject alert;

    [Header("Áudio")]
    [SerializeField] private AudioClip bossDetectionSound;
    [SerializeField] private AudioClip bossAttackSound;

    [Header("Slope")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask slopeMask;
    private RaycastHit2D slopeHit;
    private bool isSlope = false;
    private Vector2 slopeNormalPerp;

    private void Awake()
    {
        currentState = State.Idle;
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Procura o jogador na cena
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player não encontrado na cena!");
        }
    }

    private void Update()
    {
        // Verificar se está em uma rampa
        isSlope = SlopeCheck();
        
        // Atualizar o timer de ataque
        if (attackTimer < attackCooldown)
        {
            attackTimer += Time.deltaTime;
        }

        // Lógica baseada no estado atual
        switch (currentState)
        {
            case State.Idle:
                CheckForPlayer();
                break;
                
            case State.Track:
                TrackPlayer();
                CheckAttackRange();
                break;
                
            case State.Attack:
                if (!isAttacking && attackTimer >= attackCooldown)
                {
                    StartCoroutine(Attack_co());
                }
                break;
                
            case State.Dead:
                // O Boss não morre, então este estado não é usado
                break;
        }
    }

    private void CheckForPlayer()
    {
        if (player == null) return;

        // Verificar se o jogador está dentro do raio de detecção
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        
        if (distanceToPlayer <= detectionRadius)
        {
            // Alertar que encontrou o jogador
            if (alert != null)
            {
                alert.SetActive(true);
            }
            
            // Tocar som de detecção
            if (bossDetectionSound != null)
            {
                AudioSource.PlayClipAtPoint(bossDetectionSound, transform.position);
            }
            
            // Mudar para estado de perseguição
            currentState = State.Track;
            
            // Trigger de animação
            if (animator != null)
            {
                animator.SetTrigger("Track");
            }
        }
    }

    private void TrackPlayer()
    {
        if (player == null || !canMove) return;

        // Direção para o jogador
        Vector2 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        
        // Normalizar a direção
        directionToPlayer.Normalize();
        
        // Definir a orientação do sprite com base na direção
        if (directionToPlayer.x < 0)
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }
        
        // Mover em direção ao jogador
        if (isSlope)
        {
            // Movimento em rampa
            transform.position += moveSpeed * Time.deltaTime * new Vector3(
                slopeNormalPerp.x * -directionToPlayer.x, 
                slopeNormalPerp.y * -directionToPlayer.y, 
                0);
        }
        else
        {
            // Movimento normal
            transform.position += moveSpeed * Time.deltaTime * new Vector3(
                directionToPlayer.x, 
                directionToPlayer.y, 
                0);
        }
        
        // Atualizar animação
        if (animator != null)
        {
            animator.SetBool("isWalk", true);
        }
    }

    private void CheckAttackRange()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        
        if (distanceToPlayer <= attackRange)
        {
            currentState = State.Attack;
        }
    }

    private IEnumerator Attack_co()
    {
        isAttacking = true;
        canMove = false;
        
        // Trigger de animação
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        
        // Tocar som de ataque
        if (bossAttackSound != null)
        {
            AudioSource.PlayClipAtPoint(bossAttackSound, transform.position);
        }
        
        // Esperar pela animação de preparação do ataque
        yield return new WaitForSeconds(0.5f);
        
        // Executar o ataque
        AttackHitbox();
        
        // Instantiate attack effect if available
        if (attackEffect != null)
        {
            Instantiate(attackEffect, transform.position, Quaternion.identity);
        }
        
        // Esperar pela conclusão da animação
        yield return new WaitForSeconds(0.5f);
        
        // Resetar timers e flags
        attackTimer = 0f;
        isAttacking = false;
        canMove = true;
        
        // Voltar a perseguir
        currentState = State.Track;
    }

    private void AttackHitbox()
    {
        // Calcular posição do hitbox baseado na direção do jogador
        Vector2 attackPosition = (Vector2)transform.position;
        if (sr.flipX)
        {
            attackPosition.x -= attackHitboxSize.x / 2;
        }
        else
        {
            attackPosition.x += attackHitboxSize.x / 2;
        }
        
        // Detectar colisões
        Collider2D[] colliders = Physics2D.OverlapBoxAll(attackPosition, attackHitboxSize, 0);
        
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                PlayerController playerController = collider.GetComponent<PlayerController>();
                
                if (playerController != null && !playerController.invincible && !playerController.isDead)
                {
                    // Aplicar força de knockback
                    Vector2 knockbackDirection = new Vector2(
                        player.transform.position.x < transform.position.x ? -1 : 1, 
                        0.5f
                    ).normalized;
                    
                    // Mata o jogador e aplica força
                    playerController.Dead();
                    collider.GetComponent<Rigidbody2D>().AddForce(knockbackDirection * 15.0f, ForceMode2D.Impulse);
                    
                    // Shake na câmera
                    CameraControl.instance.ShakeCamera(0.3f);
                }
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        // Desenhar o raio de detecção
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        
        // Desenhar o raio de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Desenhar o hitbox de ataque
        Gizmos.color = Color.magenta;
        Vector2 attackPosition = (Vector2)transform.position;
        if (sr != null && sr.flipX)
        {
            attackPosition.x -= attackHitboxSize.x / 2;
        }
        else
        {
            attackPosition.x += attackHitboxSize.x / 2;
        }
        Gizmos.DrawWireCube(attackPosition, attackHitboxSize);
    }
    
    private bool SlopeCheck()
    {
        slopeHit = Physics2D.Raycast(transform.position, -Vector2.up, 2.0f, slopeMask);

        float slopeAngle = 0;

        RaycastHit2D temp = Physics2D.Raycast(transform.position, -Vector2.up, 2.0f, groundMask);

        if (slopeHit)
        {
            slopeNormalPerp = Vector2.Perpendicular(slopeHit.normal);
            slopeAngle = Vector2.Angle(slopeHit.normal, Vector2.up);

            Debug.DrawRay(slopeHit.point, slopeNormalPerp, Color.red);
            Debug.DrawRay(slopeHit.point, slopeHit.normal, Color.green);
        }
        else if (temp)
        {
            return false;
        }

        if (slopeAngle != 0 && (transform.position.y - slopeHit.point.y) < 1.5f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}