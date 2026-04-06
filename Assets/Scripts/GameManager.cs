/*
* Singleton du game manager pour le jeu de whack-a-mole
* @author : Félix Dupras-Simard
*/
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Spawn")]
    [SerializeField] private GameObject SpawnerTaupe;
    [SerializeField] private GameObject TaupePrefab;
    private bool[] disponibiliteSpawn =
    {
        true, true, true,
        true, true, true,
        true, true, true
    };

    // Gestion de la partie
    private bool partieTermine = false;
    private int pointPartie = 0;

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

        SpawnTaupe();
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

    public void SpawnTaupe()
    {
        if (partieTermine) return;
        int spawnChoisi = 0;

        // choisir aléatoirement parmi les cases disponibles



        Transform transformSpawnChoisi = SpawnerTaupe.transform.Find(spawnChoisi.ToString());
        disponibiliteSpawn[spawnChoisi] = false;

        Instantiate(TaupePrefab, transformSpawnChoisi.position, Quaternion.identity, transformSpawnChoisi); // Généré par Claude.AI - 2026-04-06
    }

    public void SupprimerTaupe(GameObject taupe)
    {
        if (partieTermine) return;
        if (taupe.transform.parent == null || taupe.transform.parent.tag != "CaseSpawner") return;

        int SpawnNumber = Int32.Parse(taupe.transform.parent.gameObject.name);
        disponibiliteSpawn[SpawnNumber] = true;
        Destroy(taupe);
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
