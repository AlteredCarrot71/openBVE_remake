﻿using OpenBve.Worlds;
using ReTao.OpenGl;
using System;

namespace OpenBve
{
    internal static class Renderer
    {

        // screen (output window)
        internal static int ScreenWidth = 0;
        internal static int ScreenHeight = 0;

        // first frame behavior
        internal enum LoadTextureImmediatelyMode { NotYet, Yes, NoLonger }
        internal static LoadTextureImmediatelyMode LoadTexturesImmediately = LoadTextureImmediatelyMode.NotYet;

        // transparency
        internal enum TransparencyMode
        {
            /// <summary>Textures using color-key transparency are considered opaque, producing good performance but crisp outlines. Partially transparent faces are rendered in a single pass with z-buffer writes disabled, producing good performance but more depth-sorting issues.</summary>
            Performance = 0,
            /// <summary>Textures using color-key transparency are considered opaque, producing good performance but crisp outlines. Partially transparent faces are rendered in two passes, the first rendering only opaque pixels with z-buffer writes enabled, and the second rendering only partially transparent pixels with z-buffer writes disabled, producing best quality but worse performance.</summary>
            Intermediate = 1,
            /// <summary>Textures using color-key transparency are considered partially transparent. All partially transparent faces are rendered in two passes, the first rendering only opaque pixels with z-buffer writes enabled, and the second rendering only partially transparent pixels with z-buffer writes disabled, producing best quality but worse performance.</summary>
            Quality = 2
        }

        // output mode
        internal enum OutputMode
        {
            Default = 0,
            Debug = 1,
            None = 2
        }
        internal static OutputMode CurrentOutputMode = OutputMode.Default;

        // object list
        private struct Object
        {
            internal int ObjectIndex;
            internal ObjectListReference[] FaceListReferences;
            internal ObjectType Type;
        }
        private static Object[] Objects = new Object[256];
        private static int ObjectCount = 0;

        private enum ObjectListType : byte
        {
            /// <summary>The face is fully opaque and originates from an object that is part of the static scenery.</summary>
            StaticOpaque = 1,
            /// <summary>The face is fully opaque and originates from an object that is part of the dynamic scenery or of a train exterior.</summary>
            DynamicOpaque = 2,
            /// <summary>The face is partly transparent and originates from an object that is part of the scenery or of a train exterior.</summary>
            DynamicAlpha = 3,
            /// <summary>The face is fully opaque and originates from an object that is part of the cab.</summary>
            OverlayOpaque = 4,
            /// <summary>The face is partly transparent and originates from an object that is part of the cab.</summary>
            OverlayAlpha = 5
        }
        internal enum ObjectType : byte
        {
            /// <summary>The object is part of the static scenery. The matching ObjectListType is StaticOpaque for fully opaque faces, and DynamicAlpha for all other faces.</summary>
            Static = 1,
            /// <summary>The object is part of the animated scenery or of a train exterior. The matching ObjectListType is DynamicOpaque for fully opaque faces, and DynamicAlpha for all other faces.</summary>
            Dynamic = 2,
            /// <summary>The object is part of the cab. The matching ObjectListType is OverlayOpaque for fully opaque faces, and OverlayAlpha for all other faces.</summary>
            Overlay = 3
        }

        private struct ObjectListReference
        {
            /// <summary>The type of list.</summary>
            internal ObjectListType Type;
            /// <summary>The index in the specified list.</summary>
            internal int Index;
            internal ObjectListReference(ObjectListType type, int index)
            {
                this.Type = type;
                this.Index = index;
            }
        }
        private class ObjectFace
        {
            internal int ObjectListIndex;
            internal int ObjectIndex;
            internal int FaceIndex;
            internal double Distance;
        }
        private class ObjectList
        {
            internal ObjectFace[] Faces;
            internal int FaceCount;
            internal ObjectList()
            {
                this.Faces = new ObjectFace[256];
                this.FaceCount = 0;
            }
        }
        private class ObjectGroup
        {
            internal ObjectList List;
            internal int OpenGlDisplayList;
            internal bool OpenGlDisplayListAvailable;
            internal Vectors.Vector3D WorldPosition;
            internal bool Update;
            internal ObjectGroup()
            {
                this.List = new ObjectList();
                this.OpenGlDisplayList = 0;
                this.OpenGlDisplayListAvailable = false;
                this.WorldPosition = new Vectors.Vector3D(0.0, 0.0, 0.0);
                this.Update = true;
            }
        }

        // the static opaque lists
        /// <summary>The list of static opaque face groups. Each group contains only objects that are associated the respective group index.</summary>
        private static ObjectGroup[] StaticOpaque = new ObjectGroup[] { };
        /// <summary>Whether to enforce updating all display lists.</summary>
        internal static bool StaticOpaqueForceUpdate = true;

        // all other lists
        /// <summary>The list of dynamic opaque faces to be rendered.</summary>
        private static ObjectList DynamicOpaque = new ObjectList();
        /// <summary>The list of dynamic alpha faces to be rendered.</summary>
        private static ObjectList DynamicAlpha = new ObjectList();
        /// <summary>The list of overlay opaque faces to be rendered.</summary>
        private static ObjectList OverlayOpaque = new ObjectList();
        /// <summary>The list of overlay alpha faces to be rendered.</summary>
        private static ObjectList OverlayAlpha = new ObjectList();

        // current opengl data
        private static int AlphaFuncComparison = 0;
        private static float AlphaFuncValue = 0.0f;
        private static bool AlphaTestEnabled = false;
        private static bool BlendEnabled = false;
        private static bool CullEnabled = true;
        internal static bool LightingEnabled = false;
        internal static bool FogEnabled = false;
        private static bool TexturingEnabled = false;
        private static bool EmissiveEnabled = false;

        // options
        internal static bool OptionLighting = true;
        internal static Colors.ColorRGB OptionAmbientColor = new Colors.ColorRGB(160, 160, 160);
        internal static Colors.ColorRGB OptionDiffuseColor = new Colors.ColorRGB(160, 160, 160);
        internal static Vectors.Vector3Df OptionLightPosition = new Vectors.Vector3Df(0.223606797749979f, 0.86602540378444f, -0.447213595499958f);
        internal static float OptionLightingResultingAmount = 1.0f;
        internal static bool OptionNormals = false;
        internal static bool OptionWireframe = false;
        internal static bool OptionBackfaceCulling = true;

        // interface options
        internal static bool OptionClock = false;
        internal enum SpeedDisplayMode { None, Kmph, Mph }
        internal static SpeedDisplayMode OptionSpeed = SpeedDisplayMode.None;
        internal static bool OptionFrameRates = false;
        internal static bool OptionBrakeSystems = false;

        // fade to black
        private static double FadeToBlackDueToChangeEnds = 0.0;

        // textures
        private static int TextureLogo = -1;
        private static int TexturePause = -1;

        // constants
        private const float inv255 = 1.0f / 255.0f;

        // reset
        internal static void Reset()
        {
            LoadTexturesImmediately = LoadTextureImmediatelyMode.NotYet;
            Objects = new Object[256];
            ObjectCount = 0;
            StaticOpaque = new ObjectGroup[] { };
            StaticOpaqueForceUpdate = true;
            DynamicOpaque = new ObjectList();
            DynamicAlpha = new ObjectList();
            OverlayOpaque = new ObjectList();
            OverlayAlpha = new ObjectList();
            OptionLighting = true;
            OptionAmbientColor = new Colors.ColorRGB(160, 160, 160);
            OptionDiffuseColor = new Colors.ColorRGB(160, 160, 160);
            OptionLightPosition = new Vectors.Vector3Df(0.223606797749979f, 0.86602540378444f, -0.447213595499958f);
            OptionLightingResultingAmount = 1.0f;
            OptionClock = false;
            OptionBrakeSystems = false;
        }

        // initialize
        internal static void Initialize()
        {
            // opengl
            Gl.glShadeModel(Gl.GL_SMOOTH);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glDepthFunc(Gl.GL_LEQUAL);
            Gl.glHint(Gl.GL_FOG_HINT, Gl.GL_FASTEST);
            Gl.glHint(Gl.GL_LINE_SMOOTH_HINT, Gl.GL_FASTEST);
            Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_FASTEST);
            Gl.glHint(Gl.GL_POINT_SMOOTH_HINT, Gl.GL_FASTEST);
            Gl.glHint(Gl.GL_POLYGON_SMOOTH_HINT, Gl.GL_FASTEST);
            Gl.glHint(Gl.GL_GENERATE_MIPMAP_HINT, Gl.GL_NICEST);
            Gl.glDisable(Gl.GL_DITHER);
            Gl.glCullFace(Gl.GL_FRONT);
            Gl.glEnable(Gl.GL_CULL_FACE); CullEnabled = true;
            Gl.glDisable(Gl.GL_LIGHTING); LightingEnabled = false;
            Gl.glDisable(Gl.GL_TEXTURE_2D); TexturingEnabled = false;
            // hud
            Interface.LoadHUD();
            string Path = Program.FileSystem.GetDataFolder("In-game");
            TextureLogo = TextureManager.RegisterTexture(Interface.GetCombinedFileName(Path, "logo.png"), new Colors.ColorRGB(0, 0, 0), 0, TextureManager.TextureWrapMode.ClampToEdge, TextureManager.TextureWrapMode.ClampToEdge, false);
            TextureManager.ValidateTexture(ref TextureLogo);
            // opengl
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glPushMatrix();
            Gl.glClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            Glu.gluLookAt(0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 1.0, 0.0);
            Gl.glPopMatrix();
            // prepare rendering logo
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glEnable(Gl.GL_BLEND); BlendEnabled = true;
            Gl.glDisable(Gl.GL_LIGHTING); LightingEnabled = false;
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glPushMatrix();
            Gl.glLoadIdentity();
            Gl.glOrtho(0.0, (double)ScreenWidth, 0.0, (double)ScreenHeight, -1.0, 1.0);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();
            Gl.glLoadIdentity();
            // render logo
            double size = ScreenWidth < ScreenHeight ? ScreenWidth : ScreenHeight;
            Gl.glColor3f(1.0f, 1.0f, 1.0f);
            RenderOverlayTexture(TextureLogo, 0.5 * (ScreenWidth - size), 0.5 * (ScreenHeight - size), 0.5 * (ScreenWidth + size), 0.5 * (ScreenHeight + size));
            // finalize
            Gl.glPopMatrix();
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glPopMatrix();
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glDisable(Gl.GL_BLEND);
        }

        // deinitialize
        internal static void Deinitialize()
        {
            ClearDisplayLists();
        }

        // clear display lists
        internal static void ClearDisplayLists()
        {
            for (int i = 0; i < StaticOpaque.Length; i++)
            {
                if (StaticOpaque[i] != null)
                {
                    if (StaticOpaque[i].OpenGlDisplayListAvailable)
                    {
                        Gl.glDeleteLists(StaticOpaque[i].OpenGlDisplayList, 1);
                        StaticOpaque[i].OpenGlDisplayListAvailable = false;
                    }
                }
            }
            StaticOpaqueForceUpdate = true;
        }

