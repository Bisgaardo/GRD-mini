using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomizedFood : MonoBehaviour
{
    public List<GameObject> fourItems = new List<GameObject>();
    public List<GameObject> threeItems = new List<GameObject>();
    public List<GameObject> twoItems = new List<GameObject>();
    public List<GameObject> oneItems = new List<GameObject>();
  
    public Playermovement playermovement;
    public int NumberedFood;
    

    public int fourrandom;
    public int threerandom;
    public int tworandom;
    public int onerandom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        foreach (var t in fourItems)
        {
            t.SetActive(false);
        }
        foreach (var t in threeItems)
        {
            t.SetActive(false);
        }
        foreach (var t in twoItems)
        {
            t.SetActive(false);
        }
        foreach (var t in oneItems)
        {
            t.SetActive(false);
        }

        int r = Random.Range(0, 4);
        
        if (r == 0)
        {
            randomize();
            fourItems[fourrandom].SetActive(true);
            threeItems[threerandom].SetActive(true);
            oneItems[onerandom].SetActive(true);
            remove(fourItems, threeItems, oneItems);
            randomize();
            oneItems[onerandom].SetActive(true);
            NumberedFood=4;
            remove(oneItems);

        }
        if (r == 1)
        {
            randomize();
            twoItems[tworandom].SetActive(true);
            oneItems[onerandom].SetActive(true);
            threeItems[threerandom].SetActive(true);
            remove(twoItems, oneItems, threeItems);
            randomize();
            twoItems[tworandom].SetActive(true);
            oneItems[onerandom].SetActive(true);

            NumberedFood=5;
            remove(twoItems, oneItems);


        }
        if (r == 2)
        {
            randomize();
            threeItems[threerandom].SetActive(true);
            twoItems[tworandom].SetActive(true);
            oneItems[onerandom].SetActive(true);
            remove(threeItems, twoItems, oneItems);
            randomize();
            twoItems[tworandom].SetActive(true);
            oneItems[onerandom].SetActive(true);
            NumberedFood=5;
            remove(twoItems, oneItems);

        }
        if (r == 3)
        {
            randomize();
            fourItems[fourrandom].SetActive(true);
            twoItems[tworandom].SetActive(true);
            oneItems[onerandom].SetActive(true);
            remove(fourItems,twoItems,oneItems);
            randomize();
            twoItems[tworandom].SetActive(true);
            NumberedFood=4;
            remove(twoItems);


        }

        playermovement.foodAmount = NumberedFood;
    }

    public void randomize()
    {
        fourrandom = Random.Range(0, fourItems.Count);
        threerandom = Random.Range(0, threeItems.Count);
        tworandom = Random.Range(0, twoItems.Count);
        onerandom = Random.Range(0, oneItems.Count);
    }

    private void remove(params List<GameObject>[] lists)
    {
        foreach (var list in lists)
        {
            if (list == fourItems)  list.RemoveAt(fourrandom);
            else if (list == threeItems) list.RemoveAt(threerandom);
            else if (list == twoItems)   list.RemoveAt(tworandom);
            else if (list == oneItems)   list.RemoveAt(onerandom);
        }
    }

    public void newFood()
    {
        if (twoItems != null)
        {
            randomize();
            twoItems[tworandom].SetActive(true);
            remove(twoItems);
            playermovement.foodAmount++;
        }
    }
        
}
