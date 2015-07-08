using UnityEngine;
using System.Collections;
using Leap;
using ShamanicInterface.DataStructure;
using ShamanicInterface.Classifier;
using ShamanicInterface.Utils;

public class HandGesture : MonoBehaviour {

	private SequenceBuffer buffer = new SequenceBuffer(Game.bufferSize);

	public void AddSign(Hand hand, Frame previousFrame) {
		buffer.AddSign(ShamanicInterface.Utils.Utils.HandToSign(hand, previousFrame));
	}

	public string GetAction(HMMClassifier classifier) {
		return classifier.ComputeToString(buffer.getSequence().GetArray());
	}
}
