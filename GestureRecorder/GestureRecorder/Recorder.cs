using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leap;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ShamanicInterface.Utils;

namespace GestureRecorder
{

    public class Recorder
    {
        public enum RecorderState { Starting, Reading, Idle, Storing, Saving };

        int numOfReads;
        int numOfFramesPerRead;
        bool isAuto;
        List<List<Frame>> sequencesToRead;
        public RecorderState state;

        private int actualNumOfReads;

        public Recorder(int numReads, int numFramesRead, bool auto)
        {
            numOfReads = numReads;
            numOfFramesPerRead = numFramesRead;
            isAuto = auto;
            sequencesToRead = new List<List<Frame>>();
            state = RecorderState.Starting;
            actualNumOfReads = 0;

        }

        public void Read(string path)
        {
            RecordListener listener = new RecordListener();
            listener.Initialization(numOfFramesPerRead, this);
            Controller controller = new Controller();
            controller.AddListener(listener);

            actualNumOfReads = 0;
            Console.WriteLine("Press enter to start reading frames to file " + path);
            Console.ReadLine();
            state = RecorderState.Idle;

            while (state != RecorderState.Saving)
            {
                Console.WriteLine("num: " + (actualNumOfReads + 1) + " in " + numOfReads);
                if (!isAuto)
                {
                    Console.ReadLine();
                }
                state = RecorderState.Reading;
                Console.WriteLine("Reading...");

                while (state == RecorderState.Reading)
                {
                    if (numOfFramesPerRead == 0)
                    {
                        Console.ReadLine();
                        listener.GetSequence();
                    }
                }
                while (state == RecorderState.Storing) { }
            }

            while (state != RecorderState.Saving) { }

            Utils.SaveListListFrame(sequencesToRead, path);

            controller.RemoveListener(listener);
            controller.Dispose();
        }

        public void Store(List<Frame> handSeq)
        {
            actualNumOfReads++;
            sequencesToRead.Add(handSeq);

            if (actualNumOfReads >= numOfReads)
            {
                state = RecorderState.Saving;
            }
            else
            {
                state = RecorderState.Idle;
            }
        }

        public void Save(string path)
        {
            Stream writeStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(writeStream, sequencesToRead);
            writeStream.Close();
        }
    }
}