﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Slavestefan.Aphrodite.Model;

namespace Slavestefan.Aphrodite.Web.Services
{
    public class PostingServiceHost : IDisposable
    {
        private readonly IServiceProvider _services;
        private readonly Dictionary<ulong, (PostingService Service, IServiceScope Scope, CancellationTokenSource Token)> _postingServices = new Dictionary<ulong, (PostingService, IServiceScope, CancellationTokenSource)>();


        public PostingServiceHost(IServiceProvider services)
        {
            _services = services;
        }

        internal void StartupPosting()
        {
            using var scope = _services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<PicAutoPostContext>();
            var configs = dbContext.Configurations.Where(x => x.IsRunning);
            foreach (var config in configs)
            {
                StartPostingService(config);
            }
        }

        internal void StartPostingService(PostConfiguration config)
        {
            (PostingService Service, IServiceScope Scope, CancellationTokenSource TokenSource) tuple;
            if (_postingServices.ContainsKey(config.ChannelId))
            {
                tuple = _postingServices[config.ChannelId];
                if (tuple.Service.IsRunning)
                {
                    return;
                }

                tuple.TokenSource = new CancellationTokenSource();
            }
            else
            {
                tuple.Scope = _services.CreateScope();
                tuple.Service = tuple.Scope.ServiceProvider.GetRequiredService<PostingService>();
                tuple.TokenSource = new CancellationTokenSource();
                _postingServices.Add(config.ChannelId, tuple);
            }

            tuple.Service.Start(config.ChannelId, tuple.TokenSource.Token);
        }

        internal void StopPostingService(ulong channelId)
        {
            if (!_postingServices.ContainsKey(channelId))
            {
                return;
            }

            var (service, scope, tokenSource) = _postingServices[channelId];
            tokenSource.Cancel();
            service.Stop();
            scope.Dispose();
            _postingServices.Remove(channelId);
        }

        internal List<(ulong, bool, TimeSpan)> GetPostingServiceStatus()
        {
            return _postingServices.Select(x => (x.Key, x.Value.Service.IsRunning, x.Value.Service.GetTimeUntilNextPost())).ToList();
        }

        public void Dispose()
        {
            foreach (var kvp in _postingServices)
            {
                (PostingService service, IServiceScope scope, CancellationTokenSource token) = kvp.Value;
                token.Cancel();
                service.Stop();
                scope.Dispose();
            }

            _postingServices.Clear();
        }
    }
}