using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 6.0f; 
    public GameObject goal; 
    private Rigidbody rb; 
    public GameObject panelWin;
    public TextMeshProUGUI coinText;
    public int coinsCollected = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); 
        panelWin.SetActive(false); 
    }

    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.velocity = movement * speed;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            coinsCollected++;
            UpdateCoinText();
            Destroy(other.gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Goal"))
        {
            if (coinsCollected >= 10)
            {
                panelWin.SetActive(true);  
                Time.timeScale = 0; 
            }
            else
            {
                Debug.Log("you dont have enough coins");
            }
        }
    }

    void UpdateCoinText()
    {
        coinText.text = "COINS COLLECTED: " + coinsCollected;
    }
}
