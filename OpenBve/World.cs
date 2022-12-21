using OpenBveApi.Math;
using OpenBve.Worlds;
using System;

namespace OpenBve
{
    internal static class World
    {
        #region vertices
        /// <summary>Represents a vertex consisting of 3D coordinates and 2D texture coordinates.</summary>
        internal struct Vertex
        {
            internal Vectors.Vector3D Coordinates;
            internal Vector2 TextureCoordinates;
            internal Vertex(double X, double Y, double Z)
            {
                this.Coordinates = new Vectors.Vector3D(X, Y, Z);
                this.TextureCoordinates = new Vector2(0.0, 0.0);
            }
            internal Vertex(Vectors.Vector3D Coordinates, Vector2 TextureCoordinates)
            {
                this.Coordinates = Coordinates;
                this.TextureCoordinates = TextureCoordinates;
            }
            // operators
            public static bool operator ==(Vertex A, Vertex B)
            {
                if (A.Coordinates.X != B.Coordinates.X | A.Coordinates.Y != B.Coordinates.Y | A.Coordinates.Z != B.Coordinates.Z) return false;
                if (A.TextureCoordinates.X != B.TextureCoordinates.X | A.TextureCoordinates.Y != B.TextureCoordinates.Y) return false;
                return true;
            }
            public static bool operator !=(Vertex A, Vertex B)
            {
                if (A.Coordinates.X != B.Coordinates.X | A.Coordinates.Y != B.Coordinates.Y | A.Coordinates.Z != B.Coordinates.Z) return true;
                if (A.TextureCoordinates.X != B.TextureCoordinates.X | A.TextureCoordinates.Y != B.TextureCoordinates.Y) return true;
                return false;
            }
        }
        #endregion

        #region mesh material
        /// <summary>Represents material properties.</summary>
        internal struct MeshMaterial
        {
            /// <summary>A bit mask combining constants of the MeshMaterial structure.</summary>
            internal byte Flags;
            internal Colors.ColorRGBA Color;
            internal Colors.ColorRGB TransparentColor;
            internal Colors.ColorRGB EmissiveColor;
            internal int DaytimeTextureIndex;
            internal int NighttimeTextureIndex;
            /// <summary>A value between 0 (daytime) and 255 (nighttime).</summary>
            internal byte DaytimeNighttimeBlend;
            internal MeshMaterialBlendMode BlendMode;
            /// <summary>A bit mask specifying the glow properties. Use GetGlowAttenuationData to create valid data for this field.</summary>
            internal ushort GlowAttenuationData;
            internal const int EmissiveColorMask = 1;
            internal const int TransparentColorMask = 2;
            // operators
            public static bool operator ==(MeshMaterial A, MeshMaterial B)
            {
                if (A.Flags != B.Flags) return false;
                if (A.Color.R != B.Color.R | A.Color.G != B.Color.G | A.Color.B != B.Color.B | A.Color.A != B.Color.A) return false;
                if (A.TransparentColor.R != B.TransparentColor.R | A.TransparentColor.G != B.TransparentColor.G | A.TransparentColor.B != B.TransparentColor.B) return false;
                if (A.EmissiveColor.R != B.EmissiveColor.R | A.EmissiveColor.G != B.EmissiveColor.G | A.EmissiveColor.B != B.EmissiveColor.B) return false;
                if (A.DaytimeTextureIndex != B.DaytimeTextureIndex) return false;
                if (A.NighttimeTextureIndex != B.NighttimeTextureIndex) return false;
                if (A.BlendMode != B.BlendMode) return false;
                if (A.GlowAttenuationData != B.GlowAttenuationData) return false;
                return true;
            }
            public static bool operator !=(MeshMaterial A, MeshMaterial B)
            {
                if (A.Flags != B.Flags) return true;
                if (A.Color.R != B.Color.R | A.Color.G != B.Color.G | A.Color.B != B.Color.B | A.Color.A != B.Color.A) return true;
                if (A.TransparentColor.R != B.TransparentColor.R | A.TransparentColor.G != B.TransparentColor.G | A.TransparentColor.B != B.TransparentColor.B) return true;
                if (A.EmissiveColor.R != B.EmissiveColor.R | A.EmissiveColor.G != B.EmissiveColor.G | A.EmissiveColor.B != B.EmissiveColor.B) return true;
                if (A.DaytimeTextureIndex != B.DaytimeTextureIndex) return true;
                if (A.NighttimeTextureIndex != B.NighttimeTextureIndex) return true;
                if (A.BlendMode != B.BlendMode) return true;
                if (A.GlowAttenuationData != B.GlowAttenuationData) return true;
                return false;
            }
        }
        internal enum MeshMaterialBlendMode : byte
        {
            Normal = 0,
            Additive = 1
        }
        #endregion

        #region mesh face vertex
        /// <summary>Represents a reference to a vertex and the normal to be used for that vertex.</summary>
        internal struct MeshFaceVertex
        {
            /// <summary>A reference to an element in the Vertex array of the contained Mesh structure.</summary>
            internal ushort Index;
            /// <summary>The normal to be used at the vertex.</summary>
            internal Vectors.Vector3Df Normal;
            internal MeshFaceVertex(int Index)
            {
                this.Index = (ushort)Index;
                this.Normal = new Vectors.Vector3Df(0.0f, 0.0f, 0.0f);
            }
            internal MeshFaceVertex(int Index, Vectors.Vector3Df Normal)
            {
                this.Index = (ushort)Index;
                this.Normal = Normal;
            }
            // operators
            public static bool operator ==(MeshFaceVertex A, MeshFaceVertex B)
            {
                if (A.Index != B.Index) return false;
                if (A.Normal.X != B.Normal.X) return false;
                if (A.Normal.Y != B.Normal.Y) return false;
                if (A.Normal.Z != B.Normal.Z) return false;
                return true;
            }
            public static bool operator !=(MeshFaceVertex A, MeshFaceVertex B)
            {
                if (A.Index != B.Index) return true;
                if (A.Normal.X != B.Normal.X) return true;
                if (A.Normal.Y != B.Normal.Y) return true;
                if (A.Normal.Z != B.Normal.Z) return true;
                return false;
            }
        }
        #endregion

        #region mesh face
        /// <summary>Represents a face consisting of vertices and material attributes.</summary>
        internal struct MeshFace
        {
            internal MeshFaceVertex[] Vertices;
            /// <summary>A reference to an element in the Material array of the containing Mesh structure.</summary>
            internal ushort Material;
            /// <summary>A bit mask combining constants of the MeshFace structure.</summary>
            internal byte Flags;
            internal MeshFace(int[] Vertices)
            {
                this.Vertices = new MeshFaceVertex[Vertices.Length];
                for (int i = 0; i < Vertices.Length; i++)
                {
                    this.Vertices[i] = new MeshFaceVertex(Vertices[i]);
                }
                this.Material = 0;
                this.Flags = 0;
            }
            internal void Flip()
            {
                if ((this.Flags & FaceTypeMask) == FaceTypeQuadStrip)
                {
                    for (int i = 0; i < this.Vertices.Length; i += 2)
                    {
                        MeshFaceVertex x = this.Vertices[i];
                        this.Vertices[i] = this.Vertices[i + 1];
                        this.Vertices[i + 1] = x;
                    }
                }
                else
                {
                    int n = this.Vertices.Length;
                    for (int i = 0; i < (n >> 1); i++)
                    {
                        MeshFaceVertex x = this.Vertices[i];
                        this.Vertices[i] = this.Vertices[n - i - 1];
                        this.Vertices[n - i - 1] = x;
                    }
                }
            }
            internal const int FaceTypeMask = 7;
            internal const int FaceTypePolygon = 0;
            internal const int FaceTypeTriangles = 1;
            internal const int FaceTypeTriangleStrip = 2;
            internal const int FaceTypeQuads = 3;
            internal const int FaceTypeQuadStrip = 4;
            internal const int Face2Mask = 8;
        }
        #endregion

