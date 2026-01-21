using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI scoreText;
    public Image speedBar;

    [Header("Game Over System")]
    public GameOverManager gameOverManager;

    [Header("Wizualne - Rotacja Modelu")]
    public Transform carModel;
    public float maxSteerAngle = 15f;
    public float rotationSmoothness = 10f;

    [Header("Prędkość i Przyspieszenie")]
    public float forwardSpeed = 15f;
    public float normalSpeed = 15f;
    public float maxSpeed = 30f;
    public float minSpeed = 8f;
    public float accelerationSpeed = 5f;

    [Header("Bandy i Tarcie")]
    public float scrapeSpeed = 5f;
    private bool isScraping = false;

    [Header("Skręcanie")]
    public float steeringSpeed = 10f;

    [Header("Ograniczenia")]
    public float laneDistance = 13f;

    // --- NOWE: Zmienne do sterowania dotykowego ---
    [Header("Sterowanie Dotykowe (System)")]
    private bool moveLeft = false;
    private bool moveRight = false;
    private bool moveGas = false;
    private bool moveBrake = false;
    // ----------------------------------------------

    private float score = 0f;
    private Rigidbody rb;
    private float moveInput;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        forwardSpeed = normalSpeed;

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.freezeRotation = true;
        rb.useGravity = true;
        rb.isKinematic = false;

        if (carModel == null)
        {
            Debug.LogWarning("UWAGA: Nie przypisałeś 'Car Model' w inspektorze!");
        }
    }

    void Update()
    {
        // --- 1. OBSŁUGA WEJŚCIA (Input) ---
        // Resetujemy wartości co klatkę
        moveInput = 0f;
        float speedInput = 0f;

        // A. Klawiatura (do testów na PC)
        moveInput = Input.GetAxis("Horizontal");
        speedInput = Input.GetAxis("Vertical");

        // B. Akcelerometr (opcjonalnie - jeśli telefon ma przechylanie)
        if (SystemInfo.supportsAccelerometer && Mathf.Abs(Input.acceleration.x) > 0.1f)
        {
            moveInput += Input.acceleration.x * 2.0f;
        }

        // C. Przyciski ekranowe (Nadpisują wszystko inne)
        if (moveLeft) moveInput = -1f;
        if (moveRight) moveInput = 1f;

        if (moveGas) speedInput = 1f;    // 1 oznacza "Gaz do dechy"
        if (moveBrake) speedInput = -1f; // -1 oznacza "Hamulec"

        // --- 2. ROTACJA MODELU ---
        if (carModel != null && !isDead)
        {
            float targetAngle = moveInput * maxSteerAngle;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            carModel.localRotation = Quaternion.Lerp(carModel.localRotation, targetRotation, Time.deltaTime * rotationSmoothness);
        }

        // --- 3. UI ---
        if (speedText != null)
        {
            int displaySpeed = Mathf.RoundToInt(forwardSpeed * 3.0f);
            speedText.text = displaySpeed.ToString() + " KM/H";
        }

        if (speedBar != null)
        {
            speedBar.fillAmount = forwardSpeed / maxSpeed;
            speedBar.color = Color.Lerp(Color.green, Color.red, forwardSpeed / maxSpeed);
        }

        // --- 4. PUNKTY ---
        if (!isDead)
        {
            float pointsMultiplier = 1.0f;
            if (isScraping) pointsMultiplier = 0f;
            else if (forwardSpeed >= maxSpeed * 0.9f) pointsMultiplier = 2.0f;
            else if (forwardSpeed <= minSpeed * 1.1f) pointsMultiplier = 0.2f;

            float pointsToAdd = forwardSpeed * pointsMultiplier * Time.deltaTime * 10f;
            score += pointsToAdd;

            if (scoreText != null)
            {
                scoreText.text = "PUNKTY: " + Mathf.FloorToInt(score).ToString("D6");
            }
        }

        if (isDead) return;

        // --- 5. FIZYKA PRĘDKOŚCI ---
        float targetSpeed = normalSpeed;

        if (isScraping)
        {
            targetSpeed = scrapeSpeed;
        }
        else
        {
            // Tutaj używamy naszego speedInput (z klawiatury lub przycisków)
            if (speedInput > 0.1f) targetSpeed = maxSpeed;      // Gaz
            else if (speedInput < -0.1f) targetSpeed = minSpeed; // Hamulec
        }

        float changeRate = isScraping ? accelerationSpeed * 3f : accelerationSpeed;
        forwardSpeed = Mathf.Lerp(forwardSpeed, targetSpeed, Time.deltaTime * changeRate);
    }

    void FixedUpdate()
    {
        if (isDead)
        {
            rb.linearVelocity = Vector3.zero; // Unity 6 (w starszych użyj rb.velocity)
            return;
        }

        Vector3 currentPos = rb.position;
        Vector3 forwardMove = Vector3.forward * forwardSpeed * Time.fixedDeltaTime;
        Vector3 sideMove = Vector3.right * moveInput * steeringSpeed * Time.fixedDeltaTime;
        Vector3 targetPos = currentPos + forwardMove + sideMove;

        targetPos.x = Mathf.Clamp(targetPos.x, -laneDistance, laneDistance);

        rb.MovePosition(targetPos);
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Banda")) isScraping = true;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Banda")) isScraping = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("GAME OVER!");
            isDead = true;
            forwardSpeed = 0;
            rb.isKinematic = true;

            if (gameOverManager != null)
            {
                gameOverManager.TriggerGameOver(score);
            }
        }
    }

    
    public void SteerLeftDown() { moveLeft = true; }
    public void SteerLeftUp() { moveLeft = false; }

    public void SteerRightDown() { moveRight = true; }
    public void SteerRightUp() { moveRight = false; }

    public void GasDown() { moveGas = true; }
    public void GasUp() { moveGas = false; }

    public void BrakeDown() { moveBrake = true; }
    public void BrakeUp() { moveBrake = false; }
}