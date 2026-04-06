/*
* Singleton du game manager pour le jeu de whack-a-mole
* @author : Félix Dupras-Simard
*/
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private bool partieTermine;
    private int pointPartie;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        pointPartie = 0;
    }

    /// <summary>
    /// Ajoute la valeur de point au joueur
    /// </summary>
    /// <param name="quantitePoint">La valeur à ajouter</param>
    public void AjouterPoint(int quantitePoint)
    {
        if (quantitePoint < 0) return;

        pointPartie += quantitePoint;
    }

    /// <summary>
    /// Initialisation de la partie
    /// </summary>
    public void CommencerPartie()
    {
        partieTermine = false;
    }

    /// <summary>
    /// Arrêt de la partie en cours
    /// </summary>
    private void TerminerPartie()
    {
        partieTermine = true;
    }
}
