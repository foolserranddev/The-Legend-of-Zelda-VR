using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Registration : Menu
{
    public GameObject Selector;
    public GameObject NameIndexer;
    public GameObject CancelSelector;
    public GameObject RegisterSelector;
    public Text Name;
    public char[,] chars = new char[4, 11] {
        { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K'},
        { 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V' },
        { 'W', 'X', 'Y', 'Z', '-', '.', ',', '!', '\'', '&', '.' },
        { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ' ' } };
    public float indexMultiplierRow = -0.12f;
    public float indexMultiplierCol = -0.12f;
    public float indexMultiplierName = -0.06f;
    private int rowIndex = 0;
    private int colIndex = 0;
    private int nameIndex = 0;
    private int MaxTextLength = 10;

    public override void OnPadClicked(ClickedEventArgs e)
    {
        FourCornersPadButton dir = GetOneOfFourPadButtons(e);
        switch(dir)
        {
            case FourCornersPadButton.Right:
                if (RegisterSelector.activeSelf)
                {
                    return;
                }
                else if (CancelSelector.activeSelf)
                {
                    RegisterSelector.SetActive(true);
                    CancelSelector.SetActive(false);
                    return;
                }
                colIndex += 1;
                if (colIndex >= chars.GetLength(1))
                {
                    colIndex = 0;
                    rowIndex += 1;
                    if (rowIndex > chars.GetLength(0))
                    {
                        rowIndex = 0;
                    }
                }
                break;
            case FourCornersPadButton.Left:
                if (RegisterSelector.activeSelf)
                {
                    RegisterSelector.SetActive(false);
                    CancelSelector.SetActive(true);
                    return;
                }
                else if (CancelSelector.activeSelf)
                {
                    return;
                }
                colIndex -= 1;
                if (colIndex < 0)
                {
                    colIndex = chars.GetLength(1) - 1;
                    rowIndex -= 1;
                    if (rowIndex < 0)
                    {
                        rowIndex = chars.GetLength(0) - 1;
                    }
                }
                break;

            case FourCornersPadButton.Up:
                if (RegisterSelector.activeSelf || CancelSelector.activeSelf)
                {
                    RegisterSelector.SetActive(false);
                    CancelSelector.SetActive(false);
                    Selector.SetActive(true);
                    return;
                }
                rowIndex -= 1;
                if (rowIndex < 0)
                {
                    rowIndex = chars.GetLength(0) - 1;
                }
                break;

            case FourCornersPadButton.Down:
                if (rowIndex == chars.GetLength(0) - 1)
                {
                    if (colIndex < chars.GetLength(1) / 2)
                    {
                        CancelSelector.SetActive(true);
                    }
                    else
                    {
                        RegisterSelector.SetActive(true);
                    }
                    Selector.SetActive(false);
                    return;
                }
                rowIndex += 1;
                break;
        }
        Selector.transform.localPosition = new Vector3(colIndex * indexMultiplierCol, rowIndex * indexMultiplierRow, 0);
        base.OnPadClicked(e);
    }

    public void Reset()
    {
        rowIndex = 0;
        colIndex = 0;
        nameIndex = 0;
        Name.text = "LINK      ";
        Selector.SetActive(true);
        CancelSelector.SetActive(false);
        RegisterSelector.SetActive(false);
        Selector.transform.localPosition = Vector3.zero;
        NameIndexer.transform.localPosition = Vector3.zero;
        gameObject.SetActive(false);
    }

    public override void OnMakeSelection()
    {
        if (RegisterSelector.activeSelf)
        {
            GetComponentInParent<SavePillar>().Register(Name.text);
            Reset();
        }
        else if (CancelSelector.activeSelf)
        {
            GetComponentInParent<SavePillar>().CancelRegistration();
            Reset();
        }
        else
        {
            string name = Name.text;
            StringBuilder sb = new StringBuilder(name);
            sb[nameIndex] = chars[rowIndex, colIndex];
            Name.text = sb.ToString();

            nameIndex += 1;
            if (nameIndex >= MaxTextLength)
            {
                nameIndex = 0;
            }
            NameIndexer.transform.localPosition = new Vector3(nameIndex * indexMultiplierName, 0, 0);
        }
    }
}
