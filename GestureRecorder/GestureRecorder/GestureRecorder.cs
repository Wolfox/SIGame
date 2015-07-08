using Leap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestureRecorder
{
    class RecordListener : Listener
    {

        private int numOfFramesPerSeq;
        private List<Frame> sequence;
        private Recorder parent;

        public void Initialization(int num, Recorder p)
        {
            numOfFramesPerSeq = num;
            parent = p;
            sequence = new List<Frame>();
        }

        public override void OnConnect(Controller controller)
        {
            Console.WriteLine("Connected, using SampleListener");
        }

        public override void OnFrame(Controller controller)
        {
            if (parent.state != Recorder.RecorderState.Reading) { return; }

            Frame frame = controller.Frame();
            HandList hands = frame.Hands;
            Hand hand = hands.Rightmost;
            if (!hand.IsValid) { return; }
            //if (hands.Count > 1) { Console.WriteLine("MORE THAN 1 HAND"); return; }

            sequence.Add(frame);
            if (sequence.Count >= numOfFramesPerSeq && numOfFramesPerSeq != 0)
            {
                GetSequence();
            }
        }

        public void GetSequence()
        {
            parent.state = Recorder.RecorderState.Storing;
            parent.Store(sequence);
            sequence = new List<Frame>();
        }
    }
}