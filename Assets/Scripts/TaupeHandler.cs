/*
* Gestion de la taupe
* @author : Félix Dupras-Simard
* Inspiré de https://gamedevbeginner.com/singletons-in-unity-the-right-way/ - [consulté le 2024-04-06]
*/
using UnityEngine;


public class TaupeHandler : MonoBehaviour
{   
    [Header("Informations Taupe")]
    [SerializeField] private int valeurPoint = 5;

    public int IndexSpawn { get; set; }

    // Variable Interne
    private bool taupeEstTouche;


    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        if (taupeEstTouche) return;
        if (collision.gameObject.tag != "MarteauJoueur") return;

        taupeEstTouche = true;

        GameManager.Instance.AjouterPoint(valeurPoint);
        GameManager.Instance.SupprimerTaupe(gameObject);
    }
}
