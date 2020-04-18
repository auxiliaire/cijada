using CA.Gfx.Palette.GradientEditor;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CA.Model
{
    [Serializable()]
    public class Settings : ISerializable
    {
        public bool Senescence { get; set; }
        public bool[,] CellRelation { get; set; }
        public int[] RuleLife { get; set; }
        public int[] RuleDeath { get; set; }
        public List<GradientStop> GradientMap { get; set; }

        public Settings()
        {
        }

        public Settings(SerializationInfo info, StreamingContext ctxt)
        {
            Senescence = (bool)info.GetValue("Senescence", typeof(bool));
            CellRelation = (bool[,])info.GetValue("CellRelation", typeof(bool[,]));
            RuleLife = (int[])info.GetValue("RuleLife", typeof(int[]));
            RuleDeath = (int[])info.GetValue("RuleDeath", typeof(int[]));
            GradientMap = (List<GradientStop>)info.GetValue("GradientMap", typeof(List<GradientStop>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Senescence", Senescence, typeof(bool));
            info.AddValue("CellRelation", CellRelation, typeof(bool[,]));
            info.AddValue("RuleLife", RuleLife, typeof(int[]));
            info.AddValue("RuleDeath", RuleDeath, typeof(int[]));
            info.AddValue("GradientMap", GradientMap, typeof(List<GradientStop>));
        }
    }
}

