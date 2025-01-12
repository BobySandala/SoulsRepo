using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


namespace DS_ripoff
{
    public class PlayerSelectController : MonoBehaviour
    {
        public static PlayerSelectController instance;
        public GameObject[] prefabs;
        public GameObject selectGUI;

        private Vector3 position = new Vector3(1.5f, 0, -7.2f);
        private Quaternion rotation = Quaternion.Euler(0, 180, 0);

        private int index;
        private List<GameObject> prefabInstances = new List<GameObject>();

        private readonly int gameSceneIndex = 2;
        public void Awake()
        {

            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            for (int i = 0; i < prefabs.Length; i++)
            {
                GameObject instance = Instantiate(prefabs[i], position, rotation);
                instance.SetActive(false);
                prefabInstances.Add(instance);
            }

            if (prefabInstances.Count > 0)
            {
                prefabInstances[0].SetActive(true);
            }

            //Debug.Log("Initialization complete");
        }

        private void setActiveFalse()
        {
            foreach (var instance in prefabInstances)
            {
                instance.SetActive(false);
            }
        }

        private void ChangeGUIText()
        {
            if (selectGUI != null)
            {
                Transform editGUI = selectGUI.transform.Find("EDIT_TEXT");
                if (editGUI != null)
                {
                    if (editGUI.Find("CLASS").TryGetComponent<TextMeshProUGUI>(out var textbox_CLASS))
                    {
                        textbox_CLASS.text = "Cox";
                    }
                    else
                    {
                        //Debug.LogWarning("CLASS does not have TextMeshProUGUI or is not found.");
                    }
                    if (editGUI.Find("CLASS").TryGetComponent<TextMeshProUGUI>(out var textbox_HP))
                    {
                        textbox_HP.text = "New Text for Textbox1";
                    }
                    else
                    {
                        //Debug.LogWarning("CLASS does not have TextMeshProUGUI or is not found.");
                    }
                    if (editGUI.Find("CLASS").TryGetComponent<TextMeshProUGUI>(out var textbox_SP))
                    {
                        textbox_SP.text = "New Text for Textbox1";
                    }
                    else
                    {
                        //Debug.LogWarning("CLASS does not have TextMeshProUGUI or is not found.");
                    }
                    if (editGUI.Find("CLASS").TryGetComponent<TextMeshProUGUI>(out var textbox_DMG))
                    {
                        textbox_DMG.text = "New Text for Textbox1";
                    }
                    else
                    {
                        //Debug.LogWarning("CLASS does not have TextMeshProUGUI or is not found.");
                    }
                }
                else
                {
                    //Debug.Log("Nu gaseste EDIT GUI");
                }
            }
            else
            {
                //Debug.Log("Nu gaseste GUI");
            }
        }

        private void LoadGameScene()
        {
            SceneManager.LoadScene(gameSceneIndex);
        }
        private void ChangeShownCharacter()
        {
            //Debug.Log("Switching to prefab index: " + index);
            setActiveFalse();
            prefabInstances[index].SetActive(true);
            ChangeGUIText();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                index--;
                if (index < 0)
                {
                    index = prefabInstances.Count - 1;
                }
                ChangeShownCharacter();

            } 
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                index = (index + 1) % prefabInstances.Count;
                ChangeShownCharacter();
            } 
            else if(Input.GetKeyDown(KeyCode.Return))
            {
                LoadGameScene();
            }
        }
    }
}