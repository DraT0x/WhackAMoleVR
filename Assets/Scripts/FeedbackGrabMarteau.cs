/*
* Gère les feedbacks sensoriels du marteau
* @author : Félix Dupras-Simard
* Inspiré des notes de cours d'environnements immersifs de Frédérik Taleb - [consulté le 2024-04-06]
*/

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(AudioSource))]
public class FeedbackGrabMarteau : MonoBehaviour
{
    [Header("Haptique")]
    [SerializeField] private float amplitudeGrabMarteau = 0.5f;
    [SerializeField] private float dureeGrabMarteau = 0.1f;

    [Header("Audio")]
    [SerializeField] private AudioClip sonGrabMarteau;

    private XRGrabInteractable grabInteractable;
    private AudioSource audioSource;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0.5f;
        audioSource.spatialBlend = 1f; // 100% 3D
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.maxDistance = 5f;
    }

    void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabEntered);
        grabInteractable.selectExited.AddListener(OnGrabExited);
    }

    void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabEntered);
        grabInteractable.selectExited.RemoveListener(OnGrabExited);
    }

    private void OnGrabEntered(SelectEnterEventArgs args)
    {
        var controller = args.interactorObject.transform.GetComponent<XRBaseInputInteractor>();
        controller.SendHapticImpulse(amplitudeGrabMarteau, dureeGrabMarteau);

        // Jouer le son à la position de l'objet
        audioSource.PlayOneShot(sonGrabMarteau);
    }

    private void OnGrabExited(SelectExitEventArgs args)
    {
        var controller = args.interactorObject.transform.GetComponent<XRBaseInputInteractor>();

        controller.SendHapticImpulse(amplitudeGrabMarteau * 0.3f, dureeGrabMarteau * 0.5f);
        audioSource.PlayOneShot(sonGrabMarteau);
    }
}