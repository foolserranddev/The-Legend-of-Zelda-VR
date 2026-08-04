using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveMenu : Menu
{
    public GameObject[] SelectorHearts;
    public float startDistance = 3;

    private int index = 0;

    private enum options
    {
        SaveAndContinue,
        Reload,
        ContinueWithoutSaving,
        QuitGame
    }

	// Use this for initialization
	public void OnEnable ()
    {
        index = 0;
        for (int i = 0; i < SelectorHearts.Length; i++)
        {
            SelectorHearts[i].SetActive(index == i);
        }

        Vector3 fwd = Camera.main.transform.forward;
        fwd.y = 0;
        transform.position = Camera.main.transform.position + (fwd.normalized * startDistance);

    }

    public override void OnPadClicked(ClickedEventArgs e)
    {
        FourCornersPadButton dir = GetOneOfFourPadButtons(e);
        switch (dir)
        {
            case FourCornersPadButton.Right:
                return;
            case FourCornersPadButton.Left:
                return;
            case FourCornersPadButton.Up:
                index--;
                if (index < 0) index = SelectorHearts.Length - 1;
                break;
            case FourCornersPadButton.Down:
                index++;
                if (index >= SelectorHearts.Length) index = 0;
                break;
        }
        for (int i = 0; i < SelectorHearts.Length; i++)
        {
            SelectorHearts[i].SetActive(index == i);
        }
        base.OnPadClicked(e);
    }

    public override void OnMakeSelection()
    {
        switch((options)index)
        {
            case options.QuitGame:
                gameObject.SetActive(false);
                SaveData.saveData.StartOver();
                SceneManager.LoadScene(0);
                break;
            case options.ContinueWithoutSaving:
                gameObject.SetActive(false);
                break;
            case options.SaveAndContinue:
                SaveData.saveData.Save();
                gameObject.SetActive(false);
                break;
            case options.Reload:
                gameObject.SetActive(false);
                SaveData.saveData.Reload();
                break;
        }

    }
}
