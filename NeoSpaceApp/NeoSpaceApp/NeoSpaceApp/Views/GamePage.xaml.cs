using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoSpaceApp.Models;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Devices.Sensors;
using NeoSpaceApp.Extensions;

namespace NeoSpaceApp
{
    public partial class GamePage : PhoneApplicationPage
    {
        ContentManager contentManager;
        GameTimer timer;
        SpriteBatch spriteBatch;

        SpriteFont myFont;
        ConstellationModel model;
        double horPos;
        double verPos;

        Motion motion;
        bool IsMotion;

        private double[] brightnesses = { 1.0, 0.9, 0.7, 1.0, 0.7, 0.4, 0.1 };
        private int xmid, ymid;

        Texture2D canvas;

        public GamePage()
        {
            InitializeComponent();

            // Get the content manager from the application
            contentManager = (Application.Current as App).Content;

            // Create a timer for this page
            timer = new GameTimer();
            timer.UpdateInterval = TimeSpan.FromTicks(333333);
            timer.Update += OnUpdate;
            timer.Draw += OnDraw;

            this.Width = System.Windows.Application.Current.Host.Content.ActualWidth;
            this.Height = System.Windows.Application.Current.Host.Content.ActualHeight;

            model = new ConstellationModel((int)this.Width, (int)this.Height);
            model.LoadData();

            verPos = (-model.dec) * 180.0 / Math.PI;
            horPos = (360.0 - model.ra * 180.0 / Math.PI);

            xmid = (int)this.Width / 2;
            ymid = (int)this.Height / 2;

            IsMotion = Motion.IsSupported;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the sharing mode of the graphics device to turn on XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(SharedGraphicsDeviceManager.Current.GraphicsDevice);
            canvas = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false,
    SurfaceFormat.Color);

            // TODO: use this.content to load your game content here
            myFont = contentManager.Load<SpriteFont>("SpriteFontNeo");

            // Start the timer
            timer.Start();

            TouchPanel.EnabledGestures = GestureType.Pinch | GestureType.FreeDrag;

            // If the Motion object is null, initialize it and add a CurrentValueChanged
            // event handler.
            if (Motion.IsSupported)
            {
                if (motion == null)
                {
                    motion = new Motion();
                    motion.TimeBetweenUpdates = TimeSpan.FromTicks(333333);
                    motion.CurrentValueChanged += motion_CurrentValueChanged;
                }

                // Try to start the Motion API.
                try
                {
                    motion.Start();
                }
                catch (Exception)
                {
                    IsMotion = false;
                }
            }

            base.OnNavigatedTo(e);
        }