        // initialize lighting
        internal static void InitializeLighting()
        {
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_AMBIENT, new float[] { inv255 * (float)OptionAmbientColor.R, inv255 * (float)OptionAmbientColor.G, inv255 * (float)OptionAmbientColor.B, 1.0f });
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, new float[] { inv255 * (float)OptionDiffuseColor.R, inv255 * (float)OptionDiffuseColor.G, inv255 * (float)OptionDiffuseColor.B, 1.0f });
            Gl.glLightModelfv(Gl.GL_LIGHT_MODEL_AMBIENT, new float[] { 0.0f, 0.0f, 0.0f, 1.0f });
            Gl.glCullFace(Gl.GL_FRONT); CullEnabled = true; // possibly undocumented, but required for correct lighting
            Gl.glEnable(Gl.GL_LIGHT0);
            Gl.glEnable(Gl.GL_COLOR_MATERIAL);
            Gl.glColorMaterial(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT_AND_DIFFUSE);
            Gl.glShadeModel(Gl.GL_SMOOTH);
            float x = ((float)OptionAmbientColor.R + (float)OptionAmbientColor.G + (float)OptionAmbientColor.B);
            float y = ((float)OptionDiffuseColor.R + (float)OptionDiffuseColor.G + (float)OptionDiffuseColor.B);
            if (x < y) x = y;
            OptionLightingResultingAmount = 0.00208333333333333f * x;
            if (OptionLightingResultingAmount > 1.0f) OptionLightingResultingAmount = 1.0f;
            Gl.glEnable(Gl.GL_LIGHTING); LightingEnabled = true;
            Gl.glDepthFunc(Gl.GL_LEQUAL);
        }

        // reset opengl state
        private static void ResetOpenGlState()
        {
            LastBoundTexture = 0;
            Gl.glEnable(Gl.GL_CULL_FACE); CullEnabled = true;
            Gl.glDisable(Gl.GL_LIGHTING); LightingEnabled = false;
            Gl.glDisable(Gl.GL_TEXTURE_2D); TexturingEnabled = false;
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glDisable(Gl.GL_BLEND); BlendEnabled = false;
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glDepthMask(Gl.GL_TRUE);
            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_EMISSION, new float[] { 0.0f, 0.0f, 0.0f, 1.0f }); EmissiveEnabled = false;
            SetAlphaFunc(Gl.GL_GREATER, 0.9f);
        }

        // render scene
        internal static byte[] PixelBuffer = null;
        internal static int PixelBufferOpenGlTextureIndex = 0;
        internal static void RenderScene(double TimeElapsed)
        {
            // initialize
            ResetOpenGlState();
            int OpenGlTextureIndex = 0;
            if (World.CurrentBackground.Texture >= 0)
            {
                OpenGlTextureIndex = TextureManager.UseTexture(World.CurrentBackground.Texture, TextureManager.UseMode.Normal);
            }
            if (OptionWireframe | OpenGlTextureIndex == 0)
            {
                if (Game.CurrentFog.Start < Game.CurrentFog.End)
                {
                    const float fogdistance = 600.0f;
                    float n = (fogdistance - Game.CurrentFog.Start) / (Game.CurrentFog.End - Game.CurrentFog.Start);
                    float cr = n * inv255 * (float)Game.CurrentFog.Color.R;
                    float cg = n * inv255 * (float)Game.CurrentFog.Color.G;
                    float cb = n * inv255 * (float)Game.CurrentFog.Color.B;
                    Gl.glClearColor(cr, cg, cb, 1.0f);
                }
                else
                {
                    Gl.glClearColor(0.0f, 0.0f, 0.0f, 1.0f);
                }
                Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            }
            else
            {
                Gl.glClear(Gl.GL_DEPTH_BUFFER_BIT);
            }
            Gl.glPushMatrix();
            MainLoop.UpdateViewport(MainLoop.ViewPortChangeMode.ChangeToScenery);
            if (LoadTexturesImmediately == LoadTextureImmediatelyMode.NotYet)
            {
                LoadTexturesImmediately = LoadTextureImmediatelyMode.Yes;
            }
            // setup camera
            double cx = World.AbsoluteCameraPosition.X;
            double cy = World.AbsoluteCameraPosition.Y;
            double cz = World.AbsoluteCameraPosition.Z;
            double dx = World.AbsoluteCameraDirection.X;
            double dy = World.AbsoluteCameraDirection.Y;
            double dz = World.AbsoluteCameraDirection.Z;
            double ux = World.AbsoluteCameraUp.X;
            double uy = World.AbsoluteCameraUp.Y;
            double uz = World.AbsoluteCameraUp.Z;
            Glu.gluLookAt(0.0, 0.0, 0.0, dx, dy, dz, ux, uy, uz);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, new float[] { OptionLightPosition.X, OptionLightPosition.Y, OptionLightPosition.Z, 0.0f });
            // fog
            double fd = Game.NextFog.TrackPosition - Game.PreviousFog.TrackPosition;
            if (fd != 0.0)
            {
                float fr = (float)((World.CameraTrackFollower.TrackPosition - Game.PreviousFog.TrackPosition) / fd);
                float frc = 1.0f - fr;
                Game.CurrentFog.Start = Game.PreviousFog.Start * frc + Game.NextFog.Start * fr;
                Game.CurrentFog.End = Game.PreviousFog.End * frc + Game.NextFog.End * fr;
                Game.CurrentFog.Color.R = (byte)((float)Game.PreviousFog.Color.R * frc + (float)Game.NextFog.Color.R * fr);
                Game.CurrentFog.Color.G = (byte)((float)Game.PreviousFog.Color.G * frc + (float)Game.NextFog.Color.G * fr);
                Game.CurrentFog.Color.B = (byte)((float)Game.PreviousFog.Color.B * frc + (float)Game.NextFog.Color.B * fr);
            }
            else
            {
                Game.CurrentFog = Game.PreviousFog;
            }
            // render background
            if (FogEnabled)
            {
                Gl.glDisable(Gl.GL_FOG); FogEnabled = false;
            }
            Gl.glDisable(Gl.GL_DEPTH_TEST);
            RenderBackground(dx, dy, dz, TimeElapsed);
            // fog
            double aa = Game.CurrentFog.Start;
            double bb = Game.CurrentFog.End;
            if (Game.CurrentFog.Start < Game.CurrentFog.End & Game.CurrentFog.Start < World.BackgroundImageDistance)
            {
                if (!FogEnabled)
                {
                    Gl.glFogi(Gl.GL_FOG_MODE, Gl.GL_LINEAR);
                }
                Gl.glFogf(Gl.GL_FOG_START, Game.CurrentFog.Start);
                Gl.glFogf(Gl.GL_FOG_END, Game.CurrentFog.End);
                Gl.glFogfv(Gl.GL_FOG_COLOR, new float[] { inv255 * (float)Game.CurrentFog.Color.R, inv255 * (float)Game.CurrentFog.Color.G, inv255 * (float)Game.CurrentFog.Color.B, 1.0f });
                if (!FogEnabled)
                {
                    Gl.glEnable(Gl.GL_FOG); FogEnabled = true;
                }
            }
            else if (FogEnabled)
            {
                Gl.glDisable(Gl.GL_FOG); FogEnabled = false;
            }
            // world layer
            bool optionLighting = OptionLighting;
            LastBoundTexture = 0;
            if (OptionLighting)
            {
                if (!LightingEnabled)
                {
                    Gl.glEnable(Gl.GL_LIGHTING); LightingEnabled = true;
                }
                if (World.CameraRestriction == World.CameraRestrictionMode.NotAvailable)
                {
                    Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_AMBIENT, new float[] { inv255 * (float)OptionAmbientColor.R, inv255 * (float)OptionAmbientColor.G, inv255 * (float)OptionAmbientColor.B, 1.0f });
                    Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, new float[] { inv255 * (float)OptionDiffuseColor.R, inv255 * (float)OptionDiffuseColor.G, inv255 * (float)OptionDiffuseColor.B, 1.0f });
                }
            }
            else if (LightingEnabled)
            {
                Gl.glDisable(Gl.GL_LIGHTING); LightingEnabled = false;
            }
            // static opaque
            if (Interface.CurrentOptions.DisableDisplayLists)
            {
                ResetOpenGlState();
                for (int i = 0; i < StaticOpaque.Length; i++)
                {
                    if (StaticOpaque[i] != null)
                    {
                        if (StaticOpaque[i].List != null)
                        {
                            for (int j = 0; j < StaticOpaque[i].List.FaceCount; j++)
                            {
                                if (StaticOpaque[i].List.Faces[j] != null)
                                {
                                    RenderFace(ref StaticOpaque[i].List.Faces[j], cx, cy, cz);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < StaticOpaque.Length; i++)
                {
                    if (StaticOpaque[i] != null)
                    {
                        if (StaticOpaque[i].Update | StaticOpaqueForceUpdate)
                        {
                            StaticOpaque[i].Update = false;
                            if (StaticOpaque[i].OpenGlDisplayListAvailable)
                            {
                                Gl.glDeleteLists(StaticOpaque[i].OpenGlDisplayList, 1);
                                StaticOpaque[i].OpenGlDisplayListAvailable = false;
                            }
                            if (StaticOpaque[i].List.FaceCount != 0)
                            {
                                StaticOpaque[i].OpenGlDisplayList = Gl.glGenLists(1);
                                StaticOpaque[i].OpenGlDisplayListAvailable = true;
                                ResetOpenGlState();
                                Gl.glNewList(StaticOpaque[i].OpenGlDisplayList, Gl.GL_COMPILE);
                                for (int j = 0; j < StaticOpaque[i].List.FaceCount; j++)
                                {
                                    if (StaticOpaque[i].List.Faces[j] != null)
                                    {
                                        RenderFace(ref StaticOpaque[i].List.Faces[j], cx, cy, cz);
                                    }
                                }
                                Gl.glEndList();
                            }
                            StaticOpaque[i].WorldPosition = World.AbsoluteCameraPosition;
                        }
                    }
                }
                StaticOpaqueForceUpdate = false;
                for (int i = 0; i < StaticOpaque.Length; i++)
                {
                    if (StaticOpaque[i] != null)
                    {
                        if (StaticOpaque[i].OpenGlDisplayListAvailable)
                        {
                            ResetOpenGlState();
                            Gl.glPushMatrix();
                            Gl.glTranslated(StaticOpaque[i].WorldPosition.X - World.AbsoluteCameraPosition.X, StaticOpaque[i].WorldPosition.Y - World.AbsoluteCameraPosition.Y, StaticOpaque[i].WorldPosition.Z - World.AbsoluteCameraPosition.Z);
                            Gl.glCallList(StaticOpaque[i].OpenGlDisplayList);
                            Gl.glPopMatrix();
                        }
                    }
                }
            }
            // dynamic opaque
            ResetOpenGlState();
            for (int i = 0; i < DynamicOpaque.FaceCount; i++)
            {
                RenderFace(ref DynamicOpaque.Faces[i], cx, cy, cz);
            }
            // dynamic alpha
            ResetOpenGlState();
            SortPolygons(DynamicAlpha);
            if (Interface.CurrentOptions.TransparencyMode == TransparencyMode.Performance)
            {
                Gl.glEnable(Gl.GL_BLEND); BlendEnabled = true;
                Gl.glDepthMask(Gl.GL_FALSE);
                SetAlphaFunc(Gl.GL_GREATER, 0.0f);
                for (int i = 0; i < DynamicAlpha.FaceCount; i++)
                {
                    RenderFace(ref DynamicAlpha.Faces[i], cx, cy, cz);
                }
            }
            else
            {
                Gl.glDisable(Gl.GL_BLEND); BlendEnabled = false;
                SetAlphaFunc(Gl.GL_EQUAL, 1.0f);
                Gl.glDepthMask(Gl.GL_TRUE);
                for (int i = 0; i < DynamicAlpha.FaceCount; i++)
                {
                    int r = (int)ObjectManager.Objects[DynamicAlpha.Faces[i].ObjectIndex].Mesh.Faces[DynamicAlpha.Faces[i].FaceIndex].Material;
                    if (ObjectManager.Objects[DynamicAlpha.Faces[i].ObjectIndex].Mesh.Materials[r].BlendMode == World.MeshMaterialBlendMode.Normal & ObjectManager.Objects[DynamicAlpha.Faces[i].ObjectIndex].Mesh.Materials[r].GlowAttenuationData == 0)
                    {
                        if (ObjectManager.Objects[DynamicAlpha.Faces[i].ObjectIndex].Mesh.Materials[r].Color.A == 255)
                        {
                            RenderFace(ref DynamicAlpha.Faces[i], cx, cy, cz);
                        }
                    }
                }
                Gl.glEnable(Gl.GL_BLEND); BlendEnabled = true;
                SetAlphaFunc(Gl.GL_LESS, 1.0f);
                Gl.glDepthMask(Gl.GL_FALSE);
                bool additive = false;
                for (int i = 0; i < DynamicAlpha.FaceCount; i++)
                {
                    int r = (int)ObjectManager.Objects[DynamicAlpha.Faces[i].ObjectIndex].Mesh.Faces[DynamicAlpha.Faces[i].FaceIndex].Material;
                    if (ObjectManager.Objects[DynamicAlpha.Faces[i].ObjectIndex].Mesh.Materials[r].BlendMode == World.MeshMaterialBlendMode.Additive)
                    {
                        if (!additive)
                        {
                            UnsetAlphaFunc();
                            additive = true;
                        }
                        RenderFace(ref DynamicAlpha.Faces[i], cx, cy, cz);
                    }
                    else
                    {
                        if (additive)
                        {
                            SetAlphaFunc(Gl.GL_LESS, 1.0f);
                            additive = false;
                        }
                        RenderFace(ref DynamicAlpha.Faces[i], cx, cy, cz);
                    }
                }
            }
            // motion blur
            Gl.glDisable(Gl.GL_DEPTH_TEST);
            Gl.glDepthMask(Gl.GL_FALSE);
            SetAlphaFunc(Gl.GL_GREATER, 0.0f);
            if (Interface.CurrentOptions.MotionBlur != Interface.MotionBlurMode.None)
            {
                if (LightingEnabled)
                {
                    Gl.glDisable(Gl.GL_LIGHTING);
                    LightingEnabled = false;
                }
                RenderFullscreenMotionBlur();
            }
            // overlay layer
            if (FogEnabled)
            {
                Gl.glDisable(Gl.GL_FOG); FogEnabled = false;
            }
            Gl.glLoadIdentity();
            MainLoop.UpdateViewport(MainLoop.ViewPortChangeMode.ChangeToCab);
            Glu.gluLookAt(0.0, 0.0, 0.0, dx, dy, dz, ux, uy, uz);
            if (World.CameraRestriction == World.CameraRestrictionMode.NotAvailable)
            {
                // 3d cab
                Gl.glDepthMask(Gl.GL_TRUE);
                Gl.glEnable(Gl.GL_DEPTH_TEST);
                Gl.glClear(Gl.GL_DEPTH_BUFFER_BIT);
                if (!LightingEnabled)
                {
                    Gl.glEnable(Gl.GL_LIGHTING); LightingEnabled = true;
                }
                OptionLighting = true;
                Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_AMBIENT, new float[] { 0.6f, 0.6f, 0.6f, 1.0f });
                Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, new float[] { 0.6f, 0.6f, 0.6f, 1.0f });
                // overlay opaque
                SetAlphaFunc(Gl.GL_GREATER, 0.9f);
                for (int i = 0; i < OverlayOpaque.FaceCount; i++)
                {
                    RenderFace(ref OverlayOpaque.Faces[i], cx, cy, cz);
                }
                // overlay alpha
                SortPolygons(OverlayAlpha);
                if (Interface.CurrentOptions.TransparencyMode == TransparencyMode.Performance)
                {
                    Gl.glEnable(Gl.GL_BLEND); BlendEnabled = true;
                    Gl.glDepthMask(Gl.GL_FALSE);
                    SetAlphaFunc(Gl.GL_GREATER, 0.0f);
                    for (int i = 0; i < OverlayAlpha.FaceCount; i++)
                    {
                        RenderFace(ref OverlayAlpha.Faces[i], cx, cy, cz);
                    }
                }
                else
                {
                    Gl.glDisable(Gl.GL_BLEND); BlendEnabled = false;
                    SetAlphaFunc(Gl.GL_EQUAL, 1.0f);
                    Gl.glDepthMask(Gl.GL_TRUE);
                    for (int i = 0; i < OverlayAlpha.FaceCount; i++)
                    {
                        int r = (int)ObjectManager.Objects[OverlayAlpha.Faces[i].ObjectIndex].Mesh.Faces[OverlayAlpha.Faces[i].FaceIndex].Material;
                        if (ObjectManager.Objects[OverlayAlpha.Faces[i].ObjectIndex].Mesh.Materials[r].BlendMode == World.MeshMaterialBlendMode.Normal & ObjectManager.Objects[OverlayAlpha.Faces[i].ObjectIndex].Mesh.Materials[r].GlowAttenuationData == 0)
                        {
                            if (ObjectManager.Objects[OverlayAlpha.Faces[i].ObjectIndex].Mesh.Materials[r].Color.A == 255)
                            {
                                RenderFace(ref OverlayAlpha.Faces[i], cx, cy, cz);
                            }
                        }
                    }
                    Gl.glEnable(Gl.GL_BLEND); BlendEnabled = true;
                    SetAlphaFunc(Gl.GL_LESS, 1.0f);
                    Gl.glDepthMask(Gl.GL_FALSE);
                    bool additive = false;
                    for (int i = 0; i < OverlayAlpha.FaceCount; i++)
                    {
                        int r = (int)ObjectManager.Objects[OverlayAlpha.Faces[i].ObjectIndex].Mesh.Faces[OverlayAlpha.Faces[i].FaceIndex].Material;
                        if (ObjectManager.Objects[OverlayAlpha.Faces[i].ObjectIndex].Mesh.Materials[r].BlendMode == World.MeshMaterialBlendMode.Additive)
                        {
                            if (!additive)
                            {
                                UnsetAlphaFunc();
                                additive = true;
                            }
                            RenderFace(ref OverlayAlpha.Faces[i], cx, cy, cz);
                        }
                        else
                        {
                            if (additive)
                            {
                                SetAlphaFunc(Gl.GL_LESS, 1.0f);
                                additive = false;
                            }
                            RenderFace(ref OverlayAlpha.Faces[i], cx, cy, cz);
                        }
                    }
                }
            }
            else
            {
                // not a 3d cab
                if (LightingEnabled)
                {
                    Gl.glDisable(Gl.GL_LIGHTING); LightingEnabled = true;
                }
                OptionLighting = false;
                if (!BlendEnabled)
                {
                    Gl.glEnable(Gl.GL_BLEND); BlendEnabled = true;
                }
                Gl.glDepthMask(Gl.GL_FALSE);
                Gl.glDisable(Gl.GL_DEPTH_TEST);
                UnsetAlphaFunc();
                SortPolygons(OverlayAlpha);
                for (int i = 0; i < OverlayAlpha.FaceCount; i++)
                {
                    RenderFace(ref OverlayAlpha.Faces[i], cx, cy, cz);
                }
            }
            // render overlays
            OptionLighting = optionLighting;
            if (LightingEnabled)
            {
                Gl.glDisable(Gl.GL_LIGHTING); LightingEnabled = false;
            }
            if (FogEnabled)
            {
                Gl.glDisable(Gl.GL_FOG); FogEnabled = false;
            }
            if (BlendEnabled)
            {
                Gl.glDisable(Gl.GL_BLEND); BlendEnabled = false;
            }
            UnsetAlphaFunc();
            Gl.glDisable(Gl.GL_DEPTH_TEST);
            RenderOverlays(TimeElapsed);
            // finalize rendering
            Gl.glPopMatrix();
            LoadTexturesImmediately = LoadTextureImmediatelyMode.NoLonger;
        }

        // set alpha func
        private static void SetAlphaFunc(int Comparison, float Value)
        {
            AlphaTestEnabled = true;
            AlphaFuncComparison = Comparison;
            AlphaFuncValue = Value;
            Gl.glAlphaFunc(Comparison, Value);
            Gl.glEnable(Gl.GL_ALPHA_TEST);
        }
        private static void UnsetAlphaFunc()
        {
            AlphaTestEnabled = false;
            Gl.glDisable(Gl.GL_ALPHA_TEST);
        }
        private static void RestoreAlphaFunc()
        {
            if (AlphaTestEnabled)
            {
                Gl.glAlphaFunc(AlphaFuncComparison, AlphaFuncValue);
                Gl.glEnable(Gl.GL_ALPHA_TEST);
            }
            else
            {
                Gl.glDisable(Gl.GL_ALPHA_TEST);
            }
        }

        // render face
        private static int LastBoundTexture = 0;
        private static void RenderFace(ref ObjectFace Face, double CameraX, double CameraY, double CameraZ)
        {
            if (CullEnabled)
            {
                if (!OptionBackfaceCulling || (ObjectManager.Objects[Face.ObjectIndex].Mesh.Faces[Face.FaceIndex].Flags & World.MeshFace.Face2Mask) != 0)
                {
                    Gl.glDisable(Gl.GL_CULL_FACE);
                    CullEnabled = false;
                }
            }
            else if (OptionBackfaceCulling)
            {
                if ((ObjectManager.Objects[Face.ObjectIndex].Mesh.Faces[Face.FaceIndex].Flags & World.MeshFace.Face2Mask) == 0)
                {
                    Gl.glEnable(Gl.GL_CULL_FACE);
                    CullEnabled = true;
                }
            }
            int r = (int)ObjectManager.Objects[Face.ObjectIndex].Mesh.Faces[Face.FaceIndex].Material;
            RenderFace(ref ObjectManager.Objects[Face.ObjectIndex].Mesh.Materials[r], ObjectManager.Objects[Face.ObjectIndex].Mesh.Vertices, ref ObjectManager.Objects[Face.ObjectIndex].Mesh.Faces[Face.FaceIndex], CameraX, CameraY, CameraZ);
        }
        private static void RenderFace(ref World.MeshMaterial Material, World.Vertex[] Vertices, ref World.MeshFace Face, double CameraX, double CameraY, double CameraZ)
        {
            // texture
            int OpenGlDaytimeTextureIndex = Material.DaytimeTextureIndex >= 0 ? TextureManager.UseTexture(Material.DaytimeTextureIndex, TextureManager.UseMode.Normal) : 0;
            int OpenGlNighttimeTextureIndex = Material.NighttimeTextureIndex >= 0 ? TextureManager.UseTexture(Material.NighttimeTextureIndex, TextureManager.UseMode.Normal) : 0;
            if (OpenGlDaytimeTextureIndex != 0)
            {
                if (!TexturingEnabled)
                {
                    Gl.glEnable(Gl.GL_TEXTURE_2D);
                    TexturingEnabled = true;
                }
                if (OpenGlDaytimeTextureIndex != LastBoundTexture)
                {
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, OpenGlDaytimeTextureIndex);
                    LastBoundTexture = OpenGlDaytimeTextureIndex;
                }
            }
            else
            {
                if (TexturingEnabled)
                {
                    Gl.glDisable(Gl.GL_TEXTURE_2D);
                    TexturingEnabled = false;
                    LastBoundTexture = 0;
                }
            }
            // blend mode
            float factor;
            if (Material.BlendMode == World.MeshMaterialBlendMode.Additive)
            {
                factor = 1.0f;
                if (!BlendEnabled) Gl.glEnable(Gl.GL_BLEND);
                Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE);
                if (FogEnabled)
                {
                    Gl.glDisable(Gl.GL_FOG);
                }
            }
            else if (OpenGlNighttimeTextureIndex == 0)
            {
                float blend = inv255 * (float)Material.DaytimeNighttimeBlend + 1.0f - OptionLightingResultingAmount;
                if (blend > 1.0f) blend = 1.0f;
                factor = 1.0f - 0.8f * blend;
            }
            else
            {
                factor = 1.0f;
            }
            if (OpenGlNighttimeTextureIndex != 0)
            {
                if (LightingEnabled)
                {
                    Gl.glDisable(Gl.GL_LIGHTING);
                    LightingEnabled = false;
                }
            }
            else
            {
                if (OptionLighting & !LightingEnabled)
                {
                    Gl.glEnable(Gl.GL_LIGHTING);
                    LightingEnabled = true;
                }
            }
            // render daytime polygon
            int FaceType = Face.Flags & World.MeshFace.FaceTypeMask;
            switch (FaceType)
            {
                case World.MeshFace.FaceTypeTriangles:
                    Gl.glBegin(Gl.GL_TRIANGLES);
                    break;
                case World.MeshFace.FaceTypeTriangleStrip:
                    Gl.glBegin(Gl.GL_TRIANGLE_STRIP);
                    break;
                case World.MeshFace.FaceTypeQuads:
                    Gl.glBegin(Gl.GL_QUADS);
                    break;
                case World.MeshFace.FaceTypeQuadStrip:
                    Gl.glBegin(Gl.GL_QUAD_STRIP);
                    break;
                default:
                    Gl.glBegin(Gl.GL_POLYGON);
                    break;
            }
            if (Material.GlowAttenuationData != 0)
            {
                float alphafactor = (float)GetDistanceFactor(Vertices, ref Face, Material.GlowAttenuationData, CameraX, CameraY, CameraZ);
                Gl.glColor4f(inv255 * (float)Material.Color.R * factor, inv255 * Material.Color.G * factor, inv255 * (float)Material.Color.B * factor, inv255 * (float)Material.Color.A * alphafactor);
            }
            else
            {
                Gl.glColor4f(inv255 * (float)Material.Color.R * factor, inv255 * Material.Color.G * factor, inv255 * (float)Material.Color.B * factor, inv255 * (float)Material.Color.A);
            }
            if ((Material.Flags & World.MeshMaterial.EmissiveColorMask) != 0)
            {
                Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_EMISSION, new float[] { inv255 * (float)Material.EmissiveColor.R, inv255 * (float)Material.EmissiveColor.G, inv255 * (float)Material.EmissiveColor.B, 1.0f });
                EmissiveEnabled = true;
            }
            else if (EmissiveEnabled)
            {
                Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_EMISSION, new float[] { 0.0f, 0.0f, 0.0f, 1.0f });
                EmissiveEnabled = false;
            }
            if (OpenGlDaytimeTextureIndex != 0)
            {
                if (LightingEnabled)
                {
                    for (int j = 0; j < Face.Vertices.Length; j++)
                    {
                        Gl.glNormal3f(Face.Vertices[j].Normal.X, Face.Vertices[j].Normal.Y, Face.Vertices[j].Normal.Z);
                        Gl.glTexCoord2f(Vertices[Face.Vertices[j].Index].TextureCoordinates.X, Vertices[Face.Vertices[j].Index].TextureCoordinates.Y);
                        Gl.glVertex3f((float)(Vertices[Face.Vertices[j].Index].Coordinates.X - CameraX), (float)(Vertices[Face.Vertices[j].Index].Coordinates.Y - CameraY), (float)(Vertices[Face.Vertices[j].Index].Coordinates.Z - CameraZ));
                    }
                }
                else
                {
                    for (int j = 0; j < Face.Vertices.Length; j++)
                    {
                        Gl.glTexCoord2f(Vertices[Face.Vertices[j].Index].TextureCoordinates.X, Vertices[Face.Vertices[j].Index].TextureCoordinates.Y);
                        Gl.glVertex3f((float)(Vertices[Face.Vertices[j].Index].Coordinates.X - CameraX), (float)(Vertices[Face.Vertices[j].Index].Coordinates.Y - CameraY), (float)(Vertices[Face.Vertices[j].Index].Coordinates.Z - CameraZ));
                    }
                }
            }
            else
            {
                if (LightingEnabled)
                {
                    for (int j = 0; j < Face.Vertices.Length; j++)
                    {
                        Gl.glNormal3f(Face.Vertices[j].Normal.X, Face.Vertices[j].Normal.Y, Face.Vertices[j].Normal.Z);
                        Gl.glVertex3f((float)(Vertices[Face.Vertices[j].Index].Coordinates.X - CameraX), (float)(Vertices[Face.Vertices[j].Index].Coordinates.Y - CameraY), (float)(Vertices[Face.Vertices[j].Index].Coordinates.Z - CameraZ));
                    }
                }
                else
                {
                    for (int j = 0; j < Face.Vertices.Length; j++)
                    {
                        Gl.glVertex3f((float)(Vertices[Face.Vertices[j].Index].Coordinates.X - CameraX), (float)(Vertices[Face.Vertices[j].Index].Coordinates.Y - CameraY), (float)(Vertices[Face.Vertices[j].Index].Coordinates.Z - CameraZ));
                    }
                }
            }
            Gl.glEnd();
            // render nighttime polygon
            if (OpenGlNighttimeTextureIndex != 0)
            {
                if (!TexturingEnabled)
                {
                    Gl.glEnable(Gl.GL_TEXTURE_2D);
                    TexturingEnabled = true;
                }
                if (!BlendEnabled)
                {
                    Gl.glEnable(Gl.GL_BLEND);
                }
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, OpenGlNighttimeTextureIndex);
                LastBoundTexture = 0;
                Gl.glAlphaFunc(Gl.GL_GREATER, 0.0f);
                Gl.glEnable(Gl.GL_ALPHA_TEST);
                switch (FaceType)
                {
                    case World.MeshFace.FaceTypeTriangles:
                        Gl.glBegin(Gl.GL_TRIANGLES);
                        break;
                    case World.MeshFace.FaceTypeTriangleStrip:
                        Gl.glBegin(Gl.GL_TRIANGLE_STRIP);
                        break;
                    case World.MeshFace.FaceTypeQuads:
                        Gl.glBegin(Gl.GL_QUADS);
                        break;
                    case World.MeshFace.FaceTypeQuadStrip:
                        Gl.glBegin(Gl.GL_QUAD_STRIP);
                        break;
                    default:
                        Gl.glBegin(Gl.GL_POLYGON);
                        break;
                }
                float alphafactor;
                if (Material.GlowAttenuationData != 0)
                {
                    alphafactor = (float)GetDistanceFactor(Vertices, ref Face, Material.GlowAttenuationData, CameraX, CameraY, CameraZ);
                    float blend = inv255 * (float)Material.DaytimeNighttimeBlend + 1.0f - OptionLightingResultingAmount;
                    if (blend > 1.0f) blend = 1.0f;
                    alphafactor *= blend;
                }
                else
                {
                    alphafactor = inv255 * (float)Material.DaytimeNighttimeBlend + 1.0f - OptionLightingResultingAmount;
                    if (alphafactor > 1.0f) alphafactor = 1.0f;
                }
                Gl.glColor4f(inv255 * (float)Material.Color.R * factor, inv255 * Material.Color.G * factor, inv255 * (float)Material.Color.B * factor, inv255 * (float)Material.Color.A * alphafactor);
                if ((Material.Flags & World.MeshMaterial.EmissiveColorMask) != 0)
                {
                    Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_EMISSION, new float[] { inv255 * (float)Material.EmissiveColor.R, inv255 * (float)Material.EmissiveColor.G, inv255 * (float)Material.EmissiveColor.B, 1.0f });
                    EmissiveEnabled = true;
                }
                else if (EmissiveEnabled)
                {
                    Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_EMISSION, new float[] { 0.0f, 0.0f, 0.0f, 1.0f });
                    EmissiveEnabled = false;
                }
                for (int j = 0; j < Face.Vertices.Length; j++)
                {
                    Gl.glTexCoord2f(Vertices[Face.Vertices[j].Index].TextureCoordinates.X, Vertices[Face.Vertices[j].Index].TextureCoordinates.Y);
                    Gl.glVertex3f((float)(Vertices[Face.Vertices[j].Index].Coordinates.X - CameraX), (float)(Vertices[Face.Vertices[j].Index].Coordinates.Y - CameraY), (float)(Vertices[Face.Vertices[j].Index].Coordinates.Z - CameraZ));
                }
                Gl.glEnd();
                RestoreAlphaFunc();
                if (!BlendEnabled)
                {
                    Gl.glDisable(Gl.GL_BLEND);
                }
            }
            // normals
            if (OptionNormals)
            {
                if (TexturingEnabled)
                {
                    Gl.glDisable(Gl.GL_TEXTURE_2D);
                    TexturingEnabled = false;
                }
                for (int j = 0; j < Face.Vertices.Length; j++)
                {
                    Gl.glBegin(Gl.GL_LINES);
                    Gl.glColor4f(inv255 * (float)Material.Color.R, inv255 * (float)Material.Color.G, inv255 * (float)Material.Color.B, 1.0f);
                    Gl.glVertex3f((float)(Vertices[Face.Vertices[j].Index].Coordinates.X - CameraX), (float)(Vertices[Face.Vertices[j].Index].Coordinates.Y - CameraY), (float)(Vertices[Face.Vertices[j].Index].Coordinates.Z - CameraZ));
                    Gl.glVertex3f((float)(Vertices[Face.Vertices[j].Index].Coordinates.X + Face.Vertices[j].Normal.X - CameraX), (float)(Vertices[Face.Vertices[j].Index].Coordinates.Y + Face.Vertices[j].Normal.Y - CameraY), (float)(Vertices[Face.Vertices[j].Index].Coordinates.Z + Face.Vertices[j].Normal.Z - CameraZ));
                    Gl.glEnd();
                }
            }
            // finalize
            if (Material.BlendMode == World.MeshMaterialBlendMode.Additive)
            {
                Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
                if (!BlendEnabled) Gl.glDisable(Gl.GL_BLEND);
                if (FogEnabled)
                {
                    Gl.glEnable(Gl.GL_FOG);
                }
            }
        }

        // render background
        private static void RenderBackground(double dx, double dy, double dz, double TimeElapsed)
        {
            const float scale = 0.5f;
            // fog
            const float fogdistance = 600.0f;
            if (Game.CurrentFog.Start < Game.CurrentFog.End & Game.CurrentFog.Start < fogdistance)
            {
                float cr = inv255 * (float)Game.CurrentFog.Color.R;
                float cg = inv255 * (float)Game.CurrentFog.Color.G;
                float cb = inv255 * (float)Game.CurrentFog.Color.B;
                if (!FogEnabled)
                {
                    Gl.glFogi(Gl.GL_FOG_MODE, Gl.GL_LINEAR);
                }
                float ratio = (float)World.BackgroundImageDistance / fogdistance;
                Gl.glFogf(Gl.GL_FOG_START, Game.CurrentFog.Start * ratio * scale);
                Gl.glFogf(Gl.GL_FOG_END, Game.CurrentFog.End * ratio * scale);
                Gl.glFogfv(Gl.GL_FOG_COLOR, new float[] { cr, cg, cb, 1.0f });
                if (!FogEnabled)
                {
                    Gl.glEnable(Gl.GL_FOG); FogEnabled = true;
                }
            }
            else if (FogEnabled)
            {
                Gl.glDisable(Gl.GL_FOG); FogEnabled = false;
            }
            // render
            if (World.TargetBackgroundCountdown >= 0.0)
            {
                // fade
                World.TargetBackgroundCountdown -= TimeElapsed;
                if (World.TargetBackgroundCountdown < 0.0)
                {
                    World.CurrentBackground = World.TargetBackground;
                    World.TargetBackgroundCountdown = -1.0;
                    RenderBackground(World.CurrentBackground, dx, dy, dz, 1.0f, scale);
                }
                else
                {
                    RenderBackground(World.CurrentBackground, dx, dy, dz, 1.0f, scale);
                    SetAlphaFunc(Gl.GL_GREATER, 0.0f); // ###
                    float Alpha = (float)(1.0 - World.TargetBackgroundCountdown / World.TargetBackgroundDefaultCountdown);
                    RenderBackground(World.TargetBackground, dx, dy, dz, Alpha, scale);
                }
            }
            else
            {
                // single
                RenderBackground(World.CurrentBackground, dx, dy, dz, 1.0f, scale);
            }
        }
        private static void RenderBackground(World.Background Data, double dx, double dy, double dz, float Alpha, float scale)
        {
            if (Data.Texture >= 0)
            {
                int OpenGlTextureIndex = TextureManager.UseTexture(Data.Texture, TextureManager.UseMode.LoadImmediately);
                if (OpenGlTextureIndex > 0)
                {
                    if (LightingEnabled)
                    {
                        Gl.glDisable(Gl.GL_LIGHTING);
                        LightingEnabled = false;
                    }
                    if (!TexturingEnabled)
                    {
                        Gl.glEnable(Gl.GL_TEXTURE_2D);
                        TexturingEnabled = true;
                    }
                    if (Alpha == 1.0f)
                    {
                        if (BlendEnabled)
                        {
                            Gl.glDisable(Gl.GL_BLEND);
                            BlendEnabled = false;
                        }
                    }
                    else if (!BlendEnabled)
                    {
                        Gl.glEnable(Gl.GL_BLEND);
                        BlendEnabled = true;
                    }
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, OpenGlTextureIndex);
                    Gl.glColor4f(1.0f, 1.0f, 1.0f, Alpha);
                    float y0, y1;
                    if (Data.KeepAspectRatio)
                    {
                        int tw = TextureManager.Textures[Data.Texture].Width;
                        int th = TextureManager.Textures[Data.Texture].Height;
                        double hh = Math.PI * World.BackgroundImageDistance * (double)th / ((double)tw * (double)Data.Repetition);
                        y0 = (float)(-0.5 * hh);
                        y1 = (float)(1.5 * hh);
                    }
                    else
                    {
                        y0 = (float)(-0.125 * World.BackgroundImageDistance);
                        y1 = (float)(0.375 * World.BackgroundImageDistance);
                    }
                    const int n = 32;
                    Vectors.Vector3Df[] bottom = new Vectors.Vector3Df[n];
                    Vectors.Vector3Df[] top = new Vectors.Vector3Df[n];
                    double angleValue = 2.61799387799149 - 3.14159265358979 / (double)n;
                    double angleIncrement = 6.28318530717958 / (double)n;
                    /*
					 * To ensure that the whole background cylinder is rendered inside the viewing frustum,
					 * the background is rendered before the scene with z-buffer writes disabled. Then,
					 * the actual distance from the camera is irrelevant as long as it is inside the frustum.
					 * */
                    for (int i = 0; i < n; i++)
                    {
                        float x = (float)(World.BackgroundImageDistance * Math.Cos(angleValue));
                        float z = (float)(World.BackgroundImageDistance * Math.Sin(angleValue));
                        bottom[i] = new Vectors.Vector3Df(scale * x, scale * y0, scale * z);
                        top[i] = new Vectors.Vector3Df(scale * x, scale * y1, scale * z);
                        angleValue += angleIncrement;
                    }
                    float textureStart = 0.5f * (float)Data.Repetition / (float)n;
                    float textureIncrement = -(float)Data.Repetition / (float)n;
                    double textureX = textureStart;
                    for (int i = 0; i < n; i++)
                    {
                        int j = (i + 1) % n;
                        // side wall
                        Gl.glBegin(Gl.GL_QUADS);
                        Gl.glTexCoord2d(textureX, 0.005f);
                        Gl.glVertex3f(top[i].X, top[i].Y, top[i].Z);
                        Gl.glTexCoord2d(textureX, 0.995f);
                        Gl.glVertex3f(bottom[i].X, bottom[i].Y, bottom[i].Z);
                        Gl.glTexCoord2d(textureX + textureIncrement, 0.995f);
                        Gl.glVertex3f(bottom[j].X, bottom[j].Y, bottom[j].Z);
                        Gl.glTexCoord2d(textureX + textureIncrement, 0.005f);
                        Gl.glVertex3f(top[j].X, top[j].Y, top[j].Z);
                        Gl.glEnd();
                        // top cap
                        Gl.glBegin(Gl.GL_TRIANGLES);
                        Gl.glTexCoord2d(textureX, 0.005f);
                        Gl.glVertex3f(top[i].X, top[i].Y, top[i].Z);
                        Gl.glTexCoord2d(textureX + textureIncrement, 0.005f);
                        Gl.glVertex3f(top[j].X, top[j].Y, top[j].Z);
                        Gl.glTexCoord2d(textureX + 0.5 * textureIncrement, 0.1f);
                        Gl.glVertex3f(0.0f, top[i].Y, 0.0f);
                        // bottom cap
                        Gl.glTexCoord2d(textureX + 0.5 * textureIncrement, 0.9f);
                        Gl.glVertex3f(0.0f, bottom[i].Y, 0.0f);
                        Gl.glTexCoord2d(textureX + textureIncrement, 0.995f);
                        Gl.glVertex3f(bottom[j].X, bottom[j].Y, bottom[j].Z);
                        Gl.glTexCoord2d(textureX, 0.995f);
                        Gl.glVertex3f(bottom[i].X, bottom[i].Y, bottom[i].Z);
                        Gl.glEnd();
                        // finish
                        textureX += textureIncrement;
                    }
                    Gl.glDisable(Gl.GL_TEXTURE_2D);
                    TexturingEnabled = false;
                    if (!BlendEnabled)
                    {
                        Gl.glEnable(Gl.GL_BLEND);
                        BlendEnabled = true;
                    }
                }
            }
        }

        // render fullscreen motion blur
        private static void RenderFullscreenMotionBlur()
        {
            int w = Interface.CurrentOptions.NoTextureResize ? Renderer.ScreenWidth : Interface.RoundToPowerOfTwo(ScreenWidth);
            int h = Interface.CurrentOptions.NoTextureResize ? Renderer.ScreenHeight : Interface.RoundToPowerOfTwo(ScreenHeight);
            // render
            if (PixelBufferOpenGlTextureIndex >= 0)
            {
                double strength;
                switch (Interface.CurrentOptions.MotionBlur)
                {
                    case Interface.MotionBlurMode.Low: strength = 0.0025; break;
                    case Interface.MotionBlurMode.Medium: strength = 0.0040; break;
                    case Interface.MotionBlurMode.High: strength = 0.0064; break;
                    default: strength = 0.0040; break;
                }
                double speed = Math.Abs(World.CameraSpeed);
                double denominator = strength * Game.InfoFrameRate * Math.Sqrt(speed);
                float factor;
                if (denominator > 0.001)
                {
                    factor = (float)Math.Exp(-1.0 / denominator);
                }
                else
                {
                    factor = 0.0f;
                }
                // initialize
                Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
                if (!BlendEnabled)
                {
                    Gl.glEnable(Gl.GL_BLEND);
                    BlendEnabled = true;
                }
                if (LightingEnabled)
                {
                    Gl.glDisable(Gl.GL_LIGHTING);
                    LightingEnabled = false;
                }
                Gl.glMatrixMode(Gl.GL_PROJECTION);
                Gl.glPushMatrix();
                Gl.glLoadIdentity();
                Gl.glOrtho(0.0, (double)ScreenWidth, 0.0, (double)ScreenHeight, -1.0, 1.0);
                Gl.glMatrixMode(Gl.GL_MODELVIEW);
                Gl.glPushMatrix();
                Gl.glLoadIdentity();
                if (!TexturingEnabled)
                {
                    Gl.glEnable(Gl.GL_TEXTURE_2D);
                    TexturingEnabled = true;
                }
                // render
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, PixelBufferOpenGlTextureIndex);
                Gl.glColor4f(1.0f, 1.0f, 1.0f, factor);
                Gl.glBegin(Gl.GL_POLYGON);
                Gl.glTexCoord2d(0.0, 0.0);
                Gl.glVertex2d(0.0, 0.0);
                Gl.glTexCoord2d(0.0, 1.0);
                Gl.glVertex2d(0.0, (double)h);
                Gl.glTexCoord2d(1.0, 1.0);
                Gl.glVertex2d((double)w, (double)h);
                Gl.glTexCoord2d(1.0, 0.0);
                Gl.glVertex2d((double)w, 0.0);
                Gl.glEnd();
                // finalize
                Gl.glPopMatrix();
                Gl.glMatrixMode(Gl.GL_PROJECTION);
                Gl.glPopMatrix();
                Gl.glMatrixMode(Gl.GL_MODELVIEW);
            }
            // retrieve buffer
            {
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, PixelBufferOpenGlTextureIndex);
                Gl.glCopyTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB, 0, 0, w, h, 0);
            }
        }

        // lamps
        private enum LampType
        {
            None,
            Ats, AtsOperation,
            AtsPPower, AtsPPattern, AtsPBrakeOverride, AtsPBrakeOperation, AtsP, AtsPFailure,
            Atc, AtcPower, AtcUse, AtcEmergency,
            Eb, ConstSpeed
        }
        private struct Lamp
        {
            internal LampType Type;
            internal string Text;
            internal float Width;
            internal float Height;
            internal Lamp(LampType Type)
            {
                this.Type = Type;
                switch (Type)
                {
                    case LampType.None:
                        this.Text = null;
                        break;
                    case LampType.Ats:
                        this.Text = Interface.GetInterfaceString("lamps_ats");
                        break;
                    case LampType.AtsOperation:
                        this.Text = Interface.GetInterfaceString("lamps_atsoperation");
                        break;
                    case LampType.AtsPPower:
                        this.Text = Interface.GetInterfaceString("lamps_atsppower");
                        break;
                    case LampType.AtsPPattern:
                        this.Text = Interface.GetInterfaceString("lamps_atsppattern");
                        break;
                    case LampType.AtsPBrakeOverride:
                        this.Text = Interface.GetInterfaceString("lamps_atspbrakeoverride");
                        break;
                    case LampType.AtsPBrakeOperation:
                        this.Text = Interface.GetInterfaceString("lamps_atspbrakeoperation");
                        break;
                    case LampType.AtsP:
                        this.Text = Interface.GetInterfaceString("lamps_atsp");
                        break;
                    case LampType.AtsPFailure:
                        this.Text = Interface.GetInterfaceString("lamps_atspfailure");
                        break;
                    case LampType.Atc:
                        this.Text = Interface.GetInterfaceString("lamps_atc");
                        break;
                    case LampType.AtcPower:
                        this.Text = Interface.GetInterfaceString("lamps_atcpower");
                        break;
                    case LampType.AtcUse:
                        this.Text = Interface.GetInterfaceString("lamps_atcuse");
                        break;
                    case LampType.AtcEmergency:
                        this.Text = Interface.GetInterfaceString("lamps_atcemergency");
                        break;
                    case LampType.Eb:
                        this.Text = Interface.GetInterfaceString("lamps_eb");
                        break;
                    case LampType.ConstSpeed:
                        this.Text = Interface.GetInterfaceString("lamps_constspeed");
                        break;
                    default:
                        this.Text = "TEXT";
                        break;
                }
                Fonts.FontType s = Fonts.FontType.Small;
                for (int i = 0; i < Interface.CurrentHudElements.Length; i++)
                {
                    if (Interface.CurrentHudElements[i].Subject.Equals("ats", StringComparison.OrdinalIgnoreCase))
                    {
                        s = Interface.CurrentHudElements[i].TextSize;
                        break;
                    }
                }
                MeasureString(this.Text, s, out this.Width, out this.Height);
            }
        }
        private struct LampCollection
        {
            internal Lamp[] Lamps;
            internal float Width;
        }
        private static LampCollection CurrentLampCollection;
        private static void InitializeLamps()
        {
            bool atsSn = (TrainManager.PlayerTrain.Specs.DefaultSafetySystems & TrainManager.DefaultSafetySystems.AtsSn) != 0;
            bool atsP = (TrainManager.PlayerTrain.Specs.DefaultSafetySystems & TrainManager.DefaultSafetySystems.AtsP) != 0;
            bool atc = (TrainManager.PlayerTrain.Specs.DefaultSafetySystems & TrainManager.DefaultSafetySystems.Atc) != 0;
            bool eb = (TrainManager.PlayerTrain.Specs.DefaultSafetySystems & TrainManager.DefaultSafetySystems.Eb) != 0;
            CurrentLampCollection.Width = 0.0f;
            CurrentLampCollection.Lamps = new Lamp[17];
            int Count;
            if (TrainManager.PlayerTrain.Plugin == null || !TrainManager.PlayerTrain.Plugin.IsDefault)
            {
                Count = 0;
            }
            else if (atsP & atc)
            {
                CurrentLampCollection.Lamps[0] = new Lamp(LampType.Ats);
                CurrentLampCollection.Lamps[1] = new Lamp(LampType.AtsOperation);
                CurrentLampCollection.Lamps[2] = new Lamp(LampType.None);
                CurrentLampCollection.Lamps[3] = new Lamp(LampType.AtsPPower);
                CurrentLampCollection.Lamps[4] = new Lamp(LampType.AtsPPattern);
                CurrentLampCollection.Lamps[5] = new Lamp(LampType.AtsPBrakeOverride);
                CurrentLampCollection.Lamps[6] = new Lamp(LampType.AtsPBrakeOperation);
                CurrentLampCollection.Lamps[7] = new Lamp(LampType.AtsP);
                CurrentLampCollection.Lamps[8] = new Lamp(LampType.AtsPFailure);
                CurrentLampCollection.Lamps[9] = new Lamp(LampType.None);
                CurrentLampCollection.Lamps[10] = new Lamp(LampType.Atc);
                CurrentLampCollection.Lamps[11] = new Lamp(LampType.AtcPower);
                CurrentLampCollection.Lamps[12] = new Lamp(LampType.AtcUse);
                CurrentLampCollection.Lamps[13] = new Lamp(LampType.AtcEmergency);
                Count = 14;
            }
            else if (atsP)
            {
                CurrentLampCollection.Lamps[0] = new Lamp(LampType.Ats);
                CurrentLampCollection.Lamps[1] = new Lamp(LampType.AtsOperation);
                CurrentLampCollection.Lamps[2] = new Lamp(LampType.None);
                CurrentLampCollection.Lamps[3] = new Lamp(LampType.AtsPPower);
                CurrentLampCollection.Lamps[4] = new Lamp(LampType.AtsPPattern);
                CurrentLampCollection.Lamps[5] = new Lamp(LampType.AtsPBrakeOverride);
                CurrentLampCollection.Lamps[6] = new Lamp(LampType.AtsPBrakeOperation);
                CurrentLampCollection.Lamps[7] = new Lamp(LampType.AtsP);
                CurrentLampCollection.Lamps[8] = new Lamp(LampType.AtsPFailure);
                Count = 9;
            }
            else if (atsSn & atc)
            {
                CurrentLampCollection.Lamps[0] = new Lamp(LampType.Ats);
                CurrentLampCollection.Lamps[1] = new Lamp(LampType.AtsOperation);
                CurrentLampCollection.Lamps[2] = new Lamp(LampType.None);
                CurrentLampCollection.Lamps[3] = new Lamp(LampType.Atc);
                CurrentLampCollection.Lamps[4] = new Lamp(LampType.AtcPower);
                CurrentLampCollection.Lamps[5] = new Lamp(LampType.AtcUse);
                CurrentLampCollection.Lamps[6] = new Lamp(LampType.AtcEmergency);
                Count = 7;
            }
            else if (atc)
            {
                CurrentLampCollection.Lamps[0] = new Lamp(LampType.Atc);
                CurrentLampCollection.Lamps[1] = new Lamp(LampType.AtcPower);
                CurrentLampCollection.Lamps[2] = new Lamp(LampType.AtcUse);
                CurrentLampCollection.Lamps[3] = new Lamp(LampType.AtcEmergency);
                Count = 4;
            }
            else if (atsSn)
            {
                CurrentLampCollection.Lamps[0] = new Lamp(LampType.Ats);
                CurrentLampCollection.Lamps[1] = new Lamp(LampType.AtsOperation);
                Count = 2;
            }
            else
            {
                Count = 0;
            }
            if (TrainManager.PlayerTrain.Plugin != null && TrainManager.PlayerTrain.Plugin.IsDefault)
            {
                if (Count != 0 & (eb | TrainManager.PlayerTrain.Specs.HasConstSpeed))
                {
                    CurrentLampCollection.Lamps[Count] = new Lamp(LampType.None);
                    Count++;
                }
                if (eb)
                {
                    CurrentLampCollection.Lamps[Count] = new Lamp(LampType.Eb);
                    Count++;
                }
                if (TrainManager.PlayerTrain.Specs.HasConstSpeed)
                {
                    CurrentLampCollection.Lamps[Count] = new Lamp(LampType.ConstSpeed);
                    Count++;
                }
            }
            Array.Resize<Lamp>(ref CurrentLampCollection.Lamps, Count);
            for (int i = 0; i < Count; i++)
            {
                if (CurrentLampCollection.Lamps[i].Width > CurrentLampCollection.Width)
                {
                    CurrentLampCollection.Width = CurrentLampCollection.Lamps[i].Width;
                }
            }
        }

        // render overlays
        private static void RenderOverlays(double TimeElapsed)
        {
            // initialize
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glEnable(Gl.GL_BLEND); BlendEnabled = true;
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glPushMatrix();
            Gl.glLoadIdentity();
            Gl.glOrtho(0.0, (double)ScreenWidth, 0.0, (double)ScreenHeight, -1.0, 1.0);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();
            Gl.glLoadIdentity();
            System.Globalization.CultureInfo Culture = System.Globalization.CultureInfo.InvariantCulture;
            if (CurrentOutputMode == OutputMode.Default)
            {
                // hud
                TrainManager.TrainDoorState LeftDoors = TrainManager.GetDoorsState(TrainManager.PlayerTrain, true, false);
                TrainManager.TrainDoorState RightDoors = TrainManager.GetDoorsState(TrainManager.PlayerTrain, false, true);
                for (int i = 0; i < Interface.CurrentHudElements.Length; i++)
                {
                    string Command = Interface.CurrentHudElements[i].Subject.ToLowerInvariant();
                    switch (Command)
                    {
                        case "messages":
                            {
                                // messages
                                int n = Game.Messages.Length;
                                float totalwidth = 16.0f;
                                float[] widths = new float[n];
                                float[] heights = new float[n];
                                for (int j = 0; j < n; j++)
                                {
                                    MeasureString(Game.Messages[j].DisplayText, Interface.CurrentHudElements[i].TextSize, out widths[j], out heights[j]);
                                    float a = widths[j] - j * Interface.CurrentHudElements[i].Value1;
                                    if (a > totalwidth) totalwidth = a;
                                }
                                Game.MessagesRendererSize.X += 16.0 * TimeElapsed * ((double)totalwidth - Game.MessagesRendererSize.X);
                                totalwidth = (float)Game.MessagesRendererSize.X;
                                double lcrh = 0.0;
                                /// left width/height
                                double lw = 0.0;
                                if (Interface.CurrentHudElements[i].TopLeft.BackgroundTextureIndex >= 0)
                                {
                                    int OpenGlTextureIndex = TextureManager.UseTexture(Interface.CurrentHudElements[i].TopLeft.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                    if (OpenGlTextureIndex != 0)
                                    {
                                        double u = (double)TextureManager.Textures[Interface.CurrentHudElements[i].TopLeft.BackgroundTextureIndex].ClipWidth;
                                        double v = (double)TextureManager.Textures[Interface.CurrentHudElements[i].TopLeft.BackgroundTextureIndex].ClipHeight;
                                        if (u > lw) lw = u;
                                        if (v > lcrh) lcrh = v;
                                    }
                                }
                                if (Interface.CurrentHudElements[i].CenterLeft.BackgroundTextureIndex >= 0)
                                {
                                    int OpenGlTextureIndex = TextureManager.UseTexture(Interface.CurrentHudElements[i].CenterLeft.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                    if (OpenGlTextureIndex != 0)
                                    {
                                        double u = (double)TextureManager.Textures[Interface.CurrentHudElements[i].CenterLeft.BackgroundTextureIndex].ClipWidth;
                                        double v = (double)TextureManager.Textures[Interface.CurrentHudElements[i].CenterLeft.BackgroundTextureIndex].ClipHeight;
                                        if (u > lw) lw = u;
                                        if (v > lcrh) lcrh = v;
                                    }
                                }
                                if (Interface.CurrentHudElements[i].BottomLeft.BackgroundTextureIndex >= 0)
                                {
                                    int OpenGlTextureIndex = TextureManager.UseTexture(Interface.CurrentHudElements[i].BottomLeft.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                    if (OpenGlTextureIndex != 0)
                                    {
                                        double u = (double)TextureManager.Textures[Interface.CurrentHudElements[i].BottomLeft.BackgroundTextureIndex].ClipWidth;
                                        double v = (double)TextureManager.Textures[Interface.CurrentHudElements[i].BottomLeft.BackgroundTextureIndex].ClipHeight;
                                        if (u > lw) lw = u;
                                        if (v > lcrh) lcrh = v;
                                    }
                                }
                                /// center height
                                if (Interface.CurrentHudElements[i].TopMiddle.BackgroundTextureIndex >= 0)
                                {
                                    int OpenGlTextureIndex = TextureManager.UseTexture(Interface.CurrentHudElements[i].TopMiddle.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                    if (OpenGlTextureIndex != 0)
                                    {
                                        double v = (double)TextureManager.Textures[Interface.CurrentHudElements[i].TopMiddle.BackgroundTextureIndex].ClipHeight;
                                        if (v > lcrh) lcrh = v;
                                    }
                                }
                                if (Interface.CurrentHudElements[i].CenterMiddle.BackgroundTextureIndex >= 0)
                                {
                                    int OpenGlTextureIndex = TextureManager.UseTexture(Interface.CurrentHudElements[i].CenterMiddle.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                    if (OpenGlTextureIndex != 0)
                                    {
                                        double v = (double)TextureManager.Textures[Interface.CurrentHudElements[i].CenterMiddle.BackgroundTextureIndex].ClipHeight;
                                        if (v > lcrh) lcrh = v;
                                    }
                                }
                                if (Interface.CurrentHudElements[i].BottomMiddle.BackgroundTextureIndex >= 0)
                                {
                                    int OpenGlTextureIndex = TextureManager.UseTexture(Interface.CurrentHudElements[i].BottomMiddle.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                    if (OpenGlTextureIndex != 0)
                                    {
                                        double v = (double)TextureManager.Textures[Interface.CurrentHudElements[i].BottomMiddle.BackgroundTextureIndex].ClipHeight;
                                        if (v > lcrh) lcrh = v;
                                    }
                                }
                                /// right width/height
                                double rw = 0.0;
                                if (Interface.CurrentHudElements[i].TopRight.BackgroundTextureIndex >= 0)
                                {
                                    int OpenGlTextureIndex = TextureManager.UseTexture(Interface.CurrentHudElements[i].TopRight.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                    if (OpenGlTextureIndex != 0)
                                    {
                                        double u = (double)TextureManager.Textures[Interface.CurrentHudElements[i].TopRight.BackgroundTextureIndex].ClipWidth;
                                        double v = (double)TextureManager.Textures[Interface.CurrentHudElements[i].TopRight.BackgroundTextureIndex].ClipHeight;
                                        if (u > rw) rw = u;
                                        if (v > lcrh) lcrh = v;
                                    }
                                }
                                if (Interface.CurrentHudElements[i].CenterRight.BackgroundTextureIndex >= 0)
                                {
                                    int OpenGlTextureIndex = TextureManager.UseTexture(Interface.CurrentHudElements[i].CenterRight.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                    if (OpenGlTextureIndex != 0)
                                    {
                                        double u = (double)TextureManager.Textures[Interface.CurrentHudElements[i].CenterRight.BackgroundTextureIndex].ClipWidth;
                                        double v = (double)TextureManager.Textures[Interface.CurrentHudElements[i].CenterRight.BackgroundTextureIndex].ClipHeight;
                                        if (u > rw) rw = u;
                                        if (v > lcrh) lcrh = v;
                                    }
                                }
                                if (Interface.CurrentHudElements[i].BottomRight.BackgroundTextureIndex >= 0)
                                {
                                    int OpenGlTextureIndex = TextureManager.UseTexture(Interface.CurrentHudElements[i].BottomRight.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                    if (OpenGlTextureIndex != 0)
                                    {
                                        double u = (double)TextureManager.Textures[Interface.CurrentHudElements[i].BottomRight.BackgroundTextureIndex].ClipWidth;
                                        double v = (double)TextureManager.Textures[Interface.CurrentHudElements[i].BottomRight.BackgroundTextureIndex].ClipHeight;
                                        if (u > rw) rw = u;
                                        if (v > lcrh) lcrh = v;
                                    }
                                }
                                /// start
                                double w = totalwidth + lw + rw;
                                double h = Interface.CurrentHudElements[i].Value2 * n;
                                double x = Interface.CurrentHudElements[i].Alignment.X < 0 ? 0.0 : Interface.CurrentHudElements[i].Alignment.X > 0 ? ScreenWidth - w : 0.5 * (ScreenWidth - w);
                                double y = Interface.CurrentHudElements[i].Alignment.Y < 0 ? 0.0 : Interface.CurrentHudElements[i].Alignment.Y > 0 ? ScreenHeight - h : 0.5 * (ScreenHeight - h);
                                x += Interface.CurrentHudElements[i].Position.X;
                                y += Interface.CurrentHudElements[i].Position.Y;
                                int m = 0;
                                for (int j = 0; j < n; j++)
                                {
                                    float br, bg, bb, ba;
                                    CreateBackColor(Interface.CurrentHudElements[i].BackgroundColor, Game.Messages[j].Color, out br, out bg, out bb, out ba);
                                    float tr, tg, tb, ta;
                                    CreateTextColor(Interface.CurrentHudElements[i].TextColor, Game.Messages[j].Color, out tr, out tg, out tb, out ta);
                                    float or, og, ob, oa;
                                    CreateBackColor(Interface.CurrentHudElements[i].OverlayColor, Game.Messages[j].Color, out or, out og, out ob, out oa);
                                    double tx, ty;
                                    bool preserve = false;
                                    if ((Interface.CurrentHudElements[i].Transition & Interface.HudTransition.Move) != 0)
                                    {
                                        if (Game.SecondsSinceMidnight < Game.Messages[j].Timeout)
                                        {
                                            if (Game.Messages[j].RendererAlpha == 0.0)
                                            {
                                                Game.Messages[j].RendererPosition.X = x + Interface.CurrentHudElements[i].TransitionVector.X;
                                                Game.Messages[j].RendererPosition.Y = y + Interface.CurrentHudElements[i].TransitionVector.Y;
                                                Game.Messages[j].RendererAlpha = 1.0;
                                            }
                                            tx = x;
                                            ty = y + m * (Interface.CurrentHudElements[i].Value2);
                                            preserve = true;
                                        }
                                        else if (Interface.CurrentHudElements[i].Transition == Interface.HudTransition.MoveAndFade)
                                        {
                                            tx = x;
                                            ty = y + m * (Interface.CurrentHudElements[i].Value2);
                                        }
                                        else
                                        {
                                            tx = x + Interface.CurrentHudElements[i].TransitionVector.X;
                                            ty = y + (j + 1) * Interface.CurrentHudElements[i].TransitionVector.Y;
                                        }
                                        const double speed = 2.0;
                                        double dx = (speed * Math.Abs(tx - Game.Messages[j].RendererPosition.X) + 0.1) * TimeElapsed;
                                        double dy = (speed * Math.Abs(ty - Game.Messages[j].RendererPosition.Y) + 0.1) * TimeElapsed;
                                        if (Math.Abs(tx - Game.Messages[j].RendererPosition.X) < dx)
                                        {
                                            Game.Messages[j].RendererPosition.X = tx;
                                        }
                                        else
                                        {
                                            Game.Messages[j].RendererPosition.X += Math.Sign(tx - Game.Messages[j].RendererPosition.X) * dx;
                                        }
                                        if (Math.Abs(ty - Game.Messages[j].RendererPosition.Y) < dy)
                                        {
                                            Game.Messages[j].RendererPosition.Y = ty;
                                        }
                                        else
                                        {
                                            Game.Messages[j].RendererPosition.Y += Math.Sign(ty - Game.Messages[j].RendererPosition.Y) * dy;
                                        }
                                    }
                                    else
                                    {
                                        tx = x;
                                        ty = y + m * (Interface.CurrentHudElements[i].Value2);
                                        Game.Messages[j].RendererPosition.X = 0.0;
                                        const double speed = 12.0;
                                        double dy = (speed * Math.Abs(ty - Game.Messages[j].RendererPosition.Y) + 0.1) * TimeElapsed;
                                        Game.Messages[j].RendererPosition.X = x;
                                        if (Math.Abs(ty - Game.Messages[j].RendererPosition.Y) < dy)
                                        {
                                            Game.Messages[j].RendererPosition.Y = ty;
                                        }
                                        else
                                        {
                                            Game.Messages[j].RendererPosition.Y += Math.Sign(ty - Game.Messages[j].RendererPosition.Y) * dy;
                                        }
                                    }
                                    if ((Interface.CurrentHudElements[i].Transition & Interface.HudTransition.Fade) != 0)
                                    {
                                        if (Game.SecondsSinceMidnight >= Game.Messages[j].Timeout)
                                        {
                                            Game.Messages[j].RendererAlpha -= TimeElapsed;
                                            if (Game.Messages[j].RendererAlpha < 0.0) Game.Messages[j].RendererAlpha = 0.0;
                                        }
                                        else
                                        {
                                            Game.Messages[j].RendererAlpha += TimeElapsed;
                                            if (Game.Messages[j].RendererAlpha > 1.0) Game.Messages[j].RendererAlpha = 1.0;
                                            preserve = true;
                                        }
                                    }
                                    else if (Game.SecondsSinceMidnight > Game.Messages[j].Timeout)
                                    {
                                        if (Math.Abs(Game.Messages[j].RendererPosition.X - tx) < 0.1 & Math.Abs(Game.Messages[j].RendererPosition.Y - ty) < 0.1)
                                        {
                                            Game.Messages[j].RendererAlpha = 0.0;
                                        }
                                    }
                                    if (preserve) m++;
                                    double px = Game.Messages[j].RendererPosition.X + (double)j * (double)Interface.CurrentHudElements[i].Value1;
                                    double py = Game.Messages[j].RendererPosition.Y;
                                    float alpha = (float)(Game.Messages[j].RendererAlpha * Game.Messages[j].RendererAlpha);
                                    /// graphics
                                    Interface.HudImage Left = j == 0 ? Interface.CurrentHudElements[i].TopLeft : j < n - 1 ? Interface.CurrentHudElements[i].CenterLeft : Interface.CurrentHudElements[i].BottomLeft;
                                    Interface.HudImage Middle = j == 0 ? Interface.CurrentHudElements[i].TopMiddle : j < n - 1 ? Interface.CurrentHudElements[i].CenterMiddle : Interface.CurrentHudElements[i].BottomMiddle;
                                    Interface.HudImage Right = j == 0 ? Interface.CurrentHudElements[i].TopRight : j < n - 1 ? Interface.CurrentHudElements[i].CenterRight : Interface.CurrentHudElements[i].BottomRight;
                                    /// left background
                                    if (Left.BackgroundTextureIndex >= 0)
                                    {
                                        int OpenGlTextureIndex = TextureManager.UseTexture(Left.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                        if (OpenGlTextureIndex != 0)
                                        {
                                            double u = (double)TextureManager.Textures[Left.BackgroundTextureIndex].ClipWidth;
                                            double v = (double)TextureManager.Textures[Left.BackgroundTextureIndex].ClipHeight;
                                            Gl.glColor4f(br, bg, bb, ba * alpha);
                                            RenderOverlayTexture(Left.BackgroundTextureIndex, px, py, px + u, py + v);
                                        }
                                    }
                                    /// right background
                                    if (Right.BackgroundTextureIndex >= 0)
                                    {
                                        int OpenGlTextureIndex = TextureManager.UseTexture(Right.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                        if (OpenGlTextureIndex != 0)
                                        {
                                            double u = (double)TextureManager.Textures[Right.BackgroundTextureIndex].ClipWidth;
                                            double v = (double)TextureManager.Textures[Right.BackgroundTextureIndex].ClipHeight;
                                            Gl.glColor4f(br, bg, bb, ba * alpha);
                                            RenderOverlayTexture(Right.BackgroundTextureIndex, px + w - u, py, px + w, py + v);
                                        }
                                    }
                                    /// middle background
                                    if (Middle.BackgroundTextureIndex >= 0)
                                    {
                                        int OpenGlTextureIndex = TextureManager.UseTexture(Middle.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                        if (OpenGlTextureIndex != 0)
                                        {
                                            double v = (double)TextureManager.Textures[Middle.BackgroundTextureIndex].ClipHeight;
                                            Gl.glColor4f(br, bg, bb, ba * alpha);
                                            RenderOverlayTexture(Middle.BackgroundTextureIndex, px + lw, py, px + w - rw, py + v);
                                        }
                                    }
                                    { /// text
										string t = Game.Messages[j].DisplayText;
                                        double u = widths[j];
                                        double v = heights[j];
                                        double p = Math.Round((Interface.CurrentHudElements[i].TextAlignment.X < 0 ? px : Interface.CurrentHudElements[i].TextAlignment.X > 0 ? px + w - u : px + 0.5 * (w - u)) - j * Interface.CurrentHudElements[i].Value1);
                                        double q = Math.Round(Interface.CurrentHudElements[i].TextAlignment.Y < 0 ? py : Interface.CurrentHudElements[i].TextAlignment.Y > 0 ? py + lcrh - v : py + 0.5 * (lcrh - v));
                                        p += Interface.CurrentHudElements[i].TextPosition.X;
                                        q += Interface.CurrentHudElements[i].TextPosition.Y;
                                        RenderString(p, q, Interface.CurrentHudElements[i].TextSize, t, -1, tr, tg, tb, ta * alpha, Interface.CurrentHudElements[i].TextShadow);
                                    }
                                    /// left overlay
                                    if (Left.OverlayTextureIndex >= 0)
                                    {
                                        int OpenGlTextureIndex = TextureManager.UseTexture(Left.OverlayTextureIndex, TextureManager.UseMode.LoadImmediately);
                                        if (OpenGlTextureIndex != 0)
                                        {
                                            double u = (double)TextureManager.Textures[Left.OverlayTextureIndex].ClipWidth;
                                            double v = (double)TextureManager.Textures[Left.OverlayTextureIndex].ClipHeight;
                                            Gl.glColor4f(or, og, ob, oa * alpha);
                                            RenderOverlayTexture(Left.OverlayTextureIndex, px, py, px + u, py + v);
                                        }
                                    }
                                    /// right overlay
                                    if (Right.OverlayTextureIndex >= 0)
                                    {
                                        int OpenGlTextureIndex = TextureManager.UseTexture(Right.OverlayTextureIndex, TextureManager.UseMode.LoadImmediately);
                                        if (OpenGlTextureIndex != 0)
                                        {
                                            double u = (double)TextureManager.Textures[Right.OverlayTextureIndex].ClipWidth;
                                            double v = (double)TextureManager.Textures[Right.OverlayTextureIndex].ClipHeight;
                                            Gl.glColor4f(or, og, ob, oa * alpha);
                                            RenderOverlayTexture(Right.OverlayTextureIndex, px + w - u, py, px + w, py + v);
                                        }
                                    }
                                    /// middle overlay
                                    if (Middle.OverlayTextureIndex >= 0)
                                    {
                                        int OpenGlTextureIndex = TextureManager.UseTexture(Middle.OverlayTextureIndex, TextureManager.UseMode.LoadImmediately);
                                        if (OpenGlTextureIndex != 0)
                                        {
                                            double v = (double)TextureManager.Textures[Middle.OverlayTextureIndex].ClipHeight;
                                            Gl.glColor4f(or, og, ob, oa * alpha);
                                            RenderOverlayTexture(Middle.OverlayTextureIndex, px + lw, py, px + w - rw, py + v);
                                        }
                                    }
                                }
                            }
                            break;
                        case "scoremessages":
                            {
                                // score messages
                                int n = Game.ScoreMessages.Length;
                                float totalwidth = 16.0f;
                                float[] widths = new float[n];
                                float[] heights = new float[n];
                                for (int j = 0; j < n; j++)
                                {
                                    MeasureString(Game.ScoreMessages[j].Text, Interface.CurrentHudElements[i].TextSize, out widths[j], out heights[j]);
                                    float a = widths[j] - j * Interface.CurrentHudElements[i].Value1;
                                    if (a > totalwidth) totalwidth = a;
                                }
                                Game.ScoreMessagesRendererSize.X += 16.0 * TimeElapsed * ((double)totalwidth - Game.ScoreMessagesRendererSize.X);
                                totalwidth = (float)Game.ScoreMessagesRendererSize.X;
                                double lcrh = 0.0;
                                /// left width/height
                                double lw = 0.0;
                                if (Interface.CurrentHudElements[i].TopLeft.BackgroundTextureIndex >= 0)
                                {
                                    int OpenGlTextureIndex = TextureManager.UseTexture(Interface.CurrentHudElements[i].TopLeft.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                    if (OpenGlTextureIndex != 0)
                                    {
                                        double u = (double)TextureManager.Textures[Interface.CurrentHudElements[i].TopLeft.BackgroundTextureIndex].ClipWidth;
                                        double v = (double)TextureManager.Textures[Interface.CurrentHudElements[i].TopLeft.BackgroundTextureIndex].ClipHeight;
                                        if (u > lw) lw = u;
                                        if (v > lcrh) lcrh = v;
                                    }
                                }
                                if (Interface.CurrentHudElements[i].CenterLeft.BackgroundTextureIndex >= 0)
                                {
                                    int OpenGlTextureIndex = TextureManager.UseTexture(Interface.CurrentHudElements[i].CenterLeft.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                    if (OpenGlTextureIndex != 0)
                                    {
                                        double u = (double)TextureManager.Textures[Interface.CurrentHudElements[i].CenterLeft.BackgroundTextureIndex].ClipWidth;
                                        double v = (double)TextureManager.Textures[Interface.CurrentHudElements[i].CenterLeft.BackgroundTextureIndex].ClipHeight;
                                        if (u > lw) lw = u;
                                        if (v > lcrh) lcrh = v;
                                    }
                                }
                                if (Interface.CurrentHudElements[i].BottomLeft.BackgroundTextureIndex >= 0)
                                {
                                    int OpenGlTextureIndex = TextureManager.UseTexture(Interface.CurrentHudElements[i].BottomLeft.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                    if (OpenGlTextureIndex != 0)
                                    {
                                        double u = (double)TextureManager.Textures[Interface.CurrentHudElements[i].BottomLeft.BackgroundTextureIndex].ClipWidth;
                                        double v = (double)TextureManager.Textures[Interface.CurrentHudElements[i].BottomLeft.BackgroundTextureIndex].ClipHeight;
                                        if (u > lw) lw = u;
                                        if (v > lcrh) lcrh = v;
                                    }
                                }
                                /// center height
                                if (Interface.CurrentHudElements[i].TopMiddle.BackgroundTextureIndex >= 0)
                                {
                                    int OpenGlTextureIndex = TextureManager.UseTexture(Interface.CurrentHudElements[i].TopMiddle.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                    if (OpenGlTextureIndex != 0)
                                    {
                                        double v = (double)TextureManager.Textures[Interface.CurrentHudElements[i].TopMiddle.BackgroundTextureIndex].ClipHeight;
                                        if (v > lcrh) lcrh = v;
                                    }
                                }
                                if (Interface.CurrentHudElements[i].CenterMiddle.BackgroundTextureIndex >= 0)
                                {
                                    int OpenGlTextureIndex = TextureManager.UseTexture(Interface.CurrentHudElements[i].CenterMiddle.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                    if (OpenGlTextureIndex != 0)
                                    {
                                        double v = (double)TextureManager.Textures[Interface.CurrentHudElements[i].CenterMiddle.BackgroundTextureIndex].ClipHeight;
                                        if (v > lcrh) lcrh = v;
                                    }
                                }
                                if (Interface.CurrentHudElements[i].BottomMiddle.BackgroundTextureIndex >= 0)
                                {
                                    int OpenGlTextureIndex = TextureManager.UseTexture(Interface.CurrentHudElements[i].BottomMiddle.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                    if (OpenGlTextureIndex != 0)
                                    {
                                        double v = (double)TextureManager.Textures[Interface.CurrentHudElements[i].BottomMiddle.BackgroundTextureIndex].ClipHeight;
                                        if (v > lcrh) lcrh = v;
                                    }
                                }
                                /// right width/height
                                double rw = 0.0;
                                if (Interface.CurrentHudElements[i].TopRight.BackgroundTextureIndex >= 0)
                                {
                                    int OpenGlTextureIndex = TextureManager.UseTexture(Interface.CurrentHudElements[i].TopRight.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                    if (OpenGlTextureIndex != 0)
                                    {
                                        double u = (double)TextureManager.Textures[Interface.CurrentHudElements[i].TopRight.BackgroundTextureIndex].ClipWidth;
                                        double v = (double)TextureManager.Textures[Interface.CurrentHudElements[i].TopRight.BackgroundTextureIndex].ClipHeight;
                                        if (u > rw) rw = u;
                                        if (v > lcrh) lcrh = v;
                                    }
                                }
                                if (Interface.CurrentHudElements[i].CenterRight.BackgroundTextureIndex >= 0)
                                {
                                    int OpenGlTextureIndex = TextureManager.UseTexture(Interface.CurrentHudElements[i].CenterRight.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                    if (OpenGlTextureIndex != 0)
                                    {
                                        double u = (double)TextureManager.Textures[Interface.CurrentHudElements[i].CenterRight.BackgroundTextureIndex].ClipWidth;
                                        double v = (double)TextureManager.Textures[Interface.CurrentHudElements[i].CenterRight.BackgroundTextureIndex].ClipHeight;
                                        if (u > rw) rw = u;
                                        if (v > lcrh) lcrh = v;
                                    }
                                }
                                if (Interface.CurrentHudElements[i].BottomRight.BackgroundTextureIndex >= 0)
                                {
                                    int OpenGlTextureIndex = TextureManager.UseTexture(Interface.CurrentHudElements[i].BottomRight.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                    if (OpenGlTextureIndex != 0)
                                    {
                                        double u = (double)TextureManager.Textures[Interface.CurrentHudElements[i].BottomRight.BackgroundTextureIndex].ClipWidth;
                                        double v = (double)TextureManager.Textures[Interface.CurrentHudElements[i].BottomRight.BackgroundTextureIndex].ClipHeight;
                                        if (u > rw) rw = u;
                                        if (v > lcrh) lcrh = v;
                                    }
                                }
                                /// start
                                double w = Interface.CurrentHudElements[i].Alignment.X == 0 ? lw + rw + 128 : totalwidth + lw + rw;
                                double h = Interface.CurrentHudElements[i].Value2 * n;
                                double x = Interface.CurrentHudElements[i].Alignment.X < 0 ? 0.0 : Interface.CurrentHudElements[i].Alignment.X > 0 ? ScreenWidth - w : 0.5 * (ScreenWidth - w);
                                double y = Interface.CurrentHudElements[i].Alignment.Y < 0 ? 0.0 : Interface.CurrentHudElements[i].Alignment.Y > 0 ? ScreenHeight - h : 0.5 * (ScreenHeight - h);
                                x += Interface.CurrentHudElements[i].Position.X;
                                y += Interface.CurrentHudElements[i].Position.Y;
                                int m = 0;
                                for (int j = 0; j < n; j++)
                                {
                                    float br, bg, bb, ba;
                                    CreateBackColor(Interface.CurrentHudElements[i].BackgroundColor, Game.ScoreMessages[j].Color, out br, out bg, out bb, out ba);
                                    float tr, tg, tb, ta;
                                    CreateTextColor(Interface.CurrentHudElements[i].TextColor, Game.ScoreMessages[j].Color, out tr, out tg, out tb, out ta);
                                    float or, og, ob, oa;
                                    CreateBackColor(Interface.CurrentHudElements[i].OverlayColor, Game.ScoreMessages[j].Color, out or, out og, out ob, out oa);
                                    double tx, ty;
                                    bool preserve = false;
                                    if ((Interface.CurrentHudElements[i].Transition & Interface.HudTransition.Move) != 0)
                                    {
                                        if (Game.SecondsSinceMidnight < Game.ScoreMessages[j].Timeout)
                                        {
                                            if (Game.ScoreMessages[j].RendererAlpha == 0.0)
                                            {
                                                Game.ScoreMessages[j].RendererPosition.X = x + Interface.CurrentHudElements[i].TransitionVector.X;
                                                Game.ScoreMessages[j].RendererPosition.Y = y + Interface.CurrentHudElements[i].TransitionVector.Y;
                                                Game.ScoreMessages[j].RendererAlpha = 1.0;
                                            }
                                            tx = x;
                                            ty = y + m * (Interface.CurrentHudElements[i].Value2);
                                            preserve = true;
                                        }
                                        else if (Interface.CurrentHudElements[i].Transition == Interface.HudTransition.MoveAndFade)
                                        {
                                            tx = x;
                                            ty = y + m * (Interface.CurrentHudElements[i].Value2);
                                        }
                                        else
                                        {
                                            tx = x + Interface.CurrentHudElements[i].TransitionVector.X;
                                            ty = y + (j + 1) * Interface.CurrentHudElements[i].TransitionVector.Y;
                                        }
                                        const double speed = 2.0;
                                        double dx = (speed * Math.Abs(tx - Game.ScoreMessages[j].RendererPosition.X) + 0.1) * TimeElapsed;
                                        double dy = (speed * Math.Abs(ty - Game.ScoreMessages[j].RendererPosition.Y) + 0.1) * TimeElapsed;
                                        if (Math.Abs(tx - Game.ScoreMessages[j].RendererPosition.X) < dx)
                                        {
                                            Game.ScoreMessages[j].RendererPosition.X = tx;
                                        }
                                        else
                                        {
                                            Game.ScoreMessages[j].RendererPosition.X += Math.Sign(tx - Game.ScoreMessages[j].RendererPosition.X) * dx;
                                        }
                                        if (Math.Abs(ty - Game.ScoreMessages[j].RendererPosition.Y) < dy)
                                        {
                                            Game.ScoreMessages[j].RendererPosition.Y = ty;
                                        }
                                        else
                                        {
                                            Game.ScoreMessages[j].RendererPosition.Y += Math.Sign(ty - Game.ScoreMessages[j].RendererPosition.Y) * dy;
                                        }
                                    }
                                    else
                                    {
                                        tx = x;
                                        ty = y + m * (Interface.CurrentHudElements[i].Value2);
                                        Game.ScoreMessages[j].RendererPosition.X = 0.0;
                                        const double speed = 12.0;
                                        double dy = (speed * Math.Abs(ty - Game.ScoreMessages[j].RendererPosition.Y) + 0.1) * TimeElapsed;
                                        Game.ScoreMessages[j].RendererPosition.X = x;
                                        if (Math.Abs(ty - Game.ScoreMessages[j].RendererPosition.Y) < dy)
                                        {
                                            Game.ScoreMessages[j].RendererPosition.Y = ty;
                                        }
                                        else
                                        {
                                            Game.ScoreMessages[j].RendererPosition.Y += Math.Sign(ty - Game.ScoreMessages[j].RendererPosition.Y) * dy;
                                        }
                                    }
                                    if ((Interface.CurrentHudElements[i].Transition & Interface.HudTransition.Fade) != 0)
                                    {
                                        if (Game.SecondsSinceMidnight >= Game.ScoreMessages[j].Timeout)
                                        {
                                            Game.ScoreMessages[j].RendererAlpha -= TimeElapsed;
                                            if (Game.ScoreMessages[j].RendererAlpha < 0.0) Game.ScoreMessages[j].RendererAlpha = 0.0;
                                        }
                                        else
                                        {
                                            Game.ScoreMessages[j].RendererAlpha += TimeElapsed;
                                            if (Game.ScoreMessages[j].RendererAlpha > 1.0) Game.ScoreMessages[j].RendererAlpha = 1.0;
                                            preserve = true;
                                        }
                                    }
                                    else if (Game.SecondsSinceMidnight > Game.ScoreMessages[j].Timeout)
                                    {
                                        if (Math.Abs(Game.ScoreMessages[j].RendererPosition.X - tx) < 0.1 & Math.Abs(Game.ScoreMessages[j].RendererPosition.Y - ty) < 0.1)
                                        {
                                            Game.ScoreMessages[j].RendererAlpha = 0.0;
                                        }
                                    }
                                    if (preserve) m++;
                                    double px = Game.ScoreMessages[j].RendererPosition.X + (double)j * (double)Interface.CurrentHudElements[i].Value1;
                                    double py = Game.ScoreMessages[j].RendererPosition.Y;
                                    float alpha = (float)(Game.ScoreMessages[j].RendererAlpha * Game.ScoreMessages[j].RendererAlpha);
                                    /// graphics
                                    Interface.HudImage Left = j == 0 ? Interface.CurrentHudElements[i].TopLeft : j < n - 1 ? Interface.CurrentHudElements[i].CenterLeft : Interface.CurrentHudElements[i].BottomLeft;
                                    Interface.HudImage Middle = j == 0 ? Interface.CurrentHudElements[i].TopMiddle : j < n - 1 ? Interface.CurrentHudElements[i].CenterMiddle : Interface.CurrentHudElements[i].BottomMiddle;
                                    Interface.HudImage Right = j == 0 ? Interface.CurrentHudElements[i].TopRight : j < n - 1 ? Interface.CurrentHudElements[i].CenterRight : Interface.CurrentHudElements[i].BottomRight;
                                    /// left background
                                    if (Left.BackgroundTextureIndex >= 0)
                                    {
                                        int OpenGlTextureIndex = TextureManager.UseTexture(Left.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                        if (OpenGlTextureIndex != 0)
                                        {
                                            double u = (double)TextureManager.Textures[Left.BackgroundTextureIndex].ClipWidth;
                                            double v = (double)TextureManager.Textures[Left.BackgroundTextureIndex].ClipHeight;
                                            Gl.glColor4f(br, bg, bb, ba * alpha);
                                            RenderOverlayTexture(Left.BackgroundTextureIndex, px, py, px + u, py + v);
                                        }
                                    }
                                    /// right background
                                    if (Right.BackgroundTextureIndex >= 0)
                                    {
                                        int OpenGlTextureIndex = TextureManager.UseTexture(Right.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                        if (OpenGlTextureIndex != 0)
                                        {
                                            double u = (double)TextureManager.Textures[Right.BackgroundTextureIndex].ClipWidth;
                                            double v = (double)TextureManager.Textures[Right.BackgroundTextureIndex].ClipHeight;
                                            Gl.glColor4f(br, bg, bb, ba * alpha);
                                            RenderOverlayTexture(Right.BackgroundTextureIndex, px + w - u, py, px + w, py + v);
                                        }
                                    }
                                    /// middle background
                                    if (Middle.BackgroundTextureIndex >= 0)
                                    {
                                        int OpenGlTextureIndex = TextureManager.UseTexture(Middle.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                        if (OpenGlTextureIndex != 0)
                                        {
                                            double v = (double)TextureManager.Textures[Middle.BackgroundTextureIndex].ClipHeight;
                                            Gl.glColor4f(br, bg, bb, ba * alpha);
                                            RenderOverlayTexture(Middle.BackgroundTextureIndex, px + lw, py, px + w - rw, py + v);
                                        }
                                    }
                                    { /// text
										string t = Game.ScoreMessages[j].Text;
                                        double u = widths[j];
                                        double v = heights[j];
                                        double p = Math.Round((Interface.CurrentHudElements[i].TextAlignment.X < 0 ? px : Interface.CurrentHudElements[i].TextAlignment.X > 0 ? px + w - u : px + 0.5 * (w - u)) - j * Interface.CurrentHudElements[i].Value1);
                                        double q = Math.Round(Interface.CurrentHudElements[i].TextAlignment.Y < 0 ? py : Interface.CurrentHudElements[i].TextAlignment.Y > 0 ? py + lcrh - v : py + 0.5 * (lcrh - v));
                                        p += Interface.CurrentHudElements[i].TextPosition.X;
                                        q += Interface.CurrentHudElements[i].TextPosition.Y;
                                        RenderString(p, q, Interface.CurrentHudElements[i].TextSize, t, -1, tr, tg, tb, ta * alpha, Interface.CurrentHudElements[i].TextShadow);
                                    }
                                    /// left overlay
                                    if (Left.OverlayTextureIndex >= 0)
                                    {
                                        int OpenGlTextureIndex = TextureManager.UseTexture(Left.OverlayTextureIndex, TextureManager.UseMode.LoadImmediately);
                                        if (OpenGlTextureIndex != 0)
                                        {
                                            double u = (double)TextureManager.Textures[Left.OverlayTextureIndex].ClipWidth;
                                            double v = (double)TextureManager.Textures[Left.OverlayTextureIndex].ClipHeight;
                                            Gl.glColor4f(or, og, ob, oa * alpha);
                                            RenderOverlayTexture(Left.OverlayTextureIndex, px, py, px + u, py + v);
                                        }
                                    }
                                    /// right overlay
                                    if (Right.OverlayTextureIndex >= 0)
                                    {
                                        int OpenGlTextureIndex = TextureManager.UseTexture(Right.OverlayTextureIndex, TextureManager.UseMode.LoadImmediately);
                                        if (OpenGlTextureIndex != 0)
                                        {
                                            double u = (double)TextureManager.Textures[Right.OverlayTextureIndex].ClipWidth;
                                            double v = (double)TextureManager.Textures[Right.OverlayTextureIndex].ClipHeight;
                                            Gl.glColor4f(or, og, ob, oa * alpha);
                                            RenderOverlayTexture(Right.OverlayTextureIndex, px + w - u, py, px + w, py + v);
                                        }
                                    }
                                    /// middle overlay
                                    if (Middle.OverlayTextureIndex >= 0)
                                    {
                                        int OpenGlTextureIndex = TextureManager.UseTexture(Middle.OverlayTextureIndex, TextureManager.UseMode.LoadImmediately);
                                        if (OpenGlTextureIndex != 0)
                                        {
                                            double v = (double)TextureManager.Textures[Middle.OverlayTextureIndex].ClipHeight;
                                            Gl.glColor4f(or, og, ob, oa * alpha);
                                            RenderOverlayTexture(Middle.OverlayTextureIndex, px + lw, py, px + w - rw, py + v);
                                        }
                                    }
                                }
                            }
                            break;
                        case "ats":
                            {
                                // ats lamps
                                if (CurrentLampCollection.Lamps == null)
                                {
                                    InitializeLamps();
                                }
                                double lcrh = 0.0;
                                /// left width/height
                                double lw = 0.0;
                                if (Interface.CurrentHudElements[i].TopLeft.BackgroundTextureIndex >= 0)
                                {
                                    int OpenGlTextureIndex = TextureManager.UseTexture(Interface.CurrentHudElements[i].TopLeft.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                    if (OpenGlTextureIndex != 0)
                                    {
                                        double u = (double)TextureManager.Textures[Interface.CurrentHudElements[i].TopLeft.BackgroundTextureIndex].ClipWidth;
                                        double v = (double)TextureManager.Textures[Interface.CurrentHudElements[i].TopLeft.BackgroundTextureIndex].ClipHeight;
                                        if (u > lw) lw = u;
                                        if (v > lcrh) lcrh = v;
                                    }
                                }
                                if (Interface.CurrentHudElements[i].CenterLeft.BackgroundTextureIndex >= 0)
                                {
                                    int OpenGlTextureIndex = TextureManager.UseTexture(Interface.CurrentHudElements[i].CenterLeft.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                    if (OpenGlTextureIndex != 0)
                                    {
                                        double u = (double)TextureManager.Textures[Interface.CurrentHudElements[i].CenterLeft.BackgroundTextureIndex].ClipWidth;
                                        double v = (double)TextureManager.Textures[Interface.CurrentHudElements[i].CenterLeft.BackgroundTextureIndex].ClipHeight;
                                        if (u > lw) lw = u;
                                        if (v > lcrh) lcrh = v;
                                    }
                                }
                                if (Interface.CurrentHudElements[i].BottomLeft.BackgroundTextureIndex >= 0)
                                {
                                    int OpenGlTextureIndex = TextureManager.UseTexture(Interface.CurrentHudElements[i].BottomLeft.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                    if (OpenGlTextureIndex != 0)
                                    {
                                        double u = (double)TextureManager.Textures[Interface.CurrentHudElements[i].BottomLeft.BackgroundTextureIndex].ClipWidth;
                                        double v = (double)TextureManager.Textures[Interface.CurrentHudElements[i].BottomLeft.BackgroundTextureIndex].ClipHeight;
                                        if (u > lw) lw = u;
                                        if (v > lcrh) lcrh = v;
                                    }
                                }
                                /// center height
                                if (Interface.CurrentHudElements[i].TopMiddle.BackgroundTextureIndex >= 0)
                                {
                                    int OpenGlTextureIndex = TextureManager.UseTexture(Interface.CurrentHudElements[i].TopMiddle.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                    if (OpenGlTextureIndex != 0)
                                    {
                                        double v = (double)TextureManager.Textures[Interface.CurrentHudElements[i].TopMiddle.BackgroundTextureIndex].ClipHeight;
                                        if (v > lcrh) lcrh = v;
                                    }
                                }
                                if (Interface.CurrentHudElements[i].CenterMiddle.BackgroundTextureIndex >= 0)
                                {
                                    int OpenGlTextureIndex = TextureManager.UseTexture(Interface.CurrentHudElements[i].CenterMiddle.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                    if (OpenGlTextureIndex != 0)
                                    {
                                        double v = (double)TextureManager.Textures[Interface.CurrentHudElements[i].CenterMiddle.BackgroundTextureIndex].ClipHeight;
                                        if (v > lcrh) lcrh = v;
                                    }
                                }
                                if (Interface.CurrentHudElements[i].BottomMiddle.BackgroundTextureIndex >= 0)
                                {
                                    int OpenGlTextureIndex = TextureManager.UseTexture(Interface.CurrentHudElements[i].BottomMiddle.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                    if (OpenGlTextureIndex != 0)
                                    {
                                        double v = (double)TextureManager.Textures[Interface.CurrentHudElements[i].BottomMiddle.BackgroundTextureIndex].ClipHeight;
                                        if (v > lcrh) lcrh = v;
                                    }
                                }
                                /// right width/height
                                double rw = 0.0;
                                if (Interface.CurrentHudElements[i].TopRight.BackgroundTextureIndex >= 0)
                                {
                                    int OpenGlTextureIndex = TextureManager.UseTexture(Interface.CurrentHudElements[i].TopRight.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                    if (OpenGlTextureIndex != 0)
                                    {
                                        double u = (double)TextureManager.Textures[Interface.CurrentHudElements[i].TopRight.BackgroundTextureIndex].ClipWidth;
                                        double v = (double)TextureManager.Textures[Interface.CurrentHudElements[i].TopRight.BackgroundTextureIndex].ClipHeight;
                                        if (u > rw) rw = u;
                                        if (v > lcrh) lcrh = v;
                                    }
                                }
                                if (Interface.CurrentHudElements[i].CenterRight.BackgroundTextureIndex >= 0)
                                {
                                    int OpenGlTextureIndex = TextureManager.UseTexture(Interface.CurrentHudElements[i].CenterRight.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                    if (OpenGlTextureIndex != 0)
                                    {
                                        double u = (double)TextureManager.Textures[Interface.CurrentHudElements[i].CenterRight.BackgroundTextureIndex].ClipWidth;
                                        double v = (double)TextureManager.Textures[Interface.CurrentHudElements[i].CenterRight.BackgroundTextureIndex].ClipHeight;
                                        if (u > rw) rw = u;
                                        if (v > lcrh) lcrh = v;
                                    }
                                }
                                if (Interface.CurrentHudElements[i].BottomRight.BackgroundTextureIndex >= 0)
                                {
                                    int OpenGlTextureIndex = TextureManager.UseTexture(Interface.CurrentHudElements[i].BottomRight.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                    if (OpenGlTextureIndex != 0)
                                    {
                                        double u = (double)TextureManager.Textures[Interface.CurrentHudElements[i].BottomRight.BackgroundTextureIndex].ClipWidth;
                                        double v = (double)TextureManager.Textures[Interface.CurrentHudElements[i].BottomRight.BackgroundTextureIndex].ClipHeight;
                                        if (u > rw) rw = u;
                                        if (v > lcrh) lcrh = v;
                                    }
                                }
                                /// start
                                int n = CurrentLampCollection.Lamps.Length;
                                double w = (double)CurrentLampCollection.Width + lw + rw;
                                double h = Interface.CurrentHudElements[i].Value2 * n;
                                double x = Interface.CurrentHudElements[i].Alignment.X < 0 ? 0.0 : Interface.CurrentHudElements[i].Alignment.X > 0 ? ScreenWidth - w : 0.5 * (ScreenWidth - w);
                                double y = Interface.CurrentHudElements[i].Alignment.Y < 0 ? 0.0 : Interface.CurrentHudElements[i].Alignment.Y > 0 ? ScreenHeight - h : 0.5 * (ScreenHeight - h);
                                x += Interface.CurrentHudElements[i].Position.X;
                                y += Interface.CurrentHudElements[i].Position.Y;
                                for (int j = 0; j < n; j++)
                                {
                                    if (CurrentLampCollection.Lamps[j].Type != LampType.None)
                                    {
                                        int o;
                                        if (j == 0)
                                        {
                                            o = -1;
                                        }
                                        else if (CurrentLampCollection.Lamps[j - 1].Type == LampType.None)
                                        {
                                            o = -1;
                                        }
                                        else if (j < n - 1 && CurrentLampCollection.Lamps[j + 1].Type == LampType.None)
                                        {
                                            o = 1;
                                        }
                                        else if (j == n - 1)
                                        {
                                            o = 1;
                                        }
                                        else
                                        {
                                            o = 0;
                                        }
                                        Interface.HudImage Left = o < 0 ? Interface.CurrentHudElements[i].TopLeft : o == 0 ? Interface.CurrentHudElements[i].CenterLeft : Interface.CurrentHudElements[i].BottomLeft;
                                        Interface.HudImage Middle = o < 0 ? Interface.CurrentHudElements[i].TopMiddle : o == 0 ? Interface.CurrentHudElements[i].CenterMiddle : Interface.CurrentHudElements[i].BottomMiddle;
                                        Interface.HudImage Right = o < 0 ? Interface.CurrentHudElements[i].TopRight : o == 0 ? Interface.CurrentHudElements[i].CenterRight : Interface.CurrentHudElements[i].BottomRight;
                                        Game.MessageColor sc = Game.MessageColor.Gray;
                                        if (TrainManager.PlayerTrain.Plugin.Panel.Length >= 272)
                                        {
                                            switch (CurrentLampCollection.Lamps[j].Type)
                                            {
                                                case LampType.Ats:
                                                    if (TrainManager.PlayerTrain.Plugin.Panel[256] != 0)
                                                    {
                                                        sc = Game.MessageColor.Orange;
                                                    }
                                                    break;
                                                case LampType.AtsOperation:
                                                    if (TrainManager.PlayerTrain.Plugin.Panel[258] != 0)
                                                    {
                                                        sc = Game.MessageColor.Red;
                                                    }
                                                    break;
                                                case LampType.AtsPPower:
                                                    if (TrainManager.PlayerTrain.Plugin.Panel[259] != 0)
                                                    {
                                                        sc = Game.MessageColor.Green;
                                                    }
                                                    break;
                                                case LampType.AtsPPattern:
                                                    if (TrainManager.PlayerTrain.Plugin.Panel[260] != 0)
                                                    {
                                                        sc = Game.MessageColor.Orange;
                                                    }
                                                    break;
                                                case LampType.AtsPBrakeOverride:
                                                    if (TrainManager.PlayerTrain.Plugin.Panel[261] != 0)
                                                    {
                                                        sc = Game.MessageColor.Orange;
                                                    }
                                                    break;
                                                case LampType.AtsPBrakeOperation:
                                                    if (TrainManager.PlayerTrain.Plugin.Panel[262] != 0)
                                                    {
                                                        sc = Game.MessageColor.Orange;
                                                    }
                                                    break;
                                                case LampType.AtsP:
                                                    if (TrainManager.PlayerTrain.Plugin.Panel[263] != 0)
                                                    {
                                                        sc = Game.MessageColor.Green;
                                                    }
                                                    break;
                                                case LampType.AtsPFailure:
                                                    if (TrainManager.PlayerTrain.Plugin.Panel[264] != 0)
                                                    {
                                                        sc = Game.MessageColor.Red;
                                                    }
                                                    break;
                                                case LampType.Atc:
                                                    if (TrainManager.PlayerTrain.Plugin.Panel[265] != 0)
                                                    {
                                                        sc = Game.MessageColor.Orange;
                                                    }
                                                    break;
                                                case LampType.AtcPower:
                                                    if (TrainManager.PlayerTrain.Plugin.Panel[266] != 0)
                                                    {
                                                        sc = Game.MessageColor.Orange;
                                                    }
                                                    break;
                                                case LampType.AtcUse:
                                                    if (TrainManager.PlayerTrain.Plugin.Panel[267] != 0)
                                                    {
                                                        sc = Game.MessageColor.Orange;
                                                    }
                                                    break;
                                                case LampType.AtcEmergency:
                                                    if (TrainManager.PlayerTrain.Plugin.Panel[268] != 0)
                                                    {
                                                        sc = Game.MessageColor.Red;
                                                    }
                                                    break;
                                                case LampType.Eb:
                                                    if (TrainManager.PlayerTrain.Plugin.Panel[270] != 0)
                                                    {
                                                        sc = Game.MessageColor.Green;
                                                    }
                                                    break;
                                                case LampType.ConstSpeed:
                                                    if (TrainManager.PlayerTrain.Plugin.Panel[269] != 0)
                                                    {
                                                        sc = Game.MessageColor.Orange;
                                                    }
                                                    break;
                                            }
                                        }
                                        /// colors
                                        float br, bg, bb, ba;
                                        CreateBackColor(Interface.CurrentHudElements[i].BackgroundColor, sc, out br, out bg, out bb, out ba);
                                        float tr, tg, tb, ta;
                                        CreateTextColor(Interface.CurrentHudElements[i].TextColor, sc, out tr, out tg, out tb, out ta);
                                        float or, og, ob, oa;
                                        CreateBackColor(Interface.CurrentHudElements[i].OverlayColor, sc, out or, out og, out ob, out oa);
                                        /// left background
                                        if (Left.BackgroundTextureIndex >= 0)
                                        {
                                            int OpenGlTextureIndex = TextureManager.UseTexture(Left.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                            if (OpenGlTextureIndex != 0)
                                            {
                                                double u = (double)TextureManager.Textures[Left.BackgroundTextureIndex].ClipWidth;
                                                double v = (double)TextureManager.Textures[Left.BackgroundTextureIndex].ClipHeight;
                                                Gl.glColor4f(br, bg, bb, ba);
                                                RenderOverlayTexture(Left.BackgroundTextureIndex, x, y, x + u, y + v);
                                            }
                                        }
                                        /// right background
                                        if (Right.BackgroundTextureIndex >= 0)
                                        {
                                            int OpenGlTextureIndex = TextureManager.UseTexture(Right.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                            if (OpenGlTextureIndex != 0)
                                            {
                                                double u = (double)TextureManager.Textures[Right.BackgroundTextureIndex].ClipWidth;
                                                double v = (double)TextureManager.Textures[Right.BackgroundTextureIndex].ClipHeight;
                                                Gl.glColor4f(br, bg, bb, ba);
                                                RenderOverlayTexture(Right.BackgroundTextureIndex, x + w - u, y, x + w, y + v);
                                            }
                                        }
                                        /// middle background
                                        if (Middle.BackgroundTextureIndex >= 0)
                                        {
                                            int OpenGlTextureIndex = TextureManager.UseTexture(Middle.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                            if (OpenGlTextureIndex != 0)
                                            {
                                                double v = (double)TextureManager.Textures[Middle.BackgroundTextureIndex].ClipHeight;
                                                Gl.glColor4f(br, bg, bb, ba);
                                                RenderOverlayTexture(Middle.BackgroundTextureIndex, x + lw, y, x + w - rw, y + v);
                                            }
                                        }
                                        { /// text
											string t = CurrentLampCollection.Lamps[j].Text;
                                            double u = CurrentLampCollection.Lamps[j].Width;
                                            double v = CurrentLampCollection.Lamps[j].Height;
                                            double p = Math.Round(Interface.CurrentHudElements[i].TextAlignment.X < 0 ? x : Interface.CurrentHudElements[i].TextAlignment.X > 0 ? x + w - u : x + 0.5 * (w - u));
                                            double q = Math.Round(Interface.CurrentHudElements[i].TextAlignment.Y < 0 ? y : Interface.CurrentHudElements[i].TextAlignment.Y > 0 ? y + lcrh - v : y + 0.5 * (lcrh - v));
                                            p += Interface.CurrentHudElements[i].TextPosition.X;
                                            q += Interface.CurrentHudElements[i].TextPosition.Y;
                                            RenderString(p, q, Interface.CurrentHudElements[i].TextSize, t, -1, tr, tg, tb, ta, Interface.CurrentHudElements[i].TextShadow);
                                        }
                                        /// left overlay
                                        if (Left.OverlayTextureIndex >= 0)
                                        {
                                            int OpenGlTextureIndex = TextureManager.UseTexture(Left.OverlayTextureIndex, TextureManager.UseMode.LoadImmediately);
                                            if (OpenGlTextureIndex != 0)
                                            {
                                                double u = (double)TextureManager.Textures[Left.OverlayTextureIndex].ClipWidth;
                                                double v = (double)TextureManager.Textures[Left.OverlayTextureIndex].ClipHeight;
                                                Gl.glColor4f(or, og, ob, oa);
                                                RenderOverlayTexture(Left.OverlayTextureIndex, x, y, x + u, y + v);
                                            }
                                        }
                                        /// right overlay
                                        if (Right.OverlayTextureIndex >= 0)
                                        {
                                            int OpenGlTextureIndex = TextureManager.UseTexture(Right.OverlayTextureIndex, TextureManager.UseMode.LoadImmediately);
                                            if (OpenGlTextureIndex != 0)
                                            {
                                                double u = (double)TextureManager.Textures[Right.OverlayTextureIndex].ClipWidth;
                                                double v = (double)TextureManager.Textures[Right.OverlayTextureIndex].ClipHeight;
                                                Gl.glColor4f(or, og, ob, oa);
                                                RenderOverlayTexture(Right.OverlayTextureIndex, x + w - u, y, x + w, y + v);
                                            }
                                        }
                                        /// middle overlay
                                        if (Middle.OverlayTextureIndex >= 0)
                                        {
                                            int OpenGlTextureIndex = TextureManager.UseTexture(Middle.OverlayTextureIndex, TextureManager.UseMode.LoadImmediately);
                                            if (OpenGlTextureIndex != 0)
                                            {
                                                double v = (double)TextureManager.Textures[Middle.OverlayTextureIndex].ClipHeight;
                                                Gl.glColor4f(or, og, ob, oa);
                                                RenderOverlayTexture(Middle.OverlayTextureIndex, x + lw, y, x + w - rw, y + v);
                                            }
                                        }
                                    }
                                    y += (double)Interface.CurrentHudElements[i].Value2;
                                }
                            }
                            break;
                        default:
                            {
                                // default
                                double w, h;
                                if (Interface.CurrentHudElements[i].CenterMiddle.BackgroundTextureIndex >= 0)
                                {
                                    int OpenGlTextureIndex = TextureManager.UseTexture(Interface.CurrentHudElements[i].CenterMiddle.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                    if (OpenGlTextureIndex != 0)
                                    {
                                        w = (double)TextureManager.Textures[Interface.CurrentHudElements[i].CenterMiddle.BackgroundTextureIndex].ClipWidth;
                                        h = (double)TextureManager.Textures[Interface.CurrentHudElements[i].CenterMiddle.BackgroundTextureIndex].ClipHeight;
                                    }
                                    else
                                    {
                                        w = 0.0; h = 0.0;
                                    }
                                }
                                else
                                {
                                    w = 0.0; h = 0.0;
                                }
                                double x = Interface.CurrentHudElements[i].Alignment.X < 0 ? 0.0 : Interface.CurrentHudElements[i].Alignment.X == 0 ? 0.5 * (ScreenWidth - w) : ScreenWidth - w;
                                double y = Interface.CurrentHudElements[i].Alignment.Y < 0 ? 0.0 : Interface.CurrentHudElements[i].Alignment.Y == 0 ? 0.5 * (ScreenHeight - h) : ScreenHeight - h;
                                x += Interface.CurrentHudElements[i].Position.X;
                                y += Interface.CurrentHudElements[i].Position.Y;
                                /// command
                                const double speed = 1.0;
                                Game.MessageColor sc = Game.MessageColor.None;
                                string t;
                                switch (Command)
                                {
                                    case "reverser":
                                        if (TrainManager.PlayerTrain.Specs.CurrentReverser.Driver < 0)
                                        {
                                            sc = Game.MessageColor.Orange; t = Interface.QuickReferences.HandleBackward;
                                        }
                                        else if (TrainManager.PlayerTrain.Specs.CurrentReverser.Driver > 0)
                                        {
                                            sc = Game.MessageColor.Blue; t = Interface.QuickReferences.HandleForward;
                                        }
                                        else
                                        {
                                            sc = Game.MessageColor.Gray; t = Interface.QuickReferences.HandleNeutral;
                                        }
                                        Interface.CurrentHudElements[i].TransitionState = 0.0;
                                        break;
                                    case "power":
                                        if (TrainManager.PlayerTrain.Specs.SingleHandle)
                                        {
                                            continue;
                                        }
                                        else if (TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Driver == 0)
                                        {
                                            sc = Game.MessageColor.Gray; t = Interface.QuickReferences.HandlePowerNull;
                                        }
                                        else
                                        {
                                            sc = Game.MessageColor.Blue; t = Interface.QuickReferences.HandlePower + TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Driver.ToString(Culture);
                                        }
                                        Interface.CurrentHudElements[i].TransitionState = 0.0;
                                        break;
                                    case "brake":
                                        if (TrainManager.PlayerTrain.Specs.SingleHandle)
                                        {
                                            continue;
                                        }
                                        else if (TrainManager.PlayerTrain.Cars[TrainManager.PlayerTrain.DriverCar].Specs.BrakeType == TrainManager.CarBrakeType.AutomaticAirBrake)
                                        {
                                            if (TrainManager.PlayerTrain.Specs.CurrentEmergencyBrake.Driver)
                                            {
                                                sc = Game.MessageColor.Red; t = Interface.QuickReferences.HandleEmergency;
                                            }
                                            else if (TrainManager.PlayerTrain.Specs.AirBrake.Handle.Driver == TrainManager.AirBrakeHandleState.Release)
                                            {
                                                sc = Game.MessageColor.Gray; t = Interface.QuickReferences.HandleRelease;
                                            }
                                            else if (TrainManager.PlayerTrain.Specs.AirBrake.Handle.Driver == TrainManager.AirBrakeHandleState.Lap)
                                            {
                                                sc = Game.MessageColor.Blue; t = Interface.QuickReferences.HandleLap;
                                            }
                                            else
                                            {
                                                sc = Game.MessageColor.Orange; t = Interface.QuickReferences.HandleService;
                                            }
                                        }
                                        else
                                        {
                                            if (TrainManager.PlayerTrain.Specs.CurrentEmergencyBrake.Driver)
                                            {
                                                sc = Game.MessageColor.Red; t = Interface.QuickReferences.HandleEmergency;
                                            }
                                            else if (TrainManager.PlayerTrain.Specs.CurrentHoldBrake.Driver)
                                            {
                                                sc = Game.MessageColor.Green; t = Interface.QuickReferences.HandleHoldBrake;
                                            }
                                            else if (TrainManager.PlayerTrain.Specs.CurrentBrakeNotch.Driver == 0)
                                            {
                                                sc = Game.MessageColor.Gray; t = Interface.QuickReferences.HandleBrakeNull;
                                            }
                                            else
                                            {
                                                sc = Game.MessageColor.Orange; t = Interface.QuickReferences.HandleBrake + TrainManager.PlayerTrain.Specs.CurrentBrakeNotch.Driver.ToString(Culture);
                                            }
                                        }
                                        Interface.CurrentHudElements[i].TransitionState = 0.0;
                                        break;
                                    case "single":
                                        if (!TrainManager.PlayerTrain.Specs.SingleHandle)
                                        {
                                            continue;
                                        }
                                        else if (TrainManager.PlayerTrain.Specs.CurrentEmergencyBrake.Driver)
                                        {
                                            sc = Game.MessageColor.Red; t = Interface.QuickReferences.HandleEmergency;
                                        }
                                        else if (TrainManager.PlayerTrain.Specs.CurrentHoldBrake.Driver)
                                        {
                                            sc = Game.MessageColor.Green; t = Interface.QuickReferences.HandleHoldBrake;
                                        }
                                        else if (TrainManager.PlayerTrain.Specs.CurrentBrakeNotch.Driver > 0)
                                        {
                                            sc = Game.MessageColor.Orange; t = Interface.QuickReferences.HandleBrake + TrainManager.PlayerTrain.Specs.CurrentBrakeNotch.Driver.ToString(Culture);
                                        }
                                        else if (TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Driver > 0)
                                        {
                                            sc = Game.MessageColor.Blue; t = Interface.QuickReferences.HandlePower + TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Driver.ToString(Culture);
                                        }
                                        else
                                        {
                                            sc = Game.MessageColor.Gray; t = Interface.QuickReferences.HandlePowerNull;
                                        }
                                        Interface.CurrentHudElements[i].TransitionState = 0.0;
                                        break;
                                    case "doorsleft":
                                    case "doorsright":
                                        {
                                            if ((LeftDoors & TrainManager.TrainDoorState.AllClosed) == 0 | (RightDoors & TrainManager.TrainDoorState.AllClosed) == 0)
                                            {
                                                Interface.CurrentHudElements[i].TransitionState -= speed * TimeElapsed;
                                                if (Interface.CurrentHudElements[i].TransitionState < 0.0) Interface.CurrentHudElements[i].TransitionState = 0.0;
                                            }
                                            else
                                            {
                                                Interface.CurrentHudElements[i].TransitionState += speed * TimeElapsed;
                                                if (Interface.CurrentHudElements[i].TransitionState > 1.0) Interface.CurrentHudElements[i].TransitionState = 1.0;
                                            }
                                            TrainManager.TrainDoorState Doors = Command == "doorsleft" ? LeftDoors : RightDoors;
                                            if ((Doors & TrainManager.TrainDoorState.Mixed) != 0)
                                            {
                                                sc = Game.MessageColor.Orange;
                                            }
                                            else if ((Doors & TrainManager.TrainDoorState.AllClosed) != 0)
                                            {
                                                sc = Game.MessageColor.Gray;
                                            }
                                            else if (TrainManager.PlayerTrain.Specs.DoorCloseMode == TrainManager.DoorMode.Manual)
                                            {
                                                sc = Game.MessageColor.Green;
                                            }
                                            else
                                            {
                                                sc = Game.MessageColor.Blue;
                                            }
                                            t = Command == "doorsleft" ? Interface.QuickReferences.DoorsLeft : Interface.QuickReferences.DoorsRight;
                                        }
                                        break;
                                    case "stopleft":
                                    case "stopright":
                                    case "stopnone":
                                        {
                                            int s = TrainManager.PlayerTrain.Station;
                                            if (s >= 0 && Game.PlayerStopsAtStation(s) && Interface.CurrentOptions.GameMode != Interface.GameMode.Expert)
                                            {
                                                bool cond;
                                                if (Command == "stopleft")
                                                {
                                                    cond = Game.Stations[s].OpenLeftDoors;
                                                }
                                                else if (Command == "stopright")
                                                {
                                                    cond = Game.Stations[s].OpenRightDoors;
                                                }
                                                else
                                                {
                                                    cond = !Game.Stations[s].OpenLeftDoors & !Game.Stations[s].OpenRightDoors;
                                                }
                                                if (TrainManager.PlayerTrain.StationState == TrainManager.TrainStopState.Pending & cond)
                                                {
                                                    Interface.CurrentHudElements[i].TransitionState -= speed * TimeElapsed;
                                                    if (Interface.CurrentHudElements[i].TransitionState < 0.0) Interface.CurrentHudElements[i].TransitionState = 0.0;
                                                }
                                                else
                                                {
                                                    Interface.CurrentHudElements[i].TransitionState += speed * TimeElapsed;
                                                    if (Interface.CurrentHudElements[i].TransitionState > 1.0) Interface.CurrentHudElements[i].TransitionState = 1.0;
                                                }
                                            }
                                            else
                                            {
                                                Interface.CurrentHudElements[i].TransitionState += speed * TimeElapsed;
                                                if (Interface.CurrentHudElements[i].TransitionState > 1.0) Interface.CurrentHudElements[i].TransitionState = 1.0;
                                            }
                                            t = Interface.CurrentHudElements[i].Text;
                                        }
                                        break;
                                    case "stoplefttick":
                                    case "stoprighttick":
                                    case "stopnonetick":
                                        {
                                            int s = TrainManager.PlayerTrain.Station;
                                            if (s >= 0 && Game.PlayerStopsAtStation(s) && Interface.CurrentOptions.GameMode != Interface.GameMode.Expert)
                                            {
                                                int c = Game.GetStopIndex(s, TrainManager.PlayerTrain.Cars.Length);
                                                if (c >= 0)
                                                {
                                                    bool cond;
                                                    if (Command == "stoplefttick")
                                                    {
                                                        cond = Game.Stations[s].OpenLeftDoors;
                                                    }
                                                    else if (Command == "stoprighttick")
                                                    {
                                                        cond = Game.Stations[s].OpenRightDoors;
                                                    }
                                                    else
                                                    {
                                                        cond = !Game.Stations[s].OpenLeftDoors & !Game.Stations[s].OpenRightDoors;
                                                    }
                                                    if (TrainManager.PlayerTrain.StationState == TrainManager.TrainStopState.Pending & cond)
                                                    {
                                                        Interface.CurrentHudElements[i].TransitionState -= speed * TimeElapsed;
                                                        if (Interface.CurrentHudElements[i].TransitionState < 0.0) Interface.CurrentHudElements[i].TransitionState = 0.0;
                                                    }
                                                    else
                                                    {
                                                        Interface.CurrentHudElements[i].TransitionState += speed * TimeElapsed;
                                                        if (Interface.CurrentHudElements[i].TransitionState > 1.0) Interface.CurrentHudElements[i].TransitionState = 1.0;
                                                    }
                                                    double d = TrainManager.PlayerTrain.StationDistanceToStopPoint;
                                                    double r;
                                                    if (d > 0.0)
                                                    {
                                                        r = d / Game.Stations[s].Stops[c].BackwardTolerance;
                                                    }
                                                    else
                                                    {
                                                        r = d / Game.Stations[s].Stops[c].ForwardTolerance;
                                                    }
                                                    if (r < -1.0) r = -1.0;
                                                    if (r > 1.0) r = 1.0;
                                                    y -= r * (double)Interface.CurrentHudElements[i].Value1;
                                                }
                                                else
                                                {
                                                    Interface.CurrentHudElements[i].TransitionState += speed * TimeElapsed;
                                                    if (Interface.CurrentHudElements[i].TransitionState > 1.0) Interface.CurrentHudElements[i].TransitionState = 1.0;
                                                }
                                            }
                                            else
                                            {
                                                Interface.CurrentHudElements[i].TransitionState += speed * TimeElapsed;
                                                if (Interface.CurrentHudElements[i].TransitionState > 1.0) Interface.CurrentHudElements[i].TransitionState = 1.0;
                                            }
                                            t = Interface.CurrentHudElements[i].Text;
                                        }
                                        break;
                                    case "clock":
                                        {
                                            int hours = (int)Math.Floor(Game.SecondsSinceMidnight);
                                            int seconds = hours % 60; hours /= 60;
                                            int minutes = hours % 60; hours /= 60;
                                            hours %= 24;
                                            t = hours.ToString(Culture).PadLeft(2, '0') + ":" + minutes.ToString(Culture).PadLeft(2, '0') + ":" + seconds.ToString(Culture).PadLeft(2, '0');
                                            if (OptionClock)
                                            {
                                                Interface.CurrentHudElements[i].TransitionState -= speed * TimeElapsed;
                                                if (Interface.CurrentHudElements[i].TransitionState < 0.0) Interface.CurrentHudElements[i].TransitionState = 0.0;
                                            }
                                            else
                                            {
                                                Interface.CurrentHudElements[i].TransitionState += speed * TimeElapsed;
                                                if (Interface.CurrentHudElements[i].TransitionState > 1.0) Interface.CurrentHudElements[i].TransitionState = 1.0;
                                            }
                                        }
                                        break;
                                    case "speed":
                                        if (OptionSpeed == SpeedDisplayMode.Kmph)
                                        {
                                            double kmph = Math.Abs(TrainManager.PlayerTrain.Specs.CurrentAverageSpeed) * 3.6;
                                            t = kmph.ToString("0.00", Culture) + " km/h";
                                            Interface.CurrentHudElements[i].TransitionState -= speed * TimeElapsed;
                                            if (Interface.CurrentHudElements[i].TransitionState < 0.0) Interface.CurrentHudElements[i].TransitionState = 0.0;
                                        }
                                        else if (OptionSpeed == SpeedDisplayMode.Mph)
                                        {
                                            double mph = Math.Abs(TrainManager.PlayerTrain.Specs.CurrentAverageSpeed) * 2.2369362920544;
                                            t = mph.ToString("0.00", Culture) + " mph";
                                            Interface.CurrentHudElements[i].TransitionState -= speed * TimeElapsed;
                                            if (Interface.CurrentHudElements[i].TransitionState < 0.0) Interface.CurrentHudElements[i].TransitionState = 0.0;
                                        }
                                        else
                                        {
                                            double mph = Math.Abs(TrainManager.PlayerTrain.Specs.CurrentAverageSpeed) * 2.2369362920544;
                                            t = mph.ToString("0.00", Culture) + " mph";
                                            Interface.CurrentHudElements[i].TransitionState += speed * TimeElapsed;
                                            if (Interface.CurrentHudElements[i].TransitionState > 1.0) Interface.CurrentHudElements[i].TransitionState = 1.0;
                                        }
                                        break;
                                    case "fps":
                                        int fps = (int)Math.Round(Game.InfoFrameRate);
                                        t = fps.ToString(Culture) + " fps";
                                        if (OptionFrameRates)
                                        {
                                            Interface.CurrentHudElements[i].TransitionState -= speed * TimeElapsed;
                                            if (Interface.CurrentHudElements[i].TransitionState < 0.0) Interface.CurrentHudElements[i].TransitionState = 0.0;
                                        }
                                        else
                                        {
                                            Interface.CurrentHudElements[i].TransitionState += speed * TimeElapsed;
                                            if (Interface.CurrentHudElements[i].TransitionState > 1.0) Interface.CurrentHudElements[i].TransitionState = 1.0;
                                        }
                                        break;
                                    case "ai":
                                        t = "A.I.";
                                        if (TrainManager.PlayerTrain.AI != null)
                                        {
                                            Interface.CurrentHudElements[i].TransitionState -= speed * TimeElapsed;
                                            if (Interface.CurrentHudElements[i].TransitionState < 0.0) Interface.CurrentHudElements[i].TransitionState = 0.0;
                                        }
                                        else
                                        {
                                            Interface.CurrentHudElements[i].TransitionState += speed * TimeElapsed;
                                            if (Interface.CurrentHudElements[i].TransitionState > 1.0) Interface.CurrentHudElements[i].TransitionState = 1.0;
                                        }
                                        break;
                                    case "score":
                                        if (Interface.CurrentOptions.GameMode == Interface.GameMode.Arcade)
                                        {
                                            t = Game.CurrentScore.Value.ToString(Culture) + " / " + Game.CurrentScore.Maximum.ToString(Culture);
                                            if (Game.CurrentScore.Value < 0)
                                            {
                                                sc = Game.MessageColor.Red;
                                            }
                                            else if (Game.CurrentScore.Value > 0)
                                            {
                                                sc = Game.MessageColor.Green;
                                            }
                                            else
                                            {
                                                sc = Game.MessageColor.Gray;
                                            }
                                            Interface.CurrentHudElements[i].TransitionState = 0.0;
                                        }
                                        else
                                        {
                                            Interface.CurrentHudElements[i].TransitionState = 1.0;
                                            t = "";
                                        }
                                        break;
                                    default:
                                        t = Interface.CurrentHudElements[i].Text;
                                        break;
                                }
                                // transitions
                                float alpha = 1.0f;
                                if ((Interface.CurrentHudElements[i].Transition & Interface.HudTransition.Move) != 0)
                                {
                                    double s = Interface.CurrentHudElements[i].TransitionState;
                                    x += Interface.CurrentHudElements[i].TransitionVector.X * s * s;
                                    y += Interface.CurrentHudElements[i].TransitionVector.Y * s * s;
                                }
                                if ((Interface.CurrentHudElements[i].Transition & Interface.HudTransition.Fade) != 0)
                                {
                                    alpha = (float)(1.0 - Interface.CurrentHudElements[i].TransitionState);
                                }
                                else if (Interface.CurrentHudElements[i].Transition == Interface.HudTransition.None)
                                {
                                    alpha = (float)(1.0 - Interface.CurrentHudElements[i].TransitionState);
                                }
                                /// render
                                if (alpha != 0.0f)
                                {
                                    // background
                                    if (Interface.CurrentHudElements[i].CenterMiddle.BackgroundTextureIndex >= 0)
                                    {
                                        int OpenGlTextureIndex = TextureManager.UseTexture(Interface.CurrentHudElements[i].CenterMiddle.BackgroundTextureIndex, TextureManager.UseMode.LoadImmediately);
                                        float r, g, b, a;
                                        CreateBackColor(Interface.CurrentHudElements[i].BackgroundColor, sc, out r, out g, out b, out a);
                                        Gl.glColor4f(r, g, b, a * alpha);
                                        RenderOverlayTexture(Interface.CurrentHudElements[i].CenterMiddle.BackgroundTextureIndex, x, y, x + w, y + h);
                                    }
                                    { // text
                                        float u, v;
                                        MeasureString(t, Interface.CurrentHudElements[i].TextSize, out u, out v);
                                        double p = Math.Round(Interface.CurrentHudElements[i].TextAlignment.X < 0 ? x : Interface.CurrentHudElements[i].TextAlignment.X == 0 ? x + 0.5 * (w - u) : x + w - u);
                                        double q = Math.Round(Interface.CurrentHudElements[i].TextAlignment.Y < 0 ? y : Interface.CurrentHudElements[i].TextAlignment.Y == 0 ? y + 0.5 * (h - v) : y + h - v);
                                        p += Interface.CurrentHudElements[i].TextPosition.X;
                                        q += Interface.CurrentHudElements[i].TextPosition.Y;
                                        float r, g, b, a;
                                        CreateTextColor(Interface.CurrentHudElements[i].TextColor, sc, out r, out g, out b, out a);
                                        RenderString(p, q, Interface.CurrentHudElements[i].TextSize, t, -1, r, g, b, a * alpha, Interface.CurrentHudElements[i].TextShadow);
                                    }
                                    // overlay
                                    if (Interface.CurrentHudElements[i].CenterMiddle.OverlayTextureIndex >= 0)
                                    {
                                        int OpenGlTextureIndex = TextureManager.UseTexture(Interface.CurrentHudElements[i].CenterMiddle.OverlayTextureIndex, TextureManager.UseMode.LoadImmediately);
                                        float r, g, b, a;
                                        CreateBackColor(Interface.CurrentHudElements[i].OverlayColor, sc, out r, out g, out b, out a);
                                        Gl.glColor4f(r, g, b, a * alpha);
                                        RenderOverlayTexture(Interface.CurrentHudElements[i].CenterMiddle.OverlayTextureIndex, x, y, x + w, y + h);
                                    }
                                }
                            }
                            break;
                    }
                }
                // marker
                if (Interface.CurrentOptions.GameMode != Interface.GameMode.Expert)
                {
                    double y = 8.0;
                    for (int i = 0; i < Game.MarkerTextures.Length; i++)
                    {
                        int t = TextureManager.UseTexture(Game.MarkerTextures[i], TextureManager.UseMode.LoadImmediately);
                        if (t >= 0)
                        {
                            double w = (double)TextureManager.Textures[Game.MarkerTextures[i]].ClipWidth;
                            double h = (double)TextureManager.Textures[Game.MarkerTextures[i]].ClipHeight;
                            Gl.glColor4f(1.0f, 1.0f, 1.0f, 1.0f);
                            RenderOverlayTexture(Game.MarkerTextures[i], (double)ScreenWidth - w - 8.0, y, (double)ScreenWidth - 8.0, y + h);
                            y += h + 8.0;
                        }
                    }
                }
                // timetable
                if (Timetable.CurrentTimetable == Timetable.TimetableState.Default)
                {
                    // default
                    int t = Timetable.DefaultTimetableTexture;
                    if (t >= 0)
                    {
                        int w = TextureManager.Textures[t].ClipWidth;
                        int h = TextureManager.Textures[t].ClipHeight;
                        Gl.glColor4f(1.0f, 1.0f, 1.0f, 1.0f);
                        RenderOverlayTexture(t, (double)(ScreenWidth - w), Timetable.DefaultTimetablePosition, (double)ScreenWidth, (double)h + Timetable.DefaultTimetablePosition);
                    }
                }
                else if (Timetable.CurrentTimetable == Timetable.TimetableState.Custom & Timetable.CustomObjectsUsed == 0)
                {
                    // custom
                    int td = Timetable.CurrentCustomTimetableDaytimeTextureIndex;
                    if (td >= 0)
                    {
                        int w = TextureManager.Textures[td].ClipWidth;
                        int h = TextureManager.Textures[td].ClipHeight;
                        Gl.glColor4f(1.0f, 1.0f, 1.0f, 1.0f);
                        RenderOverlayTexture(td, (double)(ScreenWidth - w), Timetable.CustomTimetablePosition, (double)ScreenWidth, (double)h + Timetable.CustomTimetablePosition);
                    }
                    int tn = Timetable.CurrentCustomTimetableDaytimeTextureIndex;
                    if (tn >= 0)
                    {
                        int w = TextureManager.Textures[tn].ClipWidth;
                        int h = TextureManager.Textures[tn].ClipHeight;
                        float alpha;
                        if (td >= 0)
                        {
                            double t = (TrainManager.PlayerTrain.Cars[TrainManager.PlayerTrain.DriverCar].FrontAxle.Follower.TrackPosition - TrainManager.PlayerTrain.Cars[TrainManager.PlayerTrain.DriverCar].Brightness.PreviousTrackPosition) / (TrainManager.PlayerTrain.Cars[TrainManager.PlayerTrain.DriverCar].Brightness.NextTrackPosition - TrainManager.PlayerTrain.Cars[TrainManager.PlayerTrain.DriverCar].Brightness.PreviousTrackPosition);
                            alpha = (float)((1.0 - t) * TrainManager.PlayerTrain.Cars[TrainManager.PlayerTrain.DriverCar].Brightness.PreviousBrightness + t * TrainManager.PlayerTrain.Cars[TrainManager.PlayerTrain.DriverCar].Brightness.NextBrightness);
                        }
                        else
                        {
                            alpha = 1.0f;
                        }
                        Gl.glColor4f(1.0f, 1.0f, 1.0f, alpha);
                        RenderOverlayTexture(tn, (double)(ScreenWidth - w), Timetable.CustomTimetablePosition, (double)ScreenWidth, (double)h + Timetable.CustomTimetablePosition);
                    }
                }
            }
            else if (CurrentOutputMode == OutputMode.Debug)
            {
                // debug
                Gl.glColor4d(0.5, 0.5, 0.5, 0.5);
                RenderOverlaySolid(0.0f, 0.0f, (double)ScreenWidth, (double)ScreenHeight);
                // actual handles
                {
                    string t = "actual: " + (TrainManager.PlayerTrain.Specs.CurrentReverser.Actual == -1 ? "B" : TrainManager.PlayerTrain.Specs.CurrentReverser.Actual == 1 ? "F" : "N");
                    if (TrainManager.PlayerTrain.Specs.SingleHandle)
                    {
                        t += " - " + (TrainManager.PlayerTrain.Specs.CurrentEmergencyBrake.Actual ? "EMG" : TrainManager.PlayerTrain.Specs.CurrentBrakeNotch.Actual != 0 ? "B" + TrainManager.PlayerTrain.Specs.CurrentBrakeNotch.Actual.ToString(Culture) : TrainManager.PlayerTrain.Specs.CurrentHoldBrake.Actual ? "HLD" : TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Actual != 0 ? "P" + TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Actual.ToString(Culture) : "N");
                    }
                    else if (TrainManager.PlayerTrain.Cars[TrainManager.PlayerTrain.DriverCar].Specs.BrakeType == TrainManager.CarBrakeType.AutomaticAirBrake)
                    {
                        t += " - " + (TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Actual != 0 ? "P" + TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Actual.ToString(Culture) : "N");
                        t += " - " + (TrainManager.PlayerTrain.Specs.CurrentEmergencyBrake.Actual ? "EMG" : TrainManager.PlayerTrain.Specs.AirBrake.Handle.Actual == TrainManager.AirBrakeHandleState.Service ? "SRV" : TrainManager.PlayerTrain.Specs.AirBrake.Handle.Actual == TrainManager.AirBrakeHandleState.Lap ? "LAP" : "REL");
                    }
                    else
                    {
                        t += " - " + (TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Actual != 0 ? "P" + TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Actual.ToString(Culture) : "N");
                        t += " - " + (TrainManager.PlayerTrain.Specs.CurrentEmergencyBrake.Actual ? "EMG" : TrainManager.PlayerTrain.Specs.CurrentBrakeNotch.Actual != 0 ? "B" + TrainManager.PlayerTrain.Specs.CurrentBrakeNotch.Actual.ToString(Culture) : TrainManager.PlayerTrain.Specs.CurrentHoldBrake.Actual ? "HLD" : "N");
                    }
                    RenderString(2.0, ScreenHeight - 46.0, Fonts.FontType.Small, t, -1, 1.0f, 1.0f, 1.0f, true);
                }
                // safety handles
                {
                    string t = "safety: " + (TrainManager.PlayerTrain.Specs.CurrentReverser.Actual == -1 ? "B" : TrainManager.PlayerTrain.Specs.CurrentReverser.Actual == 1 ? "F" : "N");
                    if (TrainManager.PlayerTrain.Specs.SingleHandle)
                    {
                        t += " - " + (TrainManager.PlayerTrain.Specs.CurrentEmergencyBrake.Safety ? "EMG" : TrainManager.PlayerTrain.Specs.CurrentBrakeNotch.Safety != 0 ? "B" + TrainManager.PlayerTrain.Specs.CurrentBrakeNotch.Safety.ToString(Culture) : TrainManager.PlayerTrain.Specs.CurrentHoldBrake.Actual ? "HLD" : TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Safety != 0 ? "P" + TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Safety.ToString(Culture) : "N");
                    }
                    else if (TrainManager.PlayerTrain.Cars[TrainManager.PlayerTrain.DriverCar].Specs.BrakeType == TrainManager.CarBrakeType.AutomaticAirBrake)
                    {
                        t += " - " + (TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Safety != 0 ? "P" + TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Safety.ToString(Culture) : "N");
                        t += " - " + (TrainManager.PlayerTrain.Specs.CurrentEmergencyBrake.Safety ? "EMG" : TrainManager.PlayerTrain.Specs.AirBrake.Handle.Safety == TrainManager.AirBrakeHandleState.Service ? "SRV" : TrainManager.PlayerTrain.Specs.AirBrake.Handle.Safety == TrainManager.AirBrakeHandleState.Lap ? "LAP" : "REL");
                    }
                    else
                    {
                        t += " - " + (TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Safety != 0 ? "P" + TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Safety.ToString(Culture) : "N");
                        t += " - " + (TrainManager.PlayerTrain.Specs.CurrentEmergencyBrake.Safety ? "EMG" : TrainManager.PlayerTrain.Specs.CurrentBrakeNotch.Safety != 0 ? "B" + TrainManager.PlayerTrain.Specs.CurrentBrakeNotch.Safety.ToString(Culture) : TrainManager.PlayerTrain.Specs.CurrentHoldBrake.Actual ? "HLD" : "N");
                    }
                    RenderString(2.0, ScreenHeight - 32.0, Fonts.FontType.Small, t, -1, 1.0f, 1.0f, 1.0f, true);
                }
                // driver handles
                {
                    string t = "driver: " + (TrainManager.PlayerTrain.Specs.CurrentReverser.Driver == -1 ? "B" : TrainManager.PlayerTrain.Specs.CurrentReverser.Driver == 1 ? "F" : "N");
                    if (TrainManager.PlayerTrain.Specs.SingleHandle)
                    {
                        t += " - " + (TrainManager.PlayerTrain.Specs.CurrentEmergencyBrake.Driver ? "EMG" : TrainManager.PlayerTrain.Specs.CurrentBrakeNotch.Driver != 0 ? "B" + TrainManager.PlayerTrain.Specs.CurrentBrakeNotch.Driver.ToString(Culture) : TrainManager.PlayerTrain.Specs.CurrentHoldBrake.Driver ? "HLD" : TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Driver != 0 ? "P" + TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Driver.ToString(Culture) : "N");
                    }
                    else if (TrainManager.PlayerTrain.Cars[TrainManager.PlayerTrain.DriverCar].Specs.BrakeType == TrainManager.CarBrakeType.AutomaticAirBrake)
                    {
                        t += " - " + (TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Driver != 0 ? "P" + TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Driver.ToString(Culture) : "N");
                        t += " - " + (TrainManager.PlayerTrain.Specs.CurrentEmergencyBrake.Driver ? "EMG" : TrainManager.PlayerTrain.Specs.AirBrake.Handle.Driver == TrainManager.AirBrakeHandleState.Service ? "SRV" : TrainManager.PlayerTrain.Specs.AirBrake.Handle.Driver == TrainManager.AirBrakeHandleState.Lap ? "LAP" : "REL");
                    }
                    else
                    {
                        t += " - " + (TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Driver != 0 ? "P" + TrainManager.PlayerTrain.Specs.CurrentPowerNotch.Driver.ToString(Culture) : "N");
                        t += " - " + (TrainManager.PlayerTrain.Specs.CurrentEmergencyBrake.Driver ? "EMG" : TrainManager.PlayerTrain.Specs.CurrentBrakeNotch.Driver != 0 ? "B" + TrainManager.PlayerTrain.Specs.CurrentBrakeNotch.Driver.ToString(Culture) : TrainManager.PlayerTrain.Specs.CurrentHoldBrake.Driver ? "HLD" : "N");
                    }
                    RenderString(2.0, ScreenHeight - 18.0, Fonts.FontType.Small, t, -1, 1.0f, 1.0f, 1.0f, true);
                }
                // debug information
                int texturesLoaded = 0;
                int texturesRegistered = 0;
                for (int i = 0; i < TextureManager.Textures.Length; i++)
                {
                    if (TextureManager.Textures[i] != null)
                    {
                        if (TextureManager.Textures[i].Loaded)
                        {
                            texturesLoaded++;
                        }
                        texturesRegistered++;
                    }
                }
                int soundsPlaying = 0;
                int soundsRegistered = 0;
                for (int i = 0; i < SoundManager.SoundSources.Length; i++)
                {
                    if (SoundManager.SoundSources[i] != null)
                    {
                        if (!SoundManager.SoundSources[i].Suppressed)
                        {
                            soundsPlaying++;
                        }
                        soundsRegistered++;
                    }
                }
                int car = 0;
                for (int i = 0; i < TrainManager.PlayerTrain.Cars.Length; i++)
                {
                    if (TrainManager.PlayerTrain.Cars[i].Specs.IsMotorCar)
                    {
                        car = i;
                        break;
                    }
                }
                double mass = 0.0;
                for (int i = 0; i < TrainManager.PlayerTrain.Cars.Length; i++)
                {
                    mass += TrainManager.PlayerTrain.Cars[i].Specs.MassCurrent;
                }
                string[] Lines = new string[] {
                    "=system",
                    "fps: " + Game.InfoFrameRate.ToString("0.0", Culture) + (MainLoop.LimitFramerate ? " (low cpu)" : ""),
                    "score: " + Game.CurrentScore.Value.ToString(Culture),
                    "",
                    "=train",
                    "speed: " + (Math.Abs(TrainManager.PlayerTrain.Specs.CurrentAverageSpeed) * 3.6).ToString("0.00", Culture) + " km/h",
                    "power (car " + car.ToString(Culture) +  "): " + (TrainManager.PlayerTrain.Cars[car].Specs.CurrentAccelerationOutput < 0.0 ? TrainManager.PlayerTrain.Cars[car].Specs.CurrentAccelerationOutput * (double)Math.Sign(TrainManager.PlayerTrain.Cars[car].Specs.CurrentSpeed) : TrainManager.PlayerTrain.Cars[car].Specs.CurrentAccelerationOutput * (double)TrainManager.PlayerTrain.Specs.CurrentReverser.Actual).ToString("0.0000", Culture) + " m/s²",
                    "acceleration: " + TrainManager.PlayerTrain.Specs.CurrentAverageAcceleration.ToString("0.0000", Culture) + " m/s²",
                    "position: " + (TrainManager.PlayerTrain.Cars[0].FrontAxle.Follower.TrackPosition - TrainManager.PlayerTrain.Cars[0].FrontAxlePosition + 0.5 * TrainManager.PlayerTrain.Cars[0].Length).ToString("0.00", Culture) + " m",
                    "elevation: " + (Game.RouteInitialElevation + TrainManager.PlayerTrain.Cars[TrainManager.PlayerTrain.DriverCar].FrontAxle.Follower.WorldPosition.Y).ToString("0.00", Culture) + " m",
                    "temperature: " + (TrainManager.PlayerTrain.Specs.CurrentAirTemperature - 273.15).ToString("0.00", Culture) + " °C",
                    "air pressure: " + (0.001 * TrainManager.PlayerTrain.Specs.CurrentAirPressure).ToString("0.00", Culture) + " kPa",
                    "air density: " + TrainManager.PlayerTrain.Specs.CurrentAirDensity.ToString("0.0000", Culture) + " kg/m³",
                    "speed of sound: " + (Game.GetSpeedOfSound(TrainManager.PlayerTrain.Specs.CurrentAirDensity) * 3.6).ToString("0.00", Culture) + " km/h",
                    "passenger ratio: " + TrainManager.PlayerTrain.Passengers.PassengerRatio.ToString("0.00"),
                    "total mass: " + mass.ToString("0.00", Culture) + " kg",
                    "plugin: " + (TrainManager.PlayerTrain.Plugin != null ? ((TrainManager.PlayerTrain.Plugin.PluginValid ? "ok" : "error") + ", message: " + (TrainManager.PlayerTrain.Plugin.PluginMessage != null ? TrainManager.PlayerTrain.Plugin.PluginMessage : "n/a")) : "n/a"),
                    "",
                    "=route",
                    "track limit: " + (TrainManager.PlayerTrain.CurrentRouteLimit == double.PositiveInfinity ? "unlimited" : ((TrainManager.PlayerTrain.CurrentRouteLimit * 3.6).ToString("0.0", Culture) + " km/h")),
                    "signal limit: " + (TrainManager.PlayerTrain.CurrentSectionLimit == double.PositiveInfinity ? "unlimited" : ((TrainManager.PlayerTrain.CurrentSectionLimit * 3.6).ToString("0.0", Culture) + " km/h")),
                    "total static objects: " + ObjectManager.ObjectsUsed.ToString(Culture),
                    "total static GL_TRIANGLES: " + Game.InfoTotalTriangles.ToString(Culture),
                    "total static GL_TRIANGLE_STRIP: " + Game.InfoTotalTriangleStrip.ToString(Culture),
                    "total static GL_QUADS: " + Game.InfoTotalQuads.ToString(Culture),
                    "total static GL_QUAD_STRIP: " + Game.InfoTotalQuadStrip.ToString(Culture),
                    "total static GL_POLYGON: " + Game.InfoTotalPolygon.ToString(Culture),
                    "total animated objects: " + ObjectManager.AnimatedWorldObjectsUsed.ToString(Culture),
                    "",
                    "=renderer",
                    "static opaque faces: " + Game.InfoStaticOpaqueFaceCount.ToString(Culture),
                    "dynamic opaque faces: " + DynamicOpaque.FaceCount.ToString(Culture),
                    "dynamic alpha faces: " + DynamicAlpha.FaceCount.ToString(Culture),
                    "overlay opaque faces: " + OverlayOpaque.FaceCount.ToString(Culture),
                    "overlay alpha faces: " + OverlayAlpha.FaceCount.ToString(Culture),
                    "textures loaded: " + texturesLoaded.ToString(Culture),
                    "textures registered: " + texturesRegistered.ToString(Culture),
                    "",
                    "=camera",
                    "position: " + World.CameraTrackFollower.TrackPosition.ToString("0.00", Culture) + " m",
                    "curve radius: " + World.CameraTrackFollower.CurveRadius.ToString("0.00", Culture) + " m",
                    "curve cant: " + (1000.0 * Math.Abs(World.CameraTrackFollower.CurveCant)).ToString("0.00", Culture) + " mm" + (World.CameraTrackFollower.CurveCant < 0.0 ? " (left)" : World.CameraTrackFollower.CurveCant > 0.0 ? " (right)" : ""),
                    "",
                    "=sound",
                    "sounds playing: " + soundsPlaying.ToString(Culture),
                    "sounds registered: " + soundsRegistered.ToString(Culture),
                    "outer radius factor: " + SoundManager.OuterRadiusFactor.ToString("0.00"),
                    "",
                    Game.InfoDebugString ?? ""
                };
                double x = 4.0;
                double y = 4.0;
                for (int i = 0; i < Lines.Length; i++)
                {
                    if (Lines[i].Length != 0)
                    {
                        if (Lines[i][0] == '=')
                        {
                            string text = Lines[i].Substring(1);
                            float width, height;
                            MeasureString(text, Fonts.FontType.Small, out width, out height);
                            Gl.glColor4f(0.5f, 0.5f, 0.7f, 0.7f);
                            RenderOverlaySolid(x, y, x + width + 4.0f, y + height + 2.0f);
                            RenderString(x, y, Fonts.FontType.Small, text, -1, 1.0f, 1.0f, 1.0f, false);
                        }
                        else
                        {
                            RenderString(x, y, Fonts.FontType.Small, Lines[i], -1, 1.0f, 1.0f, 1.0f, true);
                        }
                        y += 14.0;
                    }
                    else if (y >= (double)ScreenHeight - 240.0)
                    {
                        x += 280.0;
                        y = 4.0;
                    }
                    else
                    {
                        y += 14.0;
                    }
                }
            }
            // air brake debug output
            if (Interface.CurrentOptions.GameMode != Interface.GameMode.Expert & OptionBrakeSystems)
            {
                double oy = 64.0, y = oy, h = 16.0;
                bool[] heading = new bool[6];
                for (int i = 0; i < TrainManager.PlayerTrain.Cars.Length; i++)
                {
                    double x = 96.0, w = 128.0;
                    // brake pipe
                    if (TrainManager.PlayerTrain.Cars[i].Specs.BrakeType == TrainManager.CarBrakeType.AutomaticAirBrake | TrainManager.PlayerTrain.Cars[i].Specs.BrakeType == TrainManager.CarBrakeType.ElectromagneticStraightAirBrake)
                    {
                        if (!heading[0])
                        {
                            RenderString(x, oy - 16.0, Fonts.FontType.Small, "Brake pipe", -1, 1.0f, 1.0f, 0.0f, true);
                            heading[0] = true;
                        }
                        Gl.glColor3f(0.0f, 0.0f, 0.0f);
                        RenderOverlaySolid(x, y, x + w, y + h);
                        double p = TrainManager.PlayerTrain.Cars[i].Specs.AirBrake.BrakePipeCurrentPressure;
                        double r = p / TrainManager.PlayerTrain.Cars[i].Specs.AirBrake.BrakePipeNormalPressure;
                        Gl.glColor3f(1.0f, 1.0f, 0.0f);
                        RenderOverlaySolid(x, y, x + r * w, y + h);
                    }
                    x += w + 8.0;
                    // auxillary reservoir
                    if (TrainManager.PlayerTrain.Cars[i].Specs.BrakeType == TrainManager.CarBrakeType.AutomaticAirBrake | TrainManager.PlayerTrain.Cars[i].Specs.BrakeType == TrainManager.CarBrakeType.ElectromagneticStraightAirBrake)
                    {
                        if (!heading[1])
                        {
                            RenderString(x, oy - 16.0, Fonts.FontType.Small, "Auxillary reservoir", -1, 0.75f, 0.75f, 0.75f, true);
                            heading[1] = true;
                        }
                        Gl.glColor3f(0.0f, 0.0f, 0.0f);
                        RenderOverlaySolid(x, y, x + w, y + h);
                        double p = TrainManager.PlayerTrain.Cars[i].Specs.AirBrake.AuxillaryReservoirCurrentPressure;
                        double r = p / TrainManager.PlayerTrain.Cars[i].Specs.AirBrake.AuxillaryReservoirMaximumPressure;
                        Gl.glColor3f(0.5f, 0.5f, 0.5f);
                        RenderOverlaySolid(x, y, x + r * w, y + h);
                    }
                    x += w + 8.0;
                    // brake cylinder
                    {
                        if (!heading[2])
                        {
                            RenderString(x, oy - 16.0, Fonts.FontType.Small, "Brake cylinder", -1, 0.75f, 0.5f, 0.25f, true);
                            heading[2] = true;
                        }
                        Gl.glColor3f(0.0f, 0.0f, 0.0f);
                        RenderOverlaySolid(x, y, x + w, y + h);
                        double p = TrainManager.PlayerTrain.Cars[i].Specs.AirBrake.BrakeCylinderCurrentPressure;
                        double r = p / TrainManager.PlayerTrain.Cars[i].Specs.AirBrake.BrakeCylinderEmergencyMaximumPressure;
                        Gl.glColor3f(0.75f, 0.5f, 0.25f);
                        RenderOverlaySolid(x, y, x + r * w, y + h);
                    }
                    x += w + 8.0;
                    // main reservoir
                    if (TrainManager.PlayerTrain.Cars[i].Specs.AirBrake.Type == TrainManager.AirBrakeType.Main)
                    {
                        if (!heading[3])
                        {
                            RenderString(x, oy - 16.0, Fonts.FontType.Small, "Main reservoir", -1, 1.0f, 0.0f, 0.0f, true);
                            heading[3] = true;
                        }
                        Gl.glColor3f(0.0f, 0.0f, 0.0f);
                        RenderOverlaySolid(x, y, x + w, y + h);
                        double p = TrainManager.PlayerTrain.Cars[i].Specs.AirBrake.MainReservoirCurrentPressure;
                        double r = p / TrainManager.PlayerTrain.Cars[i].Specs.AirBrake.AirCompressorMaximumPressure;
                        Gl.glColor3f(1.0f, 0.0f, 0.0f);
                        RenderOverlaySolid(x, y, x + r * w, y + h);
                    }
                    x += w + 8.0;
                    // equalizing reservoir
                    if (TrainManager.PlayerTrain.Cars[i].Specs.AirBrake.Type == TrainManager.AirBrakeType.Main)
                    {
                        if (!heading[4])
                        {
                            RenderString(x, oy - 16.0, Fonts.FontType.Small, "Equalizing reservoir", -1, 0.0f, 0.75f, 0.0f, true);
                            heading[4] = true;
                        }
                        Gl.glColor3f(0.0f, 0.0f, 0.0f);
                        RenderOverlaySolid(x, y, x + w, y + h);
                        double p = TrainManager.PlayerTrain.Cars[i].Specs.AirBrake.EqualizingReservoirCurrentPressure;
                        double r = p / TrainManager.PlayerTrain.Cars[i].Specs.AirBrake.EqualizingReservoirNormalPressure;
                        Gl.glColor3f(0.0f, 0.75f, 0.0f);
                        RenderOverlaySolid(x, y, x + r * w, y + h);
                    }
                    x += w + 8.0;
                    // straight air pipe
                    if (TrainManager.PlayerTrain.Cars[i].Specs.BrakeType == TrainManager.CarBrakeType.ElectromagneticStraightAirBrake & TrainManager.PlayerTrain.Cars[i].Specs.AirBrake.Type == TrainManager.AirBrakeType.Main)
                    {
                        if (!heading[5])
                        {
                            RenderString(x, oy - 16.0, Fonts.FontType.Small, "Straight air pipe", -1, 0.0f, 0.75f, 1.0f, true);
                            heading[5] = true;
                        }
                        Gl.glColor3f(0.0f, 0.0f, 0.0f);
                        RenderOverlaySolid(x, y, x + w, y + h);
                        double p = TrainManager.PlayerTrain.Cars[i].Specs.AirBrake.StraightAirPipeCurrentPressure;
                        double r = p / TrainManager.PlayerTrain.Cars[i].Specs.AirBrake.BrakeCylinderEmergencyMaximumPressure;
                        Gl.glColor3f(0.0f, 0.75f, 1.0f);
                        RenderOverlaySolid(x, y, x + r * w, y + h);
                    }
                    x += w + 8.0;
                    Gl.glColor3f(0.0f, 0.0f, 0.0f);
                    y += h + 8.0;
                }
            }
            // interface
            if (Game.CurrentInterface == Game.InterfaceType.Pause)
            {
                // pause
                Gl.glColor4f(0.0f, 0.0f, 0.0f, 0.5f);
                RenderOverlaySolid(0.0, 0.0, (double)ScreenWidth, (double)ScreenHeight);
                Gl.glColor4f(1.0f, 1.0f, 1.0f, 1.0f);
                if (TexturePause >= 0)
                {
                    double w = (double)TextureManager.Textures[TexturePause].ClipWidth;
                    double h = (double)TextureManager.Textures[TexturePause].ClipHeight;
                    double x = 0.5 * (double)ScreenWidth;
                    double y = 0.5 * (double)ScreenHeight;
                    RenderOverlayTexture(TexturePause, x - 0.5 * w, y - 0.5 * h, x + 0.5 * w, y + 0.5 * h);
                }
                else
                {
                    string s = "PAUSE";
                    RenderString(0.5 * (double)ScreenWidth, 0.5 * (double)ScreenHeight, Fonts.FontType.Large, s, 0, 1.0f, 1.0f, 1.0f, true);
                }
            }
            else if (Game.CurrentInterface == Game.InterfaceType.Menu)
            {
                // menu
                Gl.glColor4f(0.0f, 0.0f, 0.0f, 0.5f);
                RenderOverlaySolid(0.0, 0.0, (double)ScreenWidth, (double)ScreenHeight);
                Gl.glColor4f(1.0f, 1.0f, 1.0f, 1.0f);
                Game.MenuEntry[] m = Game.CurrentMenu;
                const double ra = 32.0;
                const double rb = 96.0;
                const double rc = 144.0;
                const double w = 256.0;
                const double h = 28.0;
                double x = 0.5 * (double)ScreenWidth - w;
                for (int i = 0; i < Game.CurrentMenuSelection.Length; i++)
                {
                    double o = Game.CurrentMenuOffsets[i];
                    double oc = o == double.NegativeInfinity ? 0.0 : o;
                    double y = 0.5 * ((double)ScreenHeight - (double)m.Length * h) + oc;
                    double ys = y + (double)Game.CurrentMenuSelection[i] * h;
                    double ot;
                    if (ys < rc)
                    {
                        ot = oc + rc - ys;
                    }
                    else if (ys > (double)ScreenHeight - rc)
                    {
                        ot = oc + (double)ScreenHeight - rc - ys;
                    }
                    else
                    {
                        ot = oc;
                    }
                    if (o == double.NegativeInfinity)
                    {
                        o = ot;
                    }
                    else if (o < ot)
                    {
                        double d = ot - o;
                        o += (0.03 * d * d + 0.1) * TimeElapsed;
                        if (o > ot) o = ot;
                    }
                    else if (o > ot)
                    {
                        double d = o - ot;
                        o -= (0.03 * d * d + 0.1) * TimeElapsed;
                        if (o < ot) o = ot;
                    }
                    Game.CurrentMenuOffsets[i] = o;
                    for (int j = 0; j < m.Length; j++)
                    {
                        double ta;
                        if (y < rb)
                        {
                            ta = (y - ra) / (rb - ra);
                            if (ta < 0.0) ta = 0.0;
                            if (ta > 1.0) ta = 1.0;
                        }
                        else if (y > (double)ScreenHeight - rb)
                        {
                            ta = ((double)ScreenHeight - y - ra) / (rb - ra);
                            if (ta < 0.0) ta = 0.0;
                            if (ta > 1.0) ta = 1.0;
                        }
                        else
                        {
                            ta = 1.0;
                        }
                        if (ta < m[j].Alpha)
                        {
                            m[j].Alpha -= 4.0 * TimeElapsed;
                            if (m[j].Alpha < ta) m[j].Alpha = ta;
                        }
                        else if (ta > m[j].Alpha)
                        {
                            m[j].Alpha += 4.0 * TimeElapsed;
                            if (m[j].Alpha > ta) m[j].Alpha = ta;
                        }
                        if (j == Game.CurrentMenuSelection[i])
                        {
                            m[j].Highlight = 1.0;
                        }
                        else
                        {
                            m[j].Highlight -= 4.0 * TimeElapsed;
                            if (m[j].Highlight < 0.0) m[j].Highlight = 0.0;
                        }
                        float r = 1.0f;
                        float g = 1.0f;
                        float b = (float)(1.0 - m[j].Highlight);
                        float a = (float)m[j].Alpha;
                        if (j == Game.CurrentMenuSelection[i])
                        {
                            RenderString(x, y, Fonts.FontType.Medium, "➢", -1, r, g, b, a, true);
                        }
                        if (m[j] is Game.MenuCaption)
                        {
                            RenderString(x + 24.0, y, Fonts.FontType.Medium, m[j].Text, -1, 0.5f, 0.75f, 1.0f, a, true);
                        }
                        else if (m[j] is Game.MenuCommand)
                        {
                            RenderString(x + 24.0, y, Fonts.FontType.Medium, m[j].Text, -1, r, g, b, a, true);
                        }
                        else
                        {
                            RenderString(x + 24.0, y, Fonts.FontType.Medium, m[j].Text + " ➟", -1, r, g, b, a, true);
                        }
                        y += h;
                    }
                    x += w;
                    Game.MenuSubmenu n = m[Game.CurrentMenuSelection[i]] as Game.MenuSubmenu;
                    m = n == null ? null : n.Entries;
                }
            }
            // fade to black on change ends
            if (TrainManager.PlayerTrain.Station >= 0 && Game.Stations[TrainManager.PlayerTrain.Station].StationType == Game.StationType.ChangeEnds && TrainManager.PlayerTrain.StationState == TrainManager.TrainStopState.Boarding)
            {
                double time = TrainManager.PlayerTrain.StationDepartureTime - Game.SecondsSinceMidnight;
                if (time < 1.0)
                {
                    FadeToBlackDueToChangeEnds = Math.Max(0.0, 1.0 - time);
                }
                else if (FadeToBlackDueToChangeEnds > 0.0)
                {
                    FadeToBlackDueToChangeEnds -= TimeElapsed;
                    if (FadeToBlackDueToChangeEnds < 0.0)
                    {
                        FadeToBlackDueToChangeEnds = 0.0;
                    }
                }
            }
            else if (FadeToBlackDueToChangeEnds > 0.0)
            {
                FadeToBlackDueToChangeEnds -= TimeElapsed;
                if (FadeToBlackDueToChangeEnds < 0.0)
                {
                    FadeToBlackDueToChangeEnds = 0.0;
                }
            }
            if (FadeToBlackDueToChangeEnds > 0.0 & (World.CameraMode == World.CameraViewMode.Interior | World.CameraMode == World.CameraViewMode.InteriorLookAhead))
            {
                Gl.glColor4d(0.0, 0.0, 0.0, FadeToBlackDueToChangeEnds);
                RenderOverlaySolid(0.0, 0.0, (double)ScreenWidth, (double)ScreenHeight);
            }
            // finalize
            Gl.glPopMatrix();
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glPopMatrix();
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glDisable(Gl.GL_BLEND);
        }

        // get color
        private static void CreateBackColor(Colors.ColorRGBA Original, Game.MessageColor SystemColor, out float R, out float G, out float B, out float A)
        {
            if (Original.R == 0 & Original.G == 0 & Original.B == 0)
            {
                switch (SystemColor)
                {
                    case Game.MessageColor.Black:
                        R = 0.0f; G = 0.0f; B = 0.0f;
                        break;
                    case Game.MessageColor.Gray:
                        R = 0.4f; G = 0.4f; B = 0.4f;
                        break;
                    case Game.MessageColor.White:
                        R = 1.0f; G = 1.0f; B = 1.0f;
                        break;
                    case Game.MessageColor.Red:
                        R = 1.0f; G = 0.0f; B = 0.0f;
                        break;
                    case Game.MessageColor.Orange:
                        R = 0.9f; G = 0.7f; B = 0.0f;
                        break;
                    case Game.MessageColor.Green:
                        R = 0.2f; G = 0.8f; B = 0.0f;
                        break;
                    case Game.MessageColor.Blue:
                        R = 0.0f; G = 0.7f; B = 1.0f;
                        break;
                    case Game.MessageColor.Magenta:
                        R = 1.0f; G = 0.0f; B = 0.7f;
                        break;
                    default:
                        R = 1.0f; G = 1.0f; B = 1.0f;
                        break;
                }
            }
            else
            {
                R = inv255 * (float)Original.R;
                G = inv255 * (float)Original.G;
                B = inv255 * (float)Original.B;
            }
            A = inv255 * (float)Original.A;
        }
        private static void CreateTextColor(Colors.ColorRGBA Original, Game.MessageColor SystemColor, out float R, out float G, out float B, out float A)
        {
            if (Original.R == 0 & Original.G == 0 & Original.B == 0)
            {
                switch (SystemColor)
                {
                    case Game.MessageColor.Black:
                        R = 0.0f; G = 0.0f; B = 0.0f;
                        break;
                    case Game.MessageColor.Gray:
                        R = 0.4f; G = 0.4f; B = 0.4f;
                        break;
                    case Game.MessageColor.White:
                        R = 1.0f; G = 1.0f; B = 1.0f;
                        break;
                    case Game.MessageColor.Red:
                        R = 1.0f; G = 0.0f; B = 0.0f;
                        break;
                    case Game.MessageColor.Orange:
                        R = 0.9f; G = 0.7f; B = 0.0f;
                        break;
                    case Game.MessageColor.Green:
                        R = 0.3f; G = 1.0f; B = 0.0f;
                        break;
                    case Game.MessageColor.Blue:
                        R = 1.0f; G = 1.0f; B = 1.0f;
                        break;
                    case Game.MessageColor.Magenta:
                        R = 1.0f; G = 0.0f; B = 0.7f;
                        break;
                    default:
                        R = 1.0f; G = 1.0f; B = 1.0f;
                        break;
                }
            }
            else
            {
                R = inv255 * (float)Original.R;
                G = inv255 * (float)Original.G;
                B = inv255 * (float)Original.B;
            }
            A = inv255 * (float)Original.A;
        }

        // render string
        private static void RenderString(double PixelLeft, double PixelTop, Fonts.FontType FontType, string Text, int Orientation, float R, float G, float B, bool Shadow)
        {
            RenderString(PixelLeft, PixelTop, FontType, Text, Orientation, R, G, B, 1.0f, Shadow);
        }
        private static void RenderString(double PixelLeft, double PixelTop, Fonts.FontType FontType, string Text, int Orientation, float R, float G, float B, float A, bool Shadow)
        {
            if (Text == null) return;
            int Font = (int)FontType;
            double c = 1;
            double x = PixelLeft;
            double y = PixelTop;
            double tw = 0.0;
            for (int i = 0; i < Text.Length; i++)
            {
                int a = char.ConvertToUtf32(Text, i);
                if (a >= 0x10000)
                {
                    i++;
                }
                Fonts.GetTextureIndex(FontType, a);
                tw += Fonts.Characters[Font][a].Width;
            }
            if (Orientation == 0)
            {
                x -= 0.5 * tw;
            }
            else if (Orientation == 1)
            {
                x -= tw;
            }
            for (int i = 0; i < Text.Length; i++)
            {
                int b = char.ConvertToUtf32(Text, i);
                if (b >= 0x10000)
                {
                    i++;
                }
                int t = Fonts.GetTextureIndex(FontType, b);
                double w = (double)TextureManager.Textures[t].ClipWidth;
                double h = (double)TextureManager.Textures[t].ClipHeight;
                Gl.glBlendFunc(Gl.GL_ZERO, Gl.GL_ONE_MINUS_SRC_COLOR);
                Gl.glColor3f(A, A, A);
                RenderOverlayTexture(t, x, y, x + w, y + h);
                if (Shadow)
                {
                    RenderOverlayTexture(t, x + c, y + c, x + w, y + h);
                }
                Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE);
                Gl.glColor4f(R, G, B, A);
                RenderOverlayTexture(t, x, y, x + w, y + h);
                x += Fonts.Characters[Font][b].Width;
            }
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
        }
        private static void MeasureString(string Text, Fonts.FontType FontType, out float Width, out float Height)
        {
            Width = 0.0f;
            Height = 0.0f;
            if (Text == null)
            {
                return;
            }
            int Font = (int)FontType;
            for (int i = 0; i < Text.Length; i++)
            {
                int Codepoint = char.ConvertToUtf32(Text, i);
                if (Codepoint >= 0x10000)
                {
                    i++;
                }
                int Texture = Fonts.GetTextureIndex(FontType, Codepoint);
                Width += Fonts.Characters[Font][Codepoint].Width;
                if (Fonts.Characters[Font][Codepoint].Height > Height)
                {
                    Height = Fonts.Characters[Font][Codepoint].Height;
                }
            }
        }

        // render overlay texture
        private static void RenderOverlayTexture(int TextureIndex, double ax, double ay, double bx, double by)
        {
            double nay = (double)ScreenHeight - ay;
            double nby = (double)ScreenHeight - by;
            TextureManager.UseTexture(TextureIndex, TextureManager.UseMode.LoadImmediately);
            if (TextureIndex >= 0)
            {
                int OpenGlTextureIndex = TextureManager.Textures[TextureIndex].OpenGlTextureIndex;
                if (!TexturingEnabled)
                {
                    Gl.glEnable(Gl.GL_TEXTURE_2D);
                    TexturingEnabled = true;
                }
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, OpenGlTextureIndex);
            }
            else if (TexturingEnabled)
            {
                Gl.glDisable(Gl.GL_TEXTURE_2D);
                TexturingEnabled = false;
            }
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2d(0.0, 1.0);
            Gl.glVertex2d(ax, nby);
            Gl.glTexCoord2d(0.0, 0.0);
            Gl.glVertex2d(ax, nay);
            Gl.glTexCoord2d(1.0, 0.0);
            Gl.glVertex2d(bx, nay);
            Gl.glTexCoord2d(1.0, 1.0);
            Gl.glVertex2d(bx, nby);
            Gl.glEnd();
        }

        // render overlay solid
        private static void RenderOverlaySolid(double ax, double ay, double bx, double by)
        {
            double nay = (double)ScreenHeight - ay;
            double nby = (double)ScreenHeight - by;
            if (TexturingEnabled)
            {
                Gl.glDisable(Gl.GL_TEXTURE_2D);
                TexturingEnabled = false;
            }
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glVertex2d(ax, nby);
            Gl.glVertex2d(ax, nay);
            Gl.glVertex2d(bx, nay);
            Gl.glVertex2d(bx, nby);
            Gl.glEnd();
        }

        // re-add objects
        internal static void ReAddObjects()
        {
            Object[] list = new Object[ObjectCount];
            for (int i = 0; i < ObjectCount; i++)
            {
                list[i] = Objects[i];
            }
            for (int i = 0; i < list.Length; i++)
            {
                HideObject(list[i].ObjectIndex);
            }
            for (int i = 0; i < list.Length; i++)
            {
                ShowObject(list[i].ObjectIndex, list[i].Type);
            }
        }

        // show object
        internal static void ShowObject(int ObjectIndex, ObjectType Type)
        {
            if (ObjectManager.Objects[ObjectIndex] == null)
            {
                return;
            }
            if (ObjectManager.Objects[ObjectIndex].RendererIndex == 0)
            {
                if (ObjectCount >= Objects.Length)
                {
                    Array.Resize<Object>(ref Objects, Objects.Length << 1);
                }
                Objects[ObjectCount].ObjectIndex = ObjectIndex;
                Objects[ObjectCount].Type = Type;
                int f = ObjectManager.Objects[ObjectIndex].Mesh.Faces.Length;
                Objects[ObjectCount].FaceListReferences = new ObjectListReference[f];
                for (int i = 0; i < f; i++)
                {
                    bool alpha = false;
                    if (Type == ObjectType.Overlay & World.CameraRestriction != World.CameraRestrictionMode.NotAvailable)
                    {
                        alpha = true;
                    }
                    else
                    {
                        int k = ObjectManager.Objects[ObjectIndex].Mesh.Faces[i].Material;
                        if (ObjectManager.Objects[ObjectIndex].Mesh.Materials[k].Color.A != 255)
                        {
                            alpha = true;
                        }
                        else if (ObjectManager.Objects[ObjectIndex].Mesh.Materials[k].BlendMode == World.MeshMaterialBlendMode.Additive)
                        {
                            alpha = true;
                        }
                        else if (ObjectManager.Objects[ObjectIndex].Mesh.Materials[k].GlowAttenuationData != 0)
                        {
                            alpha = true;
                        }
                        else
                        {
                            int tday = ObjectManager.Objects[ObjectIndex].Mesh.Materials[k].DaytimeTextureIndex;
                            if (tday >= 0)
                            {
                                TextureManager.UseTexture(tday, TextureManager.UseMode.LoadImmediately);
                                if (TextureManager.Textures[tday].Transparency == TextureManager.TextureTransparencyMode.Alpha)
                                {
                                    alpha = true;
                                }
                                else if (TextureManager.Textures[tday].Transparency == TextureManager.TextureTransparencyMode.TransparentColor & Interface.CurrentOptions.TransparencyMode == TransparencyMode.Quality)
                                {
                                    alpha = true;
                                }
                            }
                            int tnight = ObjectManager.Objects[ObjectIndex].Mesh.Materials[k].NighttimeTextureIndex;
                            if (tnight >= 0)
                            {
                                TextureManager.UseTexture(tnight, TextureManager.UseMode.LoadImmediately);
                                if (TextureManager.Textures[tnight].Transparency == TextureManager.TextureTransparencyMode.Alpha)
                                {
                                    alpha = true;
                                }
                                else if (TextureManager.Textures[tnight].Transparency == TextureManager.TextureTransparencyMode.TransparentColor & Interface.CurrentOptions.TransparencyMode == TransparencyMode.Quality)
                                {
                                    alpha = true;
                                }
                            }
                        }
                    }
                    ObjectListType listType;
                    switch (Type)
                    {
                        case ObjectType.Static:
                            listType = alpha ? ObjectListType.DynamicAlpha : ObjectListType.StaticOpaque;
                            break;
                        case ObjectType.Dynamic:
                            listType = alpha ? ObjectListType.DynamicAlpha : ObjectListType.DynamicOpaque;
                            break;
                        case ObjectType.Overlay:
                            listType = alpha ? ObjectListType.OverlayAlpha : ObjectListType.OverlayOpaque;
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                    if (listType == ObjectListType.StaticOpaque)
                    {
                        int k = ObjectManager.Objects[ObjectIndex].Mesh.Faces[i].Material;
                        int tday = ObjectManager.Objects[ObjectIndex].Mesh.Materials[k].DaytimeTextureIndex;
                        if (tday >= 0)
                        {
                            TextureManager.Textures[tday].DontAllowUnload = true;
                        }
                        int tnight = ObjectManager.Objects[ObjectIndex].Mesh.Materials[k].NighttimeTextureIndex;
                        if (tnight >= 0)
                        {
                            TextureManager.Textures[tnight].DontAllowUnload = true;
                        }
                    }
                    if (listType == ObjectListType.StaticOpaque)
                    {
                        /*
						 * For the static opaque list, insert the face into
						 * the first vacant position in the matching group's list.
						 * */
                        int groupIndex = (int)ObjectManager.Objects[ObjectIndex].GroupIndex;
                        if (groupIndex >= StaticOpaque.Length)
                        {
                            if (StaticOpaque.Length == 0)
                            {
                                StaticOpaque = new ObjectGroup[16];
                            }
                            while (groupIndex >= StaticOpaque.Length)
                            {
                                Array.Resize<ObjectGroup>(ref StaticOpaque, StaticOpaque.Length << 1);
                            }
                        }
                        if (StaticOpaque[groupIndex] == null)
                        {
                            StaticOpaque[groupIndex] = new ObjectGroup();
                        }
                        ObjectList list = StaticOpaque[groupIndex].List;
                        int newIndex = list.FaceCount;
                        for (int j = 0; j < list.FaceCount; j++)
                        {
                            if (list.Faces[j] == null)
                            {
                                newIndex = j;
                                break;
                            }
                        }
                        if (newIndex == list.FaceCount)
                        {
                            if (list.FaceCount == list.Faces.Length)
                            {
                                Array.Resize<ObjectFace>(ref list.Faces, list.Faces.Length << 1);
                            }
                            list.FaceCount++;
                        }
                        list.Faces[newIndex] = new ObjectFace();
                        list.Faces[newIndex].ObjectListIndex = ObjectCount;
                        list.Faces[newIndex].ObjectIndex = ObjectIndex;
                        list.Faces[newIndex].FaceIndex = i;
                        StaticOpaque[groupIndex].Update = true;
                        Objects[ObjectCount].FaceListReferences[i] = new ObjectListReference(listType, newIndex);
                        Game.InfoStaticOpaqueFaceCount++;
                    }
                    else
                    {
                        /*
						 * For all other lists, insert the face at the end of the list.
						 * */
                        ObjectList list;
                        switch (listType)
                        {
                            case ObjectListType.DynamicOpaque:
                                list = DynamicOpaque;
                                break;
                            case ObjectListType.DynamicAlpha:
                                list = DynamicAlpha;
                                break;
                            case ObjectListType.OverlayOpaque:
                                list = OverlayOpaque;
                                break;
                            case ObjectListType.OverlayAlpha:
                                list = OverlayAlpha;
                                break;
                            default:
                                throw new InvalidOperationException();
                        }
                        if (list.FaceCount == list.Faces.Length)
                        {
                            Array.Resize<ObjectFace>(ref list.Faces, list.Faces.Length << 1);
                        }
                        list.Faces[list.FaceCount] = new ObjectFace();
                        list.Faces[list.FaceCount].ObjectListIndex = ObjectCount;
                        list.Faces[list.FaceCount].ObjectIndex = ObjectIndex;
                        list.Faces[list.FaceCount].FaceIndex = i;
                        Objects[ObjectCount].FaceListReferences[i] = new ObjectListReference(listType, list.FaceCount);
                        list.FaceCount++;
                    }

                }
                ObjectManager.Objects[ObjectIndex].RendererIndex = ObjectCount + 1;
                ObjectCount++;
            }
        }

        // hide object
        internal static void HideObject(int ObjectIndex)
        {
            if (ObjectManager.Objects[ObjectIndex] == null)
            {
                return;
            }
            int k = ObjectManager.Objects[ObjectIndex].RendererIndex - 1;
            if (k >= 0)
            {
                // remove faces
                for (int i = 0; i < Objects[k].FaceListReferences.Length; i++)
                {
                    ObjectListType listType = Objects[k].FaceListReferences[i].Type;
                    if (listType == ObjectListType.StaticOpaque)
                    {
                        /*
						 * For static opaque faces, set the face to be removed
						 * to a null reference. If there are null entries at
						 * the end of the list, update the number of faces used
						 * accordingly.
						 * */
                        int groupIndex = (int)ObjectManager.Objects[Objects[k].ObjectIndex].GroupIndex;
                        ObjectList list = StaticOpaque[groupIndex].List;
                        int listIndex = Objects[k].FaceListReferences[i].Index;
                        list.Faces[listIndex] = null;
                        if (listIndex == list.FaceCount - 1)
                        {
                            int count = 0;
                            for (int j = list.FaceCount - 2; j >= 0; j--)
                            {
                                if (list.Faces[j] != null)
                                {
                                    count = j + 1;
                                    break;
                                }
                            }
                            list.FaceCount = count;
                        }
                        StaticOpaque[groupIndex].Update = true;
                        Game.InfoStaticOpaqueFaceCount--;
                    }
                    else
                    {
                        /*
						 * For all other kinds of faces, move the last face into place
						 * of the face to be removed and decrement the face counter.
						 * */
                        ObjectList list;
                        switch (listType)
                        {
                            case ObjectListType.DynamicOpaque:
                                list = DynamicOpaque;
                                break;
                            case ObjectListType.DynamicAlpha:
                                list = DynamicAlpha;
                                break;
                            case ObjectListType.OverlayOpaque:
                                list = OverlayOpaque;
                                break;
                            case ObjectListType.OverlayAlpha:
                                list = OverlayAlpha;
                                break;
                            default:
                                throw new InvalidOperationException();
                        }
                        int listIndex = Objects[k].FaceListReferences[i].Index;
                        list.Faces[listIndex] = list.Faces[list.FaceCount - 1];
                        Objects[list.Faces[listIndex].ObjectListIndex].FaceListReferences[list.Faces[listIndex].FaceIndex].Index = listIndex;
                        list.FaceCount--;
                    }
                }
                // remove object
                if (k == ObjectCount - 1)
                {
                    ObjectCount--;
                }
                else
                {
                    Objects[k] = Objects[ObjectCount - 1];
                    ObjectCount--;
                    for (int i = 0; i < Objects[k].FaceListReferences.Length; i++)
                    {
                        ObjectListType listType = Objects[k].FaceListReferences[i].Type;
                        ObjectList list;
                        switch (listType)
                        {
                            case ObjectListType.StaticOpaque:
                                {
                                    int groupIndex = (int)ObjectManager.Objects[Objects[k].ObjectIndex].GroupIndex;
                                    list = StaticOpaque[groupIndex].List;
                                }
                                break;
                            case ObjectListType.DynamicOpaque:
                                list = DynamicOpaque;
                                break;
                            case ObjectListType.DynamicAlpha:
                                list = DynamicAlpha;
                                break;
                            case ObjectListType.OverlayOpaque:
                                list = OverlayOpaque;
                                break;
                            case ObjectListType.OverlayAlpha:
                                list = OverlayAlpha;
                                break;
                            default:
                                throw new InvalidOperationException();
                        }
                        int listIndex = Objects[k].FaceListReferences[i].Index;
                        list.Faces[listIndex].ObjectListIndex = k;
                    }
                    ObjectManager.Objects[Objects[k].ObjectIndex].RendererIndex = k + 1;
                }
                ObjectManager.Objects[ObjectIndex].RendererIndex = 0;
            }
        }

        // sort polygons
        private static void SortPolygons(ObjectList List)
        {
            // calculate distance
            double cx = World.AbsoluteCameraPosition.X;
            double cy = World.AbsoluteCameraPosition.Y;
            double cz = World.AbsoluteCameraPosition.Z;
            for (int i = 0; i < List.FaceCount; i++)
            {
                int o = List.Faces[i].ObjectIndex;
                int f = List.Faces[i].FaceIndex;
                if (ObjectManager.Objects[o].Mesh.Faces[f].Vertices.Length >= 3)
                {
                    int v0 = ObjectManager.Objects[o].Mesh.Faces[f].Vertices[0].Index;
                    int v1 = ObjectManager.Objects[o].Mesh.Faces[f].Vertices[1].Index;
                    int v2 = ObjectManager.Objects[o].Mesh.Faces[f].Vertices[2].Index;
                    double v0x = ObjectManager.Objects[o].Mesh.Vertices[v0].Coordinates.X;
                    double v0y = ObjectManager.Objects[o].Mesh.Vertices[v0].Coordinates.Y;
                    double v0z = ObjectManager.Objects[o].Mesh.Vertices[v0].Coordinates.Z;
                    double v1x = ObjectManager.Objects[o].Mesh.Vertices[v1].Coordinates.X;
                    double v1y = ObjectManager.Objects[o].Mesh.Vertices[v1].Coordinates.Y;
                    double v1z = ObjectManager.Objects[o].Mesh.Vertices[v1].Coordinates.Z;
                    double v2x = ObjectManager.Objects[o].Mesh.Vertices[v2].Coordinates.X;
                    double v2y = ObjectManager.Objects[o].Mesh.Vertices[v2].Coordinates.Y;
                    double v2z = ObjectManager.Objects[o].Mesh.Vertices[v2].Coordinates.Z;
                    double w1x = v1x - v0x, w1y = v1y - v0y, w1z = v1z - v0z;
                    double w2x = v2x - v0x, w2y = v2y - v0y, w2z = v2z - v0z;
                    double dx = -w1z * w2y + w1y * w2z;
                    double dy = w1z * w2x - w1x * w2z;
                    double dz = -w1y * w2x + w1x * w2y;
                    double t = dx * dx + dy * dy + dz * dz;
                    if (t != 0.0)
                    {
                        t = 1.0 / Math.Sqrt(t);
                        dx *= t; dy *= t; dz *= t;
                        double w0x = v0x - cx, w0y = v0y - cy, w0z = v0z - cz;
                        t = dx * w0x + dy * w0y + dz * w0z;
                        List.Faces[i].Distance = -t * t;
                    }
                }
            }
            // sort
            double[] distances = new double[List.FaceCount];
            for (int i = 0; i < List.FaceCount; i++)
            {
                distances[i] = List.Faces[i].Distance;
            }
            Array.Sort<double, ObjectFace>(distances, List.Faces, 0, List.FaceCount);
            // update objects
            for (int i = 0; i < List.FaceCount; i++)
            {
                Objects[List.Faces[i].ObjectListIndex].FaceListReferences[List.Faces[i].FaceIndex].Index = i;
            }
        }

        // get distance factor
        private static double GetDistanceFactor(World.Vertex[] Vertices, ref World.MeshFace Face, ushort GlowAttenuationData, double CameraX, double CameraY, double CameraZ)
        {
            if (Face.Vertices.Length != 0)
            {
                World.GlowAttenuationMode mode; double halfdistance;
                World.SplitGlowAttenuationData(GlowAttenuationData, out mode, out halfdistance);
                int i = (int)Face.Vertices[0].Index;
                double dx = Vertices[i].Coordinates.X - CameraX;
                double dy = Vertices[i].Coordinates.Y - CameraY;
                double dz = Vertices[i].Coordinates.Z - CameraZ;
                switch (mode)
                {
                    case World.GlowAttenuationMode.DivisionExponent2:
                        {
                            double t = dx * dx + dy * dy + dz * dz;
                            return t / (t + halfdistance * halfdistance);
                        }
                    case World.GlowAttenuationMode.DivisionExponent4:
                        {
                            double t = dx * dx + dy * dy + dz * dz;
                            t *= t; halfdistance *= halfdistance;
                            return t / (t + halfdistance * halfdistance);
                        }
                    default:
                        return 1.0;
                }
            }
            else
            {
                return 1.0;
            }
        }

    }
}