using System;
using Pretty_solutions.PrettyGrass.Scripts.Editor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Pretty_solutions.PrettyGrass.Scripts
{
    public class PrettyGrassFeature : ScriptableRendererFeature
    {
        [SerializeField]
        private Material gridOverlay; 

        private PrettyGrassPass grassPass;

        public override void Create()
        {
            if (gridOverlay != null)
            {
                grassPass = new PrettyGrassPass(gridOverlay);
            }
            else
            {
                Debug.LogError($"Grid won't be rendered as grid overlay material is null.", this);
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (grassPass != null && grassPass.IsValid() && VegetationPainterData.ShouldRenderGrid)
            {
                renderer.EnqueuePass(grassPass);
            }
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

    public class PrettyGrassPass : ScriptableRenderPass, IDisposable
    {
        private readonly Material gridOverlay;

        public bool IsValid()
        {
            if (gridOverlay == null)
            {
                Debug.LogError($"Grid won't be rendered as {gridOverlay.name} material is null.", gridOverlay);
                return false;
            }

            return true;
        }
        
        public PrettyGrassPass(Material gridOverlay)
        {
            this.gridOverlay = gridOverlay;
        }
        
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            using IRasterRenderGraphBuilder builder = renderGraph.AddRasterRenderPass("PrettyGrassGridOverlay", out PassData _);
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            
            builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
            builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture);
            
            builder.SetRenderFunc((PassData _,RasterGraphContext context) =>
            {
                foreach ((PrettyGrass _, Renderer renderer) in PrettyGrass.PrettyGrassToRenderers)
                {
                    context.cmd.DrawRenderer(renderer, gridOverlay);
                }
            });
        }

        public void Dispose()
        {
            
        }
    }
}