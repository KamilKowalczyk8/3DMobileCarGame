using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    [Header("Prêdkoœæ Ruchu Ulicznego")]
    public float minSpeed = 3f;
    public float maxSpeed = 7f;

    private float currentSpeed;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        currentSpeed = Random.Range(minSpeed, maxSpeed);
    }

    void FixedUpdate()
    {
        Vector3 movement = Vector3.forward * currentSpeed * Time.fixedDeltaTime;

        if (rb != null)
        {
            rb.MovePosition(rb.position + movement);
        }
        else
        {
            transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
        }
    }
}