using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScanLinesRenderFeature : ScriptableRendererFeature
{
    class ScanLinesPass : ScriptableRenderPass
    {
        private Material material;
        private ScanLinesVolumeComponent volumeComponent;

        public ScanLinesPass(Material mat)
        {
            material = mat;
            renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var stack = VolumeManager.instance.stack;
            volumeComponent = stack.GetComponent<ScanLinesVolumeComponent>();

            if (volumeComponent == null || !volumeComponent.enabled.value) return;

            material.SetFloat("_intensity", volumeComponent.intensity.value);
            material.SetFloat("_lineCount", volumeComponent.lineCount.value);

            CommandBuffer cmd = CommandBufferPool.Get("ScanLines");

            var cameraData = renderingData.cameraData;
            RTHandle source = cameraData.renderer.cameraColorTargetHandle;

            Blitter.BlitCameraTexture(cmd, source, source, material, 0);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }
    }

    [SerializeField] private Shader shader;
    private Material material;
    private ScanLinesPass pass;

    public override void Create()
    {
        if (shader == null) return;
        material = new Material(shader);
        pass = new ScanLinesPass(material);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (material == null) return;
        renderer.EnqueuePass(pass);
    }
}