using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//http://answers.unity3d.com/questions/794503/how-can-i-use-unity-46-gui-to-have-a-health-bar-th.html
public class HealthBar : MonoBehaviour
{

    public RectTransform canvasRectT;
    public RectTransform healthBar;
    public Transform objectToFollow;
    public Color fullLP;
    public Color halfLP;
    public Color nearlyDead;

    private Image image;

    void Awake()
    {
        image = this.GetComponent<Image>();
    }

    void Update()
    {
        if (objectToFollow != null)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, objectToFollow.position + new Vector3(0, 4f));

            healthBar.anchoredPosition = screenPoint - canvasRectT.sizeDelta / 2f;
            image.fillAmount = objectToFollow.GetComponent<ILivingEntity>().CurrentLP / objectToFollow.GetComponent<ILivingEntity>().MaxLP;
            if (image.fillAmount > 0.5) { image.color = fullLP; }
            else
            {
                if (image.fillAmount > 0.25)
                {
                    image.color = halfLP;
                }
                else
                {
                    image.color = nearlyDead;
                }
            }
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}
