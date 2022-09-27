using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    public float speed = 15;
    public int lifeTime = 1;
    private float mouseDirX, mouseDirY;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void initiateMoveDir(float mouseDirX, float mouseDirY)
    {
        // On récupère les coordonnées du clique de la souris depuis le script du player
        this.mouseDirX = mouseDirX;
        this.mouseDirY = mouseDirY;
    }

    // Update is called once per frame
    void Update()
    {
        // La position du projectile évolue dans la direction du clique de la souris à la 
        // vitesse paramétrée puis est détruit au bout de 3s
        transform.position += new Vector3(mouseDirX, mouseDirY, 0) * Time.deltaTime * speed;
        Destroy(gameObject, lifeTime);
    }
}
