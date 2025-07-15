namespace SAGE.Framework.Core.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    
    [AddComponentMenu("UI/Effects/Vertical Gradient")]
    public class VerticalGradient : BaseMeshEffect
    {
        public Color topColor = Color.white;
        public Color bottomColor = Color.white;
        
        public override void ModifyMesh(VertexHelper vertexHelper)
        {
            if (enabled)
            {
                UIVertex vertex = default;

                float[] t = new float[4] { 0f, 1f, 1f, 0f };

                for (int i = 0; i < vertexHelper.currentVertCount; i++)
                {
                    vertexHelper.PopulateUIVertex(ref vertex, i);

                    vertex.color *= Color.Lerp(bottomColor, topColor, t[i]);

                    vertexHelper.SetUIVertex(vertex, i);
                }
            }
        }
    }
}