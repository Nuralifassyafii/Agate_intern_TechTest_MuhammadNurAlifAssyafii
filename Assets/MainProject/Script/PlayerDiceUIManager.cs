using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDiceUIManager : MonoBehaviour
{
    private GameObject diceBar;
    private List<GameObject> listCanvasDice = new List<GameObject>();
    public bool isShow = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        diceBar = GameObject.Find("DiceBarUtama");
        listCanvasDice.Add(GameObject.Find("CanvasSkillDice"));
        listCanvasDice.Add(GameObject.Find("CanvasConditionDice"));
        listCanvasDice.Add(GameObject.Find("CanvasSpecialDice"));
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool IsDiceBarActive()
    {
        return diceBar.activeSelf;
    }

    public void CloseMenuDice()
    {
        diceBar.SetActive(!IsDiceBarActive());
        Debug.Log(IsDiceBarActive());
        ShowMenuDice("back");
    }

    public void ShowMenuDice(string canvasName)
    {
        
        for(int i = 0;i< listCanvasDice.Count; i++)
        {
            if (listCanvasDice[i].name == canvasName){
                listCanvasDice[i].SetActive(true);
                diceBar.SetActive(true);
            }
            else
            {
                listCanvasDice[i].SetActive(false);
            }
        }
    }

}
