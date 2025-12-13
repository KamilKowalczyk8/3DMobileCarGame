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

    [Header("Wizualne - Rotacja Modelu (NOWE)")]
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
            Debug.LogWarning("UWAGA: Nieypisałeś 'Car Model' w inspektorze!");
        }
    }

    void Update()
    {
        moveInput = Input.GetAxis("Horizontal");
        if (SystemInfo.supportsAccelerometer && Mathf.Abs(Input.acceleration.x) > 0.1f)
        {
            moveInput += Input.acceleration.x * 2.0f;
        }

        if (carModel != null && !isDead)
        {
            float targetAngle = moveInput * maxSteerAngle;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            carModel.localRotation = Quaternion.Lerp(carModel.localRotation, targetRotation, Time.deltaTime * rotationSmoothness);
        }

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

        float speedInput = Input.GetAxis("Vertical");
        float targetSpeed = normalSpeed;

        if (isScraping) targetSpeed = scrapeSpeed;
        else
        {
            if (speedInput > 0.1f) targetSpeed = maxSpeed;
            else if (speedInput < -0.1f) targetSpeed = minSpeed;
        }

        float changeRate = isScraping ? accelerationSpeed * 3f : accelerationSpeed;
        forwardSpeed = Mathf.Lerp(forwardSpeed, targetSpeed, Time.deltaTime * changeRate);
    }

    void FixedUpdate()
    {
        if (isDead)
        {
            rb.linearVelocity = Vector3.zero;
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
}