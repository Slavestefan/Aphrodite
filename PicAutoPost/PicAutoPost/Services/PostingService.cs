using Microsoft.EntityFrameworkCore;
using Slavestefan.Aphrodite.Model;
using Slavestefan.Aphrodite.Web.Helpers;
using System;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Slavestefan.Aphrodite.Web.Services
{
    public class PostingService : IDisposable
    {
        private readonly Bot _bot;
        private readonly PicAutoPostContext _dbContext;
        private readonly RandomService _rng;
        private readonly ILogger<PostingService> _logger;
        private DateTime _lastFire;
        private int _interval;
        private ulong _channelId;
        private CancellationToken _token;
        private Timer _timer;
        private bool InSithMode;

        public bool IsRunning { get; set; } = false;

        public PostingService(Bot bot, PicAutoPostContext dbContext, RandomService rng, ILogger<PostingService> logger)
        {
            _bot = bot;
            _dbContext = dbContext;
            _rng = rng;
            _logger = logger;
        }

        public void Start(ulong channelId, CancellationToken cancellationToken)
        {
            IsRunning = true;
            _channelId = channelId;
            var config = _dbContext.Configurations.First(x => x.ChannelId == channelId);
            _token = cancellationToken;
            double skip = 0;
            if (config.LastPost != null)
            {
                TimeSpan timeSinceLastPost = DateTime.Now - config.LastPost.Value;
                skip = timeSinceLastPost.TotalMilliseconds;
            }

            _logger.LogInformation($"Autoposting started in {channelId} for user {config.UserId}");
            _lastFire = DateTime.Now;
            _interval = (int) Math.Max(GetNextPostingInterval(config) - skip, 0);
            _timer = new Timer(Post, config, _interval, Timeout.Infinite);
        }
        
        public void Stop()
        {
            var config = _dbContext.Configurations.First(x => x.ChannelId == _channelId);
            config.IsRunning = false;
            _dbContext.SaveChanges();
            _logger.LogInformation($"Autoposting started in {config.ChannelId} for user {config.UserId}");
            IsRunning = false;
            _timer?.Dispose();
            _timer = null;
        }

        private void Post(object state)
        {
            if (_token.IsCancellationRequested)
            {
                _timer?.Dispose();
                return;
            }

            var config = (PostConfiguration) state;
            var picCount = _rng.Rng.Next(config.MinPostPerInterval, config.MaxPostPerInterval + 1);
            var pics = _dbContext.Pictures.Include(x => x.User).Where(x => x.User.DiscordId == config.UserId);
            var skips = new int?[picCount];
            for (var i = 0; i < picCount; ++i)
            {
                int skip = _rng.Rng.Next(0, pics.Count());
                while (skips.Contains(skip) && pics.Count() > picCount)
                {
                    skip = _rng.Rng.Next(0, pics.Count());
                }
                skips[i] = skip;
                var task = _bot.SendMessage(string.Empty, config.ChannelId, Converter.ToEmbed(pics.Skip(skip).Take(1).First()));
            }

            config.LastPost = DateTime.Now;
            _lastFire = DateTime.Now;
            _interval = GetNextPostingInterval(config);
            _dbContext.SaveChanges();
            _logger.LogInformation($"Posted {picCount} Pictures in channel {config.ChannelId} for user {config.UserId}. Next post in {_interval} ms.");
            _timer.Change(_interval, Timeout.Infinite);
        }

        private int GetNextPostingInterval(PostConfiguration config)
            => _rng.Rng.Next(config.MinPostingIntervalInMinutes * 1000 * 60, config.MaxPostingIntervalInMinutes * 1000 * 60);

        public TimeSpan GetTimeUntilNextPost()
        {
            return _lastFire + new TimeSpan(0, 0, 0, 0, _interval) - DateTime.Now;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        internal void TurnSithOn()
        {
            
        }
    }

    public struct PostingServiceConfig
    {
        public Uri NormalAvatarUrl { get; }
        public Uri SithAvatarUrl { get; }

        public PostingServiceConfig(string normalAvatarUrl, string sithAvatarUrl)
        {
            NormalAvatarUrl = new Uri(normalAvatarUrl);
            SithAvatarUrl = new Uri(sithAvatarUrl);
        }
    }
}