        #region mesh
        /// <summary>Represents a mesh consisting of a series of vertices, faces and material properties.</summary>
        internal struct Mesh
        {
            internal Vertex[] Vertices;
            internal MeshMaterial[] Materials;
            internal MeshFace[] Faces;
            /// <summary>Creates a mesh consisting of one face, which is represented by individual vertices, and a color.</summary>
            /// <param name="Vertices">The vertices that make up one face.</param>
            /// <param name="Color">The color to be applied on the face.</param>
            internal Mesh(Vertex[] Vertices, Colors.ColorRGBA Color)
            {
                this.Vertices = Vertices;
                this.Materials = new MeshMaterial[1];
                this.Materials[0].Color = Color;
                this.Materials[0].DaytimeTextureIndex = -1;
                this.Materials[0].NighttimeTextureIndex = -1;
                this.Faces = new MeshFace[1];
                this.Faces[0].Material = 0;
                this.Faces[0].Vertices = new MeshFaceVertex[Vertices.Length];
                for (int i = 0; i < Vertices.Length; i++)
                {
                    this.Faces[0].Vertices[i].Index = (ushort)i;
                }
            }
            /// <summary>Creates a mesh consisting of the specified vertices, faces and color.</summary>
            /// <param name="Vertices">The vertices used.</param>
            /// <param name="FaceVertices">A list of faces represented by a list of references to vertices.</param>
            /// <param name="Color">The color to be applied on all of the faces.</param>
            internal Mesh(Vertex[] Vertices, int[][] FaceVertices, Colors.ColorRGBA Color)
            {
                this.Vertices = Vertices;
                this.Materials = new MeshMaterial[1];
                this.Materials[0].Color = Color;
                this.Materials[0].DaytimeTextureIndex = -1;
                this.Materials[0].NighttimeTextureIndex = -1;
                this.Faces = new MeshFace[FaceVertices.Length];
                for (int i = 0; i < FaceVertices.Length; i++)
                {
                    this.Faces[i] = new MeshFace(FaceVertices[i]);
                }
            }
        }
        #endregion

        #region glow
        internal enum GlowAttenuationMode
        {
            None = 0,
            DivisionExponent2 = 1,
            DivisionExponent4 = 2,
        }
        /// <summary>Creates glow attenuation data from a half distance and a mode. The resulting value can be later passed to SplitGlowAttenuationData in order to reconstruct the parameters.</summary>
        /// <param name="HalfDistance">The distance at which the glow is at 50% of its full intensity. The value is clamped to the integer range from 1 to 4096. Values less than or equal to 0 disable glow attenuation.</param>
        /// <param name="Mode">The glow attenuation mode.</param>
        /// <returns>A System.UInt16 packed with the information about the half distance and glow attenuation mode.</returns>
        internal static ushort GetGlowAttenuationData(double HalfDistance, GlowAttenuationMode Mode)
        {
            if (HalfDistance <= 0.0 | Mode == GlowAttenuationMode.None) return 0;
            if (HalfDistance < 1.0)
            {
                HalfDistance = 1.0;
            }
            else if (HalfDistance > 4095.0)
            {
                HalfDistance = 4095.0;
            }
            return (ushort)((int)Math.Round(HalfDistance) | ((int)Mode << 12));
        }
        /// <summary>Recreates the half distance and the glow attenuation mode from a packed System.UInt16 that was created by GetGlowAttenuationData.</summary>
        /// <param name="Data">The data returned by GetGlowAttenuationData.</param>
        /// <param name="Mode">The mode of glow attenuation.</param>
        /// <param name="HalfDistance">The half distance of glow attenuation.</param>
        internal static void SplitGlowAttenuationData(ushort Data, out GlowAttenuationMode Mode, out double HalfDistance)
        {
            Mode = (GlowAttenuationMode)(Data >> 12);
            HalfDistance = (double)(Data & 4095);
        }
        #endregion

        #region display
        internal static double HorizontalViewingAngle;
        internal static double VerticalViewingAngle;
        internal static double OriginalVerticalViewingAngle;
        internal static double AspectRatio;
        /// <summary>The current viewing distance in the forward direction.</summary>
        internal static double ForwardViewingDistance;
        /// <summary>The current viewing distance in the backward direction.</summary>
        internal static double BackwardViewingDistance;
        /// <summary>The extra viewing distance used for determining visibility of animated objects.</summary>
        internal static double ExtraViewingDistance;
        /// <summary>The user-selected viewing distance.</summary>
        internal static double BackgroundImageDistance;
        internal struct Background
        {
            internal int Texture;
            internal int Repetition;
            internal bool KeepAspectRatio;
            internal Background(int Texture, int Repetition, bool KeepAspectRatio)
            {
                this.Texture = Texture;
                this.Repetition = Repetition;
                this.KeepAspectRatio = KeepAspectRatio;
            }
        }
        internal static Background CurrentBackground = new Background(-1, 6, false);
        internal static Background TargetBackground = new Background(-1, 6, false);
        internal const double TargetBackgroundDefaultCountdown = 0.8;
        internal static double TargetBackgroundCountdown;
        #endregion

