using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f; 
    public float rotationSpeed = 1.5f;

    private Camera mainCamera;
    private Rigidbody rb;
    private Vector3 moveDirection;

    [Header("Key Interaction")]
    private GameObject currentKeyInRange;
    private Material keyOriginalMaterial;
    private Renderer keyRenderer;
    public Text winText;
    public TMP_Text interactionText; 

    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        RotateToCursor();
        HandleInput();

        if (currentKeyInRange != null && Input.GetKeyDown(KeyCode.Space))
        {
            WinGame();
        }
    }

    void FixedUpdate()
    {
        MoveCharacter();
    }

    // Rotate character to face mouse cursor
    void RotateToCursor()
    {
        if (mainCamera == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, transform.position);

        if (ground.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 lookDir = hitPoint - transform.position;
            lookDir.y = 0f;

            if (lookDir.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRot = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), rotationSpeed * Time.deltaTime);
                transform.rotation = targetRot;
            }
        }
    }

    // Read WASD input
    void HandleInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveDirection = (transform.forward * v + transform.right * h).normalized;
    }

    // Move using Rigidbody for proper collision
    void MoveCharacter()
    {
        if (moveDirection.sqrMagnitude > 0.0001f)
        {
            Vector3 targetPosition = rb.position + moveDirection * speed * Time.fixedDeltaTime;
            rb.MovePosition(targetPosition);
        }
    }

    // Detect entering key range
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key"))
        {
            currentKeyInRange = other.gameObject;

            keyRenderer = currentKeyInRange.GetComponent<Renderer>();
            if (keyRenderer != null)
            {
                keyOriginalMaterial = keyRenderer.material;
                keyRenderer.material.color = Color.blue; 
            }

            if (interactionText != null)
            {
                interactionText.text = "Press SPACE to collect the key and win the game!";
            }

            Debug.Log("Press SPACE to win!");
        }
    }

    // Detect leaving key range
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Key") && currentKeyInRange == other.gameObject)
        {
            if (keyRenderer != null && keyOriginalMaterial != null)
            {
                keyRenderer.material.color = keyOriginalMaterial.color;
            }

            currentKeyInRange = null;

            if (interactionText != null)
            {
                interactionText.text = "";
            }
        }
    }

    // Trigger win logic
    void WinGame()
    {
        Debug.Log("You collected the key! You win!");
        Destroy(currentKeyInRange); 
        if (winText != null)
            winText.gameObject.SetActive(true);
        StartCoroutine(LoadWinScene());
    }

    // Coroutine to load WinScene after delay
    IEnumerator LoadWinScene()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("WinScene", LoadSceneMode.Single);
    }
}