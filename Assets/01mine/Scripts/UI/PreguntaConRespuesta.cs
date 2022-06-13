using UnityEngine;
using TMPro;
public class PreguntaConRespuesta : MonoBehaviour
{
    public float yPos { get; set; }
    public float downY { get; set; }

    public TMP_Text pregunta_TMP, respuesta_TMP;
    public void InizialyzeAsk(float downYPos)
    {
        yPos = downYPos;
        respuesta_TMP.gameObject.SetActive(true);
        downY = yPos + pregunta_TMP.rectTransform.sizeDelta.y + respuesta_TMP.rectTransform.sizeDelta.y;

        Vector2 newPsition = new Vector2(GetComponent<RectTransform>().anchoredPosition.x, -yPos);
        GetComponent<RectTransform>().anchoredPosition = newPsition;
        respuesta_TMP.gameObject.SetActive(false);
    }
    public void UpdateAsk(float downYPos)
    {
        yPos = downYPos;
        downY = respuesta_TMP.isActiveAndEnabled ?
            yPos + pregunta_TMP.rectTransform.sizeDelta.y + respuesta_TMP.rectTransform.sizeDelta.y
            : yPos + pregunta_TMP.rectTransform.sizeDelta.y;
        Vector2 newPsition = new Vector2(GetComponent<RectTransform>().anchoredPosition.x, -yPos);
        GetComponent<RectTransform>().anchoredPosition = newPsition;
    }
    public void Responder(bool no)
    {
        respuesta_TMP.gameObject.SetActive(!no);
        GetComponentInParent<ListadoDePreguntas>().UpdatePreguntas();
        //FindObjectOfType<ListadoDePreguntas>().UpdatePreguntas();
    }
}
