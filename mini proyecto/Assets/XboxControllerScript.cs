using UnityEngine;

public class XboxControllerScript : MonoBehaviour
{
    public float velocidad = 5f;
    public float fuerzaSalto = 10f;
    public float longitudRaycast = 2.45f;
    public LayerMask capaSuelo;

    private bool enSuelo;
    private Rigidbody2D rb;
    private Animator animator;

    // Estado de daño
    private bool dañoActivo = false;

    // Atributo de vida
    public float vida = 100f;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * longitudRaycast);
    }

    void CambiarAIdle()
    {
        animator.SetInteger("State", 0); // → Volver a Idle
        dañoActivo = false;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Verificamos si está tocando el suelo con raycast
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, capaSuelo);
        enSuelo = hit.collider != null;

        int state = animator.GetInteger("State");
        Debug.Log("estado " + state);

        switch (state)
        {
            case 0: // Idle
                if (vida <= 0)
                {
                    animator.SetInteger("State", 5); // → Death
                    break;
                }

                if (Input.GetKeyDown("w") && enSuelo)
                {
                    rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
                    animator.SetInteger("State", 3); // → Jumping
                    break;
                }
                else if ((Input.GetKey("d") || Input.GetKey("a")) && enSuelo)
                {
                    animator.SetInteger("State", 1); // → Running
                }
                else if (Input.GetKeyDown("j"))
                {
                    animator.SetInteger("State", 2); // → Attacking
                }
                break;

            case 1: // Running
                if (vida <= 0)
                {
                    animator.SetInteger("State", 5); // → Death
                    break;
                }

                if (Input.GetKeyDown("w") && enSuelo)
                {
                    rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
                    animator.SetInteger("State", 3); // → Jumping
                    break;
                }
                else if (Input.GetKeyDown("j"))
                {
                    animator.SetInteger("State", 2); // → Attacking
                }
                else if (Input.GetKey("d"))
                {
                    transform.position += new Vector3(velocidad * Time.deltaTime, 0, 0);
                    transform.localScale = new Vector3(1, 1, 1);
                }
                else if (Input.GetKey("a"))
                {
                    transform.position += new Vector3(-velocidad * Time.deltaTime, 0, 0);
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                else
                {
                    animator.SetInteger("State", 0); // → Idle
                }
                break;

            case 2: // Attacking
                if (vida <= 0)
                {
                    animator.SetInteger("State", 5); // → Death
                    break;
                }

                if (!Input.GetKey("j"))
                {
                    if ((Input.GetKey("d") || Input.GetKey("a")) && enSuelo)
                        animator.SetInteger("State", 1); // → Running
                    else
                        animator.SetInteger("State", 0); // → Idle
                }
                break;

            case 3: // Jumping
                if (vida <= 0)
                {
                    animator.SetInteger("State", 5); // → Death
                    break;
                }

                if (Input.GetKey("d"))
                {
                    transform.position += new Vector3(velocidad * Time.deltaTime, 0, 0);
                    transform.localScale = new Vector3(1, 1, 1);
                }
                else if (Input.GetKey("a"))
                {
                    transform.position += new Vector3(-velocidad * Time.deltaTime, 0, 0);
                    transform.localScale = new Vector3(-1, 1, 1);
                }

                if (enSuelo)
                {
                    if (Input.GetKey("d") || Input.GetKey("a"))
                    {
                        animator.SetInteger("State", 1); // → Running
                    }
                    else
                    {
                        animator.SetInteger("State", 0); // → Idle
                    }
                }
                break;

            case 4: // Damage
                if (!dañoActivo)
                {
                    dañoActivo = true;
                }

                Invoke("CambiarAIdle", 2f);

                if (vida <= 0)
                {
                    animator.SetInteger("State", 5); // → Death
                }
                else
                {
                    animator.SetInteger("State", 1); // → Running
                }
                break;

            case 5: // Death
                // Detenemos el movimiento en el estado de muerte
                rb.velocity = Vector2.zero; // Detenemos la velocidad
                // Evitar que se realicen acciones mientras el personaje está muerto
                break;
        }

        // Corrección por si salta estando en otro estado inesperado
        if (!enSuelo && animator.GetInteger("State") != 3)
        {
            animator.SetInteger("State", 3); // → Jumping
        }

        if (Input.GetKeyDown("k"))
        {
            animator.SetInteger("State", 4); // Forzar estado de daño
            vida -= 20f; // Ejemplo de daño
        }
    }
}