        #region driver body
        internal struct DriverBody
        {
            internal double SlowX;
            internal double FastX;
            internal double Roll;
            internal ObjectManager.Damping RollDamping;
            internal double SlowY;
            internal double FastY;
            internal double Pitch;
            internal ObjectManager.Damping PitchDamping;
        }
        internal static DriverBody CurrentDriverBody;
        internal static void UpdateDriverBody(double TimeElapsed)
        {
            if (CameraRestriction == CameraRestrictionMode.NotAvailable)
            {
                {
                    // pitch
                    double targetY = TrainManager.PlayerTrain.Specs.CurrentAverageAcceleration;
                    const double accelerationSlow = 0.25;
                    const double accelerationFast = 2.0;
                    if (CurrentDriverBody.SlowY < targetY)
                    {
                        CurrentDriverBody.SlowY += accelerationSlow * TimeElapsed;
                        if (CurrentDriverBody.SlowY > targetY)
                        {
                            CurrentDriverBody.SlowY = targetY;
                        }
                    }
                    else if (CurrentDriverBody.SlowY > targetY)
                    {
                        CurrentDriverBody.SlowY -= accelerationSlow * TimeElapsed;
                        if (CurrentDriverBody.SlowY < targetY)
                        {
                            CurrentDriverBody.SlowY = targetY;
                        }
                    }
                    if (CurrentDriverBody.FastY < targetY)
                    {
                        CurrentDriverBody.FastY += accelerationFast * TimeElapsed;
                        if (CurrentDriverBody.FastY > targetY)
                        {
                            CurrentDriverBody.FastY = targetY;
                        }
                    }
                    else if (CurrentDriverBody.FastY > targetY)
                    {
                        CurrentDriverBody.FastY -= accelerationFast * TimeElapsed;
                        if (CurrentDriverBody.FastY < targetY)
                        {
                            CurrentDriverBody.FastY = targetY;
                        }
                    }
                    double diffY = CurrentDriverBody.FastY - CurrentDriverBody.SlowY;
                    diffY = (double)Math.Sign(diffY) * diffY * diffY;
                    CurrentDriverBody.Pitch = 0.5 * Math.Atan(0.1 * diffY);
                    if (CurrentDriverBody.Pitch > 0.1)
                    {
                        CurrentDriverBody.Pitch = 0.1;
                    }
                    if (CurrentDriverBody.PitchDamping == null)
                    {
                        CurrentDriverBody.PitchDamping = new ObjectManager.Damping(6.0, 0.3);
                    }
                    ObjectManager.UpdateDamping(ref CurrentDriverBody.PitchDamping, TimeElapsed, ref CurrentDriverBody.Pitch);
                }
                {
                    // roll
                    int c = TrainManager.PlayerTrain.DriverCar;
                    double frontRadius = TrainManager.PlayerTrain.Cars[c].FrontAxle.Follower.CurveRadius;
                    double rearRadius = TrainManager.PlayerTrain.Cars[c].RearAxle.Follower.CurveRadius;
                    double radius;
                    if (frontRadius != 0.0 & rearRadius != 0.0)
                    {
                        if (frontRadius != -rearRadius)
                        {
                            radius = 2.0 * frontRadius * rearRadius / (frontRadius + rearRadius);
                        }
                        else
                        {
                            radius = 0.0;
                        }
                    }
                    else if (frontRadius != 0.0)
                    {
                        radius = 2.0 * frontRadius;
                    }
                    else if (rearRadius != 0.0)
                    {
                        radius = 2.0 * rearRadius;
                    }
                    else
                    {
                        radius = 0.0;
                    }
                    double targetX;
                    if (radius != 0.0)
                    {
                        double speed = TrainManager.PlayerTrain.Cars[c].Specs.CurrentSpeed;
                        targetX = speed * speed / radius;
                    }
                    else
                    {
                        targetX = 0.0;
                    }
                    const double accelerationSlow = 1.0;
                    const double accelerationFast = 10.0;
                    if (CurrentDriverBody.SlowX < targetX)
                    {
                        CurrentDriverBody.SlowX += accelerationSlow * TimeElapsed;
                        if (CurrentDriverBody.SlowX > targetX)
                        {
                            CurrentDriverBody.SlowX = targetX;
                        }
                    }
                    else if (CurrentDriverBody.SlowX > targetX)
                    {
                        CurrentDriverBody.SlowX -= accelerationSlow * TimeElapsed;
                        if (CurrentDriverBody.SlowX < targetX)
                        {
                            CurrentDriverBody.SlowX = targetX;
                        }
                    }
                    if (CurrentDriverBody.FastX < targetX)
                    {
                        CurrentDriverBody.FastX += accelerationFast * TimeElapsed;
                        if (CurrentDriverBody.FastX > targetX)
                        {
                            CurrentDriverBody.FastX = targetX;
                        }
                    }
                    else if (CurrentDriverBody.FastX > targetX)
                    {
                        CurrentDriverBody.FastX -= accelerationFast * TimeElapsed;
                        if (CurrentDriverBody.FastX < targetX)
                        {
                            CurrentDriverBody.FastX = targetX;
                        }
                    }
                    double diffX = CurrentDriverBody.SlowX - CurrentDriverBody.FastX;
                    diffX = (double)Math.Sign(diffX) * diffX * diffX;
                    CurrentDriverBody.Roll = 0.5 * Math.Atan(0.3 * diffX);
                    if (CurrentDriverBody.RollDamping == null)
                    {
                        CurrentDriverBody.RollDamping = new ObjectManager.Damping(6.0, 0.3);
                    }
                    ObjectManager.UpdateDamping(ref CurrentDriverBody.RollDamping, TimeElapsed, ref CurrentDriverBody.Roll);
                }
            }
        }
        #endregion

        #region mouse grab
        internal static bool MouseGrabEnabled = false;
        internal static bool MouseGrabIgnoreOnce = false;
        internal static Vector2 MouseGrabTarget = new Vector2(0.0, 0.0);
        internal static void UpdateMouseGrab(double TimeElapsed)
        {
            if (MouseGrabEnabled)
            {
                double factor;
                if (CameraMode == CameraViewMode.Interior | CameraMode == CameraViewMode.InteriorLookAhead)
                {
                    factor = 1.0;
                }
                else
                {
                    factor = 3.0;
                }
                CameraAlignmentDirection.Yaw += factor * MouseGrabTarget.X;
                CameraAlignmentDirection.Pitch -= factor * MouseGrabTarget.Y;
                MouseGrabTarget = new Vector2(0.0, 0.0);
            }
        }
        #endregion

        #region relative camera
        internal struct CameraAlignment
        {
            internal Vectors.Vector3D Position;
            internal double Yaw;
            internal double Pitch;
            internal double Roll;
            internal double TrackPosition;
            internal double Zoom;
            internal CameraAlignment(Vectors.Vector3D Position, double Yaw, double Pitch, double Roll, double TrackPosition, double Zoom)
            {
                this.Position = Position;
                this.Yaw = Yaw;
                this.Pitch = Pitch;
                this.Roll = Roll;
                this.TrackPosition = TrackPosition;
                this.Zoom = Zoom;
            }
        }
        internal static TrackManager.TrackFollower CameraTrackFollower;
        internal static CameraAlignment CameraCurrentAlignment;
        internal static CameraAlignment CameraAlignmentDirection;
        internal static CameraAlignment CameraAlignmentSpeed;
        internal static double CameraSpeed;
        internal const double CameraInteriorTopSpeed = 5.0;
        internal const double CameraInteriorTopAngularSpeed = 5.0;
        internal const double CameraExteriorTopSpeed = 50.0;
        internal const double CameraExteriorTopAngularSpeed = 10.0;
        internal const double CameraZoomTopSpeed = 2.0;
        internal enum CameraViewMode { Interior, InteriorLookAhead, Exterior, Track, FlyBy, FlyByZooming }
        internal static CameraViewMode CameraMode;
        #endregion

        #region camera memory
        internal static CameraAlignment CameraSavedInterior;
        internal static CameraAlignment CameraSavedExterior;
        internal static CameraAlignment CameraSavedTrack;
        #endregion

        #region camera restriction
        internal static Vectors.Vector3D CameraRestrictionBottomLeft = new Vectors.Vector3D(-1.0, -1.0, 1.0);
        internal static Vectors.Vector3D CameraRestrictionTopRight = new Vectors.Vector3D(1.0, 1.0, 1.0);
        internal enum CameraRestrictionMode
        {
            /// <summary>Represents a 3D cab.</summary>
            NotAvailable = -1,
            /// <summary>Represents a 2D cab with camera restriction disabled.</summary>
            Off = 0,
            /// <summary>Represents a 2D cab with camera restriction enabled.</summary>
            On = 1
        }
        internal static CameraRestrictionMode CameraRestriction = CameraRestrictionMode.NotAvailable;
        #endregion

