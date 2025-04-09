## Prefabs

Os prefabs representam os elementos reutilizáveis e funcionais do jogo *Rift Ronin*. Cada prefab possui um conjunto específico de componentes que controlam seu visual, colisão, som e comportamento através de scripts. Abaixo estão descritos os principais prefabs utilizados no jogo.

<br/>

---

### **Prefab: Jogador (Musashi)**

- **Descrição:**  
  Representa o personagem controlado pelo jogador — Musashi, o ronin amaldiçoado.

- **Quando é utilizado:**  
  Presente do início ao fim do jogo, em todos os cenários jogáveis.

- **Componentes:**  
  - **Sprite Renderer:**  
    Exibe as animações do personagem (idle, corrida, ataque, pulo, dash, parry, morte).  
  - **Animator:**  
    Controla a transição de estados com base nos inputs e eventos climáticos.
  - **BoxCollider2D / CapsuleCollider2D:**  
    Detecta colisões com inimigos, ambiente e interações.
  - **Rigidbody2D:**  
    Permite movimentação física e resposta a forças externas.
  - **Audio Source:**  
    Emite sons de ataque, passos, dano, grito de morte e interações.
  - **Scripts:**  
    - `PlayerController.cs`: movimentação, ataque e defesa.  
    - `HealthSystem.cs`: controle de vida, dano e morte.  
    - `ClimateTrigger.cs`: ativa mudança de clima ao derrotar inimigos.  
    - `InteractionHandler.cs`: lida com objetos interativos como fogueiras e NPCs.

<br/>

---

### **Prefab: Fogueira**

- **Descrição:**  
  Objeto de descanso e restauração. Tira o jogador de estado congelando

- **Quando é utilizado:**  
  Posicionado em pontos-chave quando está em clima de neve do mapa entre combates intensos.

- **Componentes:**  
  - **Sprite Renderer:**  
    Animação de fogo em pixel art.
  - **Particle System:**  
    Emite partículas simulando faíscas e brasas.
  - **CircleCollider2D (Trigger):**  
    Detecta quando o jogador entra na área de alcance.
  - **Audio Source:**  
    Som ambiente de fogo crepitando.
  - **Scripts:**  
    - `CampfireInteraction.cs`: ativa o descanso quando o jogador interage.  
    - `HealingEffect.cs`: restaura vida gradualmente durante o uso.  
    - `ClimateRelief.cs`: reduz os efeitos negativos de climas extremos.

<br/>

---

### **Prefab: Inimigo Comum**

- **Descrição:**  
  Soldado ou samurai da Irmandade da Tempestade. Possui padrões de ataque simples.

- **Quando é utilizado:**  
  Em confrontos frequentes ao longo do jogo. Aparece em grupos ou patrulhando áreas.

- **Componentes:**  
  - **Sprite Renderer:**  
    Animações de patrulha, ataque, dano e morte.
  - **Animator:**  
    Controla transições entre estados (idle, atacar, morrer).
  - **BoxCollider2D / CapsuleCollider2D:**  
    Detecta colisões com o jogador.
  - **Rigidbody2D:**  
    Movimentação física.
  - **Audio Source:**  
    Sons de passos, ataques, gritos de dor e morte.
  - **Scripts:**  
    - `EnemyAI.cs`: movimentação, visão do jogador, patrulha.  
    - `CombatBehavior.cs`: lógica de ataque e cooldown.  
    - `HealthSystem.cs`: dano e morte.  
    - `ClimateImpact.cs`: chama o sistema climático após a morte.

<br/>

---

### **Prefab: Boss (Chefe Final)**

- **Descrição:**  
  Representa o líder da Irmandade da Tempestade, o mestre final de Musashi. Possui ataques complexos e múltiplas fases.

- **Quando é utilizado:**  
  Na batalha final do jogo, em um cenário dedicado com mudanças climáticas constantes.

- **Componentes:**  
  - **Sprite Renderer:**  
    Visual detalhado com variações por fase (ex: forma humana, forma espiritual).
  - **Animator:**  
    Animações exclusivas para cada fase de batalha.
  - **PolygonCollider2D:**  
    Colisão adaptada ao formato irregular do chefe.
  - **Rigidbody2D:**  
    Permite movimentação controlada.
  - **Audio Source:**  
    Voz, gritos, efeitos de ataque mágicos e ambiente sonoro único.
  - **Scripts:**  
    - `BossAI.cs`: controle de fases, padrões de ataque, IA estratégica.  
    - `CinematicTrigger.cs`: inicia a batalha com cutscene.  
    - `ClimateChaos.cs`: altera o clima conforme a fase da luta.  
    - `DialogueHandler.cs`: exibe frases do chefe durante a luta.  
    - `DeathSequence.cs`: finaliza o jogo após a derrota ou vitória.

<br/>

Esses prefabs compõem os principais pilares do gameplay de *Rift Ronin*, equilibrando combate, narrativa, ambientação e progressão climática de forma dinâmica e integrada.
