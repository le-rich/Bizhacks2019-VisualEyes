using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashCarder : MonoBehaviour
{

    public GameObject cart;
    public GameObject wishlist;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowCart()
    {
        cart.SetActive(true);
        //cart.GetComponent<Image>().CrossFadeAlpha(0f, 2f, true);
        //foreach (Image i in cart.GetComponentsInChildren<Image>())
        //{
        //    i.CrossFadeAlpha(0f, 2f, true);
        //}
        Invoke("SetCartInactive", 1.5f);
    }

    public void ShowWishlist() {
        wishlist.SetActive(true);
        //wishlist.GetComponent<Image>().CrossFadeAlpha(0f, 2f, true);
        //foreach (Image i in wishlist.GetComponentsInChildren<Image>())
        //{
        //    i.CrossFadeAlpha(0f, 2f, true);
        //}
        Invoke("SetWishlistInactive", 1.5f);
    }


    void SetCartInactive()
    {
        //cart.GetComponent<Image>().CrossFadeAlpha(0f, 2f, true);
        //foreach(Image i in cart.GetComponentsInChildren<Image>())
        //{
        //    i.CrossFadeAlpha(0f, 2f, true);
        //}
        cart.SetActive(false);
    }

    void SetWishlistInactive()
    {
        //wishlist.GetComponent<Image>().CrossFadeAlpha(0f, 2f, true);
        //foreach (Image i in wishlist.GetComponentsInChildren<Image>())
        //{
        //    i.CrossFadeAlpha(0f, 2f, true);
        //}
        wishlist.SetActive(false);
    }
}
