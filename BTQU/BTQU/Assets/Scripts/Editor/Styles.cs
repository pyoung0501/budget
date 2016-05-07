using UnityEngine;

namespace BTQ
{
    /// <summary>
    /// Common library of styles used in the BTQ editor.
    /// </summary>
    public static class Styles
    {
        /// <summary>
        /// Style for drawing a right aligned text field.
        /// </summary>
        public static GUIStyle RightAlignedTextField
        {
            get
            {
                if (_rightAlignedTextField == null)
                {
                    _rightAlignedTextField = new GUIStyle(GUI.skin.textField);
                    _rightAlignedTextField.alignment = TextAnchor.MiddleRight;
                }

                return _rightAlignedTextField;
            }
        }

        /// <summary>
        /// Style for drawing a center justified label.
        /// </summary>
        public static GUIStyle CenterJustifiedLabel
        {
            get
            {
                if (_centerJustifiedLabel == null)
                {
                    _centerJustifiedLabel = new GUIStyle(GUI.skin.label);
                    _centerJustifiedLabel.alignment = TextAnchor.MiddleCenter;
                }

                return _centerJustifiedLabel;
            }
        }

        private static GUIStyle _rightAlignedTextField;
        private static GUIStyle _centerJustifiedLabel;
    }
}
