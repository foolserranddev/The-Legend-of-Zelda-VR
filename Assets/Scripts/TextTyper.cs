using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextTyper : MonoBehaviour
{

    public float letterPause = 0.1f;
    public AudioClip typeSound;
    public int RupeeLossAtEnd = 0;

    private AudioSource audioSource;
    private bool typing = false;
    private bool stopTyping = false;
    private string message;
    private TextMesh textComp;
    private Player player;
    private string originalMessage;

    // Use this for initialization
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = typeSound;
        textComp = GetComponent<TextMesh>();
        message = textComp.text;
        originalMessage = message;
    }

    private void OnEnable()
    {
        if (!typing) textComp.text = "";
    }

    private void OnDisable()
    {
        if (typing) Player.player.Mobilize();
        typing = false;
        textComp.text = "";
        message = originalMessage;
    }

    public void InitiateTextTyping()
    {
        if (!typing && textComp.text.Equals("") && gameObject.activeSelf)
        {
            Player.player.Immobilize();
            typing = true;
            stopTyping = false;
            StartCoroutine(TypeText());
        }
    }

    public void InitiateTextTyping(string newMessage)
    {
        message = newMessage;
        textComp.text = "";
        InitiateTextTyping();
    }


    public IEnumerator TypeText()
    {
        audioSource.Play();
        foreach (char letter in message.ToCharArray())
        {
            if (!typing) break;
            textComp.text += letter;
            if (char.IsWhiteSpace(letter))
            {
                continue;
            }
            else
            {
                yield return new WaitForSeconds(letterPause);
            }
        }
        audioSource.Stop();
        typing = false;
        if (RupeeLossAtEnd != 0) Player.player.AddRupees(-RupeeLossAtEnd);
        Player.player.Mobilize();
        if (stopTyping)
        {
            textComp.text = "";
        }
    }

    public void StopText()
    {
        stopTyping = true;
        typing = false;
        textComp.text = "";
        audioSource.Stop();
    }
}