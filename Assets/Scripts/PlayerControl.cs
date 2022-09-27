using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerControl : MonoBehaviour
{
    [Header("Script Database")]
    public SwitchPlan switchPlan;

    [Header("Player Settings")]
    public int walkSpeed = 10;
    public float attackSpeed = 1.5f;
    public int jumpForce = 8; 
    public int bulletsNbr;
    public int deadNbr = 0;
    public GameObject bulletPrefab;
    
    private float deadSpeed = 1;
    private float nextDead = 0;

    [Header("Check Point Respawn")]
    private Vector2 respawn;

    [Header("UI Settings")]
    public TextMeshProUGUI bulletsNbrUI;
    public TextMeshProUGUI deadNbrUI;

    private Rigidbody2D playerRigidbody;
    private bool switchPossible, firePossible;

    // Start is called before the first frame update
    void Start()
    {
        UpdateBulletsNbr(10);

        respawn = transform.position;

        playerRigidbody = GetComponent<Rigidbody2D>();
        switchPossible = false;
        firePossible = true;
    }

    // Update is called once per frame
    void Update()
    {
        nextDead += Time.deltaTime;

        if(Input.GetMouseButtonDown(1) && firePossible)
        {
            Shoot();
        }

        // Si vous entrez dans un "switch way" et que vous cliquez sur flêche up
        // alors le script "switch plan" vous switch de plan        
        if(switchPossible && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Z)))
        {
            switchPlan.MoveToOtherPlan();
        }

        // Pour sauter, don d'une vitesse à la masse (rigidbody) du player par un vecteur (0,1) = up
        // Le player peut sauter ssi sa vitesse est quasi nulle
        if(Input.GetKeyDown(KeyCode.Space) && Mathf.Abs(playerRigidbody.velocity.y) < 0.001f)
        {
            playerRigidbody.velocity = Vector2.up * jumpForce;
        }
    }

    // Update is called once per call
    void FixedUpdate()
    {
        float mouvement = Input.GetAxis("Horizontal"); // Récup de la direction du déplacement selon les touches de clavier 
        transform.position += new Vector3(mouvement, 0, 0) * Time.deltaTime * walkSpeed; // Modif de position du player selon la direction du mouvement
        UpdateLookDir(mouvement); // Le joueur regarde dans le sens du mvt
    }

    private void UpdateLookDir(float mouvement)
    {
        if(mouvement < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if(mouvement > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }
    
    private void Shoot()
    {
        // Récup de la position du clique de souris dans le référentiel de la camera et non de l'écran de PC
        // On soustrait la position du player car celui-ci n'est pas au centre de la caméra
        // Puis calcul de l'angle formé en les deux position (x et y) dont l'origine du vecteur est la le centre de la camera
        if(bulletsNbr > 0)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            float mouseAng = Mathf.Atan2(mousePos.y, mousePos.x);

            // Création du projectile à la position du player
            GameObject bulletClone = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            // On envoit à script du project la direction en x et y pour s'y déplacer
            bulletClone.GetComponent<BulletControl>().initiateMoveDir(Mathf.Cos(mouseAng), Mathf.Sin(mouseAng));
            
            UpdateBulletsNbr(-1);
            firePossible = false;
            StartCoroutine(CoolDown());
        }
    }

    public void UpdateBulletsNbr(int delta)
    {
        // Met à jour le nombre de munitions et le texte associé
        bulletsNbr += delta;
        bulletsNbrUI.text = bulletsNbr.ToString();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Switcher"))
        {
            respawn = transform.position;
            switchPossible = true;
        }

        // La condition de superiorité permet de ne pas se faire spam de coup
        // Défaut : si l'ennemi reste sur moi et qu'il peut me taper, je ne permets pas de vie car son collider n'est pas rentré
        if(nextDead >= deadSpeed && (other.CompareTag("BulletHit") || other.CompareTag("Hit")))
        {
            nextDead = 0;
            deadNbr++;
            deadNbrUI.text = deadNbr.ToString();
            StartCoroutine(Respawn(0.5f));
        }
        
        if(other.CompareTag("BulletHit"))
        {
            Destroy(other.gameObject); //Destroy bullet at the impact
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Switcher"))
        {
            switchPossible = false;
        }
    }

    //Permet de ne pouvoir tirer que toutes les "attack speed" secondes
    IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(attackSpeed);
        firePossible = true;
    }

    IEnumerator Respawn(float secondes)
    {
        yield return new WaitForSeconds(secondes);
        transform.position = respawn;
    }
}