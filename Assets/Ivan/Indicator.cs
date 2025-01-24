using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace GameUI 
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class Indicator : MonoBehaviour
    {
        private void OnEnable()
        {
            if (text == null)
                text = GetComponent<TextMeshProUGUI>();
        }
        [SerializeField]
        TextMeshProUGUI text;

        [ExecuteInEditMode]
        private void OnValidate()
        {
            if (text == null)
                text = GetComponent<TextMeshProUGUI>();
        }
        public void ShowValue (int value)
        {
            if (text)
                text.text = value.ToString();
            else
                Debug.Log("LOL");
        }
    }
}
