using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string targetSceneName;

private void OnTriggerStay2D(Collider2D other)
{
    if (!other.CompareTag("Player")) return;
    if (Input.GetButtonDown("Interact"))
     {
        GameManager.Instance.LoadScene(targetSceneName);
        Debug.Log("transition");
     }
    }

}
