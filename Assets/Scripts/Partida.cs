using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Partida : MonoBehaviour
{
    public Rigidbody pelota;
    public GameObject panelPausa, panelResultado;
    public Text textoPuntos, textoResultado, textoAyuda;
    public Slider progreso;
    public AudioClip rebote, victoria, derrota;
    public float separacion = 3.2f;
    public int pisos = 12;
    public bool Jugando { get; private set; }
    AudioSource sonido;
    int superados;
    bool terminada;
    void Start()
    {
        Time.timeScale = 1;
        Application.targetFrameRate = 60;
        sonido = GetComponent<AudioSource>();
        Jugando = true;
        panelPausa.SetActive(false);
        panelResultado.SetActive(false);
    }
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame && !terminada) Pausar();
        if (!Jugando) return;
        superados = Mathf.Clamp(Mathf.FloorToInt((-pelota.position.y - 0.45f) / separacion) + 1, 0, pisos);
        textoPuntos.text = (superados * 10).ToString("00");
        progreso.value = (float)superados / pisos;
        textoAyuda.gameObject.SetActive(superados == 0);
        if (pelota.position.y < -pisos * separacion - 5) Terminar(false);
    }
    public void SonarRebote() { if (rebote != null) sonido.PlayOneShot(rebote, 0.35f); }
    public void Pausar()
    {
        if (terminada) return;
        Jugando = !Jugando;
        panelPausa.SetActive(!Jugando);
        Time.timeScale = Jugando ? 1 : 0;
    }
    public void Terminar(bool ganada)
    {
        if (terminada) return;
        terminada = true;
        Jugando = false;
        pelota.linearVelocity = Vector3.zero;
        pelota.isKinematic = true;
        if (ganada) { progreso.value = 1; textoPuntos.text = (pisos * 10).ToString(); }
        textoResultado.text = ganada ? "¡TORRE COMPLETADA!" : "¡INTÉNTALO OTRA VEZ!";
        panelResultado.SetActive(true);
        var clip = ganada ? victoria : derrota;
        if (clip != null) sonido.PlayOneShot(clip, 0.6f);
    }
    public void Reiniciar() { Time.timeScale = 1; SceneManager.LoadScene("Juego"); }
    public void Menu() { Time.timeScale = 1; SceneManager.LoadScene("Menu"); }
    void OnApplicationPause(bool pausa) { if (pausa && Jugando) Pausar(); }
    void OnDestroy() { Time.timeScale = 1; }
}
