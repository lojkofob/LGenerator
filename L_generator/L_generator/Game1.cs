using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace L_generator
{
    public class Game1 : Game
    {
        public Vector2 int_pos;
        public Vector2 t_pos;
        public textures Textures = new textures();
     //   public UpdateMode mode = UpdateMode.Interface;
        public KeyboardState kbd_pr;
        public KeyboardState kbd;
        public MouseState mouse_pr;
        public MouseState mouse;
        public Vector2 d_mouse = Vector2.Zero;
        public Vector2 mouse_pos = Vector2.Zero;
        public Vector2 mouse_pos_pr = Vector2.Zero;
        public GraphicsDeviceManager graphics;
        public Color ins_color;
        public Instrument current_instrument = null;
        public int current_tex = 0;

      //  GraphicsDevice device;

        public static KeyboardState pr_state;
        public static KeyboardState state;

        public Effect effect;

        
        float time;

        public Vector2 Resolution = new Vector2(640, 480);
        public Vector2 ResolutionOver2;

        public SpriteBatch sprite_batch;
        public bool fullscreen = false ;

        public Game1()
        {
           
           if (fullscreen) Resolution = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
           ResolutionOver2 = Resolution / 2;

           graphics = new GraphicsDeviceManager(this);
           
            

          graphics.PreferredBackBufferWidth = (int)Resolution.X;
          graphics.PreferredBackBufferHeight = (int)Resolution.Y;

          graphics.IsFullScreen = fullscreen;
           

          
            Content.RootDirectory = "Content";

            IsMouseVisible = true;

        }

        public Panel Interface, Palette, Channels, Brushes, Instruments;
        public Texture_Panel t_panel;
        public CheckBox Wire, t_usage;
        public Trackbar cR, cG, cB, cA, DrawMethod, ins_size, Channel, ShowMethod, CRandomizer;

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.CullCounterClockwiseFace;
            rs.DepthBias = 10;
            rs.FillMode = FillMode.WireFrame;
            rs.MultiSampleAntiAlias = true;
            rs.SlopeScaleDepthBias = 1100;

            graphics.GraphicsDevice.RasterizerState = rs;


            sprite_batch = new SpriteBatch(GraphicsDevice);
          
            // создать описание формата вершин
            // vertexDeclaration = new VertexDeclaration(vertexList);

            // создать объект Effect используя контент менеджер
            effect = Content.Load<Effect>("Effect");

            Camera.Init(Resolution);

            int_pos = new Vector2(A._g.Resolution.X - 128, 0);

            Textures.Init();
            Fonts.Init();


          

            ins_size = new Trackbar("Brush Size", 0, 3, 1.5f, false);

            string[] mmm = { "RandomRotate", "MouseRotate", "NonRotate" };
            DrawMethod = new Trackbar("DrawMethod", mmm, 0);

            
         
            Interface = new Panel("",null);

            Wire = new CheckBox("Wire");

            
            Interface.Add(Wire);

            #region Geometry
            Panel bbb = new Panel("Geometry",Interface);
            bbb.Add(new Button("Devide", Devide));
            bbb.Add(new Button("Tesselate", Tesselate));
           // Interface.Add(bbb);
            #endregion

            #region Instruments
            Instruments = new Panel("Instruments", Interface);
            current_instrument = new Instrument("Draw", Draw_, Instruments);
            current_instrument.text_must_color = Color.White;
                                   new Instrument("Mask", Draw_Mask, Instruments);
           // Interface.Add(iii);
            #endregion

           // current_instrument = 
            #region Channels
            Channels = new Panel("Channels",Interface);
            string[] ch = { "texture" };
            Channel = new Trackbar(ch,0,Update_interface);
            Channels.Add(new Button("Add Channel", Add_Channel));
            Channels.Add(new Button("Delete Channel", Delete_Channel));
            Add_Channel();

            t_usage = new CheckBox("USED", set_usage_value);

            ShowMethod = new Trackbar(Enum.GetNames(typeof(Show_Method)), 0, set_method_value);
           
            Channels.Add(ShowMethod);
            Channels.Add(Channel);
            Channels.Add(t_usage);

           // Interface.Add(Channels);
            #endregion

            #region Brushes
            Brushes = new Panel("Brush", Interface);
            Brushes.Add(DrawMethod);
            Brushes.Add(ins_size);
           // Interface.Add(Brushes);
            #endregion

            #region Palette
            Palette = new Panel("Color",Interface);
            cA = new Trackbar("A", 0, 255, 255, false);
            cR = new Trackbar("R", 0, 255, 255, false);
            cG = new Trackbar("G", 0, 255, 255, false);
            cB = new Trackbar("B", 0, 255, 255, false);
            CRandomizer = new Trackbar("random", 0, 1, 0, false);

            CRandomizer.text_must_color = Color.FromNonPremultiplied(0,0,0,200);

            cR.size = new Vector2(128, 10); cR.text_position_add = new Vector2(0, -2);
            cG.size = new Vector2(128, 10); cG.text_position_add = new Vector2(0, -2);
            cB.size = new Vector2(128, 10); cB.text_position_add = new Vector2(0, -2);
            cA.size = new Vector2(128, 10); cA.text_position_add = new Vector2(0, -2);
          //  CRandomizer.size = new Vector2(128, 10); CRandomizer.text_position_add = new Vector2(0, -2);
            
            Palette.Add(cR);
            Palette.Add(cG);
            Palette.Add(cB);
            Palette.Add(cA);
            Palette.Add(CRandomizer);
            Palette.Add(new Button("Clear", _Clear));
           // Interface.Add(Palette);
            #endregion

            t_panel = new Texture_Panel();
            for (int i = 0; i < Textures.tex.Count; i++)
                t_panel.controls.Add(new Ins_Tex(i));

            
            effect.Parameters["Scale"].SetValue(Matrix.CreateScale(1));
            Interface.can_hiden = false;

            Update_interface();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        public void _Clear()
        {
            GraphicsDevice.SetRenderTarget(Textures.map[Channel.intvalue].map);
            GraphicsDevice.Clear(ins_color);
        }

        public void Tesselate()
        { Meshes.Plane.Tesselate(); }

        public void Devide()
        {    Meshes.Plane.Devide();  }

        public void set_usage_value()
        {
            Textures.map[Channel.intvalue].show = t_usage.Checked;
        }

        public void set_method_value()
        {
            Textures.map[Channel.intvalue].method = (Show_Method)(Enum.GetValues(typeof(Show_Method)).GetValue(ShowMethod.intvalue));
        }
        

        public void Update_interface()
        {
            t_usage.Checked = (Textures.map[Channel.intvalue].show);
            ShowMethod.Value = (int)(Textures.map[Channel.intvalue].method);
        }    
        
       
        

        public int d_wheel = 0;
        public float wheel = 0;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            kbd = Keyboard.GetState();
            Interface.color = Color.Gray;

            int r = (int)(cR.intvalue + E.rnd.Next(-255,255) * CRandomizer.value_);
            int g = (int)(cG.intvalue + E.rnd.Next(-255,255) * CRandomizer.value_);
            int b = (int)(cB.intvalue + E.rnd.Next(-255,255) * CRandomizer.value_);
            int a = (int)(cA.intvalue + E.rnd.Next(-255,255) * CRandomizer.value_);
            

            ins_color = Color.FromNonPremultiplied(r,g,b,a);
            
            mouse = Mouse.GetState();

            E.perehod(ref wheel, wheel/3 + d_wheel/1000.0f ,0.95f);

            ins_size.Value += wheel;

            effect.Parameters["ins_size"].SetValue(ins_size.value_ * ins_size.value_);

            if (E.KeyUp(Keys.Escape)) this.Exit();

            mouse_pos = new Vector2(mouse.X, mouse.Y);

            d_mouse = mouse_pos - mouse_pos_pr;

            d_wheel = mouse.ScrollWheelValue - mouse_pr.ScrollWheelValue;

            if (mouse.RightButton != ButtonState.Pressed)
            {
                if (mouse.LeftButton == ButtonState.Pressed) d_mouse = Vector2.Zero;
            }
            else
            {
                Camera.cam_speed = 20;
                Camera.Update(d_wheel>0,false,d_wheel<0,false);
                d_wheel = 0;
            }
            

            mouse_pos_pr = mouse_pos;
       
            if (kbd.IsKeyDown(Keys.Space))
            {
                if (d_wheel != 0)
                {
                    Camera.cam_speed = 20;
                    Camera.Update(d_wheel > 0, false, d_wheel < 0, false);
                    d_wheel = 0;
                }
                else Camera.Update();
              //  Mouse.SetPosition((int)ResolutionOver2.X, (int)ResolutionOver2.Y);
                E.perehod(ref int_pos, new Vector2(A._g.Resolution.X+10, -mouse_pos.Y / 2 + 10), 0.9f, 0.9f);
                E.perehod(ref t_pos, new Vector2(0, -32), 0.9f, 0.9f);
            }
            else
            {
                E.perehod(ref int_pos, new Vector2(A._g.Resolution.X - 128, -mouse_pos.Y/2 + 10), 0.9f, 0.9f);
                E.perehod(ref t_pos, new Vector2(0, 0), 0.9f, 0.9f);

                Interface.Update(int_pos);
                t_panel.Update(t_pos);

                if (E.MouseUp(MouseButtons.Left))
                {
                    if (Interface.MouseOver) Interface.OnMouseUp();
                    if (t_panel.MouseOver) t_panel.OnMouseUp(int_pos);
                }
           }

            time += 0.01f;
            mouse_pr = mouse;
            
            kbd_pr = kbd;
            
            base.Update(gameTime);
        }

        public void Delete_Channel()
        {
            if (Channel.intvalue > 0)
            {
                Textures.delete_channel(Channel.intvalue);
                Channel.DeleteVal(Channel.values[Channel.intvalue]);
                Channel.Value--;
            }
        }

        public void Add_Channel()
        {
            if (Channel.max < 4)
            {
                Channel.Addval("channel " + Channel.values.Count);
                Channel.Value = Channel.max;
                //    Update_interface();
                Textures.add_channel();
            }
        }

        public static Ray FromScreenPoint(Viewport viewport, Vector2 mousePos, Matrix view, Matrix project)
        {
            Vector3 nearPoint = viewport.Unproject(new Vector3(mousePos, 0), project, view, Matrix.Identity);
            Vector3 farPoint = viewport.Unproject(new Vector3(mousePos, 1), project, view, Matrix.Identity);

            return new Microsoft.Xna.Framework.Ray(nearPoint, farPoint - nearPoint);
        }

        public Vector3? IntersectionClosest(Ray ray, Matrix transform)
        {
            var detransform = Matrix.Invert(transform);

            Vector3 p1 = Vector3.Transform(ray.Position, detransform);
            Vector3 p2 = Vector3.Transform(ray.Direction + ray.Position, detransform);
            p2 -= p1;

            var isIntersected = false;
            var distance = 0.0f;
            for (int i = 0; i < Meshes.vertexList.Length; i += 3)
            {
                Vector3 v0 = Meshes.vertexList[Meshes.indexList[i]].Position;
                Vector3 v1 = Meshes.vertexList[Meshes.indexList[i + 1]].Position - Meshes.vertexList[Meshes.indexList[i]].Position;
                Vector3 v2 = Meshes.vertexList[Meshes.indexList[i + 2]].Position - Meshes.vertexList[Meshes.indexList[i]].Position;

                // solution of linear system
                // finds line and plane intersection point (if exists)
                float determinant =
                    -p2.Z * v1.Y * v2.X + p2.Y * v1.Z * v2.X + p2.Z * v1.X * v2.Y
                    - p2.X * v1.Z * v2.Y - p2.Y * v1.X * v2.Z + p2.X * v1.Y * v2.Z;

                if (determinant * determinant < 0.000000001f)
                    continue;

                float kramer = 1.0f / determinant;

                float t1 =
                     (p1.Z * p2.Y * v2.X - p1.Y * p2.Z * v2.X + p2.Z * v0.Y * v2.X
                    - p2.Y * v0.Z * v2.X - p1.Z * p2.X * v2.Y + p1.X * p2.Z * v2.Y
                    - p2.Z * v0.X * v2.Y + p2.X * v0.Z * v2.Y + p1.Y * p2.X * v2.Z
                    - p1.X * p2.Y * v2.Z + p2.Y * v0.X * v2.Z - p2.X * v0.Y * v2.Z) *
                    kramer;

                if (t1 < 0)
                    continue;

                float t2 =
                    -(p1.Z * p2.Y * v1.X - p1.Y * p2.Z * v1.X + p2.Z * v0.Y * v1.X
                    - p2.Y * v0.Z * v1.X - p1.Z * p2.X * v1.Y + p1.X * p2.Z * v1.Y
                    - p2.Z * v0.X * v1.Y + p2.X * v0.Z * v1.Y + p1.Y * p2.X * v1.Z
                    - p1.X * p2.Y * v1.Z + p2.Y * v0.X * v1.Z - p2.X * v0.Y * v1.Z) *
                    kramer;

                if (t2 < 0)
                    continue;

                float t3 =
                    (-p1.Z * v1.Y * v2.X + v0.Z * v1.Y * v2.X + p1.Y * v1.Z * v2.X
                    - v0.Y * v1.Z * v2.X + p1.Z * v1.X * v2.Y - v0.Z * v1.X * v2.Y
                    - p1.X * v1.Z * v2.Y + v0.X * v1.Z * v2.Y - p1.Y * v1.X * v2.Z
                    + v0.Y * v1.X * v2.Z + p1.X * v1.Y * v2.Z - v0.X * v1.Y * v2.Z) *
                    (-kramer);

                if (t3 < 0)
                    continue;

                // (t1>=0 && t2>=0 && t1+t2<=0.5)  => point is on face
                // (t3>0)  =>  point is on positive ray direction
                if (t1 + t2 > 1.0f)
                    continue;

                if (!isIntersected || distance > t3)
                {
                    isIntersected = true;
                    distance = t3;
                }
            }
            if (isIntersected)  return Vector3.Transform((p1 + p2 * distance), transform);
            return null;
        }

        Vector2 ins_origin;
        float ins_rotate;

        void Draw_brush(Vector3 tmp)
        {
            A._g.sprite_batch.Draw(Textures.tex[current_tex], (new Vector2(tmp.X, tmp.Y) + new Vector2(100, 100)) * 2048 / 200, null, ins_color, ins_rotate, ins_origin, ins_size.value_ * ins_size.value_, SpriteEffects.None, 0);
        }

        public void Draw_(dynamic tmp)
        {  ins_act1(); Draw_brush(tmp);   }

        public void Draw_Mask(dynamic tmp)
        { 
            ins_act1(); ins_color = Color.FromNonPremultiplied(255, 255, 255, cA.intvalue); Draw_brush(tmp);  
        }

        void ins_act1()
        {
            ins_origin = (new Vector2(Textures.tex[current_tex].Width, Textures.tex[current_tex].Height)) / 2;
            ins_rotate = 0;
            switch (DrawMethod.intvalue)
            {
                case 0: ins_rotate = E.rnd.Next(10000); break;
                case 1: ins_rotate = (float)E.Angle(t_vect, pr_t_vect) + MathHelper.PiOver2; break;
            }
        }

        Vector2 t_vect;
        Vector2 pr_t_vect;

        public void draw_plane()
        {
            effect.Parameters["t_v"].SetValue(tmp != null);
            if (tmp != null)  {  effect.Parameters["t_vect"].SetValue(t_vect); }
            Meshes.Plane.Draw(Matrix.CreateTranslation(new Vector3(0, 0, 0.001f)));
        }

        Vector3? tmp = null;
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
           
            GraphicsDevice.SetRenderTarget(Textures.map[Channel.intvalue].map);
           // graphics.GraphicsDevice.Clear(Color.White);

            sprite_batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone);

           // Vector2[] arr = new Vector2[1]; arr[0] = mouse_pos;
           // Vector2[] arr1 = new Vector2[1];
            
            Matrix m = Camera.view;

            Ray ray = FromScreenPoint(GraphicsDevice.Viewport, new Vector2(mouse_pos.X/Resolution.X * 2048,mouse_pos.Y/Resolution.Y * 2048) , Camera.view, Camera.proj);
            tmp = IntersectionClosest(ray, Matrix.Identity) ;

         //   tmp = Vector2.Transform(tmp, m); //(Camera.view) );
           // tmp = Vector2.Transform(tmp, m);
           // tmp.Y = -tmp.Y;
            if (t_usage.Checked)
            if (tmp!=null)
                if (current_instrument != null)
                {
                    t_vect = (new Vector2(tmp.Value.X, tmp.Value.Y) + new Vector2(90, 90)) / 200;
                    if (mouse.LeftButton == ButtonState.Pressed && !Interface.MouseOver)
                        current_instrument.Act(tmp.Value);
                }

            sprite_batch.End();

            GraphicsDevice.SetRenderTarget(null);
            graphics.GraphicsDevice.Clear(Color.White);

         //   graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

      /* 
           for (int j = 0; j < 10; j++)
           for (int i = 0; i < 10; i++)
           {
               effect.CurrentTechnique.Passes[0].Apply();
               Meshes.Box.Draw(Matrix.CreateTranslation(i*5,j*5,0));
           }
      */
            A._g.effect.Parameters["World"].SetValue(Matrix.Identity);

         // A._g.effect.CurrentTechnique.Passes[0].Apply();



            effect.Parameters["ins_tex"].SetValue(Textures.tex[current_tex]);

    /*        effect.Parameters["tex"].SetValue(Textures.map[0].map);
            effect.Parameters["tex1"].SetValue(Textures.empty_trans);
            effect.Parameters["tex2"].SetValue(Textures.empty_trans);
            effect.Parameters["tex3"].SetValue(Textures.empty_trans);*/
            int i = 0;
              //  A._g.effect.Parameters["method"].SetValue((int)Textures.map[i].method);
           //     A._g.effect.Parameters["show"].SetValue(Textures.map[i].show);
            //    A._g.effect.Parameters["tex"].SetValue(Textures.map[i].map);
            //    i++;
           // if (Textures.map[0].method != Show_Method.Opaque) 
            for (i = 0; i < Textures.map.Count; i++)
            {
                A._g.effect.Parameters["method" + i].SetValue((int)Textures.map[i].method);
                A._g.effect.Parameters["show" + i].SetValue(Textures.map[i].show);
                A._g.effect.Parameters["tex" + i].SetValue(Textures.map[i].map);
                if (Textures.map[i].method == Show_Method.Opaque) {
                    for (int j = 0; j < i; j++) { A._g.effect.Parameters["show" + j].SetValue(false); A._g.effect.Parameters["show" + j].SetValue(false); } i++; break; 
                }
            }
            
                for (; i < 5; i++)
                {
                    A._g.effect.Parameters["show" + i].SetValue(false);
                }


                    draw_plane();

            //    GraphicsDevice.SetRenderTarget(Textures.screen);
             /*   GraphicsDevice.Clear(Color.White);

                

                GraphicsDevice.SetRenderTarget(null);
                sprite_batch.Begin();
                sprite_batch.Draw(Textures.screen, Vector2.Zero, Color.White);
                sprite_batch.End();*/
            
            
           effect.Parameters["tex0"].SetValue(Textures.cube_texture);

           if (tmp != null)
           Meshes.Small_Box.Draw(Matrix.CreateTranslation(tmp.Value));

          
           sprite_batch.Begin(SpriteSortMode.Deferred,BlendState.AlphaBlend,SamplerState.AnisotropicClamp,DepthStencilState.Default,RasterizerState.CullNone);
           sprite_batch.Draw(A._g.Textures.button_texture, new Vector2(int_pos.X - 10, -50 - Resolution.Y / 10), null, Color.Gray, 0, Vector2.Zero, new Vector2(1.2f, 10 + Resolution.Y / 20), SpriteEffects.None, 0);
           Interface.Draw(int_pos);

           t_panel.Draw(t_pos);
           sprite_batch.DrawString(Fonts.font1, hhhhhh, new Vector2(2, 20), Color.White);
           sprite_batch.DrawString(Fonts.font1, hhhhhh, new Vector2(2, 22), Color.FromNonPremultiplied(0,0,0,150));
           sprite_batch.End();

         //  sprite_batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone);
         //  Interface.Draw(int_pos);
         //  sprite_batch.End();
           // вывод сцены на экран
           GraphicsDevice.SetRenderTarget(null);

            base.Draw(gameTime);
            pr_t_vect = t_vect;
        }
        public string hhhhhh = " Space + WASD - moving\nLeftMouseBtn - Draw\nRightMouseBtn - rotate view";
    }

    public static class Camera
    {
       static public Matrix proj;
       static float aspectRatio=1;
       static public void Init(Vector2 size)
        {
           // aspectRatio = (float)size.X / size.Y;
            proj = Matrix.CreatePerspective(MathHelper.PiOver2, aspectRatio, 0.9f, 1000);
            Update();
        }

       static public Vector3 position = new Vector3(100, -10, 103);
       static public Vector2 an = new Vector2(0, -MathHelper.Pi+.6f);

       static public Matrix view
       {
          // x = ρ cos1 sin2 , y= ρ sin1 sin2 , z = ρ cos2.
           get { return Matrix.CreateLookAt(position, new Vector3( position.X + (float)(Math.Cos(an.X) * Math.Sin(an.Y)),
                                                                   position.Y + (float)(Math.Sin(an.X) * Math.Sin(an.Y)),
                                                                   position.Z + (float)Math.Cos(an.Y)),
                                                                   new Vector3(0, 0, 1));
           }
       }

       static public Matrix view_proj     {      get     {  return view * proj;    }    }

       static public float cam_speed = 0.1f, mouse_sens = 0.005f;

       static public void Update()
       {
          Update(A._g.kbd.IsKeyDown(Keys.W), A._g.kbd.IsKeyDown(Keys.A), A._g.kbd.IsKeyDown(Keys.S),A._g.kbd.IsKeyDown(Keys.D));
       }

       static public void Update(bool w, bool a, bool s, bool d)
       {
          E.perehod(ref cam_speed, Math.Abs(position.Z) / 30 + 0.1f,0.95f);
           float _ax = an.X - A._g.d_mouse.X * mouse_sens;
           float _ay = an.Y - A._g.d_mouse.Y * mouse_sens;

           if (_ay < 0 && _ay > -MathHelper.Pi) an.Y = _ay;
           an.X = _ax;

           float cax = (float)(Math.Cos(an.X));
           float sax = (float)(Math.Sin(an.X));
           float cay = (float)(Math.Cos(an.Y));
           float say = (float)(Math.Sin(an.Y));

           Vector3 mov = cam_speed *
               new Vector3((s ? -cax * say : 0) + (w ? cax * say : 0) +
                           (a ? sax : 0) + (A._g.kbd.IsKeyDown(Keys.D) ? -sax : 0),
                             (s ? -sax * say : 0) + (w ? sax * say : 0) +
                             (a ? -cax : 0) + (d ? cax : 0),
                             (s ? -cay : 0) + (w ? cay : 0)
                             );

           position += mov;

           A._g.effect.Parameters["ViewProj"].SetValue(Camera.view_proj);
       }

      static public new string ToString()
       {
           return position.ToString() + "  " + an.ToString();
       }
    }

    public class Mesh 
    {
        VertexPositionTexture[] vertexList;
        VertexPositionColor[] vertexColor;
        public Matrix World = Matrix.Identity;
        public int tessellation;

        short[] indexList;

        public Mesh(VertexPositionTexture[] vertexList, short[] indexList)
        {     this.vertexList = vertexList; this.indexList = indexList; vertexColor = new VertexPositionColor[vertexList.Length];

        tessellation = (int)Math.Sqrt(indexList.Length / 6)+1;

        int i = 0;
            foreach (VertexPositionTexture vp in vertexList  )
                    vertexColor[i++] = new VertexPositionColor(vp.Position, Color.Red);
        }
        
        public void Draw(Matrix world)
        {
            Draw(world, 0);
        }

        public void Draw(Matrix world, int pass)
        {
            A._g.effect.Parameters["World"].SetValue(world);
            A._g.effect.CurrentTechnique.Passes[pass].Apply();
            A._g.graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, vertexList, 0, vertexList.Length, indexList, 0, indexList.Length / 3);

            if (A._g.Wire.Checked)
            {
                A._g.effect.CurrentTechnique.Passes[1].Apply();
                A._g.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertexColor, 0, vertexList.Length, indexList, 0, vertexList.Length - 1);
            }
            // A._g.graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>(PrimitiveType.LineList, vertexList, 0, vertexList.Length, indexList, 0, indexList.Length / 3);
        }

        public float GetHeight(Vector2 pos)
        {
            return 0;
        }

        public void Tesselate()
        {
            tessellation++ ;
            indexList = new short[(tessellation - 1) * (tessellation - 1) * 6];
            vertexList = new VertexPositionTexture[tessellation * tessellation];
            vertexColor = new VertexPositionColor[tessellation * tessellation];
            
            for (int y = 0; y < tessellation; y++)
            {
                for (int x = 0; x < tessellation; x++)
                {
                    int arrayIndex = y * tessellation + x;
                    Vector2 pos = 200 * new Vector2((float)x / (float)(tessellation - 1), (float)y / (float)(tessellation - 1)) - new Vector2(100,100);
                    VertexPositionTexture vertex = new VertexPositionTexture(new Vector3(pos, GetHeight(pos)), (pos + new Vector2(100,100))/200);
                    vertexList[arrayIndex] = vertex;
                }
            }

            for (int y = 0; y < (tessellation - 1); y++)
            {
                for (int x = 0; x < (tessellation - 1); x++)
                {
                    int arrayIndex = (y * (tessellation - 1) + x) * 6;
                    int vertexIndex = y * tessellation + x;

                    indexList[arrayIndex] = (short)vertexIndex;
                    indexList[arrayIndex + 1] = (short)(vertexIndex + 1);
                    indexList[arrayIndex + 2] = (short)(vertexIndex + tessellation);
                    indexList[arrayIndex + 3] = (short)(vertexIndex + tessellation);
                    indexList[arrayIndex + 4] = (short)(vertexIndex + 1);
                    indexList[arrayIndex + 5] = (short)(vertexIndex + tessellation + 1);
                }
            }

            int i = 0;
            foreach (VertexPositionTexture vp in vertexList)
                vertexColor[i++] = new VertexPositionColor(vp.Position, Color.Red);
        }

        public void Devide()
        {

        }

    }

    static public class Meshes 
    {
       static List<Mesh> meshes = new List<Mesh>(); 
       static public Mesh Box;
       static public Mesh Small_Box;
       static public Mesh Plane;
       static public VertexPositionTexture[] vertexList;
       static public short[] indexList;

       static public void Init_cubes_Verts(float scale)
       {
           vertexList = new VertexPositionTexture[8];
           vertexList[0] = new VertexPositionTexture(new Vector3(-1.0f, -1.0f, 1.0f) * scale, new Vector2(0, 1));
           vertexList[1] = new VertexPositionTexture(new Vector3(1.0f, -1.0f, 1.0f) * scale, new Vector2(1, 1));
           vertexList[2] = new VertexPositionTexture(new Vector3(-1.0f, -1.0f, -1.0f) * scale, new Vector2(0, 0));
           vertexList[3] = new VertexPositionTexture(new Vector3(1.0f, -1.0f, -1.0f) * scale, new Vector2(1, 0));
           vertexList[4] = new VertexPositionTexture(new Vector3(-1.0f, 1.0f, 1.0f) * scale, new Vector2(1, 1));
           vertexList[5] = new VertexPositionTexture(new Vector3(1.0f, 1.0f, 1.0f) * scale, new Vector2(0, 1));
           vertexList[6] = new VertexPositionTexture(new Vector3(-1.0f, 1.0f, -1.0f) * scale, new Vector2(1, 0));
           vertexList[7] = new VertexPositionTexture(new Vector3(1.0f, 1.0f, -1.0f) * scale, new Vector2(0, 0));
       }

       static Meshes()
        {

            indexList = new short[36];
            // пара треугольников нижней грани куба
            indexList[0] = 3; indexList[1] = 1; indexList[2] = 0;  indexList[3] = 2; indexList[4] = 3; indexList[5] = 0;
            // пара треугольников верхней грани куба
            indexList[6] = 4; indexList[7] = 5; indexList[8] = 7;  indexList[9] = 7; indexList[10] = 6; indexList[11] = 4;
            // пара треугольников левой грани куба
            indexList[12] = 4; indexList[13] = 2; indexList[14] = 0;  indexList[15] = 2; indexList[16] = 4; indexList[17] = 6;
            // пара треугольников передней грани куба
            indexList[18] = 6; indexList[19] = 3; indexList[20] = 2;  indexList[21] = 3; indexList[22] = 6; indexList[23] = 7;
            // пара треугольников правой грани куба
            indexList[24] = 1; indexList[25] = 3; indexList[26] = 7;  indexList[27] = 7; indexList[28] = 5; indexList[29] = 1;
            // пара треугольников задней грани куба
            indexList[30] = 0; indexList[31] = 1; indexList[32] = 5;  indexList[33] = 5; indexList[34] = 4; indexList[35] = 0;

            Init_cubes_Verts(1);
            Box = new Mesh(vertexList, indexList);
            Init_cubes_Verts(0.1f);
            Small_Box = new Mesh(vertexList, indexList);

            vertexList = new VertexPositionTexture[4];
            vertexList[0] = new VertexPositionTexture(new Vector3(-100.0f, 100f, 0), new Vector2(0, 1));
            vertexList[1] = new VertexPositionTexture(new Vector3(100.0f, 100f, 0), new Vector2(1, 1));
            vertexList[2] = new VertexPositionTexture(new Vector3(-100.0f, -100f, 0), new Vector2(0, 0));
            vertexList[3] = new VertexPositionTexture(new Vector3(100.0f, -100f, 0), new Vector2(1, 0));
            indexList = new short[6];
            indexList[0] = 3; indexList[1] = 1; indexList[2] = 0; indexList[3] = 2; indexList[4] = 3; indexList[5] = 0;
            Plane = new Mesh(vertexList, indexList);                        
        }
    }
    public enum MouseButtons { Left, Right }

    static public class E
    {
      public delegate void _void();
     // public delegate void _void_bool(bool flag);
      public delegate void _void_obj(dynamic obj);
     // public delegate void _void_Vector3(Vector3 val);
      static public Random rnd = new Random();
      static public bool KeyUp(Keys key) { return A._g.kbd_pr.IsKeyDown(key) && A._g.kbd.IsKeyUp(key); }
      static public bool MouseUp(MouseButtons button)
       {
           bool res = false;
           switch (button)
           {
               case MouseButtons.Left: res = A._g.mouse.LeftButton == ButtonState.Released && A._g.mouse_pr.LeftButton == ButtonState.Pressed; break;
               case MouseButtons.Right: res = A._g.mouse.RightButton == ButtonState.Released && A._g.mouse_pr.RightButton == ButtonState.Pressed; break;
           }
           return res;
       }


       static public bool limits(Vector2 val, Vector4 limit)
       {    return val.X > limit.X && val.Y > limit.Y && val.X < limit.Z && val.Y < limit.W;   }

       static public bool limits(Vector2 val, Vector2 position, Vector2 size)
       { return val.X > position.X && val.Y > position.Y && val.X < position.X + size.X && val.Y < position.Y + size.Y; }

       static public Color perehod(Color col1, Color col2, double time)
       {
           col1.R = perehod(col1.R, col2.R, time);
           col1.G = perehod(col1.G, col2.G, time);
           col1.B = perehod(col1.B, col2.B, time);
           col1.A = perehod(col1.A, col2.A, time);
           return col1;
       }

       static public void perehod(ref Color col1, Color col2, double time)
       {
           col1.R = perehod(col1.R, col2.R, time);
           col1.G = perehod(col1.G, col2.G, time);
           col1.B = perehod(col1.B, col2.B, time);
           col1.A = perehod(col1.A, col2.A, time);
       }

       static public byte perehod(byte val1, byte val2, double time)
       { return (byte)((val1 - val2) * time + val2); }

       static public byte perehod(ref byte val1, byte val2, double time)
       { return (byte)((val1 - val2) * time + val2); }

       static public void perehod(ref double val1, double val2, double time)
       { val1 = (val1 - val2) * time + val2; }
       static public void perehod(ref float val1, float val2, float time)
       { val1 = (val1 - val2) * time + val2; }

       static public void perehod(ref Vector2 val1, Vector2 val2, float time, float time2)
       { val1 = (val1 - val2) * time + val2; }

       static public double Angle(Vector2 v1, Vector2 v2)
       {
           if (v1.Y == v2.Y) return v1.Y < v2.Y ? Math.PI : 0;
           return (v1.Y < v2.Y ? Math.PI / 2 - Math.Atan((v1.X - v2.X) / (v1.Y - v2.Y)) : Math.Atan((v1.X - v2.X) / (-v1.Y + v2.Y)) - Math.PI / 2);
       }

       static public double Distance(Vector2 v1, Vector2 v2)
       { return Math.Sqrt((v1.X - v2.X) * (v1.X - v2.X) + (v1.Y - v2.Y) * (v1.Y - v2.Y)); }
       static public double Distance(Vector4 v)
       { return Math.Sqrt((v.X - v.Z) * (v.X - v.Z) + (v.Y - v.W) * (v.Y - v.W)); }
       static public double Distance(double x1, double y1, double x2, double y2)
       { return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)); }
       static public float Distance(float x1, float y1, float x2, float y2)
       { return (float)Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)); }

    }

    public class channel_map
    {
        public RenderTarget2D map;
        public bool show = true;
        public Show_Method method = Show_Method.Additive;
        public channel_map(RenderTarget2D texture)
        { this.map = texture; }
        
    }

    public class textures
    {
       public Texture2D cube_texture,
                        b_tex,
                        button_texture,
                        instrument_texture,
                        checkbox_texture,
                        square, 
                        dot,
                        empty_trans, empty_white;

       public List<Texture2D> tex = new List<Texture2D>();

       public RenderTarget2D palette, white256256, mask_map;
       public List<channel_map> map = new List<channel_map>();

       public void Init()
        {
            Type T = typeof(textures);
            System.Reflection.FieldInfo[] fis = T.GetFields();

           
            foreach (System.Reflection.FieldInfo fi in fis) if (fi.FieldType == typeof(Texture2D)) fi.SetValue(this, A._g.Content.Load<Texture2D>(fi.Name));
           
            try
            {
                int i = 0;
                while (true)
                {
                    Texture2D tt = A._g.Content.Load<Texture2D>((i++).ToString());
                    tex.Add(tt);
                }
            }
            catch
            { }

          //  screen = new RenderTarget2D(A._g.GraphicsDevice, (int)A._g.Resolution.X, (int)A._g.Resolution.Y);

           // screen = new RenderTarget2D(A._g.GraphicsDevice, (int)A._g.Resolution.X, (int)A._g.Resolution.Y, true, SurfaceFormat.Rgba64, DepthFormat.Depth24, 1, RenderTargetUsage.PreserveContents);
            
            palette = new RenderTarget2D(A._g.GraphicsDevice, 256, 256, true, SurfaceFormat.Rgba64, DepthFormat.Depth24, 1, RenderTargetUsage.PlatformContents);
            add_channel();

            white256256 = new RenderTarget2D(A._g.GraphicsDevice, 256, 256, true, SurfaceFormat.Rgba64, DepthFormat.Depth24, 1, RenderTargetUsage.PlatformContents);
            A._g.GraphicsDevice.SetRenderTarget(white256256);
            A._g.GraphicsDevice.Clear(Color.White);

          //  A._g.effect.Parameters["screen"].SetValue(screen);
          /*  mask_map = new RenderTarget2D(A._g.GraphicsDevice, 2048, 2048, true, SurfaceFormat.Rgba64, DepthFormat.Depth24, 1, RenderTargetUsage.PreserveContents);
            A._g.GraphicsDevice.SetRenderTarget(mask_map);
            A._g.GraphicsDevice.Clear(Color.White);
            A._g.GraphicsDevice.SetRenderTarget(null);*/
        }

       public void add_channel()
       {
           map.Add(new channel_map(new RenderTarget2D(A._g.GraphicsDevice, 2048, 2048, true, SurfaceFormat.Rgba64, DepthFormat.Depth24, 1, RenderTargetUsage.PreserveContents)));
           A._g.GraphicsDevice.SetRenderTarget(map[map.Count-1].map);
           A._g.GraphicsDevice.Clear(Color.Black);
           A._g.GraphicsDevice.SetRenderTarget(null);
       }
       public void delete_channel(int index)
       {
           map.RemoveAt(index);
       }
    }

    static public class Fonts
    {
        static public SpriteFont font1, font2;
        static public void Init()
        {
            font1 = A._g.Content.Load<SpriteFont>("SpriteFont1");
            font2 = A._g.Content.Load<SpriteFont>("SpriteFont2");
        }
    }
    
    public enum Show_Method { Opaque, Additive, Multiply  };
}
