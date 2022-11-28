using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image healthBarImage;
    private PlayerMovement player = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        LookForLocalPlayerInstance();
        Health();
    }

    void Health()
    {
        if (player == null) return;
        healthBarImage.fillAmount = Mathf.Clamp(player.Health / player.MaxHealth, 0.0f, 1.0f);
    }

    void LookForLocalPlayerInstance()
    {
        if (player != null) return;

        GameObject obj = GameObject.FindGameObjectWithTag("LocalPlayer");
        if (obj != null)
            player = obj.transform.GetComponentInChildren<PlayerMovement>();

    }

}
