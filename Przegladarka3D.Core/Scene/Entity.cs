using Przegladarka3D.Core.Gui;
using System;
namespace Przegladarka3D.Core.Scene;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Entity: Object3D{

	struct VertexPositionColorNormal
	{
	    public Vector3 Position;
	    public Color Color;
	    public Vector3 Normal;
	    // specjalna klasa do definicji ulozenia w pamieci VRAM GPU 
	    public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
	    (
	        new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
	        new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
	        new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
	    );
	}
	VertexPositionColorNormal[] mesh;
	VertexBuffer vertexBuffer;
	IndexBuffer indexbuffer;
	int[] indicies;
	public Entity(string modellocal, ConverterReader konwerter){
		
            var model = konwerter.STLReader(modellocal);
            mesh = new VertexPositionColorNormal[model.Item1.Length];
            for(int i = 0; i < model.Item1.Length; i++){
                mesh[i].Color = Color.Aqua;
                mesh[i].Position = model.Item1[i];
            }
            for(int i = 0; i <model.Item3.Length;i++){

             mesh[i].Normal = model.Item3[i];
            }


            for(int i = 0; i < mesh.Length;i++){
                mesh[i].Normal.Normalize();
            }
			indicies = model.Item2;
	}
	public override void Draw(GraphicsDevice graphicsDevice, BasicEffect effect){
		
            vertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionColorNormal.VertexDeclaration ,mesh.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(mesh);
            indexbuffer = new IndexBuffer(graphicsDevice,IndexElementSize.ThirtyTwoBits,indicies.Length, BufferUsage.WriteOnly);
            indexbuffer.SetData<int>(indicies,0,indicies.Length);            

			graphicsDevice.SetVertexBuffer(vertexBuffer);
			graphicsDevice.Indices = indexbuffer;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
            pass.Apply();
            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,0,0,(indicies.Length/3));
            }
	}
}
