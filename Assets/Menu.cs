using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public AudioClip selectorMoveSound;
    public AudioClip selectorSelectSound;

    private AudioSource audioSource;
    public enum FourCornersPadButton
    {
        Up,
        Down,
        Left,
        Right,
        Null
    }


    // Use this for initialization
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public virtual void OnPadClicked(ClickedEventArgs e)
    {
        audioSource.clip = selectorMoveSound;
        audioSource.Play();
    }

    public virtual void OnMakeSelection()
    {
        audioSource.clip = selectorSelectSound;
        audioSource.Play();
    }

    public FourCornersPadButton GetOneOfFourPadButtons(ClickedEventArgs e)
    {
        if (e.padX > Mathf.Abs(e.padY)) // Right
        {
            return FourCornersPadButton.Right;
        }
        else if (e.padX < -Mathf.Abs(e.padY)) // Left
        {
            return FourCornersPadButton.Left;
        }
        else if (e.padY > Mathf.Abs(e.padX)) // Top
        {
            return FourCornersPadButton.Up;
        }
        else if (e.padY < -Mathf.Abs(e.padX)) // Bottom
        {
            return FourCornersPadButton.Down;
        }
        else
        {
            return FourCornersPadButton.Null;
        }
    }
}
