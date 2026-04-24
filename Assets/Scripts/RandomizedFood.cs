using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class RandomizedFood : MonoBehaviour
{
    public List<GameObject> fourItems = new List<GameObject>();
    public List<GameObject> threeItems = new List<GameObject>();
    public List<GameObject> twoItems = new List<GameObject>();
    public List<GameObject> oneItems = new List<GameObject>();
    /*public GameObject[] fourItems;
    public GameObject[] threeItems;
    public GameObject[] twoItems;
    public GameObject[] oneItems;*/


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
        
        Debug.Log("mes" + r);

        if (r == 0)
        {
            randomize();
            fourItems[fourrandom].SetActive(true);
            twoItems[tworandom].SetActive(true);
            threeItems[threerandom].SetActive(true);
        }
        if (r == 1)
        {
            randomize();
            fourItems[fourrandom].SetActive(true);
            twoItems[tworandom].SetActive(true);
            oneItems[onerandom].SetActive(true);
            remove();
            randomize();
            twoItems[tworandom].SetActive(true);

        }
        if (r == 2)
        {
            randomize();
            threeItems[threerandom].SetActive(true);
            twoItems[tworandom].SetActive(true);
            oneItems[onerandom].SetActive(true);
            remove();
            randomize();
            twoItems[tworandom].SetActive(true);
            oneItems[onerandom].SetActive(true);

        }
        if (r == 3)
        {
            randomize();
            fourItems[fourrandom].SetActive(true);
            twoItems[tworandom].SetActive(true);
            threeItems[threerandom].SetActive(true);

        }
    }

    public void randomize()
    {
        fourrandom = Random.Range(0, fourItems.Count);
        threerandom = Random.Range(0, threeItems.Count);
        tworandom = Random.Range(0, twoItems.Count);
        onerandom = Random.Range(0, oneItems.Count);
        
    }

    private void remove()
    {
        fourItems.RemoveAt(fourrandom);
        threeItems.RemoveAt(threerandom);
        twoItems.RemoveAt(tworandom);
        oneItems.RemoveAt(onerandom);
    }
}
