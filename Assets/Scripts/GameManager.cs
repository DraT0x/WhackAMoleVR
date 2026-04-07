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
            AfficherTimer();
            if (tempsEcoule < 0f) TerminerPartie();
        }

    }

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
        tempsEcoule = 20f;
        timerActif = true;
        AfficherTimer();
        ChangerEtat(EtatJeu.EnJeu);
        StartCoroutine(SpawnLoop());
    }

    /// <summary>
    /// Arrêt de la partie en cours
    /// </summary>
    private void TerminerPartie()
    {
        Debug.Log(tempsEcoule);
        timerActif = false;
        int score = Mathf.Max(100, 1000 - Mathf.FloorToInt(tempsEcoule) * 10);
        texteScoreFinal.text = $"Score : {score}";
        ChangerEtat(EtatJeu.GameOver);
    }

    public void Rejouer()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    private void AfficherTimer()
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
    }

    private int ChoisirSpawn()
    {
        if (etatActuel != EtatJeu.EnJeu) return -1;
        int spawnChoisi = -1;

        while (spawnChoisi < 0)
        {
            int nombreAleatoire = UnityEngine.Random.Range(0, disponibiliteSpawn.Length);

            if (!disponibiliteSpawn[nombreAleatoire]) continue;
            disponibiliteSpawn[nombreAleatoire] = false;
            spawnChoisi = nombreAleatoire;
        }

        return spawnChoisi;
    }

    public void SpawnTaupe()
    {
        if (etatActuel != EtatJeu.EnJeu) return;

        int spawnChoisi = ChoisirSpawn();
        if (spawnChoisi < 0) return;

        Transform transformSpawnChoisi = SpawnerTaupe.transform.Find(spawnChoisi.ToString());
        GameObject taupe = Instantiate(TaupePrefab, transformSpawnChoisi.position, Quaternion.identity, transformSpawnChoisi); // Généré par Claude.AI - 2026-04-06

        StartCoroutine(SupprimerTaupe(taupe, 2f));
    }

    /// <summary>
    /// Détruit automatiquement la taupe après un délai si elle n'a pas été frappée
    /// </summary>
    private IEnumerator SupprimerTaupe(GameObject taupe, float delai)
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
        while (true)
        {
            if (etatActuel == EtatJeu.EnJeu)
            {
                SpawnTaupe();
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
