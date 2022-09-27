using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public EnemyType enemyType;

    [Header("Enemy Settings")]
    private GameObject hitBox;
    private int life, walkSpeed, attackSpeed, jumpForce; 
    public GameObject bulletPrefab;
    
    [Header("Action Settings")]
    private int retreatDistance, attackDistance, followDistance;

    private Rigidbody2D playerRigidbody;
    private float nextAttack;
    private Transform target;
    
    // Start is called before the first frame update
    void Start()
    {   
        hitBox = gameObject.transform.GetChild(0).gameObject;

        switch(enemyType)
        {
            case EnemyType.AuxPoings :
                EnemySettings(2, 9, 1, 8, 0, 1, 10);
            break;
            case EnemyType.AuCouteau :
                EnemySettings(2, 9, 1, 8, 0, 2, 10);
            break;
            case EnemyType.AuPistolet :
                EnemySettings(4, 8, 2, 8, 3, 5, 15);
                hitBox.tag = "Untagged";
            break;
            case EnemyType.GangChef :
                print("Coming Soon");
            break;
            case EnemyType.Boss :
                print("Coming Soon");
            break;
        }
        
        playerRigidbody = GetComponent<Rigidbody2D>(); 
        nextAttack = 0;
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    private void EnemySettings(int life, int walkSpeed, int attackSpeed, int jumpForce, int retreatDistance, int attackDistance, int followDistance)
    {
        this.life = life;
        this.walkSpeed = walkSpeed;
        this.attackSpeed = attackSpeed;
        this.jumpForce = jumpForce;
        this.retreatDistance = retreatDistance;
        this.attackDistance = attackDistance;
        this.followDistance = followDistance;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float distance = Vector2.Distance(transform.position, target.position);
        nextAttack += Time.fixedDeltaTime;

        if(distance > followDistance)
        {
            Wait();
        } 
        else 
        {
            if(distance >= retreatDistance + 1)
            {
                FollowOrRetreat(walkSpeed);
            }
            else if(distance <= retreatDistance)
            {
                FollowOrRetreat(-walkSpeed);
            }

            if(distance <= attackDistance && enemyType.Equals(EnemyType.AuPistolet))
            {
                Attack();
            }
        }
    }

    private void Wait()
    {
        // Ne fait RIEN !!!
    }

    private void FollowOrRetreat(int walkSpeed)
    {
        //Se déplace en dirction de la target
        transform.position = Vector2.MoveTowards(transform.position, target.position, walkSpeed*Time.deltaTime);
        Flip();
        Jump();
    }

    private void Flip()
    {
        // Change le sens du "regard" de l'ennemi
        Vector3 rotation = transform.eulerAngles;
        rotation.y = (transform.position.x  > target.position.x) ? 180f : 0f;
        transform.eulerAngles = rotation;
    }

    private void Jump()
    {
        Vector2 origin = transform.position + new Vector3(0, -0.6f, 0);
        Vector2 direction = ((transform.position.x  > target.position.x) ? -1 : 1)*Vector2.right;

        // Créé un laser fictif qui détecte tous colliders avec le layer "default"
        RaycastHit2D hit2D = Physics2D.Raycast(origin, direction, 2, LayerMask.GetMask("Default"));
        Debug.DrawRay(origin, 2*direction, Color.red);  
        
        // Si on détecte un collider qui va faire obstacle au mvt de l'ennemi alors saute (une fois)
        if(hit2D.collider != null && !hit2D.collider.isTrigger && GetComponent<Rigidbody2D>().velocity.y < 0.001f)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.up * jumpForce;
        }
    }

    private void Attack()
    {
        //Permet de ne pouvoir tirer que toutes les "attack speed" secondes
        if(nextAttack >= attackSpeed) //Si le temps du cool down est supérieur à la fréquence de tire
        {
            nextAttack = 0;

            Vector2 playerPos = target.transform.position - transform.position;
            float playerAngle = Mathf.Atan2(playerPos.y, playerPos.x);

            GameObject bulletClone = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            // On envoit à script du project la direction en x et y pour s'y déplacer
            bulletClone.GetComponent<BulletControl>().initiateMoveDir(Mathf.Cos(playerAngle), Mathf.Sin(playerAngle));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject); //Destroy bullet at the impact
            life--;
            if(life <= 0) { Destroy(gameObject, 0.1f); }          
        }
    }
}

public enum EnemyType {
    AuxPoings,
    AuCouteau,
    AuPistolet,
    GangChef,
    Boss
};
