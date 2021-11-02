using System;
using System.Collections.Generic;
using System.Linq;

namespace DrewMar.Domain
{
    internal class Package
    {
        public List<WoodenBoard> desks = new List<WoodenBoard>();
        public List<string> listOfDesksMakers = new List<string>();

        public double Capacity => Math.Round(desks.Sum(x => x.Weight), 4);

        public void AddDeskToList(WoodenBoard woodenBoard)
        {
            desks.Add(woodenBoard);
        }

        public void DeleteLastDeskFromList(WoodenBoard woodenBoard)
        {
            if (woodenBoard is null)
            {
                throw new ArgumentNullException(nameof(woodenBoard));
            }

            if (desks is null)
            {
                AutoClosingMessageBox.Show("Nie można usunąć deski, paczka pusta!", "Caption", 2000);
                return;
            }

            desks.RemoveAt(desks.Count - 1);
        }

        public List<string> AssignWorkersToPackage(List<string> list) => list.Distinct().ToList();

        public string GetAssignedWorkers() => string.Join(" ", listOfDesksMakers);

    }
}
