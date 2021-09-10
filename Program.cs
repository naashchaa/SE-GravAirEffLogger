using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {

        public struct eff
            {
                public eff(double hsea, double hsurf, double g1, double atmo1, double ion1)
                {
                this.g = g1;
                this.atmo = atmo1;
                this.ion = ion1;
                this.heightSea = hsea;
                this.heightSurface = hsurf;
                }

            public double g { get; }
            public double atmo { get; }
            public double ion { get; }
            public double heightSea { get; }
            public double heightSurface { get; }

            public override string ToString()
            {
                String savedParam = g.ToString(".##") + "," + atmo.ToString(".##") + "," + ion.ToString(".##") + ",\n";
                return savedParam;
            }
        }
        


        Queue<eff> preStorage;

        public Program()
        {
            preStorage = new Queue<eff>();
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
            atmthr = GridTerminalSystem.GetBlockWithName("atmthr1") as IMyThrust;
            ionthr = GridTerminalSystem.GetBlockWithName("ionthr1") as IMyThrust;
            cockpit = GridTerminalSystem.GetBlockWithName("cockpit") as IMyCockpit;
            buttons = GridTerminalSystem.GetBlockWithName("buttonpanel") as IMyButtonPanel;
            light = GridTerminalSystem.GetBlockWithName("powerlight") as IMyInteriorLight;
            indicator = GridTerminalSystem.GetBlockWithName("indlight") as IMyInteriorLight;
        }

            StringBuilder prePreStorage = new StringBuilder();
        public void Save()
        {
            Storage = "";
            while (preStorage.Count > 0)
            {
                eff eff1 = preStorage.Dequeue();
                prePreStorage.AppendFormat("{0:.##}", eff1.heightSea).Append(",").AppendFormat("{0:.##}", eff1.heightSurface).Append(",").AppendFormat("{0:0.##}", eff1.g).Append(",").AppendFormat("{0:0.##}", eff1.atmo).Append(",").AppendFormat("{0:0.##}", eff1.ion).Append(",\n");
            }
            Storage = prePreStorage.ToString();
        }

        IMyThrust atmthr;
        IMyThrust ionthr;
        IMyCockpit cockpit;
        IMyInteriorLight light;
        IMyInteriorLight indicator;

        IMyButtonPanel buttons;

        double atmeff;
        double ioneff;
        double grav;
        eff preStorageElement;
        int tickStorage = 0;
        double elevation;
        double heightSea;
        double heightSurf;

        public void Main(string argument, UpdateType updateSource)
        {
            


            if (tickStorage == 5 && cockpit.TryGetPlanetElevation(MyPlanetElevation.Sealevel, out elevation))
            {
                indicator.BlinkIntervalSeconds = (float)0.5;
                atmeff = atmthr.MaxEffectiveThrust / atmthr.MaxThrust;
                Echo($"{atmeff}");
                ioneff = ionthr.MaxEffectiveThrust / ionthr.MaxThrust;
                Echo($"{ioneff}");
                grav = cockpit.GetNaturalGravity().Length();
                cockpit.TryGetPlanetElevation(MyPlanetElevation.Sealevel, out heightSea);
                cockpit.TryGetPlanetElevation(MyPlanetElevation.Surface, out heightSurf);

                preStorageElement = new eff(heightSea, heightSurf, grav, atmeff, ioneff);
                preStorage.Enqueue(preStorageElement);
                Echo("Added element to queue");
                tickStorage = 0;
            }
            else
                tickStorage++;

            Runtime.UpdateFrequency = UpdateFrequency.Update1;

            if (!light.Enabled)
            {
                Runtime.UpdateFrequency = UpdateFrequency.None;
                indicator.BlinkIntervalSeconds = 0;
            }

            
            
        }
    }
}
