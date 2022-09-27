using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Note : la composante est Z n'existe pas donc change l'ordre des plans ne les déplacent pas selon z
// mais gère uniquement lequel sera affiché sur les autres, d'où la nécessité d'activer et désactiver
// les colliders.

public class SwitchPlan : MonoBehaviour
{
    public List<GameObject> mapElementsList = new List<GameObject>();
    private List<string> plansList = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        // Ajout du nombre de plans dans le jeu dans une liste
        plansList.Add("Plan1");
        plansList.Add("Plan2");

        ChangePlanVisible();
    }

    public void MoveToOtherPlan()
    {
        UpdateMapElementsList();

        plansList.Add(plansList[0]);
        plansList.Remove(plansList[0]);

        ChangePlanVisible();
    }

    private void ChangePlanVisible()
    {
        // Pour chaque élément consitituant la map, par défaut, l'élément est désactivé
        // et est placé au plan 1 
        foreach (var item in mapElementsList)
        {
            item.SetActive(false);
            
            // Si l'élément considéré possède le tag du 1er plan (plan visible) de la liste des plans
            // alors il est activé et il est placé au premier plan (2)
            if(item.GetComponent<MapElements>().myTag == plansList[0])
            {
                item.SetActive(true);
            }
        }
    }

    private void UpdatePlanVisible()
    {
        // Pour chaque élément consitituant la map, par défaut, son collider est désactivé
        // et est placé au plan 1 
        foreach (var item in mapElementsList)
        {
            item.GetComponent<TilemapCollider2D>().enabled = false;
            item.GetComponent<TilemapRenderer>().sortingOrder = 1;

            // Si l'élément considéré possède le tag du 1er plan (plan visible) de la liste des plans
            // alors son collider est activé et il est placé au premier plan (2)
            if(item.GetComponent<MapElements>().myTag == plansList[0])
            {
                item.GetComponent<TilemapCollider2D>().enabled = true;
                item.GetComponent<TilemapRenderer>().sortingOrder = 2;
            }
        }
    }

    private void UpdateMapElementsList()
    {
        for (int i = 0; i < mapElementsList.Count; i++)
        {
            if(mapElementsList[i] == null)
            {
                mapElementsList.Remove(mapElementsList[i]);
            }
        }
    }
}
