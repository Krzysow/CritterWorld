using CritterController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFunnyNamespace
{
    class ControllerFactory : ICritterControllerFactory
    {
        public string Author => "100484498";

        public ICritterController[] GetCritterControllers()
        {
            List<ICritterController> controllers = new List<ICritterController>();
            for (int i = 0; i < 25; i++)
            {
                controllers.Add(new BombVoyage("Bomb Voyage" + (i + 1)));
                controllers.Add(new Chestburster("Chestburster" + (i + 1)));
                controllers.Add(new Peasant("Peasant" + (i + 1)));
            }
            return controllers.ToArray();
        }
    }
}
