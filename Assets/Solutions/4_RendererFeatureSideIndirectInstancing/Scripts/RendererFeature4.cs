using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Solutions._4_RendererFeatureSideIndirectInstancing.Scripts
{
    public class RendererFeature4 : ScriptableRendererFeature
    {
        [SerializeField]
        private Mesh grassMesh;
        private GrassPass grassPass;
        
        public override void Create()
        {
            grassPass = new GrassPass(grassMesh, RenderPassEvent.AfterRenderingOpaques);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(grassPass);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                grassPass.Dispose();
            }
        }
    }

    public class PassData
    {
    }

    public class GrassPass : ScriptableRenderPass, IDisposable
    {
        private readonly Mesh elevatedQuad;
        private GraphicsBuffer argsBuffer;

        internal GrassPass(Mesh elevatedQuad, RenderPassEvent renderPassEvent)
        {
            this.elevatedQuad = elevatedQuad;
            this.renderPassEvent = renderPassEvent;

            argsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments,5, sizeof(uint));
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            using (IRasterRenderGraphBuilder builder = renderGraph.AddRasterRenderPass("Grass", out PassData passData))
            {
                UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

                builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
                builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture);

                builder.UseBuffer(renderGraph.ImportBuffer(argsBuffer));
                builder.UseBuffer(renderGraph.ImportBuffer(GrassRenderer.AllGrassRenderers[0].PositionBuffer));

                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    foreach (GrassRenderer renderer in GrassRenderer.AllGrassRenderers)
                    {
                        uint[] args = new uint[5] { elevatedQuad.GetIndexCount(0), 100000, 0, 0, 0 };
                        argsBuffer.SetData(args);
                        renderer.GrassMaterial.SetBuffer("_PositionBuffer",renderer.PositionBuffer);
                        context.cmd.DrawMeshInstancedIndirect(elevatedQuad, 0, renderer.GrassMaterial, 0,
                            argsBuffer);
                    }
                });
            }
        }

        public void Dispose()
        {
            // if (argsBuffer != null)
            // {
            //     argsBuffer.Dispose();
            //     argsBuffer = null;
            // }
        }
    }
}