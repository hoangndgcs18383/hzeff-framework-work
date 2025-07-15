namespace SAGE.Framework.Core.UI
{
    using UnityEngine.UI;
    using UnityEngine;

    [AddComponentMenu("UI/Effects/Horizontal Gradient")]
    public class HorizontalGradient : BaseMeshEffect
    {
        public Color leftColor = Color.white;
        public Color rightColor = Color.white;

        public override void ModifyMesh(VertexHelper vertexHelper)
        {
            if (enabled)
            {
                UIVertex vertex = default;

                float[] t = new float[4] { 0f, 0f, 1f, 1f };

                for (int i = 0; i < vertexHelper.currentVertCount; i++)
                {
                    vertexHelper.PopulateUIVertex(ref vertex, i);

                    vertex.color *= Color.Lerp(leftColor, rightColor, t[i]);

                    vertexHelper.SetUIVertex(vertex, i);
                }
            }
        }
    }
}