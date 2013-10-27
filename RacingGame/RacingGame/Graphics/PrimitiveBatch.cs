namespace RacingGame.Graphics
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class PrimitiveBatch : IDisposable
    {
        protected struct Shape
        {
            public List<VertexPositionColor> Vertexes;
            public List<short> Indexes;
            public PrimitiveType PrimitiveType;
            public int PrimitiveCount;

            public Shape(List<VertexPositionColor> vertexes, List<short> indexes, PrimitiveType primitiveType, int primitiveCount)
            {
                Vertexes = vertexes;
                PrimitiveType = primitiveType;
                PrimitiveCount = primitiveCount;
                Indexes = indexes;
            }

            public Shape(List<VertexPositionColor> vertexes, PrimitiveType primitiveType, int primitiveCount)
                : this(vertexes, new List<short>(), primitiveType, primitiveCount) { }
        }

        private bool _disposed = false;
        private bool _began = false;
        private BasicEffect _effect;
        private readonly int _z = 0;
        private readonly VertexDeclaration _vertexDecl;
        protected readonly List<Shape> _shapes = new List<Shape>();
        public GraphicsDevice GraphicsDevice { get; private set; }

        public PrimitiveBatch(GraphicsDevice graphicsDevice)
        {
            if (graphicsDevice == null)
                throw new ArgumentNullException("graphicsDevice");

            GraphicsDevice = graphicsDevice;
            _vertexDecl = VertexPositionColor.VertexDeclaration;

            _effect = new BasicEffect(graphicsDevice);
            _effect.VertexColorEnabled = true;
            _effect.World = Matrix.CreateRotationZ(MathHelper.ToRadians(-90));
        }

        private void AddFigure(List<VertexPositionColor> vertexes, List<short> indexes, PrimitiveType primitiveType)
        {
            int count = (indexes.Count > 0) ? indexes.Count / 3 : 1;
            Shape shape;

            if (_shapes.Count > 0 && (shape = _shapes[_shapes.Count - 1]).PrimitiveType == primitiveType)
            {
                int vertCount = shape.Vertexes.Count;

                for (int i = 0; i < indexes.Count; i++)
                    shape.Indexes.Add((short)(indexes[i] + vertCount));

                shape.Vertexes.AddRange(vertexes);

                _shapes[_shapes.Count - 1] = new Shape(
                    shape.Vertexes,
                    shape.Indexes,
                    shape.PrimitiveType,
                    shape.PrimitiveCount + count);
            }
            else
            {
                _shapes.Add(new Shape(vertexes, indexes, primitiveType, count));
            }
        }

        private Vector3 ConvertPoint(float x, float y)
        {
            return new Vector3(y / GraphicsDevice.Viewport.Height * 2f - 1f, x / GraphicsDevice.Viewport.Width * 2f - 1f, _z);
        }

        private void DrawClosedCurve(List<VertexPositionColor> vertexes)
        {
            _shapes.Add(new Shape(vertexes, PrimitiveType.LineStrip, vertexes.Count));
            vertexes.Add(vertexes[0]);
        }

        public void Begin()
        {
            if (_began == true)
                throw new InvalidOperationException("Last operation of drawing isn't completed.");

            _shapes.Clear();

            _began = true;
        }

        public void End()
        {
            if (_began == false)
                throw new InvalidOperationException("Operation of drawing isn't started.");

            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;

            GraphicsDevice.RasterizerState = rs;

            _effect.CurrentTechnique.Passes[0].Apply();

            for (int i = 0; i < _shapes.Count; i++)
            {
                if (_shapes[i].Indexes.Count == 0)
                {
                    GraphicsDevice.DrawUserPrimitives(_shapes[i].PrimitiveType, _shapes[i].Vertexes.ToArray(), 0, _shapes[i].PrimitiveCount);
                }
                else
                {
                    GraphicsDevice.DrawUserIndexedPrimitives(
                        _shapes[i].PrimitiveType, 
                        _shapes[i].Vertexes.ToArray(), 
                        0, 
                        _shapes[i].Vertexes.Count, 
                        _shapes[i].Indexes.ToArray(), 
                        0,
                        _shapes[i].PrimitiveCount);
                }
            }

            _began = false;
        }

        public virtual void Dispose()
        {
            if (!_disposed)
            {
                _effect.Dispose();
                _disposed = true;
            }
        }

        #region " Shapes "

        #region " Point "

        public void DrawPoint(float x, float y, Color color)
        {
            // XNA 3.1.
            //AddFigure(new List<VertexPositionColor>() { new VertexPositionColor(ConvertPoint(x, y), color) }, new List<int>(), PrimitiveType.PointList);

            DrawLine(x, y, color, x + 1, y, color);
        }

        public void DrawPoint(Point point, Color color)
        {
            DrawPoint(point.X, point.Y, color);
        }

        #endregion

        #region " Line "

        public void DrawLine(float x1, float y1, Color color1, float x2, float y2, Color color2)
        {
            AddFigure(new List<VertexPositionColor>() 
            { 
                new VertexPositionColor(ConvertPoint(x1, y1), color1), 
                new VertexPositionColor(ConvertPoint(x2, y2), color2) 
            },
            new List<short>(),
            PrimitiveType.LineList);
        }

        public void DrawLine(float x1, float y1, float x2, float y2, Color color)
        {
            DrawLine(x1, y1, color, x2, y2, color);
        }

        public void DrawLine(Point poin1, Color color1, Point poin2, Color color2)
        {
            DrawLine(poin1.X, poin1.Y, color1, poin2.X, poin2.Y, color2);
        }

        public void DrawLine(Point poin1, Point poin2, Color color)
        {
            DrawLine(poin1.X, poin1.Y, color, poin2.X, poin2.Y, color);
        }

        #endregion

        public void DrawClosedCurve(IEnumerable<Point> points, Color color)
        {
            if (points == null)
                throw new ArgumentNullException("points");

            List<VertexPositionColor> vertexes = new List<VertexPositionColor>();

            foreach (var point in points)
                vertexes.Add(new VertexPositionColor(ConvertPoint(point.X, point.Y), color));

            DrawClosedCurve(vertexes);
        }

        public void DrawCurve(IEnumerable<Point> points, Color color)
        {
            if (points == null)
                throw new ArgumentNullException("points");

            List<VertexPositionColor> vertexes = new List<VertexPositionColor>();

            foreach (var point in points)
                vertexes.Add(new VertexPositionColor(ConvertPoint(point.X, point.Y), color));

            _shapes.Add(new Shape(vertexes, PrimitiveType.LineStrip, vertexes.Count));
        }

        #region " Triangle "

        public void DrawTriangle(float x1, float y1, Color color1, float x2, float y2, Color color2, float x3, float y3, Color color3)
        {
            DrawClosedCurve(new List<VertexPositionColor>()
                {
                    new VertexPositionColor(ConvertPoint(x1, y1), color1),
                    new VertexPositionColor(ConvertPoint(x2, y2), color2),
                    new VertexPositionColor(ConvertPoint(x3, y3), color3)
                });
        }

        public void DrawTriangle(float x1, float y1, float x2, float y2, float x3, float y3, Color color)
        {
            DrawTriangle(x1, y1, color, x2, y2, color, x3, y3, color);
        }

        public void DrawTriangle(Point point1, Color color1, Point point2, Color color2, Point point3, Color color3)
        {
            DrawTriangle(point1.X, point1.Y, color1, point2.X, point2.Y, color2, point3.X, point3.Y, color3);
        }

        public void DrawTriangle(Point point1, Point point2, Point point3, Color color)
        {
            DrawTriangle(point1.X, point1.Y, color, point2.X, point2.Y, color, point3.X, point3.Y, color);
        }

        public void FillTriangle(float x1, float y1, Color color1, float x2, float y2, Color color2, float x3, float y3, Color color3)
        {
            AddFigure(new List<VertexPositionColor>()
                {
                    new VertexPositionColor(ConvertPoint(x1, y1), color1),
                    new VertexPositionColor(ConvertPoint(x2, y2), color2),
                    new VertexPositionColor(ConvertPoint(x3, y3), color3)
                },
                new List<short>(),
                PrimitiveType.TriangleList);
        }

        public void FillTriangle(float x1, float y1, float x2, float y2, float x3, float y3, Color color)
        {
            FillTriangle(x1, y1, color, x2, y2, color, x3, y3, color);
        }

        public void FillTriangle(Point point1, Color color1, Point point2, Color color2, Point point3, Color color3)
        {
            FillTriangle(point1.X, point1.Y, color1, point2.X, point2.Y, color2, point3.X, point3.Y, color3);
        }

        public void FillTriangle(Point point1, Point point2, Point point3, Color color)
        {
            FillTriangle(point1.X, point1.Y, color, point2.X, point2.Y, color, point3.X, point3.Y, color);
        }

        #endregion

        #region " Rectangle "

        public void DrawRectangle(float x, float y, float width, float height, Color color1, Color color2, Color color3, Color color4)
        {
            float x2 = x + width;
            float y2 = y + height;

            DrawClosedCurve(new List<VertexPositionColor>()
                {
                    new VertexPositionColor(ConvertPoint(x, y), color1),
                    new VertexPositionColor(ConvertPoint(x2, y), color2),
                    new VertexPositionColor(ConvertPoint(x2, y2), color3),
                    new VertexPositionColor(ConvertPoint(x, y2), color4)
                });
        }

        public void DrawRectangle(float x, float y, float width, float height, Color color)
        {
            DrawRectangle(x, y, width, height, color, color, color, color);
        }

        public void DrawRectangle(Point point, float width, float height, Color color1, Color color2, Color color3, Color color4)
        {
            DrawRectangle(point.X, point.Y, width, height, color1, color2, color3, color4);
        }

        public void DrawRectangle(Point point, float width, float height, Color color)
        {
            DrawRectangle(point, width, height, color, color, color, color);
        }

        public void DrawRectangle(Rectangle rectangle, Color color1, Color color2, Color color3, Color color4)
        {
            DrawRectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, color1, color2, color3, color4);
        }

        public void DrawRectangle(Rectangle rectangle, Color color)
        {
            DrawRectangle(rectangle, color, color, color, color);
        }

        public void FillRectangle(float x, float y, float width, float height, Color color1, Color color2, Color color3, Color color4)
        {
            float x2 = x + width;
            float y2 = y + height;

            AddFigure(new List<VertexPositionColor>()
                {
                    new VertexPositionColor(ConvertPoint(x, y), color1),
                    new VertexPositionColor(ConvertPoint(x2, y), color2),
                    new VertexPositionColor(ConvertPoint(x2, y2), color3),
                    new VertexPositionColor(ConvertPoint(x, y2), color4)
                },
                new List<short>() { 0, 1, 2, 2, 3, 0 },
                PrimitiveType.TriangleList);
        }

        public void FillRectangle(float x, float y, float width, float height, Color color)
        {
            FillRectangle(x, y, width, height, color, color, color, color);
        }

        public void FillRectangle(Point point, float width, float height, Color color1, Color color2, Color color3, Color color4)
        {
            FillRectangle(point.X, point.Y, width, height, color1, color2, color3, color4);
        }

        public void FillRectangle(Point point, float width, float height, Color color)
        {
            FillRectangle(point, width, height, color, color, color, color);
        }

        public void FillRectangle(Rectangle rectangle, Color color1, Color color2, Color color3, Color color4)
        {
            FillRectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, color1, color2, color3, color4);
        }

        public void FillRectangle(Rectangle rectangle, Color color)
        {
            FillRectangle(rectangle, color, color, color, color);
        }

        #endregion

        #region " Ellipse "

        public void DrawEllipse(float x, float y, float width, float height, Color color)
        {
            List<VertexPositionColor> vertexes = new List<VertexPositionColor>();

            for (int i = 0; i < 360; i++)
            {
                vertexes.Add(new VertexPositionColor(ConvertPoint(
                    (float)Math.Round(x + width * Math.Cos(i * Math.PI / 90)),
                    (float)Math.Round(y + height * Math.Sin(i * Math.PI / 90))),
                    color));
            }

            _shapes.Add(new Shape(vertexes, PrimitiveType.LineStrip, 180));
        }

        public void DrawEllipse(Point point, float width, float height, Color color)
        {
            DrawEllipse(point.X, point.Y, width, height, color);
        }

        public void DrawEllipse(Rectangle rectangle, Color color)
        {
            DrawEllipse(rectangle.X + rectangle.Width / 2, rectangle.Y + rectangle.Height / 2, rectangle.Width, rectangle.Height, color);
        }

        public void FillEllipse(float x, float y, float width, float height, Color color)
        {
            VertexPositionColor center = new VertexPositionColor(ConvertPoint(x, y), color);
            List<VertexPositionColor> vertexes = new List<VertexPositionColor>();
            List<short> indexes = new List<short>();

            double _segment = Math.PI / 90;

            for (short i = 0; i < 539; i++)
            {
                vertexes.Add(center);

                vertexes.Add(new VertexPositionColor(ConvertPoint(
                    (float)Math.Round(x + width * Math.Cos(i * _segment)),
                    (float)Math.Round(y + height * Math.Sin(i * _segment))),
                    color));

                short nextIndex = (short)(i + 1);

                vertexes.Add(new VertexPositionColor(ConvertPoint(
                    (float)Math.Round(x + width * Math.Cos(nextIndex * _segment)),
                    (float)Math.Round(y + height * Math.Sin(nextIndex * _segment))),
                    color));

                indexes.Add(0);
                indexes.Add(i);
                indexes.Add(nextIndex);
            }

            AddFigure(vertexes, indexes, PrimitiveType.TriangleList);
        }

        public void FillEllipse(Point point, float width, float height, Color color)
        {
            FillEllipse(point.X, point.Y, width, height, color);
        }

        public void FillEllipse(Rectangle rectangle, Color color)
        {
            FillEllipse(rectangle.X + rectangle.Width / 2, rectangle.Y + rectangle.Height / 2, rectangle.Width, rectangle.Height, color);
        }

        #endregion

        #region " Circle "

        public void DrawCircle(float x, float y, float radius, Color color)
        {
            DrawEllipse(x, y, radius, radius, color);
        }

        public void DrawCircle(Point point, float radius, Color color)
        {
            DrawEllipse(point, radius, radius, color);
        }

        public void FillCircle(float x, float y, float radius, Color color)
        {
            FillEllipse(x, y, radius, radius, color);
        }

        public void FillCircle(Point point, float radius, Color color)
        {
            FillEllipse(point, radius, radius, color);
        }

        #endregion

        #endregion
    }
}
