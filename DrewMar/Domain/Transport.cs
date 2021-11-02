using System;
using System.Collections.Generic;
using System.Linq;

namespace DrewMar.Domain
{
    internal class Transport
    {
        public int PackagesCount => packages.Count + 1;
        public List<Package> packages;
        public DateTime CompletionTime { get; private set; }

        public Transport()
        {
            packages = new List<Package>();
        }

        public double Capacity => packages.Sum(x => x.Capacity);

        public void AddPackage(Package package) => packages.Add(package);

        public void Complete()
        {
            CompletionTime = DateTime.Now;
        }
    }
}
