using System.Collections;
using System.Collections.Generic;
using nsSettings;
using UnityEngine;

namespace nsSettings
{
    public class GraphicsSettings : ISettingsType<GraphicsSettings>
    {
        public float cameraZoom;


        public GraphicsSettings Default
        {
            get
            {
                return new GraphicsSettings
                {
                    cameraZoom = 1f,
                };
            }
        }
    }
}

