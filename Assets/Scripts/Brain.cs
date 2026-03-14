using UnityEngine;

public class Brain : MonoBehaviour
{
    public GameManager gameManager;   // atsauce uz GameManager
    public AudioClip hit;             // skaņa, kad savāc collectible
    private AudioSource source;

    private int lineIndex = -1;       // kura līnija šim collectible pieder
    private float destroyDelay = 0.2f; // laiks, cik ilgi atskaņot skaņu pirms iznīcināšanas

    void Start()
    {
        // Pārliecināmies, ka AudioSource ir pievienots
        source = GetComponent<AudioSource>();
        if (source == null)
            source = gameObject.AddComponent<AudioSource>();
    }

    // Uzstāda līnijas index un GameManager atsauci
    public void SetLineIndex(int index, GameManager manager)
    {
        lineIndex = index;
        gameManager = manager;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Zombie"))
        {
            // Piešķir punktu GameManager
            if (gameManager != null)
            {
                gameManager.AddScore();

                // Atbrīvo līniju
                if (lineIndex >= 0)
                {
                    gameManager.FreeLine(lineIndex);
                }
            }

            // Atskaņo skaņu
            if (hit != null)
            {
                if (source != null)
                {
                    source.PlayOneShot(hit);
                }
                else
                {
                    // Alternatīva: atskaņo skaņu uz noteiktu punktu pasaulē
                    AudioSource.PlayClipAtPoint(hit, transform.position);
                }
            }

            // Izdzēš objektu ar nelielu delay, lai skaņa paspētu atskanēt
            Destroy(gameObject, destroyDelay);
        }
    }
}