using UnityEngine;
using System.Collections; // Necessário para usar Coroutines

public class CharacterController : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [Tooltip("A velocidade com que o personagem se move.")]
    public float moveSpeed = 2f;
    [Tooltip("A coordenada X onde o personagem deve parar no balcão.")]
    public float stopPositionX = 0f;
    [Tooltip("O tempo de espera para o personagem reagir antes de sair.")]
    public float reactionDelay = 1.5f;

    private Animator animator;
     private SpriteRenderer spriteRenderer;

    private enum CharacterState
    {
        Entering, 
        Waiting,
        Exiting  
    }
    private CharacterState currentState;

    private readonly int isWalkingHash = Animator.StringToHash("isWalking");
    private readonly int isAngryHash = Animator.StringToHash("isWalkingAngry");
    private readonly int isHappyHash = Animator.StringToHash("isWalking");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        currentState = CharacterState.Entering;
        animator.SetBool(isWalkingHash, true);
    }

    private void Update()
    {
        switch (currentState)
        {
            case CharacterState.Entering:
                HandleEntering();
                break;
            case CharacterState.Waiting:
                HandleWaiting();
                break;
            case CharacterState.Exiting:
                HandleExiting();
                break;
        }
    }

    private void HandleEntering()
    {
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);

        // Verifica se chegou ao ponto de parada.
        if (transform.position.x <= stopPositionX)
        {
            transform.position = new Vector3(stopPositionX, transform.position.y, transform.position.z);
            animator.SetBool(isWalkingHash, false);

            // Muda para o estado de espera.
            currentState = CharacterState.Waiting;

            // TODO: Adicione aqui a lógica para o personagem entregar/apresentar o documento.
            // Exemplo: FindObjectOfType<GameManager>().ShowDocument(this);
        }
    }

    private void HandleWaiting()
    {
        // O personagem não faz nada neste estado, apenas espera a decisão do jogador.
        // A lógica de decisão será chamada externamente pelo GameManager.
    }

    private void HandleExiting()
    {
        transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
    }


    /// <param name="wasApproved">True se o documento foi aprovado, false caso contrário.</param>
    public void StartExitSequence(bool wasApproved)
    {
        if (currentState != CharacterState.Waiting) return; // Garante que a saída só comece se ele estiver esperando.

        StartCoroutine(ExitRoutine(wasApproved));
    }

    private IEnumerator ExitRoutine(bool wasApproved)
    {
        yield return new WaitForSeconds(reactionDelay);

        spriteRenderer.flipX = true;
        if (wasApproved)
        {
            animator.SetBool(isHappyHash, true);
        }
        else
        {
            animator.SetBool(isAngryHash, true);
        }
        
        currentState = CharacterState.Exiting;
    }
}