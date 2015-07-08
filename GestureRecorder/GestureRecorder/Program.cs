using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Models.Markov.Learning;
using Accord.Statistics.Models.Markov.Topology;
using Leap;
using ShamanicInterface.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GestureRecorder
{
    class Program
    {
        static string FRAMES_PATH = "Frames/";
        static string SAMPLE_PATH = "Frames/Samples/";
        static string EXTENSION = ".frs";

        static void Main(string[] args)
        {
            MainChoice();

            Console.WriteLine("Press Enter to quit...");
            Console.ReadLine();
        }

        static void MainChoice()
        {
            Console.Write(
                "1) Read\n"
                + "2) Aggregate\n"
                //+ "3) Test\n"
                );
            int a = int.Parse(Console.ReadLine());
            switch (a)
            {
                case 1:
                    MainRead();
                    break;
                case 2:
                    MainLoad();
                    break;
                default:
                    Console.WriteLine("NOT DEFINED");
                    break;
            }

        }

        static void MainRead()
        {
            Console.WriteLine("Read!");
            string gestureName = ReadString("Gesture Name: ");
            bool bothHands = ReadYN("Read both hands separately? (y/n) ");
            int numOfSepFiles = ReadInt("Number of separated files (per hand, if the case): ");
            bool autoRead = ReadYN("Start read automatically? (y/n) ");
            int numOfRead = ReadInt("Number of gestures per file: ");
            int numOfFrames = ReadInt("Number of frames per gesture (0 to stop manually): ");

            if (bothHands)
            {
                ReadN(gestureName + "R", numOfRead, numOfFrames, autoRead, numOfSepFiles);
                ReadN(gestureName + "L", numOfRead, numOfFrames, autoRead, numOfSepFiles);
            }
            else
            {
                ReadN(gestureName, numOfRead, numOfFrames, autoRead, numOfSepFiles);
            }

            //AggregateFrames(gestureName, bothHands, numOfSepFiles);
        }

        static bool ReadYN(string text)
        {
            while (true)
            {
                Console.Write(text);
                string read = Console.ReadLine();
                if (read == "y")
                {
                    return true;
                }

                if (read == "n")
                {
                    return false;
                }
            }
        }

        static string ReadString(string text)
        {
            Console.Write(text);
            return Console.ReadLine();
        }

        static int ReadInt(string text)
        {
            Console.Write(text);
            return Convert.ToInt32(Console.ReadLine());
        }

        static void ReadN(string filename, int nRead, int nFrame, bool auto, int N)
        {
            for (int i = 0; i < N; i++)
            {
                Read(filename + i, nRead, nFrame, auto);
            }
        }

        static void Read(string filename, int nRead, int nFrame, bool auto)
        {
            Recorder recorder = new Recorder(nRead, nFrame, auto);
            string path = SAMPLE_PATH + filename + EXTENSION;
            recorder.Read(path);

        }

        static void MainLoad()
        {
            Console.WriteLine("Aggregate!");

            string filename = ReadString("filename: ");
            bool bothHands = ReadYN("Both hands? (y/n) ");
            int numOfFiles = ReadInt("Number of Files? ");

            AggregateFrames(filename, bothHands, numOfFiles);
        }

        static void AggregateFrames(string filename, bool bothHands, int numOfFiles)
        {

            List<List<Frame>> endList = new List<List<Frame>>();

            if (bothHands)
            {
                endList = MixFrames(
                    AggregateFrames(filename + "R", numOfFiles),
                    AggregateFrames(filename + "L", numOfFiles));
            }
            else
            {
                endList = AggregateFrames(filename, numOfFiles);
            }

            if (endList.Count == 0)
            {
                Console.WriteLine("Files not found, or empty files");
                return;
            }

            Console.WriteLine("Saving file " + filename + EXTENSION + " with " + endList.Count + " examples.");
            Utils.SaveListListFrame(endList, FRAMES_PATH + filename + EXTENSION);
        }

        static List<List<Frame>> AggregateFrames(string filename, int numOfFiles)
        {
            List<List<Frame>> endList = new List<List<Frame>>();

            for (int i = 0; i < numOfFiles; i++)
            {
                endList.AddRange(Utils.LoadListListFrame(SAMPLE_PATH + filename + i + EXTENSION));
            }

            return endList;
        }

        static List<List<Frame>> MixFrames(List<List<Frame>> list1, List<List<Frame>> list2)
        {
            List<List<Frame>> returnList = new List<List<Frame>>();
            int index = 0;

            for (index = 0; index < list1.Count && index < list2.Count; index++)
            {
                returnList.Add(list1[index]);
                returnList.Add(list2[index]);
            }

            returnList.AddRange(list1.GetRange(index, list1.Count - index));
            returnList.AddRange(list2.GetRange(index, list2.Count - index));

            return returnList;
        }

    }
}
