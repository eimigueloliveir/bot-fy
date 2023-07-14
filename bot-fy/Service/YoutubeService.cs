﻿using bot_fy.Discord.Extensions;
using DSharpPlus.Entities;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Playlists;
using YoutubeExplode.Search;
using YoutubeExplode.Videos;

namespace bot_fy.Service
{
    public class YoutubeService
    {
        private readonly YoutubeClient youtube = new();
        private readonly int MAX_RESULTS_PLAYLIST = 200;

        public async Task<List<string>> GetResultsAsync(string termo, DiscordChannel channel)
        {
            if (!termo.Contains("youtu.be/") && !termo.Contains("youtube.com"))
            {
                return await GetVideoByTermAsync(termo, channel);
            }
            if (termo.Contains("playlist?list"))
            {
                return await GetPlayListVideosAsync(termo, channel);
            }
            return await GetVideosAsync(termo);
        }

        public async Task<List<string>> GetVideoByTermAsync(string termo, DiscordChannel channel)
        {
            await foreach (VideoSearchResult result in youtube.Search.GetVideosAsync(termo))
            {
                await channel.SendNewMusicAsync(result);
                return new List<string>() { result.Id.Value };
            }
            return new List<string>();
        }

        private async Task<List<string>> GetPlayListVideosAsync(string link, DiscordChannel channel)
        {
            Playlist playlist = await youtube.Playlists.GetAsync(link);
            IReadOnlyList<PlaylistVideo> videosSubset = await youtube.Playlists
                .GetVideosAsync(playlist.Id)
                .CollectAsync(MAX_RESULTS_PLAYLIST);
            await channel.SendNewPlaylistAsync(playlist, videosSubset);

            return videosSubset.Select(v => v.Id.Value).ToList();
        }

        private async Task<List<string>> GetVideosAsync(string link)
        {
            var video = await youtube.Videos.GetAsync(link);
            return new List<string> { video.Id };
        }

        public async Task<Video> GetVideoAsync(string video_id)
        {
            return await youtube.Videos.GetAsync(video_id);
        }
    }
}