using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardToGPS
{
    class Program
    {
        static void Main(string[] args)
        {
            cardinalMap cardMap ;
            double lon = 0f, lat = 0f;
            float head = 0f;
            string ParentDirectory = @"c:\";
            string MainDirectory = "Waypoints";
            string wayPointIn = "xyzPos.csv";
            string GPSout = "GPSPos.csv";
            string wayPointOut = "WayPoints.txt";
            string baseGPSFile = "BaseLocation.txt";
            List<double[]> newPositions = new List<double[]>();
            List<double[]> newGPSPositions = new List<double[]>();
            bool readBaseFile = false;
            
            
            Console.WriteLine("Checking for Waypoint Directory...");
            if (Directory.Exists(Path.Combine(ParentDirectory, MainDirectory)))
            {
                string infile = Path.Combine(ParentDirectory, MainDirectory, wayPointIn);
                newPositions = infile.readCSV();
                Console.WriteLine("Checking for Waypoint file...");

                string baseFile = Path.Combine(ParentDirectory, MainDirectory,baseGPSFile);
                if (File.Exists(baseFile))
                {
                    var reader = new StreamReader(File.OpenRead(baseFile));
                    int n = 0;
                    while (!reader.EndOfStream)
                    {
                        if (n > 1)
                        {
                            Console.WriteLine("read too many lines in base file...");
                            break;
                        }
                        var line = reader.ReadLine();
                        var values = line.Split(',');
                        double[] baseGPSdouble = new double[values.Length];
                        for (int i = 0; i < values.Length; i++)
                        {
                            if (!double.TryParse(values[i], out baseGPSdouble[i]))
                            {
                                Console.WriteLine("Couldn't read base coordinate, \"{0}\" \nMake sure it is a number.", values[i]);
                            }
                        }
                        lat = baseGPSdouble[0];
                        lon = baseGPSdouble[1];
                        head = (float)baseGPSdouble[2]; 
                        readBaseFile = true;
                        n++;
                    }
                }
            }
            else
            {
                Console.WriteLine("No directory found, creating directory \"{0}\"", Path.Combine(ParentDirectory, MainDirectory));
                Console.WriteLine("Load XYZ csv file into this directory called \"{0}\"", wayPointIn);
                Directory.CreateDirectory(Path.Combine(ParentDirectory, MainDirectory));

            }
            if (readBaseFile)
            {
                Console.WriteLine("Read base location from file, ({0},{1}) heading: {2}", lat, lon, head);
            }
            else
            {
                lat = readFloat("Enter Latitude of base station: ");
                lon = readFloat("Enter Longitude of base station: ");
                head = readFloat("Enter Heading for X-Axis: ");
            }
            cardMap = new cardinalMap(lon, lat, head);

            for (int i = 0; i < newPositions.Count; i++)
            {
                newGPSPositions.Add(cardMap.newPoint(newPositions[i][0], newPositions[i][1], newPositions[i][2]));
            }


            string outFile =  Path.Combine(ParentDirectory, MainDirectory, GPSout);
            bool amendFile = new bool().UserInput(string.Format("Replace existing file, {0}? (y/n)", outFile));
            for (int i = 0; i < newGPSPositions.Count; i++)
            {
                outFile.WriteLine(string.Format("{0:0.0000000},{1:0.0000000},{2}", newGPSPositions[i][0], newGPSPositions[i][1], newGPSPositions[i][2]), (i==0)?amendFile:true);
            }


            string outWPFile = Path.Combine(ParentDirectory, MainDirectory, wayPointOut);
            amendFile = new bool().UserInput(string.Format("Replace existing file, {0}? (y/n)", outWPFile));
            outWPFile.WriteLine("QGC WPL 110", amendFile);
            outWPFile.WriteLine(string.Format("0\t1\t0\t16\t0\t1\t0\t0\t{0:0.0000000}\t{1:0.0000000}\t0\t1", lat, lon), true);
            outWPFile.WriteLine(string.Format("1\t0\t3\t22\t0\t1\t0\t0\t{0:0.0000000}\t{1:0.0000000}\t{2}\t1", lat, lon, 4), true);
            for (int i = 0; i < newGPSPositions.Count; i++)
            {
                outWPFile.WriteLine(newGPSPositions[i].GPStoMP(i+2, 5.0f, 1.0f), true);
            }
            outWPFile.WriteLine(string.Format("{3}\t0\t3\t21\t0\t1\t0\t0\t{0:0.0000000}\t{1:0.0000000}\t{2}\t1", lat, lon, 4, newGPSPositions.Count+ 2), true);
            /*
            bool AmendFile = false;
            if (File.Exists(outFile))
            {
                AmendFile.UserInput(string.Format("Replace existing file, {0}? (y/n)", outFile));
                using (StreamWriter file = new StreamWriter(outFile, AmendFile))
                {
                    for (int i = 0; i < newGPSPositions.Count; i++)
                    {
                        file.WriteLine("{0:0.0000000},{1:0.0000000},{2}", newGPSPositions[i][0], newGPSPositions[i][1], newGPSPositions[i][2]);
                    }
                }
            }
            else
            {
                Console.WriteLine("No GPS output file created.\nCreating new output file.");
                using (StreamWriter file = new StreamWriter(outFile))
                {
                    for (int i = 0; i < newGPSPositions.Count; i++)
                    {
                        file.WriteLine("{0:0.0000000},{1:0.0000000},{2}", newGPSPositions[i][0],newGPSPositions[i][1], newGPSPositions[i][2]);
                    }
                }
            }
             */
            Console.ReadLine();
        }


        static float readFloat(string msg)
        {
            Console.WriteLine(msg);
            int i = 0;
            float number;
            while (!float.TryParse(Console.ReadLine(), out number))
            {
                if (i < 1)
                {
                    Console.WriteLine("Please use numbers only with negative and a decimal point where necessary");
                }
                else if (i < 2)
                {
                    Console.WriteLine("Shouldn't include letters or random symbols, no hieroglyphics please...");
                }
                else if (i < 3)
                {
                    Console.WriteLine("Are you having a stroke, should I call the doctor?");
                }
                else if (i >= 3)
                {
                    Console.WriteLine("It should be in a format like: \"-37.3949012\"");
                }
                i++;
            }
            Console.WriteLine("Great, read {0}", number.ToString());
            return number;
        }

        public static void WriteWPFile(string fileLoc, double[] GPS)
        {

        }

    }

    class cardinalMap
    {
        double[] _BaseLL;
        float _BaseHeading;

        float _Radius =  6378137.50f;

        public cardinalMap(double Longitude, double Latitude, float heading)
        {
            this._BaseHeading = heading;
            this._BaseLL = new double[] { Latitude.toRadian(), Longitude.toRadian() };
        }

        public float[] newPoint(double x, double y)
        {
            double d = Math.Sqrt(x * x + y * y);
            double brng = Math.Atan2(y, x);
            double lat2 = Math.Asin(Math.Sin(_BaseLL[0]) * Math.Cos(d / _Radius) +
                    Math.Cos(_BaseLL[0]) * Math.Sin(d / _Radius) * Math.Cos(brng));
            double lon2 = _BaseLL[1] + Math.Atan2(Math.Sin(brng) * Math.Sin(d / _Radius) * Math.Cos(_BaseLL[0]),
                          Math.Cos(d / _Radius) - Math.Sin(_BaseLL[0]) * Math.Sin(lat2));
            return new float[] {(float)lat2, (float)lon2};
        }

        public double[] newPoint(double x, double y, double z)
        {
            double d = Math.Sqrt(x * x + y * y);
            double brng = Math.Atan2(y, x);
            double lat2 = Math.Asin(Math.Sin(_BaseLL[0]) * Math.Cos(d / _Radius) +
                    Math.Cos(_BaseLL[0]) * Math.Sin(d / _Radius) * Math.Cos(brng));
            double lon2 = _BaseLL[1] + Math.Atan2(Math.Sin(brng) * Math.Sin(d / _Radius) * Math.Cos(_BaseLL[0]),
                          Math.Cos(d / _Radius) - Math.Sin(_BaseLL[0]) * Math.Sin(lat2));
            return new double[] { lat2 * 180.0 / Math.PI, lon2 * 180.0 / Math.PI, z };
        }


    }

    static class CustomExtensions
    {
        public static float toRadian(this float degree)
        {
            return 1.0f * degree * (float)Math.PI / 180;
        }

        public static float toDegree(this float degree)
        {
            return 180.0f * degree / (float)Math.PI;
        }

        public static double toRadian(this double degree)
        {
            return 1.0 * degree * Math.PI / 180;
        }

        public static double toDegree(this double degree)
        {
            return 180.0 * degree / Math.PI;
        }

        public static bool UserInput(this bool trigger, string msg)
        {
            Console.WriteLine(msg);
            while (true)
            {
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.Y:
                        return false;
                    case ConsoleKey.N:
                        return true;

                    default:
                        Console.WriteLine("Type y or n only");
                        break;
                }
            }
        }

        public static string GPStoMP(this double[] wp, int index, float delay, float radius)
        {
            // <INDEX> <CURRENT WP> <COORD FRAME> <COMMAND> <PARAM1> <PARAM2> <PARAM3> <PARAM4> <PARAM5/X/LONGITUDE> <PARAM6/Y/LATITUDE> <PARAM7/Z/ALTITUDE> <AUTOCONTINUE>
            return string.Format("{0}\t0\t3\t16\t{1}\t{2}\t{3}\t{4}\t{5:0.0000000}\t{6:0.0000000}\t{7}\t1", index, delay, radius, 0, 0, wp[0], wp[1], wp[2]);
        }

        /// <summary>
        /// Writes the msg to a file at the strings location. 
        /// Checks if the file exists and asks for amendment option.
        /// </summary>
        /// <param name="outFile"></param> The string representing the file location and format
        /// <param name="msg"></param> The msg to be printed on a new line.
        public static void WriteLine(this string outFile, string msg, bool amend)
        {
            if (File.Exists(outFile))
            {
                using (StreamWriter file = new StreamWriter(outFile, amend))
                {
                    file.WriteLine(msg);
                }
            }
            else
            {
                Console.WriteLine("No GPS output file created.\nCreating new output file.");
                using (StreamWriter file = new StreamWriter(outFile))
                {
                    file.WriteLine(msg);
                }
            }
        }


        public static List<double[]> readCSV(this string file)
        {

            if (File.Exists(file))
            {
                var reader = new StreamReader(File.OpenRead(file));
                List<double[]> newPositions = new List<double[]>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    double[] newPos = new double[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (!double.TryParse(values[i], out newPos[i]))
                        {
                            Console.WriteLine("Couldn't read coordinate, \"{0}\" \nMake sure it is a number.", values[i]);
                        }
                    }
                    newPositions.Add(newPos);
                }
                Console.WriteLine("Read {0} points.", newPositions.Count);
                return newPositions;
            }
            else
            {
                Console.WriteLine("No such csv file, \"{0}\"", file);
                return null;
            }
        }
    }

}
