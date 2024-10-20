using System.Collections.Generic;
using Common;
using Common.Scripts;
using Solutions._3._RF;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Solutions._3_RendererFeatureSideInstancing.Scripts
{
    public class RendererFeature3 : ScriptableRendererFeature
    {
        [SerializeField]
        private Mesh elevatedQuad;
        
        private GrassPass grassPass;

        public override void Create()
        {
            grassPass = new GrassPass(elevatedQuad, RenderPassEvent.AfterRenderingOpaques);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(grassPass);
        }
    }

    public class PassData
    {
        public Mesh ElevatedQuad;

        public Dictionary<GrassRenderer, Matrix4x4[]> GrassMatricesTRS;

        public void Initialize(Mesh elevatedQuad)
        {
            ElevatedQuad = elevatedQuad;
            GrassMatricesTRS = new Dictionary<GrassRenderer, Matrix4x4[]>();
        }
    }

    public class GrassPass : ScriptableRenderPass
    {
        private readonly Mesh elevatedQuad;

        internal GrassPass( Mesh elevatedQuad, RenderPassEvent passEvent)
        {
            this.elevatedQuad = elevatedQuad;
            renderPassEvent = passEvent;
        }
        
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            using IRasterRenderGraphBuilder builder =
                renderGraph.AddRasterRenderPass("GrassPass", out PassData passData);

            passData.Initialize(elevatedQuad);
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            
            passData.GrassMatricesTRS.Clear();

            foreach (GrassRenderer renderer in GrassRenderer.AllGrassRenderers)
            {
                Matrix4x4[] matrices = renderer.GetGrassPositions().AsPositionMatricesArray();
                passData.GrassMatricesTRS.Add(renderer, matrices);
            }
            
            builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
            builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture, AccessFlags.ReadWrite);

            builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
            {
                foreach (GrassRenderer renderer in GrassRenderer.AllGrassRenderers)
                {
                    Matrix4x4[] matrices = data.GrassMatricesTRS[renderer];
                    
                    context.cmd.DrawMeshInstanced(
                        data.ElevatedQuad,
                        0,
                        renderer.GrassMaterial,
                        0,
                        matrices,
                        matrices.Length);
                }
            });
        }
    }
}