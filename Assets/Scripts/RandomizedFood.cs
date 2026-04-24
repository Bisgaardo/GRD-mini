using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class RandomizedFood : MonoBehaviour
{
    public GameObject[] sixItems;
    public GameObject[] fourItems;
    public GameObject[] threeItems;
    public GameObject[] twoItems;
    public GameObject[] oneItems;

    public int sixrandom;
    public int fourrandom;
    public int threerandom;
    public int tworandom;
    public int onerandom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        foreach (var t in sixItems)
        {
            t.SetActive(false);
        }
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
        
        Debug.Log("mes" + r);

        if (r == 0)
        {
            randomize();
            sixItems[sixrandom].SetActive(true);
            twoItems[tworandom].SetActive(true);
            oneItems[onerandom].SetActive(true);
            Debug.Log("" +sixrandom + tworandom + onerandom);
        }
        if (r == 1)
        {
            randomize();
            fourItems[fourrandom].SetActive(true);
            twoItems[tworandom].SetActive(true);
            oneItems[onerandom].SetActive(true);
            Debug.Log("" +sixrandom + tworandom + onerandom);

        }
        if (r == 2)
        {
            randomize();
            threeItems[threerandom].SetActive(true);
            twoItems[tworandom].SetActive(true);
            randomize();
            twoItems[tworandom].SetActive(true);
            oneItems[onerandom].SetActive(true);
            Debug.Log("" +threerandom + tworandom + onerandom);

        }
        if (r == 3)
        {
            randomize();
            fourItems[fourrandom].SetActive(true);
            twoItems[tworandom].SetActive(true);
            
            randomize();
            twoItems[tworandom].SetActive(true);

            oneItems[onerandom].SetActive(true);
            Debug.Log("" +fourrandom + tworandom + onerandom);

        }
    }

    public void randomize()
    {
        sixrandom = Random.Range(0, sixItems.Length);
        fourrandom = Random.Range(0, fourItems.Length);
        threerandom = Random.Range(0, threeItems.Length);
        tworandom = Random.Range(0, twoItems.Length);
        onerandom = Random.Range(0, oneItems.Length);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
