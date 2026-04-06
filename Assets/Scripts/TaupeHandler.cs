/*
* Gestion de la taupe
* @author : Félix Dupras-Simard
* Inspiré de https://gamedevbeginner.com/singletons-in-unity-the-right-way/ - [consulté le 2024-04-06]
*/
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TaupeHandler : MonoBehaviour
{
    [Header("Informations Taupe")]
    [SerializeField] private int valeurPoint = 5;

    [Header("Audio")]
    [SerializeField] private AudioClip sonContactMarteau;
    private AudioSource audioSource;

    public int IndexSpawn { get; set; }
    private bool taupeEstTouche;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0.1f;
        audioSource.spatialBlend = 1f; // 100% 3D
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.maxDistance = 5f;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (taupeEstTouche) return;
        if (collision.gameObject.tag != "MarteauJoueur") return;

        taupeEstTouche = true;
        audioSource.PlayOneShot(sonContactMarteau);

        GameManager.Instance.AjouterPoint(valeurPoint);
        //GameManager.Instance.SupprimerTaupe(gameObject);
    }
}
