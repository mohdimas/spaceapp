using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace NeoSpaceApp.Models
{
    class ConstellationModel
    {
        //  Variables used as parameters: must be predefined here!
        public double ra = 0.7408745, dec = 39.8714421;
        public double angle = 45.0;
        public int dataset = 0, linedataset = 2, altlinedataset = 3; // dataset: 0 bsc, 1 ybs

        // Global variables
        public double[,] stars, stars_math;
        public int numstars, usestars, loadedlists;
        public int[] starlists;
        public int width, height;
        public double[,] lines, lines_math;
        public int numlines;
        public double[,] altlines, altlines_math;
        public int altnumlines;
        public bool uselines, usealtlines;
        public String[] constnames;
        public double[,] constcoords, constcoords_math;
        public int numconst;
        public bool usenames;

        public ConstellationModel(int _width, int _height)
        {
            width = _width;
            height = _height;

            ra = ra * Math.PI / 12.0;
            dec = dec * Math.PI / 180;
            angle = angle * Math.PI / 180;

            stars = new double[10000,3];
            stars_math = new double[10000,4];
            starlists = new int[7];
            numstars = 0;
            usestars = 0;
            lines = new double[1000,4];
            lines_math = new double[1000,8];
            numlines = 0;
            altlines = new double[1000,4];
            altlines_math = new double[1000,8];
            altnumlines = 0;
            constnames = new String[100];
            constcoords = new double[100,2];
            constcoords_math = new double[100,4];
            numconst = 0;
            uselines = true;
            usealtlines = true;
            usenames = true;
        }

        public void LoadData()
        {
            getData(1);
            getData(2);
            getConsts();
            getData(3);
            getStdLines();
            getData(4);
            getAltLines();
            getData(5);
            getData(6);
            getData(7);
        }

        public void getConsts() 
        {
            int i = 0, j = 0;

            try
            {
                var s = "data/averagebdys.dat";
                var si = Application.GetResourceStream(new Uri(s, UriKind.Relative));
                if (si != null)
                {
                    using (var reader = new StreamReader(si.Stream))
                    {
                        var content = reader.ReadToEnd();
                        var contents = content.Split(new string[] { " ", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var c in contents)
                        {
                            if (i == 0)
                            {
                                constnames[j] = c;
                            }
                            else if (i == 1)
                            {
                                constcoords[j,0] = double.Parse(c) * Math.PI / 12.0;
                                constcoords_math[j,0] = Math.Sin(constcoords[j,0]);
                                constcoords_math[j,1] = Math.Cos(constcoords[j,0]);
                            }
                            else
                            {
                                constcoords[j, 1] = double.Parse(c) * Math.PI / 180.0;
                                constcoords_math[j,2] = Math.Sin(constcoords[j,1]);
                                constcoords_math[j,3] = Math.Cos(constcoords[j,1]);
                                j++;
                                i = -1;
                            }
                            i++;
                        }
                    }
                }
            }
            catch (Exception)
            {
                numconst = 0;
                usenames = false;
                return;
            }

            numconst = j;
            //deliverEvent(new Event(this, theApp.NEW_STUFF, this));
        }

        public void getStdLines()
        {
            numlines = getLines(linedataset);
            //theApp.newStuff();
        }

        public void getAltLines()
        {
            if (linedataset == altlinedataset)
            {
                if (numlines == 0)
                {
                    uselines = false;
                }
            }
            else
            {
                altnumlines = getAltLines(altlinedataset);
                if (numlines == 0)
                {
                    usealtlines = true;
                    if (altnumlines == 0)
                    {
                        uselines = false;
                    }
                }
                //deliverEvent(new Event(this, NEW_STUFF, this));
            }
        }

        public int getAltLines(int catalog)
        {
            int i = 0, j = 0;
            String s;

            try
            {
                if (catalog == 0)
                    s = "data/lines.dat";
                else
                    s = "data/lines" + catalog + ".dat";

                var si = Application.GetResourceStream(new Uri(s, UriKind.Relative));
                if (si != null)
                {
                    using (var reader = new StreamReader(si.Stream))
                    {
                        var content = reader.ReadToEnd();
                        var contents = content.Split(new string[] { " ", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var c in contents)
                        {
                            altlines[j, i] = double.Parse(c);
                            altlines_math[j, 2 * i] = Math.Sin(double.Parse(c));
                            altlines_math[j, 2 * i + 1] = Math.Cos(double.Parse(c));
                            if ((++i) == 4)
                            {
                                j++;
                                i = 0;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return 0;
            }

            return j;
        }

        public int getLines(int catalog) {
            int i = 0, j = 0;
            String s;

            try
            {
                if (catalog == 0)
                    s = "data/lines.dat";
                else
                    s = "data/lines" + catalog + ".dat";

                var si = Application.GetResourceStream(new Uri(s, UriKind.Relative));
                if (si != null)
                {
                    using (var reader = new StreamReader(si.Stream))
                    {
                        var content = reader.ReadToEnd();
                        var contents = content.Split(new string[] { " ", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var c in contents)
                        {
                            lines[j,i] = double.Parse(c);
                            lines_math[j, 2 * i] = Math.Sin(double.Parse(c));
                            lines_math[j, 2 * i + 1] = Math.Cos(double.Parse(c));
                            if ((++i) == 4)
                            {
                                j++;
                                i = 0;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return 0;
            }

            return j;
        }

        public void getData(int n) 
        {
            int i;
            String url;

            switch (dataset) {
                case 0:
                    if (n == 0)
                        url = "data/bsc.dat";
                    else
                        url = "data/bsc" + n + ".dat";
                    break;

                case 1:
                    if (n == 0)
                        url = "data/YBS.dat";
                    else
                        url = "data/YBS" + n + ".dat";
                    break;

                default:
                    //Console.Out.WriteLine("Invalid data set requested");
                    return;
            }

            i = getFile(url, numstars);
            if (i > 0) {
                numstars += i; 
                starlists[loadedlists++] = numstars;
            }
        }

        public int getFile(String s, int n) {
            int i, j = 0;
            try
            {
                var si = Application.GetResourceStream(new Uri(s, UriKind.Relative));
                if (si != null)
                {
                    using (var reader = new StreamReader(si.Stream))
                    {
                        var content = reader.ReadToEnd();
                        var contents = content.Split(new string[] { " ", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                        i = 0;
                        j = 0;
                        foreach (var c in contents)
                        {
                            stars[n + j, i] = double.Parse(c);
                            if (i != 2)
                            {
                                stars_math[n + j, 2 * i] = Math.Sin(double.Parse(c));
                                stars_math[n + j, 2 * i + 1] = Math.Cos(double.Parse(c));
                            }
                            if ((++i) == 3)
                            {
                                j++;
                                i = 0;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return 0;
            }
            
            return j;
        }
    }
}
