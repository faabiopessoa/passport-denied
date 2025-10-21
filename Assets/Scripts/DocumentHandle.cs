using UnityEngine;
using UnityEngine.InputSystem; // Namespace importante para o novo Input System!

public class DocumentHandle : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 offset;
    private bool isDragging = false;

    // Variáveis para guardar nossas ações
    private PlayerControls controls;
    private InputAction clickAction;
    private InputAction mousePositionAction;

    void Awake()
    {
        mainCamera = Camera.main;
        controls = new PlayerControls();

        // Encontra as ações que criamos no nosso asset
        clickAction = controls.Gameplay.Click;
        mousePositionAction = controls.Gameplay.MousePosition;
    }

    // Boa prática: ativar e desativar as ações junto com o objeto
    private void OnEnable()
    {
        controls.Gameplay.Enable();

        // Inscreve nossas funções nos eventos de clique
        clickAction.performed += OnClickPerformed;
        clickAction.canceled += OnClickCanceled;
    }

    private void OnDisable()
    {
        // Remove a inscrição para evitar erros
        clickAction.performed -= OnClickPerformed;
        clickAction.canceled -= OnClickCanceled;
        
        controls.Gameplay.Disable();
    }

    // Esta função é chamada QUANDO o botão de clique é PRESSIONADO
    private void OnClickPerformed(InputAction.CallbackContext context)
    {
        // 1. Pega a posição do mouse na tela
        Vector2 mousePos = mousePositionAction.ReadValue<Vector2>();

        // 2. Lança um raio da câmera para o mundo do jogo nesta posição
        RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(mousePos), Vector2.zero);

        // 3. Verifica se o raio atingiu ALGUM collider
        if (hit.collider != null)
        {
            // 4. Verifica se o collider que atingimos é o DESTE objeto
            if (hit.collider.gameObject == this.gameObject)
            {
                Debug.Log("CLIQUE DETECTADO em " + gameObject.name + " com o novo Input System!");
                isDragging = true;
                offset = transform.position - mainCamera.ScreenToWorldPoint(mousePos);
            }
        }
    }

    // Esta função é chamada QUANDO o botão de clique é SOLTO
    private void OnClickCanceled(InputAction.CallbackContext context)
    {
        Debug.Log("CLIQUE SOLTO de " + gameObject.name);
        isDragging = false;
    }

    // A lógica de arrastar agora vai no Update
    void Update()
    {
        if (isDragging)
        {
            Vector2 mousePos = mousePositionAction.ReadValue<Vector2>();
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePos);
            transform.position = worldPosition + offset;
            Debug.Log("ARRASTANDO " + gameObject.name);
        }
    }
}