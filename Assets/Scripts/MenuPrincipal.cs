using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuPrincipal : MonoBehaviour
{
    public GameObject instrucciones;
    public void Jugar() { Time.timeScale = 1; SceneManager.LoadScene("Juego"); }
    public void Ayuda() { instrucciones.SetActive(!instrucciones.activeSelf); }
}