        void motion_CurrentValueChanged(object sender, SensorReadingEventArgs<MotionReading> e)
        {
            verPos = 120 - MathHelper.ToDegrees(e.SensorReading.Attitude.Pitch);
            horPos = -MathHelper.ToDegrees(e.SensorReading.Attitude.Yaw);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Stop the timer
            timer.Stop();

            if (motion != null)
                motion.Stop();

            // Set the sharing mode of the graphics device to turn off XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(false);

            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// Allows the page to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        private void OnUpdate(object sender, GameTimerEventArgs e)
        {
            // TODO: Add your update logic here
            if (TouchPanel.IsGestureAvailable && !IsDrawing)
            {
                var gesture = TouchPanel.ReadGesture();

                if (gesture.GestureType == GestureType.FreeDrag && !IsMotion)
                {
                    foreach (TouchLocation location in TouchPanel.GetState())
                    {
                        TouchLocation prev;
                        if (location.TryGetPreviousLocation(out prev))
                        {
                            if (location.Position.X - prev.Position.X != 0)
                            {
                                horPos += (prev.Position.X - location.Position.X) * 0.1;

                                if (horPos > 360)
                                    horPos = 0;

                                if (horPos < 0)
                                    horPos = 360;
                            }

                            if (location.Position.Y - prev.Position.Y != 0)
                            {
                                verPos += (prev.Position.Y - location.Position.Y) * 0.1;

                                if (verPos > 360)
                                    verPos = 0;

                                if (verPos < 0)
                                    verPos = 360;
                            }
                        }
                    }
                }
                else if (gesture.GestureType == GestureType.Pinch)
                {
                    /*float scaleFactor = PinchZoom.GetScaleFactor(gesture.Position, gesture.Position2,
                gesture.Delta, gesture.Delta2);

                    if(scaleFactor > 1)
                        model.angle--;
                    else if(scaleFactor < 1)
                        model.angle++;

                    if (model.angle > 60)
                        model.angle = 60;
                    if (model.angle < 30)
                        model.angle = 30;
                    */
                }
            }

            model.ra = (360.0 - horPos) * Math.PI / 180.0;
            model.dec = -verPos * Math.PI / 180.0;
        }

        /// <summary>
        /// Allows the page to draw itself.
        /// </summary>
        private void OnDraw(object sender, GameTimerEventArgs e)
        {
            SharedGraphicsDeviceManager.Current.GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            if (model.uselines)
                if (model.usealtlines)
                    PaintAltLines();
                else
                    PaintLines();

            //use all stars
            model.usestars = model.starlists[model.loadedlists - 2];
            PaintStars();

            if (model.usenames)
                PaintNames();
            spriteBatch.End();
        }

        private void DrawCenteredString(String s, int x, int y, Color? color = null)
        {
            if (color == null)
                color = Color.Red;

            var si = myFont.MeasureString(s);
            spriteBatch.DrawString(myFont, s, new Vector2(x - (si.X / 2), y - (si.Y / 2)), (Color)color);
        }

        private void DrawLine(Vector2 start, Vector2 end, Color? color = null)
        {
            if (color == null)
                color = Color.Green;

            canvas.SetData<Color>(new[] { (Color)color });

            spriteBatch.Draw(canvas, start, null, (Color) color,
                         (float)Math.Atan2(end.Y - start.Y, end.X - start.X),
                         new Vector2(0f, (float)canvas.Height / 2),
                         new Vector2(Vector2.Distance(start, end), 1f),
                         SpriteEffects.None, 0f);
        }

        private void PaintNames()
        {
            int i, x, y;
            double dist, ex, ey, ez;
            double cr, sr, cd, sd, cr0, sr0, cd0, sd0;

            dist = (double)Math.Min(xmid, ymid) / Math.Tan(model.angle / 2.0);

            sr = Math.Sin(model.ra);
            cr = Math.Cos(model.ra);
            sd = Math.Sin(model.dec);
            cd = Math.Cos(model.dec);

            for (i = model.numconst - 1; i >= 0; i--)
            {
                sr0 = model.constcoords_math[i,0];
                cr0 = model.constcoords_math[i,1];
                sd0 = model.constcoords_math[i,2];
                cd0 = model.constcoords_math[i,3];
                ez = cd * cd0 * (cr * cr0 + sr * sr0) + sd * sd0;
                if (ez > 0.1)
                {
                    ey = -sd * cd0 * (cr * cr0 + sr * sr0) + cd * sd0;
                    ex = -cd0 * (sr * cr0 - cr * sr0);
                    x = xmid + (int)(-dist * ex / ez);
                    y = ymid + (int)(-dist * ey / ez);

                    var offset = 100;
                    if (x >= 0 - offset && x <= this.Width + offset && y >= 0 - offset && y <= this.Height + offset)
                        DrawCenteredString(model.constnames[i], x, y);
                }
            }
        }

        private void PaintStars()
        {
            int i, x, y;
            double dist, ex, ey, ez;
            double cr, sr, cd, sd, cr0, sr0, cd0, sd0;
            int b;

            dist = (double)Math.Min(xmid, ymid) / Math.Tan(model.angle / 2.0);

            sr = Math.Sin(model.ra);
            cr = Math.Cos(model.ra);
            sd = Math.Sin(model.dec);
            cd = Math.Cos(model.dec);

            for (i = model.usestars - 1; i >= 0; i--)
            {
                b = (int)model.stars[i,2];
                b = b < 0 ? 0 : (b > 6 ? 6 : b);
                var color = new Color((float)255.0, (float)255.0, (float)255.0);
                color = color * (float)brightnesses[b];

                /* Projected view */
                /*
                      ex = -cos(top.stars[i][1]) * (sin(top.ra)*cos(top.stars[i][0]) - 
                                   cos(top.ra)*sin(top.stars[i][0]));
                      ey = -cos(top.stars[i][1])*sin(top.dec) * 
                            (cos(top.ra)*cos(top.stars[i][0]) + 
                             sin(top.ra)*sin(top.stars[i][0]) + 
                        cos(top.stars[i][1])*cos(top.dec);
                      ez = cos(top.stars[i][1])*cos(top.dec) * 
                            (cos(top.ra)*cos(top.stars[i][0]) + 
                             sin(top.ra)*sin(top.stars[i][0]) + 
                        sin(top.stars[i][1])*sin(top.dec);
                */

                sr0 = model.stars_math[i,0];
                cr0 = model.stars_math[i,1];
                sd0 = model.stars_math[i,2];
                cd0 = model.stars_math[i,3];

                ez = cd * cd0 * (cr * cr0 + sr * sr0) + sd * sd0;

                if (ez > 0.1)
                {

                    ey = -sd * cd0 * (cr * cr0 + sr * sr0) + cd * sd0;
                    ex = -cd0 * (sr * cr0 - cr * sr0);

                    x = xmid + (int)(-dist * ex / ez);
                    y = ymid + (int)(-dist * ey / ez);

                    if (x >= 0 && x <= this.Width && y >= 0 && y <= this.Height)
                    {
                        switch (b)
                        {
                            case 0:
                                DrawLine(new Vector2(x - 2, y), new Vector2(x + 2, y), color);
                                DrawLine(new Vector2(x, y - 2), new Vector2(x, y + 2), color);
                                DrawLine(new Vector2(x - 1, y - 1), new Vector2(x + 1, y + 1), color);
                                DrawLine(new Vector2(x + 1, y - 1), new Vector2(x - 1, y + 1), color);
                                break;
                            case 1:
                            case 2:
                                DrawLine(new Vector2(x - 2, y - 2), new Vector2(x + 2, y + 2), color);
                                DrawLine(new Vector2(x + 2, y - 2), new Vector2(x - 2, y + 2), color);
                                break;
                            default:
                                DrawLine(new Vector2(x - 1, y), new Vector2(x + 1, y), color);
                                DrawLine(new Vector2(x, y - 1), new Vector2(x, y + 1), color);
                                //DrawLine(new Vector2(x, y), new Vector2(x, y), color);
                                break;
                        }
                    }
                }
            }
        }

        private void PaintAltLines()
        {
            int i, x1, x2, y1, y2;
            double dist, ex1, ex2, ey1, ey2, ez1, ez2;
            double cr, sr, cd, sd, cr1, sr1, cd1, sd1, cr2, sr2, cd2, sd2;

            dist = (double)Math.Min(xmid, ymid) / Math.Tan(model.angle / 2.0);
            sr = Math.Sin(model.ra);
            cr = Math.Cos(model.ra);
            sd = Math.Sin(model.dec);
            cd = Math.Cos(model.dec);

            for (i = model.altnumlines - 1; i >= 0; i--)
            {
                sr1 = model.altlines_math[i,0];
                cr1 = model.altlines_math[i,1];
                sd1 = model.altlines_math[i,2];
                cd1 = model.altlines_math[i,3];
                sr2 = model.altlines_math[i,4];
                cr2 = model.altlines_math[i,5];
                sd2 = model.altlines_math[i,6];
                cd2 = model.altlines_math[i,7];
                ez1 = cd * cd1 * (cr * cr1 + sr * sr1) + sd * sd1;
                ez2 = cd * cd2 * (cr * cr2 + sr * sr2) + sd * sd2;
                if (ez1 > 0.1 && ez2 > 0.1)
                {
                    ey1 = -sd * cd1 * (cr * cr1 + sr * sr1) + cd * sd1;
                    ey2 = -sd * cd2 * (cr * cr2 + sr * sr2) + cd * sd2;
                    ex1 = -cd1 * (sr * cr1 - cr * sr1);
                    ex2 = -cd2 * (sr * cr2 - cr * sr2);

                    x1 = xmid + (int)(-dist * ex1 / ez1);
                    x2 = xmid + (int)(-dist * ex2 / ez2);
                    y1 = ymid + (int)(-dist * ey1 / ez1);
                    y2 = ymid + (int)(-dist * ey2 / ez2);
                    
                    if ((x1 >= 0 && x1 <= this.Width && y1 >= 0 && y1 <= this.Height) || (x2 >= 0 && x2 <= this.Width && y2 >= 0 && y2 <= this.Height))
                    {
                        DrawLine(new Vector2(x1, y1), new Vector2(x2, y2));
                    }
                }
            }
        }

        private void PaintLines()
        {
            int i, x1, x2, y1, y2;
            double dist, ex1, ex2, ey1, ey2, ez1, ez2;
            double cr, sr, cd, sd, cr1, sr1, cd1, sd1, cr2, sr2, cd2, sd2;

            dist = (double)Math.Min(xmid, ymid) / Math.Tan(model.angle / 2.0);
            sr = Math.Sin(model.ra);
            cr = Math.Cos(model.ra);
            sd = Math.Sin(model.dec);
            cd = Math.Cos(model.dec);

            for (i = model.numlines - 1; i >= 0; i--)
            {
                sr1 = model.lines_math[i,0];
                cr1 = model.lines_math[i,1];
                sd1 = model.lines_math[i,2];
                cd1 = model.lines_math[i,3];
                sr2 = model.lines_math[i,4];
                cr2 = model.lines_math[i,5];
                sd2 = model.lines_math[i,6];
                cd2 = model.lines_math[i,7];
                ez1 = cd * cd1 * (cr * cr1 + sr * sr1) + sd * sd1;
                ez2 = cd * cd2 * (cr * cr2 + sr * sr2) + sd * sd2;
                if (ez1 > 0.1 && ez2 > 0.1)
                {
                    ey1 = -sd * cd1 * (cr * cr1 + sr * sr1) + cd * sd1;
                    ey2 = -sd * cd2 * (cr * cr2 + sr * sr2) + cd * sd2;
                    ex1 = -cd1 * (sr * cr1 - cr * sr1);
                    ex2 = -cd2 * (sr * cr2 - cr * sr2);

                    x1 = xmid + (int)(-dist * ex1 / ez1);
                    x2 = xmid + (int)(-dist * ex2 / ez2);
                    y1 = ymid + (int)(-dist * ey1 / ez1);
                    y2 = ymid + (int)(-dist * ey2 / ez2);

                    if ((x1 >= 0 && x1 <= this.Width && y1 >= 0 && y1 <= this.Height) || (x2 >= 0 && x2 <= this.Width && y2 >= 0 && y2 <= this.Height))
                    {
                        DrawLine(new Vector2(x1, y1), new Vector2(x2, y2));
                    }
                }
            }
        }
    }
}