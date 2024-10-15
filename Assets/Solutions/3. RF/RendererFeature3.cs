using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Solutions._3._RF
{
    public class RendererFeature3 : ScriptableRendererFeature
    {
        [SerializeField]
        private RfResources resources;
        
        private Pass3 pass3;

        public override void Create()
        {
            pass3 = new Pass3(resources, RenderPassEvent.AfterRenderingOpaques);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(pass3);
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

    public class Pass3 : ScriptableRenderPass
    {
        private RfResources resources;

        internal Pass3( RfResources resources, RenderPassEvent passEvent)
        {
            this.resources = resources;
            renderPassEvent = passEvent;
        }
        
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            using IRasterRenderGraphBuilder builder =
                renderGraph.AddRasterRenderPass("GrassPass", out PassData passData);

            passData.Initialize(resources.ElevatedQuad);
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            
            passData.GrassMatricesTRS.Clear();

            foreach (GrassRenderer renderer in GrassRenderer.AllGrassRenderers)
            {
                Matrix4x4[] matrices = renderer.GetGrassPositions().AsPositionMatricesArray();
                passData.GrassMatricesTRS.Add(renderer, matrices);
            }
            
            builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
            builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture);

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