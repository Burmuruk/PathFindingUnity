using SotomaYorch.Game;
using UnityEngine;

public class ScrCoin : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(20.0f * Vector3.up * Time.deltaTime);
        transform.position += Vector3.up * 0.001f * Mathf.Sin(Time.time);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Agent"))
        {
            var referee = FindObjectOfType<ScrGameReferee>();
            if (referee)
                referee.AddScore(10);

            //gameObject.SetActive(false);
        }
    }
}
