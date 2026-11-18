using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class VerificarHelix
{
    static int fase;
    static double inicio;
    static float menorY, mayorY;
    static bool reboto;
    static VerificarHelix() {
        EditorApplication.playModeStateChanged += estado => {
            if(estado==PlayModeStateChange.EnteredEditMode && SessionState.GetBool("HelixVolverMenu",false)) {
                SessionState.SetBool("HelixVolverMenu",false);
                EditorSceneManager.OpenScene("Assets/Scenes/Menu.unity");
            }
        };
        EditorApplication.update += Actualizar;
    }
    [MenuItem("Helix/Verificar funcionamiento")]
    public static void Preparar() {
        if(EditorApplication.isPlayingOrWillChangePlaymode) return;
        EditorSceneManager.OpenScene("Assets/Scenes/Juego.unity");
        SessionState.SetBool("HelixVerificar",true);
        EditorApplication.isPlaying=true;
    }
    static void Actualizar() {
        if(!SessionState.GetBool("HelixVerificar",false) || !EditorApplication.isPlaying || EditorApplication.isCompiling) return;
        var p=Object.FindAnyObjectByType<Partida>();
        if(p==null) return;
        if(fase==0) { inicio=EditorApplication.timeSinceStartup; menorY=100; mayorY=-100; fase=1; }
        if(fase==1) {
            menorY=Mathf.Min(menorY,p.pelota.position.y); mayorY=Mathf.Max(mayorY,p.pelota.position.y);
            if(p.pelota.linearVelocity.y>5) reboto=true;
            if(EditorApplication.timeSinceStartup-inicio<4) return;
            Debug.Log("Rebote fisico: "+reboto+"; rango Y: "+menorY+" a "+mayorY+"\n");
            p.Pausar();
            Debug.Log("Pausa: "+(!p.Jugando && Time.timeScale==0 && p.panelPausa.activeSelf)+"\n");
            p.Pausar();
            var peligro=System.Array.Find(Object.FindObjectsByType<Superficie>(),s=>s.peligrosa);
            Vector3 posicion=Quaternion.Euler(0,peligro.transform.eulerAngles.y,0)*new Vector3(1.65f,0,1.65f);
            posicion.y=peligro.transform.position.y+1;
            p.pelota.constraints=RigidbodyConstraints.FreezeRotation;
            p.pelota.position=posicion; p.pelota.linearVelocity=Vector3.down*2;
            inicio=EditorApplication.timeSinceStartup; fase=2;
        } else if(fase==2 && EditorApplication.timeSinceStartup-inicio>1.5) {
            Debug.Log("Colision roja/derrota: "+(!p.Jugando && p.panelResultado.activeSelf)+"\n");
            p.Reiniciar(); fase=3; inicio=EditorApplication.timeSinceStartup;
        } else if(fase==3 && EditorApplication.timeSinceStartup-inicio>1) {
            p.pelota.position=new Vector3(0.75f,-36.9f,-2.3f); p.pelota.linearVelocity=Vector3.down*2;
            inicio=EditorApplication.timeSinceStartup; fase=4;
        } else if(fase==4 && EditorApplication.timeSinceStartup-inicio>1.5) {
            Debug.Log("Meta/victoria: "+(!p.Jugando && p.textoResultado.text.Contains("COMPLETADA"))+"\n");
            p.Menu(); fase=5;
        }
    }
    [InitializeOnLoadMethod] static void Escuchar() {
        EditorApplication.update += () => {
            if(fase==5 && SceneManager.GetActiveScene().name=="Menu") {
                Debug.Log("Regreso a Menu: True\n");
                SessionState.SetBool("HelixVerificar",false); fase=6;
                SessionState.SetBool("HelixVolverMenu",true); EditorApplication.isPlaying=false;
            }
        };
    }
}