        #region absolute camera
        internal static Vectors.Vector3D AbsoluteCameraPosition;
        internal static Vectors.Vector3D AbsoluteCameraDirection;
        internal static Vectors.Vector3D AbsoluteCameraUp;
        internal static Vectors.Vector3D AbsoluteCameraSide;
        #endregion

        #region camera restriction
        internal static void InitializeCameraRestriction()
        {
            if ((CameraMode == CameraViewMode.Interior | CameraMode == CameraViewMode.InteriorLookAhead) & CameraRestriction == CameraRestrictionMode.On)
            {
                CameraAlignmentSpeed = new CameraAlignment();
                UpdateAbsoluteCamera(0.0);
                if (!PerformCameraRestrictionTest())
                {
                    CameraCurrentAlignment = new CameraAlignment();
                    VerticalViewingAngle = OriginalVerticalViewingAngle;
                    MainLoop.UpdateViewport(MainLoop.ViewPortChangeMode.NoChange);
                    UpdateAbsoluteCamera(0.0);
                    UpdateViewingDistances();
                    if (!PerformCameraRestrictionTest())
                    {
                        CameraCurrentAlignment.Position.Z = 0.8;
                        UpdateAbsoluteCamera(0.0);
                        PerformProgressiveAdjustmentForCameraRestriction(ref CameraCurrentAlignment.Position.Z, 0.0, true);
                        if (!PerformCameraRestrictionTest())
                        {
                            CameraCurrentAlignment.Position.X = 0.5 * (CameraRestrictionBottomLeft.X + CameraRestrictionTopRight.X);
                            CameraCurrentAlignment.Position.Y = 0.5 * (CameraRestrictionBottomLeft.Y + CameraRestrictionTopRight.Y);
                            CameraCurrentAlignment.Position.Z = 0.0;
                            UpdateAbsoluteCamera(0.0);
                            if (PerformCameraRestrictionTest())
                            {
                                PerformProgressiveAdjustmentForCameraRestriction(ref CameraCurrentAlignment.Position.X, 0.0, true);
                                PerformProgressiveAdjustmentForCameraRestriction(ref CameraCurrentAlignment.Position.Y, 0.0, true);
                            }
                            else
                            {
                                CameraCurrentAlignment.Position.Z = 0.8;
                                UpdateAbsoluteCamera(0.0);
                                PerformProgressiveAdjustmentForCameraRestriction(ref CameraCurrentAlignment.Position.Z, 0.0, true);
                                if (!PerformCameraRestrictionTest())
                                {
                                    CameraCurrentAlignment = new CameraAlignment();
                                }
                            }
                        }
                    }
                    UpdateAbsoluteCamera(0.0);
                }
            }
        }
        internal static bool PerformProgressiveAdjustmentForCameraRestriction(ref double Source, double Target, bool Zoom)
        {
            if ((CameraMode != CameraViewMode.Interior & CameraMode != CameraViewMode.InteriorLookAhead) | CameraRestriction != CameraRestrictionMode.On)
            {
                Source = Target;
                return true;
            }
            else
            {
                double best = Source;
                const int Precision = 8;
                double a = Source;
                double b = Target;
                Source = Target;
                if (Zoom) ApplyZoom();
                if (PerformCameraRestrictionTest())
                {
                    return true;
                }
                else
                {
                    double x = 0.5 * (a + b);
                    bool q = true;
                    for (int i = 0; i < Precision; i++)
                    {
                        Source = x;
                        if (Zoom) ApplyZoom();
                        q = PerformCameraRestrictionTest();
                        if (q)
                        {
                            a = x;
                            best = x;
                        }
                        else
                        {
                            b = x;
                        }
                        x = 0.5 * (a + b);
                    }
                    Source = best;
                    if (Zoom) ApplyZoom();
                    return q;
                }
            }
        }
        internal static bool PerformCameraRestrictionTest()
        {
            if (World.CameraRestriction == CameraRestrictionMode.On)
            {
                Vectors.Vector3D[] p = new Vectors.Vector3D[] { CameraRestrictionBottomLeft, CameraRestrictionTopRight };
                Vector2[] r = new Vector2[2];
                for (int j = 0; j < 2; j++)
                {
                    // determine relative world coordinates
                    //Vectors.Rotate(ref p[j].X, ref p[j].Y, ref p[j].Z, World.AbsoluteCameraDirection.X, World.AbsoluteCameraDirection.Y, World.AbsoluteCameraDirection.Z, World.AbsoluteCameraUp.X, World.AbsoluteCameraUp.Y, World.AbsoluteCameraUp.Z, World.AbsoluteCameraSide.X, World.AbsoluteCameraSide.Y, World.AbsoluteCameraSide.Z);
                    p[j].Rotate(World.AbsoluteCameraDirection, World.AbsoluteCameraUp, World.AbsoluteCameraSide);
                    double rx = -Math.Tan(World.CameraCurrentAlignment.Yaw) - World.CameraCurrentAlignment.Position.X;
                    double ry = -Math.Tan(World.CameraCurrentAlignment.Pitch) - World.CameraCurrentAlignment.Position.Y;
                    double rz = -World.CameraCurrentAlignment.Position.Z;
                    p[j].X += rx * World.AbsoluteCameraSide.X + ry * World.AbsoluteCameraUp.X + rz * World.AbsoluteCameraDirection.X;
                    p[j].Y += rx * World.AbsoluteCameraSide.Y + ry * World.AbsoluteCameraUp.Y + rz * World.AbsoluteCameraDirection.Y;
                    p[j].Z += rx * World.AbsoluteCameraSide.Z + ry * World.AbsoluteCameraUp.Z + rz * World.AbsoluteCameraDirection.Z;
                    // determine screen coordinates
                    double ez = AbsoluteCameraDirection.X * p[j].X + AbsoluteCameraDirection.Y * p[j].Y + AbsoluteCameraDirection.Z * p[j].Z;
                    if (ez == 0.0) return false;
                    double ex = AbsoluteCameraSide.X * p[j].X + AbsoluteCameraSide.Y * p[j].Y + AbsoluteCameraSide.Z * p[j].Z;
                    double ey = AbsoluteCameraUp.X * p[j].X + AbsoluteCameraUp.Y * p[j].Y + AbsoluteCameraUp.Z * p[j].Z;
                    r[j].X = ex / (ez * Math.Tan(0.5 * HorizontalViewingAngle));
                    r[j].Y = ey / (ez * Math.Tan(0.5 * VerticalViewingAngle));
                }
                return r[0].X <= -1.0025 & r[1].X >= 1.0025 & r[0].Y <= -1.0025 & r[1].Y >= 1.0025;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region update absolute camera
        internal static void UpdateAbsoluteCamera(double TimeElapsed)
        {
            // zoom
            double zm = CameraCurrentAlignment.Zoom;
            AdjustAlignment(ref CameraCurrentAlignment.Zoom, CameraAlignmentDirection.Zoom, ref CameraAlignmentSpeed.Zoom, TimeElapsed, true);
            if (zm != CameraCurrentAlignment.Zoom)
            {
                ApplyZoom();
            }
            if (CameraMode == CameraViewMode.FlyBy | CameraMode == CameraViewMode.FlyByZooming)
            {
                // fly-by
                AdjustAlignment(ref CameraCurrentAlignment.Position.X, CameraAlignmentDirection.Position.X, ref CameraAlignmentSpeed.Position.X, TimeElapsed);
                AdjustAlignment(ref CameraCurrentAlignment.Position.Y, CameraAlignmentDirection.Position.Y, ref CameraAlignmentSpeed.Position.Y, TimeElapsed);
                double tr = CameraCurrentAlignment.TrackPosition;
                AdjustAlignment(ref CameraCurrentAlignment.TrackPosition, CameraAlignmentDirection.TrackPosition, ref CameraAlignmentSpeed.TrackPosition, TimeElapsed);
                if (tr != CameraCurrentAlignment.TrackPosition)
                {
                    TrackManager.UpdateTrackFollower(ref CameraTrackFollower, CameraCurrentAlignment.TrackPosition, true, false);
                    UpdateViewingDistances();
                }
                // camera
                // position to focus on
                double tx, ty, tz;
                double zoomMultiplier;
                {
                    const double heightFactor = 0.75;
                    TrainManager.Train bestTrain = null;
                    double bestDistanceSquared = double.MaxValue;
                    TrainManager.Train secondBestTrain = null;
                    double secondBestDistanceSquared = double.MaxValue;
                    foreach (TrainManager.Train train in TrainManager.Trains)
                    {
                        if (train.State == TrainManager.TrainState.Available)
                        {
                            Vectors.Vector3D FollowerPosition = (0.5 * (train.Cars[0].FrontAxle.Follower.WorldPosition + train.Cars[0].RearAxle.Follower.WorldPosition)) - CameraTrackFollower.WorldPosition;
                            double d = (FollowerPosition.X * FollowerPosition.X) + (FollowerPosition.Y * FollowerPosition.Y) + (FollowerPosition.Z * FollowerPosition.Z);
                            if (d < bestDistanceSquared)
                            {
                                secondBestTrain = bestTrain;
                                secondBestDistanceSquared = bestDistanceSquared;
                                bestTrain = train;
                                bestDistanceSquared = d;
                            }
                            else if (d < secondBestDistanceSquared)
                            {
                                secondBestTrain = train;
                                secondBestDistanceSquared = d;
                            }
                        }
                    }
                    if (bestTrain != null)
                    {
                        const double maxDistance = 100.0;
                        double bestDistance = Math.Sqrt(bestDistanceSquared);
                        double secondBestDistance = Math.Sqrt(secondBestDistanceSquared);
                        if (secondBestTrain != null && secondBestDistance - bestDistance <= maxDistance)
                        {
                            Vectors.Vector3D BestTrainPosition = 0.5 * (bestTrain.Cars[0].FrontAxle.Follower.WorldPosition + bestTrain.Cars[0].RearAxle.Follower.WorldPosition);
                            Vectors.Vector3D SecondBestTrainPosition = 0.5 * (secondBestTrain.Cars[0].FrontAxle.Follower.WorldPosition + secondBestTrain.Cars[0].RearAxle.Follower.WorldPosition);

                            double t = 0.5 - (secondBestDistance - bestDistance) / (2.0 * maxDistance);
                            if (t < 0.0) t = 0.0;
                            t = 2.0 * t * t; /* in order to change the shape of the interpolation curve */
                            tx = (1.0 - t) * BestTrainPosition.X + t * SecondBestTrainPosition.X;
                            ty = (1.0 - t) * BestTrainPosition.Y + t * SecondBestTrainPosition.Y;
                            tz = (1.0 - t) * BestTrainPosition.Z + t * SecondBestTrainPosition.Z;
                            zoomMultiplier = 1.0 - 2.0 * t;
                        }
                        else
                        {
                            tx = 0.5 * (bestTrain.Cars[0].FrontAxle.Follower.WorldPosition.X + bestTrain.Cars[0].RearAxle.Follower.WorldPosition.X);
                            ty = 0.5 * (bestTrain.Cars[0].FrontAxle.Follower.WorldPosition.Y + bestTrain.Cars[0].RearAxle.Follower.WorldPosition.Y) + heightFactor * bestTrain.Cars[0].Height;
                            tz = 0.5 * (bestTrain.Cars[0].FrontAxle.Follower.WorldPosition.Z + bestTrain.Cars[0].RearAxle.Follower.WorldPosition.Z);
                            zoomMultiplier = 1.0;
                        }
                    }
                    else
                    {
                        tx = 0.0;
                        ty = 0.0;
                        tz = 0.0;
                        zoomMultiplier = 1.0;
                    }
                }
                // camera
                {
                    double dx = CameraTrackFollower.WorldDirection.X;
                    double dy = CameraTrackFollower.WorldDirection.Y;
                    double dz = CameraTrackFollower.WorldDirection.Z;
                    double ux = CameraTrackFollower.WorldUp.X;
                    double uy = CameraTrackFollower.WorldUp.Y;
                    double uz = CameraTrackFollower.WorldUp.Z;
                    double sx = CameraTrackFollower.WorldSide.X;
                    double sy = CameraTrackFollower.WorldSide.Y;
                    double sz = CameraTrackFollower.WorldSide.Z;
                    double ox = CameraCurrentAlignment.Position.X;
                    double oy = CameraCurrentAlignment.Position.Y;
                    double oz = CameraCurrentAlignment.Position.Z;
                    double cx = CameraTrackFollower.WorldPosition.X + (sx * ox) + (ux * oy) + (dx * oz);
                    double cy = CameraTrackFollower.WorldPosition.Y + (sy * ox) + (uy * oy) + (dy * oz);
                    double cz = CameraTrackFollower.WorldPosition.Z + (sz * ox) + (uz * oy) + (dz * oz);
                    AbsoluteCameraPosition = new Vectors.Vector3D(cx, cy, cz);
                    dx = tx - cx;
                    dy = ty - cy;
                    dz = tz - cz;
                    double t = 1.0 / Math.Sqrt((dx * dx) + (dy * dy) + (dz * dz));
                    dx *= t;
                    dy *= t;
                    dz *= t;
                    AbsoluteCameraDirection = new Vectors.Vector3D(dx, dy, dz);
                    AbsoluteCameraSide = new Vectors.Vector3D(dz, 0.0, -dx);
                    AbsoluteCameraSide.Normalize();
                    AbsoluteCameraUp = Vectors.Vector3D.Cross(new Vectors.Vector3D(dx, dy, dz), AbsoluteCameraSide);
                    UpdateViewingDistances();
                    if (CameraMode == CameraViewMode.FlyByZooming)
                    {
                        // zoom
                        const double fadeOutDistance = 600.0; /* the distance with the highest zoom factor is half the fade-out distance */
                        const double maxZoomFactor = 7.0; /* the zoom factor at half the fade-out distance */
                        const double factor = 256.0 / (fadeOutDistance * fadeOutDistance * fadeOutDistance * fadeOutDistance * fadeOutDistance * fadeOutDistance * fadeOutDistance * fadeOutDistance);
                        double zoom;
                        if (t < fadeOutDistance)
                        {
                            double tdist4 = fadeOutDistance - t; tdist4 *= tdist4; tdist4 *= tdist4;
                            double t4 = t * t; t4 *= t4;
                            zoom = 1.0 + factor * zoomMultiplier * (maxZoomFactor - 1.0) * tdist4 * t4;
                        }
                        else
                        {
                            zoom = 1.0;
                        }
                        World.VerticalViewingAngle = World.OriginalVerticalViewingAngle / zoom;
                        MainLoop.UpdateViewport(MainLoop.ViewPortChangeMode.NoChange);
                    }
                }
            }
            else
            {
                // non-fly-by
                {
                    // current alignment
                    AdjustAlignment(ref CameraCurrentAlignment.Position.X, CameraAlignmentDirection.Position.X, ref CameraAlignmentSpeed.Position.X, TimeElapsed);
                    AdjustAlignment(ref CameraCurrentAlignment.Position.Y, CameraAlignmentDirection.Position.Y, ref CameraAlignmentSpeed.Position.Y, TimeElapsed);
                    AdjustAlignment(ref CameraCurrentAlignment.Position.Z, CameraAlignmentDirection.Position.Z, ref CameraAlignmentSpeed.Position.Z, TimeElapsed);
                    if ((CameraMode == CameraViewMode.Interior | CameraMode == CameraViewMode.InteriorLookAhead) & CameraRestriction == CameraRestrictionMode.On)
                    {
                        if (CameraCurrentAlignment.Position.Z > 0.75)
                        {
                            CameraCurrentAlignment.Position.Z = 0.75;
                        }
                    }
                    bool q = CameraAlignmentSpeed.Yaw != 0.0 | CameraAlignmentSpeed.Pitch != 0.0 | CameraAlignmentSpeed.Roll != 0.0;
                    AdjustAlignment(ref CameraCurrentAlignment.Yaw, CameraAlignmentDirection.Yaw, ref CameraAlignmentSpeed.Yaw, TimeElapsed);
                    AdjustAlignment(ref CameraCurrentAlignment.Pitch, CameraAlignmentDirection.Pitch, ref CameraAlignmentSpeed.Pitch, TimeElapsed);
                    AdjustAlignment(ref CameraCurrentAlignment.Roll, CameraAlignmentDirection.Roll, ref CameraAlignmentSpeed.Roll, TimeElapsed);
                    double tr = CameraCurrentAlignment.TrackPosition;
                    AdjustAlignment(ref CameraCurrentAlignment.TrackPosition, CameraAlignmentDirection.TrackPosition, ref CameraAlignmentSpeed.TrackPosition, TimeElapsed);
                    if (tr != CameraCurrentAlignment.TrackPosition)
                    {
                        TrackManager.UpdateTrackFollower(ref CameraTrackFollower, CameraCurrentAlignment.TrackPosition, true, false);
                        q = true;
                    }
                    if (q)
                    {
                        UpdateViewingDistances();
                    }
                }
                // camera
                double cx = CameraTrackFollower.WorldPosition.X;
                double cy = CameraTrackFollower.WorldPosition.Y;
                double cz = CameraTrackFollower.WorldPosition.Z;
                double dx = CameraTrackFollower.WorldDirection.X;
                double dy = CameraTrackFollower.WorldDirection.Y;
                double dz = CameraTrackFollower.WorldDirection.Z;
                double ux = CameraTrackFollower.WorldUp.X;
                double uy = CameraTrackFollower.WorldUp.Y;
                double uz = CameraTrackFollower.WorldUp.Z;
                double sx = CameraTrackFollower.WorldSide.X;
                double sy = CameraTrackFollower.WorldSide.Y;
                double sz = CameraTrackFollower.WorldSide.Z;
                double lookaheadYaw;
                double lookaheadPitch;
                if (CameraMode == CameraViewMode.InteriorLookAhead)
                {
                    // look-ahead
                    double d = 20.0;
                    if (TrainManager.PlayerTrain.Specs.CurrentAverageSpeed > 0.0)
                    {
                        d += 3.0 * (Math.Sqrt(TrainManager.PlayerTrain.Specs.CurrentAverageSpeed * TrainManager.PlayerTrain.Specs.CurrentAverageSpeed + 1.0) - 1.0);
                    }
                    d -= TrainManager.PlayerTrain.Cars[TrainManager.PlayerTrain.DriverCar].FrontAxlePosition;
                    TrackManager.TrackFollower f = TrainManager.PlayerTrain.Cars[TrainManager.PlayerTrain.DriverCar].FrontAxle.Follower;
                    f.TriggerType = TrackManager.EventTriggerType.None;
                    TrackManager.UpdateTrackFollower(ref f, f.TrackPosition + d, true, false);
                    double rx = f.WorldPosition.X - cx + CameraTrackFollower.WorldSide.X * TrainManager.PlayerTrain.Cars[TrainManager.PlayerTrain.DriverCar].DriverX + CameraTrackFollower.WorldUp.X * TrainManager.PlayerTrain.Cars[TrainManager.PlayerTrain.DriverCar].DriverY + CameraTrackFollower.WorldDirection.X * TrainManager.PlayerTrain.Cars[TrainManager.PlayerTrain.DriverCar].DriverZ;
                    double ry = f.WorldPosition.Y - cy + CameraTrackFollower.WorldSide.Y * TrainManager.PlayerTrain.Cars[TrainManager.PlayerTrain.DriverCar].DriverX + CameraTrackFollower.WorldUp.Y * TrainManager.PlayerTrain.Cars[TrainManager.PlayerTrain.DriverCar].DriverY + CameraTrackFollower.WorldDirection.Y * TrainManager.PlayerTrain.Cars[TrainManager.PlayerTrain.DriverCar].DriverZ;
                    double rz = f.WorldPosition.Z - cz + CameraTrackFollower.WorldSide.Z * TrainManager.PlayerTrain.Cars[TrainManager.PlayerTrain.DriverCar].DriverX + CameraTrackFollower.WorldUp.Z * TrainManager.PlayerTrain.Cars[TrainManager.PlayerTrain.DriverCar].DriverY + CameraTrackFollower.WorldDirection.Z * TrainManager.PlayerTrain.Cars[TrainManager.PlayerTrain.DriverCar].DriverZ;
                    Vectors.Normalize(ref rx, ref ry, ref rz);
                    double t = dz * (sy * ux - sx * uy) + dy * (-sz * ux + sx * uz) + dx * (sz * uy - sy * uz);
                    if (t != 0.0)
                    {
                        t = 1.0 / t;
                        double tx = (rz * (-dy * ux + dx * uy) + ry * (dz * ux - dx * uz) + rx * (-dz * uy + dy * uz)) * t;
                        double ty = (rz * (dy * sx - dx * sy) + ry * (-dz * sx + dx * sz) + rx * (dz * sy - dy * sz)) * t;
                        double tz = (rz * (sy * ux - sx * uy) + ry * (-sz * ux + sx * uz) + rx * (sz * uy - sy * uz)) * t;
                        lookaheadYaw = tx * tz != 0.0 ? Math.Atan2(tx, tz) : 0.0;
                        if (ty < -1.0)
                        {
                            lookaheadPitch = -0.5 * Math.PI;
                        }
                        else if (ty > 1.0)
                        {
                            lookaheadPitch = 0.5 * Math.PI;
                        }
                        else
                        {
                            lookaheadPitch = Math.Asin(ty);
                        }
                    }
                    else
                    {
                        lookaheadYaw = 0.0;
                        lookaheadPitch = 0.0;
                    }
                }
                else
                {
                    lookaheadYaw = 0.0;
                    lookaheadPitch = 0.0;
                }
                {
                    // cab pitch and yaw
                    Vectors.Vector3D APos = CameraCurrentAlignment.Position;
                    Vectors.Vector3D WDir = CameraTrackFollower.WorldDirection;
                    Vectors.Vector3D WUp = CameraTrackFollower.WorldUp;
                    if ((CameraMode == CameraViewMode.Interior | CameraMode == CameraViewMode.InteriorLookAhead) & TrainManager.PlayerTrain != null)
                    {
                        int c = TrainManager.PlayerTrain.DriverCar;
                        if (c >= 0)
                        {
                            if (TrainManager.PlayerTrain.Cars[c].CarSections.Length == 0 || !TrainManager.PlayerTrain.Cars[c].CarSections[0].Overlay)
                            {
                                double a = TrainManager.PlayerTrain.Cars[TrainManager.PlayerTrain.DriverCar].DriverPitch;
                                double cosa = Math.Cos(-a);
                                double sina = Math.Sin(-a);
                                WDir.Rotate(CameraTrackFollower.WorldSide, cosa, sina);
                                WUp.Rotate(CameraTrackFollower.WorldSide, cosa, sina);
                            }
                        }
                    }
                    cx += sx * APos.X + WUp.X * APos.Y + WDir.X * APos.Z;
                    cy += sy * APos.X + WUp.Y * APos.Y + WDir.Y * APos.Z;
                    cz += sz * APos.X + WUp.Z * APos.Y + WDir.Z * APos.Z;
                }
                // yaw, pitch, roll
                double headYaw = CameraCurrentAlignment.Yaw + lookaheadYaw;
                if ((CameraMode == CameraViewMode.Interior | CameraMode == CameraViewMode.InteriorLookAhead) & TrainManager.PlayerTrain != null)
                {
                    if (TrainManager.PlayerTrain.DriverCar >= 0)
                    {
                        headYaw += TrainManager.PlayerTrain.Cars[TrainManager.PlayerTrain.DriverCar].DriverYaw;
                    }
                }
                double headPitch = CameraCurrentAlignment.Pitch + lookaheadPitch;
                if ((CameraMode == CameraViewMode.Interior | CameraMode == CameraViewMode.InteriorLookAhead) & TrainManager.PlayerTrain != null)
                {
                    if (TrainManager.PlayerTrain.DriverCar >= 0)
                    {
                        headPitch += TrainManager.PlayerTrain.Cars[TrainManager.PlayerTrain.DriverCar].DriverPitch;
                    }
                }
                double bodyPitch = 0.0;
                double bodyRoll = 0.0;
                double headRoll = CameraCurrentAlignment.Roll;
                // rotation
                if (CameraRestriction == CameraRestrictionMode.NotAvailable & (CameraMode == CameraViewMode.Interior | CameraMode == CameraViewMode.InteriorLookAhead))
                {
                    // with body and head
                    bodyPitch += CurrentDriverBody.Pitch;
                    headPitch -= 0.2 * CurrentDriverBody.Pitch;
                    bodyRoll += CurrentDriverBody.Roll;
                    headRoll += 0.2 * CurrentDriverBody.Roll;
                    const double bodyHeight = 0.6;
                    const double headHeight = 0.1;
                    {
                        // body pitch
                        double ry = (Math.Cos(-bodyPitch) - 1.0) * bodyHeight;
                        double rz = Math.Sin(-bodyPitch) * bodyHeight;
                        cx += dx * rz + ux * ry;
                        cy += dy * rz + uy * ry;
                        cz += dz * rz + uz * ry;
                        if (bodyPitch != 0.0)
                        {
                            double cosa = Math.Cos(-bodyPitch);
                            double sina = Math.Sin(-bodyPitch);
                            Vectors.Rotate(ref dx, ref dy, ref dz, sx, sy, sz, cosa, sina);
                            Vectors.Rotate(ref ux, ref uy, ref uz, sx, sy, sz, cosa, sina);
                        }
                    }
                    {
                        // body roll
                        double rx = Math.Sin(bodyRoll) * bodyHeight;
                        double ry = (Math.Cos(bodyRoll) - 1.0) * bodyHeight;
                        cx += sx * rx + ux * ry;
                        cy += sy * rx + uy * ry;
                        cz += sz * rx + uz * ry;
                        if (bodyRoll != 0.0)
                        {
                            double cosa = Math.Cos(-bodyRoll);
                            double sina = Math.Sin(-bodyRoll);
                            Vectors.Rotate(ref ux, ref uy, ref uz, dx, dy, dz, cosa, sina);
                            Vectors.Rotate(ref sx, ref sy, ref sz, dx, dy, dz, cosa, sina);
                        }
                    }
                    {
                        // head yaw
                        double rx = Math.Sin(headYaw) * headHeight;
                        double rz = (Math.Cos(headYaw) - 1.0) * headHeight;
                        cx += sx * rx + dx * rz;
                        cy += sy * rx + dy * rz;
                        cz += sz * rx + dz * rz;
                        if (headYaw != 0.0)
                        {
                            double cosa = Math.Cos(headYaw);
                            double sina = Math.Sin(headYaw);
                            Vectors.Rotate(ref dx, ref dy, ref dz, ux, uy, uz, cosa, sina);
                            Vectors.Rotate(ref sx, ref sy, ref sz, ux, uy, uz, cosa, sina);
                        }
                    }
                    {
                        // head pitch
                        double ry = (Math.Cos(-headPitch) - 1.0) * headHeight;
                        double rz = Math.Sin(-headPitch) * headHeight;
                        cx += dx * rz + ux * ry;
                        cy += dy * rz + uy * ry;
                        cz += dz * rz + uz * ry;
                        if (headPitch != 0.0)
                        {
                            double cosa = Math.Cos(-headPitch);
                            double sina = Math.Sin(-headPitch);
                            Vectors.Rotate(ref dx, ref dy, ref dz, sx, sy, sz, cosa, sina);
                            Vectors.Rotate(ref ux, ref uy, ref uz, sx, sy, sz, cosa, sina);
                        }
                    }
                    {
                        // head roll
                        double rx = Math.Sin(headRoll) * headHeight;
                        double ry = (Math.Cos(headRoll) - 1.0) * headHeight;
                        cx += sx * rx + ux * ry;
                        cy += sy * rx + uy * ry;
                        cz += sz * rx + uz * ry;
                        if (headRoll != 0.0)
                        {
                            double cosa = Math.Cos(-headRoll);
                            double sina = Math.Sin(-headRoll);
                            Vectors.Rotate(ref ux, ref uy, ref uz, dx, dy, dz, cosa, sina);
                            Vectors.Rotate(ref sx, ref sy, ref sz, dx, dy, dz, cosa, sina);
                        }
                    }
                }
                else
                {
                    // without body or head
                    double totalYaw = headYaw;
                    double totalPitch = headPitch + bodyPitch;
                    double totalRoll = bodyRoll + headRoll;
                    if (totalYaw != 0.0)
                    {
                        double cosa = Math.Cos(totalYaw);
                        double sina = Math.Sin(totalYaw);
                        Vectors.Rotate(ref dx, ref dy, ref dz, ux, uy, uz, cosa, sina);
                        Vectors.Rotate(ref sx, ref sy, ref sz, ux, uy, uz, cosa, sina);
                    }
                    if (totalPitch != 0.0)
                    {
                        double cosa = Math.Cos(-totalPitch);
                        double sina = Math.Sin(-totalPitch);
                        Vectors.Rotate(ref dx, ref dy, ref dz, sx, sy, sz, cosa, sina);
                        Vectors.Rotate(ref ux, ref uy, ref uz, sx, sy, sz, cosa, sina);
                    }
                    if (totalRoll != 0.0)
                    {
                        double cosa = Math.Cos(-totalRoll);
                        double sina = Math.Sin(-totalRoll);
                        Vectors.Rotate(ref ux, ref uy, ref uz, dx, dy, dz, cosa, sina);
                        Vectors.Rotate(ref sx, ref sy, ref sz, dx, dy, dz, cosa, sina);
                    }
                }
                // finish
                AbsoluteCameraPosition = new Vectors.Vector3D(cx, cy, cz);
                AbsoluteCameraDirection = new Vectors.Vector3D(dx, dy, dz);
                AbsoluteCameraUp = new Vectors.Vector3D(ux, uy, uz);
                AbsoluteCameraSide = new Vectors.Vector3D(sx, sy, sz);
            }
        }
        private static void AdjustAlignment(ref double Source, double Direction, ref double Speed, double TimeElapsed)
        {
            AdjustAlignment(ref Source, Direction, ref Speed, TimeElapsed, false);
        }
        private static void AdjustAlignment(ref double Source, double Direction, ref double Speed, double TimeElapsed, bool Zoom)
        {
            if (Direction != 0.0 | Speed != 0.0)
            {
                if (TimeElapsed > 0.0)
                {
                    if (Direction == 0.0)
                    {
                        double d = (0.025 + 5.0 * Math.Abs(Speed)) * TimeElapsed;
                        if (Speed >= -d & Speed <= d)
                        {
                            Speed = 0.0;
                        }
                        else
                        {
                            Speed -= (double)Math.Sign(Speed) * d;
                        }
                    }
                    else
                    {
                        double t = Math.Abs(Direction);
                        double d = ((1.15 - 1.0 / (1.0 + 0.025 * Math.Abs(Speed)))) * TimeElapsed;
                        Speed += Direction * d;
                        if (Speed < -t)
                        {
                            Speed = -t;
                        }
                        else if (Speed > t)
                        {
                            Speed = t;
                        }
                    }
                    double x = Source + Speed * TimeElapsed;
                    if (!PerformProgressiveAdjustmentForCameraRestriction(ref Source, x, Zoom))
                    {
                        Speed = 0.0;
                    }
                }
            }
        }
        private static void ApplyZoom()
        {
            World.VerticalViewingAngle = World.OriginalVerticalViewingAngle * Math.Exp(World.CameraCurrentAlignment.Zoom);
            if (World.VerticalViewingAngle < 0.001) World.VerticalViewingAngle = 0.001;
            if (World.VerticalViewingAngle > 1.5) World.VerticalViewingAngle = 1.5;
            MainLoop.UpdateViewport(MainLoop.ViewPortChangeMode.NoChange);
        }
        #endregion

        #region update viewing distance
        internal static void UpdateViewingDistances()
        {
            double f = Math.Atan2(World.CameraTrackFollower.WorldDirection.Z, World.CameraTrackFollower.WorldDirection.X);
            double c = Math.Atan2(World.AbsoluteCameraDirection.Z, World.AbsoluteCameraDirection.X) - f;
            if (c < -Math.PI)
            {
                c += 2.0 * Math.PI;
            }
            else if (c > Math.PI)
            {
                c -= 2.0 * Math.PI;
            }
            double a0 = c - 0.5 * World.HorizontalViewingAngle;
            double a1 = c + 0.5 * World.HorizontalViewingAngle;
            double max;
            if (a0 <= 0.0 & a1 >= 0.0)
            {
                max = 1.0;
            }
            else
            {
                double c0 = Math.Cos(a0);
                double c1 = Math.Cos(a1);
                max = c0 > c1 ? c0 : c1;
                if (max < 0.0) max = 0.0;
            }
            double min;
            if (a0 <= -Math.PI | a1 >= Math.PI)
            {
                min = -1.0;
            }
            else
            {
                double c0 = Math.Cos(a0);
                double c1 = Math.Cos(a1);
                min = c0 < c1 ? c0 : c1;
                if (min > 0.0) min = 0.0;
            }
            double d = World.BackgroundImageDistance + World.ExtraViewingDistance;
            World.ForwardViewingDistance = d * max;
            World.BackwardViewingDistance = -d * min;
            ObjectManager.UpdateVisibility(World.CameraTrackFollower.TrackPosition + World.CameraCurrentAlignment.Position.Z, true);
        }
        #endregion

        // ================================
        #region mesh create normals
        internal static void CreateNormals(ref Mesh Mesh)
        {
            for (int i = 0; i < Mesh.Faces.Length; i++)
            {
                CreateNormals(ref Mesh, i);
            }
        }
        internal static void CreateNormals(ref Mesh Mesh, int FaceIndex)
        {
            if (Mesh.Faces[FaceIndex].Vertices.Length >= 3)
            {
                int i0 = (int)Mesh.Faces[FaceIndex].Vertices[0].Index;
                int i1 = (int)Mesh.Faces[FaceIndex].Vertices[1].Index;
                int i2 = (int)Mesh.Faces[FaceIndex].Vertices[2].Index;
                double ax = Mesh.Vertices[i1].Coordinates.X - Mesh.Vertices[i0].Coordinates.X;
                double ay = Mesh.Vertices[i1].Coordinates.Y - Mesh.Vertices[i0].Coordinates.Y;
                double az = Mesh.Vertices[i1].Coordinates.Z - Mesh.Vertices[i0].Coordinates.Z;
                double bx = Mesh.Vertices[i2].Coordinates.X - Mesh.Vertices[i0].Coordinates.X;
                double by = Mesh.Vertices[i2].Coordinates.Y - Mesh.Vertices[i0].Coordinates.Y;
                double bz = Mesh.Vertices[i2].Coordinates.Z - Mesh.Vertices[i0].Coordinates.Z;
                double nx = ay * bz - az * by;
                double ny = az * bx - ax * bz;
                double nz = ax * by - ay * bx;
                double t = nx * nx + ny * ny + nz * nz;
                if (t != 0.0)
                {
                    t = 1.0 / Math.Sqrt(t);
                    float mx = (float)(nx * t);
                    float my = (float)(ny * t);
                    float mz = (float)(nz * t);
                    for (int j = 0; j < Mesh.Faces[FaceIndex].Vertices.Length; j++)
                    {
                        if (Mesh.Faces[FaceIndex].Vertices[j].Normal.IsZero())
                        {
                            Mesh.Faces[FaceIndex].Vertices[j].Normal = new Vectors.Vector3Df(mx, my, mz);
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < Mesh.Faces[FaceIndex].Vertices.Length; j++)
                    {
                        if (Mesh.Faces[FaceIndex].Vertices[j].Normal.IsZero())
                        {
                            Mesh.Faces[FaceIndex].Vertices[j].Normal = new Vectors.Vector3Df(0.0f, 1.0f, 0.0f);
                        }
                    }
                }
            }
        }
        #endregion
    }
}