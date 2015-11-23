using System;
using System.Collections.Generic;
using CA.Gfx.Palette.GradientEditor;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace CA.Model
{
	[Serializable()]
	public class Settings : ISerializable
	{
		private bool senescence;
		private bool[,] cellRelation;
		private int[] ruleLife;
		private int[] ruleDeath;
		private List<GradientStop> gradientMap;

		public bool Senescence {
			get { return this.senescence; }
			set { this.senescence = value; }
		}

		public bool[,] CellRelation {
			get { return this.cellRelation; }
			set { this.cellRelation = value; }
		}

		public int[] RuleLife {
			get { return this.ruleLife; }
			set { this.ruleLife = value; }
		}

		public int[] RuleDeath {
			get { return this.ruleDeath; }
			set { this.ruleDeath = value; }
		}

		public List<GradientStop> GradientMap {
			get { return this.gradientMap; }
			set { this.gradientMap = value; }
		}

		// CONSTRUCTOR
		public Settings ()
		{
		}

		public Settings(SerializationInfo info, StreamingContext ctxt)
		{
			this.senescence = (bool)info.GetValue("Senescence", typeof(bool));
			this.cellRelation = (bool[,])info.GetValue("CellRelation", typeof(bool[,]));
			this.ruleLife = (int[])info.GetValue("RuleLife", typeof(int[]));
			this.ruleDeath = (int[])info.GetValue("RuleDeath", typeof(int[]));
			this.gradientMap = (List<GradientStop>)info.GetValue("GradientMap", typeof(List<GradientStop>));
		}

		public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			info.AddValue("Senescence", this.senescence);
			info.AddValue("CellRelation", this.cellRelation);
			info.AddValue("RuleLife", this.ruleLife);
			info.AddValue("RuleDeath", this.ruleDeath);
			info.AddValue("GradientMap", this.gradientMap);
		}
	}
}

