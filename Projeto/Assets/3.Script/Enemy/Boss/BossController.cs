using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossState
{
    Idle,
    Track,
    Attack
}

public class BossController : MonoBehaviour
{
    [Header("Estado")]
    public BossState currentState;
    private bool canMove = true;

    // Componentes
    private Animator animator;
    private SpriteRenderer sr;
    private Rigidbody2D rb;

    [Header("Perseguição")]
    [SerializeField] private float moveSpeed = 5.0f;
    private GameObject player;
    
    [Header("Ataque")]
    [SerializeField] private float attackRange = 2.0f;
    [SerializeField] private float attackCooldown = 2.0f;
    [SerializeField] private Vector2 boxSize = new Vector2(3.0f, 2.0f);
    private bool isAttacking = false;
    private float attackTimer = 0f;

    [Header("Efeitos")]
    [SerializeField] private GameObject attackEffect;
    [SerializeField] private GameObject alert;

    [Header("LayerMask")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask slopeMask;

    // Slope
    private RaycastHit2D slopeHit;
    private bool isSlope = false;
    private Vector2 slopeNormalPerp;

    private void Awake()
    {
        currentState = BossState.Idle;
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        
        Debug.Log("Boss inicializado no estado: " + currentState);
    }

    private void Start()
    {
        // Encontrar o jogador
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player não encontrado na cena!");
        }
        else
        {
            Debug.Log("Player encontrado com sucesso");
        }
        
        // Garantir que o alerta esteja desativado no início
        if (alert != null)
        {
            alert.SetActive(false);
        }

        // ActivateBoss();
    }

    private void Update()
    {
        // Verificar se está em uma rampa
        isSlope = SlopeCheck();
        
        // Gerenciamento do timer de ataque
        if (attackTimer < attackCooldown)
        {
            attackTimer += Time.deltaTime;
        }

        // Lógica baseada no estado atual
        switch (currentState)
        {
            case BossState.Idle:
                // Espera pelo BossTrigger ativar
                break;
                
            case BossState.Track:
                if (player != null && canMove)
                {
                    TrackPlayer();
                    CheckAttackRange();
                }
                break;
                
            case BossState.Attack:
                if (!isAttacking && attackTimer >= attackCooldown)
                {
                    StartCoroutine(Attack_co());
                }
                break;
        }
    }

    // Ativado pelo BossTrigger
    public void ActivateBoss()
    {
        
        if (currentState != BossState.Idle) 
        {
            return;
        }
        
        currentState = BossState.Track;
        
        if (alert != null)
        {
            alert.SetActive(true);
            Debug.Log("Alerta ativado");
        }
        
        if (animator != null)
        {
            animator.SetTrigger("Track");
        }
        else
        {
            Debug.LogWarning("Animator não encontrado no Boss");
        }
        
        // Shake na câmera para efeito dramático
        if (CameraControl.instance != null)
        {
            CameraControl.instance.ShakeCamera(0.5f);
            Debug.Log("Camera shake ativado");
        }
    }

    private void TrackPlayer()
    {
        if (player == null) return;
        
        // Determinar direção para o jogador
        float directionX = player.transform.position.x < transform.position.x ? -1 : 1;
        
        // Orientação do sprite
        if (directionX < 0)
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }
        
        // Animação de movimentação
        if (animator != null)
        {
            animator.SetBool("isRunning", true);
        }
        
        // Movimento baseado em rampa ou normal
        if (isSlope)
        {
            // Movimento em rampa
            transform.position += moveSpeed * Time.deltaTime * new Vector3(
                slopeNormalPerp.x * -directionX, 
                slopeNormalPerp.y * -directionX, 
                0);
        }
        else
        {
            // Movimento normal
            transform.position += moveSpeed * Time.deltaTime * new Vector3(directionX, 0, 0);
        }
    }

    private void CheckAttackRange()
    {
        if (player == null) return;
        
        float distToPlayer = Vector2.Distance(transform.position, player.transform.position);
        
        if (distToPlayer <= attackRange && currentState != BossState.Attack)
        {
            currentState = BossState.Attack;
            Debug.Log("Player no alcance de ataque! Mudando para estado de Ataque");
        }
    }

    private IEnumerator Attack_co()
    {
        isAttacking = true;
        canMove = false;
        
        if (animator != null)
        {
            animator.SetBool("isRunning", false);
            animator.SetTrigger("Attack");
        }
        
        yield return new WaitForSeconds(0.5f);
        
        AttackHitbox();
        
        yield return new WaitForSeconds(0.75f);
        
        isAttacking = false;
        canMove = true;
        attackTimer = 0f;
        
        currentState = BossState.Track;
        animator.SetTrigger("Track");
        animator.SetBool("isRunning", true);
    }

    private void AttackHitbox()
    {
        if (player == null) return;
        
        Vector2 attackPosition = transform.position;
        
        if (sr.flipX)
        {
            attackPosition.x -= boxSize.x / 2;
        }
        else
        {
            attackPosition.x += boxSize.x / 2;
        }
        
        Collider2D[] colliders = Physics2D.OverlapBoxAll(attackPosition, boxSize, 0);
        
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                PlayerController playerController = collider.GetComponent<PlayerController>();
                
                if (playerController != null && !playerController.invincible && !playerController.isDead)
                {
                    playerController.Dead();
                    
                    if (collider.transform.position.x <= transform.position.x)
                    {
                        collider.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 15.0f, ForceMode2D.Impulse);
                    }
                    else
                    {
                        collider.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 15.0f, ForceMode2D.Impulse);
                    }
                    
                    CameraControl.instance.ShakeCamera(0.25f);
                }
            }
        }
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
    
    // Método para verificar continuamente durante o Update se o player está dentro do alcance de ataque
    private void LateUpdate()
    {
        // Se o boss não estiver atacando, verificar constantemente se está no alcance para atacar
        if (currentState == BossState.Track && player != null)
        {
            float distToPlayer = Vector2.Distance(transform.position, player.transform.position);
            
            if (distToPlayer <= attackRange && attackTimer >= attackCooldown)
            {
                currentState = BossState.Attack;
            }
        }
        // Se estiver atacando mas o player saiu do alcance, voltar a perseguir
        else if (currentState == BossState.Attack && !isAttacking && player != null)
        {
            float distToPlayer = Vector2.Distance(transform.position, player.transform.position);
            
            if (distToPlayer > attackRange)
            {
                currentState = BossState.Track;
                if (animator != null)
                {
                    animator.SetBool("isRunning", true);
                }
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        // Desenhar o raio de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Desenhar o hitbox de ataque
        Gizmos.color = Color.magenta;
        Vector2 attackPosition = transform.position;
        
        if (sr != null && sr.flipX)
        {
            attackPosition.x -= boxSize.x / 2;
        }
        else
        {
            attackPosition.x += boxSize.x / 2;
        }
        
        Gizmos.DrawWireCube(attackPosition, boxSize);
    }
}