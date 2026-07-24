using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMov : MonoBehaviour
{
    [Header("Componentes")]
    [SerializeField] private Rigidbody2D rb;
    // [SerializeField] private Animator anim; // Desactivado temporalmente en lo que pongo las animaciones
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private AudioSource audioSource;

    [Header("Ajustes de Movimiento")]
    [SerializeField] private float velMov = 8f;
    [SerializeField] private float velSalto = 12f;
    [SerializeField] private int maxSaltos = 2;

    [Header("Detección de Suelo")]
    [SerializeField] private Transform checkSuelo;
    [SerializeField] private float radioSuelo = 0.2f;
    [SerializeField] private LayerMask capaSuelo;

    [Header("Efectos de Audio")]
    [SerializeField] private AudioClip jumpSound;

    // Variables de Estado
    private float movX;
    private int saltosRestantes;
    private bool estaEnSuelo;
    private bool mirandoDerecha = true;

    // Estado público accesible por otros scripts
    public static int vida = 100;

    void Start()
    {
        // Auto-asignación de componentes si no están asignados en el Inspector
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        // if (anim == null) anim = GetComponent<Animator>(); // Desactivado temporalmente
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        saltosRestantes = maxSaltos;
    }

    void Update()
    {
        // 1. Leer Entrada de Controles 
        LeerEntrada();

        // 2. Comprobar Suelo
        ComprobarSuelo();

        // 3. Salto mediante Teclado/Botón
        if (Input.GetButtonDown("Jump"))
        {
            EjecutarSalto();
        }

        // 4. Actualizar Animaciones (Desactivado)
        // ActualizarAnimaciones();
    }

    void FixedUpdate()
    {
        // Aplicar Movimiento Físico
        AplicarMovimiento();
    }

    private void LeerEntrada()
    {
        // Se lee la entrada directamente del teclado (A/D o Flechas)
        movX = Input.GetAxisRaw("Horizontal");
    }

    private void AplicarMovimiento()
    {
        // Aplicar velocidad horizontal manteniendo la velocidad vertical actual
        rb.velocity = new Vector2(movX * velMov, rb.velocity.y);

        // Voltear el Sprite según la dirección
        if (movX > 0 && !mirandoDerecha)
        {
            VoltearSprite();
        }
        else if (movX < 0 && mirandoDerecha)
        {
            VoltearSprite();
        }
    }

    public void EjecutarSalto() // Método público para conectarlo a un Botón UI táctil si usas móvil
    {
        if (saltosRestantes > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, velSalto);
            saltosRestantes--;

            // Reproducir Sonido
            if (audioSource != null && jumpSound != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }
        }
    }

    private void ComprobarSuelo()
    {
        
        if (checkSuelo != null)
        {
            bool sueloAnterior = estaEnSuelo;
            estaEnSuelo = Physics2D.OverlapCircle(checkSuelo.position, radioSuelo, capaSuelo);

            if (estaEnSuelo && !sueloAnterior)
            {
                saltosRestantes = maxSaltos; // Resetear saltos al tocar tierra
            }
        }
        else
        {
            // Fallback si no hay CheckSuelo configurado
            if (Mathf.Abs(rb.velocity.y) < 0.01f)
            {
                estaEnSuelo = true;
                saltosRestantes = maxSaltos;
            }
            else
            {
                estaEnSuelo = false;
            }
        }
    }

    private void VoltearSprite()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    /* --- SECCIÓN DE ANIMACIONES DESACTIVADA ---
    private void ActualizarAnimaciones()
    {
        if (anim == null) return;

        anim.SetFloat("VelocidadX", Mathf.Abs(movX));
        anim.SetFloat("VelocidadY", rb.velocity.y);
        anim.SetBool("estaEnSuelo", estaEnSuelo);
    }
    */

    public void TomarDaño(int cantidad)
    {
        vida -= cantidad;

        /* // Lógica de animación de daño desactivada
        if (anim != null)
        {
            anim.SetTrigger("Daño");
        }
        */

        if (vida <= 0)
        {
            // Lógica de muerte
            Debug.Log("Luck ha caído...");
        }
    }

    // Dibujar el círculo de detección de suelo en la ventana
    private void OnDrawGizmosSelected()
    {
        if (checkSuelo != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(checkSuelo.position, radioSuelo);
        }
    }
}