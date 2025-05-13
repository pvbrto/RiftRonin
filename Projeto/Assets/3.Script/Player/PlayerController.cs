using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Status")]
    public bool isDead = false;
    public bool invincible = false;

    [Header("Move")]
    [SerializeField] private float playerSpeed = 10.0f;
    public bool canMove = true;
    private bool isRunLeft = false;
    private bool isRunRight = false;
    private float x;
    private Vector3 feetPosition;

    [Header("Slope")]
    [SerializeField] private LayerMask slopeMask;
    private RaycastHit2D slopeHit;
    private bool isSlope = false;
    private Vector2 slopeNormalPerp;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 10.0f;
    [SerializeField] private LayerMask groundMask;
    public bool isGround = true;

    [Header("Crouch")]
    private float crouchSpeed = 15.0f;
    private bool isCrouch = false;
    private float crouchBuffer = 0f;
    private float crouchCoolTime = 1.0f;
    public bool isRoll = false;
    public bool fallThrough = false;

    [Header("Attack")]
    [SerializeField] private float attackForce = 20.0f;
    [SerializeField] private Vector2 boxSize; // Attack hitbox size
    private Vector3 mousePoint;
    private Vector3 targetPosition;
    private int attackCount = 0;
    private float attackCoolTime = 0.3f;
    private bool isAttack = false;
    private float angleZ = 0;

    [Header("WallSlide")]
    public bool grabWall = false;
    public bool slideWall = false;
    private float wallInputTimer = 0;

    [Header("ParticleSystem")]
    [SerializeField] private GameObject ps;
    [SerializeField] private GameObject laserHitps;

    [Header("Prefabs")]
    [SerializeField] private GameObject slashPrefab;
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private GameObject laserHitPrefab;
    private GameObject laserHit;

    [Header("UI")]
    [SerializeField] private Sprite grabWallImage;
    [SerializeField] private Sprite slideWallImage;
    [SerializeField] private GameObject shiftButtonUI;
    [SerializeField] private GameObject songTitle;
    [SerializeField] private GameObject mapTitle;

    [Header("Neve")]
    [SerializeField] private float snowSlowDownFactor = 0.99f;  // Fator de desaceleração gradual na neve
    [SerializeField] private float minSpeedInSnow = 3.0f;  // Velocidade mínima na neve
    private bool nearfogueira = false;  // Verifica se o jogador está perto de uma fogueira
    [SerializeField] private float fogueiraRestoreSpeed = 10.0f;  // Velocidade restaurada perto da fogueira

    // Components
    private Animator animator;
    private Rigidbody2D rb;
    private bool isSliding = false;
    private bool nevando = false;

    // CheckTimeForAnimation
    private float timeBetweenInput = 0;

    // Novas variáveis para controle do deslize
    private float slideForceDuration = 0.5f;  // Duração do deslizamento
    private float slideForceTimer = 5f;  // Timer para aplicar a força contínua


    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        SetSpeedBasedOnBackground();
    }

    private void Update()
    {
        GetMouseInput();
        UpdateStatus();
        SetSpeedBasedOnBackground();

        if (nearfogueira && nevando) 
        {
            playerSpeed = fogueiraRestoreSpeed;
        }
        else 
        {
            // Aplica a desaceleração gradual na neve
            if (nevando)  // Checa se o jogador está na neve
            {
                nevando = false;
                StartCoroutine(LentidaoNeve());
            }
            else
            {
                // Se o jogador não está na neve, a velocidade volta ao valor base
                SetSpeedBasedOnBackground();  // Recalcula a velocidade base com base no fundo
            }
        }

        // Lógica de deslize
        if (Input.GetKeyDown(KeyCode.D))  // Quando a tecla "D" for pressionada
        {
            isSliding = true;  // Inicia o deslize
            slideForceTimer = slideForceDuration;  // Reseta o timer do deslize
            x = 1f;  // Direção positiva (direita)
        }
        if (Input.GetKeyDown(KeyCode.A))  // Quando a tecla "A" for pressionada
        {
            isSliding = true;  // Inicia o deslize
            slideForceTimer = slideForceDuration;  // Reseta o timer do deslize
            x = -1f;  // Direção negativa (esquerda)
        }

        if (slideForceTimer > 0)
        {
            slideForceTimer -= Time.deltaTime;  // Decrementa o timer de deslize
        }
        else
        {
            isSliding = false;  // Quando o tempo de deslize acabou, para o deslize
        }

        if (grabWall || slideWall)
        {
            GetInputOnWall();
            rb.linearVelocity += Vector2.up * 0.1f; // Se estiver em paredão
        }
        else
        {
            GetInputToMove();  // Se não estiver em paredão, pode andar normalmente
        }

        ClampMaxVelocity();
    }

    private IEnumerator LentidaoNeve()
    {
        yield return new WaitForSeconds(1.5f);
        playerSpeed = Mathf.Max(playerSpeed * snowSlowDownFactor, minSpeedInSnow);
        nevando = true;
    }

    private void SetSpeedBasedOnBackground()
    {
        GameObject backgroundContainer = GameObject.Find("BackgroundContainer");

        if (backgroundContainer != null)
        {
            Transform activeBackground = null;

            foreach (Transform child in backgroundContainer.transform)
            {
                if (child.gameObject.activeSelf)
                {
                    activeBackground = child;
                    break;
                }
            }

            if (activeBackground != null)
            {
                string bgName = activeBackground.name;
                string bgInitial = bgName.Substring(0, 1);

                switch (bgInitial)
                {
                    case "I":
                        nevando = true;
                        break;
                    case "T":
                        playerSpeed = 15.0f;
                        if (isSliding)
                        {
                            if (slideForceTimer > 0f)
                            {
                                rb.AddForce(new Vector2(x * 0.5f, 0), ForceMode2D.Force);
                            }
                        }
                        break;
                    default:
                        playerSpeed = 10.0f;
                        break;
                }

                // Debug.Log("Background ativo: " + bgName + " | Velocidade do player: " + playerSpeed);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Fogueira"))
        {
            nearfogueira = true;  // O jogador entrou na área da fogueira
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Fogueira"))
        {
            nearfogueira = false;  // O jogador saiu da área da fogueira
        }
    }

    public void GrabWall(int direction) // 
    {
        // direction 0:  , 1: 
        if (direction == 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (direction == 1)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }

        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation; // ¿ ̰ 
        animator.enabled = false;
        grabWall = true;
        GetComponent<SpriteRenderer>().sprite = grabWallImage;
    }

    public void ExitWall() // 
    {
        GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        animator.enabled = true;
        grabWall = false;
        slideWall = false;
    }

    public void ResetStatus()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        grabWall = false;
        slideWall = false;
        isDead = false;
        invincible = false;
        canMove = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        ps.SetActive(true);
        animator.enabled = true;
        animator.speed = 1.0f;
    }

    public void Dead()
    {
        //GameManager.instance.BGM_Stop();
        CameraControl.instance.ShakeCamera(0.25f);
        StartCoroutine(Dead_co());
    }

    private IEnumerator Dead_co()
    {
        isDead = true;
        invincible = true;
        canMove = false;
        ps.SetActive(false);
        animator.SetTrigger("Dead");

        yield return new WaitForSeconds(1.0f);

       // GameManager.instance.GameOver(0);
    }

    public void LaserDead()
    {
        CameraControl.instance.ShakeCamera(0.25f);
        StartCoroutine(LaserDead_co());
    }

    private IEnumerator LaserDead_co()
    {
        isDead = true;
        invincible = true;
        canMove = false;
        ps.SetActive(false);
        animator.enabled = false;

        // laser dead animation
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

        GetComponent<SpriteRenderer>().enabled = false;

        Vector3 spawnPosition = new Vector3(transform.position.x + 0.5f, transform.position.y - 0.25f, transform.position.z);

        laserHit = Instantiate(laserHitPrefab, spawnPosition, Quaternion.identity);
        laserHit.GetComponent<LaserHitControl>().Fade();

        Destroy(laserHit, 2.5f);

        laserHitps.GetComponent<ParticleSystem>().Play();
        
        yield return new WaitForSeconds(3.0f);

        //GameManager.instance.GameOver(0);
    }

    public void Outro()
    {
        ps.GetComponent<ParticleSystem>().Clear();
        ps.GetComponent<ParticleSystem>().Stop();
        GetComponent<Animator>().SetTrigger("Dance");
    }

    private void Intro()
    {
        StartCoroutine(Intro_co());
    }

    private IEnumerator Intro_co()
    {
        // 0. Ʈ 
        yield return new WaitForSeconds(3.0f);

        // 1. 
        animator.SetBool("isRun", true);

        while (transform.position.x < -15)
        {
            transform.position += Vector3.right * 0.05f;
            yield return null;
        }
        transform.position = new Vector3(-15, -2.822f, 0);

        // 2. 
        animator.SetBool("isRun", false);
        animator.SetTrigger("PlaySong");
        yield return new WaitForSeconds(2.0f);
        GameManager.instance.BGM_Play();

        // 3. UI 
        songTitle.GetComponent<SongTitle>().StartAnimation();
        mapTitle.GetComponent<MapTitle>().StartAnimation();
        yield return new WaitForSeconds(3.0f);


        // 4. 콺  Է¹޾  Է 
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameManager.instance.gameStart = true;
                songTitle.GetComponent<SongTitle>().StopAnimation();
                mapTitle.GetComponent<MapTitle>().StopAnimation();
                GameManager.instance.StartTimer();
                break;
            }

            yield return null;
        }

        yield break;
    }

    private void ClampMaxVelocity()
    {
        if (rb.linearVelocity.x > 10.0f)
        {
            rb.linearVelocity = new Vector2(10.0f, rb.linearVelocity.y);
        }
        else if (rb.linearVelocity.x < -10.0f)
        {
            rb.linearVelocity = new Vector2(-10.0f, rb.linearVelocity.y);
        }

        if (rb.linearVelocity.y > 20.0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 20.0f);
        }
        else if (rb.linearVelocity.y < -20.0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -20.0f);
        }
    }

    private void GetInputOnWall()
    {
        if (!canMove)
        {
            return;
        }

        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))) // Է
        {
            StartCoroutine(InvincibleWhileFlip_co());

            Vector2 jumpDirection;

            EffectManager.instance.JumpDust(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z));

            if (GetComponent<SpriteRenderer>().flipX == false) //   ϶ 
            {
                jumpDirection = Vector2.up + Vector2.left;
            }
            else //   ϶ 
            {
                jumpDirection = Vector2.up + Vector2.right;
            }

            ExitWall();
            rb.AddForce(jumpDirection * 17f, ForceMode2D.Impulse);
            animator.SetTrigger("Flip");
        }

        if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))) // 
        {
            if (GetComponent<SpriteRenderer>().flipX == false) //   ϶ ¸
            {
                if (wallInputTimer < 0.25f)
                {
                    wallInputTimer += Time.deltaTime;
                }
                else
                {
                    wallInputTimer = 0;
                    ExitWall();
                }
            }
        }

        if ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))) // 
        {
            if (GetComponent<SpriteRenderer>().flipX == true) //   ϶ ¸
            {
                if (wallInputTimer < 0.25f)
                {
                    wallInputTimer += Time.deltaTime;
                }
                else
                {
                    wallInputTimer = 0;
                    ExitWall();
                }
            }
        }

        if ((Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))) // Ű 
        {
            wallInputTimer = 0; // Ÿ ʱȭ
        }

        if ((Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))) // Ű 
        {
            wallInputTimer = 0; // Ÿ ʱȭ
        }
    }

    private IEnumerator InvincibleWhileFlip_co()
    {
        invincible = true;
        canMove = false;

        yield return new WaitForSeconds(0.25f);
        canMove = true;

        yield return new WaitForSeconds(0.75f);
        invincible = false;
    }

    private void GetInputToMove()
    {
        if (!canMove)
        {
            return;
        }

        // [ Է ]
        x = Input.GetAxisRaw("Horizontal"); //¿ ̵ Է

        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && !isRoll) // Է
        {
            Vector2 jumpDirection;

            if (isGround) // 
            {
                jumpDirection = Vector2.up;

                EffectManager.instance.JumpDust(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z));
                AudioManager.Instance.PlayJumpSound();

                rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
                animator.SetTrigger("Jump");
            }
        }

        if ((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) && isGround) // ̱ Է
        {
            fallThrough = true;

            crouchBuffer = 0;

            animator.ResetTrigger("UnCrouch");
            animator.ResetTrigger("Roll");

            if (!isCrouch)
            {
                animator.SetTrigger("Crouch");
                isCrouch = true;
            }

            // Ʒ  Ʒ  ÿ
            if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) && crouchCoolTime >= 1.0f) // 
            {
                fallThrough = false;
                invincible = true;
                isRoll = true;
                crouchCoolTime = 0;
                transform.GetComponent<SpriteRenderer>().flipX = true;
                StartCoroutine(Roll_co(Vector2.left));
            }

            if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) && crouchCoolTime >= 1.0f) // 
            {
                fallThrough = false;
                invincible = true;
                isRoll = true;
                crouchCoolTime = 0;
                transform.GetComponent<SpriteRenderer>().flipX = false;
                StartCoroutine(Roll_co(Vector2.right));
            }
        }

        // Ʒ  ϶  Ʒ
        if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) && crouchCoolTime >= 1.0f && crouchBuffer < 0.25f && isGround) // 
        {
            fallThrough = false;
            invincible = true;
            isRoll = true;
            crouchCoolTime = 0;
            transform.GetComponent<SpriteRenderer>().flipX = true;
            StartCoroutine(Roll_co(Vector2.left));
        }

        if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) && crouchCoolTime >= 1.0f && crouchBuffer < 0.25f && isGround) // 
        {
            fallThrough = false;
            invincible = true;
            isRoll = true;
            crouchCoolTime = 0;
            transform.GetComponent<SpriteRenderer>().flipX = false;
            StartCoroutine(Roll_co(Vector2.right));
        }

        if (isCrouch && (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))) // ̱ 
        {
            fallThrough = false;
            isCrouch = false;
            animator.SetTrigger("UnCrouch");
        }

        // [ ̵ ]
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Precrouch")) //  ä ̵ 
        {
            return;
        }

        if (x != 0)
        {
            if (x > 0)
            {
                transform.GetComponent<SpriteRenderer>().flipX = false;

                isRunLeft = false;

                if (!isRunRight && !isRoll)
                {
                    isRunRight = true;
                    RunDust(1);
                }
            }
            else if (x < 0)
            {
                transform.GetComponent<SpriteRenderer>().flipX = true;

                isRunRight = false;

                if (!isRunLeft && !isRoll)
                {
                    isRunLeft = true;
                    RunDust(0);
                }
            }

            timeBetweenInput = 0;
            animator.SetBool("isRun", true);

            // [ ̵ ]
            if (isGround && !isSlope) // ̵
            {
                transform.position += playerSpeed * Time.deltaTime * new Vector3(x, 0, 0); 
            }
            else if (isGround && isSlope) // ο ̵
            {
                transform.position += playerSpeed * Time.deltaTime * new Vector3(slopeNormalPerp.x * -x, slopeNormalPerp.y * -x, 0);
            }
            else // ߿ ̵
            {
                transform.position += 0.5f * playerSpeed * Time.deltaTime * new Vector3(x, 0, 0); 
            }
        }
        else if (timeBetweenInput > 0.125f)
        {
            isRunLeft = false;
            isRunRight = false;
            animator.SetBool("isRun", false);
        }
        else
        {
            timeBetweenInput += Time.deltaTime;
        }
    }

    // private void GetShiftInput()
    // {
    //     if (Input.GetKeyDown(KeyCode.LeftShift)) // ο
    //     {
    //         shiftButtonUI.GetComponent<ShiftButtonImageOnOff>().SetImageOff();
    //         GameManager.instance.StartSlowMotion();
    //     }

    //     if (Input.GetKeyUp(KeyCode.LeftShift)) // ο 
    //     {
    //         shiftButtonUI.GetComponent<ShiftButtonImageOnOff>().SetImageOn();
    //         GameManager.instance.StopSlowMotion();
    //     }
    // }

    private void GetMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (attackCoolTime < 0.3f) //  Ÿ   Ұ
            {
                return;
            }

            attackCoolTime = 0;
            AudioManager.Instance.PlayAttackSound();

            targetPosition = mousePoint - transform.position;
            targetPosition.Normalize();

            angleZ = Mathf.Atan2(targetPosition.y, targetPosition.x) * 180 / Mathf.PI; // ũ źƮ  ͸ ̿  (rotation z) 

            if (mousePoint.x >= transform.position.x)
            {
                //Debug.Log("  ϶");
                transform.GetComponent<SpriteRenderer>().flipX = false;
            }
            else
            {
                //Debug.Log("  ");
                transform.GetComponent<SpriteRenderer>().flipX = true;
            }

            animator.SetTrigger("Attack");
            AttackHitbox(targetPosition);
            
            GameObject slashEffect = Instantiate(slashPrefab, transform.position, Quaternion.Euler(new Vector3(0, 0, angleZ)));
            slashEffect.transform.SetParent(gameObject.transform);
            Destroy(slashEffect, 0.3f);

            // Physics
            StartCoroutine(AttackMove_co(targetPosition));
        }
    }

    private IEnumerator AttackMove_co(Vector3 targetPosition)
    {
        if (grabWall)
        {
            ExitWall();
        }

        isAttack = true;
        canMove = false;
        invincible = true;
        slideWall = false;

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // 
        if (attackCount == 0)
        {
            attackCount += 1;
            rb.AddForce(targetPosition * attackForce, ForceMode2D.Impulse); // ù  ϰ
        }
        else
        {
            rb.AddForce(targetPosition * (attackForce * 0.33f), ForceMode2D.Impulse); //  ϰ
        }

        yield return new WaitForSeconds(0.33f); // 

        canMove = true;
        isAttack = false;
        invincible = false;
    }

    private void AttackHitbox(Vector3 targetPosition)
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position + targetPosition * 2.0f, boxSize, 0);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                if (collider.GetComponent<GruntController>().currentState == State.Dead)
                {
                    return;
                }

                CameraControl.instance.ShakeCamera(0.15f);

                GameObject line = Instantiate(linePrefab, transform.position, Quaternion.Euler(new Vector3(0, 0, angleZ)));
                Destroy(line, 0.1f);

                collider.GetComponent<GruntController>().Dead(transform.position);
            }
           /* else if (collider.CompareTag("Gangster"))
            {
                if (collider.GetComponent<GangsterController>().currentState == State.Dead)
                {
                    return;
                }

                CameraControl.instance.ShakeCamera(0.15f);

                GameObject line = Instantiate(linePrefab, transform.position, Quaternion.Euler(new Vector3(0, 0, angleZ)));
                Destroy(line, 0.1f);

                collider.GetComponent<GangsterController>().Dead(transform.position);
            }
            else if (collider.CompareTag("Door"))
            {
                collider.GetComponent<DoorControl>().OpenDoor();
            }*/
        }
    }

    /*private void OnDrawGizmos() // Attack Hitbox 
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + targetPosition * 2.0f, boxSize);
    }*/

    private void RunDust(int direction)
    {
        if (!isGround)
        {
            return;
        }

        EffectManager.instance.RunDust(direction, feetPosition);
    }

    private void UpdateStatus() // [  Ʈ ]
    {
        feetPosition = new Vector3(transform.position.x, transform.position.y - 1.0f, transform.position.z); // ÷̾  Ʈ

        mousePoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z)); // 콺 Ʈ

        // 
        if (!isGround && GroundCheck() && rb.linearVelocity.y <= 0)
        {
            if (grabWall)
            {
                ExitWall();
            }

            slideWall = false;
            animator.SetTrigger("Idle");

            // 
            EffectManager.instance.LandDust(feetPosition);
        }

        isGround = GroundCheck();
        isSlope = SlopeCheck();

        if (isGround)
        { 
            attackCount = 0;
        }

        if (isSlope || (!isAttack && isGround && x == 0))
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        if (rb.linearVelocity == Vector2.zero && x == 0)
        {
            ps.GetComponent<RunParticle>().Particle_Off();
        }
        else
        {
            if (x < 0 || rb.linearVelocity.x < 0)
            {
                if (isRoll)
                {
                    ps.GetComponent<RunParticle>().Particle_On(0, true);
                }
                else
                {
                    ps.GetComponent<RunParticle>().Particle_On(0, false);
                }
            }
            else if (x > 0 || rb.linearVelocity.x > 0)
            {
                if (isRoll)
                {
                    ps.GetComponent<RunParticle>().Particle_On(1, true);
                }
                else
                {
                    ps.GetComponent<RunParticle>().Particle_On(1, false);
                }
            }
            
        }

        animator.SetBool("isFall", FallCheck());
        crouchCoolTime += Time.deltaTime;
        crouchBuffer += Time.deltaTime;
        attackCoolTime += Time.deltaTime;
    }

    private IEnumerator Roll_co(Vector2 direction)
    {
        canMove = false;

        animator.SetTrigger("Roll");

        float timer = 0;

        float[] timer_check = new float[4] { 0.05f, 0.1f, 0.15f, 0.2f };
        int index = 0;

        while (timer < 0.25f)
        {
            if (index < 4 && timer > timer_check[index])
            {
                index += 1;
                EffectManager.instance.RollDust(feetPosition);
            }

            timer += Time.deltaTime;
            transform.Translate(crouchSpeed * Time.deltaTime * direction);
            yield return null;
        }

        canMove = true;
        isRoll = false;
        invincible = false;
    }

    private bool GroundCheck()
    {
        bool isGround = Physics2D.OverlapCircle(feetPosition, 0.5f, groundMask);

        return isGround;
    }

    private bool FallCheck()
    {
        bool isFall;

        if (rb.linearVelocity.y < 0 && !GroundCheck())
        {
            isFall = true;
        }
        else
        {
            isFall = false;
        }

        return isFall;
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
        else if(temp)
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
