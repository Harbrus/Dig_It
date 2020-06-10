using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] int jewelDeposited = 0;
    [SerializeField] float interactableDistance = 1f;
    [SerializeField] GameObject playerPrefab;

    public GameObject interactPrompt;
    public GameObject jewelDepositedText;

    bool canComplete = false;

    // Update is called once per frame
    void Update()
    {
        if (!canComplete && Vector3.Distance(playerPrefab.transform.position, transform.position) <= interactableDistance)
        {
            interactPrompt.SetActive(true);
            // if pad is used (check in the game/inputmanager) overwrite what's written

            if (GameManager.Instance.Points > 0 && Input.GetButtonDown("Interact"))
            {
                DepositJewel();
            }
        }
        else
        {
            interactPrompt.SetActive(false);
        }

        if (!canComplete && jewelDeposited >= LevelManager.Instance.jewelToWin)
        {
            GetComponent<Collider2D>().isTrigger = true;
            GetComponent<SpriteRenderer>().enabled = false;
            // feedback

            canComplete = true;
            interactPrompt.SetActive(false);
            jewelDepositedText.SetActive(false);
        }
    }

    public void DepositJewel()
    {
        jewelDeposited++;
        // Show number of jewel - visual feedback jewel charging the door.
        jewelDepositedText.GetComponent<TextMeshProUGUI>().text = jewelDeposited + " Jewels Deposited";
        GameManager.Instance.LosePoint(1, false); // do not flash
        LevelManager.Instance.currentJewelAvailable--;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(canComplete && collision.CompareTag("Player"))
        {
            // load next level
        }
    }

}
