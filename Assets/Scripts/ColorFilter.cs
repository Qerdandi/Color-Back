using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Linq;

public class ColorFilter : MonoBehaviour
{
    private PostProcessVolume volume;
    public ColorGrading colorGrading;
    private AnimationCurve animationCurve = new AnimationCurve();
    private Dictionary<string, float> colorTimeDico = new Dictionary<string, float>();

    // Start is called before the first frame update
    void Start()
    {
        volume = GetComponent<PostProcessVolume>(); //Récup du volume de post processing PP dans la camera
        volume.profile.TryGetSettings(out colorGrading); //Récup de l'effet "color grading" dans le volume PP

        BlackAndWhite();
    }

    private void BlackAndWhite()
    {
        colorTimeDico.Add("Rouge", 0f);
        colorTimeDico.Add("Orange", 0.01f);
        colorTimeDico.Add("Jaune", 0.2f);
        colorTimeDico.Add("Vert", 0.332f);
        colorTimeDico.Add("Bleu", 0.65f);
        colorTimeDico.Add("Violet", 0.8f);

        // Pour chaque couleur du dictionnaire, créé un point sur une courbe dont l'amplitude est 
        // nulle et le temps correspond à la valeur de le clé (couleur)
        foreach (string key in colorTimeDico.Keys)
        {
            animationCurve.AddKey(colorTimeDico[key], 0);      
        }

        // On insert la courbe créée dans hueVSSatCurve de l'effet color grading du PP
        colorGrading.hueVsSatCurve.Override(
            new Spline(
                animationCurve,
                0, //valeur par défaut en cas de manque de valeur
                true, //la courbe est-elle périodique ?
                new Vector2(0, 1) //jsp à quoi ça sert !
            )
        );
    }

    private void AddColor(string color)
    {
        // Lorsqu'il faut activer une couleur particulière, on modifie les points de la courbe 
        // déjà existants en modifiant leur amplitude à 0.5 (= saturation)
        animationCurve.MoveKey(
            colorTimeDico.Keys.ToList().IndexOf(color), // Récupère l'index dans le dic dont la clé est "color"
            new Keyframe(colorTimeDico[color], 0.5f)
        );
    }

    // Update is called once per frame
    void Update()
    {
        switch (Input.inputString)
        {
            case "i":
                AddColor("Orange");
                break;
            case "o":
                AddColor("Jaune");
                break;
            case "p":
                AddColor("Vert");
                break;
            case "k":
                AddColor("Bleu");
                break;
            case "l":
                AddColor("Violet");
                break;
            case "m":
                AddColor("Rouge");
                break;
        }
    }
}
