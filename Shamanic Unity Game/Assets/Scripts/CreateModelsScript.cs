using UnityEngine;
using System.Collections;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Models.Markov.Learning;
using Accord.Statistics.Models.Markov.Topology;
using Accord.Statistics.Distributions.Fitting;
using ShamanicInterface.DataStructure;
using ShamanicInterface.Utils;

public class CreateModelsScript : MonoBehaviour {



	// Use this for initialization
	void Start () {
			CreateModels();
	}

	public void CreateModels() {
		System.IO.Directory.CreateDirectory("GestureModels");
		CreateModelFromFrames("Frames/GRAB.frs", "GestureModels"+"/GRAB.bin");
		CreateModelFromFrames("Frames/DRINK_NL.frs", "GestureModels"+"/DRINK_NL.bin");
		CreateModelFromFrames("Frames/DRINK_PT.frs", "GestureModels"+"/DRINK_PT.bin");
		CreateModelFromFrames("Frames/OPEN_HAND.frs", "GestureModels"+"/OPEN_HAND.bin");
		CreateModelFromFrames("Frames/CLOSE_HAND.frs", "GestureModels"+"/CLOSE_HAND.bin");
		CreateMoveModels();
		CreateNumberModels();
		CreatePauseModels();
		CreateSoundModels();
		CreateOptionsModels();
	}

	public void CreateMoveModels() {
		CreateModelFromFrames("Frames/POINT_FRONT.frs", "GestureModels"+"/POINT_FRONT.bin");
		CreateModelFromFrames("Frames/POINT_RIGHT.frs", "GestureModels"+"/POINT_RIGHT.bin");
		CreateModelFromFrames("Frames/POINT_LEFT.frs", "GestureModels"+"/POINT_LEFT.bin");
		CreateModelFromFrames("Frames/POINT_BACK.frs", "GestureModels"+"/POINT_BACK.bin");
		CreateModelFromFrames("Frames/OPEN_FRONT.frs", "GestureModels"+"/OPEN_FRONT.bin");
		CreateModelFromFrames("Frames/OPEN_RIGHT.frs", "GestureModels"+"/OPEN_RIGHT.bin");
		CreateModelFromFrames("Frames/OPEN_LEFT.frs", "GestureModels"+"/OPEN_LEFT.bin");
	}

	public void CreateNumberModels() {
		CreateModelFromFrames("Frames/NUM1.frs", "GestureModels"+"/NUM1.bin");
		CreateModelFromFrames("Frames/NUM2.frs", "GestureModels"+"/NUM2.bin");
		CreateModelFromFrames("Frames/NUM3.frs", "GestureModels"+"/NUM3.bin");
	}

	public void CreateSoundModels() {
		CreateModelFromFrames("Frames/INDEX_HUSH.frs", "GestureModels"+"/INDEX_HUSH.bin");
		CreateModelFromFrames("Frames/MOUTH_MIMIC.frs", "GestureModels"+"/MOUTH_MIMIC.bin");
	}

	public void CreatePauseModels() {
		CreateModelFromFrames("Frames/HALT_HAND.frs", "GestureModels"+"/HALT_HAND.bin");
		CreateModelFromFrames("Frames/HAND_ROTATING.frs", "GestureModels"+"/HAND_ROTATING.bin");
		CreateModelFromFrames("Frames/INDEX_ROTATING.frs", "GestureModels"+"/INDEX_ROTATING.bin");
		CreateModelFromFrames("Frames/WAVE.frs", "GestureModels"+"/WAVE.bin");
	}

	public void CreateOptionsModels() {
		CreateModelFromFrames("Frames/THE_RING.frs", "GestureModels"+"/THE_RING.bin");
		CreateModelFromFrames("Frames/THUMBS_DOWN.frs", "GestureModels"+"/THUMBS_DOWN.bin");
		CreateModelFromFrames("Frames/THUMBS_UP.frs", "GestureModels"+"/THUMBS_UP.bin");
		CreateModelFromFrames("Frames/WAVE_NO_THANKS.frs", "GestureModels"+"/WAVE_NO_THANKS.bin");
	}

	public void CreateTestModels() {
		/*CreateModelFromFrames("Frames/OPEN_HAND.frs", "GestureModels/OPEN_HAND.bin");
		CreateModelFromFrames("Frames/HALT_HAND.frs", "GestureModels/HALT_HAND.bin");
		CreateModelFromFrames("Frames/HALT_HANDR.frs", "GestureModels/HALT_HANDR.bin");
		CreateModelFromFrames("Frames/HALT_HANDR_2x.frs", "GestureModels/HALT_HANDR_2x.bin");
		CreateModelFromFrames("Frames/HALT_HAND_half.frs", "GestureModels/HALT_HAND_half.bin");
		CreateModelFromFrames("Frames/HALT_HAND_half2x.frs", "GestureModels/HALT_HAND_half2x.bin");
		CreateModelFromFrames("Frames/HALT_HAND_alt.frs", "GestureModels/HALT_HAND_alt.bin");*/
	}

	public static void CreateModelFromFrames(string readPath, string writePath) {

		SequenceList seq = Utils.FramesToSequenceList(Utils.LoadListListFrame(readPath));

		HiddenMarkovModel<MultivariateNormalDistribution> hmm;
		MultivariateNormalDistribution mnd = new MultivariateNormalDistribution(seq.GetArray()[0][0].Length);
		hmm = new HiddenMarkovModel<MultivariateNormalDistribution>(new Forward(5), mnd);

		var teacher = new BaumWelchLearning<MultivariateNormalDistribution>(hmm);
		teacher.Tolerance = 0.0001;
		teacher.Iterations = 0;
		teacher.FittingOptions = new NormalOptions()
		{
			Diagonal = true,      // only diagonal covariance matrices
			Regularization = 1e-5 // avoid non-positive definite errors
		};
		
		double logLikelihood = teacher.Run(seq.GetArray());
		
		Debug.Log(readPath + " - " + seq.sequences.Count + " - " + logLikelihood);

		hmm.Save(writePath);
	}
}
