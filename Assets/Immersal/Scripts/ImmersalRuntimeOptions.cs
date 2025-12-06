using Immersal.XR;
using TMPro;
using UnityEngine;

public class ImmersalRuntimeOptions : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pointCloudVisualizerText;
    
    private bool areVisualizersActive = true;

    void Start()
    {
        ToggleVisualizations(); // set them to false by default
    }

    public void ToggleVisualizations()
    {
        areVisualizersActive = !areVisualizersActive;
        var visualizers = FindObjectsOfType<XRMapVisualization>();
        foreach (var visualizer in visualizers)
        {
            visualizer.renderMode = !areVisualizersActive ? XRMapVisualization.RenderMode.DoNotRender : 
                XRMapVisualization.RenderMode.EditorAndRuntime;

            pointCloudVisualizerText.text = !areVisualizersActive ? "Visualizers Off" : "Visualizers On";
        }
    }
}
