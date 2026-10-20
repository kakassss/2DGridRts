using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Rts2D.ScreenSettings.Manager;

namespace Rts2D.ScreenSettings.View
{
    /// <summary>
    /// Put this on a scene button. Each click switches to the next resolution preset
    /// and the label shows the current one.
    /// </summary>
    public class ResolutionButtonView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text label;

        private ResolutionManager _resolution;

        [Inject]
        private void Construct(ResolutionManager resolution) => _resolution = resolution;

        private void Start()
        {
            label.text = _resolution.CurrentLabel;
            button.onClick.AddListener(_resolution.ApplyNextPreset);
            _resolution.LabelChanged += SetLabel;
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(_resolution.ApplyNextPreset);
            _resolution.LabelChanged -= SetLabel;
        }

        private void SetLabel(string text) => label.text = text;
    }
}
