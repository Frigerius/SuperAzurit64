using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Compass : MonoBehaviour
{

    public RectTransform canvasRectT;
    public RectTransform symbolRect;
    public RectTransform compassRect;
    public Transform objectToFollow;
    public CompassType type;

    public enum CompassType
    {
        Enemy,
        Flag
    }

    void Awake()
    {

        
        if(type == CompassType.Flag && !GameSettingsController.Instance.CurrentSettings.FlagCompass)
        {
            Destroy(gameObject);
        }
        if(type == CompassType.Enemy&& !GameSettingsController.Instance.CurrentSettings.EnemyCompass)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {

        if (objectToFollow != null)
        {

            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, objectToFollow.position);

            if (screenPoint.x > 0 && screenPoint.y > 0 && screenPoint.x < canvasRectT.sizeDelta.x && screenPoint.y < canvasRectT.sizeDelta.y)
            {
                symbolRect.GetComponent<Image>().enabled = false;
                compassRect.GetComponent<Image>().enabled = false;
            }
            else
            {
                screenPoint -= canvasRectT.sizeDelta / 2f;
                float m = screenPoint.y / screenPoint.x;
                Vector2 pos = new Vector2();
                if (screenPoint.y > 0)
                {
                    pos.x = ((canvasRectT.sizeDelta.y / 2f) * 0.9f) / m;
                    pos.y = (canvasRectT.sizeDelta.y / 2f) * 0.85f;
                }else{
                    pos.x = ((-canvasRectT.sizeDelta.y / 2f) * 0.9f) / m;
                    pos.y = (-canvasRectT.sizeDelta.y / 2f) * 0.85f;
                }
                if (pos.x < -canvasRectT.sizeDelta.x / 2 * 0.9f)
                {
                    pos.x = -canvasRectT.sizeDelta.x / 2 * 0.9f;
                    pos.y = m * -canvasRectT.sizeDelta.x / 2 * 0.85f;
                }
                else if (pos.x > canvasRectT.sizeDelta.x / 2 * 0.9f)
                {
                    pos.x = canvasRectT.sizeDelta.x / 2 * 0.9f;
                    pos.y = m * canvasRectT.sizeDelta.x / 2 * 0.85f;
                }

                symbolRect.anchoredPosition = pos;
                float rotation = Mathf.Atan2(screenPoint.y, screenPoint.x) * Mathf.Rad2Deg;
                compassRect.rotation = Quaternion.Euler(new Vector3(0,0,rotation-45));
                compassRect.GetComponent<Image>().enabled = true;
                symbolRect.GetComponent<Image>().enabled = true;
            }


        }
        else
        {
            Destroy(gameObject);
        }
    }
    [System.Serializable]
    public class HighScoreButtonStrings
    {
        public string name, scoreId;
    }
}
