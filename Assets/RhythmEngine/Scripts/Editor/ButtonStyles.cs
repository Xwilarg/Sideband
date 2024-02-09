using UnityEngine;

namespace RhythmEngine
{
    public static class ButtonStyles
    {
        public static GUIStyle SelectedButton
        {
            get
            {
                var style = new GUIStyle("AppToolbarButtonRight")
                {
                    normal = { textColor = Color.white },
                    fontStyle = FontStyle.Bold,
                    fontSize = 14
                };

                return style;
            }
        }

        public static GUIStyle UnselectedButton
        {
            get
            {
                var style = new GUIStyle(SelectedButton)
                {
                    normal = { textColor = Color.gray }
                };

                return style;
            }
        }
    }
}
