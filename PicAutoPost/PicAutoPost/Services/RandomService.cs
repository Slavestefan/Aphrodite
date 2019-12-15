using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace PicAutoPost.Services
{
    public class RandomService
    {
        public Random Rng { get; } = new Random();
    }
}