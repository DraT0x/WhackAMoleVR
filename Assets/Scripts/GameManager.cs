/*
* Singleton du game manager pour le jeu de whack-a-mole
* @author : Félix Dupras-Simard
* Inspiré des notes de cours d'environnements immersifs de Frédérik Taleb - [consulté le 2024-04-06]
*/
using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Spawn")]
    [SerializeField] private GameObject SpawnerTaupe;
    [SerializeField] private GameObject TaupePrefab;
    [SerializeField] private AudioClip audioSpawn;
    private bool[] disponibiliteSpawn =
    {
        true, true, true,
        true, true, true,
        true, true, true
    };

    /// <summary>
    /// Gestion de la partie
    /// </summary>
    public enum EtatJeu { Menu, EnJeu, GameOver }

    [Header("Canvas")]
    [SerializeField] private GameObject canvasMenu;
    [SerializeField] private GameObject canvasHUD;
    [SerializeField] private GameObject canvasGameOver;

    [Header("Textes")]
    [SerializeField] private TextMeshProUGUI texteTimer;
    [SerializeField] private TextMeshProUGUI texteScoreActuel;
    [SerializeField] private TextMeshProUGUI texteScoreFinal;

    private EtatJeu etatActuel;
    private int pointPartie = 0;
    private float tempsEcoule;
    private bool timerActif;

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
    }

    void Start()
    {
        ChangerEtat(EtatJeu.Menu);
    }

    void Update()
    {
        if (timerActif)
        {
            tempsEcoule -= Time.deltaTime;
            AfficherCompteRebours();
            if (tempsEcoule < 0f) TerminerPartie();
        }

    }

    /// <summary>
    /// Change l'état du jeu
    /// </summary>
    /// <param name="nouvelEtat">Nouvel état du jeu</param>
    public void ChangerEtat(EtatJeu nouvelEtat)
    {
        etatActuel = nouvelEtat;
        canvasMenu.SetActive(etatActuel == EtatJeu.Menu);
        canvasHUD.SetActive(etatActuel == EtatJeu.EnJeu);
        canvasGameOver.SetActive(etatActuel == EtatJeu.GameOver);
    }

    /// <summary>
    /// Initialisation de la partie
    /// </summary>
    public void CommencerPartie()
    {
        tempsEcoule = 30f;
        timerActif = true;
        pointPartie = 0;
        AfficherCompteRebours();
        ChangerEtat(EtatJeu.EnJeu);
        StartCoroutine(SpawnLoop());
        texteScoreActuel.text = $"Score : {pointPartie}";
    }

    /// <summary>
    /// Arrêt de la partie en cours
    /// </summary>
    private void TerminerPartie()
    {
        timerActif = false;
        texteScoreFinal.text = $"Score : {pointPartie}";
        ChangerEtat(EtatJeu.GameOver);
        SupprimerAllTaupe();
    }

    /// <summary>
    /// Redémarre la partie
    /// </summary>
    /// J'ai laissé une méthode séparé si je devais implémenter une logique de code 
    /// Changement de map
    public void Rejouer()
    {
        CommencerPartie();
    }

    /// <summary>
    /// Affiche le compte à rebours
    /// </summary>
    private void AfficherCompteRebours()
    {
        int minutes = Mathf.FloorToInt(tempsEcoule / 60f);
        int secondes = Mathf.FloorToInt(tempsEcoule % 60f);
        texteTimer.text = $"{minutes:00}:{secondes:00}";
    }

    /// <summary>
    /// Ajoute la valeur de point au joueur
    /// </summary>
    /// <param name="quantitePoint">La valeur à ajouter</param>
    public void AjouterPoint(int quantitePoint)
    {
        if (quantitePoint < 0) return;

        pointPartie += quantitePoint;
        texteScoreActuel.text = $"Score : {pointPartie}";
    }

    /// <summary>
    /// Sélection du spawn de la taupe
    /// </summary>
    /// <returns>Retourne le spawn choisi</returns>
    private int ChoisirSpawn()
    {
        if (etatActuel != EtatJeu.EnJeu) return -1;
        int spawnChoisi = -1;

        // Vérifie si il y a une case disponible
        // Généré par Claude.AI - 2026-04-07 
        // prompt : Fait en sorte que la vérification du tableau soit plus optimisé.
        if (!System.Array.Exists(disponibiliteSpawn, dispo => dispo)) return -1;

        while (spawnChoisi < 0)
        {
            int nombreAleatoire = UnityEngine.Random.Range(0, disponibiliteSpawn.Length);

            if (!disponibiliteSpawn[nombreAleatoire]) continue;
            disponibiliteSpawn[nombreAleatoire] = false;
            spawnChoisi = nombreAleatoire;
        }

        return spawnChoisi;
    }

    /// <summary>
    /// Apparition de la taupe
    /// </summary>
    public void SpawnTaupe()
    {
        if (etatActuel != EtatJeu.EnJeu) return;

        int spawnChoisi = ChoisirSpawn();
        if (spawnChoisi < 0) return;

        Transform transformSpawnChoisi = SpawnerTaupe.transform.Find(spawnChoisi.ToString());
        GameObject taupe = Instantiate(TaupePrefab, transformSpawnChoisi.position, Quaternion.identity, transformSpawnChoisi); // Généré par Claude.AI - 2026-04-06
        transformSpawnChoisi.GetComponent<AudioSource>().PlayOneShot(audioSpawn);

        StartCoroutine(SupprimerTaupe(taupe, 3f));
    }

    /// <summary>
    /// Détruit toute les taupes présentes dans le spawner
    /// </summary>
    private void SupprimerAllTaupe()
    {
        for (int i = 0; i < disponibiliteSpawn.Length; i++)
        {
            disponibiliteSpawn[i] = true;
        }

        foreach (Transform spawner in SpawnerTaupe.transform)
        {
            foreach (Transform enfant in spawner)
            {
                Destroy(enfant.gameObject);
            }
        }
    }

    /// <summary>
    /// Détruit automatiquement la taupe après un délai si elle n'a pas été frappée
    /// </summary>
    public IEnumerator SupprimerTaupe(GameObject taupe, float delai)
    {
        yield return new WaitForSeconds(delai);

        if (taupe != null && etatActuel == EtatJeu.EnJeu && taupe.transform.parent != null && taupe.transform.parent.CompareTag("CaseSpawner"))
        {
            int SpawnNumber = Int32.Parse(taupe.transform.parent.gameObject.name);
            disponibiliteSpawn[SpawnNumber] = true;
            Destroy(taupe);
        }
    }

    IEnumerator SpawnLoop()
    {
        while (etatActuel == EtatJeu.EnJeu)
        {
            SpawnTaupe();
            yield return new WaitForSeconds(1f);
        }
    }
}
