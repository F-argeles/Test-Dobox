using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextHandler : MonoBehaviour
{
    private TextMeshProUGUI m_TextMesh;

    void Awake()
    {
        m_TextMesh = GetComponent<TextMeshProUGUI>();
    }

    public void SetText(string text)
    {
        m_TextMesh = GetComponent<TextMeshProUGUI>();

        if (m_TextMesh != null)
        {
            m_TextMesh.text = text;
        }
    }

}
