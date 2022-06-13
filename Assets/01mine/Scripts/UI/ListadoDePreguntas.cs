using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListadoDePreguntas : MonoBehaviour
{
    [SerializeField]
    GameObject askPrefab;
    [SerializeField]
    RectTransform parent;
    public List<GameObject> preguuntasUI;
    public List<Pregunta> preguntasInput;
    public RectTransform rTransform {  get; private set; }
    private void OnEnable()
    {
        InitializeAskList();
    }
    public void InitializeAskList()
    {
        if (preguuntasUI != null)
        {
            foreach (GameObject item in preguuntasUI)
            {
                Destroy(item);
            }
            preguuntasUI.Clear();
        }
        else
            preguuntasUI = new List<GameObject>();
        rTransform = GetComponent<RectTransform>();
        if (preguntasInput != null && preguntasInput.Count > 0)
        {
            foreach (Pregunta p in preguntasInput)
            {
                GameObject toadd = Instantiate(askPrefab, transform);
                toadd.GetComponent<PreguntaConRespuesta>().pregunta_TMP.text = p.pregunta_m;
                toadd.GetComponent<PreguntaConRespuesta>().respuesta_TMP.text = p.respuesta_m;
                preguuntasUI.Add(toadd);
            }
        }
        StartCoroutine(WaitToDo());

    }
    public void UpdatePreguntas()
    {
        if (preguuntasUI != null && preguuntasUI.Count > 0)
        {
            for (int k = 0; k < preguuntasUI.Count; k++)
            {
                if(k > 0)
                    preguuntasUI[k].GetComponent<PreguntaConRespuesta>().UpdateAsk(preguuntasUI[k - 1].GetComponent<PreguntaConRespuesta>().downY);
                else
                    preguuntasUI[k].GetComponent<PreguntaConRespuesta>().UpdateAsk(0);
            }
        }
        Vector2 newSizeDelta = new Vector2(rTransform.sizeDelta.x, preguuntasUI[preguuntasUI.Count - 1].GetComponent<PreguntaConRespuesta>().downY);
        rTransform.sizeDelta = newSizeDelta;
        if(parent)
        {
            parent.sizeDelta = new Vector2(parent.sizeDelta.x, -rTransform.anchoredPosition.y + rTransform.sizeDelta.y);
        }
    }
    IEnumerator WaitToDo()
    {
        yield return new WaitForEndOfFrame();
        if (preguuntasUI != null && preguuntasUI.Count > 0)
        {
            for (int k = 0; k < preguuntasUI.Count; k++)
            {
                if (k == 0)
                    preguuntasUI[k].GetComponent<PreguntaConRespuesta>().InizialyzeAsk(0);
                else
                    preguuntasUI[k].GetComponent<PreguntaConRespuesta>().InizialyzeAsk(preguuntasUI[k - 1].GetComponent<PreguntaConRespuesta>().downY);
            }
        }
        UpdatePreguntas();
    }
}
[System.Serializable]
public class Pregunta
{
    public string pregunta_m, respuesta_m;
    public Pregunta(string pregunta, string respuesta)
    {
        pregunta_m = pregunta;
        respuesta_m = respuesta;
    }
}
