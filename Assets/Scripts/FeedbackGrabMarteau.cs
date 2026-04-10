/*
* Gére les feedbacks sensoriels du marteau
* @author : Félix Dupras-Simard
* Inspiré des notes de cours d'environnements immersifs de Frédérik Taleb - [consulté le 2024-04-06]
*/

using Unity.VisualScripting;
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
    [SerializeField] private float amplitudeHitMarteau = 0.6f;
    [SerializeField] private float dureeHitMarteau = 0.3f;

    [Header("Audio")]
    [SerializeField] private AudioClip sonGrabMarteau;

    private XRGrabInteractable grabInteractable;
    private AudioSource audioSource;

    private XRBaseInputInteractor controller;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0.5f;
        audioSource.spatialBlend = 1f;
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

    /// <summary>
    /// Sélection du marteau
    /// </summary>
    /// <param name="args">args du grab</param>
    private void OnGrabEntered(SelectEnterEventArgs args)
    {
        controller = args.interactorObject.transform.GetComponent<XRBaseInputInteractor>();
        controller.SendHapticImpulse(amplitudeGrabMarteau, dureeGrabMarteau);

        audioSource.PlayOneShot(sonGrabMarteau);
    }

    /// <summary>
    /// Collision avec le marteau
    /// </summary>
    /// <param name="collision">Information sur la collision</param>
    void OnCollisionEnter(Collision collision)
    {
        if (controller == null) return;

        controller.SendHapticImpulse(amplitudeHitMarteau, dureeHitMarteau);
    }

    /// <summary>
    /// Drop du marteau
    /// </summary>args du grab
    /// <param name="args"></param>
    private void OnGrabExited(SelectExitEventArgs args)
    {
        controller = args.interactorObject.transform.GetComponent<XRBaseInputInteractor>();
        controller.SendHapticImpulse(amplitudeGrabMarteau * 0.3f, dureeGrabMarteau * 0.5f);

        audioSource.PlayOneShot(sonGrabMarteau);
    }
}