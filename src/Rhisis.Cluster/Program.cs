﻿using System;

namespace Rhisis.Cluster
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            using (var server = new ClusterServer())
                server.Start();
        }
    }
}