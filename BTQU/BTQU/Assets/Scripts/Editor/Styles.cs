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

        public static GUIStyle RightAlignedLabel
        {
            get
            {
                if(_rightAlignedLabel == null)
                {
                    _rightAlignedLabel = new GUIStyle(GUI.skin.label);
                    _rightAlignedLabel.alignment = TextAnchor.MiddleRight;
                }

                return _rightAlignedLabel;
            }
        }

        public static GUIStyle RightAlignedBoldLabel
        {
            get
            {
                if(_rightAlignedBoldLabel == null)
                {
                    _rightAlignedBoldLabel = new GUIStyle(GUI.skin.label);
                    _rightAlignedBoldLabel.alignment = TextAnchor.MiddleRight;
                    _rightAlignedBoldLabel.fontStyle = FontStyle.Bold;
                }

                return _rightAlignedBoldLabel;
            }
        }

        public static GUIStyle RightAlignedBoldWrappedLabel
        {
            get
            {
                if (_rightAlignedBoldWrappedLabel == null)
                {
                    _rightAlignedBoldWrappedLabel = new GUIStyle(GUI.skin.label);
                    _rightAlignedBoldWrappedLabel.alignment = TextAnchor.MiddleRight;
                    _rightAlignedBoldWrappedLabel.fontStyle = FontStyle.Bold;
                    _rightAlignedBoldWrappedLabel.wordWrap = true;
                }

                return _rightAlignedBoldWrappedLabel;
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
        private static GUIStyle _rightAlignedBoldLabel;
        private static GUIStyle _rightAlignedLabel;
        private static GUIStyle _rightAlignedBoldWrappedLabel;
    }
}
